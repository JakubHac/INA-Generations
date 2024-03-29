﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using ScottPlot.Plottable;

namespace INA_Generations
{
	public partial class MainForm
	{
		/// <summary>
		/// Executes the Hill Climb algorithm with the given parameters
		/// </summary>
		private void ExecuteClimbers()
		{
			if (!(
				    ParseHelper.ParseDouble(Climbers_AInput.Text, "A", out double a) &&
				    ParseHelper.ParseDouble(Climbers_BInput.Text, "B", out double b) &&
				    ParseHelper.ParseDouble(Climbers_DInput.SelectedKey, "D", out double d, "en-US") &&
				    ParseHelper.ParseLong(Climbers_TInput.Text, "T", out long t) &&
				    ParseHelper.ParseLong(Climbers_IInput.Text, "Iteracje", out long iters)
			    )
			   )
			{
				return;
			}

			Singleton.RandomRoulette = RouletteType.Disabled;
			Singleton.TargetFunction = Climbers_TargetFunctionDropdown.SelectedKey switch
			{
				"Maksimum" => TargetFunction.Max,
				"Minimum" => TargetFunction.Min
			};

			Singleton.a = a;
			Singleton.b = b;
			Singleton.d = d;
			int l = (int)Math.Floor(Math.Log((b - a) / d, 2) + 1.0);
			Singleton.l = l;

			ClimbersPlot.Reset();

			if (t == 1)
			{
				ClimbersExecuteSingleIter();
			}
			else if (Climbers_Analysis.Checked.Value)
			{
				double bestInRange = BruteForceBestValue();
				List<ClimbersOutput> AnalysisData = new();
				long aggregateNumberOfSolutions = 0;
				for (int i = 1; i < (t + 1); i++)
				{
					long solutions = 0;
					for (int j = 0; j < iters; j++)
					{
						var best = ClimbersOuterLoop(i);
						switch (Singleton.TargetFunction)
						{
							case TargetFunction.Max:
								if (best.Any(x => x.Fx >= bestInRange)) solutions++;
								break;
							case TargetFunction.Min:
								if (best.Any(x => x.Fx <= bestInRange)) solutions++;
								break;
						}
					}

					aggregateNumberOfSolutions += solutions;
					double hitPercent = solutions / (double)iters;
					double aggregateHitPercent = aggregateNumberOfSolutions / (double)(i * iters);
					AnalysisData.Add(new(i, solutions, aggregateNumberOfSolutions, hitPercent,
						aggregateHitPercent));
				}

				ClimbersMultiOutputTable.SetData(AnalysisData);

				ClimbersOutputTable.Visible = false;
				ClimbersMultiOutputTable.Visible = true;
				
				SignalPlot hitPercentPlot = new();
				hitPercentPlot.Color = Color.LawnGreen;
				hitPercentPlot.SampleRate = 1;
				hitPercentPlot.MinRenderIndex = 0;
				hitPercentPlot.Label = "% rozwiązań dla każdego T";
				ClimbersPlot.Plot.Add(hitPercentPlot);
				hitPercentPlot.Ys = AnalysisData.Select(x => x.HitPercent).ToArray();
				hitPercentPlot.MaxRenderIndex = hitPercentPlot.PointCount - 1;
				
				SignalPlot aggregateHitPercentPlot = new();
				aggregateHitPercentPlot.Color = Color.OrangeRed;
				aggregateHitPercentPlot.SampleRate = 1;
				aggregateHitPercentPlot.MinRenderIndex = 0;
				aggregateHitPercentPlot.Label = "Kumulatywny % rozwiązań";
				ClimbersPlot.Plot.Add(aggregateHitPercentPlot);
				aggregateHitPercentPlot.Ys = AnalysisData.Select(x => x.AggregateHitPercent).ToArray();
				aggregateHitPercentPlot.MaxRenderIndex = aggregateHitPercentPlot.PointCount - 1;
				ClimbersPlot.Plot.AxisAuto(0.05f, 0.1f);
				ClimbersPlot.Plot.Legend(enable: true);
				ClimbersPlot.Refresh();
			}
			else
			{
				ClimbersExecuteMultiIter(t);
			}
		}
		
		/// <summary>
		/// Finds the best value in the range [a, b] by checking every single value
		/// </summary>
		/// <returns></returns>
		private double BruteForceBestValue()
		{
			Specimen tester = new(0);
			double bestSoFar = Singleton.TargetFunction == TargetFunction.Max ? double.MinValue : double.MaxValue;
			while (tester.XReal <= Singleton.b)
			{
				switch (Singleton.TargetFunction)
				{
					case TargetFunction.Max:
						if (bestSoFar < tester.Fx)
						{
							bestSoFar = tester.Fx;
						}
						break;
					case TargetFunction.Min:
						if (bestSoFar > tester.Fx)
						{
							bestSoFar = tester.Fx;
						}
						break;
				}
				tester = new(tester.XInt + 1);
			}
			return bestSoFar;
		}
		
		/// <summary>
		/// Executes the Hill Climb algorithm for multiple iterations
		/// </summary>
		/// <param name="t"></param>
		private void ClimbersExecuteMultiIter(long t)
		{
			var bestOfEveryIter = ClimbersOuterLoop(t);
			List<double> bestFxSoFar = new() { bestOfEveryIter[0].Fx };
			for (int i = 1; i < bestOfEveryIter.Count; i++)
			{
				double bestOfThisIter = bestOfEveryIter[i].Fx;
				double lastBest = bestFxSoFar.Last();
				switch (Singleton.TargetFunction)
				{
					case TargetFunction.Max:
						bestFxSoFar.Add(lastBest > bestOfThisIter ? lastBest : bestOfThisIter);
						break;
					case TargetFunction.Min:
						bestFxSoFar.Add(lastBest < bestOfThisIter ? lastBest : bestOfThisIter);
						break;
				}
			}

			SignalPlot climbersPlot = new();
			climbersPlot.Color = Color.LawnGreen;
			climbersPlot.SampleRate = 1;
			climbersPlot.MinRenderIndex = 0;
			ClimbersPlot.Plot.Add(climbersPlot);
			climbersPlot.Ys = bestFxSoFar.ToArray();
			climbersPlot.MaxRenderIndex = climbersPlot.PointCount - 1;
			ClimbersPlot.Plot.AxisAuto(0.05f, 0.1f);
			ClimbersPlot.Plot.Legend(enable: false);
			ClimbersPlot.Refresh();
			ObservableCollection<Specimen> dataStore = (ObservableCollection<Specimen>)ClimbersOutputTable.DataStore;
			dataStore.Clear();
			foreach (var specimen in bestOfEveryIter)
			{
				dataStore.Add(specimen);
			}

			ClimbersOutputTable.Visible = true;
			ClimbersMultiOutputTable.Visible = false;
		}

		/// <summary>
		/// Executes the outer loop of the Hill Climb algorithm
		/// </summary>
		/// <param name="t">number of iterations</param>
		/// <returns>Best specimens of every iteration</returns>
		private static List<Specimen> ClimbersOuterLoop(long t)
		{
			List<Specimen> BestOfEveryIter = new();
			for (int i = 0; i < t; i++)
			{
				var progress = ClimbersInnerLoop();
				Specimen best = progress[0];
				for (int j = 1; j < progress.Count; j++)
				{
					switch (Singleton.TargetFunction)
					{
						case TargetFunction.Max:
							if (progress[j].Fx > best.Fx)
							{
								best = progress[j];
							}

							break;
						case TargetFunction.Min:
							if (progress[j].Fx < best.Fx)
							{
								best = progress[j];
							}

							break;
					}
				}

				BestOfEveryIter.Add(best);
			}

			return BestOfEveryIter;
		}

		/// <summary>
		/// Executes the Hill Climb algorithm for a single iteration
		/// </summary>
		private void ClimbersExecuteSingleIter()
		{
			var progressHistory = ClimbersInnerLoop();

			SignalPlot climbersPlot = new();
			climbersPlot.Color = Color.LawnGreen;
			climbersPlot.SampleRate = 1;
			climbersPlot.MinRenderIndex = 0;
			ClimbersPlot.Plot.Add(climbersPlot);
			climbersPlot.Ys = progressHistory.Select(x => x.Fx).ToArray();
			climbersPlot.MaxRenderIndex = climbersPlot.PointCount - 1;
			ClimbersPlot.Plot.AxisAuto(0.05f, 0.1f);
			ClimbersPlot.Plot.Legend(enable: false);
			ClimbersPlot.Refresh();
			ClimbersOutputTable.SetData(progressHistory);

			ClimbersOutputTable.Visible = true;
			ClimbersMultiOutputTable.Visible = false;
		}

		/// <summary>
		/// Inner loop of the Hill Climb algorithm, which executes until a local maximum is found
		/// </summary>
		/// <returns></returns>
		private static List<Specimen> ClimbersInnerLoop()
		{
			Specimen Vc = new();
			List<Specimen> progressHistory = new() { Vc };
			bool local = false;
			do
			{
				FindBestNeighbor(Vc, out var bestFx, out var bestNeighbor);
				bool neighborIsBetter = IsNeighborBetter(bestFx, Vc);
				if (neighborIsBetter)
				{
					Vc = new(bestNeighbor);
					progressHistory.Add(Vc);
				}
				else
				{
					local = true;
				}
			} while (!local);

			return progressHistory;
		}

		/// <summary>
		/// Checks if the neighbor is better than the current specimen
		/// </summary>
		/// <param name="bestFx">best F(x)</param>
		/// <param name="Vc">neighbor specimen</param>
		/// <returns></returns>
		private static bool IsNeighborBetter(double bestFx, Specimen Vc)
		{
			bool neighborIsBetter = false;
			switch (Singleton.TargetFunction)
			{
				case TargetFunction.Max:
					if (bestFx > Vc.Fx) neighborIsBetter = true;
					break;
				case TargetFunction.Min:
					if (bestFx < Vc.Fx) neighborIsBetter = true;
					break;
			}

			return neighborIsBetter;
		}

		/// <summary>
		/// Finds the best neighbor of the current specimen
		/// </summary>
		/// <param name="Vc">current specimen</param>
		/// <param name="bestFx">best F(x) found</param>
		/// <param name="bestNeighbor">best neighbors xBin</param>
		private static void FindBestNeighbor(Specimen Vc, out double bestFx, out string bestNeighbor)
		{
			var neighborsBin = FindNeighborsXBin(Vc);
			var neighborsFx = CalculateNeighborsFx(neighborsBin);

			bestFx = Double.MinValue;
			bestNeighbor = "";
			for (int j = 0; j < neighborsBin.Length; j++)
			{
				switch (Singleton.TargetFunction)
				{
					case TargetFunction.Max:
						if (neighborsFx[j] > bestFx)
						{
							bestFx = neighborsFx[j];
							bestNeighbor = neighborsBin[j];
						}

						break;
					case TargetFunction.Min:
						if (neighborsFx[j] < bestFx)
						{
							bestFx = neighborsFx[j];
							bestNeighbor = neighborsBin[j];
						}

						break;
				}
			}
		}

		/// <summary>
		/// Calculates the F(x) of all neighbors
		/// </summary>
		/// <param name="neighbors">neighbors xBin values</param>
		/// <returns>neighbors F(x) values</returns>
		private static double[] CalculateNeighborsFx(string[] neighbors)
		{
			double[] neighborsFx = new double[neighbors.Length];
			for (int j = 0; j < neighbors.Length; j++)
			{
				neighborsFx[j] = MathHelper.Fx(MathHelper.XBinToXReal(neighbors[j]));
			}

			return neighborsFx;
		}

		/// <summary>
		/// Returns all neighbors of the current specimen
		/// </summary>
		/// <param name="Vc">current specimen</param>
		/// <returns>xBin values of the neighbors</returns>
		private static string[] FindNeighborsXBin(Specimen Vc)
		{
			string[] neighbors = new string[Vc.XBin.Length];
			for (int j = 0; j < neighbors.Length; j++)
			{
				neighbors[j] =
					new(Vc.XBin.Select((x, index) => index == j ? x == '0' ? '1' : '0' : x).ToArray());
			}

			return neighbors;
		}
	}
}