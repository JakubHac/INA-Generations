namespace INA_Generations
{
	public class ClimbersOutput
	{
		public long NumberOfSteps;
		public long NumberOfSolutions;
		public long AggregateNumberOfSolutions;
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

		// public string DisplayNumberOfSteps => "";//NumberOfSteps.ToString();
		// public string DisplayNumberOfSolutions => "";//NumberOfSolutions.ToString();
		// public string DisplayHitPercent => "";//HitPercent.ToString("P");
		// public string DisplayAggregateNumberOfSolutions => "";//AggregateNumberOfSolutions.ToString();
		// public string DisplayAggregateHitPercent => "";//AggregateHitPercent.ToString("P");

	}
}