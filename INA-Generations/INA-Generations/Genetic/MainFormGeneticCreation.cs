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
			
		}
	}
}