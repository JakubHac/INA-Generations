using System;

namespace INA_Generations
{
	public static class Singleton
	{
		public static Random Random = new Random();
		public static bool RandomRoulette = true;
		public static string Platform;

		public static double GetRandomWithRoulette()
		{
			if (RandomRoulette)
			{
				var roulette = new RouletteDialog();
				roulette.ShowModal();
				return roulette.Value;
			}
			else
			{
				return Random.NextDouble();
			}
		}
	}
}