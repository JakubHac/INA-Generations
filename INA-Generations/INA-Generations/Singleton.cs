using System;
using System.Collections.Generic;
using System.Linq;

namespace INA_Generations
{
	public static class Singleton
	{
		public static Random Random = new Random();
		public static RouletteType RandomRoulette = RouletteType.Disabled;
		public static string Platform;
		public static TargetFunction TargetFunction = TargetFunction.Max;
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
			if (RandomRoulette == RouletteType.Disabled) return Random.NextDouble();
			var roulette = new RouletteDialog();
			roulette.ShowModal();
			return roulette.Value;
		}
		
		public static (T result, double r) GetRandomWithRoulette<T>(List<(T obj, string displayName, double chance)> chances)
		{
			double sumChance = chances.Sum(x => x.chance);
			var casted = chances.Select(x => ((object)x.obj, x.displayName, x.chance / sumChance)).ToList();
			
			if (RandomRoulette == RouletteType.Disabled)
			{
				double rand = Random.NextDouble();
				for (int i = 0; i < chances.Count; i++)
				{
					if (rand <= chances[i].chance)
					{
						return (chances[i].obj, rand);
					}

					rand -= chances[i].chance;
				}
			}
			var roulette = new RouletteDialog(casted);
			roulette.ShowModal();
			return ((T)roulette.Result, roulette.Value);
		}
	}
}