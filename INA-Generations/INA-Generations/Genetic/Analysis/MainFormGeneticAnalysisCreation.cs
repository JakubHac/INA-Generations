using System;
using System.Collections.Generic;
using Eto.Forms;

namespace INA_Generations
{
	public partial class MainForm : Form
	{
		private TextBox Analysis_AInput;
		private TextBox Analysis_BInput;
		private DropDown Analysis_DInput;
		private TextBox Analysis_NInput;
		private Button Analysis_StartButton;
		private GridView AnalysisOutputTable;
		private TextBox Analysis_PKValue;
		private TextBox Analysis_PMValue;
		private CheckBox Analysis_EliteCheckbox;
		private TextBox Analysis_IterInput;
		private TextBox Analysis_TInput;
		private TabPage AnalysisResultsPage;
		private StackLayout Analysis_Inputs;
		private DropDown Analysis_TargetFunctionDropdown;

		private void CreateGeneticsAnalysis()
		{
			Analysis_Inputs = CreateAnalysisInputs();
			
			CreateAnalysisOutputTable();
			
			AnalysisResultsPage = new()
			{
				Content = new StackLayout()
				{
					Orientation = Orientation.Vertical,
					Padding = 10,
					Items =
					{
						Analysis_Inputs,
						AnalysisOutputTable
					}
				},
				Text = "Analiza"
			};
			
			RefreshItemsSize += () =>
			{
				if (AnalysisOutputTable != null)
				{
					AnalysisOutputTable.Width = Width - 40 - 30;
					AnalysisOutputTable.Height = Height - 140 - 20;
				}
			};
		}
		
		private StackLayout CreateAnalysisInputs()
        {
            Analysis_AInput = new()
            {
                Text = "-4",
                Width = 40
            };
            Analysis_BInput = new()
            {
                Text = "12",
                Width = 40
            };
            Analysis_DInput = new()
            {
                Items = { "1", "0.1", "0.01", "0.001" },
                SelectedIndex = 3
            };
            Analysis_PKValue = new()
            {
                Text = "",
                Width = 150
            };

            for (int pk = 50; pk <= 90; pk += 10)
            {
                Analysis_PKValue.Text += $";{(((double)pk) / 100.0)}";
            }

            Analysis_PKValue.Text = Analysis_PKValue.Text.Substring(1);

            Analysis_PMValue = new()
            {
                Text =
                    $"{0.0001.ToString("0.0000")};{0.0005.ToString("0.0000")};{0.001.ToString("0.000")};{0.005.ToString("0.000")};{0.01.ToString("0.00")};{0.05.ToString("0.00")}",
                Width = 150
            };
            Analysis_NInput = new()
            {
                Text = "",
                Width = 150
            };

            for (int n = 30; n <= 80; n += 10)
            {
                Analysis_NInput.Text += $";{n}";
            }

            Analysis_NInput.Text = Analysis_NInput.Text.Substring(1);

            Analysis_TInput = new()
            {
                Text = "",
                Width = 150
            };
            for (int t = 50; t <= 150; t += 10)
            {
                Analysis_TInput.Text += $";{t}";
            }

            Analysis_TInput.Text = Analysis_TInput.Text.Substring(1);

            Analysis_IterInput = new()
            {
                Text = "100",
                ToolTip =
                    "Ilość iteracji dla każdej permutacji parametrów\nz których wynik jest brany jako wartość średnia\nżeby zredukować wpływ \"szczęścia\" na wynik",
                Width = 40
            };
            Analysis_StartButton = new()
            {
                Text = "Start",
                ToolTip = "Może potrwać bardzo długo!",
                Command = new Command((object sender, EventArgs e) => StartAnalysis())
            };
            Analysis_TargetFunctionDropdown = new()
            {
                Items = { "Maksimum", "Minimum" },
                SelectedIndex = 0
            };
            Analysis_EliteCheckbox = new()
            {
                Checked = true,
                ThreeState = false
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
		
		private void ClearAnalysisOutputTable()
		{
			AnalysisOutputTable.DataStore = Array.Empty<AnalysisDataRow>();
		}
		
		private void AddAnalysisDataToTable(List<AnalysisDataRow> dataAnalysis)
		{
			AnalysisOutputTable.DataStore = dataAnalysis;
		}
		
		private void CreateAnalysisOutputTable()
		{
			AnalysisOutputTable = new()
			{
				Width = Width - 42
			};
			
			AnalysisOutputTable.AddColumns<AnalysisDataRow>();
		}
	}
}