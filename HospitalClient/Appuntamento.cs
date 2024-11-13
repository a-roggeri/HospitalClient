using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalClient
{
	internal class Appuntamento
	{
		static void MenuAppuntamenti()
		{
			bool back = false;
			while (!back)
			{
				Console.Clear();
				Console.WriteLine("Gestione Appuntamenti:");
				Console.WriteLine("1. Crea Appuntamento");
				Console.WriteLine("2. Visualizza Appuntamenti");
				Console.WriteLine("3. Visualizza Dettagli Appuntamento");
				Console.WriteLine("4. Aggiorna Appuntamento");
				Console.WriteLine("5. Annulla Appuntamento");
				Console.WriteLine("6. Completa Appuntamento");
				Console.WriteLine("7. Visualizza Storico Appuntamenti");
				Console.WriteLine("8. Torna al Menu Principale");
				Console.Write("Seleziona un'opzione: ");
				string choice = Console.ReadLine();

				switch (choice)
				{
					case "1":
						CreaAppuntamento();
						break;
					case "2":
						VisualizzaAppuntamenti();
						break;
					case "3":
						Console.Write("Inserisci ID dell'appuntamento: ");
						int id = int.Parse(Console.ReadLine());
						VisualizzaDettagliAppuntamento(id);
						break;
					case "4":
						Console.Write("Inserisci ID dell'appuntamento: ");
						id = int.Parse(Console.ReadLine());
						AggiornaAppuntamento(id);
						break;
					case "5":
						Console.Write("Inserisci ID dell'appuntamento: ");
						id = int.Parse(Console.ReadLine());
						AnnullaAppuntamento(id);
						break;
					case "6":
						Console.Write("Inserisci ID dell'appuntamento: ");
						id = int.Parse(Console.ReadLine());
						CompletaAppuntamento(id);
						break;
					case "7":
						VisualizzaStoricoAppuntamenti();
						break;
					case "8":
						back = true;
						break;
					default:
						Console.WriteLine("Opzione non valida. Premere un tasto per continuare...");
						Console.ReadKey();
						break;
				}
			}
		}
		static void FetchAppuntamenti()
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();

			string query = @"SELECT p.nome AS paziente_nome, p.cognome AS paziente_cognome, per.nome AS personale_nome, per.cognome AS personale_cognome, a.data, a.ora, a.motivo, a.stato
                             FROM Appuntamenti a
                             JOIN Pazienti p ON a.paziente_id = p.ID
                             LEFT JOIN Personale per ON a.personale_id = per.ID";
			using var cmd = new NpgsqlCommand(query, connection);
			using var reader = cmd.ExecuteReader();

			Console.WriteLine("\nElenco Appuntamenti:");
			while (reader.Read())
			{
				Console.WriteLine($"{reader["paziente_nome"]} {reader["paziente_cognome"]} con {reader["personale_nome"]} {reader["personale_cognome"]}, Data: {reader["data"]}, Ora: {reader["ora"]}, Motivo: {reader["motivo"]}, Stato: {reader["stato"]}");
			}
			Console.WriteLine("Premere Invio per continuare.");
			Console.ReadLine();
		}
		static void PromptUpdateAppuntamentoStatus()
		{
			Console.Write("ID Appuntamento: ");
			int appuntamentoId = int.Parse(FetchInput("ID Appuntamento: "));

			Console.Write("Nuovo Stato (programmato/completato/annullato): ");
			string nuovoStato;
			while (!IsValidStatoAppuntamento(nuovoStato = Console.ReadLine().ToLower()))
			{
				Console.Write("Stato non valido. Inserisci uno stato valido (programmato/completato/annullato): ");
			}

			UpdateAppuntamentoStatus(appuntamentoId, nuovoStato);
		}

		static void UpdateAppuntamentoStatus(int appuntamentoId, string nuovoStato)
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();

			string query = "UPDATE Appuntamenti SET stato = @stato WHERE ID = @id";
			using var cmd = new NpgsqlCommand(query, connection);

			cmd.Parameters.AddWithValue("stato", nuovoStato);
			cmd.Parameters.AddWithValue("id", appuntamentoId);

			cmd.ExecuteNonQuery();
			Console.WriteLine($"Stato dell'appuntamento ID {appuntamentoId} aggiornato a '{nuovoStato}'.");
			Console.WriteLine("Premere Invio per continuare.");
			Console.ReadLine();
		}
		static void PromptAddAppuntamento()
		{
			Console.Write("ID Paziente: ");
			int pazienteId = int.Parse(Console.ReadLine());

			Console.Write("ID Personale: ");
			int personaleId = int.Parse(Console.ReadLine());

			Console.Write("Data (YYYY-MM-DD): ");
			DateTime data;
			while (!IsValidDate(Console.ReadLine(), out data))
			{
				Console.Write("Data non valida. Inserisci una data valida (YYYY-MM-DD): ");
			}

			Console.Write("Ora (HH:MM): ");
			TimeSpan ora;
			while (!TimeSpan.TryParse(Console.ReadLine(), out ora))
			{
				Console.Write("Ora non valida. Inserisci l'ora in formato HH:MM: ");
			}

			Console.Write("Motivo (lascia vuoto se non specificato): ");
			string motivo = Console.ReadLine();
			motivo = string.IsNullOrEmpty(motivo) ? null : motivo;

			Console.Write("Stato (programmato/completato/annullato): ");
			string stato;
			while (!IsValidStatoAppuntamento(stato = Console.ReadLine().ToLower()))
			{
				Console.Write("Stato non valido. Inserisci uno stato valido (programmato/completato/annullato): ");
			}

			AddAppuntamento(pazienteId, personaleId, data, ora, motivo, stato);
		}

		static void AddAppuntamento(int pazienteId, int personaleId, DateTime data, TimeSpan ora, string? motivo, string stato)
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();

			string query = @"INSERT INTO Appuntamenti (paziente_id, personale_id, data, ora, motivo, stato) 
                             VALUES (@pazienteId, @personaleId, @data, @ora, @motivo, @stato)";
			using var cmd = new NpgsqlCommand(query, connection);

			cmd.Parameters.AddWithValue("pazienteId", pazienteId);
			cmd.Parameters.AddWithValue("personaleId", personaleId);
			cmd.Parameters.AddWithValue("data", data);
			cmd.Parameters.AddWithValue("ora", ora);
			cmd.Parameters.AddWithValue("motivo", motivo ?? (object)DBNull.Value);
			cmd.Parameters.AddWithValue("stato", stato);

			cmd.ExecuteNonQuery();
			Console.WriteLine("Appuntamento aggiunto con successo.");
			Console.WriteLine("Premere Invio per continuare.");
			Console.ReadLine();
		}
	}
}
