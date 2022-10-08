using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Eto;
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
			MinimumSize = new Size(700, 400);

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
					OutputTable
				}
			};
		}

		private void ClearOutputTable()
		{
			((ObservableCollection<Specimen>)OutputTable.DataStore).Clear();
		}

		private void AddSpecimenToOutput(Specimen specimen)
		{
			((ObservableCollection<Specimen>)OutputTable.DataStore).Add(specimen);
		}

		private void CreateOutputTable()
		{
			OutputTable = new GridView()
			{
				DataStore = new ObservableCollection<Specimen>(),
				Width = 600,
				Columns =
				{
					new GridColumn()
					{
						Width = 100,
						HeaderText = "n",
						DataCell = new TextBoxCell
						{
							Binding = Binding.Property<Specimen, string>(x => x.LP.ToString())
						}
					},
					new GridColumn()
					{
						Width = 100,
						HeaderText = "xReal",
						DataCell = new TextBoxCell
						{
							Binding = Binding.Property<Specimen, string>(x => x.xReal.ToString())
						}
					},
					new GridColumn()
					{
						Width = 100,
						HeaderText = "xReal -> xInt",
						DataCell = new TextBoxCell
						{
							Binding = Binding.Property<Specimen, string>(x => x.xInt_xReal.ToString())
						}
					},
					new GridColumn()
					{
						Width = 100,
						HeaderText = "xInt -> xBin",
						DataCell = new TextBoxCell
						{
							Binding = Binding.Property<Specimen, string>(x => x.xBin_xInt.ToString())
						}
					},
					new GridColumn()
					{
						Width = 100,
						HeaderText = "xBin -> xInt",
						DataCell = new TextBoxCell
						{
							Binding = Binding.Property<Specimen, string>(x => x.xInt_xBin.ToString())
						}
					},
					new GridColumn()
					{
						Width = 100,
						HeaderText = "xInt -> xReal",
						DataCell = new TextBoxCell
						{
							Binding = Binding.Property<Specimen, string>(x => x.xReal_xInt.ToString())
						}
					}
				},
			};
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