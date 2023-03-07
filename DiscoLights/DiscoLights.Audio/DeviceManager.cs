using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace DiscoLights.Audio
{
	public sealed class DeviceManager
	{
		#region Constants

		private const int maxDeviceNameLength = 31;

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the devices.
		/// </summary>
		/// <returns></returns>
		public static string[] GetDevices()
		{
			List<string> devices = new List<string>(WaveIn.DeviceCount);

			for (int i = 0; i < WaveIn.DeviceCount; i++)
			{
				WaveInCapabilities capabilities;
				
				try
				{
					capabilities = WaveIn.GetCapabilities(i);
				}
				catch (Exception ex)
				{
					LogMaster.Instance.Error(String.Format("Failed to get capabilities of device #{0}.", i), ex);

					continue;
				}

				string deviceName = String.IsNullOrEmpty(capabilities.ProductName) ?
					String.Format("Unknown device #{0}", i) : capabilities.ProductName;

				if (deviceName.Length == maxDeviceNameLength)
				{
					deviceName += "...";
				}

				devices.Add(deviceName);
			}

			return devices.ToArray();
		}

		#endregion
	}
}
