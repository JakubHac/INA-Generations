using System;
using System.Collections.Generic;
using System.Data;
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
				    ParseHelper.ParseDouble(DInput.SelectedKey, "D", out double d, "en-US") &&
				    ParseHelper.ParseLong(TInput.Text, "T", out long t)
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

			if (t < 1)
			{
				MessageBox.Show("T < 1, program wygeneruje tylko pierwsze pokolenie", MessageBoxType.Error);
				return;
			}

			SyncPKValueToSlider();
			SyncPMValueToSlider();

			Singleton.PK = PKSlider.Value / (double)PKSlider.MaxValue;
			Singleton.PM = PMSlider.Value / (double)PMSlider.MaxValue;

			int l = (int)Math.Floor(Math.Log((b - a) / d, 2) + 1.0);
			bool elite = EliteCheckbox.Checked.Value;

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
			DataRow[] data = new DataRow[n];
			CreateInitialData(data, n);
			AddDataToTable(data);
			for (int i = 0; i < t; i++)
			{
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
				if (i + 1 < t)
				{
					MoveDataToNextGeneration(data, elite);
				}
			}
		}

		private void CreateInitialData(DataRow[] data, long n)
		{
			for (int i = 0; i < n; i++)
			{
				Specimen specimen = new Specimen();
				data[i] = new DataRow(specimen, i + 1);
			}
		}

		private void MoveDataToNextGeneration(DataRow[] data, bool elite)
		{
			double? eliteXReal = null;
			if (elite)
			{
				double bestGx = Double.MinValue;
				foreach (var dataRow in data)
				{
					if (dataRow.GxValue > bestGx)
					{
						bestGx = dataRow.GxValue;
						eliteXReal = dataRow.OriginalSpecimen.xReal;
					}
				}
			}

			bool eliteMadeIt = !elite;

			for (int i = 0; i < data.Length; i++)
			{
				var temp = data[i];
				double finalXReal = temp.FinalXRealValue;
				if (!eliteMadeIt)
				{
					if (finalXReal == eliteXReal)
					{
						eliteMadeIt = true;
					}
				}
				data[i] = new DataRow(new Specimen(finalXReal), temp.Index);
			}

			if (elite && !eliteMadeIt)
			{
				int index = Singleton.Random.Next(0, data.Length - 1);
				var temp = data[index];
				data[index] = new DataRow(new Specimen(eliteXReal.Value), temp.Index);
			}
		}

		private void Finalize(DataRow[] data)
		{
			foreach (var row in data)
			{
				row.FinalXRealValue = MathHelper.XBinToXReal(row.MutatedChromosomeValue);
				row.FinalFxRealValue = MathHelper.Fx(row.FinalXRealValue);
			}
		}

		private void Mutate(DataRow[] data)
		{
			foreach (var dataRow in data)
			{
				dataRow.MutatedGenesValue = new SortedSet<int>();
				for (int j = 0; j < Singleton.l; j++)
				{
					if (Singleton.Random.NextDouble() < Singleton.PM)
					{
						dataRow.MutatedGenesValue.Add(j);
					}
				}

				dataRow.MutatedChromosomeValue = dataRow.AfterChild.Item2.Select((x, index) =>
				{
					if (dataRow.MutatedGenesValue.Contains(index))
					{
						return x == '0' ? '1' : '0';
					}

					return x;
				}).Aggregate("", (s, c) => $"{s}{c}");
			}
		}

		private void Fuck(DataRow[] data)
		{
			foreach (var dataRow in data)
			{
				if (dataRow.ParentsWith == null) continue;
				dataRow.ChildXBin =
					$"{dataRow.FirstParentXBin.Item2.Substring(0, dataRow.PCValue.Value)} | {dataRow.SecondParentXBin.Item2.Substring(dataRow.PCValue.Value)}";
			}
		}

		private void RandomizePC(DataRow[] data)
		{
			foreach (var dataRow in data)
			{
				if (dataRow.ParentsWith == null || dataRow.PCValue != null) continue;
				
				switch (Singleton.RandomRoulette)
				{
					case RouletteType.Disabled:
					case RouletteType.Gradient:
						int pc = 1 + (int)Math.Round(Singleton.GetRandomWithRoulette() * (Singleton.l - 2));
						dataRow.PCValue = pc;
						dataRow.ParentsWith.PCValue = pc;
						break;
					case RouletteType.PieChart:
						List<(int obj, string displayName, double chance)> possiblePC =
							new List<(int obj, string displayName, double chance)>();
						for (int j = 1; j < Singleton.l; j++)
						{
							possiblePC.Add((
								j, 
								$"{dataRow.FirstParentXBin.Item2.Substring(0, j)} | {dataRow.SecondParentXBin.Item2.Substring(j)}"
								, 1f / ((float)Singleton.l - 1f)));
						}
						var result = Singleton.GetRandomWithRoulette(possiblePC);
						dataRow.PCValue = result.result;
						dataRow.ParentsWith.PCValue = result.result;
						break;
				}
			}
		}

		private void PairParents(DataRow[] data)
		{
			for (int i = 0; i < data.Length; i++)
			{
				DataRow row = data[i];
				if (!row.isParent || row.ParentsWith != null) continue;
				DataRow pair = null;
				if (i + 1 < data.Length)
				{
					for (int j = i + 1; j < data.Length; j++)
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

		private void Parenting(DataRow[] data)
		{
			foreach (var row in data)
			{
				row.RandomizeParenting();
				OutputTable.Invalidate();
			}
		}

		private void Selection(DataRow[] data)
		{
			List<(Specimen OriginalSpecimen, string xBin_xInt, double Px)> chances = data.Select(x => (x.OriginalSpecimen, x.OriginalSpecimen.xBin_xInt, x.PxValue)).ToList();
			foreach (var row in data)
			{
				switch (Singleton.RandomRoulette)
				{
					case RouletteType.Disabled:
					case RouletteType.Gradient:
						row.RandomizeSelection();
						int selectedIndex = data.Length - 1;
						for (int i = 0; i < data.Length; i++)
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

		private void CalculateQx(DataRow[] data)
		{
			double sum = 0.0;
			foreach (var dataRow in data)
			{
				sum += dataRow.PxValue;
				dataRow.QxValue = sum;
			}
		}

		private void CalculatePx(DataRow[] data)
		{
			double sum = data.Sum(x => x.GxValue);
			foreach (var dataRow in data)
			{
				dataRow.PxValue = dataRow.GxValue / sum;
			}
		}

		private static void CalculateGx(DataRow[] data)
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