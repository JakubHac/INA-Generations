namespace INA_Generations
{
	public class ClimbersOutput
	{
		[DisplayInGridView("Kroki")]
		public long NumberOfSteps;
		[DisplayInGridView("Ilość Rozwiązań")]
		public long NumberOfSolutions;
		[DisplayInGridView("Procent Ilości Rozwiązań")]
		public double HitPercent;
		[DisplayInGridView("Kumulatywna Ilość rozwiązań")]
		public long AggregateNumberOfSolutions;
		[DisplayInGridView("Kumulatywny Procent Ilości Rozwiązań")]
		public double AggregateHitPercent;

		public ClimbersOutput(long numberOfSteps, long numberOfSolutions, long aggregateNumberOfSolutions, double hitPercent, double aggregateHitPercent)
		{
			NumberOfSteps = numberOfSteps;
			NumberOfSolutions = numberOfSolutions;
			AggregateNumberOfSolutions = aggregateNumberOfSolutions;
			HitPercent = hitPercent;
			AggregateHitPercent = aggregateHitPercent;
		}
	}
}