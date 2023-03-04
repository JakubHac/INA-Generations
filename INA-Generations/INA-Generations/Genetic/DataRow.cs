using System;
using System.Collections.Generic;
using System.Linq;

namespace INA_Generations
{
	public class DataRow
	{
		// public static DataRow Empty = new(null, -1);

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

		public void RandomizeSelection(bool skipRoulette = false)
		{
			SelectionRandom = skipRoulette ? Singleton.Random.NextDouble() : Singleton.GetRandomWithRoulette();
		}
		
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

		[DisplayInGridView("N")]
		public string N => Index.ToString();
		[DisplayInGridView("xReal")]
		public string xReal => OriginalSpecimen?.XReal.ToString();
		[DisplayInGridView("F(x)")]
		public string Fx => OriginalSpecimen?.Fx.ToString("N20").TrimEnd('0');
		[DisplayInGridView("G(x)")]
		public string Gx => GxValue.ToString("N20").TrimEnd('0');
		[DisplayInGridView("P(x)")]
		public string Px => PxValue.ToString("N20").TrimEnd('0');
		[DisplayInGridView("Q(x)")]
		public string Qx => QxValue.ToString("N20").TrimEnd('0');
		[DisplayInGridView("r")]
		public string R1 => SelectionRandom.ToString("N20").TrimEnd('0');
		[DisplayInGridView("sel xReal")]
		public string SelectionXReal => SelectionValue?.XReal.ToString();
		[DisplayInGridView("sel xBin")]
		public string SelectionXBin => SelectionValue?.XBin;
		[DisplayInGridView("Rodzic 1")]
		public string FirstParentXBin => isParent ? SelectionXBin : "-";
		[DisplayInGridView("Rodzic 2")]
		public string SecondParentXBin => ParentsWith != null ? ParentsWith.SelectionXBin : "-";
		[DisplayInGridView("PC")]
		public string PC => PCValue != null ? PCValue.ToString() : "-";
		[DisplayInGridView("Dziecko")]
		public string Child => ChildXBin ?? "-";
		[DisplayInGridView("Po Krzyżowaniu")]
		public string AfterChild => ChildXBin != null ? ChildXBin.Replace(" | ", "") : SelectionXBin;
		[DisplayInGridView("Zmutowane Geny")]
		public string MutatedGenes => MutatedGenesValue.Count > 0 ? MutatedGenesValue.Aggregate("", (s, i) => $"{s},{i+1}").Substring(1) : "-";
		[DisplayInGridView("M xBin")]
		public string MutatedChromosome => ReplacedByElite ? "NADPISANY PRZEZ ELITĘ" : MutatedChromosomeValue ?? "-";
		[DisplayInGridView("M xReal")]
		public string FinalXReal => FinalXRealValue.ToString();
		[DisplayInGridView("M F(x)")]
		public string FinalFxReal => FinalFxRealValue.ToString("N20").TrimEnd('0');

		public string ChildXBin = null;
		public double GxValue;
		public double PxValue;
		public double QxValue;
		public Specimen SelectionValue;
		public string MutatedChromosomeValue = null;
		public double FinalXRealValue;
		public double FinalFxRealValue;
		public bool isParent => ParentRandom < Singleton.PK;
		public DataRow ParentsWith = null;
		public long Index;
	}
}