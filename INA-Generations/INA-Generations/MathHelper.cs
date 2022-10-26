using System;

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
	}
}