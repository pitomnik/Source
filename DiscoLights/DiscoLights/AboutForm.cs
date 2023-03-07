using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DiscoLights
{
	partial class AboutForm : Form
	{
		#region Private Members

		private readonly string[] dependencies = new[] { "NAudio.dll", "log4net.dll"};

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="AboutForm"/> class.
		/// </summary>
		public AboutForm()
		{
			InitializeComponent();

			Text = String.Format("About {0}", AssemblyTitle);
			labelProductName.Text = AssemblyProduct;
			labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
			labelCopyright.Text = AssemblyCopyright;
			labelCompanyName.Text = AssemblyCompany;
			textBoxDescription.Text = ReadFilesInfo(dependencies);
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets the assembly title.
		/// </summary>
		public string AssemblyTitle
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (attributes.Length > 0)
				{
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if (titleAttribute.Title != "")
					{
						return titleAttribute.Title;
					}
				}
				return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		/// <summary>
		/// Gets the assembly version.
		/// </summary>
		public string AssemblyVersion
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		/// <summary>
		/// Gets the assembly description.
		/// </summary>
		public string AssemblyDescription
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		/// <summary>
		/// Gets the assembly product.
		/// </summary>
		public string AssemblyProduct
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		/// <summary>
		/// Gets the assembly copyright.
		/// </summary>
		public string AssemblyCopyright
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		/// <summary>
		/// Gets the assembly company.
		/// </summary>
		public string AssemblyCompany
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyCompanyAttribute)attributes[0]).Company;
			}
		}

		#endregion

		#region Private Events

		/// <summary>
		/// Handles the Click event of the CompanyName control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void CompanyName_Click(object sender, EventArgs e)
		{
			Process.Start("http://www.gasanov.net");
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Reads the files info.
		/// </summary>
		/// <param name="fileNames">The file names.</param>
		/// <returns></returns>
		private string ReadFilesInfo(string[] fileNames)
		{
			StringBuilder sb = new StringBuilder();

			foreach (string dependency in dependencies)
			{
				string info;

				try
				{
					info = ReadFileInfo(dependency);
				}
				catch (Exception ex)
				{
					LogMaster.Instance.Error("Failed to get file info.", ex);

					continue;
				}

				sb.Append(info);
				sb.Append(Environment.NewLine);
			}

			if (sb.Length > Environment.NewLine.Length)
			{
				sb.Length -= Environment.NewLine.Length;
			}

			return sb.ToString();
		}

		/// <summary>
		/// Reads the file info.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns></returns>
		private string ReadFileInfo(string fileName)
		{
			string path = Path.Combine(Application.StartupPath, fileName);

			if (!File.Exists(path))
			{
				throw new FileNotFoundException(String.Format("File '{0}' not found.", fileName), path);
			}

			StringBuilder sb = new StringBuilder();
			FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(path);

			sb.AppendFormat("{0}", fvi.ProductName);
			sb.AppendLine();
			sb.AppendFormat("\t{0}", fvi.CompanyName);
			sb.AppendLine();
			sb.AppendFormat("\t{0}", fvi.FileDescription);
			sb.AppendLine();
			sb.AppendFormat("\t{0}", fvi.FileVersion);

			return sb.ToString();
		}

		#endregion
	}
}
