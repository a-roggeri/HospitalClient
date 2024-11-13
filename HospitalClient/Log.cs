using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalClient
{
	internal class Log
	{
		static void MenuLog()
		{
			bool back = false;
			while (!back)
			{
				Console.Clear();
				Console.WriteLine("Gestione Log:");
				Console.WriteLine("1. Visualizza Log Eventi");
				Console.WriteLine("2. Visualizza Log Modifiche");
				Console.WriteLine("3. Torna al Menu Principale");
				Console.Write("Seleziona un'opzione: ");
				string choice = Console.ReadLine();

				switch (choice)
				{
					case "1":
						VisualizzaLogEventi();
						break;
					case "2":
						VisualizzaLogModifiche();
						break;
					case "3":
						back = true;
						break;
					default:
						Console.WriteLine("Opzione non valida. Premere un tasto per continuare...");
						Console.ReadKey();
						break;
				}
			}
		}
	}
}
