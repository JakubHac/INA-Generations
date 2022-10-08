using System;
using Eto.Forms;

namespace INA_Generations
{
	public partial class MainForm : Form
	{
		private void ExecuteGeneration()
		{
			if (!(
				    ParseHelper.ParseDouble(AInput.Text, "A", out double a) && 
				    ParseHelper.ParseDouble(BInput.Text, "B", out double b) && 
				    ParseHelper.ParseLong(NInput.Text, "N", out long n) &&
				    ParseHelper.ParseDouble(DInput.SelectedKey, "D", out double d, "en-US")
			    )
				)
			{
				return;
			}

			if (n < 0)
			{
				MessageBox.Show("N nie może być mniejsze od 0", MessageBoxType.Error);
				return;
			}
			
			int l = (int)Math.Floor(Math.Log((b-a)/d,2) + 1.0);
			LOutput.Text = l.ToString();
			ClearOutputTable();
			Specimen[] generation = new Specimen[n];
			for (int i = 0; i < n; i++)
			{
				Specimen specimen = new Specimen(a, b, i + 1, l, d);
				generation[i] = specimen;
				AddSpecimenToOutput(specimen);
			}
		}
		
	}

}