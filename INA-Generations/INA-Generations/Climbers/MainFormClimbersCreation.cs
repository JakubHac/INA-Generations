using Eto.Forms;
using ScottPlot.Eto;

namespace INA_Generations
{
	public partial class MainForm : Form
	{
		private TextBox Climbers_AInput;
		private TextBox Climbers_BInput;
		private DropDown Climbers_DInput;
		private Button Climbers_StartButton;
		private StackLayout ClimbersOutputColumns;
		
		private GridView ClimbersOutputTable;
		private GridView ClimbersMultiOutputTable;
		private bool ClimbersShowMultiOutputTable = false;
		
		private TextBox Climbers_TInput;
		
		private TabPage ClimbersRawDataPage;
		private TabPage ClimbersPlotPage;
		
		private PlotView ClimbersPlot;
		
		private StackLayout Climbers_Inputs;
		
		private DropDown Climbers_TargetFunctionDropdown;
	}
}