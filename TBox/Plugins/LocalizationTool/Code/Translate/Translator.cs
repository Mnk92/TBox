using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Mnk.TBox.Plugins.LocalizationTool.Code.Translate
{
    class Translator : ITranstlator
    {
        public string Translate(string text, string locFrom, string locTo)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            using (var cl = new WebClient())
            {
                var data = cl.DownloadData(BuildUri(text, locFrom, locTo));
                return GetResult(data);
            }
        }

        private static string BuildUri(string text, string locFrom, string locTo)
        {
            return
                string.Format("http://translate.google.com/translate_a/t?client=j&text={0}&sl={1}&oe=UTF-8&ie=UTF-8&tl={2}",
                    HttpUtility.UrlEncode(text), locFrom, locTo
                    );
        }

        class Translation
        {
            public class Sentences
            {
                public string trans { get; set; }
            }
            public IList<Sentences> sentences { get; set; }
        }

        public string GetResult(byte[] response)
        {
            using (var s = new MemoryStream(response))
            {
                var sentences = JsonSerializer.Deserialize<Translation>(s).sentences;
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
}
