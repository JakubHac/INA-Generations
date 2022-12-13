using System;
using Eto.Drawing;
using Eto.Forms;

namespace INA_Generations
{
    public partial class MainForm : Form
    {
        
        private TabControl TabsControl;
        const int inputsSeparation = 30;
        private const string sliderPrecision = "0.00000000";

        private Action RefreshItemsSize;

        public MainForm()
        {
            Title = "INA Generacje Hac 19755";
            Width = 1260;
            Height = 400;
            MinimumSize = new Size(1220, 400);
            Resizable = true;

            RefreshItemsSize = () =>
            {
                if (TabsControl != null)
                {
                    TabsControl.Width = Width - 40;
                    TabsControl.Height = Height - 60;
                }
            };

            CreateGenetics();
            CreateGeneticsAnalysis();
            CreateClimbers();
            
            TabsControl = new TabControl()
            {
                Pages =
                {
                    RawDataPage, PlotPage, GroupedResultsPage, AnalysisResultsPage, ClimbersRawDataPage,
                    ClimbersPlotPage
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

            this.SizeChanged += (sender, args) => { RefreshItemsSize(); };
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
                            OutputTable
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
                            GroupedOutputTable
                        }
                    };
                    break;
                case 4:
                    ClimbersRawDataPage.Content = new StackLayout()
                    {
                        Orientation = Orientation.Vertical,
                        Padding = 10,
                        Items =
                        {
                            Climbers_Inputs,
                            ClimbersOutputTable,
                            ClimbersMultiOutputTable
                        }
                    };
                    break;
                case 5:
                    ClimbersPlotPage.Content = new StackLayout()
                    {
                        Orientation = Orientation.Vertical,
                        Padding = 10,
                        Items =
                        {
                            Climbers_Inputs,
                            ClimbersPlot
                        }
                    };
                    break;
            }
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
        
    }
}