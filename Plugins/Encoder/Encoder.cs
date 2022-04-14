using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using Mnk.Library.Common;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.Encoder;
using Mnk.TBox.Core.PluginsShared.Encoders;
using Mnk.Library.WpfControls.Dialogs.StateSaver;
using Mnk.Library.WpfControls.Menu;
using Mnk.Library.WpfSyntaxHighlighter;
using Mnk.TBox.Plugins.Encoder.Code;
using Mnk.TBox.Plugins.Encoder.Components;

namespace Mnk.TBox.Plugins.Encoder
{
    [PluginInfo(typeof(EncoderLang), 144, PluginGroup.Development)]
    public sealed class Encoder : SingleDialogPlugin<Config, Dialog>
    {
        private readonly Operation[] operations;
        public Encoder() : base(EncoderLang.Show)
        {
            operations = GetOperations().ToArray();
            Menu = CreateMenu(operations).ToArray();
        }

        protected override Dialog CreateDialog()
        {
            var dialog = base.CreateDialog();
            dialog.Init(operations);
            return dialog;
        }

        private static IEnumerable<Operation> GetOperations()
        {
            yield return new Operation { Header = EncoderLang.EncodeCstring, Work = CommonEncoders.EncodeString };
            yield return new Operation { Header = EncoderLang.DecodeCstring, Work = CommonEncoders.DecodeString };
            yield return new Operation { Header = EncoderLang.EncodeUri, Work = WebUtility.UrlEncode };
            yield return new Operation { Header = EncoderLang.DecodeUri, Work = WebUtility.UrlDecode };
            yield return new Operation { Header = EncoderLang.EncodeHtml, Work = WebUtility.HtmlEncode };
            yield return new Operation { Header = EncoderLang.DecodeHtml, Work = WebUtility.HtmlDecode };
            yield return new Operation { Header = EncoderLang.EncodeToBase64, Work = Base64Encode.EncodeTo64 };
            yield return new Operation { Header = EncoderLang.DecodeToBase64, Work = Base64Encode.DecodeFrom64 };
            yield return new Operation { Header = EncoderLang.FormatXml, Work = XmlHelper.Format, Format = "xml" };
            yield return new Operation { Header = EncoderLang.FormatFqlSimple, Work = FqlParser.ParseSimple, Format = "mssql" };
            yield return new Operation { Header = EncoderLang.FormatFqlAdvanced, Work = FqlParser.ParseWithSubItems, Format = "mssql" };
            yield return new Operation { Header = EncoderLang.FormatSQL, Work = x => new SqlParser().Parse(x), Format = "mssql" };
            yield return new Operation { Header = EncoderLang.FormatHtml, Work = x => new HtmlParser().Parse(x), Format = "html" };
            yield return new Operation { Header = EncoderLang.FormatJSON, Work = x => new JsonParser().Format(x), Format = "js" };
            yield return new Operation { Header = EncoderLang.FormatClikeCode, Work = x => new CppCodeFormatter().Format(x), Format = "js" };
            yield return new Operation { Header = EncoderLang.MinimizeToLine, Work = CommonEncoders.Minimize };
        }

        private IEnumerable<UMenuItem> CreateMenu(IEnumerable<Operation> ops)
        {
            yield return new UMenuItem
            {
                Header = EncoderLang.Show + "...",
                OnClick = o => Dialog.Do(Context.DoSync, d => d.ShowAndActivate(), Config.States)
            };
            foreach (var o in ops)
            {
                var tmp = o;
                var header = o.Header;
                yield return new UMenuItem { Header = o.Header + "...", OnClick = x => Work(tmp, header) };
            }
        }

        private void Work(Operation o, string title)
        {
            Dialog.Do(Context.DoSync,
                d => d.ShowDialog(EncoderLang.PluginName,
                    Clipboard.ContainsText() ? Clipboard.GetText() : string.Empty,
                    title, o.Format),
                    Config.States
                    );
        }

        public override void Init(IPluginContext context)
        {
            base.Init(context);
            context.AddTypeToWarmingUp(typeof(SyntaxHighlighter));
        }
    }
}
