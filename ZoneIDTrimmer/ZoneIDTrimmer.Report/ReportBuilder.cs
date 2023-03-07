using System;
using System.Collections.Generic;
using ZoneIDTrimmer.Shared;

namespace ZoneIDTrimmer.Report
{
    public class ReportBuilder : IPathObserver
    {
        private readonly string _reportName;
        private readonly List<Tuple<PathType, string>> _ignoredPaths;
        private readonly List<Tuple<PathType, string>> _trimmedPaths;
        private readonly List<Tuple<PathType, string>> _failedPaths;

        public ReportBuilder(string reportName)
        {
            _reportName = reportName;
            _ignoredPaths = new List<Tuple<PathType, string>>();
            _trimmedPaths = new List<Tuple<PathType, string>>();
            _failedPaths = new List<Tuple<PathType, string>>();
        }

        public override string ToString()
        {
            var template = new ReportTemplate(_reportName, _ignoredPaths, _trimmedPaths, _failedPaths);

            return template.TransformText();
        }

        public void Notify(TrimEvent trimEvent, PathType pathType, string path)
        {
            switch (trimEvent)
            {
                case TrimEvent.Ignored:
                    _ignoredPaths.Add(new Tuple<PathType, string>(pathType, path));
                    break;
                case TrimEvent.Trimmed:
                    _trimmedPaths.Add(new Tuple<PathType, string>(pathType, path));
                    break;
                case TrimEvent.Failed:
                    _failedPaths.Add(new Tuple<PathType, string>(pathType, path));
                    break;
            }
        }
    }
}
