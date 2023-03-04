using System;
using Eto.Forms;
using ScottPlot.Eto;

namespace INA_Generations
{
	public partial class MainForm : Form
	{
		private TextBox AInput;
		private TextBox BInput;
		private DropDown DInput;
		private TextBox NInput;
		private Button StartButton;
		private GridView OutputTable;
		private GridView GroupedOutputTable;
		private Label LOutput;
		private Slider PKSlider;
		private TextBox PKValue;
		private Slider PMSlider;
		private TextBox PMValue;
		private CheckBox EliteCheckbox;
		private CheckBox BenchmarkCheckbox;
		private TextBox TInput;
		private TabPage RawDataPage;
		private TabPage PlotPage;
		private TabPage GroupedResultsPage;
		private PlotView Plot;
		private StackLayout Inputs;
		private StackLayout SecondaryInputs;
		private DropDown RouletteTypeDropdown;
		private DropDown TargetFunctionDropdown;

		/// <summary>
		/// Creates all the controls needed for genetic algorithm
		/// </summary>
		private void CreateGenetics()
		{
			Inputs = CreateInputs();
			SecondaryInputs = CreateSecondaryInputs();

			CreateOutputTable();
			CreateGroupedOutputTable();

			Plot = new();
			Plot.Width = 600;
			Plot.Height = 400;
			RawDataPage = new()
			{
				Content = new StackLayout
				{
					Orientation = Orientation.Vertical,
					Padding = 10,
					Items =
					{
						Inputs,
						SecondaryInputs,
						OutputTable
					}
				},
				Text = "Populacja"
			};
			PlotPage = new()
			{
				Content = new StackLayout
				{
					Orientation = Orientation.Vertical,
					Padding = 10,
					Items =
					{
						Plot
					}
				},
				Text = "Wykres"
			};

			GroupedResultsPage = new()
			{
				Content = new StackLayout
				{
					Orientation = Orientation.Vertical,
					Padding = 10,
					Items =
					{
						GroupedOutputTable
					}
				},
				Text = "Wyniki"
			};

			RefreshItemsSize += () =>
			{
				if (OutputTable != null)
				{
					OutputTable.Width = Width - 40 - 30;
					OutputTable.Height = Height - 180 - 20;
				}

				if (GroupedOutputTable != null)
				{
					GroupedOutputTable.Width = Width - 40 - 30;
					GroupedOutputTable.Height = Height - 180 - 20;
				}

				if (Plot != null)
				{
					Plot.Width = Width - 40 - 30 - 30;
					Plot.Height = Height - 200;
				}
			};
		}

		/// <summary>
		/// Creates the secondary inputs for genetic algorithm
		/// </summary>
		/// <returns></returns>
		private StackLayout CreateSecondaryInputs()
		{
			LOutput = new()
			{
				Text = "0"
			};

			RouletteTypeDropdown = new()
			{
				Items = { "Wyłączona", "Zakres (0;1)", "Koło Fortuny" },
				SelectedIndex = 0
			};
			TargetFunctionDropdown = new()
			{
				Items = { "Maksimum", "Minimum" },
				SelectedIndex = 0
			};

			TInput = new()
			{
				Text = "50"
			};

			EliteCheckbox = new()
			{
				ThreeState = false,
				Checked = true
			};

			BenchmarkCheckbox = new()
			{
				ThreeState = false,
				Checked = false
			};

			return new()
			{
				Orientation = Orientation.Horizontal,
				AlignLabels = true,
				VerticalContentAlignment = VerticalAlignment.Center,
				HorizontalContentAlignment = HorizontalAlignment.Center,
				Padding = 10,
				Items =
				{
					Label("L:"),
					LOutput,
					SeparationPanel(),
					Label("Ruletka:"),
					RouletteTypeDropdown,
					SeparationPanel(),
					Label("Funkcja Celu:"),
					TargetFunctionDropdown,
					SeparationPanel(),
					Label("T:"),
					TInput,
					SeparationPanel(),
					Label("Elita:"),
					EliteCheckbox,
					SeparationPanel(),
					Label("Benchmark:"),
					BenchmarkCheckbox
				}
			};
		}

		/// <summary>
		/// Creates the output table for genetic algorithm
		/// </summary>
		private void CreateOutputTable()
		{
			OutputTable = new()
			{
				Width = Width - 42
			};
			OutputTable.AddColumns<DataRow>();
		}

		/// <summary>
		/// Creates the table for the grouped output of the genetic algorithm
		/// </summary>
		private void CreateGroupedOutputTable()
		{
			GroupedOutputTable = new()
			{
				Width = Width - 42
			};
			GroupedOutputTable.AddColumns<GroupDataRow>();
		}

		/// <summary>
		/// Creates the inputs for genetic algorithm
		/// </summary>
		/// <returns></returns>
		private StackLayout CreateInputs()
		{
			AInput = new()
			{
				Text = "-4"
			};
			BInput = new()
			{
				Text = "12"
			};
			DInput = new()
			{
				Items = { "1", "0.1", "0.01", "0.001" },
				SelectedIndex = 3
			};
			NInput = new()
			{
				Text = "60"
			};
			PKSlider = new()
			{
				MinValue = 0,
				MaxValue = 1_000_000_000,
				Value = (int)(1_000_000_000.0 * 0.8),
				Width = 150,
				ToolTip = "Prawdopodobieństwo Krzyżowania"
			};
			PKValue = new()
			{
				Text = 0.8.ToString(sliderPrecision),
				Width = 80,
				ToolTip = "Prawdopodobieństwo Krzyżowania"
			};

			PKValue.KeyDown += (sender, args) =>
			{
				if (args.Key == Keys.Enter)
				{
					try
					{
						double val = double.Parse(PKValue.Text);
						if (val > 1.0)
						{
							PKValue.Text = 1.0.ToString(sliderPrecision);
							val = 1.0;
						}
						else if (val < 0)
						{
							PKValue.Text = 0.0.ToString(sliderPrecision);
							val = 0.0;
						}

						PKSlider.Value = (int)Math.Round(val * PKSlider.MaxValue);
					}
					catch (Exception e)
					{
						SyncPKValueToSlider();
					}
				}
			};
			PKSlider.ValueChanged += (sender, args) => SyncPKValueToSlider();

			PMSlider = new()
			{
				MinValue = 0,
				MaxValue = 1_000_000_000,
				Value = (int)(1_000_000_000.0 * 0.005),
				Width = 150,
				ToolTip = "Prawdopodobieństwo Mutacji pojedynczego bitu"
			};
			PMValue = new()
			{
				Text = 0.005.ToString(sliderPrecision),
				Width = 80,
				ToolTip = "Prawdopodobieństwo Mutacji pojedynczego bitu"
			};

			PMValue.KeyDown += (sender, args) =>
			{
				if (args.Key == Keys.Enter)
				{
					try
					{
						double val = double.Parse(PMValue.Text);
						if (val > 1.0)
						{
							PMValue.Text = 1.0.ToString(sliderPrecision);
							val = 1.0;
						}
						else if (val < 0)
						{
							PMValue.Text = 0.0.ToString(sliderPrecision);
							val = 0.0;
						}

						PMSlider.Value = (int)Math.Round(val * PMSlider.MaxValue);
					}
					catch (Exception e)
					{
						SyncPMValueToSlider();
					}
				}
			};
			PMSlider.ValueChanged += (sender, args) => SyncPMValueToSlider();

			StartButton = new()
			{
				Text = "START",
				Command = new Command((sender, args) => ExecuteGeneration())
			};

			var inputs = new StackLayout
			{
				Orientation = Orientation.Horizontal,
				AlignLabels = true,
				VerticalContentAlignment = VerticalAlignment.Center,
				HorizontalContentAlignment = HorizontalAlignment.Center,
				Padding = 10,
				Items =
				{
					Label("A:"),
					AInput,
					SeparationPanel(),
					Label("B:"),
					BInput,
					SeparationPanel(),
					Label("D:"),
					DInput,
					SeparationPanel(),
					Label("N:"),
					NInput,
					SeparationPanel(),
					Label("PK", "Prawdopodobieństwo Krzyżowania"),
					PKSlider,
					PKValue,
					SeparationPanel(),
					Label("PM", "Prawdopodobieństwo Mutacji pojedynczego bitu"),
					PMSlider,
					PMValue,
					SeparationPanel(),
					StartButton
				}
			};


			return inputs;
		}

		/// <summary>
		/// Synchronizes the PK value with slider
		/// </summary>
		private void SyncPKValueToSlider()
		{
			double val = PKSlider.Value;
			PKValue.Text = (val / PKSlider.MaxValue).ToString(sliderPrecision);
		}

		/// <summary>
		/// Synchronizes the PM value with slider
		/// </summary>
		private void SyncPMValueToSlider()
		{
			double val = PMSlider.Value;
			PMValue.Text = (val / PMSlider.MaxValue).ToString(sliderPrecision);
		}
	}
}