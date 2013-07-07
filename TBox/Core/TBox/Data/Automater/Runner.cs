using System;
using Mnk.TBox.Core.PluginsShared.Automator;
using Solution.Scripts;

namespace Solution
{
    static class Runner
    {
        [STAThread]
        public static void Main()
        {
            new PutObjects()
                {
                    FilesMasks = new[] { "*.exe", "*.pdb" },
                    PackageMask = "Package*.zip",
                    PathToDirectoryWithPackage = "D:/os",
                    RemoveAfterUnpack = false,
                    TargetPathes = new[] { "D:/os" }
                }.Run(new ScriptContext());
        }
    }
}
