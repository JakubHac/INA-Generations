using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;

namespace INA_Generations
{
	public class RouletteDialog : Dialog
	{
		public double Value = Singleton.Random.NextDouble();
		private ProgressBar Roulette;

		public RouletteDialog()
		{
			Width = 600;
			Height = 200;

			Roulette = new ProgressBar()
			{
				Width = 600,
				Height = 200
			};
			Roulette.MinValue = 0;
			Roulette.MaxValue = 100_000_000;
			Roulette.Value = 0;
			
			Content = new StackLayout()
			{
				Items =
				{
					Roulette
				}

			};
			
			Application.Instance.InvokeAsync(Animate);

			string PrintPair(KeyValuePair<object, object> pair)
			{
				string result = "";
				try
				{
					result += pair.Key.ToString();
				}
				catch (Exception e)
				{
					result += "could not get key";
				}

				result += " : ";
				
				try
				{
					result += pair.Value.ToString();
				}
				catch (Exception e)
				{
					result += "could not get value";
				}

				return result;
			}
		}

		public async Task Animate()
		{
			try
			{
				await Task.Delay(16);

				for (int i = 0; i < 100_000_000 / 2_000_000; i++)
				{
					Roulette.Value = i * 2_000_000;
					Roulette.Invalidate();
					await Task.Delay(16);
				}
			
				Roulette.Value = (int)Math.Round(Value * 100_000_000);
				Roulette.Invalidate();
				await Task.Delay(500);
				Close();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}

			
		}
	}
}