using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace PhotosTree
{
	public class PhotoReader
	{
		#region Private Members

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly PhotoReader instance = new PhotoReader();

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PhotoReader"/> class.
		/// </summary>
		private PhotoReader()
		{
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static PhotoReader Instance
		{
			[DebuggerNonUserCode]
			get { return instance; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Reads the specified path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		public Photo Read(string path)
		{
			if (!File.Exists(path))
			{
				throw new ArgumentException("Path not found.", "path");
			}

			Image image = Image.FromFile(path);

			try
			{
				PropertyItem property = image.GetPropertyItem(0x9003);
				string dateString = Encoding.ASCII.GetString(property.Value);
				DateTime dateTaken = DateTime.ParseExact(dateString, "yyyy:MM:dd HH:mm:ss\0", null);

				return new Photo(dateTaken);
			}
			finally
			{
				image.Dispose();
			}
		}

		#endregion
	}
}
