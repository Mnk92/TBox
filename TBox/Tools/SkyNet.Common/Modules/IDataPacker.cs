using System.IO;

namespace Mnk.TBox.Tools.SkyNet.Common.Modules
{
    public interface IDataPacker
    {
        string Unpack(Stream stream);
    }
}
