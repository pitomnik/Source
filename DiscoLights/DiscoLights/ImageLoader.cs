using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using DiscoLights.Properties;

namespace DiscoLights
{
	internal class ImageLoader
	{
		#region Constants

		public static readonly LightColor[] Colors = new[]
			{
				LightColor.Red,
				LightColor.Amber,
				LightColor.Yellow,
				LightColor.Green,
				LightColor.Blue,
				LightColor.Ultraviolet,
				LightColor.White,
			};

		#endregion

		#region Private Members

		private static readonly ImageLoader instance = new ImageLoader();

		#endregion

		#region Constructors

		/// <summary>
		/// Prevents a default instance of the <see cref="ImageLoader"/> class from being created.
		/// </summary>
		private ImageLoader()
		{
			// do nothing
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets the instance.
		/// </summary>
		public static ImageLoader Instance
		{
			[DebuggerNonUserCode]
			get { return instance; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Loads the images.
		/// </summary>
		/// <param name="colors">The colors.</param>
		/// <param name="state">The state.</param>
		/// <param name="size">The size.</param>
		/// <returns></returns>
		public List<Image> LoadImages(IEnumerable<LightColor> colors, LightState state, LightSize size)
		{
			List<Image> images = new List<Image>();

			foreach (LightColor color in colors)
			{
				string name = String.Concat(color, state, size);
				object image = Images.ResourceManager.GetObject(name);

				if (image == null)
				{
					throw new Exception(String.Format("Missing resource '{0}'.", name));
				}


				images.Add((Image)image);
			}

			return images;
		}

		/// <summary>
		/// Loads the icons.
		/// </summary>
		/// <param name="colors">The colors.</param>
		/// <param name="state">The state.</param>
		/// <returns></returns>
		public List<Icon> LoadIcons(IEnumerable<LightColor> colors, LightState state)
		{
			List<Icon> icons = new List<Icon>();

			foreach (LightColor color in colors)
			{
				string name = String.Concat(color, state);
				object icon = Images.ResourceManager.GetObject(name);

				if (icon == null)
				{
					throw new Exception(String.Format("Missing resource '{0}'.", name));
				}


				icons.Add((Icon)icon);
			}

			return icons;
		}

		#endregion
	}
}
