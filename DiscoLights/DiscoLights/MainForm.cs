using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DiscoLights.Audio;
using DiscoLights.Properties;
using DiscoLights.Shared;

namespace DiscoLights
{
	public partial class MainForm : Form
	{
		#region Private Members

		private readonly string[] devices;
		private List<Image> offImages;
		private List<Image> onImages;
		private PictureBox[] lights;
		private AudioLights audio;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MainForm"/> class.
		/// </summary>
		public MainForm()
		{
			InitializeComponent();

            MinimumSize = new Size(0, 0);
            Text = Application.ProductName;

			devices = DeviceManager.GetDevices();

			int deviceNumber = AppConfig.Instance.ReadValue<int>(ConfigKey.DeviceNumber);

			if (devices.Length > 0)
			{
				if (deviceNumber < 0 || deviceNumber > devices.Length - 1)
				{
					deviceNumber = 0;
				}
			}
			else
			{
				deviceNumber = AudioLights.DefaultDeviceNumber;
			}

			CreateDiscoLights(deviceNumber);

			CreateWindow();

			RelocateWindow(Screen.PrimaryScreen.Bounds);

			TopMost = AppConfig.Instance.ReadValue<bool>(ConfigKey.AlwaysOnTop);
		}

		#endregion

		#region Private Events

		/// <summary>
		/// Handles the Shown event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void MainForm_Shown(object sender, EventArgs e)
		{
			if (devices.Length > 0)
			{
				ShowWelcomeMessage();

                if (AppConfig.Instance.ReadValue(ConfigKey.Enabled, true))
                {
                    EnableDiscoLights();
                }
			}
			else
			{
				MessageBox.Show(Resources.NoSourceFound, Application.ProductName,
					MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		/// <summary>
		/// Handles the KeyUp event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
		private void MainForm_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				CloseWindow();
			}
		}

		/// <summary>
		/// Handles the FormClosing event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			OnFormClosing();
		}

		/// <summary>
		/// Handles the LightsChanged event of the Audio control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DiscoLights.Audio.LightsEventArgs"/> instance containing the event data.</param>
		private void Audio_LightsChanged(object sender, LightsEventArgs e)
		{
			if (e != null)
			{
				OnLightsChanged(e.States);
			}
		}

		/// <summary>
		/// Handles the MouseDown event of the Control control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		private void Control_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				NativeMethods.ReleaseCapture();
				NativeMethods.SendMessage(Handle, NativeMethods.WM_NCLBUTTONDOWN, new IntPtr(NativeMethods.HTCAPTION), IntPtr.Zero);
			}
		}

		/// <summary>
		/// Handles the Click event of the Size control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Size_Click(object sender, EventArgs e)
		{
			MenuItem item = sender as MenuItem;

			if (item != null)
			{
				OnSizeChanged(item);
			}
		}

		/// <summary>
		/// Handles the Click event of the Source control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Source_Click(object sender, EventArgs e)
		{
			MenuItem item = sender as MenuItem;

			if (item != null)
			{
				OnSourceChanged(item);
			}
		}

		/// <summary>
		/// Handles the Click event of the AlwaysOnTop control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void AlwaysOnTop_Click(object sender, EventArgs e)
		{
			MenuItem item = sender as MenuItem;

			if (item != null)
			{
				OnAlwaysOnTopChanged(item);
			}
		}

		/// <summary>
		/// Handles the Click event of the Horizontal control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Horizontal_Click(object sender, EventArgs e)
		{
			MenuItem item = sender as MenuItem;

			if (item != null)
			{
				OnHorizontalChanged(item);
			}
		}

		/// <summary>
		/// Handles the Click event of the Enabled control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Enabled_Click(object sender, EventArgs e)
		{
			MenuItem menuItem = sender as MenuItem;

			if (menuItem != null)
			{
				OnMenuEnabled(menuItem);
			}
		}

		/// <summary>
		/// Handles the Click event of the Donate control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Donate_Click(object sender, EventArgs e)
		{
			Process.Start("http://www.gasanov.net/Donation.asp");
		}

		/// <summary>
		/// Handles the Click event of the About control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void About_Click(object sender, EventArgs e)
		{
			ShowAboutWindow();
		}

		/// <summary>
		/// Handles the Click event of the Exit control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Exit_Click(object sender, EventArgs e)
		{
			CloseWindow();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Creates the window.
		/// </summary>
		private void CreateWindow()
		{
			LightSize size = AppConfig.Instance.ReadValue<LightSize>(ConfigKey.LightSize, LightSize.X64);
			Orientation orientation = AppConfig.Instance.ReadValue<Orientation>(ConfigKey.Orientation, Orientation.Horizontal);

			CreateWindow(size, orientation);
		}

		/// <summary>
		/// Creates the window.
		/// </summary>
		/// <param name="size">The size.</param>
		private void CreateWindow(LightSize size)
		{
			Orientation orientation = AppConfig.Instance.ReadValue<Orientation>(ConfigKey.Orientation, Orientation.Horizontal);

			CreateWindow(size, orientation);
		}

		/// <summary>
		/// Creates the window.
		/// </summary>
		/// <param name="orientation">The orientation.</param>
		private void CreateWindow(Orientation orientation)
		{
			LightSize size = AppConfig.Instance.ReadValue<LightSize>(ConfigKey.LightSize, LightSize.X64);

			CreateWindow(size, orientation);
		}

		/// <summary>
		/// Creates the window.
		/// </summary>
		private void CreateWindow(LightSize size, Orientation orientation)
		{
			if (lights != null)
			{
				for (int i = 0; i < audio.NumberOfLights; i++)
				{
					Controls.Remove(lights[i]);

					offImages[i].Dispose();
					onImages[i].Dispose();

					lights[i].Dispose();
				}
			}

			offImages = ImageLoader.Instance.LoadImages(ImageLoader.Colors, LightState.Off, size);
			onImages = ImageLoader.Instance.LoadImages(ImageLoader.Colors, LightState.On, size);

			lights = CreateControls(offImages, orientation, size);

			ResizeWindow(lights, orientation);
		}

		/// <summary>
		/// Creates the controls.
		/// </summary>
		/// <param name="images">The images.</param>
		/// <param name="orientation">The orientation.</param>
		/// <param name="size">The size.</param>
		/// <returns></returns>
		private PictureBox[] CreateControls(List<Image> images, Orientation orientation, LightSize size)
		{
			PictureBox[] controls = new PictureBox[images.Count];
			ContextMenu menu = CreateContextMenu();

			for (int i = 0; i < images.Count; i++)
			{
				Image image = images[i];
				PictureBox control = new PictureBox();

				control.Image = image;
				control.Size = image.Size;

				if (orientation == Orientation.Horizontal)
				{
					control.Top = 0;
					control.Left = image.Width * i;
				}
				else
				{
					control.Top = image.Height * i;
					control.Left = 0;
				}

				control.ContextMenu = menu;

				control.MouseDown += new MouseEventHandler(Control_MouseDown);

				controls[i] = control;
			}

			Controls.AddRange(controls);

			return controls;
		}

		/// <summary>
		/// Creates the context menu.
		/// </summary>
		/// <returns></returns>
		private ContextMenu CreateContextMenu()
		{
			if (ContextMenu != null)
			{
				return ContextMenu;
			}

			ContextMenu menu = new ContextMenu();

			menu.MenuItems.Add(CreateSizeMenu());

			menu.MenuItems.Add(CreateSourceMenu());

			menu.MenuItems.Add(new MenuItem("-"));

			menu.MenuItems.Add(CreateAlwaysOnTopMenu());

			menu.MenuItems.Add(CreateHorizontalMenu());

			menu.MenuItems.Add(CreateEnabledMenu());

			menu.MenuItems.Add(new MenuItem("-"));

			menu.MenuItems.Add(CreateDonateMenu());

			menu.MenuItems.Add(CreateAboutMenu());

			menu.MenuItems.Add(new MenuItem("-"));

			menu.MenuItems.Add(CreateExitMenu());

			ContextMenu = menu;

			return menu;
		}

		/// <summary>
		/// Creates the size menu.
		/// </summary>
		private MenuItem CreateSizeMenu()
		{
			MenuItem menu = new MenuItem();
			LightSize size = AppConfig.Instance.ReadValue<LightSize>(ConfigKey.LightSize, LightSize.X64);

			menu.Text = Resources.Size;

			foreach (LightSize value in Enum.GetValues(typeof(LightSize)))
			{
				MenuItem item = new MenuItem();

				item.Text = Resources.ResourceManager.GetString(value.ToString());
				item.Tag = value;
				item.RadioCheck = true;
				if (value == size)
				{
					item.Checked = true;
				}
				item.Click += Size_Click;

				menu.MenuItems.Add(item);
			}

			return menu;
		}

		/// <summary>
		/// Creates the source menu.
		/// </summary>
		/// <returns></returns>
		private MenuItem CreateSourceMenu()
		{
			MenuItem menu = new MenuItem(Resources.Source);
			int device = AppConfig.Instance.ReadValue<int>(ConfigKey.DeviceNumber);

			for (int i = 0; i < devices.Length; i++)
			{
				MenuItem item = new MenuItem();

				item.Text = devices[i];
				item.RadioCheck = true;
				if (i == device)
				{
					item.Checked = true;
				}
				item.Click += Source_Click;

				menu.MenuItems.Add(item);
			}

			return menu;
		}

		/// <summary>
		/// Creates the always on top menu.
		/// </summary>
		/// <returns></returns>
		private MenuItem CreateAlwaysOnTopMenu()
		{
			MenuItem menu = new MenuItem();

			menu.Text = Resources.AlwaysOnTop;
			menu.Checked = AppConfig.Instance.ReadValue<bool>(ConfigKey.AlwaysOnTop);
			menu.Click += AlwaysOnTop_Click;

			return menu;
		}

		/// <summary>
		/// Creates the horizontal menu.
		/// </summary>
		/// <returns></returns>
		private MenuItem CreateHorizontalMenu()
		{
			MenuItem menu = new MenuItem();

			menu.Text = Resources.Horizontal;
			menu.Checked = AppConfig.Instance.ReadValue<Orientation>(ConfigKey.Orientation) == Orientation.Horizontal;
			menu.Click += Horizontal_Click;

			return menu;
		}

		/// <summary>
		/// Creates the enabled menu.
		/// </summary>
		/// <returns></returns>
		private MenuItem CreateEnabledMenu()
		{
			MenuItem menu = new MenuItem();

			menu.Text = Resources.Enabled;
			menu.Enabled = devices.Length > 0;
			menu.Checked = menu.Enabled && AppConfig.Instance.ReadValue<bool>(ConfigKey.Enabled, true);
			menu.Click += Enabled_Click;

			return menu;
		}

		/// <summary>
		/// Creates the donate menu.
		/// </summary>
		/// <returns></returns>
		private MenuItem CreateDonateMenu()
		{
			MenuItem menu = new MenuItem();

			menu.Text = Resources.Donate;
			menu.Click += Donate_Click;

			return menu;
		}

		/// <summary>
		/// Creates the about menu.
		/// </summary>
		/// <returns></returns>
		private MenuItem CreateAboutMenu()
		{
			MenuItem menu = new MenuItem();

			menu.Text = Resources.About;
			menu.Click += About_Click;

			return menu;
		}

		/// <summary>
		/// Creates the exit menu.
		/// </summary>
		/// <returns></returns>
		private MenuItem CreateExitMenu()
		{
			MenuItem menu = new MenuItem();

			menu.Text = Resources.Exit;
			menu.Click += Exit_Click;

			return menu;
		}

		/// <summary>
		/// Resizes the window.
		/// </summary>
		/// <param name="controls">The controls.</param>
		/// <param name="orientation">The orientation.</param>
		private void ResizeWindow(Control[] controls, Orientation orientation)
		{
			if (controls.Length == 0)
			{
				Text = Application.ProductName;
				FormBorderStyle = FormBorderStyle.FixedToolWindow;
			}
			else
			{
				Control lastControl = controls[controls.Length - 1];

				if (orientation == Orientation.Horizontal)
				{
					Width = lastControl.Width * controls.Length;
					Height = lastControl.Height;
				}
				else
				{
					Width = lastControl.Width;
                    Height = lastControl.Height * controls.Length;
				}
			}
		}

		/// <summary>
		/// Relocates the window.
		/// </summary>
		/// <param name="bounds">The bounds.</param>
		private void RelocateWindow(Rectangle bounds)
		{
			int top = AppConfig.Instance.ReadValue<int>(ConfigKey.WindowTop);
			int left = AppConfig.Instance.ReadValue<int>(ConfigKey.WindowLeft);

			if (top < bounds.Top || top > bounds.Bottom)
			{
				top = bounds.Top;
			}

			if (left < bounds.Left || left > bounds.Right)
			{
				left = bounds.Left;
			}

			Top = top;
			Left = left;
		}

		/// <summary>
		/// Creates the disco lights.
		/// </summary>
		/// <param name="deviceNumber">The device number.</param>
		private void CreateDiscoLights(int deviceNumber)
		{
			if (audio != null)
			{
				audio.LightsChanged -= Audio_LightsChanged;

				audio.Dispose();

				audio = null;
			}

			audio = new AudioLights(Handle, deviceNumber, ImageLoader.Colors.Length,
				AppConfig.Instance.ReadValue<double>(ConfigKey.NoiseLevel, 0));

			audio.LightsChanged += Audio_LightsChanged;
		}

		/// <summary>
		/// Enables the disco lights.
		/// </summary>
		/// <returns></returns>
		private bool EnableDiscoLights()
		{
			Application.DoEvents();

			Cursor = Cursors.WaitCursor;

			try
			{
				audio.Enable();
			}
			catch (Exception ex)
			{
				LogMaster.Instance.Error(String.Format("Failed to enable lights on '{0}'.", devices[audio.DeviceNumber]), ex);
			}
			finally
			{
				Cursor = Cursors.Default;
			}

			return audio.Enabled;
		}

		/// <summary>
		/// Shows the welcome message.
		/// </summary>
		private void ShowWelcomeMessage()
		{
			string message = String.Format(Resources.Welcome, Application.ProductName, devices[audio.DeviceNumber]);

			NativeMethods.SHMessageBoxCheck(Handle, message, Application.ProductName,
				NativeMethods.MessageBoxCheckFlags.MB_OK | NativeMethods.MessageBoxCheckFlags.MB_ICONINFORMATION,
				(int)MessageBoxDefaultButton.Button1, String.Concat(Application.ProductName, ".", "Welcome"));
		}

		/// <summary>
		/// Called when lights changed.
		/// </summary>
		/// <param name="states">The states.</param>
		private void OnLightsChanged(bool[] states)
		{
			for (int i = 0; i < audio.NumberOfLights; i++)
			{
				lights[i].Image = states[i] ? onImages[i] : offImages[i];
			}

			Refresh();
		}

		/// <summary>
		/// Called when size changed.
		/// </summary>
		/// <param name="menuItem">The menu item.</param>
		private void OnSizeChanged(MenuItem menuItem)
		{
			LightSize size = (LightSize)menuItem.Tag;

			AppConfig.Instance.WriteValue(ConfigKey.LightSize, size);

			foreach (MenuItem item in menuItem.Parent.MenuItems)
			{
				item.Checked = false;
			}

			menuItem.Checked = true;

			CreateWindow(size);
		}

		/// <summary>
		/// Called when source changed.
		/// </summary>
		/// <param name="menuItem">The menu item.</param>
		private void OnSourceChanged(MenuItem menuItem)
		{
			int deviceNumber = Array.IndexOf(devices, menuItem.Text);

			if (deviceNumber == -1)
			{
				LogMaster.Instance.WarnFormat("Unexpected device: '{0}'.", menuItem.Text);

				return;
			}

			bool wasEnabled = audio.Enabled;

			CreateDiscoLights(deviceNumber);

			if (wasEnabled)
			{
				EnableDiscoLights();
			}

			foreach (MenuItem item in menuItem.Parent.MenuItems)
			{
				item.Checked = false;
			}

			menuItem.Checked = true;

			AppConfig.Instance.WriteValue(ConfigKey.DeviceNumber, deviceNumber);
		}

		/// <summary>
		/// Called when always on top changed.
		/// </summary>
		/// <param name="menuItem">The menu item.</param>
		private void OnAlwaysOnTopChanged(MenuItem menuItem)
		{
			menuItem.Checked = !menuItem.Checked;

			TopMost = menuItem.Checked;

			AppConfig.Instance.WriteValue(ConfigKey.AlwaysOnTop, TopMost);
		}

		/// <summary>
		/// Called when horizontal changed.
		/// </summary>
		/// <param name="menuItem">The menu item.</param>
		private void OnHorizontalChanged(MenuItem menuItem)
		{
			menuItem.Checked = !menuItem.Checked;
			Orientation orientation = menuItem.Checked ?
				Orientation.Horizontal : Orientation.Vertical;

			CreateWindow(orientation);

			AppConfig.Instance.WriteValue(ConfigKey.Orientation, orientation);
		}

		/// <summary>
		/// Called when menu enabled.
		/// </summary>
		/// <param name="menuItem">The menu item.</param>
		private void OnMenuEnabled(MenuItem menuItem)
		{
			if (menuItem.Checked) // was checked
			{
				audio.Disable();
			}
			else
			{
				EnableDiscoLights();
			}

			menuItem.Checked = audio.Enabled;

			AppConfig.Instance.WriteValue(ConfigKey.Enabled, audio.Enabled);
		}

		/// <summary>
		/// Shows the about window.
		/// </summary>
		private void ShowAboutWindow()
		{
			bool topmost = TopMost;

			if (topmost)
			{
				TopMost = false;
			}

			try
			{
				using (AboutForm form = new AboutForm())
				{
					form.ShowDialog();
				}
			}
			finally
			{
				if (topmost)
				{
					TopMost = true;
				}
			}
		}

		/// <summary>
		/// Called when form closing.
		/// </summary>
		private void OnFormClosing()
		{
			AppConfig.Instance.WriteValue(ConfigKey.WindowTop, Top);
			AppConfig.Instance.WriteValue(ConfigKey.WindowLeft, Left);
		}

		/// <summary>
		/// Closes the window.
		/// </summary>
		private void CloseWindow()
		{
			Close();
		}

		#endregion
	}
}
