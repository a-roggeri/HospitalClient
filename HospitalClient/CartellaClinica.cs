using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalClient
{
	internal class CartellaClinica
	{
		static void MenuCartelleCliniche()
		{
			bool back = false;
			while (!back)
			{
				Console.Clear();
				Console.WriteLine("Gestione Cartelle Cliniche:");
				Console.WriteLine("1. Crea Cartella Clinica");
				Console.WriteLine("2. Visualizza Cartelle Cliniche");
				Console.WriteLine("3. Visualizza Dettagli Cartella Clinica");
				Console.WriteLine("4. Aggiorna Cartella Clinica");
				Console.WriteLine("5. Elimina Cartella Clinica");
				Console.WriteLine("6. Visualizza Ricoveri Settimanali");
				Console.WriteLine("7. Torna al Menu Principale");
				Console.Write("Seleziona un'opzione: ");
				var choice = Console.ReadLine();

				switch (choice)
				{
					case "1":
						CreaCartellaClinica();
						break;
					case "2":
						VisualizzaCartelleCliniche();
						break;
					case "3":
						Console.Write("Inserisci ID della cartella clinica: ");
						int id = int.Parse(Console.ReadLine());
						VisualizzaDettagliCartellaClinica(id);
						break;
					case "4":
						Console.Write("Inserisci ID della cartella clinica: ");
						id = int.Parse(Console.ReadLine());
						AggiornaCartellaClinica(id);
						break;
					case "5":
						Console.Write("Inserisci ID della cartella clinica: ");
						id = int.Parse(Console.ReadLine());
						EliminaCartellaClinica(id);
						break;
					case "6":
						VisualizzaRicoveriSettimanali();
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
		static void FetchCartelleCliniche()
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();

			string query = @"SELECT p.nome AS paziente_nome, p.cognome AS paziente_cognome, cc.data_creazione, cc.note_mediche, cc.stato_attuale
                             FROM Cartelle_Cliniche cc
                             JOIN Pazienti p ON cc.paziente_id = p.ID";
			using var cmd = new NpgsqlCommand(query, connection);
			using var reader = cmd.ExecuteReader();

			Console.WriteLine("\nElenco Cartelle Cliniche:");
			while (reader.Read())
			{
				Console.WriteLine($"{reader["paziente_nome"]} {reader["paziente_cognome"]}, Data Creazione: {reader["data_creazione"]}, Note: {reader["note_mediche"]}, Stato Attuale: {reader["stato_attuale"]}");
			}
			Console.WriteLine("Premere Invio per continuare.");
			Console.ReadLine();
		}
	}
}
