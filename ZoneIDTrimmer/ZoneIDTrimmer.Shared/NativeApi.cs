using System;
using System.Runtime.InteropServices;

namespace ZoneIDTrimmer.Shared
{
    public sealed class NativeApi
    {
        // ReSharper disable InconsistentNaming

        public const int S_OK = 0x0;
        public const int S_FALSE = 0x1;
        
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;

        public const uint CREATE_NEW = 1;
        public const uint CREATE_ALWAYS = 2;
        public const uint OPEN_EXISTING = 3;
        public const uint OPEN_ALWAYS = 4;
        public const uint TRUNCATE_EXISTING = 5;

        public const uint FILE_BEGIN = 0;
        public const uint FILE_CURRENT = 1;
        public const uint FILE_END = 2;

        public const uint FILE_SHARE_NONE = 0;
        public const uint FILE_SHARE_READ = 1;

        public const uint SHCNE_ALLEVENTS = 0x7FFFFFFF;
        public const uint SHCNE_ASSOCCHANGED = 0x8000000;
        public const uint SHCNE_ATTRIBUTES = 0x800;
        public const uint SHCNE_CREATE = 0x2;
        public const uint SHCNE_DELETE = 0x4;
        public const uint SHCNE_DISKEVENTS = 0x2381F;
        public const uint SHCNE_DRIVEADD = 0x100;
        public const uint SHCNE_DRIVEADDGUI = 0x10000;
        public const uint SHCNE_DRIVEREMOVED = 0x80;
        public const uint SHCNE_EXTENDED_EVENT = 0x4000000;
        public const uint SHCNE_FREESPACE = 0x40000;
        public const uint SHCNE_GLOBALEVENTS = 0xC0581E0;
        public const uint SHCNE_uintERRUPT = 0x80000000;
        public const uint SHCNE_MEDIAINSERTED = 0x20;
        public const uint SHCNE_MEDIAREMOVED = 0x40;
        public const uint SHCNE_MKDIR = 0x8;
        public const uint SHCNE_NETSHARE = 0x200;
        public const uint SHCNE_NETUNSHARE = 0x400;
        public const uint SHCNE_RENAMEFOLDER = 0x20000;
        public const uint SHCNE_RENAMEITEM = 0x1;
        public const uint SHCNE_RMDIR = 0x10;
        public const uint SHCNE_SERVERDISCONNECT = 0x4000;
        public const uint SHCNE_UPDATEDIR = 0x1000;
        public const uint SHCNE_UPDATEIMAGE = 0x8000;
        public const uint SHCNE_UPDATEITEM = 0x2000;

        public const int SHCNF_ACCEPT_INTERRUPTS = 0x1;
        public const int SHCNF_ACCEPT_NON_INTERRUPTS = 0x2;
        public const int SHCNF_DWORD = 0x3;
        public const int SHCNF_FLUSH = 0x1000;
        public const int SHCNF_FLUSHNOWAIT = 0x2000;
        public const int SHCNF_IDLIST = 0x0;
        public const int SHCNF_NO_PROXY = 0x8000;
        public const int SHCNF_PATHA = 0x1;
        public const int SHCNF_PATHW = 0x5;
        public const int SHCNF_PRINTERA = 0x2;
        public const int SHCNF_PRINTERW = 0x6;
        public const int SHCNF_TYPE = 0xFF;

        public const int ISIOI_ICONFILE = 0x1;
        public const int ISIOI_ICONINDEX = 0x2;

        // ReSharper restore InconsistentNaming

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool DeleteFile(string lpFileName);

        [DllImport("shell32.dll", SetLastError = true)]
        internal static extern void SHChangeNotify(uint wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);
        
        public static bool StreamExists(string path, string stream)
        {
            var fileName = CombineFileName(path, stream);
            var handle = CreateFile(fileName, GENERIC_READ, FILE_SHARE_READ, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            var exists = handle.ToInt32() != -1;

            if (exists) CloseHandle(handle);

            return exists;
        }

        public static bool DeleteStream(string path, string stream)
        {
            string fileName = CombineFileName(path, stream);

            return DeleteFile(fileName);
        }

        private static string CombineFileName(string path, string stream)
        {
            return String.Format("{0}:{1}", path, stream);
        }
    }
}
