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
		public static double PM = 0.5;

		public static double a;
		public static double b;
		public static double d;
		public static int l;
		
		
		public static double RandomXReal()
		{
			int accuracy = MathHelper.Accuracy(d);
			var truexReal = Random.NextDouble() * (b - a) + a;
			return Math.Round(truexReal, accuracy);
		}
		
		public static double GetRandomWithRoulette()
		{
			if (!RandomRoulette) return Random.NextDouble();
			var roulette = new RouletteDialog();
			roulette.ShowModal();
			return roulette.Value;
		}
	}
}