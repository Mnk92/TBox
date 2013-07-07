using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Core.PluginsShared.Automator;
using Mnk.Library.ScriptEngine;

namespace Solution.Scripts
{
	public class OptimizeSolution : IScript
	{
		private static readonly ILog Log = LogManager.GetLogger<OptimizeSolution>();

		[DirectoryList()]
		public string[] Solutions { get; set; }

        [StringList("..\\Lib", "..\\packages")]
        public string[] PathsToLibs { get; set; }

        [StringList("Content", "None" )]
        public string[] SectionToPreserveNewestNames { get; set; }

        [StringList("AjaxMin")]
        public string[] TasksToRemove { get; set; }

        [Bool(true)]
        public bool DisableBuildEvents { get; set; }

		[Bool(false)]
		public bool DisableTests { get; set; }

		public void Run(IScriptContext context)
		{
            if (!Solutions.Aggregate(false, (current, path) => ProcessSolution(context.PathResolver.Resolve(path)) || current))
            {
                Log.Write("Nothing to optimize");
            }
		}

	    private bool ProcessSolution(string path)
	    {
	        var optimezedCount = 0;
	        foreach (var subdir in new DirectoryInfo(path).GetDirectories())
	        {
	            var projectFile = Path.Combine(path, subdir.Name, subdir.Name + ".csproj");
                if(!File.Exists(projectFile))continue;
                var doc = XDocument.Load(projectFile, LoadOptions.PreserveWhitespace);
                if (!ProccessProject(doc.Root, subdir.FullName)) continue;
	            ++optimezedCount;
	            doc.Save(projectFile);
	        }
	        return optimezedCount > 0;
	    }

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        static extern bool CreateHardLink(
            string lpFileName,
            string lpExistingFileName,
            IntPtr lpSecurityAttributes
        );

        private static IEnumerable<string> GetOutputPaths(XElement root, string rootDirPath)
        {
            return root.Elements()
                       .Where(x => x.Name.LocalName.EqualsIgnoreCase("PropertyGroup"))
                       .Where(x => x.Attribute("Condition")!=null)
                       .Where(x => x.Attribute("Condition").Value.Contains("$(Configuration)|$(Platform)"))
                       .Select(x => GetElement(x, "OutputPath"))
                       .Where(x => x != null)
                       .Select(x => Path.GetFullPath(Path.Combine(rootDirPath, x.Value)));
        }

        private bool ProccessProject(XElement root, string rootDirPath)
	    {
            var outputPaths = GetOutputPaths(root, rootDirPath).Distinct().ToArray();
            var pathToLibs = PathsToLibs.Select(x => Path.GetFullPath(Path.Combine(rootDirPath, x))).ToArray();
            var finded = false;
	        foreach (var s in outputPaths)
	        {
		        if(Directory.Exists(s))Directory.Delete(s, true);
	        }
            foreach (var node in root.Elements()
                .Where(x => x.Name.LocalName.EqualsIgnoreCase("ItemGroup"))
                .SelectMany(x => x.Elements().Where(y => y.Name.LocalName.EqualsIgnoreCase("Reference")))
                )
            {
                finded = ProcessLib(root, rootDirPath, node, pathToLibs, outputPaths) || finded;
            }
            finded = DisableResourcesCopy(root) || finded;
            finded = TasksToRemove.Aggregate(false, (current, task) => RemoveTask(root, task) || current) || finded;
            if (DisableBuildEvents)
            {
                finded = DisableAllBuildEvents(root) || finded;
            }
            return finded;
	    }

	    private static bool ProcessLib(XElement root, string rootDirPath, XElement node, string[] pathToLibs, string[] outputPaths)
	    {
	        var name = node.Attribute("Include").Value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).First();
	        var hintPathEl = GetElement(node, "HintPath");
	        if (hintPathEl == null) return false;
	        var hintPath = Path.GetFullPath(Path.Combine(rootDirPath, hintPathEl.Value));
	        if (!File.Exists(hintPath))
	        {
				Log.Write("Can't find path: " + hintPath, rootDirPath);
	            return false;
	        }
	        if (!pathToLibs.Any(x => hintPath.StartsWith(x, StringComparison.OrdinalIgnoreCase))) return false;
	        var target = node.Elements()
	                         .FirstOrDefault(x => x.Name.LocalName.EqualsIgnoreCase("Private"));
	        var canRemove = false;
	        foreach (var s in outputPaths)
	        {
	            var targetPath = Path.Combine(s, name + Path.GetExtension(hintPath));
	            if (!Directory.Exists(s)) Directory.CreateDirectory(s);
	            if (File.Exists(targetPath)) File.Delete(targetPath);
	            CreateHardLink(targetPath, hintPath, IntPtr.Zero);
	            canRemove = true;
	        }
	        if (!canRemove) return false;
	        if (target == null)
	        {
	            node.Add(target = new XElement(root.GetDefaultNamespace() + "Private"));
	        }
			if (target.Value.EqualsIgnoreCase("True"))
			{
				target.Value = "False";
				return true;
			}
	        return false;
	    }

	    private static XElement GetElement(XElement node, string name)
	    {
	        return node.Elements().FirstOrDefault(y => y.Name.LocalName.EqualsIgnoreCase(name));
	    }

        private bool DisableResourcesCopy(XElement root)
        {
            var finded = false;
            foreach (var node in root.Elements()
                .Where(x => string.Equals(x.Name.LocalName, "ItemGroup"))
                .SelectMany(x => x.Elements().Where(y => SectionToPreserveNewestNames.Contains(y.Name.LocalName)))
                )
            {
                var target = GetElement(node, "CopyToOutputDirectory");
                if (target == null) continue;
	            if (!target.Value.EqualsIgnoreCase("Always")) continue;
	            target.Value = "PreserveNewest";
	            finded = true;
            }
            return finded;
        }

        private static bool RemoveTask(XElement root, string task)
        {
            var finded = false;
            foreach (var node in root.Elements()
                .Where(x => string.Equals(x.Name.LocalName, "Target")))
            {
                var target = node.Elements()
					.FirstOrDefault(x => x.Name.LocalName.EqualsIgnoreCase(task));
                if (target == null) continue;
                target.Remove();
                finded = true;
            }
            return finded;
        }

        private static bool DisableAllBuildEvents(XElement root)
        {
            var finded = false;
            foreach (var node in root.Elements()
                .Where(x => string.Equals(x.Name.LocalName, "PropertyGroup")))
            {
                var target = node.Elements().FirstOrDefault(x => x.Name.LocalName.EndsWith("BuildEvent"));
                if (target == null) continue;
                target.Remove();
                finded = true;
            }
            return finded;
        }

	}
}
