using System;

namespace PhotosTree
{
	public class Photo
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Photo"/> class.
		/// </summary>
		/// <param name="dateTaken">The date taken.</param>
		public Photo(DateTime dateTaken)
		{
			DateTaken = dateTaken;
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets or sets the date taken.
		/// </summary>
		/// <value>The date taken.</value>
		public DateTime DateTaken
		{
			get;
			set;
		}

		#endregion
	}
}
