using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using ScottPlot.Plottable;

namespace INA_Generations
{
	
	public partial class MainForm
	{
		private void ExecuteClimbers()
		{
			if (!(
				    ParseHelper.ParseDouble(Climbers_AInput.Text, "A", out double a) &&
				    ParseHelper.ParseDouble(Climbers_BInput.Text, "B", out double b) &&
				    ParseHelper.ParseDouble(Climbers_DInput.SelectedKey, "D", out double d, "en-US") &&
				    ParseHelper.ParseLong(Climbers_TInput.Text, "T", out long t)
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

			List<Specimen> climbers = new List<Specimen>();
			Dictionary<long, long> StepsToSolutionsDict = new Dictionary<long, long>();
			bool local = false;
			for (int i = 0; i < t; i++)
			{
				Specimen Vc = new Specimen();
				long steps = 0;
				if (t <= 1)
				{
					climbers.Add(Vc);
				}

				do
				{
					var neighbors = FindNeighborsXBin(Vc);
					var neighborsFx = CalculateNeighborsFx(neighbors);

					if (Singleton.TargetFunction == TargetFunction.Max)
					{
						double bestFx = Double.MinValue;
						string bestNeighbor = "";
						for (int j = 0; j < neighbors.Length; j++)
						{
							if (neighborsFx[j] > bestFx)
							{
								bestFx = neighborsFx[j];
								bestNeighbor = neighbors[j];
							}
						}

						if (bestFx > Vc.Fx)
						{
							steps++;
							Vc = new Specimen(bestNeighbor);
							if (t <= 1)
							{
								climbers.Add(Vc);
							}
						}
						else
						{
							local = true;
						}
					}
					else
					{
						double bestFx = Double.MaxValue;
						string bestNeighbor = "";
						for (int j = 0; j < neighbors.Length; j++)
						{
							if (neighborsFx[j] < bestFx)
							{
								bestFx = neighborsFx[j];
								bestNeighbor = neighbors[j];
							}
						}

						if (bestFx < Vc.Fx)
						{
							Vc = new Specimen(bestNeighbor);
							if (t <= 1)
							{
								climbers.Add(Vc);
							}
						}
						else
						{
							local = true;
						}
					}
				} while (!local);

				if (t > 1)
				{
					if (StepsToSolutionsDict.ContainsKey(steps))
					{
						StepsToSolutionsDict[steps]++;
					}
					else
					{
						StepsToSolutionsDict.Add(steps, 1);
					}

					climbers.Add(Vc);
				}
			}

			ClimbersPlot.Reset();

			if (t <= 1)
			{
				SignalPlot ClimbersSingleIterPlot = new SignalPlot();
				ClimbersSingleIterPlot.Color = Color.LawnGreen;
				ClimbersSingleIterPlot.FillBelow(Color.Lime, 1f);
				ClimbersSingleIterPlot.SampleRate = 1;
				ClimbersSingleIterPlot.MinRenderIndex = 0;
				ClimbersPlot.Plot.Add(ClimbersSingleIterPlot);
				List<double> PlotData = new List<double>();
				ObservableCollection<Specimen>
					dataStore = (ObservableCollection<Specimen>)ClimbersOutputTable.DataStore;
				dataStore.Clear();
				foreach (var specimen in climbers)
				{
					dataStore.Add(specimen);
					PlotData.Add(specimen.Fx);
				}

				ClimbersSingleIterPlot.Ys = PlotData.ToArray();
				ClimbersSingleIterPlot.MaxRenderIndex = ClimbersSingleIterPlot.PointCount - 1;

				ClimbersOutputTable.Visible = true;
				ClimbersMultiOutputTable.Visible = false;
			}
			else
			{
				long maxSteps = StepsToSolutionsDict.Max(x => x.Key);
				long aggregateSum = 0;
				var Solutions = new List<ClimbersOutput>();

				// List<double> PlotData = new List<double>();

				for (long i = 0; i <= maxSteps; i++)
				{
					if (StepsToSolutionsDict.ContainsKey(i))
					{
						long solutions = StepsToSolutionsDict[i];
						aggregateSum += solutions;
						double hitPercent = (double)solutions / (double)t;
						double aggregateHitPercent = (double)aggregateSum / (double)t;

						Solutions.Add(new ClimbersOutput(i, solutions, aggregateSum, hitPercent, aggregateHitPercent));
					}
					else
					{
						double aggregateHitPercent = (double)aggregateSum / (double)t;
						Solutions.Add(new ClimbersOutput(i, 0, aggregateSum, 0, aggregateHitPercent));
					}
				}

				ObservableCollection<ClimbersOutput> dataStore =
					(ObservableCollection<ClimbersOutput>)ClimbersMultiOutputTable.DataStore;
				dataStore.Clear();
				foreach (var climbersOutput in Solutions)
				{
					dataStore.Add(climbersOutput);
				}

				ClimbersOutputTable.Visible = false;
				ClimbersMultiOutputTable.Visible = true;

				// SignalPlot ClimbersSingleIterPlot = new SignalPlot();
				// ClimbersSingleIterPlot.Color = Color.Goldenrod;
				// ClimbersSingleIterPlot.SampleRate = 1;
				// ClimbersSingleIterPlot.MinRenderIndex = 0;
				// ClimbersPlot.Plot.Add(ClimbersSingleIterPlot);

			}

			ClimbersPlot.Plot.AxisAuto(0.05f, 0.1f);
			ClimbersPlot.Refresh();

		}

		private static double[] CalculateNeighborsFx(string[] neighbors)
		{
			double[] neighborsFx = new double[neighbors.Length];
			for (int j = 0; j < neighbors.Length; j++)
			{
				neighborsFx[j] = MathHelper.Fx(MathHelper.XBinToXReal(neighbors[j]));
			}

			return neighborsFx;
		}

		private static string[] FindNeighborsXBin(Specimen Vc)
		{
			string[] neighbors = new string[Vc.XBin.Length];
			for (int j = 0; j < neighbors.Length; j++)
			{
				neighbors[j] =
					new string(Vc.XBin.Select((x, index) => index == j ? x == '0' ? '1' : '0' : x).ToArray());
			}

			return neighbors;
		}
	}
}