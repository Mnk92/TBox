using System;
using System.Linq;
using System.Windows;
using Mnk.Library.WpfControls.Tools;

namespace Mnk.TBox.Plugins.PasswordsStorage.Code
{
    class PasswordGenerator : IPasswordGenerator
    {
        private static readonly char[] NonAlphaSymbols = { '.', '-', '_', ':', '!', '?', '+', '*', '^', '&' };
        private static readonly char[] LowerCaseSymbols = Enumerable.Range('a', 'z' - 'a').Select(x => (char)x).ToArray();
        private static readonly char[] Symbols = Enumerable.Range('0', '9' - '0').Select(x => (char)x).ToArray().Concat(
                                                LowerCaseSymbols.Concat(
                                                    LowerCaseSymbols.Select(char.ToUpper))).ToArray();

        public string Generate(int passwordLength, int passwordNonAlphaCharacters)
        {
            var random = new Random();
            var password = Enumerable.Range(0, passwordLength)
                .Select(_ => Symbols[random.Next(0, Symbols.Length - 1)])
                .ToArray();
            foreach (var id in Enumerable.Range(0, passwordLength)
                         .OrderBy(_ => random.Next())
                         .Take(Math.Min(passwordNonAlphaCharacters, passwordLength)))
            {
                password[id] = NonAlphaSymbols[random.Next(0, NonAlphaSymbols.Length - 1)];
            }

            var passwordString = new string(password);
            Clipboard.SetText(passwordString);
            return passwordString.EncryptPassword();
        }
    }
}
