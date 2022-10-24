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
		public double Value;
		private Bitmap Bitmap;
		private Graphics Graphics;
		int imageSize = 600;

		public RouletteDialog()
		{
			Title = "Koło Fortuny Jakuba Hac. Wszelkie prawa zastrzeżone";
			Bitmap = new Bitmap(imageSize, imageSize, PixelFormat.Format32bppRgb);
			Graphics = new Graphics(Bitmap);
			DrawWheel();
			
			Application.Instance.InvokeAsync(Animate);
		}

		private double DrawWheel(double offset = 0f)
		{
			Graphics.Clear(Colors.White);
			for (int j = 0; j < 360; j += 3)
			{
				Graphics.FillPie(new Color(j / 360f, j / 360f, j / 360f), 0f, 0f, imageSize, imageSize, j + (float)offset, 3.5f);
			}

			Graphics.DrawEllipse(Colors.Black, 0, 0, imageSize, imageSize);
			Graphics.DrawLine(Colors.Red, imageSize / 2f, 0f, imageSize / 2f, 100);
			double result = (1.0 - ((offset + 90.0) % 360.0 / 360.0));
			Graphics.DrawText(Fonts.Monospace(36f), Colors.Red, imageSize/2f - 150f, 150, $"{result:0.00000000}" );
			Graphics.Flush();
			Content = new ImageView()
			{
				Image = Bitmap
			};
			return result;
		}

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
					Value = DrawWheel(angle);
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