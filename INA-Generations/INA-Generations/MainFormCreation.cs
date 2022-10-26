using System;
using System.Collections.ObjectModel;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;

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
		private Label LOutput;
		private Slider PKSlider;
		private TextBox PKValue;
		private Slider PMSlider;
		private TextBox PMValue;
		
		private DropDown RouletteTypeDropdown;
		private DropDown TargetFunctionDropdown;
		
		private Scrollable OutputTableScrollable;
		
		const int inputsSeparation = 30;

		public MainForm()
		{
			Title = "INA Generacje Hac 19755";
			Width = 1200;
			Height = 400;
			MinimumSize = new Size(1200, 400);
			Resizable = true;
			var inputs = CreateInputs();
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
			
			CreateOutputTable();
			
			Content = new StackLayout
			{
				Orientation = Orientation.Vertical,
				Padding = 10,
				Items =
				{
					inputs,
					new StackLayout()
					{
						Orientation = Orientation.Horizontal,
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
							TargetFunctionDropdown
						}
					},
					OutputTableScrollable
				}
			};
			
			this.SizeChanged += (sender, args) =>
			{
				if (OutputTable != null)
				{
					OutputTable.Width = Width - 42;
				}

				if (OutputTableScrollable != null)
				{
					OutputTableScrollable.Width = Width - 40;
					OutputTableScrollable.Height = Height - 180;
				}
			};
		}

		private void ClearOutputTable()
		{
			((ObservableCollection<DataRow>)OutputTable.DataStore).Clear();
			OutputTable.Invalidate();
		}

		private DataRow CreateDataRowForSpecimen(Specimen specimen, long lp)
		{
			DataRow dataRow = new DataRow(specimen, lp);
			((ObservableCollection<DataRow>)OutputTable.DataStore).Add(dataRow);
			return dataRow;
		}

		private void CreateOutputTable()
		{
			OutputTable = new GridView()
			{
				DataStore = new ObservableCollection<DataRow>(),
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
						Binding = Binding.Property<DataRow, string>(x => (((string, string value))property.GetValue(x)).value)
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
				Text = "10"
			};
			PKSlider = new Slider()
			{
				MinValue = 0,
				MaxValue = 100_000_000,
				Value = 50_000_000,
				Width = 150,
				ToolTip = "Prawdopodobieństwo Krzyżowania"
			};
			PKValue = new TextBox()
			{
				Text = 0.5.ToString("0.00"),
				Width = 50,
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
							PKValue.Text = 1.0.ToString("0.00");
							val = 1.0;
						}
						else if (val < 0)
						{
							PKValue.Text = 0.0.ToString("0.00");
							val = 0.0;
						}
						PKSlider.Value = (int)Math.Round(val * 100_000_000.0);
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
				MaxValue = 100_000_000,
				Value = 2_000_000,
				Width = 150,
				ToolTip = "Prawdopodobieństwo Mutacji pojedynczego bitu"
			};
			PMValue = new TextBox()
			{
				Text = 0.02.ToString("0.00"),
				Width = 50,
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
							PMValue.Text = 1.0.ToString("0.00");
							val = 1.0;
						}
						else if (val < 0)
						{
							PMValue.Text = 0.0.ToString("0.00");
							val = 0.0;
						}
						PMSlider.Value = (int)Math.Round(val * 100_000_000.0);
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
			PKValue.Text = (val / 100_000_000.0).ToString("0.00");
		}
		
		private void SyncPMValueToSlider()
		{
			double val = PMSlider.Value;
			PMValue.Text = (val / 100_000_000.0).ToString("0.00");
		}
	}
}