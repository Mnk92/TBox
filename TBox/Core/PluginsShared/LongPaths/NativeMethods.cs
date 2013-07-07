using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Mnk.TBox.Core.PluginsShared.LongPaths
{
    static class NativeMethods
    {
        [Flags]
        public enum EFileAccess : uint
        {
            GenericRead = 0x80000000,
            GenericWrite = 0x40000000,
            GenericExecute = 0x20000000,
            GenericAll = 0x10000000,
        }
        [Flags]
        public enum EFileShare : uint
        {
            None = 0x00000000,
            Read = 0x00000001,
            Write = 0x00000002,
            Delete = 0x00000004,
        }
        public enum ECreationDisposition : uint
        {
            New = 1,
            CreateAlways = 2,
            OpenExisting = 3,
            OpenAlways = 4,
            TruncateExisting = 5,
        }
        [Flags]
        public enum EFileAttributes : uint
        {
            Readonly = 0x00000001,
            Hidden = 0x00000002,
            System = 0x00000004,
            Directory = 0x00000010,
            Archive = 0x00000020,
            Device = 0x00000040,
            Normal = 0x00000080,
            Temporary = 0x00000100,
            SparseFile = 0x00000200,
            ReparsePoint = 0x00000400,
            Compressed = 0x00000800,
            Offline = 0x00001000,
            NotContentIndexed = 0x00002000,
            Encrypted = 0x00004000,
            WriteThrough = 0x80000000,
            Overlapped = 0x40000000,
            NoBuffering = 0x20000000,
            RandomAccess = 0x10000000,
            SequentialScan = 0x08000000,
            DeleteOnClose = 0x04000000,
            BackupSemantics = 0x02000000,
            PosixSemantics = 0x01000000,
            OpenReparsePoint = 0x00200000,
            OpenNoRecall = 0x00100000,
            FirstPipeInstance = 0x00080000
        }

        public static EFileShare ToNative(this FileShare share)
        {
            switch (share)
            {
                case FileShare.Delete:
                    return EFileShare.Delete;
                case FileShare.Inheritable:
                    throw new NotSupportedException("Inheritible is not supported.");
                case FileShare.None:
                    return EFileShare.None;
                case FileShare.Read:
                    return EFileShare.Read;
                case FileShare.ReadWrite:
                    return EFileShare.Read | EFileShare.Write;
                case FileShare.Write:
                    return EFileShare.Write;
            }
            throw new NotSupportedException();
        }

        public static EFileAccess ToNative(this FileAccess access)
        {
            switch (access)
            {
                case FileAccess.Read:
                    return EFileAccess.GenericRead;
                case FileAccess.Write:
                    return EFileAccess.GenericWrite;
                case FileAccess.ReadWrite:
                    return EFileAccess.GenericRead | EFileAccess.GenericWrite;
            }
            throw new NotSupportedException();
        }

        public static ECreationDisposition ToNative(this FileMode mode)
        {
            switch (mode)
            {
                case FileMode.Create:
                    return ECreationDisposition.CreateAlways;
                case FileMode.CreateNew:
                    return ECreationDisposition.New;
                case FileMode.Open:
                    return ECreationDisposition.OpenExisting;
                case FileMode.OpenOrCreate:
                    return ECreationDisposition.OpenAlways;
                case FileMode.Truncate:
                    return ECreationDisposition.TruncateExisting;
                case FileMode.Append:
                    return ECreationDisposition.OpenAlways;
            }
            throw new NotSupportedException();
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern SafeFileHandle CreateFile(
            string lpFileName,
            EFileAccess dwDesiredAccess,
            EFileShare dwShareMode,
            IntPtr lpSecurityAttributes,
            ECreationDisposition dwCreationDisposition,
            EFileAttributes dwFlagsAndAttributes,
            IntPtr hTemplateFile);
    }
}
