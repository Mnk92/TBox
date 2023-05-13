using System.Reflection;

namespace Mnk.TBox.Plugins.Market.Service
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            // Fix to load dependencies correctly
            AppDomain.CurrentDomain.AssemblyResolve += (s, a) =>
            {
                return (from dir in new[] { "Libraries", "Localization" }
                        select Path.GetFullPath(
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", dir, new AssemblyName(a.Name).Name + ".dll"))
                    into assemblyPath
                        where File.Exists(assemblyPath)
                        select Assembly.LoadFrom(assemblyPath)).FirstOrDefault();
            };
            App.Run(args);
        }
    }
}
