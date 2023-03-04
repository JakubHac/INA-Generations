namespace INA_Generations
{
	public class AnalysisDataRow
	{
		public long NValue;
		public long TValue;
		public double PKValue;
		public double PMValue;
		public double AvgFXValue;

		public AnalysisDataRow(long nValue, long value, double pkValue, double pmValue, double avgFxValue)
		{
			NValue = nValue;
			TValue = value;
			PKValue = pkValue;
			PMValue = pmValue;
			AvgFXValue = avgFxValue;
		}

		[DisplayInGridView("N")]
		public string N => NValue.ToString("D");
		[DisplayInGridView("T")]
		public string T => TValue.ToString("D");
		[DisplayInGridView("PK")]
		public string PK => PKValue.ToString("0." + new string('#', 99));
		[DisplayInGridView("PM")]
		public string PM => PMValue.ToString("0." + new string('#', 99));
		[DisplayInGridView("Koszt")]
		public string Cost => (NValue * TValue).ToString("D");
		[DisplayInGridView("Avg F(x)")]
		public string AvgFX => AvgFXValue.ToString("0." + new string('#', 99));
	}
}