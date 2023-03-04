namespace INA_Generations
{
	public class GroupDataRow
	{
		public GroupDataRow()
		{
		}

		public GroupDataRow(long index, double xRealValue, double percentValue)
		{
			Index = index;
			XRealValue = xRealValue;
			XBinValue = MathHelper.XIntToXBin(MathHelper.XRealToXInt(xRealValue));
			FxValue = MathHelper.Fx(xRealValue);
			PercentValue = percentValue;
		}


		private long Index;
		private double XRealValue;
		public string XBinValue = "";
		private double FxValue;
		private double PercentValue;
		
		[DisplayInGridView("N")]
		public string N => Index.ToString();
		[DisplayInGridView("xReal")]
		public string XReal => XRealValue.ToString();
		[DisplayInGridView("xBin")]
		public string XBin => XBinValue;
		[DisplayInGridView("F(x)")]
		public string Fx => FxValue.ToString();
		[DisplayInGridView("%")]
		public string Percent => PercentValue.ToString();
	}
}