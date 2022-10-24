namespace INA_Generations
{
	public class DataRow
	{
		public static DataRow Empty = new DataRow();

		public Specimen OriginalSpecimen = null;
		public double SelectionRandom;
		
		public void RandomizeSelection()
		{
			SelectionRandom = Singleton.GetRandomWithRoulette();
		}

		public (string, string) N => ("N", OriginalSpecimen?.LP.ToString());
		public (string, string) xReal => ("xReal",OriginalSpecimen?.xReal.ToString());
		public (string, string) Fx => ("F(x)",OriginalSpecimen?.FxReal.ToString());
		public (string, string) Gx => ("G(x)", "");
		public (string, string) Px => ("P(x)", "");
		public (string, string) Qx => ("Q(x)", "");
		public (string, string) R1 => ("r", SelectionRandom.ToString());

		
	}
}