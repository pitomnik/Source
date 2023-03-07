using NAudio.Dsp;
using NAudio.Wave;
using System;
using System.Diagnostics;

namespace DiscoLights.Audio
{
	#region AudioLights Class

	public class AudioLights : IDisposable
	{
		#region Constants

		public const int DefaultDeviceNumber = -1;

		#endregion

		#region Private Members

		private bool disposed;
		private readonly WaveIn wave;
		private readonly double[] oldIntensity;
		private readonly SampleAggregator aggregator;
		private event EventHandler<LightsEventArgs> lightsChanged;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="AudioLights"/> class.
		/// </summary>
		/// <param name="parentWindow">The window.</param>
		/// <param name="deviceNumber">The device.</param>
		/// <param name="numberOfLights">The number of lights.</param>
		/// <param name="noiseLevel">The noise level.</param>
		public AudioLights(IntPtr parentWindow, int deviceNumber, int numberOfLights, double noiseLevel)
		{
			wave = new WaveIn(parentWindow);

			if (deviceNumber != DefaultDeviceNumber)
			{
				wave.DeviceNumber = deviceNumber;
			}

			NumberOfLights = numberOfLights;
			NoiseLevel = noiseLevel;

			wave.DataAvailable += Wave_DataAvailable;
			wave.RecordingStopped += Wave_RecordingStopped;

			oldIntensity = new double[numberOfLights];

			aggregator = new SampleAggregator();

			aggregator.PerformFFT = true;
			aggregator.FftCalculated += Aggregator_FftCalculated;
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="AudioLights"/> is reclaimed by garbage collection.
		/// </summary>
		~AudioLights()
		{
			Dispose(false);
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets the device number.
		/// </summary>
		public int DeviceNumber
		{
			[DebuggerNonUserCode]
			get { return wave.DeviceNumber; }
		}

		/// <summary>
		/// Gets the number of lights.
		/// </summary>
		public int NumberOfLights
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the noise level.
		/// </summary>
		public double NoiseLevel
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="AudioLights"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		public bool Enabled
		{
			get;
			private set;
		}
		
		#endregion

		#region Public Methods

		/// <summary>
		/// Enables this instance.
		/// </summary>
		public void Enable()
		{
			if (!Enabled)
			{
				wave.StartRecording();
				Enabled = true;
			}
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		public void Disable()
		{
			if (Enabled)
			{
				wave.StopRecording();
				Enabled = false;
			}
		}

		#endregion

		#region Public Events

		/// <summary>
		/// Occurs when lights changed.
		/// </summary>
		public event EventHandler<LightsEventArgs> LightsChanged
		{
			[DebuggerNonUserCode]
			add { lightsChanged += value; }
			[DebuggerNonUserCode]
			remove { lightsChanged -= value; }
		}

		#endregion

		#region Private Events

		/// <summary>
		/// Handles the DataAvailable event of the Wave control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="NAudio.Wave.WaveInEventArgs"/> instance containing the event data.</param>
		private void Wave_DataAvailable(object sender, WaveInEventArgs e)
		{
			if (e != null)
			{
				OnDataAvailable(e.Buffer, e.BytesRecorded);
			}
		}

		/// <summary>
		/// Handles the RecordingStopped event of the Wave control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Wave_RecordingStopped(object sender, EventArgs e)
		{
			Enabled = false;
		}

		/// <summary>
		/// Handles the FftCalculated event of the Aggregator control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="DiscoLights.FftEventArgs"/> instance containing the event data.</param>
		private void Aggregator_FftCalculated(object sender, FftEventArgs e)
		{
			if (e != null)
			{
				OnFftCalculated(e.Result);
			}
		}

		#endregion

		#region Private Events

		/// <summary>
		/// Called when data available.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="bytesRecorded">The bytes recorded.</param>
		private void OnDataAvailable(byte[] buffer, int bytesRecorded)
		{
			for (int index = 0; index < bytesRecorded; index += 2)
			{
				short sample = (short)((buffer[index + 1] << 8) | buffer[index + 0]);
				float sample32 = sample / 32768f;

				aggregator.Add(sample32);
			}
		}

		/// <summary>
		/// Called when FFT calculated.
		/// </summary>
		/// <param name="fftResults">The FFT results.</param>
		public void OnFftCalculated(Complex[] fftResults)
		{
			int resultsHalfLength = fftResults.Length / 2;
			double[] newIntensity = new double[NumberOfLights];

			for (int i = 0; i < resultsHalfLength; i++)
			{
				int band = (int)((double)i / resultsHalfLength * newIntensity.Length);

				newIntensity[band] += (float)Math.Sqrt((double)(fftResults[i].X * fftResults[i].X + fftResults[i].Y * fftResults[i].Y));
			}

			OnLightsChanged(newIntensity);

			newIntensity.CopyTo(oldIntensity, 0);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}

			if (disposing)
			{
				try
				{
					wave.Dispose();
				}
				catch
				{
					// do nothing
				}
			}

			disposed = true;
		}

		/// <summary>
		/// Called when lights changed.
		/// </summary>
		/// <param name="newIntensity">The new intensity.</param>
		private void OnLightsChanged(double[] newIntensity)
		{
			if (lightsChanged == null)
			{
				return;
			}

			bool[] states = new bool[oldIntensity.Length];

			for (int i = 0; i < NumberOfLights; i++)
			{
				states[i] = newIntensity[i] - oldIntensity[i] > NoiseLevel;
			}

			lightsChanged(this, new LightsEventArgs(states));
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		#endregion
	}

	#endregion

	#region Event Handlers

	public delegate void LightsChangedEventHandler(object sender, LightsEventArgs e);

	#endregion

	#region LightsEventArgs Class

	public class LightsEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LightsEventArgs"/> class.
		/// </summary>
		/// <param name="states">The states.</param>
		public LightsEventArgs(bool[] states)
		{
			States = states;
		}

		/// <summary>
		/// Gets the states.
		/// </summary>
		public bool[] States { get; private set; }
	}

	#endregion
}
