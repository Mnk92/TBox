using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mnk.Library.Common.SaveLoad;
using Mnk.Library.ScriptEngine.Core;

namespace Mnk.TBox.Tests.Common
{
    class ScriptsFixture
    {
        static ScriptsFixture()
        {
            CompilerCore.Serializer = new ConfigurationSerializer<IDictionary<string, IList<string>>>(Path.GetTempFileName());
            foreach (var path in Directory.GetFiles(Environment.CurrentDirectory, "*.dll"))
            {
                Assembly.LoadFile(path);
            }
        }
    }
}
