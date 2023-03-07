using System;
using System.IO;
using System.Diagnostics;

using PhotosTree.PhotoCopiers;

namespace PhotosTree
{
	class Program
	{
		#region Switches Class

		private sealed class Switches
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public static readonly string[] List = new[] { "/s", "/y", "/l", "/hl", "/url" };

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public static readonly string Subdirectories = List[0];
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public static readonly string Overwrite = List[1];
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public static readonly string Link = List[2];
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public static readonly string HardLink = List[3];
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public static readonly string Url = List[4];
		}

		#endregion

        #region Constructor

        /// <summary>
        /// Initializes the <see cref="Program"/> class.
        /// </summary>
        static Program()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        #endregion

		#region Entry Point

		/// <summary>
		/// Entry point.
		/// </summary>
		/// <param name="args">The args.</param>
		static void Main(string[] args)
		{
			ShowAbout();
			Console.WriteLine();

			if (args.Length < 2)
			{
				ShowUsage();
				return;
			}

			string source = args[0];
			string target = args[1];
			string format = args.Length >= 3 && !args[2].StartsWith("/") ?
				args[2] : Formats.Default;

			if (!Directory.Exists(args[0]))
			{
				Console.WriteLine("Path not found: '{0}'", args[0]);
				return;
			}

			if (!ValidateFormat(format))
			{
				Console.WriteLine("Format is invalid: '{0}'", format);
				return;
			}

			string invalidSwitch = ValidateSwitches(args);

			if (invalidSwitch != null)
			{
				Console.WriteLine("Switch is invalid: '{0}'", invalidSwitch);
				return;
			}

			const string pattern = "*.*";
			bool recursively = Array.IndexOf(args, Switches.Subdirectories) != -1;
			string[] files = Directory.GetFiles(source, pattern, recursively ?
				SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

			if (files.Length == 0)
			{
				Console.WriteLine("No files found: '{0}'", args[0]);
				return;
			}

			CopyType copyType = ReadCopyType(args);
			IPhotoCopier copier = PhotoCopier.Create(copyType);
			bool overwrite = Array.IndexOf(args, Switches.Overwrite) != -1;
			TreeBuilder builder = new TreeBuilder(target, format, copier, overwrite);

			builder.Progress += OnProgress;
			builder.Finish += OnFinish;

			builder.Build(files);

			Console.ReadKey(false);
		}

		#endregion

		#region Private Events

        /// <summary>
        /// Called when unhandled exception occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            Console.WriteLine("Unexpected error has occured.");

            var error = e.ExceptionObject as Exception;

            if (error != null)
            {
                Console.WriteLine(error.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(false);

            Environment.Exit(1);
        }

		/// <summary>
		/// Called on progress.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="PhotosTree.TreeBuilderEventArgs"/> instance containing the event data.</param>
		private static void OnProgress(object sender, TreeBuilderEventArgs e)
		{
			double percent = (double)e.Done / (double)e.Total;
			string message = String.Format("{0} | {1} - {2}", percent.ToString("000.00%"),
				e.Path, String.IsNullOrEmpty(e.Error) ? "OK" : e.Error);

			Console.WriteLine(message);
		}

		/// <summary>
		/// Handles the Finish event of the builder control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private static void OnFinish(object sender, EventArgs e)
		{
			Environment.Exit(0);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Shows the about.
		/// </summary>
		private static void ShowAbout()
		{
			Console.WriteLine("{0} v{1} - Orders photos in chronological tree.",
				ProgramInfo.Instance.Product, ProgramInfo.Instance.Version);
			Console.WriteLine(ProgramInfo.Instance.Copyright);
		}

		/// <summary>
		/// Shows the usage.
		/// </summary>
		private static void ShowUsage()
		{
			Console.WriteLine("Usage: {0} source target [format] [{1}] [{2}] [{3}]",
				ProgramInfo.Instance.Product, Switches.Subdirectories, Switches.Overwrite, String.Join("|", Switches.List, 2, 3));
			Console.WriteLine();
			Console.WriteLine("Format options: {0}", String.Join(", ", Formats.List));
			Console.WriteLine("Format samples: {0}\\{1}\\{2}, {3}\\{4}\\Week No {5}\\{6}",
				Formats.Year, Formats.Month, Formats.Day, Formats.Year, Formats.MonthName, Formats.Week, Formats.Day);
			Console.WriteLine();
			Console.WriteLine("\t{0}\tSearch all subdirectories.", Switches.Subdirectories);
			Console.WriteLine("\t{0}\tOverwrite existing files/links.", Switches.Overwrite);
			Console.WriteLine("\t{0}\tCreate shortcuts in target folder.", Switches.Link);
			Console.WriteLine("\t{0}\tCreate hard links in target folder.", Switches.HardLink);
			Console.WriteLine("\t{0}\tCreate internet links in target folder.", Switches.Url);
			Console.WriteLine();
			Console.WriteLine("The default tree format is {0}. " +
				"System settings are used to translate month names and calculate week numbers. " +
				"Unless specified otherwise photos will be copied (duplicated). " +
				"To save space use link switches ({1} is valid for NTFS only). " +
				"Process may be stopped by pressing any key.",
				Formats.Default, Switches.HardLink);
		}

		/// <summary>
		/// Validates the format.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <returns></returns>
		private static bool ValidateFormat(string format)
		{
			if (String.IsNullOrEmpty(format)) return false;

			foreach (string option in Formats.List)
			{
				if (format.Contains(option)) return true;
			}

			return false;
		}

		/// <summary>
		/// Validates the switches.
		/// </summary>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		private static string ValidateSwitches(string[] args)
		{
			foreach (string arg in args)
			{
				if (arg.StartsWith("/") && Array.IndexOf(Switches.List, arg) == -1)
				{
					return arg;
				}
			}

			return null;
		}

		/// <summary>
		/// Reads the type of the copy.
		/// </summary>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		private static CopyType ReadCopyType(string[] args)
		{
			if (Array.IndexOf(args, Switches.Link) != -1)
			{
				return CopyType.Link;
			}

			if (Array.IndexOf(args, Switches.HardLink) != -1)
			{
				return CopyType.HardLink;
			}

			if (Array.IndexOf(args, Switches.Url) != -1)
			{
				return CopyType.Url;
			}

			return CopyType.File;
		}

		#endregion
	}
}
