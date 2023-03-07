using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;

using TouchlessLib;

namespace RoboComp
{
	public partial class MainForm : Form
	{
		#region Private Members

		private readonly List<Keys> directionKeys;
		private readonly TouchlessMgr manager;
		private readonly string captureHome;
		private readonly Motor motor;

		#endregion

		#region Constructors

		public MainForm()
		{
			manager = new TouchlessMgr();
			captureHome = CreateCapturePath();
			motor = new Motor(AppConfig.CruiseSpeed, AppConfig.SpeedDelta, AppConfig.TurnStep);

			directionKeys = new List<Keys>();

			directionKeys.Add(Keys.Up);
			directionKeys.Add(Keys.Down);
			directionKeys.Add(Keys.Left);
			directionKeys.Add(Keys.Right);

			TopMost = !Debugger.IsAttached;

			InitializeComponent();
		}

		#endregion

		#region Private Events

		private void MainForm_Load(object sender, EventArgs e)
		{
			FindCamera();
			UpdateStatus();
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (!directionKeys.Contains(e.KeyCode))
			{
				return;
			}

			OnKeyDown(e.KeyCode);
		}

		private void MainForm_KeyUp(object sender, KeyEventArgs e)
		{
			if (!directionKeys.Contains(e.KeyCode))
			{
				return;
			}

			OnKeyUp(e.KeyCode);
		}

		private void Camera_OnImageCaptured(object sender, CameraEventArgs e)
		{
			if (e.Image != null)
			{
				OnImageCaptured(e.Image);
			}
		}

		#endregion

		#region Private Methods

		private string CreateCapturePath()
		{
			string workingFolder = Path.GetDirectoryName(Application.ExecutablePath);
			string captureFodler = @"Capture\" + Guid.NewGuid().ToString().Split('-')[0];

			return Path.Combine(workingFolder, captureFodler);
		}

		private void FindCamera()
		{
			Trace.WriteLine(String.Format("Found {0} camera(s).", manager.Cameras.Count));

			foreach (Camera camera in manager.Cameras)
			{
				string name = camera.ToString();

				Trace.WriteLine("\t" + name);

				if (AppConfig.Camera.Equals(name))
				{
					manager.CurrentCamera = camera;

					manager.CurrentCamera.CaptureWidth = picCamera.Width;
					manager.CurrentCamera.CaptureHeight = picCamera.Height;

					manager.CurrentCamera.OnImageCaptured += new EventHandler<CameraEventArgs>(Camera_OnImageCaptured);
				}
			}

			if (manager.CurrentCamera != null)
			{
				Trace.WriteLine(String.Format("Current camera: {0}.", manager.CurrentCamera));
			}

			if (!Directory.Exists(captureHome))
			{
				Directory.CreateDirectory(captureHome);
			}
		}

		private void UpdateStatus()
		{
			Text = String.Format("Motor: {0}. Camera: {1}. Direction: {2}. Left speed: {3}. Right speed: {4}.",
				motor.Started ? "On" : "Off", manager.CurrentCamera ?? (object)"None", motor.CurrentDirection, motor.LeftSpeed, motor.RightSpeed);

			Trace.WriteLine(Text);
		}

		private void OnKeyDown(Keys key)
		{
			switch (key)
			{
				case Keys.Up:
					motor.Forward();
					break;
				case Keys.Down:
					motor.Reverse();
					break;
				case Keys.Left:
					motor.TurnLeft();
					break;
				case Keys.Right:
					motor.TurnRight();
					break;
			}

			UpdateStatus();
		}

		private void OnKeyUp(Keys key)
		{
			if ((key == Keys.Up || key == Keys.Down) &&
				motor.Started)
			{
				motor.Stop();
			}
			else if (key == Keys.Left || key == Keys.Right)
			{
				motor.StopTurning();
			}

			UpdateStatus();
		}

		private void OnImageCaptured(Bitmap image)
		{
			if (AppConfig.Capture)
			{
				string file = Path.ChangeExtension(DateTime.Now.Ticks.ToString(), ".jpg");
				string path = Path.Combine(captureHome, file);

				try
				{
					image.Save(path, ImageFormat.Jpeg);
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.ToString());
				}
			}

			picCamera.Image = image;
		}

		#endregion
	}
}
