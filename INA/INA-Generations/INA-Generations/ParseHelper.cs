using System;
using System.Globalization;
using Eto.Forms;

namespace INA_Generations
{
	public static class ParseHelper
	{
		public static bool ParseDouble(string text, string textName, out double output, string culture = "")
		{
			if (culture.Length == 0)
			{
				culture = CultureInfo.CurrentCulture.ToString();
			}
			
			try
			{
				output = Double.Parse(text, NumberStyles.Number, new CultureInfo(culture));
				return true;
			}
			catch (ArgumentNullException _)
			{
				MessageBox.Show($"{textName} jest puste", MessageBoxType.Error);
			}
			catch (FormatException _)
			{
				MessageBox.Show($"{textName} nie jest poprawnego formatu, przykład {12.1}", MessageBoxType.Error);
			}
			catch (OverflowException _)
			{
				MessageBox.Show($"{textName} musi być z przedziału od {double.MinValue} do {double.MaxValue}", MessageBoxType.Error);
			}
			output = Double.NaN;
			return false;
		}
		
		public static bool ParseLong(string text, string textName, out long output, string culture = "")
		{
			if (culture.Length == 0)
			{
				culture = CultureInfo.CurrentCulture.ToString();
			}

			try
			{
				output = long.Parse(text, NumberStyles.Integer, new CultureInfo(culture));
				return true;
			}
			catch (ArgumentNullException _)
			{
				MessageBox.Show($"{textName} jest puste", MessageBoxType.Error);
			}
			catch (ArgumentException _)
			{
				MessageBox.Show($"{textName} nie jest poprawnego formatu, przykład {10}", MessageBoxType.Error);
			}
			catch (FormatException _)
			{
				MessageBox.Show($"{textName} nie jest poprawnego formatu, przykład {10}", MessageBoxType.Error);
			}
			catch (OverflowException _)
			{
				MessageBox.Show($"{textName} musi być z przedziału od {long.MinValue} do {long.MaxValue}", MessageBoxType.Error);
			}
			output = 0;
			return false;
		}
		
		public static bool ParseInt(string text, string textName, out int output, string culture = "")
		{
			if (culture.Length == 0)
			{
				culture = CultureInfo.CurrentCulture.ToString();
			}

			try
			{
				output = int.Parse(text, NumberStyles.Integer, new CultureInfo(culture));
				return true;
			}
			catch (ArgumentNullException _)
			{
				MessageBox.Show($"{textName} jest puste", MessageBoxType.Error);
			}
			catch (ArgumentException _)
			{
				MessageBox.Show($"{textName} nie jest poprawnego formatu, przykład {10}", MessageBoxType.Error);
			}
			catch (FormatException _)
			{
				MessageBox.Show($"{textName} nie jest poprawnego formatu, przykład {10}", MessageBoxType.Error);
			}
			catch (OverflowException _)
			{
				MessageBox.Show($"{textName} musi być z przedziału od {int.MinValue} do {int.MaxValue}", MessageBoxType.Error);
			}
			output = 0;
			return false;
		}
	}
}