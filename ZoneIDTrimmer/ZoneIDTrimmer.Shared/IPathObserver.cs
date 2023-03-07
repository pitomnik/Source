namespace ZoneIDTrimmer.Shared
{
    public enum TrimEvent
    {
        Ignored,
        Scanned,
        Trimmed,
        Failed,
    }

    public enum PathType
    {
        File,
        Folder,
    }

    public interface IPathObserver
    {
        void Notify(TrimEvent trimEvent, PathType pathType, string path);
    }
}
