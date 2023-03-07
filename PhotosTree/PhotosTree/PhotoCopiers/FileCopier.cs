using System.IO;

namespace PhotosTree.PhotoCopiers
{
	public class FileCopier : PhotoCopier
	{
		#region PhotoCopier Members

		/// <summary>
		/// Copies the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		protected override void Copy(string source, string target)
		{
			File.Copy(source, target);
		}

		#endregion
	}
}
