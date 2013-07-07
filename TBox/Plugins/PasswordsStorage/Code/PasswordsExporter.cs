using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using Mnk.Library.WpfControls.Tools;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Plugins.PasswordsStorage.Code.Settings;
using ServiceStack.Text;

namespace Mnk.TBox.Plugins.PasswordsStorage.Code
{
    class PasswordsExporter : IPasswordsExporter
    {
        private const string DefaultFileName = "backup.json";
        public void Import(IConfigManager<Config> cm)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                FileName = DefaultFileName
            };
            if (dialog.ShowDialog() != true) return;
            DoImport(cm, dialog.FileName);
        }

        public void Export(IConfigManager<Config> cm)
        {
            var dialog = new SaveFileDialog
            {
                FileName = DefaultFileName
            };
            if (dialog.ShowDialog() != true) return;
            DoExport(cm, dialog.FileName);
        }

        private static void DoImport(IConfigManager<Config> cm, string path)
        {
            Dictionary<string, List<LoginInfo>> profiles;
            using (var f = File.OpenRead(path))
            {
                 profiles = JsonSerializer.DeserializeFromStream<Dictionary<string, List<LoginInfo>>>(f);
            }

            var actual = cm.Config.Profiles;
            foreach (var profile in profiles)
            {
                var activeProfile = actual.FirstOrDefault(x => string.Equals(x.Key, profile.Key));
                if (activeProfile == null)
                {
                    actual.Add(activeProfile = new Profile { Key = profile.Key });
                }
                foreach (var line in profile.Value)
                {
                    var activeLine = activeProfile.LoginInfos.FirstOrDefault(x => string.Equals(x.Key, line.Key));
                    if (activeLine == null)
                    {
                        activeProfile.LoginInfos.Add(activeLine = new LoginInfo {Key = line.Key});
                    }
                    activeLine.Login = line.Login;
                    activeLine.Comment = line.Comment;
                    activeLine.Password = line.Password.EncryptPassword();
                }
            }
        }

        private static void DoExport(IConfigManager<Config> cm, string path)
        {
            var profiles = new Dictionary<string, List<LoginInfo>>();
            var actual = cm.Config.Profiles;
            foreach (var profile in actual)
            {
                var activeProfile = new List<LoginInfo>();
                profiles.Add(profile.Key, activeProfile);
                foreach (var line in profile.LoginInfos)
                {
                    activeProfile.Add(new LoginInfo
                    {
                        Key = line.Key,
                        Login = line.Login,
                        Comment = line.Comment,
                        Password = line.Password.DecryptPassword()
                    });
                }
            }
            using (var f = File.OpenWrite(path))
            {
                JsonSerializer.SerializeToStream(profiles, f);
            }
        }

    }
}