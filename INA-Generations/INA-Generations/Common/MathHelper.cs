using System;
using Eto.Forms;

namespace INA_Generations
{
	public static class MathHelper
	{
		public static double ConvertToRadians(double angle)
		{
			return (Math.PI / 180.0) * angle;
		}

		public static double SinAngle(double angle)
		{
			return Math.Sin(ConvertToRadians(angle));
		}
		
		public static double CosAngle(double angle)
		{
			return Math.Cos(ConvertToRadians(angle));
		}

		public static int Accuracy(double d)
		{
			return d switch
			{
				1.0 => 0,
				0.1 => 1,
				0.01 => 2,
				0.001 => 3
			};
		}

		public static long XBinToXInt(string xBin)
		{
			return Convert.ToInt64(xBin, 2);
		}

		public static double XIntToXReal(long xInt)
		{
			double trueXReal = ((Singleton.b - Singleton.a) * xInt) / (Math.Pow(2.0, Singleton.l) - 1.0) + Singleton.a;
			return Math.Round(trueXReal, Accuracy(Singleton.d));
		}
		
		public static double XBinToXReal(string xBin)
		{
			return XIntToXReal(XBinToXInt(xBin));
		}

		public static double Fx(double xReal)
		{
			return (xReal % 1.0) * (Math.Cos(20.0 * Math.PI * xReal) - Math.Sin(xReal));
		}

		public static long XRealToXInt(double xReal)
		{
			return (long)Math.Round((1.0 / (Singleton.b - Singleton.a)) * (xReal - Singleton.a) * ((Math.Pow(2.0, Singleton.l)) - 1.0));
		}

		public static string XIntToXBin(long xInt)
		{
			return Convert.ToString(xInt, 2).PadLeft(Singleton.l, '0');
		}
		
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