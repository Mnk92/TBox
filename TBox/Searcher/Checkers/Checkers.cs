using System;
using Mnk.Rat.Settings;

namespace Mnk.Rat.Checkers
{
    public interface IFileChecker
    {
        bool Check(string text);
    }

    public interface IName
    {
        bool Compare(string name);
        bool ContainMe(string name);
        int Length();
    }

    public sealed class NameCaseComparer : IName
    {
        private readonly string name;
        public NameCaseComparer(string name) { this.name = name; }
        public bool Compare(string val) { return string.Equals(name, val, StringComparison.Ordinal); }
        public bool ContainMe(string val) { return val.IndexOf(name, StringComparison.Ordinal) >= 0; }
        public int Length() { return name.Length; }
    }

    public sealed class NameNoCaseComparer : IName
    {
        private readonly string name;
        public NameNoCaseComparer(string name) { this.name = name; }
        public bool Compare(string val) { return string.Equals(name, val, StringComparison.OrdinalIgnoreCase); }
        public bool ContainMe(string val) { return val.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0; }
        public int Length() { return name.Length; }
    }

    public class FileCheckByNameExactMatch : IFileChecker
    {
        private readonly IName name;
        public FileCheckByNameExactMatch(IName name) { this.name = name; }
        public bool Check(string text)
        {
            return name.Compare(text);
        }
    }

    public class FileCheckByNameBeginFrom : IFileChecker
    {
        private readonly IName name;
        public FileCheckByNameBeginFrom(IName name) { this.name = name; }
        public bool Check(string text)
        {
            if (name.Length() > text.Length)
                return false;
            return name.Compare(text.Substring(0, name.Length()));
        }
    }
    public class FileCheckByNameEnds : IFileChecker
    {
        private readonly IName name;
        public FileCheckByNameEnds(IName name) { this.name = name; }
        public bool Check(string text)
        {
            if (name.Length() > text.Length)
                return false;
            return name.Compare(text.Substring(text.Length - name.Length(), name.Length()));
        }
    }

    public class FileCheckByNameContains : IFileChecker
    {
        private readonly IName name;
        public FileCheckByNameContains(IName name) { this.name = name; }
        public bool Check(string text)
        {
            return name.ContainMe(text);
        }
    }

    public static class FileCheckerFactory
    {
        public static IName GetNameComparer(string name, bool needMathCase)
        {
            if (needMathCase)
                return new NameCaseComparer(name);
            return new NameNoCaseComparer(name);
        }

        public static IFileChecker Create(CompareType compareType, bool needMathCase, string name)
        {
            switch (compareType)
            {
                case CompareType.ExactMath:
                    return new FileCheckByNameExactMatch(GetNameComparer(name, needMathCase));
                case CompareType.BeginWith:
                    return new FileCheckByNameBeginFrom(GetNameComparer(name, needMathCase));
                case CompareType.EndWith:
                    return new FileCheckByNameEnds(GetNameComparer(name, needMathCase));
                case CompareType.Contain:
                    return new FileCheckByNameContains(GetNameComparer(name, needMathCase));
            }
            throw new ArgumentException("Searcher. Unknown file checker: " + compareType);
        }
    }
}
