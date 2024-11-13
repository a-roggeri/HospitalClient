using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalClient
{
	internal class Analysis
	{
		static void MenuAnalisi()
		{
			bool back = false;
			while (!back)
			{
				Console.Clear();
				Console.WriteLine("Analisi e Report:");
				Console.WriteLine("1. Visualizza Report Settimanale Appuntamenti");
				Console.WriteLine("2. Visualizza Analisi Mensile Pazienti");
				Console.WriteLine("3. Torna al Menu Principale");
				Console.Write("Seleziona un'opzione: ");
				string choice = Console.ReadLine();

				switch (choice)
				{
					case "1":
						VisualizzaReportSettimanaleAppuntamenti();
						break;
					case "2":
						VisualizzaAnalisiMensilePazienti();
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
