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
			bool exit = false;
			while (!exit)
			{
				string? ruolo = AutenticazioneUtente();
				RefreshAllMaterializedViews();
				Console.Clear();
				Console.WriteLine($"=== Sistema di Gestione Ospedaliera ===\nRuolo: {ruolo}");
				switch (ruolo)
				{
					case "ADMIN":
						MenuAdmin();
						break;
					case "MEDICO":
						MenuMedico();
						break;
					case "INFERMIERE":
						MenuInfermiere();
						break;
					case "RECEPTIONIST":
						MenuReceptionist();
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

		static string? AutenticazioneUtente()
		{
			string? ruolo = null;
			while (string.IsNullOrEmpty(ruolo))
			{
				Console.Write("Inserisci il tuo nome e cognome (lascia vuoto per uscire): ");
				var input = Console.ReadLine();
				if (string.IsNullOrEmpty(input) || !input.Contains(" ")) return null;

				string[] parts = input.Split(" ");
				string nome = parts[0];
				string cognome = parts[1];

				using var connection = new NpgsqlConnection(connectionString);
				connection.Open();
				string query = "SELECT ruolo FROM Personale WHERE UPPER(nome) = UPPER(@nome) AND UPPER(cognome) = UPPER(@cognome)";
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
		static void MenuAdmin()
		{
			bool back = false;
			while (!back)
			{
				Console.Clear();
				Console.WriteLine("Menu Admin:");
				Console.WriteLine("Pazienti:");
				Console.WriteLine("  1. Crea Paziente");
				Console.WriteLine("  2. Visualizza Pazienti");
				Console.WriteLine("  3. Visualizza Dettagli Paziente");
				Console.WriteLine("  4. Aggiorna Paziente");
				Console.WriteLine("  5. Elimina Paziente");
				Console.WriteLine("  6. Visualizza Analisi Mensile Pazienti");

				Console.WriteLine("\nPersonale:");
				Console.WriteLine("  7. Crea Personale");
				Console.WriteLine("  8. Visualizza Personale");
				Console.WriteLine("  9. Visualizza Dettagli Personale");
				Console.WriteLine("  10. Aggiorna Personale");
				Console.WriteLine("  11. Elimina Personale");
				Console.WriteLine("  12. Visualizza Report Settimanale Appuntamenti");

				Console.WriteLine("\nCartelle Cliniche:");
				Console.WriteLine("  13. Crea Cartella Clinica");
				Console.WriteLine("  14. Visualizza Cartelle Cliniche");
				Console.WriteLine("  15. Visualizza Dettagli Cartella Clinica");
				Console.WriteLine("  16. Aggiorna Cartella Clinica");
				Console.WriteLine("  17. Elimina Cartella Clinica");
				Console.WriteLine("  18. Visualizza Ricoveri Settimanali");

				Console.WriteLine("\nAppuntamenti:");
				Console.WriteLine("  19. Crea Appuntamento");
				Console.WriteLine("  20. Visualizza Appuntamenti");
				Console.WriteLine("  21. Visualizza Dettagli Appuntamento");
				Console.WriteLine("  22. Aggiorna Appuntamento");
				Console.WriteLine("  23. Annulla Appuntamento");
				Console.WriteLine("  24. Completa Appuntamento");
				Console.WriteLine("  25. Visualizza Storico Appuntamenti");

				Console.WriteLine("\nPrescrizioni:");
				Console.WriteLine("  26. Crea Prescrizione");
				Console.WriteLine("  27. Visualizza Prescrizioni");
				Console.WriteLine("  28. Visualizza Dettagli Prescrizione");
				Console.WriteLine("  29. Aggiorna Prescrizione");
				Console.WriteLine("  30. Elimina Prescrizione");

				Console.WriteLine("\nLog:");
				Console.WriteLine("  31. Visualizza Log Eventi");
				Console.WriteLine("  32. Visualizza Log Modifiche");

				Console.WriteLine("\nAnalisi e Report:");
				Console.WriteLine("  33. Visualizza Report Settimanale Appuntamenti");
				Console.WriteLine("  34. Visualizza Analisi Mensile Pazienti");

				Console.WriteLine("\n35. Torna indietro");
				Console.Write("Seleziona un'opzione: ");
				var choice = Console.ReadLine();

				switch (choice)
				{
					case "1": CreaPaziente(); break;
					case "2": VisualizzaPazienti(); break;
					case "3": VisualizzaDettagliPaziente(); break;
					case "4": AggiornaPaziente(); break;
					case "5": EliminaPaziente(); break;
					case "6": VisualizzaAnalisiMensilePazienti(); break;
					case "7": CreaPersonale(); break;
					case "8": VisualizzaPersonale(); break;
					case "9": VisualizzaDettagliPersonale(); break;
					case "10": AggiornaPersonale(); break;
					case "11": EliminaPersonale(); break;
					case "12": VisualizzaReportSettimanaleAppuntamenti(); break;
					case "13": CreaCartellaClinica(); break;
					case "14": VisualizzaCartelleCliniche(); break;
					case "15": VisualizzaDettagliCartellaClinica(); break;
					case "16": AggiornaCartellaClinica(); break;
					case "17": EliminaCartellaClinica(); break;
					case "18": VisualizzaRicoveriSettimanali(); break;
					case "19": CreaAppuntamento(); break;
					case "20": VisualizzaAppuntamenti(); break;
					case "21": VisualizzaDettagliAppuntamento(); break;
					case "22": AggiornaAppuntamento(); break;
					case "23": AnnullaAppuntamento(); break;
					case "24": CompletaAppuntamento(); break;
					case "25": VisualizzaStoricoAppuntamenti(); break;
					case "26": CreaPrescrizione(); break;
					case "27": VisualizzaPrescrizioni(); break;
					case "28": VisualizzaDettagliPrescrizione(); break;
					case "29": AggiornaPrescrizione(); break;
					case "30": EliminaPrescrizione(); break;
					case "31": VisualizzaLogEventi(); break;
					case "32": VisualizzaLogModifiche(); break;
					case "33": VisualizzaReportSettimanaleAppuntamenti(); break;
					case "34": VisualizzaAnalisiMensilePazienti(); break;
					case "35": back = true; break;
					default: Console.WriteLine("Opzione non valida. Premere un tasto per continuare..."); Console.ReadKey(); break;
				}
			}
		}

		static void MenuMedico()
		{
			bool back = false;
			while (!back)
			{
				Console.Clear();
				Console.WriteLine("Menu Medico:");
				Console.WriteLine("Pazienti:");
				Console.WriteLine("  1. Crea Paziente");
				Console.WriteLine("  2. Visualizza Pazienti");
				Console.WriteLine("  3. Visualizza Dettagli Paziente");
				Console.WriteLine("  4. Aggiorna Paziente");
				Console.WriteLine("  5. Visualizza Analisi Mensile Pazienti");

				Console.WriteLine("\nPersonale:");
				Console.WriteLine("  6. Visualizza Personale");
				Console.WriteLine("  7. Visualizza Report Settimanale Appuntamenti");

				Console.WriteLine("\nCartelle Cliniche:");
				Console.WriteLine("  8. Crea Cartella Clinica");
				Console.WriteLine("  9. Visualizza Cartelle Cliniche");
				Console.WriteLine("  10. Visualizza Dettagli Cartella Clinica");
				Console.WriteLine("  11. Aggiorna Cartella Clinica");
				Console.WriteLine("  12. Visualizza Ricoveri Settimanali");

				Console.WriteLine("\nAppuntamenti:");
				Console.WriteLine("  13. Crea Appuntamento");
				Console.WriteLine("  14. Visualizza Appuntamenti");
				Console.WriteLine("  15. Visualizza Dettagli Appuntamento");
				Console.WriteLine("  16. Aggiorna Appuntamento");
				Console.WriteLine("  17. Completa Appuntamento");

				Console.WriteLine("\nPrescrizioni:");
				Console.WriteLine("  18. Crea Prescrizione");
				Console.WriteLine("  19. Visualizza Prescrizioni");
				Console.WriteLine("  20. Visualizza Dettagli Prescrizione");
				Console.WriteLine("  21. Aggiorna Prescrizione");

				Console.WriteLine("\nAnalisi e Report:");
				Console.WriteLine("  22. Visualizza Report Settimanale Appuntamenti");
				Console.WriteLine("  23. Visualizza Analisi Mensile Pazienti");

				Console.WriteLine("\n24. Torna indietro");
				Console.Write("Seleziona un'opzione: ");
				var choice = Console.ReadLine();

				switch (choice)
				{
					case "1": CreaPaziente(); break;
					case "2": VisualizzaPazienti(); break;
					case "3": VisualizzaDettagliPaziente(); break;
					case "4": AggiornaPaziente(); break;
					case "5": VisualizzaAnalisiMensilePazienti(); break;
					case "6": VisualizzaPersonale(); break;
					case "7": VisualizzaReportSettimanaleAppuntamenti(); break;
					case "8": CreaCartellaClinica(); break;
					case "9": VisualizzaCartelleCliniche(); break;
					case "10": VisualizzaDettagliCartellaClinica(); break;
					case "11": AggiornaCartellaClinica(); break;
					case "12": VisualizzaRicoveriSettimanali(); break;
					case "13": CreaAppuntamento(); break;
					case "14": VisualizzaAppuntamenti(); break;
					case "15": VisualizzaDettagliAppuntamento(); break;
					case "16": AggiornaAppuntamento(); break;
					case "17": CompletaAppuntamento(); break;
					case "18": CreaPrescrizione(); break;
					case "19": VisualizzaPrescrizioni(); break;
					case "20": VisualizzaDettagliPrescrizione(); break;
					case "21": AggiornaPrescrizione(); break;
					case "22": VisualizzaReportSettimanaleAppuntamenti(); break;
					case "23": VisualizzaAnalisiMensilePazienti(); break;
					case "24": back = true; break;
					default: Console.WriteLine("Opzione non valida. Premere un tasto per continuare..."); Console.ReadKey(); break;
				}
			}
		}
		static void MenuInfermiere()
		{
			bool back = false;
			while (!back)
			{
				Console.Clear();
				Console.WriteLine("Menu Infermiere:");
				Console.WriteLine("Pazienti:");
				Console.WriteLine("  1. Visualizza Pazienti");
				Console.WriteLine("  2. Visualizza Dettagli Paziente");
				Console.WriteLine("  3. Aggiorna Paziente");

				Console.WriteLine("\nAppuntamenti:");
				Console.WriteLine("  4. Crea Appuntamento");
				Console.WriteLine("  5. Visualizza Appuntamenti");
				Console.WriteLine("  6. Visualizza Dettagli Appuntamento");
				Console.WriteLine("  7. Aggiorna Appuntamento");

				Console.WriteLine("\n8. Torna indietro");
				Console.Write("Seleziona un'opzione: ");
				var choice = Console.ReadLine();

				switch (choice)
				{
					case "1": VisualizzaPazienti(); break;
					case "2": VisualizzaDettagliPaziente(); break;
					case "3": AggiornaPaziente(); break;
					case "4": CreaAppuntamento(); break;
					case "5": VisualizzaAppuntamenti(); break;
					case "6": VisualizzaDettagliAppuntamento(); break;
					case "7": AggiornaAppuntamento(); break;
					case "8": back = true; break;
					default: Console.WriteLine("Opzione non valida. Premere un tasto per continuare..."); Console.ReadKey(); break;
				}
			}
		}

		static void MenuReceptionist()
		{
			bool back = false;
			while (!back)
			{
				Console.Clear();
				Console.WriteLine("Menu Receptionist:");
				Console.WriteLine("Pazienti:");
				Console.WriteLine("  1. Crea Paziente");
				Console.WriteLine("  2. Visualizza Pazienti");
				Console.WriteLine("  3. Visualizza Dettagli Paziente");

				Console.WriteLine("\nAppuntamenti:");
				Console.WriteLine("  4. Crea Appuntamento");
				Console.WriteLine("  5. Visualizza Appuntamenti");
				Console.WriteLine("  6. Visualizza Dettagli Appuntamento");

				Console.WriteLine("\n7. Torna indietro");
				Console.Write("Seleziona un'opzione: ");
				var choice = Console.ReadLine();

				switch (choice)
				{
					case "1": CreaPaziente(); break;
					case "2": VisualizzaPazienti(); break;
					case "3": VisualizzaDettagliPaziente(); break;
					case "4": CreaAppuntamento(); break;
					case "5": VisualizzaAppuntamenti(); break;
					case "6": VisualizzaDettagliAppuntamento(); break;
					case "7": back = true; break;
					default: Console.WriteLine("Opzione non valida. Premere un tasto per continuare..."); Console.ReadKey(); break;
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

		static string FetchInput(string prompt)
		{
			string? input = string.Empty;
			Console.Write(prompt);
			input = Console.ReadLine();
			while (string.IsNullOrEmpty(input))
			{
				Console.Write("Dato mancante. Riprova:");
				input = Console.ReadLine();
			}
			return input;
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
			while (!IsValidSesso(sesso = Console.ReadLine()?.ToUpper()))
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

		static void PromptAddPersonale()
		{
			throw new NotImplementedException();
		}

		static void PromptAddCartellaClinica()
		{
			throw new NotImplementedException();
		}
		static void AddPersonale()
		{
			throw new NotImplementedException();
		}

		static void AddCartellaClinica()
		{
			throw new NotImplementedException();
		}
		static bool IsValidSesso(string? sesso) => sesso == "M" || sesso == "F" || sesso == "O";
		static bool IsValidStatoAppuntamento(string? stato) => Array.Exists(new[] { "PROGRAMMATO", "COMPLETATO", "ANNULLATO" }, s => s.Equals(stato, StringComparison.OrdinalIgnoreCase));
		static bool IsValidEmail(string? email) => Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
		static bool IsValidPhone(string? phone) => Regex.IsMatch(phone, @"^[0-9]{10,15}$");
		static bool IsValidDate(string? dateStr, out DateTime date) => DateTime.TryParse(dateStr, out date) && date <= DateTime.Today;
	}
}