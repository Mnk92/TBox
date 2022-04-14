using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mnk.TBox.Core.PluginsShared.Automator;
using Mnk.Library.ScriptEngine;

namespace Solution.Scripts
{
	public sealed class TestParams : IScript 
	{
		[Bool(false)]
		public bool BoolParam{get;set;}

		[String("test")]
		public string StringParam{get;set;}

		[Guid("fe1d4f8e-1764-4d49-9d48-759bffceeedd")]
		public Guid GuidParam{get;set;}

		[File("d:\text.txt")]
		public string FileParam{get;set;}

		[Directory("d:\test")]
		public string DirectoryParam{get;set;}

		[Integer(Min=0,Max=10,Value=3)]
		public int IntParam{get;set;}

		[Double(Min=-10,Max=10,Value=0.5)]
		public double RealParam{get;set;}

		[StringList( "1", "2", "3")]
		public string[] StringListParam{get;set;}

		[GuidList("C97F361A-FE72-4532-813B-F54BDF922770")]
		public IList<Guid> GuidListParam{get;set;}

		[FileList("f1", "f2", "f3")]
		public IList<string> FileListParam{get;set;}

		[DirectoryList("d1", "d2", "d3")]
		public IList<string> FolderListParam{get;set;}

		[IntegerList(Min=0,Max=10,Values=new[]{1,2,3,4,5,6,7})]
		public IList<int> IntListParam{get;set;}

		[DoubleList(Min=-100, Max=100, Values=new double[]{0, -1, 10})]
		public IList<double> RealListParam{get;set;}

		[StringDictionary("key", "value")]
		public IDictionary<string,string> StringDictionaryParam { get; set; }

		[GuidDictionary("Test", "C97F361A-FE72-4532-813B-F54BDF922770")]
		public IDictionary<string, Guid> GuidDictionaryParam { get; set; }

		[FileDictionary( "file", "D:/test.txt", "file2", "D:/test2.txt")]
		public IDictionary<string, string> FileDictionaryParam { get; set; }

		[DirectoryDictionary()]
		public IDictionary<string, string> FolderDictionaryParam { get; set; }

		[IntegerDictionary(Min=0, Max=10)]
		public IDictionary<string, int> IntDictionaryParam { get; set; }

		[DoubleDictionary(Min=-100, Max=100)]
		public IDictionary<string, double> RealDictionaryParam { get; set; }

		public void Run(IScriptContext context)
		{
			MessageBox.Show(StringParam, "Hello world", MessageBoxButtons.OK, MessageBoxIcon.Information);
			MessageBox.Show(FileParam, "Hello world", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
