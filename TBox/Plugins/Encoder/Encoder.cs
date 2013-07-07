using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows;
using Mnk.Library.Common;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.Encoder;
using Mnk.TBox.Core.PluginsShared.Encoders;
using Mnk.Library.WpfControls.Dialogs.StateSaver;
using Mnk.Library.WpfSyntaxHighlighter;
using Mnk.Library.WpfWinForms;
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

		protected  override Dialog CreateDialog()
		{
			var dialog = base.CreateDialog();
			dialog.Init(operations);
			return dialog;
		}

		private static IEnumerable<Operation> GetOperations()
		{
			yield return new Operation { Header = EncoderLang.EncodeCstring, Work = x => CommonEncoders.EncodeString(x) };
			yield return new Operation { Header = EncoderLang.DecodeCstring, Work = x => CommonEncoders.DecodeString(x) };
			yield return new Operation { Header = EncoderLang.EncodeUri, Work = x => HttpUtility.UrlEncode(x) };
			yield return new Operation { Header = EncoderLang.DecodeUri, Work = x => HttpUtility.UrlDecode(x) };
			yield return new Operation { Header = EncoderLang.EncodeHtml, Work = x => HttpUtility.HtmlEncode(x)};
			yield return new Operation { Header = EncoderLang.DecodeHtml, Work = x => HttpUtility.HtmlDecode(x) };
			yield return new Operation { Header = EncoderLang.EncodeToBase64, Work = x => Base64Encode.EncodeTo64(x) };
			yield return new Operation { Header = EncoderLang.DecodeToBase64, Work = x => Base64Encode.DecodeFrom64(x) };
			yield return new Operation { Header = EncoderLang.FormatXml, Work = x => XmlHelper.Format(x), Format = "xml" };
			yield return new Operation { Header = EncoderLang.FormatFqlSimple, Work = x => FqlParser.ParseSimple(x), Format = "mssql" };
			yield return new Operation { Header = EncoderLang.FormatFqlAdvanced, Work = x => FqlParser.ParseWithSubItems(x), Format = "mssql" };
			yield return new Operation { Header = EncoderLang.FormatSQL, Work = x => new SqlParser().Parse(x), Format = "mssql" };
			yield return new Operation { Header = EncoderLang.FormatHtml, Work = x => new HtmlParser().Parse(x), Format = "html" };
			yield return new Operation { Header = EncoderLang.FormatJSON, Work = x => new JsonParser().Format(x), Format = "js" };
			yield return new Operation { Header = EncoderLang.FormatClikeCode, Work = x => new CppCodeFormatter().Format(x), Format = "js" };
			yield return new Operation { Header = EncoderLang.MinimizeToLine, Work = x => CommonEncoders.Minimize(x) };
		}

		private IEnumerable<UMenuItem> CreateMenu(IEnumerable<Operation> ops)
		{
			yield return new UMenuItem 
			{ 
				Header = EncoderLang.Show + "...", 
				OnClick = o => Dialog.Do(Context.DoSync, d=>d.ShowAndActivate(), Config.States)
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
