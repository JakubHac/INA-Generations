using System;
using System.Globalization;
using System.Linq;
using Eto.Forms;

namespace INA_Generations
{
	public static class ParseHelper
	{
		/// <summary>
		/// Parses a double from a string
		/// </summary>
		/// <param name="text">string we are parsing</param>
		/// <param name="textName">name of the textBox this string comes from</param>
		/// <param name="output">output value</param>
		/// <param name="culture">culture override, if empty uses Current Culture</param>
		/// <returns>true if the parsing was successful, false if the parsing failed</returns>
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

		/// <summary>
		/// Parses a long[] from a string
		/// </summary>
		/// <param name="text">string we are parsing</param>
		/// <param name="fieldName">name of the textBox this string comes from</param>
		/// <param name="longArray">output value</param>
		/// <param name="errorMessage">error message to show if the parsing failed</param>
		/// <returns>true if the parsing was successful, false if the parsing failed</returns>
		public static bool ParseLongArray(string text, string fieldName, string errorMessage, out long[] longArray)
		{
			longArray = Array.Empty<long>();
			try
			{
				longArray = text.Split(';').Where(x => x.Trim().Length > 0).Select(x =>
				{
					ParseLong(x, fieldName, out long n);
					return n;
				}).ToArray();

				return true;
			}
			catch (Exception e)
			{
				MessageBox.Show(errorMessage);
				return false;
			}
		}

		/// <summary>
		/// Parses a double[] from a string
		/// </summary>
		/// <param name="text">string we are parsing</param>
		/// <param name="fieldName">name of the textBox this string comes from</param>
		/// <param name="doubleArray">output value</param>
		/// <param name="errorMessage">error message to show if the parsing failed</param>
		/// <returns>true if the parsing was successful, false if the parsing failed</returns>
		public static bool ParseDoubleArray(string text, string fieldName, string errorMessage, out double[] doubleArray)
		{
			doubleArray = Array.Empty<double>();
			try
			{
				doubleArray = text.Split(';').Where(x => x.Trim().Length > 0).Select(x =>
				{
					ParseDouble(x, fieldName, out double n);
					return n;
				}).ToArray();

				return true;
			}
			catch (Exception e)
			{
				MessageBox.Show(errorMessage);
				return false;
			}
		}
		
		/// <summary>
		/// Parses a long from a string
		/// </summary>
		/// <param name="text">string we are parsing</param>
		/// <param name="textName">name of the textBox this string comes from</param>
		/// <param name="output">output value</param>
		/// <param name="culture">culture override, if empty uses Current Culture</param>
		/// <returns>true if the parsing was successful, false if the parsing failed</returns>
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
	}
}