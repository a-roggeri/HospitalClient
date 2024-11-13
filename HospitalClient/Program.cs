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
				Console.Clear();
				Console.Write("Inserisci il tuo nome e cognome (inserire 0 per uscire): ");
				var input = Console.ReadLine();
				if (input == "0") return null;
				if (string.IsNullOrEmpty(input) || !input.Contains(" ")) continue;
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

		static void CreaPaziente()
		{
			Console.Write("Nome: ");
			string nome = Console.ReadLine();
			Console.Write("Cognome: ");
			string cognome = Console.ReadLine();
			Console.Write("Data di nascita (YYYY-MM-DD): ");
			string dataDiNascita = Console.ReadLine();
			Console.Write("Sesso (M/F/O): ");
			string sesso = Console.ReadLine();
			Console.Write("Indirizzo: ");
			string indirizzo = Console.ReadLine();
			Console.Write("Telefono: ");
			string telefono = Console.ReadLine();
			Console.Write("Email: ");
			string email = Console.ReadLine();

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "INSERT INTO Pazienti (nome, cognome, data_di_nascita, sesso, indirizzo, telefono, email) VALUES (@nome, @cognome, @dataDiNascita, @sesso, @indirizzo, @telefono, @email)";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("nome", nome);
			cmd.Parameters.AddWithValue("cognome", cognome);
			cmd.Parameters.AddWithValue("dataDiNascita", DateTime.Parse(dataDiNascita));
			cmd.Parameters.AddWithValue("sesso", sesso);
			cmd.Parameters.AddWithValue("indirizzo", indirizzo);
			cmd.Parameters.AddWithValue("telefono", telefono);
			cmd.Parameters.AddWithValue("email", email);

			try
			{
				cmd.ExecuteNonQuery();
				Console.WriteLine("Paziente creato con successo.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore: {ex.Message}");
			}
		}

		static void VisualizzaPazienti()
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "SELECT * FROM Pazienti";
			using var cmd = new NpgsqlCommand(query, connection);
			using var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Console.WriteLine($"ID: {reader["ID"]}, Nome: {reader["nome"]}, Cognome: {reader["cognome"]}, Data di nascita: {reader["data_di_nascita"]}, Sesso: {reader["sesso"]}, Indirizzo: {reader["indirizzo"]}, Telefono: {reader["telefono"]}, Email: {reader["email"]}, Data registrazione: {reader["data_registrazione"]}");
			}
			Console.WriteLine("Premere un tasto per continuare..."); 
			Console.ReadKey();
		}

		static void VisualizzaDettagliPaziente()
		{
			Console.Write("Inserisci l'ID del paziente: ");
			int id = int.Parse(Console.ReadLine());

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "SELECT * FROM Pazienti WHERE ID = @id";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("id", id);
			using var reader = cmd.ExecuteReader();
			if (reader.Read())
			{
				Console.WriteLine($"ID: {reader["ID"]}, Nome: {reader["nome"]}, Cognome: {reader["cognome"]}, Data di nascita: {reader["data_di_nascita"]}, Sesso: {reader["sesso"]}, Indirizzo: {reader["indirizzo"]}, Telefono: {reader["telefono"]}, Email: {reader["email"]}, Data registrazione: {reader["data_registrazione"]}");
			}
			else
			{
				Console.WriteLine("Paziente non trovato.");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void AggiornaPaziente()
		{
			Console.Write("Inserisci l'ID del paziente da aggiornare: ");
			int id = int.Parse(Console.ReadLine());

			Console.Write("Nome: ");
			string nome = Console.ReadLine();
			Console.Write("Cognome: ");
			string cognome = Console.ReadLine();
			Console.Write("Data di nascita (YYYY-MM-DD): ");
			string dataDiNascita = Console.ReadLine();
			Console.Write("Sesso (M/F/O): ");
			string sesso = Console.ReadLine();
			Console.Write("Indirizzo: ");
			string indirizzo = Console.ReadLine();
			Console.Write("Telefono: ");
			string telefono = Console.ReadLine();
			Console.Write("Email: ");
			string email = Console.ReadLine();

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "UPDATE Pazienti SET nome = @nome, cognome = @cognome, data_di_nascita = @dataDiNascita, sesso = @sesso, indirizzo = @indirizzo, telefono = @telefono, email = @email WHERE ID = @id";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("nome", nome);
			cmd.Parameters.AddWithValue("cognome", cognome);
			cmd.Parameters.AddWithValue("dataDiNascita", DateTime.Parse(dataDiNascita));
			cmd.Parameters.AddWithValue("sesso", sesso);
			cmd.Parameters.AddWithValue("indirizzo", indirizzo);
			cmd.Parameters.AddWithValue("telefono", telefono);
			cmd.Parameters.AddWithValue("email", email);
			cmd.Parameters.AddWithValue("id", id);

			try
			{
				cmd.ExecuteNonQuery();
				Console.WriteLine("Paziente aggiornato con successo.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore: {ex.Message}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void EliminaPaziente()
		{
			Console.Write("Inserisci l'ID del paziente da eliminare: ");
			int id = int.Parse(Console.ReadLine());

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "DELETE FROM Pazienti WHERE ID = @id";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("id", id);

			try
			{
				cmd.ExecuteNonQuery();
				Console.WriteLine("Paziente eliminato con successo.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore: {ex.Message}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void VisualizzaAnalisiMensilePazienti()
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "SELECT * FROM analisi_mensile_pazienti";
			using var cmd = new NpgsqlCommand(query, connection);
			using var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Console.WriteLine($"Mese: {reader["mese"]}, Numero pazienti: {reader["numero_pazienti"]}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void CreaPersonale()
		{
			Console.Write("Nome: ");
			string nome = Console.ReadLine();
			Console.Write("Cognome: ");
			string cognome = Console.ReadLine();
			Console.Write("Ruolo (ADMIN/MEDICO/INFERMIERE/RECEPTIONIST): ");
			string ruolo = Console.ReadLine();
			Console.Write("Reparto: ");
			string reparto = Console.ReadLine();
			Console.Write("Telefono: ");
			string telefono = Console.ReadLine();
			Console.Write("Email: ");
			string email = Console.ReadLine();
			Console.Write("Data di assunzione (YYYY-MM-DD): ");
			string dataAssunzione = Console.ReadLine();

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "INSERT INTO Personale (nome, cognome, ruolo, reparto, telefono, email, data_assunzione) VALUES (@nome, @cognome, @ruolo, @reparto, @telefono, @email, @dataAssunzione)";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("nome", nome);
			cmd.Parameters.AddWithValue("cognome", cognome);
			cmd.Parameters.AddWithValue("ruolo", ruolo);
			cmd.Parameters.AddWithValue("reparto", reparto);
			cmd.Parameters.AddWithValue("telefono", telefono);
			cmd.Parameters.AddWithValue("email", email);
			cmd.Parameters.AddWithValue("dataAssunzione", DateTime.Parse(dataAssunzione));

			try
			{
				cmd.ExecuteNonQuery();
				Console.WriteLine("Personale creato con successo.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore: {ex.Message}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void VisualizzaPersonale()
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "SELECT * FROM Personale";
			using var cmd = new NpgsqlCommand(query, connection);
			using var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Console.WriteLine($"ID: {reader["ID"]}, Nome: {reader["nome"]}, Cognome: {reader["cognome"]}, Ruolo: {reader["ruolo"]}, Reparto: {reader["reparto"]}, Telefono: {reader["telefono"]}, Email: {reader["email"]}, Data assunzione: {reader["data_assunzione"]}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void VisualizzaDettagliPersonale()
		{
			Console.Write("Inserisci l'ID del personale: ");
			int id = int.Parse(Console.ReadLine());

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "SELECT * FROM Personale WHERE ID = @id";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("id", id);
			using var reader = cmd.ExecuteReader();
			if (reader.Read())
			{
				Console.WriteLine($"ID: {reader["ID"]}, Nome: {reader["nome"]}, Cognome: {reader["cognome"]}, Ruolo: {reader["ruolo"]}, Reparto: {reader["reparto"]}, Telefono: {reader["telefono"]}, Email: {reader["email"]}, Data assunzione: {reader["data_assunzione"]}");
			}
			else
			{
				Console.WriteLine("Personale non trovato.");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}
		static void AggiornaPersonale()
		{
			Console.Write("Inserisci l'ID del personale da aggiornare: ");
			int id = int.Parse(Console.ReadLine());

			Console.Write("Nome: ");
			string nome = Console.ReadLine();
			Console.Write("Cognome: ");
			string cognome = Console.ReadLine();
			Console.Write("Ruolo (ADMIN/MEDICO/INFERMIERE/RECEPTIONIST): ");
			string ruolo = Console.ReadLine();
			Console.Write("Reparto: ");
			string reparto = Console.ReadLine();
			Console.Write("Telefono: ");
			string telefono = Console.ReadLine();
			Console.Write("Email: ");
			string email = Console.ReadLine();
			Console.Write("Data di assunzione (YYYY-MM-DD): ");
			string dataAssunzione = Console.ReadLine();

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "UPDATE Personale SET nome = @nome, cognome = @cognome, ruolo = @ruolo, reparto = @reparto, telefono = @telefono, email = @email, data_assunzione = @dataAssunzione WHERE ID = @id";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("nome", nome);
			cmd.Parameters.AddWithValue("cognome", cognome);
			cmd.Parameters.AddWithValue("ruolo", ruolo);
			cmd.Parameters.AddWithValue("reparto", reparto);
			cmd.Parameters.AddWithValue("telefono", telefono);
			cmd.Parameters.AddWithValue("email", email);
			cmd.Parameters.AddWithValue("dataAssunzione", DateTime.Parse(dataAssunzione));
			cmd.Parameters.AddWithValue("id", id);

			try
			{
				cmd.ExecuteNonQuery();
				Console.WriteLine("Personale aggiornato con successo.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore: {ex.Message}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void EliminaPersonale()
		{
			Console.Write("Inserisci l'ID del personale da eliminare: ");
			int id = int.Parse(Console.ReadLine());

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "DELETE FROM Personale WHERE ID = @id";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("id", id);

			try
			{
				cmd.ExecuteNonQuery();
				Console.WriteLine("Personale eliminato con successo.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore: {ex.Message}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void CreaAppuntamento()
		{
			Console.Write("ID Paziente: ");
			int pazienteId = int.Parse(Console.ReadLine());
			Console.Write("ID Personale: ");
			int personaleId = int.Parse(Console.ReadLine());
			Console.Write("Data (YYYY-MM-DD): ");
			string data = Console.ReadLine();
			Console.Write("Ora (HH:MM:SS): ");
			string ora = Console.ReadLine();
			Console.Write("Motivo: ");
			string motivo = Console.ReadLine();

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "INSERT INTO Appuntamenti (paziente_id, personale_id, data, ora, motivo) VALUES (@pazienteId, @personaleId, @data, @ora, @motivo)";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("pazienteId", pazienteId);
			cmd.Parameters.AddWithValue("personaleId", personaleId);
			cmd.Parameters.AddWithValue("data", DateTime.Parse(data));
			cmd.Parameters.AddWithValue("ora", TimeSpan.Parse(ora));
			cmd.Parameters.AddWithValue("motivo", motivo);

			try
			{
				cmd.ExecuteNonQuery();
				Console.WriteLine("Appuntamento creato con successo.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore: {ex.Message}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void VisualizzaAppuntamenti()
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "SELECT * FROM Appuntamenti";
			using var cmd = new NpgsqlCommand(query, connection);
			using var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Console.WriteLine($"ID: {reader["ID"]}, Paziente ID: {reader["paziente_id"]}, Personale ID: {reader["personale_id"]}, Data: {reader["data"]}, Ora: {reader["ora"]}, Motivo: {reader["motivo"]}, Stato: {reader["stato"]}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void AggiornaAppuntamento()
		{
			Console.Write("Inserisci l'ID dell'appuntamento da aggiornare: ");
			int id = int.Parse(Console.ReadLine());

			Console.Write("ID Paziente: ");
			int pazienteId = int.Parse(Console.ReadLine());
			Console.Write("ID Personale: ");
			int personaleId = int.Parse(Console.ReadLine());
			Console.Write("Data (YYYY-MM-DD): ");
			string data = Console.ReadLine();
			Console.Write("Ora (HH:MM:SS): ");
			string ora = Console.ReadLine();
			Console.Write("Motivo: ");
			string motivo = Console.ReadLine();
			Console.Write("Stato (PROGRAMMATO/COMPLETATO/ANNULLATO): ");
			string stato = Console.ReadLine();

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "UPDATE Appuntamenti SET paziente_id = @pazienteId, personale_id = @personaleId, data = @data, ora = @ora, motivo = @motivo, stato = @stato WHERE ID = @id";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("pazienteId", pazienteId);
			cmd.Parameters.AddWithValue("personaleId", personaleId);
			cmd.Parameters.AddWithValue("data", DateTime.Parse(data));
			cmd.Parameters.AddWithValue("ora", TimeSpan.Parse(ora));
			cmd.Parameters.AddWithValue("motivo", motivo);
			cmd.Parameters.AddWithValue("stato", stato);
			cmd.Parameters.AddWithValue("id", id);

			try
			{
				cmd.ExecuteNonQuery();
				Console.WriteLine("Appuntamento aggiornato con successo.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore: {ex.Message}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void EliminaAppuntamento()
		{
			Console.Write("Inserisci l'ID dell'appuntamento da eliminare: ");
			int id = int.Parse(Console.ReadLine());

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "DELETE FROM Appuntamenti WHERE ID = @id";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("id", id);

			try
			{
				cmd.ExecuteNonQuery();
				Console.WriteLine("Appuntamento eliminato con successo.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore: {ex.Message}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}
		static void VisualizzaReportSettimanaleAppuntamenti()
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "SELECT * FROM Appuntamenti WHERE data >= current_date - interval '7 days'";
			using var cmd = new NpgsqlCommand(query, connection);
			using var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Console.WriteLine($"ID: {reader["ID"]}, Paziente ID: {reader["paziente_id"]}, Personale ID: {reader["personale_id"]}, Data: {reader["data"]}, Ora: {reader["ora"]}, Motivo: {reader["motivo"]}, Stato: {reader["stato"]}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void CreaCartellaClinica()
		{
			Console.Write("ID Paziente: ");
			int pazienteId = int.Parse(Console.ReadLine());
			Console.Write("Diagnosi: ");
			string diagnosi = Console.ReadLine();
			Console.Write("Trattamento: ");
			string trattamento = Console.ReadLine();
			Console.Write("Data di apertura (YYYY-MM-DD): ");
			string dataApertura = Console.ReadLine();

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "INSERT INTO CartelleCliniche (paziente_id, diagnosi, trattamento, data_apertura) VALUES (@pazienteId, @diagnosi, @trattamento, @dataApertura)";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("pazienteId", pazienteId);
			cmd.Parameters.AddWithValue("diagnosi", diagnosi);
			cmd.Parameters.AddWithValue("trattamento", trattamento);
			cmd.Parameters.AddWithValue("dataApertura", DateTime.Parse(dataApertura));

			try
			{
				cmd.ExecuteNonQuery();
				Console.WriteLine("Cartella clinica creata con successo.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore: {ex.Message}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void VisualizzaCartelleCliniche()
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "SELECT * FROM Cartelle_Cliniche";
			using var cmd = new NpgsqlCommand(query, connection);
			using var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Console.WriteLine($"ID: {reader["ID"]}, Paziente ID: {reader["paziente_id"]}, Data Creazione: {reader["data_creazione"]}, Note Mediche: {reader["note_mediche"]}, Stato Attuale: {reader["stato_attuale"]}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void VisualizzaDettagliCartellaClinica()
		{
			Console.Write("Inserisci l'ID della cartella clinica: ");
			int id = int.Parse(Console.ReadLine());

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "SELECT * FROM Cartelle_Cliniche WHERE ID = @id";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("id", id);
			using var reader = cmd.ExecuteReader();
			if (reader.Read())
			{
				Console.WriteLine($"ID: {reader["ID"]}, Paziente ID: {reader["paziente_id"]}, Data Creazione: {reader["data_creazione"]}, Note Mediche: {reader["note_mediche"]}, Stato Attuale: {reader["stato_attuale"]}");
			}
			else
			{
				Console.WriteLine("Cartella clinica non trovata.");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void AggiornaCartellaClinica()
		{
			Console.Write("Inserisci l'ID della cartella clinica da aggiornare: ");
			int id = int.Parse(Console.ReadLine());

			Console.Write("Diagnosi: ");
			string diagnosi = Console.ReadLine();
			Console.Write("Trattamento: ");
			string trattamento = Console.ReadLine();
			Console.Write("Data di apertura (YYYY-MM-DD): ");
			string dataApertura = Console.ReadLine();

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "UPDATE CartelleCliniche SET diagnosi = @diagnosi, trattamento = @trattamento, data_apertura = @dataApertura WHERE ID = @id";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("diagnosi", diagnosi);
			cmd.Parameters.AddWithValue("trattamento", trattamento);
			cmd.Parameters.AddWithValue("dataApertura", DateTime.Parse(dataApertura));
			cmd.Parameters.AddWithValue("id", id);

			try
			{
				cmd.ExecuteNonQuery();
				Console.WriteLine("Cartella clinica aggiornata con successo.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore: {ex.Message}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void EliminaCartellaClinica()
		{
			Console.Write("Inserisci l'ID della cartella clinica da eliminare: ");
			int id = int.Parse(Console.ReadLine());

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "DELETE FROM CartelleCliniche WHERE ID = @id";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("id", id);

			try
			{
				cmd.ExecuteNonQuery();
				Console.WriteLine("Cartella clinica eliminata con successo.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore: {ex.Message}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void VisualizzaRicoveriSettimanali()
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "SELECT * FROM vista_ricoveri_settimanali";
			using var cmd = new NpgsqlCommand(query, connection);
			using var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Console.WriteLine($"Paziente ID: {reader["paziente_id"]}, Numero Ricoveri: {reader["numero_ricoveri"]}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}
		static void VisualizzaDettagliAppuntamento()
		{
			Console.Write("Inserisci l'ID dell'appuntamento: ");
			int id = int.Parse(Console.ReadLine());

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "SELECT * FROM Appuntamenti WHERE ID = @id";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("id", id);
			using var reader = cmd.ExecuteReader();
			if (reader.Read())
			{
				Console.WriteLine($"ID: {reader["ID"]}, Paziente ID: {reader["paziente_id"]}, Personale ID: {reader["personale_id"]}, Data: {reader["data"]}, Ora: {reader["ora"]}, Motivo: {reader["motivo"]}, Stato: {reader["stato"]}");
			}
			else
			{
				Console.WriteLine("Appuntamento non trovato.");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void AnnullaAppuntamento()
		{
			Console.Write("Inserisci l'ID dell'appuntamento da annullare: ");
			int id = int.Parse(Console.ReadLine());

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "UPDATE Appuntamenti SET stato = 'ANNULLATO' WHERE ID = @id";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("id", id);

			try
			{
				cmd.ExecuteNonQuery();
				Console.WriteLine("Appuntamento annullato con successo.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore: {ex.Message}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void VisualizzaLogModifiche()
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "SELECT * FROM LogModifiche";
			using var cmd = new NpgsqlCommand(query, connection);
			using var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Console.WriteLine($"ID: {reader["ID"]}, Tabella: {reader["tabella"]}, Azione: {reader["azione"]}, Data: {reader["data"]}, Utente: {reader["utente"]}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}
		static void CompletaAppuntamento()
		{
			Console.Write("Inserisci l'ID dell'appuntamento da completare: ");
			int id = int.Parse(Console.ReadLine());

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "UPDATE Appuntamenti SET stato = 'COMPLETATO' WHERE ID = @id";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("id", id);

			try
			{
				cmd.ExecuteNonQuery();
				Console.WriteLine("Appuntamento completato con successo.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore: {ex.Message}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void VisualizzaStoricoAppuntamenti()
		{
			Console.Write("Inserisci l'ID del paziente: ");
			int pazienteId = int.Parse(Console.ReadLine());

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "SELECT * FROM Appuntamenti WHERE paziente_id = @pazienteId";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("pazienteId", pazienteId);
			using var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Console.WriteLine($"ID: {reader["ID"]}, Paziente ID: {reader["paziente_id"]}, Personale ID: {reader["personale_id"]}, Data: {reader["data"]}, Ora: {reader["ora"]}, Motivo: {reader["motivo"]}, Stato: {reader["stato"]}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void CreaPrescrizione()
		{
			Console.Write("ID Paziente: ");
			int pazienteId = int.Parse(Console.ReadLine());
			Console.Write("ID Medico: ");
			int medicoId = int.Parse(Console.ReadLine());
			Console.Write("Descrizione: ");
			string descrizione = Console.ReadLine();
			Console.Write("Data (YYYY-MM-DD): ");
			string data = Console.ReadLine();

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "INSERT INTO Prescrizioni (paziente_id, medico_id, descrizione, data) VALUES (@pazienteId, @medicoId, @descrizione, @data)";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("pazienteId", pazienteId);
			cmd.Parameters.AddWithValue("medicoId", medicoId);
			cmd.Parameters.AddWithValue("descrizione", descrizione);
			cmd.Parameters.AddWithValue("data", DateTime.Parse(data));

			try
			{
				cmd.ExecuteNonQuery();
				Console.WriteLine("Prescrizione creata con successo.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore: {ex.Message}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void VisualizzaPrescrizioni()
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "SELECT * FROM Prescrizioni";
			using var cmd = new NpgsqlCommand(query, connection);
			using var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Console.WriteLine($"ID: {reader["ID"]}, Cartella Clinica ID: {reader["cartella_clinica_id"]}, Farmaco: {reader["farmaco"]}, Dosaggio: {reader["dosaggio"]}, Frequenza: {reader["frequenza"]}, Durata: {reader["durata"]}, Note: {reader["note"]}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void VisualizzaDettagliPrescrizione()
		{
			Console.Write("Inserisci l'ID della prescrizione: ");
			int id = int.Parse(Console.ReadLine());

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "SELECT * FROM Prescrizioni WHERE ID = @id";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("id", id);
			using var reader = cmd.ExecuteReader();
			if (reader.Read())
			{
				Console.WriteLine($"ID: {reader["ID"]}, Cartella Clinica ID: {reader["cartella_clinica_id"]}, Farmaco: {reader["farmaco"]}, Dosaggio: {reader["dosaggio"]}, Frequenza: {reader["frequenza"]}, Durata: {reader["durata"]}, Note: {reader["note"]}");
			}
			else
			{
				Console.WriteLine("Prescrizione non trovata.");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void AggiornaPrescrizione()
		{
			Console.Write("Inserisci l'ID della prescrizione da aggiornare: ");
			int id = int.Parse(Console.ReadLine());

			Console.Write("Descrizione: ");
			string descrizione = Console.ReadLine();
			Console.Write("Data (YYYY-MM-DD): ");
			string data = Console.ReadLine();

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "UPDATE Prescrizioni SET descrizione = @descrizione, data = @data WHERE ID = @id";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("descrizione", descrizione);
			cmd.Parameters.AddWithValue("data", DateTime.Parse(data));
			cmd.Parameters.AddWithValue("id", id);

			try
			{
				cmd.ExecuteNonQuery();
				Console.WriteLine("Prescrizione aggiornata con successo.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore: {ex.Message}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void EliminaPrescrizione()
		{
			Console.Write("Inserisci l'ID della prescrizione da eliminare: ");
			int id = int.Parse(Console.ReadLine());

			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "DELETE FROM Prescrizioni WHERE ID = @id";
			using var cmd = new NpgsqlCommand(query, connection);
			cmd.Parameters.AddWithValue("id", id);

			try
			{
				cmd.ExecuteNonQuery();
				Console.WriteLine("Prescrizione eliminata con successo.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore: {ex.Message}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}

		static void VisualizzaLogEventi()
		{
			using var connection = new NpgsqlConnection(connectionString);
			connection.Open();
			string query = "SELECT * FROM LogEventi";
			using var cmd = new NpgsqlCommand(query, connection);
			using var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				Console.WriteLine($"ID: {reader["ID"]}, Evento: {reader["evento"]}, Data: {reader["data"]}, Utente: {reader["utente"]}");
			}
			Console.WriteLine("Premere un tasto per continuare...");
			Console.ReadKey();
		}
	}
}