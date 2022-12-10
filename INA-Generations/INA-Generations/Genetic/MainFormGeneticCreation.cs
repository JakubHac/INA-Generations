using System;
using System.Reflection;
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

		private void CreateGenetics()
		{
			Inputs = CreateInputs();
			SecondaryInputs = CreateSecondaryInputs();

			CreateOutputTable();
			CreateGroupedOutputTable();

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
						OutputTable
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
        
        private void AddDataToTable(DataRow[] data)
        {
	        OutputTable.DataStore = data;
        }

        private void AddGroupDataToTable(GroupDataRow[] dataGroups)
        {
	        GroupedOutputTable.DataStore = dataGroups;
        }
        
        private void CreateOutputTable()
        {
	        OutputTable = new GridView()
	        {
		        DataStore = new DataRow[0],
		        Width = Width - 42
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