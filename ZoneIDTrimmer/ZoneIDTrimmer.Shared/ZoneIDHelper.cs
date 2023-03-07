using System;

namespace ZoneIDTrimmer.Shared
{
    public class ZoneIDHelper
    {
        private const string StreamName = "Zone.Identifier";

        public static bool ZoneIDExists(string path)
        {
            return NativeApi.StreamExists(path, StreamName);
        }

        public static bool DeleteZoneID(string path)
        {
            return NativeApi.DeleteStream(path, StreamName);
        }

        public static void NotifyShell()
        {
            NativeApi.SHChangeNotify(NativeApi.SHCNE_ASSOCCHANGED, NativeApi.SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
