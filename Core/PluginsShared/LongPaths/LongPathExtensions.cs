using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mnk.Library.Common.Log;
using ZetaLongPaths;

namespace Mnk.TBox.Core.PluginsShared.LongPaths
{
    public static class LongPathExtensions
    {
        public static void CopyFilesTo(this ZlpDirectoryInfo info, string destination, bool overwrite = true)
        {
            var dirs = info.GetDirectories();
            if (!ZlpIOHelper.DirectoryExists(destination))
            {
                ZlpIOHelper.CreateDirectory(destination);
            }
            foreach (var file in info.GetFiles())
            {
                var target = ZlpPathHelper.Combine(destination, file.Name);
                if (!overwrite && ZlpIOHelper.FileExists(target)) continue;
                file.CopyTo(target, overwrite);
            }
            foreach (var dir in dirs)
            {
                dir.CopyFilesTo(ZlpPathHelper.Combine(destination, dir.Name), overwrite);
            }
        }

        public static void MoveFilesTo(this ZlpDirectoryInfo info, string destination, string mask = "*.*")
        {
            var dirs = info.GetDirectories();
            if (!ZlpIOHelper.DirectoryExists(destination))
            {
                ZlpIOHelper.CreateDirectory(destination);
            }
            foreach (var file in
                mask.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .SelectMany(info.GetFiles))
            {
                file.MoveTo(ZlpPathHelper.Combine(destination, file.Name));
            }
            foreach (var dir in dirs)
            {
                dir.MoveFilesTo(ZlpPathHelper.Combine(destination, dir.Name), mask);
            }
        }

        public static IEnumerable<ZlpDirectoryInfo> SafeEnumerateDirectories(this ZlpDirectoryInfo info, ILog log)
        {
            try
            {
                return info.GetDirectories();
            }
            catch (DirectoryNotFoundException) { }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                log.Write(ex, "Can't access to directory: " + info.FullName);
            }
            return new ZlpDirectoryInfo[0];
        }

        public static IEnumerable<ZlpFileInfo> SafeEnumerateFiles(this ZlpDirectoryInfo info, ILog log, string mask = "*.*")
        {
            try
            {
                return info.GetFiles(mask);
            }
            catch (FileNotFoundException) { }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                log.Write(ex, "Can't access to directory files: " + info.FullName);
            }
            return new ZlpFileInfo[0];
        }

        public static void MoveIfExist(this ZlpDirectoryInfo source, string destination)
        {
            if (!source.Exists) return;
            source.MoveFilesTo(destination);
            source.Delete(false);
        }

        public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            var fileHandle = NativeMethods.CreateFile(@"\\?\" + path, access.ToNative(), share.ToNative(), IntPtr.Zero,
                mode.ToNative(), 0, IntPtr.Zero);
            var lastWin32Error = Marshal.GetLastWin32Error();
            if (fileHandle.IsInvalid)
            {
                throw new System.ComponentModel.Win32Exception(lastWin32Error);
            }
            var result = new FileStream(fileHandle, access);
            if (mode == FileMode.Append)
            {
                result.Seek(0, SeekOrigin.End);
            }
            return result;
        }

        public static FileStream OpenRead(string path)
        {
            return Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

    }
}
