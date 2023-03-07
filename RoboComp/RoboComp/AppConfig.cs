using System;
using System.Diagnostics;
using System.Configuration;

namespace RoboComp
{
	internal class AppConfig
	{
		public static int CruiseSpeed
		{
			[DebuggerNonUserCode]
			get { return Convert.ToInt32(ConfigurationManager.AppSettings["CruiseSpeed"]); }
		}

		public static int SpeedDelta
		{
			[DebuggerNonUserCode]
			get { return Convert.ToInt32(ConfigurationManager.AppSettings["SpeedDelta"]); }
		}

		public static int TurnStep
		{
			[DebuggerNonUserCode]
			get { return Convert.ToInt32(ConfigurationManager.AppSettings["TurnStep"]); }
		}

		public static string Camera
		{
			[DebuggerNonUserCode]
			get { return ConfigurationManager.AppSettings["Camera"]; }
		}

		public static bool Capture
		{
			[DebuggerNonUserCode]
			get { return Convert.ToBoolean(ConfigurationManager.AppSettings["Capture"]); }
		}
	}
}
