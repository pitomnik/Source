using System;
using System.Reflection;
using System.Diagnostics;

namespace PhotosTree
{
	public class ProgramInfo
	{
		#region Private Members

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly ProgramInfo instance = new ProgramInfo();

		private Version version;
		private readonly string product, company, copyright;

		#endregion

		#region Constructors

		private ProgramInfo()
		{
			Assembly assembly = GetType().Assembly;

			version = assembly.GetName().Version;
			product = (assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true)[0] as AssemblyProductAttribute).Product;
			company = (assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true)[0] as AssemblyCompanyAttribute).Company;
			copyright = (assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true)[0] as AssemblyCopyrightAttribute).Copyright;
		}

		#endregion

		#region Public Members

		public static ProgramInfo Instance
		{
			[DebuggerNonUserCode]
			get { return instance; }
		}

		public string Version
		{
			[DebuggerNonUserCode]
			get { return version.ToString(); }
		}

		public string Product
		{
			[DebuggerNonUserCode]
			get { return product; }
		}

		public string Company
		{
			[DebuggerNonUserCode]
			get { return company; }
		}

		public string Copyright
		{
			[DebuggerNonUserCode]
			get { return copyright; }
		}

		#endregion
	}
}
