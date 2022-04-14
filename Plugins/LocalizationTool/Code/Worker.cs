using System.Collections.Generic;
using System.Linq;
using Mnk.Library.Common.MT;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Plugins.LocalizationTool.Code.Parsers;
using Mnk.TBox.Plugins.LocalizationTool.Code.Translate;

namespace Mnk.TBox.Plugins.LocalizationTool.Code
{
    class Worker
    {
        private readonly ITranslator translator = new Translator();
        private readonly ISourceParser sourceParser = new SourceParser();
        private readonly ITemplatesParser templatesParser = new TemplatesParser();

        public string Translate(string translateFrom, string[] languages, string inputFormat, string outputFormat, string template, string value, IUpdater u)
        {
            var source = sourceParser.Parse(new KeyValueParser(inputFormat), value);
            var all = new Dictionary<string, IDictionary<string, string>>();
            var i = 0;
            foreach (var locale in languages.Where(x => !x.EqualsIgnoreCase(translateFrom)))
            {
                if (u.UserPressClose) return string.Empty;
                var translations = source.ToDictionary(
                    x => x.Key, y => translator.Translate(y.Value, translateFrom, locale).Result);
                u.Update(locale, ++i / (float)languages.Length);
                all[locale] = translations;
            }
            return templatesParser.Save(template ?? string.Empty, new KeyValueParser(outputFormat), source, all);
        }
    }
}
