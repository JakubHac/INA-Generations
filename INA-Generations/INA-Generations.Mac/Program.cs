using System;
using Eto.Forms;

namespace INA_Generations.Mac
{
	class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			Singleton.Platform = Eto.Platforms.Mac64;
			new Application(Singleton.Platform).Run(new MainForm());
		}
	}
}