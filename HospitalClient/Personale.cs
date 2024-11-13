using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalClient
{
	internal class Personale
	{
		static void MenuPersonale()
		{
			bool back = false;
			while (!back)
			{
				Console.Clear();
				Console.WriteLine("Gestione Personale:");
				Console.WriteLine("1. Crea Personale");
				Console.WriteLine("2. Visualizza Personale");
				Console.WriteLine("3. Visualizza Dettagli Personale");
				Console.WriteLine("4. Aggiorna Personale");
				Console.WriteLine("5. Elimina Personale");
				Console.WriteLine("6. Visualizza Report Settimanale Appuntamenti");
				Console.WriteLine("7. Torna al Menu Principale");
				Console.Write("Seleziona un'opzione: ");
				var choice = Console.ReadLine();

				switch (choice)
				{
					case "1":
						CreaPersonale();
						break;
					case "2":
						VisualizzaPersonale();
						break;
					case "3":
						Console.Write("Inserisci ID del personale: ");
						int id = int.Parse(Console.ReadLine());
						VisualizzaDettagliPersonale(id);
						break;
					case "4":
						Console.Write("Inserisci ID del personale: ");
						id = int.Parse(Console.ReadLine());
						AggiornaPersonale(id);
						break;
					case "5":
						Console.Write("Inserisci ID del personale: ");
						id = int.Parse(Console.ReadLine());
						EliminaPersonale(id);
						break;
					case "6":
						VisualizzaReportSettimanaleAppuntamenti();
						break;
					case "7":
						back = true;
						break;
					default:
						Console.WriteLine("Opzione non valida. Premere un tasto per continuare...");
						Console.ReadKey();
						break;
				}
			}
		}
		static void FetchPersonale()
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();

			string query = "SELECT * FROM Personale";
			using var cmd = new NpgsqlCommand(query, connection);
			using var reader = cmd.ExecuteReader();

			Console.WriteLine("\nElenco Personale:");
			while (reader.Read())
			{
				Console.WriteLine($"{reader["nome"]} {reader["cognome"]}, Email: {reader["email"]}, Ruolo: {reader["ruolo"]}, Reparto: {reader["reparto"]}");
			}
			Console.WriteLine("Premere Invio per continuare.");
			Console.ReadLine();
		}
	}
}
