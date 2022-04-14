using Mnk.Rat.Finders.Scanner;

namespace Mnk.Rat.Finders.Parsers
{
    public interface IParser
    {
        bool Parse(AddInfo info);
    }
}
