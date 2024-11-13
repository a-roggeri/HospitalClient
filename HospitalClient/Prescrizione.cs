using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalClient
{
	internal class Prescrizione
	{
		static void MenuPrescrizioni()
		{
			bool back = false;
			while (!back)
			{
				Console.Clear();
				Console.WriteLine("Gestione Prescrizioni:");
				Console.WriteLine("1. Crea Prescrizione");
				Console.WriteLine("2. Visualizza Prescrizioni");
				Console.WriteLine("3. Visualizza Dettagli Prescrizione");
				Console.WriteLine("4. Aggiorna Prescrizione");
				Console.WriteLine("5. Elimina Prescrizione");
				Console.WriteLine("6. Torna al Menu Principale");
				Console.Write("Seleziona un'opzione: ");
				string choice = Console.ReadLine();

				switch (choice)
				{
					case "1":
						CreaPrescrizione();
						break;
					case "2":
						VisualizzaPrescrizioni();
						break;
					case "3":
						Console.Write("Inserisci ID della prescrizione: ");
						int id = int.Parse(Console.ReadLine());
						VisualizzaDettagliPrescrizione(id);
						break;
					case "4":
						Console.Write("Inserisci ID della prescrizione: ");
						id = int.Parse(Console.ReadLine());
						AggiornaPrescrizione(id);
						break;
					case "5":
						Console.Write("Inserisci ID della prescrizione: ");
						id = int.Parse(Console.ReadLine());
						EliminaPrescrizione(id);
						break;
					case "6":
						back = true;
						break;
					default:
						Console.WriteLine("Opzione non valida. Premere un tasto per continuare...");
						Console.ReadKey();
						break;
				}
			}
		}
		static void FetchPrescrizioni()
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();

			string query = @"SELECT p.nome AS paziente_nome, p.cognome AS paziente_cognome, pr.farmaco, pr.dosaggio, pr.frequenza, pr.durata, pr.note
                             FROM Prescrizioni pr
                             JOIN Cartelle_Cliniche cc ON pr.cartella_clinica_id = cc.ID
                             JOIN Pazienti p ON cc.paziente_id = p.ID";
			using var cmd = new NpgsqlCommand(query, connection);
			using var reader = cmd.ExecuteReader();

			Console.WriteLine("\nElenco Prescrizioni:");
			while (reader.Read())
			{
				Console.WriteLine($"{reader["paziente_nome"]} {reader["paziente_cognome"]}, Farmaco: {reader["farmaco"]}, Dosaggio: {reader["dosaggio"]}, Frequenza: {reader["frequenza"]}, Durata: {reader["durata"]}, Note: {reader["note"]}");
			}
			Console.WriteLine("Premere Invio per continuare.");
			Console.ReadLine();
		}
		static void PromptAddPrescrizione()
		{
			Console.Write("ID Cartella Clinica: ");
			int cartellaClinicaId = int.Parse(Console.ReadLine());
			Console.Write("Farmaco: ");
			string farmaco = Console.ReadLine();
			Console.Write("Dosaggio: ");
			string dosaggio = Console.ReadLine();
			Console.Write("Frequenza: ");
			string frequenza = Console.ReadLine();
			Console.Write("Durata (in giorni, lascia vuoto se non specificato): ");
			string durataStr = Console.ReadLine();
			int? durata = string.IsNullOrEmpty(durataStr) ? (int?)null : int.Parse(durataStr);
			Console.Write("Note (lascia vuoto se non specificato): ");
			string note = Console.ReadLine();
			note = string.IsNullOrEmpty(note) ? null : note;

			AddPrescrizione(cartellaClinicaId, farmaco, dosaggio, frequenza, durata, note);
		}

		static void AddPrescrizione(int cartellaClinicaId, string farmaco, string dosaggio, string frequenza, int? durata, string? note)
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();

			string query = @"INSERT INTO Prescrizioni (cartella_clinica_id, farmaco, dosaggio, frequenza, durata, note) 
                             VALUES (@cartellaClinicaId, @farmaco, @dosaggio, @frequenza, @durata, @note)";
			using var cmd = new NpgsqlCommand(query, connection);

			cmd.Parameters.AddWithValue("cartellaClinicaId", cartellaClinicaId);
			cmd.Parameters.AddWithValue("farmaco", farmaco);
			cmd.Parameters.AddWithValue("dosaggio", dosaggio);
			cmd.Parameters.AddWithValue("frequenza", frequenza);
			cmd.Parameters.AddWithValue("durata", durata ?? (object)DBNull.Value);
			cmd.Parameters.AddWithValue("note", note ?? (object)DBNull.Value);

			cmd.ExecuteNonQuery();
			Console.WriteLine("Prescrizione aggiunta con successo.");
			Console.WriteLine("Premere Invio per continuare.");
			Console.ReadLine();
		}
	}
}
