using System;

namespace INA_Generations
{
	public static class Singleton
	{
		public static Random Random = new Random();
		public static bool RandomRoulette = false;
		public static string Platform;
		public static bool LookingForMax = true;
		public static double PK = 0.5;

		public static double GetRandomWithRoulette()
		{
			if (!RandomRoulette) return Random.NextDouble();
			var roulette = new RouletteDialog();
			roulette.ShowModal();
			return roulette.Value;

		}
	}
}