using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SurfedAndFound.Msie
{
	public sealed class IID
	{
		public static readonly Guid IWebBrowserApp = new Guid("{0002DF05-0000-0000-C000-000000000046}");
		public static readonly Guid IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");
	}

	[Flags]
	public enum DBIMF
	{
		NORMAL = 0x0001,
		FIXED = 0x0002,
		FIXEDBMP = 0x0004,
		VARIABLEHEIGHT = 0x0008,
		UNDELETEABLE = 0x0010,
		DEBOSSED = 0x0020,
		BKCOLOR = 0x0040,
		USECHEVRON = 0x0080,
		BREAK = 0x0100,
		ADDTOFRONT = 0x0200,
		TOPALIGN = 0x0400,
		NOGRIPPER = 0x0800,
		ALWAYSGRIPPER = 0x1000,
		NOMARGINS = 0x2000,
	}

	[Flags]
	public enum DBIM : uint
	{
		MINSIZE = 0x0001,
		MAXSIZE = 0x0002,
		INTEGRAL = 0x0004,
		ACTUAL = 0x0008,
		TITLE = 0x0010,
		MODEFLAGS = 0x0020,
		BKCOLOR = 0x0040
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DESKBANDINFO
	{
		public DBIM dwMask;
		public Point ptMinSize;
		public Point ptMaxSize;
		public Point ptIntegral;
		public Point ptActual;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
		public String wszTitle;
		public DBIMF dwModeFlags;
		public Int32 crBkgnd;
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("FC4801A3-2BA9-11CF-A229-00AA003D7352")]
	public interface IObjectWithSite
	{
		void SetSite([In, MarshalAs(UnmanagedType.IUnknown)] Object pUnkSite);
		void GetSite(ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out Object ppvSite);
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("00000114-0000-0000-C000-000000000046")]
	public interface IOleWindow
	{
		void GetWindow(out IntPtr phwnd);
		void ContextSensitiveHelp([In] bool fEnterMode);
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("012dd920-7b26-11d0-8ca9-00a0c92dbfe8")]
	public interface IDockingWindow
	{
		void GetWindow(out IntPtr phwnd);
		void ContextSensitiveHelp([In] bool fEnterMode);
		void ShowDW([In] bool fShow);
		void CloseDW([In] UInt32 dwReserved);
		void ResizeBorderDW(IntPtr prcBorder, [In, MarshalAs(UnmanagedType.IUnknown)] Object punkToolbarSite, bool fReserved);
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("EB0FE172-1A3A-11D0-89B3-00A0C90A90AC")]
	public interface IDeskBand
	{
		void GetWindow(out IntPtr phwnd);
		void ContextSensitiveHelp([In] bool fEnterMode);
		void ShowDW([In] bool fShow);
		void CloseDW([In] UInt32 dwReserved);
		void ResizeBorderDW(IntPtr prcBorder, [In, MarshalAs(UnmanagedType.IUnknown)] Object punkToolbarSite, bool fReserved);
		void GetBandInfo(UInt32 dwBandID, UInt32 dwViewMode, ref DESKBANDINFO pdbi);
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("0000010c-0000-0000-C000-000000000046")]
	public interface IPersist
	{
		void GetClassID(out Guid pClassID);
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("00000109-0000-0000-C000-000000000046")]
	public interface IPersistStream
	{
		void GetClassID(out Guid pClassID);
		void IsDirty();
		void Load([In, MarshalAs(UnmanagedType.Interface)] Object pStm);
		void Save([In, MarshalAs(UnmanagedType.Interface)] Object pStm, [In] bool fClearDirty);
		void GetSizeMax([Out] out UInt64 pcbSize);
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
	public interface _IServiceProvider
	{
		void QueryService(ref Guid guid, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out Object Obj);
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("68284faa-6a48-11d0-8c78-00c04fd918b4")]
	public interface IInputObject
	{
		void UIActivateIO(Int32 fActivate, ref MSG msg);
		[PreserveSig]
		Int32 HasFocusIO();
		[PreserveSig]
		Int32 TranslateAcceleratorIO(ref MSG msg);
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("f1db8392-7331-11d0-8c99-00a0c92dbfe8")]
	public interface IInputObjectSite
	{
		[PreserveSig]
		Int32 OnFocusChangeIS([MarshalAs(UnmanagedType.IUnknown)] Object punkObj, Int32 fSetFocus);
	}

	public struct MSG
	{
		public IntPtr hWnd;
		public int message;
		public int wParam;
		public int lParam;
		public int time;
		public POINT pt;
	}

	public struct POINT
	{
		public int x;
		public int y;
	}
}
