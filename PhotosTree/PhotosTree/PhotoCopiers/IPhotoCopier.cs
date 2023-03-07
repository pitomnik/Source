namespace PhotosTree.PhotoCopiers
{
	public interface IPhotoCopier
	{
		/// <summary>
		/// Copies the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		/// <param name="overwrite">if set to <c>true</c> overwrite existing file.</param>
		void Copy(string source, string target, bool overwrite);
	}
}
