using System;

namespace InterProcessObjectDemo
{
	[Serializable]
	public class Counter
	{
		private int number;

		public int GetNumber()
		{
			return ++number;
		}
	}
}
