using System;

namespace INA_Generations
{
	public class Specimen
	{
		public double LP;
		public double xReal;
		public long xInt_xReal;
		public string xBin_xInt;
		public long xInt_xBin;
		public double xReal_xInt;

		public Specimen(double a, double b, long lp, int l, double d)
		{
			LP = lp;
			int accuracy = d switch
			{
				1.0 => 0,
				0.1 => 1,
				0.01 => 2,
				0.001 => 3
			};
			xReal = Math.Round(Singleton.Random.NextDouble() * (b - a) + a, accuracy);
			xInt_xReal = (long)Math.Round((1.0 / (b - a)) * (xReal - a) * ((Math.Pow(2, l)) - 1));
			xBin_xInt = Convert.ToString(xInt_xReal, 2).PadLeft(l, '0');
			xInt_xBin = Convert.ToInt64(xBin_xInt, 2);
			xReal_xInt = Math.Round(((b-a)*xInt_xBin)/(Math.Pow(2,l) + 1)+a, accuracy);
		}
	}
}