using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.MT;
using Mnk.Library.ScriptEngine;
using Mnk.Library.ScriptEngine.Core;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.Library.WpfControls.Dialogs.StateSaver;
using Mnk.Library.WpfControls.Icons;
using Mnk.Library.WpfControls.Menu;
using Mnk.Library.WpfSyntaxHighlighter;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Core.PluginsShared.Automator;
using Mnk.TBox.Core.PluginsShared.ScriptEngine;
using Mnk.TBox.Locales.Localization.Plugins.Automator;
using Mnk.TBox.Plugins.Automator.Code;
using Mnk.TBox.Plugins.Automator.Code.Settings;

namespace Mnk.TBox.Plugins.Automator
{
    [PluginInfo(typeof(AutomatorLang), 12, PluginGroup.Development)]
    public sealed class Automater : ConfigurablePlugin<Settings, Config>
    {
        private readonly Lazy<ScriptRunner> scriptRunner;
        private readonly LazyDialog<EditorDialog> editorDialog;
        private readonly LazyDialog<ScriptsRunner> runnerDialog;
        private static readonly ILog Log = LogManager.GetLogger<Automater>();
        public Automater()
        {
            scriptRunner = new Lazy<ScriptRunner>(() => new ScriptRunner(Context.PathResolver));
            Dialogs.Add(editorDialog = new LazyDialog<EditorDialog>(CreateEditor));
            Dialogs.Add(runnerDialog = new LazyDialog<ScriptsRunner>(CreateRunner));
        }

        private EditorDialog CreateEditor()
        {
            return new EditorDialog { Context = Context, Icon = Icon.ToImageSource() };
        }

        private ScriptsRunner CreateRunner()
        {
            return new ScriptsRunner(Context.PathResolver) { Context = Context, Icon = Icon.ToImageSource() };
        }

        public override void Init(IPluginContext context)
        {
            base.Init(context);
            context.AddTypeToWarmingUp(typeof(SyntaxHighlighter));
        }

        public override void OnRebuildMenu()
        {
            base.OnRebuildMenu();
            Menu = FillMenu(Config.Profiles.Where(x => x.Operations.Count > 0).ToArray())
                .Concat(new[]
                    {
                        new USeparator(),
                        new UMenuItem{Header = AutomatorLang.Editor, OnClick = ShowIde}
                    })
                .ToArray();
        }

        private IEnumerable<UMenuItem> FillMenu(IList<Profile> ops)
        {
            if (ops.Count == 1)
            {
                var p = ops.First();
                return new[]
                {
                    new UMenuItem{Header = p.Key, IsEnabled = false},
                }
                .Concat(FillOperationMenu(p));
            }

            return ops.Select(p => new UMenuItem
            {
                Header = p.Key,
                Items = FillOperationMenu(p)
            });
        }

        private UMenuItem[] FillOperationMenu(Profile p)
        {
            return p.Operations.Select(o => new UMenuItem
            {
                Header = o.Key,
                OnClick = x => DoWork(o, x)
            })
                .Concat(new[]
                {
                    new USeparator(),
                    new UMenuItem
                    {
                        Header = AutomatorLang.RunAll,
                        OnClick = x=>RunAll(p)
                    }
                })
                .ToArray();
        }

        private void RunAll(Profile profile)
        {
            DialogsCache.ShowProgress(u => RunAll(u, profile.Operations), AutomatorLang.RunScript + ": " + profile.Key, null, false, true, Icon.ToImageSource());
        }

        private void RunAll(IUpdater u, IList<MultiFileOperation> operations)
        {
            var i = 0;
            var count = operations.Sum(x => x.Pathes.CheckedItems.Count());
            var folder = Context.DataProvider.ReadOnlyDataPath;
            foreach (var op in operations)
            {
                var r = new ScriptRunner(Context.PathResolver);
                foreach (var path in op.Pathes.CheckedItems)
                {
                    if (u.UserPressClose) return;
                    u.Update(path.Key, i++ / (float)count);
                    var result = false;
                    var key = path.Key;
                    var p = op.Parameters;
                    Execute(() => r.Run(Path.Combine(folder, key), p, a => Context.DoSync(a), new NullUpdater()), out result);
                    if (!result) return;
                }
            }
        }

        private static void Execute(Action action, out bool result)
        {
            result = false;
            try
            {
                action();
                result = true;
            }
            catch (CompilerExceptions cex)
            {
                Log.Write(cex.ToString());
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Error running script");
            }
        }

        private void ShowIde(object o)
        {
            editorDialog.Do(Context.DoSync, x => x.ShowDialog(GetPaths(), scriptRunner.Value), Config.States);
        }

        private void DoWork(MultiFileOperation operation, object context)
        {
            if (context is NonUserRunContext)
            {
                RunAll(new NullUpdater(), new[] { operation });
            }
            else
            {
                runnerDialog.Do(Context.DoSync, x => x.ShowDialog(operation, scriptRunner.Value, null), Config.States);
            }
        }

        protected override Settings CreateSettings()
        {
            var s = base.CreateSettings();
            s.FilePathsGetter = GetPaths;
            return s;
        }

        private IList<string> GetPaths()
        {
            var dir = new DirectoryInfo(Context.DataProvider.ReadOnlyDataPath);
            if (!dir.Exists) return new string[0];
            var length = dir.FullName.Length + 1;
            return dir
                .EnumerateFiles("*.cs", SearchOption.AllDirectories)
                .Select(x => x.FullName.Substring(length))
                .ToArray();
        }
    }
}
