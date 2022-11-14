using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using ScottPlot.Eto;

namespace INA_Generations
{
	public partial class MainForm : Form
	{
		private TextBox AInput;
		private TextBox Analysis_AInput;
		private TextBox BInput;
		private TextBox Analysis_BInput;
		private DropDown DInput;
		private DropDown Analysis_DInput;
		private TextBox NInput;
		private TextBox Analysis_NInput;
		private Button StartButton;
		private Button Analysis_StartButton;
		private GridView OutputTable;
		private GridView GroupedOutputTable;
		private GridView AnalysisOutputTable;
		private Label LOutput;
		private Slider PKSlider;
		private TextBox PKValue;
		private TextBox Analysis_PKValue;
		private Slider PMSlider;
		private TextBox PMValue;
		private TextBox Analysis_PMValue;
		private CheckBox EliteCheckbox;
		private CheckBox Analysis_EliteCheckbox;
		private CheckBox BenchmarkCheckbox;
		private TextBox TInput;
		private TextBox Analysis_IterInput;
		private TextBox Analysis_TInput;
		private TabControl TabsControl;
		private TabPage RawDataPage;
		private TabPage PlotPage;
		private TabPage GroupedResultsPage;
		private TabPage AnalysisResultsPage;
		private PlotView Plot;
		private StackLayout Inputs;
		private StackLayout SecondaryInputs;
		private StackLayout Analysis_Inputs;

		private DropDown RouletteTypeDropdown;
		private DropDown TargetFunctionDropdown;
		private DropDown Analysis_TargetFunctionDropdown;

		private Scrollable OutputTableScrollable;
		private Scrollable GroupedOutputTableScrollable;
		private Scrollable AnalysisOutputTableScrollable;

		const int inputsSeparation = 30;

		private const string sliderPrecision = "0.00000000";

		public MainForm()
		{
			Title = "INA Generacje Hac 19755";
			Width = 1260;
			Height = 400;
			MinimumSize = new Size(1220, 400);
			Resizable = true;
			Inputs = CreateInputs();
			Analysis_Inputs = CreateAnalysisInputs();
			SecondaryInputs = CreateSecondaryInputs();
			CreateOutputTable();
			CreateGroupedOutputTable();
			CreateAnalysisOutputTable();
			Plot = new PlotView();
			Plot.Width = 600;
			Plot.Height = 400;

			RawDataPage = new TabPage()
			{
				Content = new StackLayout()
				{
					Orientation = Orientation.Vertical,
					Padding = 10,
					Items =
					{
						Inputs,
						SecondaryInputs,
						OutputTableScrollable
					}
				},
				Text = "Populacja"
			};
			PlotPage = new TabPage()
			{
				Content = new StackLayout()
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

			GroupedResultsPage = new TabPage()
			{
				Content = new StackLayout()
				{
					Orientation = Orientation.Vertical,
					Padding = 10,
					Items =
					{
						GroupedOutputTableScrollable
					}
				},
				Text = "Wyniki"
			};
			AnalysisResultsPage = new TabPage()
			{
				Content = new StackLayout()
				{
					Orientation = Orientation.Vertical,
					Padding = 10,
					Items =
					{
						Analysis_Inputs,
						AnalysisOutputTableScrollable
					}
				},
				Text = "Analiza"
			};

			TabsControl = new TabControl()
			{
				Pages =
				{
					RawDataPage, PlotPage, GroupedResultsPage, AnalysisResultsPage
				}
			};

			TabsControl.SelectedIndexChanged += TabsControlOnSelectedIndexChanged;

			Content = new StackLayout
			{
				Orientation = Orientation.Vertical,
				Padding = 10,
				Items =
				{
					TabsControl
				}
			};

			this.SizeChanged += (sender, args) =>
			{
				if (OutputTable != null)
				{
					OutputTable.Width = Width - 40 - 30 - 30;
				}

				if (GroupedOutputTable != null)
				{
					GroupedOutputTable.Width = Width - 40 - 30 - 30;
				}

				if (AnalysisOutputTable != null)
				{
					AnalysisOutputTable.Width = Width - 40 - 30 - 30;
				}

				if (Plot != null)
				{
					Plot.Width = Width - 40 - 30 - 30;
					Plot.Height = Height - 200;
				}

				if (OutputTableScrollable != null)
				{
					OutputTableScrollable.Width = Width - 40 - 30;
					OutputTableScrollable.Height = Height - 180 - 20;
				}

				if (GroupedOutputTableScrollable != null)
				{
					GroupedOutputTableScrollable.Width = Width - 40 - 30;
					GroupedOutputTableScrollable.Height = Height - 180 - 20;
				}

				if (AnalysisOutputTableScrollable != null)
				{
					AnalysisOutputTableScrollable.Width = Width - 40 - 30;
					AnalysisOutputTableScrollable.Height = Height - 140 - 20;
				}

				if (TabsControl != null)
				{
					TabsControl.Width = Width - 40;
					TabsControl.Height = Height - 60;
				}
			};
		}

		private StackLayout CreateAnalysisInputs()
		{
			Analysis_AInput = new TextBox()
			{
				Text = "-4",
				Width = 40
			};
			Analysis_BInput = new TextBox()
			{
				Text = "12",
				Width = 40
			};
			Analysis_DInput = new DropDown()
			{
				Items = { "1", "0.1", "0.01", "0.001" },
				SelectedIndex = 3
			};
			Analysis_PKValue = new TextBox()
			{
				Text = "",
				Width = 150
			};
			
			for (int pk = 50; pk <= 90; pk+=10)
			{
				Analysis_PKValue.Text += $";{(((double)pk)/100.0)}";
			}
			Analysis_PKValue.Text = Analysis_PKValue.Text.Substring(1);
			
			Analysis_PMValue = new TextBox()
			{
				Text = $"{0.0001.ToString("0.0000")};{0.0005.ToString("0.0000")};{0.001.ToString("0.000")};{0.005.ToString("0.000")};{0.01.ToString("0.00")};{0.05.ToString("0.00")}",
				Width = 150
			};
			Analysis_NInput = new TextBox()
			{
				Text = "",
				Width = 150
			};

			for (int n = 30; n <= 80; n+=10)
			{
				Analysis_NInput.Text += $";{n}";
			}
			Analysis_NInput.Text = Analysis_NInput.Text.Substring(1);
			
			Analysis_TInput = new TextBox()
			{
				Text = "",
				Width = 150
			};
			for (int t = 50; t <= 150; t+=10)
			{
				Analysis_TInput.Text += $";{t}";
			}
			Analysis_TInput.Text = Analysis_TInput.Text.Substring(1);
			
			Analysis_IterInput = new TextBox()
			{
				Text = "100",
				ToolTip =
					"Ilość iteracji dla każdej permutacji parametrów\nz których wynik jest brany jako wartość średnia\nżeby zredukować wpływ \"szczęścia\" na wynik",
				Width = 40
			};
			Analysis_StartButton = new Button()
			{
				Text = "Start",
				ToolTip = "Może potrwać bardzo długo!",
				Command = new Command((object sender, EventArgs e) => StartAnalysis())
			};
			Analysis_TargetFunctionDropdown = new DropDown()
			{
				Items = { "Maksimum", "Minimum" },
				SelectedIndex = 0
			};
			Analysis_EliteCheckbox = new CheckBox()
			{
				Checked = true,
				ThreeState = false
			};
			
			return new StackLayout()
			{
				Orientation = Orientation.Horizontal,
				AlignLabels = true,
				VerticalContentAlignment = VerticalAlignment.Center,
				HorizontalContentAlignment = HorizontalAlignment.Center,
				Padding = 10,
				Items =
				{
					Label("A:"),
					Analysis_AInput,
					Label("B:"),
					Analysis_BInput,
					Label("D:"),
					Analysis_DInput,
					Label("Elita"),
					Analysis_EliteCheckbox,
					Label("Cel"),
					Analysis_TargetFunctionDropdown,
					Label("PK[]:"),
					Analysis_PKValue,
					Label("PM[]:"),
					Analysis_PMValue,
					Label("N[]:"),
					Analysis_NInput,
					Label("T[]:"),
					Analysis_TInput,
					Label("Iters:", 
						"Ilość iteracji dla każdej permutacji parametrów\nz których wynik jest brany jako wartość średnia\nżeby zredukować wpływ \"szczęścia\" na wynik"),
					Analysis_IterInput,
					Analysis_StartButton
				}
			};
		}


		private void TabsControlOnSelectedIndexChanged(object sender, EventArgs e)
		{
			switch (TabsControl.SelectedIndex)
			{
				case 0:
					RawDataPage.Content = new StackLayout()
					{
						Orientation = Orientation.Vertical,
						Padding = 10,
						Items =
						{
							Inputs,
							SecondaryInputs,
							OutputTableScrollable
						}
					};
					break;
				case 1:
					PlotPage.Content = new StackLayout()
					{
						Orientation = Orientation.Vertical,
						Padding = 10,
						Items =
						{
							Inputs,
							SecondaryInputs,
							Plot
						}
					};
					break;
				case 2:
					GroupedResultsPage.Content = new StackLayout()
					{
						Orientation = Orientation.Vertical,
						Padding = 10,
						Items =
						{
							Inputs,
							SecondaryInputs,
							GroupedOutputTableScrollable
						}
					};
					break;
			}
		}

		private StackLayout CreateSecondaryInputs()
		{
			LOutput = new Label()
			{
				Text = "0"
			};

			RouletteTypeDropdown = new DropDown()
			{
				Items = { "Wyłączona", "Zakres (0;1)", "Koło Fortuny" },
				SelectedIndex = 0
			};
			TargetFunctionDropdown = new DropDown()
			{
				Items = { "Maksimum", "Minimum" },
				SelectedIndex = 0
			};

			TInput = new TextBox()
			{
				Text = "50"
			};

			EliteCheckbox = new CheckBox()
			{
				ThreeState = false,
				Checked = true
			};

			BenchmarkCheckbox = new CheckBox()
			{
				ThreeState = false,
				Checked = false
			};

			return new StackLayout()
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

		private void ClearOutputTable()
		{
			OutputTable.DataStore = Array.Empty<DataRow>();
		}

		private void ClearGroupOutputTable()
		{
			GroupedOutputTable.DataStore = Array.Empty<GroupDataRow>();
		}

		private void ClearAnalysisOutputTable()
		{
			AnalysisOutputTable.DataStore = Array.Empty<AnalysisDataRow>();
		}

		private void AddDataToTable(DataRow[] data)
		{
			OutputTable.DataStore = data;
		}

		private void AddGroupDataToTable(GroupDataRow[] dataGroups)
		{
			GroupedOutputTable.DataStore = dataGroups;
		}

		private void AddAnalysisDataToTable(List<AnalysisDataRow> dataAnalysis)
		{
			AnalysisOutputTable.DataStore = dataAnalysis;
		}

		private void CreateOutputTable()
		{
			OutputTable = new GridView()
			{
				DataStore = new DataRow[0],
				Width = Width - 42
			};

			OutputTableScrollable = new Scrollable()
			{
				Content = OutputTable,
				Width = Width - 40,
				Height = Height - 180
			};

			foreach (PropertyInfo property in typeof(DataRow).GetProperties())
			{
				if (property.PropertyType != typeof((string, string))) continue;

				OutputTable.Columns.Add(new GridColumn()
				{
					HeaderText = (((string title, string))property.GetValue(DataRow.Empty)).title,
					DataCell = new TextBoxCell()
					{
						Binding = Binding.Property<DataRow, string>(x =>
							(((string, string value))property.GetValue(x)).value)
					}
				});
			}
		}

		private void CreateGroupedOutputTable()
		{
			GroupedOutputTable = new GridView()
			{
				DataStore = new GroupDataRow[0],
				Width = Width - 42
			};

			GroupedOutputTableScrollable = new Scrollable()
			{
				Content = GroupedOutputTable,
				Width = Width - 40,
				Height = Height - 180
			};

			foreach (PropertyInfo property in typeof(GroupDataRow).GetProperties())
			{
				if (property.PropertyType != typeof((string, string))) continue;

				GroupedOutputTable.Columns.Add(new GridColumn()
				{
					HeaderText = (((string title, string))property.GetValue(GroupDataRow.Empty)).title,
					DataCell = new TextBoxCell()
					{
						Binding = Binding.Property<GroupDataRow, string>(x =>
							(((string, string value))property.GetValue(x)).value)
					}
				});
			}
		}

		private void CreateAnalysisOutputTable()
		{
			AnalysisOutputTable = new GridView()
			{
				DataStore = new AnalysisDataRow[0],
				Width = Width - 42
			};

			AnalysisOutputTableScrollable = new Scrollable()
			{
				Content = AnalysisOutputTable,
				Width = Width - 40,
				Height = Height - 180
			};

			foreach (PropertyInfo property in typeof(AnalysisDataRow).GetProperties())
			{
				if (property.PropertyType != typeof((string, string))) continue;

				AnalysisOutputTable.Columns.Add(new GridColumn()
				{
					HeaderText = (((string title, string))property.GetValue(AnalysisDataRow.Empty)).title,
					DataCell = new TextBoxCell()
					{
						Binding = Binding.Property<AnalysisDataRow, string>(x =>
							(((string, string value))property.GetValue(x)).value)
					}
				});
			}
		}

		private StackLayout CreateInputs()
		{
			AInput = new TextBox()
			{
				Text = "-4"
			};
			BInput = new TextBox()
			{
				Text = "12"
			};
			DInput = new DropDown()
			{
				Items = { "1", "0.1", "0.01", "0.001" },
				SelectedIndex = 3
			};
			NInput = new TextBox()
			{
				Text = "60"
			};
			PKSlider = new Slider()
			{
				MinValue = 0,
				MaxValue = 1_000_000_000,
				Value = (int)(1_000_000_000.0 * 0.8),
				Width = 150,
				ToolTip = "Prawdopodobieństwo Krzyżowania"
			};
			PKValue = new TextBox()
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

						PKSlider.Value = (int)Math.Round(val * (double)PKSlider.MaxValue);
					}
					catch (Exception e)
					{
						SyncPKValueToSlider();
					}
				}
			};
			PKSlider.ValueChanged += (sender, args) => SyncPKValueToSlider();

			PMSlider = new Slider()
			{
				MinValue = 0,
				MaxValue = 1_000_000_000,
				Value = (int)(1_000_000_000.0 * 0.005),
				Width = 150,
				ToolTip = "Prawdopodobieństwo Mutacji pojedynczego bitu"
			};
			PMValue = new TextBox()
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

						PMSlider.Value = (int)Math.Round(val * (double)PMSlider.MaxValue);
					}
					catch (Exception e)
					{
						SyncPMValueToSlider();
					}
				}
			};
			PMSlider.ValueChanged += (sender, args) => SyncPMValueToSlider();

			StartButton = new Button()
			{
				Text = "START",
				Command = new Command((sender, args) => ExecuteGeneration())
			};

			var inputs = new StackLayout()
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

		Label Label(string text, string tooltip = null)
		{
			if (tooltip == null)
			{
				return new Label()
				{
					Text = text
				};
			}

			return new Label()
			{
				Text = text,
				ToolTip = tooltip
			};
		}

		Panel SeparationPanel()
		{
			return new Panel()
			{
				Width = inputsSeparation
			};
		}

		private void SyncPKValueToSlider()
		{
			double val = PKSlider.Value;
			PKValue.Text = (val / (double)PKSlider.MaxValue).ToString(sliderPrecision);
		}

		private void SyncPMValueToSlider()
		{
			double val = PMSlider.Value;
			PMValue.Text = (val / (double)PMSlider.MaxValue).ToString(sliderPrecision);
		}
	}
}