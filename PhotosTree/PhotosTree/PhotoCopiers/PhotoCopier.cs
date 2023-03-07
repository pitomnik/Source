using System;
using System.IO;

namespace PhotosTree.PhotoCopiers
{
	#region CopyType Enum

	public enum CopyType
	{
		File,
		Link,
		HardLink,
		Url,
	}

	#endregion

	#region PhotoCopier Class

	public abstract class PhotoCopier : IPhotoCopier
	{
		#region IPhotoCopier Members

		/// <summary>
		/// Copies the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		/// <param name="overwrite">if set to <c>true</c> overwrite existing file.</param>
		public void Copy(string source, string target, bool overwrite)
		{
			FormatTarget(ref target);

			if (File.Exists(target))
			{
				if (overwrite)
				{
					File.Delete(target);
				}
				else
				{
					throw new Exception(String.Format("'{0}' already exists.", target));
				}
			}

			Copy(source, target);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static IPhotoCopier Create(CopyType type)
		{
			switch (type)
			{
				case CopyType.File:
					return new FileCopier();
				case CopyType.Link:
					return new LinkCopier();
				case CopyType.HardLink:
					return new HardLinkCopier();
				case CopyType.Url:
					return new UrlCopier();
				default:
					throw new NotImplementedException();
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Formats the target.
		/// </summary>
		/// <param name="path">The path.</param>
		protected virtual void FormatTarget(ref string path)
		{
		}

		/// <summary>
		/// Copies the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		protected abstract void Copy(string source, string target);

		#endregion
	}

	#endregion
}
