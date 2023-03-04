namespace INA_Generations
{
	public class GroupDataRow
	{
		public GroupDataRow(long index, double xRealValue, double percentValue)
		{
			Index = index;
			XRealValue = xRealValue;
			XBinValue = MathHelper.XIntToXBin(MathHelper.XRealToXInt(xRealValue));
			FxValue = MathHelper.Fx(xRealValue);
			PercentValue = percentValue;
		}
		
		[DisplayInGridView("N")]
		public long Index;
		[DisplayInGridView("xReal")]
		public double XRealValue;
		[DisplayInGridView("xBin")]
		public string XBinValue = "";
		[DisplayInGridView("F(x)")]
		public double FxValue;
		[DisplayInGridView("%",-1)]
		public string PercentString => (PercentValue / 100.0).ToString("P");
		
		public double PercentValue;
	}
}