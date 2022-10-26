using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;

namespace INA_Generations
{
	public partial class MainForm
	{
		private void ExecuteGeneration()
		{
			if (!(
				    ParseHelper.ParseDouble(AInput.Text, "A", out double a) &&
				    ParseHelper.ParseDouble(BInput.Text, "B", out double b) &&
				    ParseHelper.ParseLong(NInput.Text, "N", out long n) &&
				    ParseHelper.ParseDouble(DInput.SelectedKey, "D", out double d, "en-US")
			    )
			   )
			{
				return;
			}

			if (n < 0)
			{
				MessageBox.Show("N nie może być mniejsze od 0", MessageBoxType.Error);
				return;
			}

			SyncPKValueToSlider();
			SyncPMValueToSlider();

			Singleton.PK = PKSlider.Value / 100_000_000.0;
			Singleton.PM = PMSlider.Value / 100_000_000.0;

			int l = (int)Math.Floor(Math.Log((b - a) / d, 2) + 1.0);

			Singleton.a = a;
			Singleton.b = b;
			Singleton.d = d;
			Singleton.l = l;

			Singleton.RandomRoulette = RouletteTypeDropdown.SelectedKey switch
			{
				"Wyłączona" => RouletteType.Disabled,
				"Zakres (0;1)" => RouletteType.Gradient,
				"Koło Fortuny" => RouletteType.PieChart
			};

			Singleton.TargetFunction = TargetFunctionDropdown.SelectedKey switch
			{
				"Maksimum" => TargetFunction.Max, 
				"Minimum" => TargetFunction.Min
			};


			LOutput.Text = l.ToString();
			ClearOutputTable();

			List<DataRow> data = new List<DataRow>();

			for (int i = 0; i < n; i++)
			{
				Specimen specimen = new Specimen();
				data.Add(CreateDataRowForSpecimen(specimen, i + 1));
			}

			CalculateGx(data);
			CalculatePx(data);
			CalculateQx(data);
			Selection(data);
			Parenting(data);
			PairParents(data);
			RandomizePC(data);
			Fuck(data);
			Mutate(data);
			Finalize(data);
		}

		private void Finalize(List<DataRow> data)
		{
			foreach (var row in data)
			{
				row.FinalXRealValue = MathHelper.XBinToXReal(row.MutatedChromosomeValue);
				row.FinalFxRealValue = MathHelper.Fx(row.FinalXRealValue);
			}
		}

		private void Mutate(List<DataRow> data)
		{
			for (int i = 0; i < data.Count; i++)
			{
				data[i].MutatedGenesValue = new SortedSet<int>();
				for (int j = 0; j < Singleton.l; j++)
				{
					if (Singleton.Random.NextDouble() < Singleton.PM)
					{
						data[i].MutatedGenesValue.Add(j);
					}
				}

				data[i].MutatedChromosomeValue = data[i].AfterChild.Item2.Select((x, index) =>
				{
					if (data[i].MutatedGenesValue.Contains(index))
					{
						return x == '0' ? '1' : '0';
					}

					return x;
				}).Aggregate("", (s, c) => $"{s}{c}");
			}
		}

		private void Fuck(List<DataRow> data)
		{
			for (int i = 0; i < data.Count; i++)
			{
				var row = data[i];
				if (row.ParentsWith == null) continue;
				row.ChildXBin =
					$"{row.FirstParentXBin.Item2.Substring(0, row.PCValue.Value)} | {row.SecondParentXBin.Item2.Substring(row.PCValue.Value)}";
			}
		}

		private void RandomizePC(List<DataRow> data)
		{
			for (int i = 0; i < data.Count; i++)
			{
				var row = data[i];
				if (row.ParentsWith == null || row.PCValue != null) continue;
				
				switch (Singleton.RandomRoulette)
				{
					case RouletteType.Disabled:
					case RouletteType.Gradient:
						int pc = 1 + (int)Math.Round(Singleton.GetRandomWithRoulette() * (Singleton.l - 2));
						row.PCValue = pc;
						row.ParentsWith.PCValue = pc;
						break;
					case RouletteType.PieChart:
						List<(int obj, string displayName, double chance)> possiblePC =
							new List<(int obj, string displayName, double chance)>();
						for (int j = 1; j < Singleton.l; j++)
						{
							possiblePC.Add((
								j, 
								$"{row.FirstParentXBin.Item2.Substring(0, j)} | {row.SecondParentXBin.Item2.Substring(j)}"
								, 1f / ((float)Singleton.l - 1f)));
						}
						var result = Singleton.GetRandomWithRoulette(possiblePC);
						row.PCValue = result.result;
						row.ParentsWith.PCValue = result.result;
						break;
				}
				OutputTable.Invalidate();
			}
			
			
		}

		private void PairParents(List<DataRow> data)
		{
			for (int i = 0; i < data.Count; i++)
			{
				DataRow row = data[i];
				if (!row.isParent || row.ParentsWith != null) continue;
				DataRow pair = null;
				if (i + 1 < data.Count)
				{
					for (int j = i + 1; j < data.Count; j++)
					{
						if (!data[j].isParent) continue;
						pair = data[j];
						break;
					}
				}

				if (pair != null)
				{
					row.ParentsWith = pair;
					pair.ParentsWith = row;
				}
			}
		}

		private void Parenting(List<DataRow> data)
		{
			foreach (var row in data)
			{
				row.RandomizeParenting();
				OutputTable.Invalidate();
			}
		}

		private void Selection(List<DataRow> data)
		{
			List<(Specimen OriginalSpecimen, string xBin_xInt, double Px)> chances = data.Select(x => (x.OriginalSpecimen, x.OriginalSpecimen.xBin_xInt, x.PxValue)).ToList();
			foreach (var row in data)
			{
				switch (Singleton.RandomRoulette)
				{
					case RouletteType.Disabled:
					case RouletteType.Gradient:
						row.RandomizeSelection();
						int selectedIndex = data.Count - 1;
						double lastQ = 0.0;
						for (int i = 0; i < data.Count; i++)
						{
							if (data[i].QxValue >= row.SelectionRandom)
							{
								selectedIndex = i;
								break;
							}
						}

						row.SelectionValue = data[selectedIndex].OriginalSpecimen;
						break;
					case RouletteType.PieChart:
						var result = Singleton.GetRandomWithRoulette(chances);
						row.SelectionRandom = result.r;
						row.SelectionValue = result.result;
						break;
				}
				
				OutputTable.Invalidate();
			}
		}

		private void CalculateQx(List<DataRow> data)
		{
			double sum = 0.0;
			foreach (var dataRow in data)
			{
				sum += dataRow.PxValue;
				dataRow.QxValue = sum;
			}
		}

		private void CalculatePx(List<DataRow> data)
		{
			double sum = data.Sum(x => x.GxValue);
			foreach (var dataRow in data)
			{
				dataRow.PxValue = dataRow.GxValue / sum;
			}
		}

		private static void CalculateGx(List<DataRow> data)
		{
			switch (Singleton.TargetFunction)
			{
				case INA_Generations.TargetFunction.Max:
					double min = data.Min(x => x.OriginalSpecimen.FxReal);
					foreach (var dataRow in data)
					{
						dataRow.GxValue = dataRow.OriginalSpecimen.FxReal - min + Singleton.d;
					}

					break;
				case INA_Generations.TargetFunction.Min:
					double max = data.Max(x => x.OriginalSpecimen.FxReal);
					foreach (var dataRow in data)
					{
						dataRow.GxValue = -(dataRow.OriginalSpecimen.FxReal - max) + Singleton.d;
					}

					break;
				default:
					throw new NotImplementedException();
			}
		}
	}
}