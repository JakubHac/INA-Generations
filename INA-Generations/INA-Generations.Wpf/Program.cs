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
			Singleton.Platform = Platforms.Wpf;
			new Application(Singleton.Platform).Run(new MainForm());
		}
	}
}