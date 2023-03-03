﻿namespace INA_Generations
{
	public class AnalysisDataRow
	{
		public static AnalysisDataRow Empty = new();
		
		public long NValue = 0;
		public long TValue = 0;
		public double PKValue = 0;
		public double PMValue = 0;
		public double AvgFXValue = 0;

		public AnalysisDataRow(long nValue, long value, double pkValue, double pmValue, double avgFxValue)
		{
			NValue = nValue;
			TValue = value;
			PKValue = pkValue;
			PMValue = pmValue;
			AvgFXValue = avgFxValue;
		}

		public AnalysisDataRow()
		{
		}

		public (string, string) N => ("N", NValue.ToString("D"));
		public (string, string) T => ("T", TValue.ToString("D"));
		public (string, string) PK => ("PK", PKValue.ToString("0." + new string('#', 99)));
		public (string, string) PM => ("PM", PMValue.ToString("0." + new string('#', 99)));
		public (string, string) Cost => ("Koszt", (NValue * TValue).ToString("D"));
		public (string, string) AvgFX => ("Avg F(x)", AvgFXValue.ToString("0." + new string('#', 99)));
	}
}