using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Globalization;

using PhotosTree.PhotoCopiers;

namespace PhotosTree
{
	public class TreeBuilder
	{
		#region Private Members

		private readonly string target;
		private readonly string format;
		private readonly IPhotoCopier copier;
		private readonly bool overwrite;
		private CultureInfo culture;
		private int total, done;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeBuilder"/> class.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="format">The format.</param>
		/// <param name="copier">The copier.</param>
		/// <param name="overwrite">if set to <c>true</c> overwrite existing file.</param>
		public TreeBuilder(string target, string format, IPhotoCopier copier, bool overwrite)
		{
			if (String.IsNullOrEmpty(target)) throw new ArgumentException("No target specified.", "target");
			if (String.IsNullOrEmpty(format)) throw new ArgumentException("No format specified.", "format");
			if (copier == null) throw new ArgumentNullException("copier");

			this.target = target;
			this.format = format;
			this.copier = copier;
			this.overwrite = overwrite;

			culture = CultureInfo.InstalledUICulture;
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets or sets the culture.
		/// </summary>
		/// <value>The culture.</value>
		public CultureInfo Culture
		{
			[DebuggerNonUserCode]
			get { return culture; }
			[DebuggerNonUserCode]
			set { culture = value; }
		}

		#endregion

		#region Public Events

		/// <summary>
		/// Occurs on progress.
		/// </summary>
		public event EventHandler<TreeBuilderEventArgs> Progress;

		/// <summary>
		/// Occurs on finish.
		/// </summary>
		public event EventHandler Finish;

		#endregion

		#region Public Methods

		/// <summary>
		/// Builds this instance.
		/// </summary>
		/// <param name="files">The files.</param>
		public void Build(string[] files)
		{
			if (files == null || files.Length == 0) throw new ArgumentException("No files specified.", "files");

			total += files.Length;

			foreach (string file in files)
			{
				ThreadPool.QueueUserWorkItem(InternalBuild, file);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Internals the build.
		/// </summary>
		/// <param name="file">The file.</param>
		private void InternalBuild(object file)
		{
			string path = (string)file;

			try
			{
				InternalBuild(path);
			}
			catch (Exception ex)
			{
				EventLog.WriteEntry(ProgramInfo.Instance.Product, ex.ToString(), EventLogEntryType.Error);
				FireProgress(path, ex.Message);
			}
		}

		/// <summary>
		/// Internals the build.
		/// </summary>
		/// <param name="file">The file.</param>
		private void InternalBuild(string file)
		{
			Photo photo = PhotoReader.Instance.Read(file);

			if (photo.DateTaken.Equals(DateTime.MinValue))
			{
				return;
			}

			string folder = CreateFolderName(photo.DateTaken);
			string path = Path.Combine(target, folder);

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			string fileName = Path.GetFileName(file);
			string fullPath = Path.Combine(path, fileName);

			copier.Copy(file, fullPath, overwrite);

			FireProgress(file);
		}

		/// <summary>
		/// Creates the name of the folder.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <returns></returns>
		private string CreateFolderName(DateTime date)
		{
			string folder = format;

			folder = folder.Replace(Formats.Year, date.Year.ToString());
			folder = folder.Replace(Formats.Month, date.Month.ToString("00"));
			folder = folder.Replace(Formats.MonthName, GetMonthName(date));
			folder = folder.Replace(Formats.Week, GetWeekOfYear(date).ToString());
			folder = folder.Replace(Formats.Day, date.Day.ToString("00"));
			folder = folder.Replace(Formats.Hour, date.Hour.ToString("00"));
			folder = folder.Replace(Formats.Minute, date.Minute.ToString("00"));

			return folder;
		}

		/// <summary>
		/// Gets the name of the month.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <returns></returns>
		private string GetMonthName(DateTime date)
		{
			return Culture.DateTimeFormat.GetMonthName(date.Month);
		}

		/// <summary>
		/// Gets the week of year.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <returns></returns>
		private int GetWeekOfYear(DateTime date)
		{
			return Culture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, Culture.DateTimeFormat.FirstDayOfWeek);
		}

		/// <summary>
		/// Fires the progress.
		/// </summary>
		/// <param name="path">The path.</param>
		private void FireProgress(string path)
		{
			FireProgress(path, null);
		}

		/// <summary>
		/// Fires the progress.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="error">The error.</param>
		private void FireProgress(string path, string error)
		{
			Interlocked.Increment(ref done);

			if (Progress != null)
			{
				TreeBuilderEventArgs args = new TreeBuilderEventArgs(total, done, path, error);

				Progress(this, args);
			}

			if (done == total && Finish != null)
			{
				Finish(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}
