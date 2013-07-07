using System;
using System.Collections.Generic;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Tools;

namespace Mnk.Rat.Finders
{
    sealed class UnicList
    {
        private static readonly ILog Log = LogManager.GetLogger<UnicList>();
        private readonly List<string> data = new List<string>();

        public string[] ToArray()
        {
            return data.ToArray();
        }

        private int Find(string value)
        {
            return data.IndexOf(value);
        }

        public int Add(string value)
        {
            var id = Find(value);
            if (id >= 0)
            {
                return id;
            }
            data.Add(value);
            return data.Count - 1;
        }

        public bool Save(string fileName)
        {
            try
            {
                data.Save(fileName);
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Can't save file: " + fileName);
                return false;
            }
            return true;
        }
    }
}
