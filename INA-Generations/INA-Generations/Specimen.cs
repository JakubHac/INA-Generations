namespace INA_Generations
{
	public class Specimen
	{
		public double xReal;
		public long XInt;
		public string XBin;
		public double Fx;

		public Specimen()
		{
			xReal = Singleton.RandomXReal();
			XInt = MathHelper.XRealToXInt(xReal);
			XBin = MathHelper.XIntToXBin(XInt);
			Fx = MathHelper.Fx(xReal);
		}
		
		public Specimen(double xReal)
		{
			this.xReal = xReal;
			XInt = MathHelper.XRealToXInt(xReal);
			XBin = MathHelper.XIntToXBin(XInt);
			Fx = MathHelper.Fx(xReal);
		}
		
		public Specimen(long xInt)
		{
			XInt = xInt;
			XBin = MathHelper.XIntToXBin(XInt);
			xReal = MathHelper.XIntToXReal(xInt);
			Fx = MathHelper.Fx(xReal);
		}
		
		public Specimen(string xBin)
		{
			XBin = xBin;
			XInt = MathHelper.XBinToXInt(XBin);
			xReal = MathHelper.XIntToXReal(XInt);
			Fx = MathHelper.Fx(xReal);
		}
	}
}