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

		/// <summary>
		/// Randomly generated specimen
		/// </summary>
		public Specimen()
		{
			XReal = Singleton.RandomXReal();
			XInt = MathHelper.XRealToXInt(XReal);
			XBin = MathHelper.XIntToXBin(XInt);
			Fx = MathHelper.Fx(XReal);
		}
		
		/// <summary>
		/// Creates specimen from xReal
		/// </summary>
		/// <param name="xReal">xReal value of the specimen</param>
		public Specimen(double xReal)
		{
			XReal = xReal;
			XInt = MathHelper.XRealToXInt(xReal);
			XBin = MathHelper.XIntToXBin(XInt);
			Fx = MathHelper.Fx(xReal);
		}
		
		/// <summary>
		/// Creates specimen from xInt
		/// </summary>
		/// <param name="xInt">xInt value of the specimen</param>
		public Specimen(long xInt)
		{
			XInt = xInt;
			XBin = MathHelper.XIntToXBin(XInt);
			XReal = MathHelper.XIntToXReal(xInt);
			Fx = MathHelper.Fx(XReal);
		}
		
		/// <summary>
		/// Creates specimen from xBin
		/// </summary>
		/// <param name="xBin">xBin value of the specimen</param>
		public Specimen(string xBin)
		{
			XBin = xBin;
			XInt = MathHelper.XBinToXInt(XBin);
			XReal = MathHelper.XIntToXReal(XInt);
			Fx = MathHelper.Fx(XReal);
		}
	}
}