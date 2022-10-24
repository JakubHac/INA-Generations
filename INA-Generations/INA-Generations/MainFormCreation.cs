using System.Collections.ObjectModel;
using System.Linq;
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

		public MainForm()
		{
			Title = "INA Generacje Hac 19755";
			MinimumSize = new Size(900, 400);

			var inputs = CreateInputs();
			LOutput = new Label()
			{
				Text = "0"
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
							new Label()
							{
								Text = "L:"
							},
							LOutput
						}
					},
					new Scrollable()
					{
						Height = 250,
						Content = OutputTable
					}
				}
			};
		}

		private void ClearOutputTable()
		{
			((ObservableCollection<DataRow>)OutputTable.DataStore).Clear();
		}

		private void AddSpecimenToOutput(Specimen specimen)
		{
			((ObservableCollection<DataRow>)OutputTable.DataStore).Add(new DataRow(){OriginalSpecimen = specimen});
		}

		private void CreateOutputTable()
		{
			OutputTable = new GridView()
			{
				DataStore = new ObservableCollection<DataRow>(),
				Width = 800
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
			StartButton = new Button()
			{
				Text = "START",
				Command = new Command((sender, args) => ExecuteGeneration())
			};
			


			int inputsSeparation = 30;

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
					StartButton
				}
			};

			Label Label(string text)
			{
				return new Label()
				{
					Text = text
				};
			}

			Panel SeparationPanel()
			{
				return new Panel()
				{
					Width = inputsSeparation
				};
			}

			return inputs;
		}
	}
}