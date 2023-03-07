using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ZoneIDTrimmer.Shared;

namespace ZoneIDTrimmer.Report
{
    public partial class ReportTemplate
    {
        private readonly string _reportName;
        private readonly IEnumerable<Tuple<PathType, string>> _ignoredPaths;
        private readonly IEnumerable<Tuple<PathType, string>> _trimmedPaths;
        private readonly IEnumerable<Tuple<PathType, string>> _failedPaths;

        public ReportTemplate(string reportName, IEnumerable<Tuple<PathType, string>> ignoredPaths, IEnumerable<Tuple<PathType, string>> trimmedPaths, IEnumerable<Tuple<PathType, string>> failedPaths)
        {
            _reportName = reportName;
            _ignoredPaths = ignoredPaths;
            _trimmedPaths = trimmedPaths;
            _failedPaths = failedPaths;
        }

        private string BuildPathsHtml(IEnumerable<string> paths)
        {
            var stringBuilder = new StringBuilder();

            foreach (var line in paths)
            {
                var encodedPath = HttpUtility.HtmlEncode(line);
                var pathHref = "file:///" + encodedPath.Replace("\\", "/");
                var pathHtml = String.Format("<a href=\"{0}\">{1}</a>", pathHref, encodedPath);

                stringBuilder.Append(pathHtml).Append("<br/>").AppendLine();
            }

            return stringBuilder.ToString();
        }

        private string BuildPathGroupHtml(string groupName, IEnumerable<string> paths)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("<p>").AppendLine();
            stringBuilder.AppendFormat("<b>{0}</b>", groupName).AppendLine();
            stringBuilder.Append("</p>").AppendLine();

            if (paths.Any())
            {
                stringBuilder.Append("<div>").AppendLine();

                var pathsHtml = BuildPathsHtml(paths);

                stringBuilder.Append(pathsHtml);

                stringBuilder.Append("</div>").AppendLine();
            }

            stringBuilder.Append("<hr/>");

            return stringBuilder.ToString();
        }
    }
}
