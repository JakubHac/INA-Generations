using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Eto.Forms;

namespace INA_Generations
{
	public partial class MainForm
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
			
			List<DataRow> data = new List<DataRow>();
			
			for (int i = 0; i < n; i++)
			{
				Specimen specimen = new Specimen(a, b, i + 1, l, d);
				data.Add(CreateDataRowForSpecimen(specimen));
			}

			CalculateGx(data, d);
			CalculatePx(data);
			CalculateQx(data);
			Selection(data);
			
		}

		private void Selection(List<DataRow> data)
		{
			foreach (var row in data)
			{
				row.RandomizeSelection();
				int selectedIndex = data.Count - 1;
				double lastQ = 0.0;
				for (int i = 0; i < data.Count; i++)
				{
					if (data[i].QxValue >= row.SelectionRandom)
					{
						selectedIndex = i;
						break;
					}
				}

				row.SelectionValue = data[selectedIndex].OriginalSpecimen;
				OutputTable.Invalidate();
			}
		}

		private void CalculateQx(List<DataRow> data)
		{
			double sum = 0.0;
			foreach (var dataRow in data)
			{
				sum += dataRow.PxValue;
				dataRow.QxValue = sum;
			}
		}

		private void CalculatePx(List<DataRow> data)
		{
			double sum = data.Sum(x => x.GxValue);
			foreach (var dataRow in data)
			{
				dataRow.PxValue = dataRow.GxValue / sum;
			}
		}

		private static void CalculateGx(List<DataRow> data, double d)
		{
			if (Singleton.LookingForMax)
			{
				double min = data.Min(x => x.OriginalSpecimen.FxReal);
				foreach (var dataRow in data)
				{
					dataRow.GxValue = dataRow.OriginalSpecimen.FxReal - min + d;
				}
			}
			else
			{
				double max = data.Max(x => x.OriginalSpecimen.FxReal);
				foreach (var dataRow in data)
				{
					dataRow.GxValue = -(dataRow.OriginalSpecimen.FxReal - max) + d;
				}
			}
		}
	}

}