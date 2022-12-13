namespace INA_Generations
{
	public partial class MainForm
	{
		private double BruteForceBestValue()
		{
			Specimen tester = new Specimen(0);
			double bestSoFar = Singleton.TargetFunction == TargetFunction.Max ? double.MinValue : double.MaxValue;
			while (tester.xReal <= Singleton.b)
			{
				switch (Singleton.TargetFunction)
				{
					case TargetFunction.Max:
						if (bestSoFar < tester.Fx)
						{
							bestSoFar = tester.Fx;
						}
						break;
					case TargetFunction.Min:
						if (bestSoFar > tester.Fx)
						{
							bestSoFar = tester.Fx;
						}
						break;
				}
				tester = new Specimen(tester.XInt + 1);
			}
			return bestSoFar;
		}

	}
}