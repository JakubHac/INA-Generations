using System;
using System.Collections.Generic;
using System.Linq;

namespace INA_Generations
{
	public class DataRow
	{
		public static DataRow Empty = new(null, -1);

		public Specimen OriginalSpecimen = null;
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

		public (string, string) N => ("N", Index.ToString());
		public (string, string) xReal => ("xReal", OriginalSpecimen?.xReal.ToString());
		public (string, string) Fx => ("F(x)", OriginalSpecimen?.Fx.ToString("N20").TrimEnd('0'));
		public (string, string) Gx => ("G(x)", GxValue.ToString("N20").TrimEnd('0'));
		public (string, string) Px => ("P(x)", PxValue.ToString("N20").TrimEnd('0'));
		public (string, string) Qx => ("Q(x)", QxValue.ToString("N20").TrimEnd('0'));
		public (string, string) R1 => ("r", SelectionRandom.ToString("N20").TrimEnd('0'));

		public (string, string) SelectionXReal => ("sel xReal", SelectionValue?.xReal.ToString());
		public (string, string) SelectionXBin => ("sel xBin", SelectionValue?.XBin);

		public (string, string) FirstParentXBin => ("Rodzic 1", isParent ? SelectionXBin.Item2 : "-");
		public (string, string) SecondParentXBin => ("Rodzic 2", ParentsWith != null ? ParentsWith.SelectionXBin.Item2 : "-");
		public (string, string) PC => ("PC", PCValue != null ? PCValue.ToString() : "-");
		
		public (string, string) Child => ("Dziecko", ChildXBin ?? "-");

		public (string, string) AfterChild => ("Po Krzyżowaniu", ChildXBin != null ? ChildXBin.Replace(" | ", "") : SelectionXBin.Item2);

		public (string, string) MutatedGenes => ("Zmutowane Geny", MutatedGenesValue.Count > 0 ? MutatedGenesValue.Aggregate("", (s, i) => $"{s},{i+1}").Substring(1) : "-");
		public (string, string) MutatedChromosome => ("M xBin", ReplacedByElite ? "NADPISANY PRZEZ ELITĘ" : MutatedChromosomeValue ?? "-");
		public (string, string) FinalXReal => ("M xReal", FinalXRealValue.ToString());
		public (string, string) FinalFxReal => ("M F(x)", FinalFxRealValue.ToString("N20").TrimEnd('0'));

		public string ChildXBin = null;
		public double GxValue = 0.0;
		public double PxValue = 0.0;
		public double QxValue = 0.0;
		public Specimen SelectionValue;
		public string MutatedChromosomeValue = null;
		public double FinalXRealValue = 0.0;
		public double FinalFxRealValue = 0.0;
		public bool isParent => ParentRandom < Singleton.PK;
		public DataRow ParentsWith = null;
		public long Index;
	}
}