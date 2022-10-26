namespace INA_Generations
{
	public class DataRow
	{
		public static DataRow Empty = new DataRow();

		public Specimen OriginalSpecimen = null;
		public double SelectionRandom;
		public double ParentRandom;

		public DataRow(Specimen originalSpecimen)
		{
			OriginalSpecimen = originalSpecimen;
		}

		public DataRow()
		{
		}


		public void RandomizeSelection()
		{
			SelectionRandom = Singleton.GetRandomWithRoulette();
		}
		
		public void RandomizeParenting()
		{
			ParentRandom = Singleton.GetRandomWithRoulette();
		}

		public (string, string) N => ("N", OriginalSpecimen?.LP.ToString());
		public (string, string) xReal => ("xReal",OriginalSpecimen?.xReal.ToString());
		public (string, string) Fx => ("F(x)",OriginalSpecimen?.FxReal.ToString("N20").TrimEnd('0'));
		public (string, string) Gx => ("G(x)", GxValue.ToString("N20").TrimEnd('0'));
		public (string, string) Px => ("P(x)", PxValue.ToString("N20").TrimEnd('0'));
		public (string, string) Qx => ("Q(x)", QxValue.ToString("N20").TrimEnd('0'));
		public (string, string) R1 => ("r", SelectionRandom.ToString("N20").TrimEnd('0'));

		public (string, string) SelectionXReal => ("sel xReal", SelectionValue?.xReal.ToString());
		public (string, string) SelectionXBin => ("sel xBin", SelectionValue?.xBin_xInt);
		
		//public (string, string) ParentXBin => ("Rodzic", )

		public double GxValue = 0.0;
		public double PxValue = 0.0;
		public double QxValue = 0.0;
		public Specimen SelectionValue;
		//public bool 
	}
}