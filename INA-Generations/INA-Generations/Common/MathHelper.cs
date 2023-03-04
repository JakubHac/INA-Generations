using System;

namespace INA_Generations
{
	public static class MathHelper
	{
		/// <summary>
		/// Counts the number of decimal places in a double
		/// </summary>
		/// <param name="d">input value</param>
		/// <returns>number of decimal places</returns>
		public static int Accuracy(double d)
		{
			int count = 0;
			while (d % 1 != 0)
			{
				d *= 10;
				count++;
			}

			return count;
		}

		/// <summary>
		/// Converts xReal to xInt
		/// </summary>
		/// <param name="xBin">input value</param>
		/// <returns>xInt value calculated from input value</returns>
		public static long XBinToXInt(string xBin)
		{
			return Convert.ToInt64(xBin, 2);
		}

		/// <summary>
		/// Converts xInt to xReal
		/// </summary>
		/// <param name="xInt">input value</param>
		/// <returns>xReal value calculated from input value</returns>
		public static double XIntToXReal(long xInt)
		{
			double trueXReal = ((Singleton.b - Singleton.a) * xInt) / (Math.Pow(2.0, Singleton.l) - 1.0) + Singleton.a;
			return Math.Round(trueXReal, Accuracy(Singleton.d));
		}
		
		/// <summary>
		/// Converts xBin to xReal
		/// </summary>
		/// <param name="xBin">input value</param>
		/// <returns>xReal value calculated from input value</returns>
		public static double XBinToXReal(string xBin)
		{
			return XIntToXReal(XBinToXInt(xBin));
		}
		
		/// <summary>
		/// Converts xReal to xInt
		/// </summary>
		/// <param name="xReal">input value</param>
		/// <returns>xInt value calculated from input value</returns>
		public static long XRealToXInt(double xReal)
		{
			return (long)Math.Round((1.0 / (Singleton.b - Singleton.a)) * (xReal - Singleton.a) * ((Math.Pow(2.0, Singleton.l)) - 1.0));
		}

		/// <summary>
		/// Converts xInt to xBin
		/// </summary>
		/// <param name="xInt">input value</param>
		/// <returns>xBin value calculated from input value</returns>
		public static string XIntToXBin(long xInt)
		{
			return Convert.ToString(xInt, 2).PadLeft(Singleton.l, '0');
		}

		/// <summary>
		/// Calculates F(x) value for a given xReal
		/// </summary>
		/// <param name="xReal">input value</param>
		public static double Fx(double xReal)
		{
			return (xReal % 1.0) * (Math.Cos(20.0 * Math.PI * xReal) - Math.Sin(xReal));
		}
		
		/// <summary>
		/// Uses binary search to find the index of the closest value to the selection
		/// </summary>
		/// <param name="data">Search domain</param>
		/// <param name="selection">Value we are looking for</param>
		/// <returns>Index of the closest value</returns>
		public static long BinarySearchForQ(DataRow[] data, double selection) {
			long minNum = 0;
			long maxNum = data.Length - 1;

			if (maxNum == 0)
			{
				return maxNum;
			}
			
			if (data[minNum].QxValue >= selection)
			{
				return 0;
			}
			
			if (data[maxNum].QxValue <= selection)
			{
				return maxNum;
			}

			if (data[maxNum].QxValue >= selection && data[maxNum - 1].QxValue <= selection)
			{
				return maxNum;
			}

			long iter = 0;

			while (minNum <= maxNum) {
				long mid = (minNum + maxNum) / 2;
				if (minNum == maxNum)
				{
					return mid;
				}
				if (data[mid].QxValue >= selection)
				{
					if (data[mid - 1].QxValue <= selection)
					{
						return mid;
					}

					maxNum = mid;
				}
				else
				{
					minNum = mid;
				}

				iter++;
				if (iter > data.Length)
				{
					return -1;
				}
			}
			return 0;
		}
	}
}