using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace DiscoLights.Tests
{
	[TestFixture]
	public class ImageLoaderTester
	{
		[Test]
		public void LoadEveryImage()
		{
			foreach (LightColor color in Enum.GetValues(typeof(LightColor)))
			{
				foreach (LightState state in Enum.GetValues(typeof(LightState)))
				{
					foreach (LightSize size in Enum.GetValues(typeof(LightSize)))
					{
						LightColor[] colors = new LightColor[] { color };
						List<Image> images = ImageLoader.Instance.LoadImages(colors, state, size);

						Assert.AreEqual(colors.Length, images.Count);
					}
				}
			}
		}

		[Test]
		public void LoadEveryIcon()
		{
			foreach (LightColor color in Enum.GetValues(typeof(LightColor)))
			{
				foreach (LightState state in Enum.GetValues(typeof(LightState)))
				{
					LightColor[] colors = new LightColor[] { color };
					List<Icon> icons = ImageLoader.Instance.LoadIcons(colors, state);

					Assert.AreEqual(colors.Length, icons.Count);
				}
			}
		}
	}
}
