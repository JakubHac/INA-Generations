using System;
using Eto;
using Eto.Forms;

namespace INA_Generations.Gtk
{
	class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			Singleton.Platform = Platforms.Gtk;
			new Application(Singleton.Platform).Run(new MainForm());
		}
	}
}