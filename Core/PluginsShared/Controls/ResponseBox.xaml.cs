using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Mnk.Library.Common;
using Mnk.Library.Common.Network;
using Mnk.TBox.Core.PluginsShared.Encoders;

namespace Mnk.TBox.Core.PluginsShared.Controls
{
	/// <summary>
	/// Interaction logic for ResponseBox.xaml
	/// </summary>
	public partial class ResponseBox
	{
		protected ResponseInfo LastResponse = null;
		private readonly KeyValuePair<string, Func<string, string>>[] operations;

		public ResponseBox()
		{
			InitializeComponent();
			operations = new[]
				             {
								 new KeyValuePair<string, Func<string, string>>("", x=>x), 
					             new KeyValuePair<string, Func<string, string>>("Json", x=>new JsonParser().Format(x)), 
					             new KeyValuePair<string, Func<string, string>>("Html", x=>new HtmlParser().Parse(x)), 
					             new KeyValuePair<string, Func<string, string>>("Xml", XmlHelper.Format)
				             };
			Formatters.ItemsSource = new ObservableCollection<string>(operations.Select(x => x.Key));
		}

		public ResponseInfo Value
		{
			get { return LastResponse; }
			set
			{
				if (value == null)
				{
					LastResponse = null;
					Response.Text = string.Empty;
					ResponseHeaders.Text = string.Empty;
				}
				else
				{
					LastResponse = value;
					Response.Text = value.Body;
					ResponseHeaders.Text = value.Headers;
					Formatters_OnSelectionChanged(this, null);
				}
			}
		}

		private void Formatters_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Formatters.SelectedIndex == -1 || LastResponse == null) return;
			var op = operations[Formatters.SelectedIndex].Value;
			ExceptionsHelper.HandleException(
				() => Response.Text = op(LastResponse.Body),
				ex => { Response.Text = ex.Message; }
				);
		}

	}
}
