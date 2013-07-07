using System;
using System.IO;

namespace Mnk.TBox.Core.Contracts
{
    public static class Folders
    {
        public readonly static string LocalFolder = AppDomain.CurrentDomain.BaseDirectory;
        public readonly static string UserRootFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TBox");
        public readonly static string UserLogsFolder = Path.Combine(UserRootFolder, "Logs");
        public readonly static string UserToolsFolder = Path.Combine(UserRootFolder, "Tools");

        static Folders()
        {
            CreateIfNotExist(UserRootFolder);
            CreateIfNotExist(UserLogsFolder);
            CreateIfNotExist(UserToolsFolder);
        }

        private static void CreateIfNotExist(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }
    }
}
