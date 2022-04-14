using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mnk.TBox.Plugins.LocalizationTool.Code.Translate
{
    class Translator : ITranslator
    {
        public async Task<string> Translate(string text, string locFrom, string locTo)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            using var cl = new HttpClient();
            var data = await cl.GetAsync(BuildUri(text, locFrom, locTo));
            return GetResult(await data.Content.ReadAsByteArrayAsync());
        }

        private static string BuildUri(string text, string locFrom, string locTo)
        {
            return
                $"http://translate.google.com/translate_a/t?client=j&text={WebUtility.UrlEncode(text)}&sl={locFrom}&oe=UTF-8&ie=UTF-8&tl={locTo}";
        }

        class Translation
        {
            public class Sentences
            {
                public string trans { get; set; }
            }
            public IList<Sentences> sentences { get; set; }
        }

        private static string GetResult(byte[] response)
        {
            using var s = new MemoryStream(response);
            var sentences = JsonSerializer.Deserialize<Translation>(s)?.sentences;
            if (sentences == null || sentences.Count == 0) return string.Empty;
            var sb = new StringBuilder();
            foreach (var line in sentences)
            {
                sb.Append(line.trans);
            }
            return sb.ToString();
        }
    }
}
