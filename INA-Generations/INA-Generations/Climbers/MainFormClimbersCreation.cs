using System;
using System.Collections.ObjectModel;
using Eto.Forms;
using ScottPlot.Eto;

namespace INA_Generations
{
	public partial class MainForm : Form
	{
		private TextBox Climbers_AInput;
		private TextBox Climbers_BInput;
		private TextBox Climbers_IInput;
		private DropDown Climbers_DInput;
		private Button Climbers_StartButton;
		private StackLayout ClimbersOutputColumns;
		private GridView ClimbersOutputTable;
		private GridView ClimbersMultiOutputTable;
		private bool ClimbersShowMultiOutputTable = false;

		private TextBox Climbers_TInput;
		private CheckBox Climbers_Analysis;

		private TabPage ClimbersRawDataPage;
		private TabPage ClimbersPlotPage;

		private PlotView ClimbersPlot;

		private StackLayout Climbers_Inputs;

		private DropDown Climbers_TargetFunctionDropdown;

		private void CreateClimbers()
		{
			Climbers_Inputs = CreateClimbersInputs();

			CreateClimbersOutputTable();
			CreateClimbersMultiOutputTable();

			ClimbersPlot = new();
			ClimbersPlot.Width = 600;
			ClimbersPlot.Height = 400;

			ClimbersMultiOutputTable.Visible = false;

			ClimbersRawDataPage = new()
			{
				Content = new StackLayout()
				{
					Orientation = Orientation.Vertical,
					Padding = 10,
					Items =
					{
						Climbers_Inputs,
						ClimbersOutputTable,
						ClimbersMultiOutputTable
					}
				},
				Text = "Wspinacze"
			};

			ClimbersPlotPage = new()
			{
				Content = new StackLayout()
				{
					Orientation = Orientation.Vertical,
					Padding = 10,
					Items =
					{
						ClimbersPlot
					}
				},
				Text = "Wykres Wspinaczy"
			};
			
			RefreshItemsSize += () =>
			{
				if (ClimbersOutputTable != null)
				{
					ClimbersOutputTable.Width = Width - 70;
					ClimbersOutputTable.Height = Height - 180;
				}

				if (ClimbersMultiOutputTable != null)
				{
					ClimbersMultiOutputTable.Width = Width - 70;
					ClimbersMultiOutputTable.Height = Height - 180;
				}

				if (ClimbersPlot != null)
				{
					ClimbersPlot.Width = Width - 40 - 30 - 30;
					ClimbersPlot.Height = Height - 200;
				}
			};
		}
		
		private void CreateClimbersMultiOutputTable()
        {
            ClimbersMultiOutputTable = new()
            {
                DataStore = new ObservableCollection<ClimbersOutput>(),
                Width = Width - 42,
                Columns =
                {
                    new GridColumn()
                    {
                        HeaderText = "Kroki",
                        DataCell = new TextBoxCell()
                        {
                            Binding = Binding.Property<ClimbersOutput, string>(x => x.NumberOfSteps.ToString())
                        }
                    },
                    new GridColumn()
                    {
                        HeaderText = "Ilość Rozwiązań",
                        DataCell = new TextBoxCell()
                        {
                            Binding = Binding.Property<ClimbersOutput, string>(x => x.NumberOfSolutions.ToString())
                        }
                    },
                    new GridColumn()
                    {
                        HeaderText = "Procent Ilości Rozwiązań",
                        DataCell = new TextBoxCell()
                        {
                            Binding = Binding.Property<ClimbersOutput, string>(x => x.HitPercent.ToString("P"))
                        }
                    },
                    new GridColumn()
                    {
                        HeaderText = "Kumulatywna Ilość rozwiązań",
                        DataCell = new TextBoxCell()
                        {
                            Binding = Binding.Property<ClimbersOutput, string>(x =>
                                x.AggregateNumberOfSolutions.ToString())
                        }
                    },
                    new GridColumn()
                    {
                        HeaderText = "Kumulatywny Procent Rozwiązań",
                        DataCell = new TextBoxCell()
                        {
                            Binding = Binding.Property<ClimbersOutput, string>(x => x.AggregateHitPercent.ToString("P"))
                        }
                    }
                }
            };
        }

        private StackLayout CreateClimbersInputs()
        {
            Climbers_AInput = new()
            {
                Text = "-4",
                Width = 50
            };
            Climbers_BInput = new()
            {
                Text = "12",
                Width = 50
            };
            Climbers_DInput = new()
            {
                Items = { "1", "0.1", "0.01", "0.001" },
                SelectedIndex = 3
            };
            Climbers_TInput = new()
            {
                Text = "100",
                Width = 50
            };
            Climbers_IInput = new()
            {
	            Text = "100",
	            Width = 50,
	            ToolTip = "Iteracje analizy dla każdego T"
            };
            Climbers_Analysis = new()
            {
	            ThreeState = false,
	            Checked = true,
	            ToolTip = "Analiza od 1 do T, ma sens tylko dla T > 1"
            };
            Climbers_StartButton = new()
            {
                Text = "Start",
                Command = new Command((object sender, EventArgs e) => ExecuteClimbers())
            };
            Climbers_TargetFunctionDropdown = new()
            {
                Items = { "Maksimum", "Minimum" },
                SelectedIndex = 0
            };

            return new()
            {
                Orientation = Orientation.Vertical,
                Items =
                {
                    new StackLayout()
                    {
                        Orientation = Orientation.Horizontal,
                        AlignLabels = true,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        Padding = 10,
                        Items =
                        {
                            Label("A:"),
                            Climbers_AInput,
                            Label("B:"),
                            Climbers_BInput,
                            Label("D:"),
                            Climbers_DInput,
                            Label("T:"),
                            Climbers_TInput,
                            Label("Cel:"),
                            Climbers_TargetFunctionDropdown,
                            Label("Analiza:", "Analiza od 1 do T, ma sens tylko dla T > 1"),
                            Climbers_Analysis,
                            Label("Iteracje:", "Iteracje analizy dla każdego T"),
                            Climbers_IInput,
                            SeparationPanel(),
                            Climbers_StartButton
                        }
                    },
                    Label(
                        "Dla T = 1, wykres jest do pętli wewnętrznej, dla T > 1 wykres jest dla pętli zewnętrznej")
                }
            };
        }

        public void CreateClimbersOutputTable()
        {
            ClimbersOutputTable = new()
            {
                DataStore = new ObservableCollection<Specimen>(),
                Width = Width - 42,
                Columns =
                {
                    new GridColumn()
                    {
                        HeaderText = "xReal",
                        DataCell = new TextBoxCell()
                        {
                            Binding = Binding.Property<Specimen, string>(x => x.xReal.ToString())
                        }
                    },
                    new GridColumn()
                    {
                        HeaderText = "xBin",
                        DataCell = new TextBoxCell()
                        {
                            Binding = Binding.Property<Specimen, string>(x => x.XBin.ToString())
                        }
                    },
                    new GridColumn()
                    {
                        HeaderText = "F(x)",
                        DataCell = new TextBoxCell()
                        {
                            Binding = Binding.Property<Specimen, string>(x => x.Fx.ToString())
                        }
                    }
                }
            };
        }
	}
}