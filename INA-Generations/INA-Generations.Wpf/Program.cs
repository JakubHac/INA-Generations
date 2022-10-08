using System;
using Eto;
using Eto.Forms;

namespace INA_Generations.Wpf
{
	class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			new Application(Platforms.Wpf).Run(new MainForm());
		}
	}
}