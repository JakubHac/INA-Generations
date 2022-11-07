using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

			bool addDataAtStart = Singleton.RandomRoulette != RouletteType.Disabled;
			
			LOutput.Text = l.ToString();
			ClearOutputTable();
			DataRow[] data = new DataRow[n];
			CreateInitialData(data, n);
			
			if (addDataAtStart)
			{
				AddDataToTable(data);
			}
			
			Stopwatch benchmark = Stopwatch.StartNew();
			List<(string,long)> benchmarks = new List<(string, long)>();

			for (int i = 0; i < t; i++)
			{
				CalculateGx(data);
				benchmarks.Add(("Gx", benchmark.ElapsedMilliseconds));
				benchmark.Restart();
				CalculatePx(data);
				benchmarks.Add(("Px", benchmark.ElapsedMilliseconds));
				benchmark.Restart();
				CalculateQx(data);
				benchmarks.Add(("Qx", benchmark.ElapsedMilliseconds));
				benchmark.Restart();
				Selection(data);
				benchmarks.Add(("Selection", benchmark.ElapsedMilliseconds));
				benchmark.Restart();
				Parenting(data);
				benchmarks.Add(("Parenting", benchmark.ElapsedMilliseconds));
				benchmark.Restart();
				PairParents(data);
				benchmarks.Add(("Pairing", benchmark.ElapsedMilliseconds));
				benchmark.Restart();
				RandomizePC(data);
				benchmarks.Add(("Pc", benchmark.ElapsedMilliseconds));
				benchmark.Restart();
				Fuck(data);
				benchmarks.Add(("Fucking", benchmark.ElapsedMilliseconds));
				benchmark.Restart();
				Mutate(data);
				benchmarks.Add(("Mutating", benchmark.ElapsedMilliseconds));
				benchmark.Restart();
				Finalize(data);
				benchmarks.Add(("Finalization", benchmark.ElapsedMilliseconds));
				benchmark.Restart();
				if (i + 1 < t)
				{
					MoveDataToNextGeneration(data, elite);
					benchmarks.Add(("Moving Data", benchmark.ElapsedMilliseconds));
					benchmark.Restart();
				}
			}

			if (!addDataAtStart)
			{
				AddDataToTable(data);
				benchmarks.Add(("Display", benchmark.ElapsedMilliseconds));
				benchmark.Stop();
			}

			if (BenchmarkCheckbox.Checked.Value)
			{
				MessageBox.Show(benchmarks.Aggregate($"All: {benchmarks.Sum(x => x.Item2)}[ms]", (s, tuple) => $"{s}\n{tuple.Item1}: {tuple.Item2}[ms]"),"Benchmark", MessageBoxType.Information);
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
			Parallel.ForEach(data, row =>
			{
				row.FinalXRealValue = MathHelper.XBinToXReal(row.MutatedChromosomeValue);
				row.FinalFxRealValue = MathHelper.Fx(row.FinalXRealValue);
			});
		}

		private void Mutate(DataRow[] data)
		{
			Parallel.ForEach(data, dataRow =>
			{
				dataRow.MutatedGenesValue = new SortedSet<int>();
				char[] chromosome = dataRow.AfterChild.Item2.ToCharArray();
				for (int j = 0; j < Singleton.l; j++)
				{
					if (Singleton.Random.NextDouble() < Singleton.PM)
					{
						dataRow.MutatedGenesValue.Add(j);
						chromosome[j] = chromosome[j] == '0' ? '1' : '0';
					}
				}
				dataRow.MutatedChromosomeValue = new string(chromosome);
			});
		}

		private void Fuck(DataRow[] data)
		{
			Parallel.ForEach(data, dataRow =>
			{
				if (dataRow.ParentsWith == null) return;
				dataRow.ChildXBin =
					$"{dataRow.FirstParentXBin.Item2.Substring(0, dataRow.PCValue.Value)} | {dataRow.SecondParentXBin.Item2.Substring(dataRow.PCValue.Value)}";
			});
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
			switch (Singleton.RandomRoulette)
			{
				case RouletteType.Disabled:
					Parallel.ForEach(data, row =>
					{
						row.RandomizeParenting();
					});
					break;
				case RouletteType.Gradient:
					foreach (var row in data)
					{
						row.RandomizeParenting();
						OutputTable.Invalidate();
					}
					break;
				case RouletteType.PieChart:
					foreach (var row in data)
					{
						row.RandomizeParenting();
						OutputTable.Invalidate();
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			
		}

		private void Selection(DataRow[] data)
		{
			switch (Singleton.RandomRoulette)
			{
				case RouletteType.Disabled:
					Parallel.ForEach(data, row =>
					{
						try
						{
							row.RandomizeSelection(true);
							long selectedIndex = MathHelper.BinarySearchForQ(data, row.SelectionRandom);
							if (selectedIndex == -1)
							{
								MessageBox.Show($"Iter Limit Reached for {row.SelectionRandom}");
								selectedIndex = 0;
							}
							row.SelectionValue = data[selectedIndex].OriginalSpecimen;
						}
						catch (Exception e)
						{
							MessageBox.Show(e.ToString());
						}
					});
					break;
				case RouletteType.Gradient:
					foreach (var row in data)
					{
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
						OutputTable.Invalidate();
					}
					break;
				case RouletteType.PieChart:
					List<(Specimen OriginalSpecimen, string xBin_xInt, double Px)> chances = data.Select(x => (x.OriginalSpecimen, x.OriginalSpecimen.xBin_xInt, x.PxValue)).ToList();
					foreach (var row in data)
					{
						var result = Singleton.GetRandomWithRoulette(chances);
						row.SelectionRandom = result.r;
						row.SelectionValue = result.result;
						OutputTable.Invalidate();
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
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
			Parallel.ForEach(data, dataRow =>
			{
				dataRow.PxValue = dataRow.GxValue / sum;
			});
		}

		private static void CalculateGx(DataRow[] data)
		{
			switch (Singleton.TargetFunction)
			{
				case INA_Generations.TargetFunction.Max:
					double min = data.Min(x => x.OriginalSpecimen.FxReal);
					Parallel.ForEach(data, dataRow =>
					{
						dataRow.GxValue = dataRow.OriginalSpecimen.FxReal - min + Singleton.d;
					});
					break;
				case INA_Generations.TargetFunction.Min:
					double max = data.Max(x => x.OriginalSpecimen.FxReal);
					Parallel.ForEach(data, dataRow =>
					{
						dataRow.GxValue = -(dataRow.OriginalSpecimen.FxReal - max) + Singleton.d;
					});

					break;
				default:
					throw new NotImplementedException();
			}
		}
	}
}