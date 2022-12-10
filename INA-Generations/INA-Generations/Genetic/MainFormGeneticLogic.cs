using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eto.Forms;
using ScottPlot.Plottable;

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

			GroupingCancellationTokenSource.Cancel();
			GroupingCancellationTokenSource = new CancellationTokenSource();
			GroupingCancellationToken = CancellationToken.None;

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
			ClearGroupOutputTable();
			bool isBenchmarkRun = BenchmarkCheckbox.Checked.Value && t == 1;
			Stopwatch benchmark = isBenchmarkRun ? Stopwatch.StartNew() : null;
			Stopwatch benchmark_total = isBenchmarkRun ? Stopwatch.StartNew() : null;
			DataRow[] data = new DataRow[n];
			CreateInitialData(data, n);
			string benchmarks = "";
			if (isBenchmarkRun)
			{
				benchmarks = $"Creating Random Data: {benchmark.ElapsedMilliseconds}[ms]\n";
			}

			InitPlot(data, out var MinFx, out var MaxFx, out var AvgFx, out var MinGxPlot, out var AvgGxPlot, out var MaxGxPlot);

			if (addDataAtStart)
			{
				AddDataToTable(data);
			}

			for (int i = 0; i < t; i++)
			{
				CalculateGx(data);
				if (isBenchmarkRun)
				{
					benchmarks += $"Gx: {benchmark.ElapsedMilliseconds}[ms]\n";
					benchmark.Restart();
				}

				CalculatePx(data);
				if (isBenchmarkRun)
				{
					benchmarks += $"Px: {benchmark.ElapsedMilliseconds}[ms]\n";
					benchmark.Restart();
				}

				CalculateQx(data);
				if (isBenchmarkRun)
				{
					benchmarks += $"Qx: {benchmark.ElapsedMilliseconds}[ms]\n";
					benchmark.Restart();
				}

				Selection(data);
				if (isBenchmarkRun)
				{
					benchmarks += $"Selection: {benchmark.ElapsedMilliseconds}[ms]\n";
					benchmark.Restart();
				}

				Parenting(data);
				if (isBenchmarkRun)
				{
					benchmarks += $"Parenting: {benchmark.ElapsedMilliseconds}[ms]\n";
					benchmark.Restart();
				}

				PairParents(data);
				if (isBenchmarkRun)
				{
					benchmarks += $"Pairing: {benchmark.ElapsedMilliseconds}[ms]\n";
					benchmark.Restart();
				}

				RandomizePC(data);
				if (isBenchmarkRun)
				{
					benchmarks += $"Pc: {benchmark.ElapsedMilliseconds}[ms]\n";
					benchmark.Restart();
				}

				Fuck(data);
				if (isBenchmarkRun)
				{
					benchmarks += $"Children: {benchmark.ElapsedMilliseconds}[ms]\n";
					benchmark.Restart();
				}

				Mutate(data, elite);
				if (isBenchmarkRun)
				{
					benchmarks += $"Mutating: {benchmark.ElapsedMilliseconds}[ms]\n";
					benchmark.Restart();
				}

				Finalize(data);

				if (isBenchmarkRun)
				{
					benchmarks += $"Finalization: {benchmark.ElapsedMilliseconds}[ms]\n";
					benchmark.Restart();
				}

				MinFx.Add(data.Min(x => x.FinalFxRealValue));
				MaxFx.Add(data.Max(x => x.FinalFxRealValue));
				AvgFx.Add(data.Average(x => x.FinalFxRealValue));

				if (i + 1 < t)
				{
					MoveDataToNextGeneration(data);
				}
			}

			if (!addDataAtStart)
			{
				AddDataToTable(data);
			}

			MinGxPlot.Ys = MinFx.ToArray();
			MinGxPlot.MaxRenderIndex = MinGxPlot.PointCount - 1;
			MaxGxPlot.Ys = MaxFx.ToArray();
			MaxGxPlot.MaxRenderIndex = MaxGxPlot.PointCount - 1;
			AvgGxPlot.Ys = AvgFx.ToArray();
			AvgGxPlot.MaxRenderIndex = AvgGxPlot.PointCount - 1;

			Plot.Plot.AxisAuto(0.05f, 0.1f);
			Plot.Refresh();

			if (isBenchmarkRun)
			{
				benchmarks += $"Display: {benchmark.ElapsedMilliseconds}[ms]\n";
				benchmark.Stop();
				benchmarks += $"Total: {benchmark_total.ElapsedMilliseconds}[ms]";
				benchmark_total.Stop();
			}

			if (isBenchmarkRun && BenchmarkCheckbox.Checked.Value)
			{
				MessageBox.Show(benchmarks, "Benchmark Results", MessageBoxType.Question);
			}

			AddGroupDataToTable(new GroupDataRow[]
			{
				new GroupDataRow() { xBinValue = "grupowanie odbywa się na osobnym wątku, chwilę trzeba poczekać" }
			});
			GroupingCancellationToken = GroupingCancellationTokenSource.Token;
			Task.Run(() => GroupData(data, GroupingCancellationToken), GroupingCancellationToken);
			
			void InitPlot(DataRow[] data, out List<double> MinFx, out List<double> MaxFx, out List<double> AvgFx, out SignalPlot MinGxPlot,
				out SignalPlot AvgGxPlot, out SignalPlot MaxGxPlot)
			{
				MinFx = new List<double>();
				MaxFx = new List<double>();
				AvgFx = new List<double>();
				MinFx.Add(data.Min(x => x.OriginalSpecimen.Fx));
				MaxFx.Add(data.Max(x => x.OriginalSpecimen.Fx));
				AvgFx.Add(data.Average(x => x.OriginalSpecimen.Fx));
				Plot.Reset();
				MinGxPlot = new SignalPlot();
				MinGxPlot.Color = Color.Red;
				MinGxPlot.FillBelow(Color.Crimson, 1f);
				MinGxPlot.SampleRate = 1;
				MinGxPlot.MinRenderIndex = 0;
				AvgGxPlot = new SignalPlot();
				AvgGxPlot.Color = Color.DodgerBlue;
				AvgGxPlot.FillBelow(Color.Turquoise, 1f);
				AvgGxPlot.SampleRate = 1;
				AvgGxPlot.MinRenderIndex = 0;
				MaxGxPlot = new SignalPlot();
				MaxGxPlot.Color = Color.LawnGreen;
				MaxGxPlot.FillBelow(Color.Lime, 1f);
				MaxGxPlot.SampleRate = 1;
				MaxGxPlot.MinRenderIndex = 0;

				Plot.Plot.Add(MaxGxPlot);
				Plot.Plot.Add(AvgGxPlot);
				Plot.Plot.Add(MinGxPlot);
			}
		}

		class DataGroup : IComparer<DataGroup>, IComparable<DataGroup>
		{
			public double XReal;
			public double Percent;
			public double Fx;

			public DataGroup(double xReal, double percent, double fx)
			{
				XReal = xReal;
				Percent = percent;
				Fx = fx;
			}

			public int CompareTo(DataGroup other)
			{
				if (ReferenceEquals(this, other)) return 0;
				if (ReferenceEquals(null, other)) return 1;
				return Fx.CompareTo(other.Fx);
			}

			public int Compare(DataGroup x, DataGroup y)
			{
				if (ReferenceEquals(x, y)) return 0;
				if (ReferenceEquals(null, y)) return 1;
				if (ReferenceEquals(null, x)) return -1;
				return x.Fx.CompareTo(y.Fx);
			}
		}

		private void GroupData(DataRow[] data, CancellationToken token)
		{
			try
			{
				Dictionary<double, long> XRealCounts = new Dictionary<double, long>();
				foreach (var dataRow in data)
				{
					if (token.IsCancellationRequested)
					{
						return;
					}

					if (XRealCounts.ContainsKey(dataRow.FinalXRealValue))
					{
						XRealCounts[dataRow.FinalXRealValue]++;
					}
					else
					{
						XRealCounts.Add(dataRow.FinalXRealValue, 1);
					}
				}

				double dataCountAsDouble = data.Length * 0.01;

				SortedSet<DataGroup> sortedGroups = new SortedSet<DataGroup>();

				foreach (var xRealCount in XRealCounts)
				{
					if (token.IsCancellationRequested)
					{
						return;
					}

					sortedGroups.Add(new DataGroup(xRealCount.Key, xRealCount.Value / dataCountAsDouble,
						MathHelper.Fx(xRealCount.Key)));
				}

				IEnumerable<DataGroup> groupsData = (Singleton.TargetFunction == TargetFunction.Max)
					? sortedGroups
					: sortedGroups.Reverse();

				GroupDataRow[] Groups = new GroupDataRow[sortedGroups.Count];

				long i = 0;
				foreach (var group in groupsData)
				{
					if (token.IsCancellationRequested)
					{
						return;
					}

					Groups[i] = new GroupDataRow(i + 1, group.XReal, group.Percent);
					i++;
				}

				Application.Instance.InvokeAsync(() => AddGroupDataToTable(Groups));
			}
			catch (Exception e)
			{
				MessageBox.Show($"Error while grouping: {e}");
			}
		}

		private void CreateInitialData(DataRow[] data, long n)
		{
			Parallel.For(0, n, i =>
			{
				Specimen specimen = new Specimen();
				data[i] = new DataRow(specimen, i + 1);
			});
		}

		private void MoveDataToNextGeneration(DataRow[] data)
		{
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = new DataRow(new Specimen(data[i].FinalXRealValue), data[i].Index);
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

		private void Mutate(DataRow[] data, bool elite)
		{
			long eliteXInt = 0;
			double bestFx = 0;
			if (elite)
			{
				eliteXInt = FindElite(data, eliteXInt, ref bestFx);
			}

			bool eliteMadeIt = !elite;

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

				string xBin = new string(chromosome);
				dataRow.MutatedChromosomeValue = xBin;
				eliteMadeIt = CheckIfEliteAlreadyMadeIt(eliteMadeIt, xBin, eliteXInt, bestFx);
			});


			ReplaceWithEliteIfNecessary(data, elite, eliteMadeIt, eliteXInt);
		}

		private static bool CheckIfEliteAlreadyMadeIt(bool eliteMadeIt, string xBin, long eliteXInt, double bestFx)
		{
			if (!eliteMadeIt)
			{
				long xInt = MathHelper.XBinToXInt(xBin);
				if (xInt == eliteXInt)
				{
					eliteMadeIt = true;
					return eliteMadeIt;
				}

				double xReal = MathHelper.XIntToXReal(xInt);
				double Fx = MathHelper.Fx(xReal);
				if (Singleton.TargetFunction == TargetFunction.Max)
				{
					if (Fx > bestFx)
					{
						eliteMadeIt = true;
					}
				}
				else
				{
					if (Fx < bestFx)
					{
						eliteMadeIt = true;
					}
				}
			}

			return eliteMadeIt;
		}

		private static long FindElite(DataRow[] data, long eliteXInt, ref double bestFx)
		{
			double bestGX = Double.MinValue;
			foreach (var row in data)
			{
				if (row.GxValue > bestGX)
				{
					eliteXInt = row.OriginalSpecimen.XInt;
					bestGX = row.GxValue;
					bestFx = row.OriginalSpecimen.Fx;
				}
			}

			return eliteXInt;
		}

		private static void ReplaceWithEliteIfNecessary(DataRow[] data, bool elite, bool eliteMadeIt, long eliteXInt)
		{
			if (!elite || eliteMadeIt) return;
			int index = Singleton.Random.Next(0, data.Length - 1);
			data[index].ReplacedByElite = true;
			data[index].MutatedChromosomeValue = MathHelper.XIntToXBin(eliteXInt);
		}

		[SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
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
					Parallel.ForEach(data, row => { row.RandomizeParenting(); });
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
					List<(Specimen OriginalSpecimen, string xBin_xInt, double Px)> chances =
						data.Select(x => (x.OriginalSpecimen, xBin_xInt: x.OriginalSpecimen.XBin, x.PxValue)).ToList();
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
			Parallel.ForEach(data, dataRow => { dataRow.PxValue = dataRow.GxValue / sum; });
		}

		private static void CalculateGx(DataRow[] data)
		{
			switch (Singleton.TargetFunction)
			{
				case TargetFunction.Max:
					double min = data.Min(x => x.OriginalSpecimen.Fx);
					Parallel.ForEach(data,
						dataRow => { dataRow.GxValue = dataRow.OriginalSpecimen.Fx - min + Singleton.d; });
					break;
				case TargetFunction.Min:
					double max = data.Max(x => x.OriginalSpecimen.Fx);
					Parallel.ForEach(data,
						dataRow => { dataRow.GxValue = -(dataRow.OriginalSpecimen.Fx - max) + Singleton.d; });

					break;
				default:
					throw new NotImplementedException();
			}
		}
	}
}