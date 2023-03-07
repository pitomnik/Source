using System;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;

using SurfedAndFound.UI;
using SurfedAndFound.Logic.Managers;
using SurfedAndFound.Shared.Types;
using SurfedAndFound.Shared.Win32;
using SurfedAndFound.Shared.Tools;

using SHDocVw;

namespace SurfedAndFound.Msie
{
	[ComVisible(true)]
	[Guid("37C3869F-6111-4692-8581-623FB7B01B6F")]
	public class MsieSearchPane : SearchPane, IObjectWithSite, IDeskBand, IDockingWindow, IOleWindow, IInputObject
	{
		#region Private Members

		private WebBrowserClass browser;
		private IInputObjectSite site;
		private object missing;

		#endregion

		#region Constructors

		static MsieSearchPane()
		{
			Application.EnableVisualStyles();
			ProgramInfo.Instance.Locate();
		}

		public MsieSearchPane()
			: base()
		{
			missing = Type.Missing;

			txtQuery.GotFocus += new EventHandler(txtQuery_GotFocus);
		}

		#endregion

		#region IObjectWithSite Members

		public void SetSite(object pUnkSite)
		{
			if (site != null)
			{
				Marshal.ReleaseComObject(site);
				site = null;
			}

			if (browser != null)
			{
				Marshal.ReleaseComObject(browser);
				browser = null;
			}

			site = (IInputObjectSite)pUnkSite;
			
			if (site != null)
			{
				_IServiceProvider sp = site as _IServiceProvider;
				Guid guid = IID.IWebBrowserApp;
				Guid riid = IID.IUnknown;

				try
				{
					object obj;

					sp.QueryService(ref guid, ref riid, out obj);

					browser = (WebBrowserClass)Marshal.CreateWrapperOfType(obj as IWebBrowser, typeof(WebBrowserClass));
				}
				catch (COMException ex)
				{
					Debug.Assert(false, ex.Message);
				}
			}
		}

		public void GetSite(ref Guid riid, out object ppvSite)
		{
			ppvSite = site;
		}

		#endregion

		#region IDeskBand Members

		public void GetWindow(out IntPtr phwnd)
		{
			phwnd = Handle;
		}

		public void ContextSensitiveHelp(bool fEnterMode)
		{
			// do nothing
		}

		public void ShowDW(bool fShow)
		{
			if (fShow)
			{
				Show();
			}
			else
			{
				StopSearch();
				Hide();
			}
		}

		public void CloseDW(uint dwReserved)
		{
			StopSearch();
			SaveState();
			Dispose(true);
		}

		public void ResizeBorderDW(IntPtr prcBorder, object punkToolbarSite, bool fReserved)
		{
			// do nothing
		}

		public void GetBandInfo(uint dwBandID, uint dwViewMode, ref DESKBANDINFO pdbi)
		{
			if ((pdbi.dwMask & DBIM.MINSIZE) != 0)
			{
				pdbi.ptMinSize.X = 0;
				pdbi.ptMinSize.Y = 0;
			}

			if ((pdbi.dwMask & DBIM.MAXSIZE) != 0)
			{
				pdbi.ptMaxSize.X = -1;
				pdbi.ptMaxSize.Y = -1;
			}

			if ((pdbi.dwMask & DBIM.INTEGRAL) != 0)
			{
				pdbi.ptIntegral.X = 1;
				pdbi.ptIntegral.Y = 1;
			}

			if ((pdbi.dwMask & DBIM.ACTUAL) != 0)
			{
				pdbi.ptActual.X = 0;
				pdbi.ptActual.Y = 0;
			}

			if ((pdbi.dwMask & DBIM.TITLE) != 0)
			{
				pdbi.wszTitle = ProgramInfo.Instance.Name;
			}

			if ((pdbi.dwMask & DBIM.MODEFLAGS) != 0)
			{
				pdbi.dwModeFlags = DBIMF.VARIABLEHEIGHT;
			}

			if ((pdbi.dwMask & DBIM.BKCOLOR) != 0)
			{
				pdbi.dwMask &= ~DBIM.BKCOLOR;
			}
		}

		#endregion

		#region IInputObject Members

		public void UIActivateIO(int fActivate, ref MSG msg)
		{
			if (fActivate != 0)
			{
				Control control = GetNextControl(this, ModifierKeys == Keys.Shift ? false : true);

				if (control != null)
				{
					control.Select();
				}

				Focus();
			}
		}

		public int HasFocusIO()
		{
			return ContainsFocus || txtQuery.Focused || txtQuery.ContainsFocus ?
				Constants.S_OK : Constants.S_FALSE;
		}

		public int TranslateAcceleratorIO(ref MSG msg)
		{
			if (msg.message == Constants.WM_KEYDOWN)
			{
				switch (msg.wParam)
				{
					case (int)Keys.Tab:
						if (SelectNextControl(ActiveControl, ModifierKeys == Keys.Shift ? false : true, true, true, false))
						{
							return Constants.S_OK;
						}
						break;
					case (int)Keys.Back:
						if (User32.SendMessage(msg.hWnd, Constants.WM_CHAR, msg.wParam, msg.lParam) == 1)
						{
							return Constants.S_OK;
						}
						break;
					case (int)Keys.Delete:
					case (int)Keys.Home:
					case (int)Keys.End:
					case (int)Keys.Up:
					case (int)Keys.Down:
					case (int)Keys.Left:
					case (int)Keys.Right:
						if (User32.SendMessage(msg.hWnd, msg.message, msg.wParam, msg.lParam) == 1)
						{
							return Constants.S_OK;
						}
						break;
				}
			}

			return Constants.S_FALSE;
		}

		#endregion

		#region Private Events

		private void txtQuery_GotFocus(object sender, EventArgs e)
		{
			OnGotFocus(e);
		}

		#endregion

		#region Private Methods

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			
			site.OnFocusChangeIS(this, Constants.TRUE);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);

			site.OnFocusChangeIS(this, Constants.FALSE);
		}

		protected override void OnSearchProgress(SearchArgs args)
		{
			base.OnSearchProgress(args);

			double percent = (double)args.CurrentCount / (double)args.TotalCount;

			browser.StatusText = String.Format("[{0:#0.#%}] - {1}", percent, args.Info.Url);
		}

		protected override void OnResultClick(WebInfo info)
		{
			base.OnResultClick(info);

			browser.Navigate(info.Url, ref missing, ref missing, ref missing, ref missing);
		}

		#endregion
	}
}
