namespace INA_Generations
{
	public class Specimen
	{
		public double xReal;
		public long xInt_xReal;
		public string xBin_xInt;
		public long xInt_xBin;
		public double xReal_xInt;
		public double FxReal;

		public Specimen()
		{
			xReal = Singleton.RandomXReal();
			xInt_xReal = MathHelper.XRealToXInt(xReal);
			xBin_xInt = MathHelper.XIntToXBin(xInt_xReal);
			xInt_xBin = MathHelper.XBinToXInt(xBin_xInt);
			xReal_xInt = MathHelper.XIntToXReal(xInt_xBin);
			FxReal = MathHelper.Fx(xReal_xInt);
		}
		
		public Specimen(double xReal)
		{
			this.xReal = xReal;
			xInt_xReal = MathHelper.XRealToXInt(xReal);
			xBin_xInt = MathHelper.XIntToXBin(xInt_xReal);
			xInt_xBin = MathHelper.XBinToXInt(xBin_xInt);
			xReal_xInt = MathHelper.XIntToXReal(xInt_xBin);
			FxReal = MathHelper.Fx(xReal_xInt);
		}
		
		public Specimen(long xInt)
		{
			xInt_xReal = xInt;
			xBin_xInt = MathHelper.XIntToXBin(xInt_xReal);
			xInt_xBin = MathHelper.XBinToXInt(xBin_xInt);
			xReal_xInt = MathHelper.XIntToXReal(xInt_xBin);
			this.xReal = xReal_xInt;
			FxReal = MathHelper.Fx(xReal_xInt);
		}
		
		public Specimen(string xBin)
		{
			xBin_xInt = xBin;
			xInt_xBin = MathHelper.XBinToXInt(xBin_xInt);
			xReal_xInt = MathHelper.XIntToXReal(xInt_xBin);
			xReal = xReal_xInt;
			xInt_xReal = MathHelper.XRealToXInt(xReal);
			FxReal = MathHelper.Fx(xReal_xInt);
		}
	}
}