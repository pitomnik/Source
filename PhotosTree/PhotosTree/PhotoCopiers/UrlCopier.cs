using System;
using System.IO;
using System.Text;

namespace PhotosTree.PhotoCopiers
{
	public class UrlCopier : PhotoCopier
	{
		#region PhotoCopier Members

		/// <summary>
		/// Formats the target.
		/// </summary>
		/// <param name="path">The path.</param>
		protected override void FormatTarget(ref string path)
		{
			path = Path.ChangeExtension(path, ".url");
		}

		/// <summary>
		/// Copies the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		protected override void Copy(string source, string target)
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("[InternetShortcut]");
			sb.Append("URL=file:///");
			sb.AppendLine(source.Replace("\\", "/"));

			File.WriteAllText(target, sb.ToString());
		}

		#endregion
	}
}
