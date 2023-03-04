namespace INA_Generations
{
	public class Specimen
	{
		
		[DisplayInGridView("xReal")]
		public double XReal;
		public long XInt;
		[DisplayInGridView("xBin")]
		public string XBin;
		[DisplayInGridView("F(x)")]
		public double Fx;

		public Specimen()
		{
			XReal = Singleton.RandomXReal();
			XInt = MathHelper.XRealToXInt(XReal);
			XBin = MathHelper.XIntToXBin(XInt);
			Fx = MathHelper.Fx(XReal);
		}
		
		public Specimen(double xReal)
		{
			XReal = xReal;
			XInt = MathHelper.XRealToXInt(xReal);
			XBin = MathHelper.XIntToXBin(XInt);
			Fx = MathHelper.Fx(xReal);
		}
		
		public Specimen(long xInt)
		{
			XInt = xInt;
			XBin = MathHelper.XIntToXBin(XInt);
			XReal = MathHelper.XIntToXReal(xInt);
			Fx = MathHelper.Fx(XReal);
		}
		
		public Specimen(string xBin)
		{
			XBin = xBin;
			XInt = MathHelper.XBinToXInt(XBin);
			XReal = MathHelper.XIntToXReal(XInt);
			Fx = MathHelper.Fx(XReal);
		}
	}
}