namespace INA_Generations
{
	public class ClimbersOutput
	{
		[DisplayInGridView("Kroki")] 
		public long NumberOfSteps;
		[DisplayInGridView("Ilość Rozwiązań")]
		public long NumberOfSolutions;
		[DisplayInGridView("Kumulatywna Ilość rozwiązań", -2)]
		public long AggregateNumberOfSolutions;
		[DisplayInGridView("Procent Ilości Rozwiązań", -1)]
		public string HitPercentString => HitPercent.ToString("P");
		[DisplayInGridView("Kumulatywny Procent Ilości Rozwiązań", -3)]
		public string AggregateHitPercentString => AggregateHitPercent.ToString("P");
		
		public double HitPercent;
		
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