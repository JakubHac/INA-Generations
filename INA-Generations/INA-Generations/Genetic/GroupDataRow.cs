namespace INA_Generations
{
	public class GroupDataRow
	{
		public static GroupDataRow Empty = new GroupDataRow();

		public GroupDataRow()
		{
		}

		public GroupDataRow(long index, double xRealValue, double percentValue)
		{
			Index = index;
			this.xRealValue = xRealValue;
			this.xBinValue = MathHelper.XIntToXBin(MathHelper.XRealToXInt(xRealValue));
			this.FxValue = MathHelper.Fx(xRealValue);
			PercentValue = percentValue;
		}


		public long Index = 0;
		public double xRealValue = 0;
		public string xBinValue = "";
		public double FxValue = 0;
		public double PercentValue = 0;
		
		public (string, string) N => ("N", Index.ToString());
		public (string, string) xReal => ("xReal", xRealValue.ToString());
		public (string, string) xBin => ("xBin", xBinValue);
		public (string, string) Fx => ("F(x)", FxValue.ToString());
		public (string, string) Percent => ("%", PercentValue.ToString());
	}
}