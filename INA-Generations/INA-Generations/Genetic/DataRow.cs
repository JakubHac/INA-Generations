using System;
using System.Collections.Generic;
using System.Linq;

namespace INA_Generations
{
	public class DataRow
	{
		[DisplayInGridView("N")] public long Index;

		[DisplayInGridView("xReal", -1)] public string xReal => OriginalSpecimen?.XReal.ToString();
		[DisplayInGridView("F(x)", -2)] public string Fx => OriginalSpecimen?.Fx.ToString("N20").TrimEnd('0');
		[DisplayInGridView("G(x)", -3)] public string Gx => GxValue.ToString("N20").TrimEnd('0');
		[DisplayInGridView("P(x)", -4)] public string Px => PxValue.ToString("N20").TrimEnd('0');
		[DisplayInGridView("Q(x)", -5)] public string Qx => QxValue.ToString("N20").TrimEnd('0');
		[DisplayInGridView("r", -6)] public string R1 => SelectionRandom.ToString("N20").TrimEnd('0');
		[DisplayInGridView("sel xReal", -7)] public string SelectionXReal => SelectionValue?.XReal.ToString();
		[DisplayInGridView("sel xBin", -8)] public string SelectionXBin => SelectionValue?.XBin;
		[DisplayInGridView("Rodzic 1", -9)] public string FirstParentXBin => isParent ? SelectionXBin : "-";

		[DisplayInGridView("Rodzic 2", -10)]
		public string SecondParentXBin => ParentsWith != null ? ParentsWith.SelectionXBin : "-";

		[DisplayInGridView("PC", -11)] public string PC => PCValue != null ? PCValue.ToString() : "-";
		[DisplayInGridView("Dziecko", -12)] public string Child => ChildXBin ?? "-";

		[DisplayInGridView("Po Krzyżowaniu", -13)]
		public string AfterChild => ChildXBin != null ? ChildXBin.Replace(" | ", "") : SelectionXBin;

		[DisplayInGridView("Zmutowane Geny", -14)]
		public string MutatedGenes => MutatedGenesValue.Count > 0
			? MutatedGenesValue.Aggregate("", (s, i) => $"{s},{i + 1}").Substring(1)
			: "-";

		[DisplayInGridView("M xBin", -15)]
		public string MutatedChromosome => ReplacedByElite ? "NADPISANY PRZEZ ELITĘ" : MutatedChromosomeValue ?? "-";

		[DisplayInGridView("M xReal", -16)] public double FinalXRealValue;

		[DisplayInGridView("M F(x)", -17)] public string FinalFxReal => FinalFxRealValue.ToString("N20").TrimEnd('0');


		public string ChildXBin = null;
		public double GxValue;
		public double PxValue;
		public double QxValue;
		public Specimen SelectionValue;
		public string MutatedChromosomeValue = null;
		public double FinalFxRealValue;
		public bool isParent => ParentRandom < Singleton.PK;
		public DataRow ParentsWith = null;
		public Specimen OriginalSpecimen;
		public double SelectionRandom;
		public double ParentRandom = Double.PositiveInfinity;
		public int? PCValue = null;
		public SortedSet<int> MutatedGenesValue = new();
		public bool ReplacedByElite = false;

		public DataRow(Specimen originalSpecimen, long index)
		{
			OriginalSpecimen = originalSpecimen;
			Index = index;
		}

		/// <summary>
		/// Randomizes SelectionRandom value
		/// </summary>
		/// <param name="skipRoulette">Whether to use the animated roulette or not</param>
		public void RandomizeSelection(bool skipRoulette = false)
		{
			SelectionRandom = skipRoulette ? Singleton.Random.NextDouble() : Singleton.GetRandomWithRoulette();
		}

		/// <summary>
		/// Randomizes ParentRandom value
		/// </summary>
		public void RandomizeParenting()
		{
			switch (Singleton.RandomRoulette)
			{
				case RouletteType.Disabled:
					ParentRandom = Singleton.Random.NextDouble();
					break;
				case RouletteType.Gradient:
					ParentRandom = Singleton.GetRandomWithRoulette();
					break;
				case RouletteType.PieChart:
					if (Singleton.PK <= 0.0 + double.Epsilon)
					{
						ParentRandom = 0.0;
					}
					else
					{
						List<(bool obj, string displayName, double chance)> pieChart =
							new()
							{
								(true, "Zostań Rodzicem", Singleton.PK - double.Epsilon),
								(false, "Nie Bądź Rodzicem", 1f - Singleton.PK)
							};
						var result = Singleton.GetRandomWithRoulette(pieChart);
						ParentRandom = result.r;
					}

					break;
			}
		}
	}
}