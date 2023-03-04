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
		private List<(object obj, string displayName, Color color, double chance)> Chances = new();
		public double Value;
		public object Result;
		private Bitmap Bitmap;
		private Graphics Graphics;
		int imageSize = 600;

		public RouletteDialog()
		{
			Title = "Koło Fortuny Jakuba Hac. Wszelkie prawa zastrzeżone";
			Bitmap = new(imageSize, imageSize, PixelFormat.Format32bppRgb);
			Graphics = new(Bitmap);
			DrawWheel();
			
			Application.Instance.InvokeAsync(Animate);
		}
		
		public RouletteDialog(List<(object obj, string displayName, double chance)> chances)
		{
			Title = "Koło Fortuny Jakuba Hac. Wszelkie prawa zastrzeżone";
			Chances = chances.Select(x => (x.obj, x.displayName, new ColorHSB((float)Singleton.Random.NextDouble() * 360f, 1f, 1f).ToColor(), x.chance)).ToList();
			Bitmap = new(imageSize, imageSize, PixelFormat.Format32bppRgb);
			Graphics = new(Bitmap);
			DrawWheel();
			
			Application.Instance.InvokeAsync(Animate);
		}

		/// <summary>
		/// Draws the wheel with the given offset
		/// </summary>
		/// <param name="offset">offset angle in degrees</param>
		private void DrawWheel(double offset = 0f)
		{
			Graphics.Clear(Colors.White);
			double result = 0.0;

			if (Chances.Count > 0)
			{
				float angle = (float)offset;
				for (int i = 0; i < Chances.Count; i += 1)
				{
					float sweepAngle = (float)(Chances[i].chance * 360.0);
					Graphics.FillPie(Chances[i].color, 0f, 0f, imageSize, imageSize, angle, sweepAngle);
					angle += sweepAngle;
				}
				
				Graphics.DrawEllipse(Colors.Black, 0, 0, imageSize, imageSize);
				Graphics.DrawLine(Colors.Black, imageSize / 2f, 0f, imageSize / 2f, 100);
				var tmp_result = (1.0 - ((offset + 90.0) % 360.0 / 360.0));
				result = tmp_result;
				(object obj, string displayName, Color color, double chance) selected = (null, "ERROR", Colors.Black, 0f);
				for (int i = 0; i < Chances.Count; i++)
				{
					if (tmp_result <= Chances[i].chance)
					{
						selected = Chances[i];
						break;
					}

					tmp_result -= Chances[i].chance;
				}
				Result = selected.obj;				
				Graphics.DrawText(Fonts.Monospace(36f), Colors.Black, imageSize/2f - (selected.displayName.Length * 36f * 0.42f), 150f, selected.displayName);
			}
			else
			{
				for (int i = 0; i < 360; i += 3)
				{
					Graphics.FillPie(new Color(i / 360f, i / 360f, i / 360f), 0f, 0f, imageSize, imageSize, i + (float)offset, 3.5f);
				}
				Graphics.DrawEllipse(Colors.Black, 0, 0, imageSize, imageSize);
				Graphics.DrawLine(Colors.Red, imageSize / 2f, 0f, imageSize / 2f, 100);
				result = (1.0 - ((offset + 90.0) % 360.0 / 360.0));
				Graphics.DrawText(Fonts.Monospace(36f), Colors.Red, imageSize/2f - 150f, 150, $"{result:0.00000000}" );
			}
			Graphics.Flush();
			Content = new ImageView
			{
				Image = Bitmap
			};
			Value = result;
		}

		/// <summary>
		/// Animate the wheel
		/// </summary>
		public async Task Animate()
		{
			try
			{
				await Task.Delay(16);

				double angleChange = Singleton.Random.NextDouble() * 60.0 + 60.0;
				double angle = Singleton.Random.NextDouble() * 360.0;

				while (angleChange > 0.0)
				{
					angle += angleChange;
					angleChange -= 1.0 + Singleton.Random.NextDouble();
					DrawWheel(angle);
					await Task.Delay(16);
				}
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