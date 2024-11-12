using System;
using Npgsql;
using System.Text.RegularExpressions;

namespace HospitalClient
{
	class Program
	{
		static string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=admin;Database=hospital_management";

		static void Main(string[] args)
		{
			string ruolo = AutenticazioneUtente();
			if (string.IsNullOrEmpty(ruolo))
			{
				Console.WriteLine("Accesso negato. Utente non trovato.");
				return;
			}

			bool exit = false;
			while (!exit)
			{
				RefreshAllMaterializedViews();
				Console.Clear();
				Console.WriteLine($"=== Sistema di Gestione Ospedaliera ===\nRuolo: {ruolo}");
				switch (ruolo)
				{
					case "Admin":
						exit = MenuAdmin();
						break;
					case "Medico":
						exit = MenuMedico();
						break;
					case "Infermiere":
						exit = MenuInfermiere();
						break;
					case "Receptionist":
						exit = MenuReceptionist();
						break;
					default:
						Console.WriteLine("Ruolo non riconosciuto. Accesso negato.");
						exit = true;
						break;
				}
			}
		}

		static void RefreshAllMaterializedViews()
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();

			string query1 = "REFRESH MATERIALIZED VIEW vista_ricoveri_settimanali;";
			using var cmd1 = new NpgsqlCommand(query1, connection);
			cmd1.ExecuteNonQuery();

			string query2 = "REFRESH MATERIALIZED VIEW analisi_mensile_pazienti;";
			using var cmd2 = new NpgsqlCommand(query2, connection);
			cmd2.ExecuteNonQuery();
		}

		static string AutenticazioneUtente()
		{
			string ruolo = null;
			while (string.IsNullOrEmpty(ruolo))
			{
				Console.Write("Inserisci il tuo Nome e Cognome: ");
				var input = Console.ReadLine();
				if (string.IsNullOrEmpty(input) || !input.Contains(" ")) continue;

				string[] parts = input.Split(" ");
				string nome = parts[0];
				string cognome = parts[1];

				using var connection = new NpgsqlConnection(connectionString);
				connection.Open();
				string query = "SELECT ruolo FROM Personale WHERE nome = @nome AND cognome = @cognome";
				using var cmd = new NpgsqlCommand(query, connection);
				cmd.Parameters.AddWithValue("nome", nome);
				cmd.Parameters.AddWithValue("cognome", cognome);

				var result = cmd.ExecuteScalar();
				if (result != null)
				{
					ruolo = result.ToString();
					Console.WriteLine($"Accesso effettuato come {ruolo}. Premere Invio per continuare.");
					Console.ReadLine();
				}
				else
				{
					Console.WriteLine("Utente non trovato. Riprova.");
				}
			}
			return ruolo;
		}

		static bool MenuAdmin()
		{
			Console.WriteLine("1. Visualizza Pazienti");
			Console.WriteLine("2. Aggiungi Paziente");
			Console.WriteLine("3. Visualizza Report Settimanale Appuntamenti");
			Console.WriteLine("4. Aggiorna Stato Appuntamento");
			Console.WriteLine("5. Aggiungi Appuntamento");
			Console.WriteLine("6. Visualizza Prescrizioni");
			Console.WriteLine("7. Visualizza Appuntamenti");
			Console.WriteLine("8. Visualizza Cartelle Cliniche");
			Console.WriteLine("9. Esci");
			Console.Write("Seleziona un'opzione: ");
			var choice = Console.ReadLine();

			switch (choice)
			{
				case "1":
					FetchPazienti();
					break;
				case "2":
					PromptAddPaziente();
					break;
				case "3":
					FetchReportSettimanale();
					break;
				case "4":
					PromptUpdateAppuntamentoStatus();
					break;
				case "5":
					PromptAddAppuntamento();
					break;
				case "6":
					FetchPrescrizioni();
					break;
				case "7":
					FetchAppuntamenti();
					break;
				case "8":
					FetchCartelleCliniche();
					break;
				case "9":
					return true;
				default:
					Console.WriteLine("Opzione non valida.");
					Console.ReadLine();
					break;
			}
			return false;
		}

		static bool MenuMedico()
		{
			Console.WriteLine("1. Visualizza Pazienti");
			Console.WriteLine("2. Visualizza Report Settimanale Appuntamenti");
			Console.WriteLine("3. Aggiungi Prescrizione");
			Console.WriteLine("4. Modifica Stato Appuntamento");
			Console.WriteLine("5. Visualizza Prescrizioni");
			Console.WriteLine("6. Visualizza Appuntamenti");
			Console.WriteLine("7. Visualizza Cartelle Cliniche");
			Console.WriteLine("8. Esci");
			Console.Write("Seleziona un'opzione: ");
			var choice = Console.ReadLine();

			switch (choice)
			{
				case "1":
					FetchPazienti();
					break;
				case "2":
					FetchReportSettimanale();
					break;
				case "3":
					PromptAddPrescrizione();
					break;
				case "4":
					PromptUpdateAppuntamentoStatus();
					break;
				case "5":
					FetchPrescrizioni();
					break;
				case "6":
					FetchAppuntamenti();
					break;
				case "7":
					FetchCartelleCliniche();
					break;
				case "8":
					return true;
				default:
					Console.WriteLine("Opzione non valida.");
					Console.ReadLine();
					break;
			}
			return false;
		}

		static bool MenuInfermiere()
		{
			Console.WriteLine("1. Visualizza Pazienti");
			Console.WriteLine("2. Aggiungi Prescrizione");
			Console.WriteLine("3. Modifica Stato Appuntamento");
			Console.WriteLine("4. Visualizza Prescrizioni");
			Console.WriteLine("5. Visualizza Appuntamenti");
			Console.WriteLine("6. Visualizza Cartelle Cliniche");
			Console.WriteLine("7. Esci");
			Console.Write("Seleziona un'opzione: ");
			var choice = Console.ReadLine();

			switch (choice)
			{
				case "1":
					FetchPazienti();
					break;
				case "2":
					PromptAddPrescrizione();
					break;
				case "3":
					PromptUpdateAppuntamentoStatus();
					break;
				case "4":
					FetchPrescrizioni();
					break;
				case "5":
					FetchAppuntamenti();
					break;
				case "6":
					FetchCartelleCliniche();
					break;
				case "7":
					return true;
				default:
					Console.WriteLine("Opzione non valida.");
					Console.ReadLine();
					break;
			}
			return false;
		}
		static string FetchInput(string prompt, Func<string, bool> check = null)
		{
			string? input = string.Empty;
			Console.Write(prompt);
			input = Console.ReadLine();
			if (check != null)
			{
				while (string.IsNullOrEmpty(input) || !check(input))
				{
					Console.Write("Dato mancante o errato. Riprovare:");
					input = Console.ReadLine();
				}
			}
			else
			{
				while (string.IsNullOrEmpty(input))
				{
					Console.Write("Dato mancante. Riprovare:");
					input = Console.ReadLine();
				}
			}
			
			return input;
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

		static bool MenuReceptionist()
		{
			Console.WriteLine("1. Visualizza Pazienti");
			Console.WriteLine("2. Aggiungi Appuntamento");
			Console.WriteLine("3. Modifica Stato Appuntamento");
			Console.WriteLine("4. Esci");
			Console.Write("Seleziona un'opzione: ");
			var choice = Console.ReadLine();

			switch (choice)
			{
				case "1":
					FetchPazienti();
					break;
				case "2":
					PromptAddAppuntamento();
					break;
				case "3":
					PromptUpdateAppuntamentoStatus();
					break;
				case "4":
					return true;
				default:
					Console.WriteLine("Opzione non valida.");
					Console.ReadLine();
					break;
			}
			return false;
		}

		static void FetchPazienti()
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();

			string query = "SELECT * FROM Pazienti";
			using var cmd = new NpgsqlCommand(query, connection);
			using var reader = cmd.ExecuteReader();

			Console.WriteLine("\nElenco Pazienti:");
			while (reader.Read())
			{
				Console.WriteLine($"{reader["nome"]} {reader["cognome"]}, Email: {reader["email"]}");
			}
			Console.WriteLine("Premere Invio per continuare.");
			Console.ReadLine();
		}

		static void PromptAddPaziente()
		{
			string nome = FetchInput("Nome: ");
			Console.Write("Cognome: ");
			string cognome = FetchInput("Cognome: ");

			Console.Write("Data di Nascita (YYYY-MM-DD): ");
			DateTime dataDiNascita;
			while (!IsValidDate(Console.ReadLine(), out dataDiNascita))
			{
				Console.Write("Data non valida. Inserisci una data valida (YYYY-MM-DD): ");
			}

			Console.Write("Sesso (M/F/O): ");
			string sesso;
			while (!IsValidSesso(sesso = Console.ReadLine().ToUpper()))
			{
				Console.Write("Sesso non valido. Inserisci M, F o O: ");
			}

			Console.Write("Indirizzo (lascia vuoto se non specificato): ");
			string? indirizzo = Console.ReadLine();
			indirizzo = string.IsNullOrEmpty(indirizzo) ? null : indirizzo;

			Console.Write("Telefono (solo numeri, da 10 a 15 cifre): ");
			string telefono;
			while (!IsValidPhone(telefono = Console.ReadLine()))
			{
				Console.Write("Numero di telefono non valido. Riprova: ");
			}

			Console.Write("Email: ");
			string email;
			while (!IsValidEmail(email = Console.ReadLine()))
			{
				Console.Write("Email non valida. Riprova: ");
			}

			AddPaziente(nome, cognome, dataDiNascita, sesso, indirizzo, telefono, email);
		}

		static void AddPaziente(string nome, string cognome, DateTime dataDiNascita, string sesso, string? indirizzo, string telefono, string email)
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();

			string query = @"INSERT INTO Pazienti (nome, cognome, data_di_nascita, sesso, indirizzo, telefono, email) 
                             VALUES (@nome, @cognome, @dataDiNascita, @sesso, @indirizzo, @telefono, @email)";
			using var cmd = new NpgsqlCommand(query, connection);

			cmd.Parameters.AddWithValue("nome", nome);
			cmd.Parameters.AddWithValue("cognome", cognome);
			cmd.Parameters.AddWithValue("dataDiNascita", dataDiNascita);
			cmd.Parameters.AddWithValue("sesso", sesso);
			cmd.Parameters.AddWithValue("indirizzo", indirizzo ?? (object)DBNull.Value);
			cmd.Parameters.AddWithValue("telefono", telefono);
			cmd.Parameters.AddWithValue("email", email);

			cmd.ExecuteNonQuery();
			Console.WriteLine($"Paziente {nome} {cognome} aggiunto con successo.");
			Console.WriteLine("Premere Invio per continuare.");
			Console.ReadLine();
		}

		static void FetchReportSettimanale()
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();

			string query = "SELECT * FROM report_settimanale_appuntamenti()";
			using var cmd = new NpgsqlCommand(query, connection);
			using var reader = cmd.ExecuteReader();

			Console.WriteLine("\nReport Settimanale degli Appuntamenti:");
			while (reader.Read())
			{
				string personaleNome = reader.GetString(reader.GetOrdinal("personale_nome"));
				string personaleCognome = reader.GetString(reader.GetOrdinal("personale_cognome"));
				long conteggioAppuntamenti = reader.GetInt64(reader.GetOrdinal("conteggio_appuntamenti"));
				Console.WriteLine($"{personaleNome} {personaleCognome} - Appuntamenti: {conteggioAppuntamenti}");
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

		static bool IsValidSesso(string sesso) => sesso == "M" || sesso == "F" || sesso == "O";
		static bool IsValidStatoAppuntamento(string stato) => Array.Exists(new[] { "programmato", "completato", "annullato" }, s => s.Equals(stato, StringComparison.OrdinalIgnoreCase));
		static bool IsValidEmail(string email) => Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
		static bool IsValidPhone(string phone) => Regex.IsMatch(phone, @"^[0-9]{10,15}$");
		static bool IsValidDate(string dateStr, out DateTime date) => DateTime.TryParse(dateStr, out date) && date <= DateTime.Today;
	}
}