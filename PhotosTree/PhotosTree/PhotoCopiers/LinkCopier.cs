using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;

namespace PhotosTree.PhotoCopiers
{
	public class LinkCopier : PhotoCopier
	{
		#region Private Members

		private readonly Type type;
		private readonly object shell;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="LinkCopier"/> class.
		/// </summary>
		public LinkCopier()
		{
			type = Type.GetTypeFromProgID("WScript.Shell");

			if (type == null) throw new Exception("Failed to get type from 'WScript.Shell'.");

			shell = Activator.CreateInstance(type);
		}

		#endregion

		#region PhotoCopier Members

		/// <summary>
		/// Formats the target.
		/// </summary>
		/// <param name="path">The path.</param>
		protected override void FormatTarget(ref string path)
		{
			path = Path.ChangeExtension(path, ".lnk");
		}

		/// <summary>
		/// Copies the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		protected override void Copy(string source, string target)
		{
			object link = null;
				
			try
			{
				link = type.InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, Type.DefaultBinder, shell, new[] { target });

				type.InvokeMember("TargetPath", BindingFlags.SetProperty, Type.DefaultBinder, link, new[] { source });

				type.InvokeMember("Save", BindingFlags.InvokeMethod, Type.DefaultBinder, link, null);
			}
			finally
			{
				if (link != null)
				{
					Marshal.ReleaseComObject(link);
				}
			}
		}

		#endregion
	}
}
