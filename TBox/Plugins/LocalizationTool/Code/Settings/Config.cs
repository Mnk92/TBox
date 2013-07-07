using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.Common.UI.ModelsContainers;
using Mnk.TBox.Core.Contracts;
using Mnk.Library.WpfControls.Dialogs.StateSaver;

namespace Mnk.TBox.Plugins.LocalizationTool.Code.Settings
{
    [Serializable]
    public class Config : ICloneable, IConfigWithDialogStates
    {
        public int SelectedLanguageFrom { get; set; }
        public string Source { get; set; }
        public int SelectedOutputFormat { get; set; }
        public int SelectedInputFormat { get; set; }
        public int SelectedTemplate { get; set; }
        public CheckableDataCollection<CheckableData> Formats { get; set; }
        public ObservableCollection<Data> Languages { get; set; }
        public ObservableCollection<Data<string>> Templates { get; set; }
        public IDictionary<string, DialogState> States { get; set; }

        public Config()
        {
            Source = string.Empty;
            SelectedInputFormat = 0;
            SelectedOutputFormat = 3;
            SelectedLanguageFrom = 0;
            SelectedTemplate = 1;
            Formats = new CheckableDataCollection<CheckableData>(new[]
            {
                new CheckableData{Key = "$(key)\\t$(value)"}, 
                new CheckableData{Key = "$(key)=\"$(value)\""},
                new CheckableData{Key = "\"$(key)\":\"$(value)\","},
                new CheckableData{Key = "translations[\"$(key)\"] = \"$(value)\";"}
                
            });
            Languages = new ObservableCollection<Data>(new[]
            {
                new Data{Key = "en-US"}, 
                new Data{Key = "de"},
                new Data{Key = "es"},
                new Data{Key = "fr"},
                new Data{Key = "it"},
                new Data{Key = "ja"},
                new Data{Key = "ko"},
                new Data{Key = "th"},
                new Data{Key = "tr"},
                new Data{Key = "zh-cn"},
                new Data{Key = "ru"}
            });
            Templates = new ObservableCollection<Data<string>>(new []
            {
                new Data<string>
                {
                    Key = "JSON",
                    Value = @"
{
    'zh': [ 
$(lang: zh-cn)$
'':''],
    'de'[
$(lang: de)$
'':''],
    'tr':[
$(lang: tr)$
'':''],
    'ru':[
$(lang: ru)$
'':''],
    'default':[
$(lang: default)$
'':'']
}"}, 
                new Data<string>
                {
                    Key = "JavaScript",
                    Value = @"
switch (locale) {
    case 'zh':
        $(lang: zh-cn)$
        break;
    case 'de':
        $(lang: de)$
        break;
    case 'tr':
        $(lang: tr)$
        break;
    case 'ru':
        $(lang: ru)$
        break;
    default:
        $(lang: default)$
        break;
}"}, 
                  new Data<string>
                {
                    Key = "Ini",
                    Value = @"
[zh]
$(lang: zh-cn)$

[de]
$(lang: de)$

[tr]
$(lang: tr)$

[ru]
$(lang: ru)$

[default]
 $(lang: default)$
"}, 
            });
            States = new Dictionary<string, DialogState>();
        }

        public object Clone()
        {
            return new Config
            {
                Formats = Formats.Clone(),
                Languages = Languages.Clone(),
                Templates = Templates.Clone(),
                SelectedTemplate = SelectedTemplate,
                SelectedInputFormat = SelectedInputFormat,
                SelectedOutputFormat = SelectedOutputFormat,
                Source = Source,
                SelectedLanguageFrom = SelectedLanguageFrom,
                States = States
            };
        }

    }
}
