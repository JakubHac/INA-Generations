using System;
using System.Collections.Generic;
using System.Linq;

namespace INA_Generations
{
	public partial class MainForm
	{

		/// <summary>
		/// Executes the analysis process of the genetic algorithm with the given parameters
		/// </summary>
		private void StartAnalysis()
		{
			if (!(ParseHelper.ParseDouble(Analysis_AInput.Text, "A", out double a) &&
			      ParseHelper.ParseDouble(Analysis_BInput.Text, "B", out double b) &&
			      ParseHelper.ParseDouble(Analysis_DInput.SelectedKey, "D", out double d, "en-US") &&
			      ParseHelper.ParseLong(Analysis_IterInput.Text, "Iter", out long iters) &&
			      ParseHelper.ParseLongArray(Analysis_NInput.Text, "N",
				      $"Błąd przy interpretacji N, powinny być wartości rozdzielane ; np. {10} ; {20} ; {30}", out long[] Ns) &&
			      ParseHelper.ParseLongArray(Analysis_TInput.Text, "T", $"Błąd przy interpretacji T, powinny być wartości rozdzielane ; np. {10} ; {20} ; {30}", out long[] Ts) &&
			      ParseHelper.ParseDoubleArray(Analysis_PKValue.Text, "PK",
				      $"Błąd przy interpretacji PK, powinny być wartości rozdzielane ; np. {0.8} ; {0.5}",
				      out double[] PKs) &&
			      ParseHelper.ParseDoubleArray(Analysis_PKValue.Text, "PM",
				      $"Błąd przy interpretacji PM, powinny być wartości rozdzielane ; np. {0.1} ; {0.01}",
				      out double[] PMs)
			    ))
			{
				return;
			}
			
			Singleton.RandomRoulette = RouletteType.Disabled;
			Singleton.TargetFunction = TargetFunctionDropdown.SelectedKey switch
			{
				"Maksimum" => TargetFunction.Max,
				"Minimum" => TargetFunction.Min
			};

			List<AnalysisDataRow> analysisDataRows = new();

			bool elite = Analysis_EliteCheckbox.Checked.Value;
			Singleton.d = d;
			Singleton.a = a;
			Singleton.b = b;
			int l = (int)Math.Floor(Math.Log((b - a) / d, 2) + 1.0);
			Singleton.l = l;

			AnalysisOutputTable.ClearData<AnalysisDataRow>();

			for (long n = 0; n < Ns.Length; n++)
			{
				for (long t = 0; t < Ts.Length; t++)
				{
					for (long pk = 0; pk < PKs.Length; pk++)
					{
						Singleton.PK = PKs[pk];
						for (long pm = 0; pm < PMs.Length; pm++)
						{
							Singleton.PM = PMs[pm];
							ExecutePermutation(n, t, pk, pm);
						}
					}
				}
			}

			void ExecutePermutation(long l1, long l2, long pk1, long pm1)
			{
				List<double> FXs = new();
				for (long i = 0; i < iters; i++)
				{
					DataRow[] data = CreateInitialData(Ns[l1]);

					for (int j = 0; j < Ts[l2]; j++)
					{
						RunGeneration(data, elite, j, Ts[l2]);
					}

					FXs.Add(data.Average(x => x.FinalFxRealValue));
				}

				analysisDataRows.Add(new(Ns[l1], Ts[l2], PKs[pk1], PMs[pm1], FXs.Average()));
			}

			if (Singleton.TargetFunction == TargetFunction.Max)
			{
				analysisDataRows = analysisDataRows.OrderByDescending(x => x.AvgFXValue)
					.ThenBy(x => x.NValue * x.TValue).ToList();
			}
			else
			{
				analysisDataRows = analysisDataRows.OrderBy(x => x.AvgFXValue).ThenBy(x => x.NValue * x.TValue)
					.ToList();
			}

			AnalysisOutputTable.SetData(analysisDataRows);
		}

		/// <summary>
		/// Runs the genetic algorithm for one generation
		/// </summary>
		/// <param name="data">current generation</param>
		/// <param name="elite">should the elite specimen be carried over</param>
		/// <param name="currentGenerationIndex">current generation index</param>
		/// <param name="desiredGenerationCount">max generation index</param>
		private void RunGeneration(DataRow[] data, bool elite, int currentGenerationIndex, long desiredGenerationCount)
		{
			CalculateGx(data);
			CalculatePx(data);
			CalculateQx(data);
			Selection(data);
			Parenting(data);
			PairParents(data);
			RandomizePC(data);
			CreateChildren(data);
			Mutate(data, elite);
			Finalize(data);

			if (currentGenerationIndex + 1 <desiredGenerationCount)
			{
				MoveDataToNextGeneration(data);
			}
		}
	}
}