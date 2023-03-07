using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TaskDialogInterop;
using ZoneIDTrimmer.Shared;
using ZoneIDTrimmer.View.Properties;

namespace ZoneIDTrimmer.View
{
    public class TaskDialogView : IToolView, IPathObserver
    {
        private const string DetailsLinkUri = "#details";
        private const string HomepageLinkUri = "#homepage";
        private const int CustomButtonOffset = 500;
        private const VistaProgressBarState StartBarState = VistaProgressBarState.Normal;
        private const VistaProgressBarState FinishBarState = VistaProgressBarState.Paused;

        private readonly List<KeyValuePair<string, Func<bool>>> _commands;
        private readonly Stopwatch _stopwatch;
        private string _title;

        private int _foldersIgnored, _filesIgnored;
        private int _foldersScanned, _filesScanned;
        private int _foldersTrimmed, _filesTrimmed;
        private int _foldersFailed, _filesFailed;

        private VistaProgressBarState _barState;
        private string _mainInstruction;
        private string _expandedInfo;

        public event EventHandler OnCreate;
        public event EventHandler OnDonate;
        public event EventHandler OnClose;
        public event EventHandler OnHomepageClick;
        public event EventHandler OnDetailsClick;

        public TaskDialogView()
        {
            _commands = new List<KeyValuePair<string, Func<bool>>>
            {
                new KeyValuePair<string, Func<bool>>(Resources.CommandDonate, FireOnDonate),
                new KeyValuePair<string, Func<bool>>(Resources.CommandClose, FireOnClose)
            };
            _stopwatch = new Stopwatch();
        }

        protected bool Finished
        {
            get;
            private set;
        }

        public bool AutoClose
        {
            get;
            private set;
        }

        public TimeSpan AutoCloseDelay
        {
            get;
            private set;
        }

        public void Show(string title, bool autoClose, TimeSpan autoCloseDelay)
        {
            _title = title;
            AutoClose = autoClose;
            AutoCloseDelay = autoCloseDelay;

            var options = CreateOptions(title);

            try
            {
                var result = TaskDialog.Show(options);

                if (result.VerificationChecked.HasValue)
                {
                    AutoClose = result.VerificationChecked.Value;
                }
            }
            finally
            {
                FireOnClose();
            }
        }

        public void NotifyTrimStart()
        {
            _barState = StartBarState;
            _mainInstruction = Resources.StatusStart;
            _expandedInfo = Resources.DetailsStart;
        }

        public void NotifyTrimFinish()
        {
            _barState = FinishBarState;
            _mainInstruction = Resources.StatusFinish;
            _expandedInfo = Resources.DetailsFinish;

            if (AutoClose)
            {
                _stopwatch.Start();
            }
        }

        public void Notify(TrimEvent trimEvent, PathType pathType, string path)
        {
            switch (trimEvent)
            {
                case TrimEvent.Ignored:
                    NotifyPathIgnored(pathType, path);
                    break;
                case TrimEvent.Scanned:
                    NotifyPathScanned(pathType, path);
                    break;
                case TrimEvent.Trimmed:
                    NotifyPathTrimmed(pathType, path);
                    break;
                case TrimEvent.Failed:
                    NotifyPathFailed(pathType, path);
                    break;
            }
        }

        public void NotifyPathIgnored(PathType pathType, string path)
        {
            switch (pathType)
            {
                case PathType.File:
                    _filesIgnored++;
                    break;
                case PathType.Folder:
                    _foldersIgnored++;
                    break;
            }

            _expandedInfo = BuildExpandedInfo(path);
        }

        public void NotifyPathScanned(PathType pathType, string path)
        {
            switch (pathType)
            {
                case PathType.File:
                    _filesScanned++;
                    break;
                case PathType.Folder:
                    _foldersScanned++;
                    break;
            }

            _expandedInfo = BuildExpandedInfo(path);
        }

        public void NotifyPathTrimmed(PathType pathType, string path)
        {
            switch (pathType)
            {
                case PathType.File:
                    _filesTrimmed++;
                    break;
                case PathType.Folder:
                    _foldersTrimmed++;
                    break;
            }
        }

        public void NotifyPathFailed(PathType pathType, string path)
        {
            switch (pathType)
            {
                case PathType.File:
                    _filesFailed++;
                    break;
                case PathType.Folder:
                    _foldersFailed++;
                    break;
            }
        }

        private TaskDialogOptions CreateOptions(string title)
        {
            var options = new TaskDialogOptions
            {
                Title = title,
                CustomMainIcon = Shared.Properties.Resources.Trimmer,
                MainInstruction = Resources.StatusInit,
                Content = BuildContent(),
                VerificationText = Resources.CloseWhenFinished,
                ExpandedInfo = Resources.DetailsInit,
                FooterText = String.Format(Resources.HomepageLink, HomepageLinkUri),
                FooterIcon = VistaTaskDialogIcon.Information,
                CustomButtons = _commands.Select(x => x.Key).ToArray(),
                AllowDialogCancellation = true,
                ShowMarqueeProgressBar = true,
                EnableCallbackTimer = true,
                VerificationByDefault = AutoClose,
                Callback = Callback,
            };

            return options;
        }

        private string BuildExpandedInfo(string path)
        {
            return path.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).Last();
        }

        private string BuildContent()
        {
            return String.Format(Resources.TotalInfo, _foldersIgnored, _filesIgnored, _foldersScanned, _filesScanned, _foldersTrimmed, _filesTrimmed, _foldersFailed, _filesFailed) +
                Environment.NewLine + Environment.NewLine + String.Format(Finished ? Resources.DetailsLinkEnabled : Resources.DetailsLinkDisabled, DetailsLinkUri);
        }

        private bool Callback(IActiveTaskDialog dialog, VistaTaskDialogNotificationArgs args, object callbackData)
        {
            var result = false;

            switch (args.Notification)
            {
                case VistaTaskDialogNotification.Created:
                    dialog.SetProgressBarMarquee(true, 10);
                    FireOnCreate();
                    break;
                case VistaTaskDialogNotification.Timer:
                    if (_barState != FinishBarState || !Finished)
                    {
                        Finished = _barState == FinishBarState;

                        dialog.SetProgressBarState(_barState);
                        dialog.SetMainInstruction(_mainInstruction);
                        dialog.SetContent(BuildContent());
                        dialog.SetExpandedInformation(_expandedInfo);
                    }

                    if (Finished)
                    {
                        var elapsed = _stopwatch.Elapsed;

                        if (elapsed > AutoCloseDelay)
                        {
                            _stopwatch.Stop();
                            FireOnClose();
                        }
                        else if (AutoClose)
                        {
                            var remaining = AutoCloseDelay - elapsed;
                            dialog.SetWindowTitle(String.Format(Resources.ClosingIn, remaining.Seconds));
                        }
                    }
                    break;
                case VistaTaskDialogNotification.ButtonClicked:
                    if (args.ButtonId >= CustomButtonOffset)
                    {
                        result = HandleButtonClick(args.ButtonId);
                    }
                    break;
                case VistaTaskDialogNotification.VerificationClicked:
                    AutoClose = args.VerificationFlagChecked;

                    if (AutoClose)
                    {
                        _stopwatch.Start();
                    }
                    else
                    {
                        _stopwatch.Stop();
                        _stopwatch.Reset();
                        dialog.SetWindowTitle(_title);
                    }
                    break;
                case VistaTaskDialogNotification.HyperlinkClicked:
                    HandleHyperlinkClick(args.Hyperlink);
                    break;
            }

            return result;
        }

        private bool HandleButtonClick(int buttonId)
        {
            var index = buttonId - CustomButtonOffset;
            var action = _commands[index].Value;

            return action();
        }

        private void HandleHyperlinkClick(string hyperlink)
        {
            if (DetailsLinkUri.Equals(hyperlink))
            {
                FireOnDetailsClick();
            }
            else if (HomepageLinkUri.Equals(hyperlink))
            {
                FireOnHomepageClick();
            }
        }

        private void FireOnCreate()
        {
            if (OnCreate != null) OnCreate(this, EventArgs.Empty);
        }

        private bool FireOnDonate()
        {
            if (OnDonate != null) OnDonate(this, EventArgs.Empty);

            return true;
        }

        private bool FireOnClose()
        {
            if (OnClose != null) OnClose(this, EventArgs.Empty);

            return false;
        }

        private void FireOnDetailsClick()
        {
            if (OnDetailsClick != null) OnDetailsClick(this, EventArgs.Empty);
        }

        private void FireOnHomepageClick()
        {
            if (OnHomepageClick != null) OnHomepageClick(this, EventArgs.Empty);
        }
    }
}
