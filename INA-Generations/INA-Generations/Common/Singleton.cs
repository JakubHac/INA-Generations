using System;
using System.Collections.Generic;
using System.Linq;

namespace INA_Generations
{
	public static class Singleton
	{
		[ThreadStatic] private static Random localRandom;
		public static Random Random => localRandom ??= new();
		public static RouletteType RandomRoulette = RouletteType.Disabled;
		public static string Platform;
		public static TargetFunction TargetFunction = TargetFunction.Max;
		public static double PK = 0.5;
		public static double PM = 0.5;

		public static double a = -4;
		public static double b = 12;
		public static double d = 0.001;
		public static int l;
		
		/// <summary>
		/// Returns a random xReal value from the interval [a, b]
		/// </summary>
		public static double RandomXReal()
		{
			int accuracy = MathHelper.Accuracy(d);
			var truexReal = Random.NextDouble() * (b - a) + a;
			return Math.Round(truexReal, accuracy);
		}
		
		/// <summary>
		/// Uses an animated roulette to get a random value
		/// </summary>
		/// <returns></returns>
		public static double GetRandomWithRoulette()
		{
			if (RandomRoulette == RouletteType.Disabled) return Random.NextDouble();
			var roulette = new RouletteDialog();
			roulette.ShowModal();
			return roulette.Value;
		}
		
		/// <summary>
		/// Uses an animated roulette to select a random value from a list
		/// </summary>
		/// <param name="chances">Definitions of values to be displayed on the roulette</param>
		/// <returns>Selected value and r from interval [0,1] that was used to select the value</returns>
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