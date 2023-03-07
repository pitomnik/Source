using System;

namespace ZoneIDTrimmer.Shared
{
    public interface IToolView
    {
        event EventHandler OnCreate;
        event EventHandler OnDonate;
        event EventHandler OnClose;
        event EventHandler OnDetailsClick;
        event EventHandler OnHomepageClick;
        bool AutoClose { get; }
        TimeSpan AutoCloseDelay { get; }
        void Show(string title, bool autoClose, TimeSpan autoCloseDelay);
        void NotifyTrimStart();
        void NotifyTrimFinish();
    }
}
