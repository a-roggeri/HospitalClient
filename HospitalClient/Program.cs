using Microsoft.Data.SqlClient;
using Npgsql;
using System.Text.RegularExpressions;

namespace HospitalClient
{
	class Program
	{
		public static readonly string CONNECTION_STRING = 
			"Server=localhost;" +
			"Database=hospital_management;" +
			"User Id=postgres;" +
			"Password=admin;";
		static void Main(string[] args)
		{
			bool exit = false;
			while (!exit)
			{
				string? ruolo = AutenticazioneUtente();
				RefreshAllMaterializedViews();
				Console.Write("\x1b[H\x1b[2J\x1b[3J");
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
			using var connection = new NpgsqlConnection(CONNECTION_STRING);
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
			Console.Write("\x1b[H\x1b[2J\x1b[3J");
			string? ruolo = null;
			while (string.IsNullOrEmpty(ruolo))
			{
				Console.Write("Inserisci il tuo nome e cognome (inserire 0 per uscire): ");
				var input = Console.ReadLine();
				if (input == "0") return null;
				if (string.IsNullOrEmpty(input) || !input.Contains(" ")) continue;
				string[] parts = input.Split(" ");
				string nome = parts[0];
				string cognome = parts[1];

				using var connection = new NpgsqlConnection(CONNECTION_STRING);
				connection.Open();
				string query = "SELECT ruolo FROM Personale WHERE UPPER(nome) = UPPER(@nome) AND UPPER(cognome) = UPPER(@cognome)";
				using var cmd = new NpgsqlCommand(query, connection);
				cmd.Parameters.AddWithValue("nome", nome);
				cmd.Parameters.AddWithValue("cognome", cognome);

				var result = cmd.ExecuteScalar();
				if (result != null)
				{
					Console.Write("\x1b[H\x1b[2J\x1b[3J");
					ruolo = result.ToString();
					Console.WriteLine($"Accesso effettuato come {ruolo}. Premere un tasto per continuare...");
					Console.ReadKey();
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
				Console.Write("\x1b[H\x1b[2J\x1b[3J");
				Console.WriteLine("Menu Admin:");

				Console.WriteLine("Pazienti:");
				Console.WriteLine("  1. Crea Paziente");
				Console.WriteLine("  2. Visualizza Pazienti");
				Console.WriteLine("  3. Visualizza Dettagli Paziente");
				Console.WriteLine("  4. Aggiorna Paziente");
				Console.WriteLine("  5. Elimina Paziente");
				Console.WriteLine("  6. Visualizza Analisi Mensile Pazienti");
				Console.WriteLine("  7. Cerca Pazienti per Nome o Cognome");
				Console.WriteLine("  8. Conta Pazienti per Sesso");

				Console.WriteLine("\nPersonale:");
				Console.WriteLine("  9. Crea Personale");
				Console.WriteLine("  10. Visualizza Personale");
				Console.WriteLine("  11. Visualizza Dettagli Personale");
				Console.WriteLine("  12. Aggiorna Personale");
				Console.WriteLine("  13. Elimina Personale");
				Console.WriteLine("  14. Visualizza Report Settimanale Appuntamenti");
				Console.WriteLine("  15. Conta Personale per Ruolo");

				Console.WriteLine("\nCartelle Cliniche:");
				Console.WriteLine("  16. Crea Cartella Clinica");
				Console.WriteLine("  17. Visualizza Cartelle Cliniche");
				Console.WriteLine("  18. Visualizza Dettagli Cartella Clinica");
				Console.WriteLine("  19. Aggiorna Cartella Clinica");
				Console.WriteLine("  20. Elimina Cartella Clinica");
				Console.WriteLine("  21. Visualizza Ricoveri Settimanali");
				Console.WriteLine("  22. Ricerca Full-Text nelle Note Mediche");

				Console.WriteLine("\nAppuntamenti:");
				Console.WriteLine("  23. Crea Appuntamento");
				Console.WriteLine("  24. Visualizza Appuntamenti");
				Console.WriteLine("  25. Visualizza Dettagli Appuntamento");
				Console.WriteLine("  26. Aggiorna Appuntamento");
				Console.WriteLine("  27. Annulla Appuntamento");
				Console.WriteLine("  28. Completa Appuntamento");
				Console.WriteLine("  29. Visualizza Storico Appuntamenti");
				Console.WriteLine("  30. Ricerca Full-Text nei Motivi degli Appuntamenti");
				Console.WriteLine("  31. Conta Appuntamenti per Stato");

				Console.WriteLine("\nPrescrizioni:");
				Console.WriteLine("  32. Crea Prescrizione");
				Console.WriteLine("  33. Visualizza Prescrizioni");
				Console.WriteLine("  34. Visualizza Dettagli Prescrizione");
				Console.WriteLine("  35. Aggiorna Prescrizione");
				Console.WriteLine("  36. Elimina Prescrizione");
				Console.WriteLine("  37. Ricerca Prescrizioni per Farmaco");

				Console.WriteLine("\nLog:");
				Console.WriteLine("  38. Visualizza Log Eventi");
				Console.WriteLine("  39. Visualizza Log Modifiche");

				Console.WriteLine("\nAnalisi e Report:");
				Console.WriteLine("  40. Visualizza Report Settimanale Appuntamenti");
				Console.WriteLine("  41. Visualizza Analisi Mensile Pazienti");

				Console.WriteLine("\n42. Torna indietro");
				Console.Write("Seleziona un'opzione: ");
				var choice = Console.ReadLine();
				Console.WriteLine("\n");
				switch (choice)
				{
					case "1": CreaPaziente(); break;
					case "2": VisualizzaPazienti(); break;
					case "3": VisualizzaDettagliPaziente(); break;
					case "4": AggiornaPaziente(); break;
					case "5": EliminaPaziente(); break;
					case "6": VisualizzaAnalisiMensilePazienti(); break;
					case "7": CercaPazientiPerNomeOCognome(); break;
					case "8": ContaPazientiPerSesso(); break;
					case "9": CreaPersonale(); break;
					case "10": VisualizzaPersonale(); break;
					case "11": VisualizzaDettagliPersonale(); break;
					case "12": AggiornaPersonale(); break;
					case "13": EliminaPersonale(); break;
					case "14": VisualizzaReportSettimanaleAppuntamenti(); break;
					case "15": ContaPersonalePerRuolo(); break;
					case "16": CreaCartellaClinica(); break;
					case "17": VisualizzaCartelleCliniche(); break;
					case "18": VisualizzaDettagliCartellaClinica(); break;
					case "19": AggiornaCartellaClinica(); break;
					case "20": EliminaCartellaClinica(); break;
					case "21": VisualizzaRicoveriSettimanali(); break;
					case "22": RicercaFullTextNoteMediche(); break;
					case "23": CreaAppuntamento(); break;
					case "24": VisualizzaAppuntamenti(); break;
					case "25": VisualizzaDettagliAppuntamento(); break;
					case "26": AggiornaAppuntamento(); break;
					case "27": AnnullaAppuntamento(); break;
					case "28": CompletaAppuntamento(); break;
					case "29": VisualizzaStoricoAppuntamenti(); break;
					case "30": RicercaFullTextMotiviAppuntamenti(); break;
					case "31": ContaAppuntamentiPerStato(); break;
					case "32": CreaPrescrizione(); break;
					case "33": VisualizzaPrescrizioni(); break;
					case "34": VisualizzaDettagliPrescrizione(); break;
					case "35": AggiornaPrescrizione(); break;
					case "36": EliminaPrescrizione(); break;
					case "37": RicercaPrescrizioniPerFarmaco(); break;
					case "38": VisualizzaLogEventi(); break;
					case "39": VisualizzaLogModifiche(); break;
					case "40": VisualizzaReportSettimanaleAppuntamenti(); break;
					case "41": VisualizzaAnalisiMensilePazienti(); break;
					case "42": back = true; break;
					default: Console.WriteLine("Opzione non valida. Premere un tasto per continuare..."); Console.ReadKey(); break;
				}
			}
		}

		static void MenuMedico()
		{
			bool back = false;
			while (!back)
			{
				Console.Write("\x1b[H\x1b[2J\x1b[3J");
				Console.WriteLine("Menu Medico:");

				Console.WriteLine("Pazienti:");
				Console.WriteLine("  1. Visualizza Pazienti");
				Console.WriteLine("  2. Visualizza Dettagli Paziente");

				Console.WriteLine("\nCartelle Cliniche:");
				Console.WriteLine("  3. Crea Cartella Clinica");
				Console.WriteLine("  4. Visualizza Cartelle Cliniche");
				Console.WriteLine("  5. Visualizza Dettagli Cartella Clinica");
				Console.WriteLine("  6. Aggiorna Cartella Clinica");
				Console.WriteLine("  7. Ricerca Full-Text nelle Note Mediche");

				Console.WriteLine("\nAppuntamenti:");
				Console.WriteLine("  8. Visualizza Appuntamenti");
				Console.WriteLine("  9. Visualizza Dettagli Appuntamento");
				Console.WriteLine("  10. Completa Appuntamento");

				Console.WriteLine("\nPrescrizioni:");
				Console.WriteLine("  11. Crea Prescrizione");
				Console.WriteLine("  12. Visualizza Prescrizioni");
				Console.WriteLine("  13. Visualizza Dettagli Prescrizione");
				Console.WriteLine("  14. Aggiorna Prescrizione");
				Console.WriteLine("  15. Ricerca Prescrizioni per Farmaco");

				Console.WriteLine("\nLog:");
				Console.WriteLine("  16. Visualizza Log Modifiche");

				Console.WriteLine("\n17. Torna indietro");
				Console.Write("Seleziona un'opzione: ");
				var choice = Console.ReadLine();
				Console.WriteLine("\n");
				switch (choice)
				{
					case "1": VisualizzaPazienti(); break;
					case "2": VisualizzaDettagliPaziente(); break;
					case "3": CreaCartellaClinica(); break;
					case "4": VisualizzaCartelleCliniche(); break;
					case "5": VisualizzaDettagliCartellaClinica(); break;
					case "6": AggiornaCartellaClinica(); break;
					case "7": RicercaFullTextNoteMediche(); break;
					case "8": VisualizzaAppuntamenti(); break;
					case "9": VisualizzaDettagliAppuntamento(); break;
					case "10": CompletaAppuntamento(); break;
					case "11": CreaPrescrizione(); break;
					case "12": VisualizzaPrescrizioni(); break;
					case "13": VisualizzaDettagliPrescrizione(); break;
					case "14": AggiornaPrescrizione(); break;
					case "15": RicercaPrescrizioniPerFarmaco(); break;
					case "16": VisualizzaLogModifiche(); break;
					case "17": back = true; break;
					default: Console.WriteLine("Opzione non valida. Premere un tasto per continuare..."); Console.ReadKey(); break;
				}
			}
		}
		static void MenuInfermiere()
		{
			bool back = false;
			while (!back)
			{
				Console.Write("\x1b[H\x1b[2J\x1b[3J");
				Console.WriteLine("Menu Infermiere:");

				Console.WriteLine("Pazienti:");
				Console.WriteLine("  1. Visualizza Pazienti");
				Console.WriteLine("  2. Visualizza Dettagli Paziente");

				Console.WriteLine("\nCartelle Cliniche:");
				Console.WriteLine("  3. Visualizza Cartelle Cliniche");
				Console.WriteLine("  4. Visualizza Dettagli Cartella Clinica");
				Console.WriteLine("  5. Ricerca Full-Text nelle Note Mediche");

				Console.WriteLine("\nAppuntamenti:");
				Console.WriteLine("  6. Visualizza Appuntamenti");
				Console.WriteLine("  7. Visualizza Dettagli Appuntamento");

				Console.WriteLine("\nPrescrizioni:");
				Console.WriteLine("  8. Visualizza Prescrizioni");
				Console.WriteLine("  9. Visualizza Dettagli Prescrizione");
				Console.WriteLine("  10. Ricerca Prescrizioni per Farmaco");

				Console.WriteLine("\n11. Torna indietro");
				Console.Write("Seleziona un'opzione: ");
				var choice = Console.ReadLine();
				Console.WriteLine("\n");
				switch (choice)
				{
					case "1": VisualizzaPazienti(); break;
					case "2": VisualizzaDettagliPaziente(); break;
					case "3": VisualizzaCartelleCliniche(); break;
					case "4": VisualizzaDettagliCartellaClinica(); break;
					case "5": RicercaFullTextNoteMediche(); break;
					case "6": VisualizzaAppuntamenti(); break;
					case "7": VisualizzaDettagliAppuntamento(); break;
					case "8": VisualizzaPrescrizioni(); break;
					case "9": VisualizzaDettagliPrescrizione(); break;
					case "10": RicercaPrescrizioniPerFarmaco(); break;
					case "11": back = true; break;
					default: Console.WriteLine("Opzione non valida. Premere un tasto per continuare..."); Console.ReadKey(); break;
				}
			}
		}

		static void MenuReceptionist()
		{
			bool back = false;
			while (!back)
			{
				Console.Write("\x1b[H\x1b[2J\x1b[3J");
				Console.WriteLine("Menu Receptionist:");

				Console.WriteLine("Pazienti:");
				Console.WriteLine("  1. Crea Paziente");
				Console.WriteLine("  2. Visualizza Pazienti");
				Console.WriteLine("  3. Visualizza Dettagli Paziente");
				Console.WriteLine("  4. Aggiorna Paziente");
				Console.WriteLine("  5. Elimina Paziente");

				Console.WriteLine("\nAppuntamenti:");
				Console.WriteLine("  6. Crea Appuntamento");
				Console.WriteLine("  7. Visualizza Appuntamenti");
				Console.WriteLine("  8. Visualizza Dettagli Appuntamento");
				Console.WriteLine("  9. Aggiorna Appuntamento");
				Console.WriteLine("  10. Annulla Appuntamento");
				Console.WriteLine("  11. Completa Appuntamento");

				Console.WriteLine("\n12. Torna indietro");
				Console.Write("Seleziona un'opzione: ");
				var choice = Console.ReadLine();
				Console.WriteLine("\n");
				switch (choice)
				{
					case "1": CreaPaziente(); break;
					case "2": VisualizzaPazienti(); break;
					case "3": VisualizzaDettagliPaziente(); break;
					case "4": AggiornaPaziente(); break;
					case "5": EliminaPaziente(); break;
					case "6": CreaAppuntamento(); break;
					case "7": VisualizzaAppuntamenti(); break;
					case "8": VisualizzaDettagliAppuntamento(); break;
					case "9": AggiornaAppuntamento(); break;
					case "10": AnnullaAppuntamento(); break;
					case "11": CompletaAppuntamento(); break;
					case "12": back = true; break;
					default: Console.WriteLine("Opzione non valida. Premere un tasto per continuare..."); Console.ReadKey(); break;
				}
			}
		}
		
		public static void CreaPaziente()
		{
			string nome;
			do
			{
				Console.Write("Inserisci il nome del paziente: ");
				nome = Console.ReadLine().Trim();
				if (string.IsNullOrEmpty(nome))
				{
					Console.WriteLine("Il nome non può essere vuoto.");
				}
			} while (string.IsNullOrEmpty(nome));

			string cognome;
			do
			{
				Console.Write("Inserisci il cognome del paziente: ");
				cognome = Console.ReadLine().Trim();
				if (string.IsNullOrEmpty(cognome))
				{
					Console.WriteLine("Il cognome non può essere vuoto.");
				}
			} while (string.IsNullOrEmpty(cognome));

			DateTime dataDiNascita;
			bool dataValida = false;
			do
			{
				Console.Write("Inserisci la data di nascita (YYYY-MM-DD): ");
				string inputData = Console.ReadLine().Trim();
				if (DateTime.TryParse(inputData, out dataDiNascita))
				{
					dataValida = true;
				}
				else
				{
					Console.WriteLine("Formato data non valido. Inserisci la data nel formato YYYY-MM-DD.");
				}
			} while (!dataValida);

			string sesso;
			do
			{
				Console.Write("Inserisci il sesso (M/F/O): ");
				sesso = Console.ReadLine().Trim().ToUpper();
				if (sesso != "M" && sesso != "F" && sesso != "O")
				{
					Console.WriteLine("Sesso non valido. Inserisci M, F o O.");
				}
			} while (sesso != "M" && sesso != "F" && sesso != "O");

			string indirizzo;
			do
			{
				Console.Write("Inserisci l'indirizzo: ");
				indirizzo = Console.ReadLine().Trim();
				if (string.IsNullOrEmpty(indirizzo))
				{
					Console.WriteLine("L'indirizzo non può essere vuoto.");
				}
			} while (string.IsNullOrEmpty(indirizzo));

			string telefono;
			do
			{
				Console.Write("Inserisci il numero di telefono: ");
				telefono = Console.ReadLine().Trim();
				if (!Regex.IsMatch(telefono, @"^\+?\d{7,15}$"))
				{
					Console.WriteLine("Numero di telefono non valido. Inserisci un numero con 7-15 cifre, può includere il prefisso internazionale.");
					telefono = null;
				}
			} while (string.IsNullOrEmpty(telefono));

			string email;
			do
			{
				Console.Write("Inserisci l'email: ");
				email = Console.ReadLine().Trim();
				if (!Regex.IsMatch(email, @"^[\w\.-]+@[\w\.-]+\.\w{2,}$"))
				{
					Console.WriteLine("Email non valida. Inserisci un'email valida.");
					email = null;
				}
			} while (string.IsNullOrEmpty(email));


			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"
                    INSERT INTO Pazienti (nome, cognome, data_di_nascita, sesso, indirizzo, telefono, email)
                    VALUES (@Nome, @Cognome, @DataDiNascita, @Sesso, @Indirizzo, @Telefono, @Email)";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Nome", nome);
						cmd.Parameters.AddWithValue("@Cognome", cognome);
						cmd.Parameters.AddWithValue("@DataDiNascita", dataDiNascita);
						cmd.Parameters.AddWithValue("@Sesso", sesso);
						cmd.Parameters.AddWithValue("@Indirizzo", indirizzo);
						cmd.Parameters.AddWithValue("@Telefono", telefono);
						cmd.Parameters.AddWithValue("@Email", email);

						cmd.ExecuteNonQuery();
						Console.WriteLine("Paziente inserito correttamente con i dati forniti.");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante l'inserimento del paziente: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void VisualizzaPazienti()
		{
			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = "SELECT ID, nome, cognome, data_di_nascita, sesso, indirizzo, telefono, email, data_registrazione FROM Pazienti";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						Console.WriteLine("Lista dei pazienti:");
						Console.WriteLine("ID\tNome\tCognome\tData di Nascita\tSesso\tIndirizzo\tTelefono\tEmail\tData Registrazione");

						while (reader.Read())
						{
							Console.WriteLine($"{reader["ID"]}\t{reader["nome"]}\t{reader["cognome"]}\t" +
											  $"{reader["data_di_nascita"]:yyyy-MM-dd}\t{reader["sesso"]}\t" +
											  $"{reader["indirizzo"]}\t{reader["telefono"]}\t" +
											  $"{reader["email"]}\t{reader["data_registrazione"]}");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la visualizzazione dei pazienti: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void VisualizzaDettagliPaziente()
		{
			Console.Write("Inserisci l'ID del paziente di cui vuoi visualizzare i dettagli: ");
			string inputId = Console.ReadLine().Trim();
			int pazienteId;

			while (!int.TryParse(inputId, out pazienteId))
			{
				Console.WriteLine("ID non valido. Inserisci un numero intero.");
				inputId = Console.ReadLine().Trim();
			}

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = "SELECT * FROM Pazienti WHERE ID = @ID";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@ID", pazienteId);

						using (NpgsqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								Console.WriteLine("Dettagli del paziente:");
								Console.WriteLine($"ID: {reader["ID"]}");
								Console.WriteLine($"Nome: {reader["nome"]}");
								Console.WriteLine($"Cognome: {reader["cognome"]}");
								Console.WriteLine($"Data di Nascita: {((DateTime)reader["data_di_nascita"]).ToString("yyyy-MM-dd")}");
								Console.WriteLine($"Sesso: {reader["sesso"]}");
								Console.WriteLine($"Indirizzo: {reader["indirizzo"]}");
								Console.WriteLine($"Telefono: {reader["telefono"]}");
								Console.WriteLine($"Email: {reader["email"]}");
								Console.WriteLine($"Data Registrazione: {((DateTime)reader["data_registrazione"]).ToString("yyyy-MM-dd HH:mm:ss")}");
							}
							else
							{
								Console.WriteLine("Paziente non trovato.");
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la visualizzazione dei dettagli del paziente: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void AggiornaPaziente()
		{
			Console.Write("Inserisci l'ID del paziente da aggiornare: ");
			string inputId = Console.ReadLine().Trim();
			int pazienteId;

			while (!int.TryParse(inputId, out pazienteId))
			{
				Console.WriteLine("ID non valido. Inserisci un numero intero.");
				inputId = Console.ReadLine().Trim();
			}

			string nome;
			Console.Write("Inserisci il nuovo nome del paziente (lascia vuoto per non modificare): ");
			nome = Console.ReadLine().Trim();

			string cognome;
			Console.Write("Inserisci il nuovo cognome del paziente (lascia vuoto per non modificare): ");
			cognome = Console.ReadLine().Trim();

			DateTime dataDiNascita = DateTime.MinValue;
			bool dataValida = false;
			do
			{
				Console.Write("Inserisci la nuova data di nascita (YYYY-MM-DD, lascia vuoto per non modificare): ");
				string inputData = Console.ReadLine().Trim();
				if (string.IsNullOrEmpty(inputData) || DateTime.TryParse(inputData, out dataDiNascita))
				{
					dataValida = true;
				}
				else
				{
					Console.WriteLine("Formato data non valido.");
				}
			} while (!dataValida);

			string sesso;
			do
			{
				Console.Write("Inserisci il nuovo sesso (M/F/O, lascia vuoto per non modificare): ");
				sesso = Console.ReadLine().Trim().ToUpper();
				if (string.IsNullOrEmpty(sesso) || sesso == "M" || sesso == "F" || sesso == "O")
				{
					break;
				}
				else
				{
					Console.WriteLine("Sesso non valido.");
				}
			} while (true);

			string indirizzo;
			Console.Write("Inserisci il nuovo indirizzo (lascia vuoto per non modificare): ");
			indirizzo = Console.ReadLine().Trim();

			string telefono;
			do
			{
				Console.Write("Inserisci il nuovo numero di telefono (lascia vuoto per non modificare): ");
				telefono = Console.ReadLine().Trim();
				if (string.IsNullOrEmpty(telefono) || Regex.IsMatch(telefono, @"^\+?\d{7,15}$"))
				{
					break;
				}
				else
				{
					Console.WriteLine("Numero di telefono non valido.");
				}
			} while (true);

			string email;
			do
			{
				Console.Write("Inserisci la nuova email (lascia vuoto per non modificare): ");
				email = Console.ReadLine().Trim();
				if (string.IsNullOrEmpty(email) || Regex.IsMatch(email, @"^[\w\.-]+@[\w\.-]+\.\w{2,}$"))
				{
					break;
				}
				else
				{
					Console.WriteLine("Email non valida.");
				}
			} while (true);

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"UPDATE Pazienti SET 
                                    nome = @Nome,
                                    cognome = @Cognome,
                                    data_di_nascita = @DataDiNascita,
                                    sesso = @Sesso,
                                    indirizzo = @Indirizzo,
                                    telefono = @Telefono,
                                    email = @Email
                                WHERE ID = @ID";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@ID", pazienteId);
						cmd.Parameters.AddWithValue("@Nome", nome);
						cmd.Parameters.AddWithValue("@Cognome", cognome);
						cmd.Parameters.AddWithValue("@DataDiNascita", dataDiNascita);
						cmd.Parameters.AddWithValue("@Sesso", sesso);
						cmd.Parameters.AddWithValue("@Indirizzo", indirizzo);
						cmd.Parameters.AddWithValue("@Telefono", telefono);
						cmd.Parameters.AddWithValue("@Email", email);

						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected > 0)
						{
							Console.WriteLine("Paziente aggiornato correttamente.");
						}
						else
						{
							Console.WriteLine("Nessun paziente trovato con l'ID specificato.");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante l'aggiornamento del paziente: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void EliminaPaziente()
		{
			Console.Write("Inserisci l'ID del paziente da eliminare: ");
			string inputId = Console.ReadLine().Trim();
			int pazienteId;

			while (!int.TryParse(inputId, out pazienteId))
			{
				Console.WriteLine("ID non valido. Inserisci un numero intero.");
				inputId = Console.ReadLine().Trim();
			}

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = "DELETE FROM Pazienti WHERE ID = @ID";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@ID", pazienteId);

						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected > 0)
						{
							Console.WriteLine("Paziente eliminato correttamente.");
						}
						else
						{
							Console.WriteLine("Nessun paziente trovato con l'ID specificato.");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante l'eliminazione del paziente: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void VisualizzaAnalisiMensilePazienti()
		{
			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = "SELECT * FROM analisi_mensile_pazienti ORDER BY mese DESC";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						Console.WriteLine("Analisi Mensile dei Pazienti:");
						Console.WriteLine("Mese\t\tNumero Pazienti");

						while (reader.Read())
						{
							Console.WriteLine($"{((DateTime)reader["mese"]).ToString("yyyy-MM")}\t{reader["numero_pazienti"]}");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la visualizzazione dell'analisi mensile dei pazienti: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void CercaPazientiPerNomeOCognome()
		{
			Console.Write("Inserisci il nome o cognome del paziente da cercare: ");
			string inputRicerca = Console.ReadLine().Trim();

			while (string.IsNullOrEmpty(inputRicerca))
			{
				Console.WriteLine("Il campo di ricerca non può essere vuoto.");
				inputRicerca = Console.ReadLine().Trim();
			}

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"
                    SELECT ID, nome, cognome, data_di_nascita, telefono, email
                    FROM Pazienti
                    WHERE nome LIKE @Ricerca OR cognome LIKE @Ricerca";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Ricerca", "%" + inputRicerca + "%");

						using (NpgsqlDataReader reader = cmd.ExecuteReader())
						{
							Console.WriteLine("Risultati della ricerca:");
							Console.WriteLine("ID\tNome\tCognome\tData di Nascita\tTelefono\tEmail");

							if (!reader.HasRows)
							{
								Console.WriteLine("Nessun paziente trovato con il nome o cognome specificato.");
							}

							while (reader.Read())
							{
								Console.WriteLine($"{reader["ID"]}\t{reader["nome"]}\t{reader["cognome"]}\t" +
												  $"{((DateTime)reader["data_di_nascita"]).ToString("yyyy-MM-dd")}\t" +
												  $"{reader["telefono"]}\t{reader["email"]}");
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la ricerca dei pazienti: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void ContaPazientiPerSesso()
		{
			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"
                    SELECT sesso, COUNT(ID) AS NumeroPazienti
                    FROM Pazienti
                    GROUP BY sesso";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						Console.WriteLine("Conteggio dei pazienti per sesso:");
						Console.WriteLine("Sesso\tNumero Pazienti");

						while (reader.Read())
						{
							Console.WriteLine($"{reader["sesso"]}\t{reader["NumeroPazienti"]}");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante il conteggio dei pazienti per sesso: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void CreaPersonale()
		{
			string nome;
			do
			{
				Console.Write("Inserisci il nome del personale: ");
				nome = Console.ReadLine().Trim();
				if (string.IsNullOrEmpty(nome))
				{
					Console.WriteLine("Il nome non può essere vuoto.");
				}
			} while (string.IsNullOrEmpty(nome));

			string cognome;
			do
			{
				Console.Write("Inserisci il cognome del personale: ");
				cognome = Console.ReadLine().Trim();
				if (string.IsNullOrEmpty(cognome))
				{
					Console.WriteLine("Il cognome non può essere vuoto.");
				}
			} while (string.IsNullOrEmpty(cognome));

			string ruolo;
			do
			{
				Console.Write("Inserisci il ruolo (ADMIN/MEDICO/INFERMIERE/RECEPTIONIST): ");
				ruolo = Console.ReadLine().Trim().ToUpper();
				if (ruolo != "ADMIN" && ruolo != "MEDICO" && ruolo != "INFERMIERE" && ruolo != "RECEPTIONIST")
				{
					Console.WriteLine("Ruolo non valido. Inserisci ADMIN, MEDICO, INFERMIERE o RECEPTIONIST.");
					ruolo = null;
				}
			} while (string.IsNullOrEmpty(ruolo));

			Console.Write("Inserisci il reparto: ");
			string reparto = Console.ReadLine().Trim();

			string telefono;
			do
			{
				Console.Write("Inserisci il numero di telefono: ");
				telefono = Console.ReadLine().Trim();
				if (!Regex.IsMatch(telefono, @"^\+?\d{7,15}$"))
				{
					Console.WriteLine("Numero di telefono non valido. Inserisci un numero con 7-15 cifre, può includere il prefisso internazionale.");
					telefono = null;
				}
			} while (string.IsNullOrEmpty(telefono));

			string email;
			do
			{
				Console.Write("Inserisci l'email: ");
				email = Console.ReadLine().Trim();
				if (!Regex.IsMatch(email, @"^[\w\.-]+@[\w\.-]+\.\w{2,}$"))
				{
					Console.WriteLine("Email non valida. Inserisci un'email valida.");
					email = null;
				}
			} while (string.IsNullOrEmpty(email));

			DateTime dataAssunzione;
			bool dataValida = false;
			do
			{
				Console.Write("Inserisci la data di assunzione (YYYY-MM-DD): ");
				string inputData = Console.ReadLine().Trim();
				if (DateTime.TryParse(inputData, out dataAssunzione))
				{
					dataValida = true;
				}
				else
				{
					Console.WriteLine("Formato data non valido. Inserisci la data nel formato YYYY-MM-DD.");
				}
			} while (!dataValida);

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"
                    INSERT INTO Personale (nome, cognome, ruolo, reparto, telefono, email, data_assunzione) 
                    VALUES (@Nome, @Cognome, @Ruolo, @Reparto, @Telefono, @Email, @DataAssunzione)";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Nome", nome);
						cmd.Parameters.AddWithValue("@Cognome", cognome);
						cmd.Parameters.AddWithValue("@Ruolo", ruolo);
						cmd.Parameters.AddWithValue("@Reparto", reparto);
						cmd.Parameters.AddWithValue("@Telefono", telefono);
						cmd.Parameters.AddWithValue("@Email", email);
						cmd.Parameters.AddWithValue("@DataAssunzione", dataAssunzione);

						cmd.ExecuteNonQuery();
						Console.WriteLine("Personale inserito correttamente.");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante l'inserimento del personale: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void VisualizzaPersonale()
		{
			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = "SELECT ID, nome, cognome, ruolo, reparto, telefono, email, data_assunzione FROM Personale";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						Console.WriteLine("Lista del personale:");
						Console.WriteLine("ID\tNome\tCognome\tRuolo\tReparto\tTelefono\tEmail\tData Assunzione");

						while (reader.Read())
						{
							Console.WriteLine($"{reader["ID"]}\t{reader["nome"]}\t{reader["cognome"]}\t" +
											  $"{reader["ruolo"]}\t{reader["reparto"]}\t{reader["telefono"]}\t" +
											  $"{reader["email"]}\t{((DateTime)reader["data_assunzione"]).ToString("yyyy-MM-dd")}");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la visualizzazione del personale: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void VisualizzaDettagliPersonale()
		{
			Console.Write("Inserisci l'ID del personale di cui vuoi visualizzare i dettagli: ");
			string inputId = Console.ReadLine().Trim();
			int personaleId;

			while (!int.TryParse(inputId, out personaleId))
			{
				Console.WriteLine("ID non valido. Inserisci un numero intero.");
				inputId = Console.ReadLine().Trim();
			}

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = "SELECT * FROM Personale WHERE ID = @ID";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@ID", personaleId);

						using (NpgsqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								Console.WriteLine("Dettagli del personale:");
								Console.WriteLine($"ID: {reader["ID"]}");
								Console.WriteLine($"Nome: {reader["nome"]}");
								Console.WriteLine($"Cognome: {reader["cognome"]}");
								Console.WriteLine($"Ruolo: {reader["ruolo"]}");
								Console.WriteLine($"Reparto: {reader["reparto"]}");
								Console.WriteLine($"Telefono: {reader["telefono"]}");
								Console.WriteLine($"Email: {reader["email"]}");
								Console.WriteLine($"Data Assunzione: {((DateTime)reader["data_assunzione"]).ToString("yyyy-MM-dd")}");
							}
							else
							{
								Console.WriteLine("Personale non trovato.");
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la visualizzazione dei dettagli del personale: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void AggiornaPersonale()
		{
			Console.Write("Inserisci l'ID del personale da aggiornare: ");
			string inputId = Console.ReadLine().Trim();
			int personaleId;

			while (!int.TryParse(inputId, out personaleId))
			{
				Console.WriteLine("ID non valido. Inserisci un numero intero.");
				inputId = Console.ReadLine().Trim();
			}

			Console.Write("Inserisci il nuovo nome del personale (lascia vuoto per non modificare): ");
			string nome = Console.ReadLine().Trim();

			Console.Write("Inserisci il nuovo cognome del personale (lascia vuoto per non modificare): ");
			string cognome = Console.ReadLine().Trim();

			string ruolo;
			do
			{
				Console.Write("Inserisci il nuovo ruolo (ADMIN/MEDICO/INFERMIERE/RECEPTIONIST, lascia vuoto per non modificare): ");
				ruolo = Console.ReadLine().Trim().ToUpper();
				if (string.IsNullOrEmpty(ruolo) || ruolo == "ADMIN" || ruolo == "MEDICO" || ruolo == "INFERMIERE" || ruolo == "RECEPTIONIST")
				{
					break;
				}
				else
				{
					Console.WriteLine("Ruolo non valido.");
				}
			} while (true);

			Console.Write("Inserisci il nuovo reparto (lascia vuoto per non modificare): ");
			string reparto = Console.ReadLine().Trim();

			string telefono;
			do
			{
				Console.Write("Inserisci il nuovo numero di telefono (lascia vuoto per non modificare): ");
				telefono = Console.ReadLine().Trim();
				if (string.IsNullOrEmpty(telefono) || Regex.IsMatch(telefono, @"^\+?\d{7,15}$"))
				{
					break;
				}
				else
				{
					Console.WriteLine("Numero di telefono non valido.");
				}
			} while (true);

			string email;
			do
			{
				Console.Write("Inserisci la nuova email (lascia vuoto per non modificare): ");
				email = Console.ReadLine().Trim();
				if (string.IsNullOrEmpty(email) || Regex.IsMatch(email, @"^[\w\.-]+@[\w\.-]+\.\w{2,}$"))
				{
					break;
				}
				else
				{
					Console.WriteLine("Email non valida.");
				}
			} while (true);

			DateTime? dataAssunzione = null;
			bool dataValida = false;
			do
			{
				Console.Write("Inserisci la nuova data di assunzione (YYYY-MM-DD, lascia vuoto per non modificare): ");
				string inputData = Console.ReadLine().Trim();
				if (string.IsNullOrEmpty(inputData))
				{
					dataValida = true;
				}
				else if (DateTime.TryParse(inputData, out DateTime tempData))
				{
					dataAssunzione = tempData;
					dataValida = true;
				}
				else
				{
					Console.WriteLine("Formato data non valido.");
				}
			} while (!dataValida);

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"
                UPDATE Personale SET 
                    nome = COALESCE(NULLIF(@Nome, ''), nome),
                    cognome = COALESCE(NULLIF(@Cognome, ''), cognome),
                    ruolo = COALESCE(NULLIF(@Ruolo, ''), ruolo),
                    reparto = COALESCE(NULLIF(@Reparto, ''), reparto),
                    telefono = COALESCE(NULLIF(@Telefono, ''), telefono),
                    email = COALESCE(NULLIF(@Email, ''), email),
                    data_assunzione = COALESCE(@DataAssunzione, data_assunzione)
                WHERE ID = @ID";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@ID", personaleId);
						cmd.Parameters.AddWithValue("@Nome", string.IsNullOrEmpty(nome) ? (object)DBNull.Value : nome);
						cmd.Parameters.AddWithValue("@Cognome", string.IsNullOrEmpty(cognome) ? (object)DBNull.Value : cognome);
						cmd.Parameters.AddWithValue("@Ruolo", string.IsNullOrEmpty(ruolo) ? (object)DBNull.Value : ruolo);
						cmd.Parameters.AddWithValue("@Reparto", string.IsNullOrEmpty(reparto) ? (object)DBNull.Value : reparto);
						cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(telefono) ? (object)DBNull.Value : telefono);
						cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email);
						cmd.Parameters.AddWithValue("@DataAssunzione", dataAssunzione.HasValue ? (object)dataAssunzione.Value : DBNull.Value);

						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected > 0)
						{
							Console.WriteLine("Personale aggiornato correttamente.");
						}
						else
						{
							Console.WriteLine("Nessun personale trovato con l'ID specificato.");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante l'aggiornamento del personale: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void EliminaPersonale()
		{
			Console.Write("Inserisci l'ID del personale da eliminare: ");
			string inputId = Console.ReadLine().Trim();
			int personaleId;

			while (!int.TryParse(inputId, out personaleId))
			{
				Console.WriteLine("ID non valido. Inserisci un numero intero.");
				inputId = Console.ReadLine().Trim();
			}

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = "DELETE FROM Personale WHERE ID = @ID";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@ID", personaleId);

						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected > 0)
						{
							Console.WriteLine("Personale eliminato correttamente.");
						}
						else
						{
							Console.WriteLine("Nessun personale trovato con l'ID specificato.");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante l'eliminazione del personale: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void VisualizzaReportSettimanaleAppuntamenti()
		{
			using (NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=admin;Database=hospital_management"))
			{
				try
				{
					conn.Open();
					string query = @"
                SELECT p.nome AS NomePersonale, p.cognome AS CognomePersonale, COUNT(a.ID) AS ConteggioAppuntamenti
                FROM Personale p
                JOIN Appuntamenti a ON p.ID = a.personale_id
                WHERE a.data >= CURRENT_DATE - INTERVAL '7 days'
                GROUP BY p.nome, p.cognome
                ORDER BY ConteggioAppuntamenti DESC";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						Console.WriteLine("Report Settimanale degli Appuntamenti:");
						Console.WriteLine("Nome\tCognome\tNumero di Appuntamenti");

						while (reader.Read())
						{
							Console.WriteLine($"{reader["NomePersonale"]}\t{reader["CognomePersonale"]}\t{reader["ConteggioAppuntamenti"]}");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la visualizzazione del report settimanale degli appuntamenti: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void ContaPersonalePerRuolo()
		{
			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"
                    SELECT ruolo, COUNT(ID) AS NumeroPersonale
                    FROM Personale
                    GROUP BY ruolo";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						Console.WriteLine("Conteggio del personale per ruolo:");
						Console.WriteLine("Ruolo\tNumero Personale");

						while (reader.Read())
						{
							Console.WriteLine($"{reader["ruolo"]}\t{reader["NumeroPersonale"]}");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante il conteggio del personale per ruolo: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void CreaCartellaClinica()
		{
			Console.Write("Inserisci l'ID del paziente: ");
			string inputPazienteId = Console.ReadLine().Trim();
			int pazienteId;

			while (!int.TryParse(inputPazienteId, out pazienteId))
			{
				Console.WriteLine("ID paziente non valido. Inserisci un numero intero.");
				inputPazienteId = Console.ReadLine().Trim();
			}

			Console.Write("Inserisci le note mediche (può essere vuoto): ");
			string noteMediche = Console.ReadLine().Trim();

			string statoAttuale;
			do
			{
				Console.Write("Inserisci lo stato attuale (RICOVERATO/AMBULATORIALE/OSSERVAZIONE/DIMESSO): ");
				statoAttuale = Console.ReadLine().Trim().ToUpper();
				if (statoAttuale != "RICOVERATO" && statoAttuale != "AMBULATORIALE" && statoAttuale != "OSSERVAZIONE" && statoAttuale != "DIMESSO")
				{
					Console.WriteLine("Stato attuale non valido.");
					statoAttuale = null;
				}
			} while (string.IsNullOrEmpty(statoAttuale));

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"
                    INSERT INTO Cartelle_Cliniche (paziente_id, note_mediche, stato_attuale)
                    VALUES (@PazienteId, @NoteMediche, @StatoAttuale)";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@PazienteId", pazienteId);
						cmd.Parameters.AddWithValue("@NoteMediche", string.IsNullOrEmpty(noteMediche) ? (object)DBNull.Value : noteMediche);
						cmd.Parameters.AddWithValue("@StatoAttuale", statoAttuale);

						cmd.ExecuteNonQuery();
						Console.WriteLine("Cartella clinica creata correttamente.");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la creazione della cartella clinica: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void VisualizzaCartelleCliniche()
		{
			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"
                    SELECT ID, paziente_id, data_creazione, note_mediche, stato_attuale 
                    FROM Cartelle_Cliniche 
                    ORDER BY data_creazione DESC";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						Console.WriteLine("Elenco delle Cartelle Cliniche:");
						Console.WriteLine("ID\tPaziente ID\tData Creazione\t\tNote Mediche\tStato Attuale");

						while (reader.Read())
						{
							Console.WriteLine($"{reader["ID"]}\t{reader["paziente_id"]}\t" +
											  $"{((DateTime)reader["data_creazione"]).ToString("yyyy-MM-dd HH:mm:ss")}\t" +
											  $"{(reader["note_mediche"] != DBNull.Value ? reader["note_mediche"] : "N/D")}\t" +
											  $"{reader["stato_attuale"]}");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la visualizzazione delle cartelle cliniche: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void VisualizzaDettagliCartellaClinica()
		{
			Console.Write("Inserisci l'ID della cartella clinica da visualizzare: ");
			string inputId = Console.ReadLine().Trim();
			int cartellaId;

			while (!int.TryParse(inputId, out cartellaId))
			{
				Console.WriteLine("ID non valido. Inserisci un numero intero.");
				inputId = Console.ReadLine().Trim();
			}

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = "SELECT * FROM Cartelle_Cliniche WHERE ID = @ID";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@ID", cartellaId);

						using (NpgsqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								Console.WriteLine("Dettagli della Cartella Clinica:");
								Console.WriteLine($"ID: {reader["ID"]}");
								Console.WriteLine($"Paziente ID: {reader["paziente_id"]}");
								Console.WriteLine($"Data Creazione: {((DateTime)reader["data_creazione"]).ToString("yyyy-MM-dd HH:mm:ss")}");
								Console.WriteLine($"Note Mediche: {(reader["note_mediche"] != DBNull.Value ? reader["note_mediche"] : "N/D")}");
								Console.WriteLine($"Stato Attuale: {reader["stato_attuale"]}");
							}
							else
							{
								Console.WriteLine("Cartella clinica non trovata.");
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la visualizzazione dei dettagli della cartella clinica: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void AggiornaCartellaClinica()
		{
			Console.Write("Inserisci l'ID della cartella clinica da aggiornare: ");
			string inputId = Console.ReadLine().Trim();
			int cartellaId;

			while (!int.TryParse(inputId, out cartellaId))
			{
				Console.WriteLine("ID non valido. Inserisci un numero intero.");
				inputId = Console.ReadLine().Trim();
			}

			Console.Write("Inserisci le nuove note mediche (lascia vuoto per non modificare): ");
			string noteMediche = Console.ReadLine().Trim();

			string statoAttuale;
			do
			{
				Console.Write("Inserisci il nuovo stato attuale (RICOVERATO/AMBULATORIALE/OSSERVAZIONE/DIMESSO, lascia vuoto per non modificare): ");
				statoAttuale = Console.ReadLine().Trim().ToUpper();
				if (string.IsNullOrEmpty(statoAttuale) || statoAttuale == "RICOVERATO" || statoAttuale == "AMBULATORIALE" || statoAttuale == "OSSERVAZIONE" || statoAttuale == "DIMESSO")
				{
					break;
				}
				else
				{
					Console.WriteLine("Stato attuale non valido.");
				}
			} while (true);

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"
                    UPDATE Cartelle_Cliniche SET 
                        note_mediche = COALESCE(NULLIF(@NoteMediche, ''), note_mediche),
                        stato_attuale = COALESCE(NULLIF(@StatoAttuale, ''), stato_attuale)
                    WHERE ID = @ID";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@ID", cartellaId);
						cmd.Parameters.AddWithValue("@NoteMediche", string.IsNullOrEmpty(noteMediche) ? (object)DBNull.Value : noteMediche);
						cmd.Parameters.AddWithValue("@StatoAttuale", string.IsNullOrEmpty(statoAttuale) ? (object)DBNull.Value : statoAttuale);

						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected > 0)
						{
							Console.WriteLine("Cartella clinica aggiornata correttamente.");
						}
						else
						{
							Console.WriteLine("Nessuna cartella clinica trovata con l'ID specificato.");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante l'aggiornamento della cartella clinica: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void EliminaCartellaClinica()
		{
			Console.Write("Inserisci l'ID della cartella clinica da eliminare: ");
			string inputId = Console.ReadLine().Trim();
			int cartellaId;

			while (!int.TryParse(inputId, out cartellaId))
			{
				Console.WriteLine("ID non valido. Inserisci un numero intero.");
				inputId = Console.ReadLine().Trim();
			}

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = "DELETE FROM Cartelle_Cliniche WHERE ID = @ID";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@ID", cartellaId);

						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected > 0)
						{
							Console.WriteLine("Cartella clinica eliminata correttamente.");
						}
						else
						{
							Console.WriteLine("Nessuna cartella clinica trovata con l'ID specificato.");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante l'eliminazione della cartella clinica: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void VisualizzaRicoveriSettimanali()
		{
			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = "SELECT * FROM vista_ricoveri_settimanali";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						Console.WriteLine("Ricoveri Settimanali:");
						Console.WriteLine("Paziente ID\tNumero Ricoveri");

						while (reader.Read())
						{
							Console.WriteLine($"{reader["paziente_id"]}\t\t{reader["numero_ricoveri"]}");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la visualizzazione dei ricoveri settimanali: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void RicercaFullTextNoteMediche()
    {
        Console.Write("Inserisci i termini di ricerca per le note mediche: ");
        string terminiRicerca = Console.ReadLine().Trim();

        while (string.IsNullOrEmpty(terminiRicerca))
        {
            Console.WriteLine("Il campo di ricerca non può essere vuoto.");
            terminiRicerca = Console.ReadLine().Trim();
        }

        // Formattazione della stringa per la ricerca full-text
        terminiRicerca = terminiRicerca.Replace(" ", " & "); // PostgreSQL usa & per "AND" nei termini di ricerca full-text

        using (NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=admin;Database=hospital_management"))
        {
            try
            {
                conn.Open();
                string query = @"
                SELECT ID, paziente_id, data_creazione, note_mediche, stato_attuale
                FROM Cartelle_Cliniche
                WHERE to_tsvector('italian', note_mediche) @@ to_tsquery('italian', @TerminiRicerca)";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TerminiRicerca", terminiRicerca);

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("Risultati della Ricerca nelle Note Mediche:");
                        Console.WriteLine("ID\tPaziente ID\tData Creazione\t\tNote Mediche\tStato Attuale");

                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Nessun risultato trovato per i termini di ricerca specificati.");
                        }

                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["ID"]}\t{reader["paziente_id"]}\t" +
                                              $"{((DateTime)reader["data_creazione"]).ToString("yyyy-MM-dd HH:mm:ss")}\t" +
                                              $"{reader["note_mediche"]}\t{reader["stato_attuale"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore durante la ricerca full-text nelle note mediche: " + ex.Message);
            }
        }
        Console.WriteLine("\nPremere un tasto per continuare...");
        Console.ReadKey();
    }
		public static void CreaAppuntamento()
		{
			Console.Write("Inserisci l'ID del paziente: ");
			string inputPazienteId = Console.ReadLine().Trim();
			int pazienteId;

			while (!int.TryParse(inputPazienteId, out pazienteId))
			{
				Console.WriteLine("ID paziente non valido. Inserisci un numero intero.");
				inputPazienteId = Console.ReadLine().Trim();
			}

			Console.Write("Inserisci l'ID del personale: ");
			string inputPersonaleId = Console.ReadLine().Trim();
			int personaleId;

			while (!int.TryParse(inputPersonaleId, out personaleId))
			{
				Console.WriteLine("ID personale non valido. Inserisci un numero intero.");
				inputPersonaleId = Console.ReadLine().Trim();
			}

			DateTime dataAppuntamento;
			bool dataValida = false;
			do
			{
				Console.Write("Inserisci la data dell'appuntamento (YYYY-MM-DD): ");
				string inputData = Console.ReadLine().Trim();
				if (DateTime.TryParse(inputData, out dataAppuntamento))
				{
					dataValida = true;
				}
				else
				{
					Console.WriteLine("Formato data non valido.");
				}
			} while (!dataValida);

			TimeSpan oraAppuntamento;
			bool oraValida = false;
			do
			{
				Console.Write("Inserisci l'ora dell'appuntamento (HH:MM): ");
				string inputOra = Console.ReadLine().Trim();
				if (TimeSpan.TryParse(inputOra, out oraAppuntamento))
				{
					oraValida = true;
				}
				else
				{
					Console.WriteLine("Formato ora non valido.");
				}
			} while (!oraValida);

			Console.Write("Inserisci il motivo dell'appuntamento: ");
			string motivo = Console.ReadLine().Trim();

			while (string.IsNullOrEmpty(motivo))
			{
				Console.WriteLine("Il motivo non può essere vuoto.");
				motivo = Console.ReadLine().Trim();
			}

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"
                    INSERT INTO Appuntamenti (paziente_id, personale_id, data, ora, motivo)
                    VALUES (@PazienteId, @PersonaleId, @Data, @Ora, @Motivo)";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@PazienteId", pazienteId);
						cmd.Parameters.AddWithValue("@PersonaleId", personaleId);
						cmd.Parameters.AddWithValue("@Data", dataAppuntamento);
						cmd.Parameters.AddWithValue("@Ora", oraAppuntamento);
						cmd.Parameters.AddWithValue("@Motivo", motivo);

						cmd.ExecuteNonQuery();
						Console.WriteLine("Appuntamento creato correttamente.");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la creazione dell'appuntamento: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void VisualizzaAppuntamenti()
		{
			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"
                    SELECT ID, paziente_id, personale_id, data, ora, motivo, stato 
                    FROM Appuntamenti 
                    ORDER BY data DESC, ora DESC";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						Console.WriteLine("Elenco degli Appuntamenti:");
						Console.WriteLine("ID\tPaziente ID\tPersonale ID\tData\t\tOra\t\tMotivo\t\tStato");

						while (reader.Read())
						{
							Console.WriteLine($"{reader["ID"]}\t{reader["paziente_id"]}\t\t{reader["personale_id"]}\t\t" +
											  $"{((DateTime)reader["data"]).ToString("yyyy-MM-dd")}\t" +
											  $"{((TimeSpan)reader["ora"]).ToString(@"hh\:mm")}\t" +
											  $"{reader["motivo"]}\t{reader["stato"]}");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la visualizzazione degli appuntamenti: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void VisualizzaDettagliAppuntamento()
		{
			Console.Write("Inserisci l'ID dell'appuntamento da visualizzare: ");
			string inputId = Console.ReadLine().Trim();
			int appuntamentoId;

			while (!int.TryParse(inputId, out appuntamentoId))
			{
				Console.WriteLine("ID non valido. Inserisci un numero intero.");
				inputId = Console.ReadLine().Trim();
			}

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = "SELECT * FROM Appuntamenti WHERE ID = @ID";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@ID", appuntamentoId);

						using (NpgsqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								Console.WriteLine("Dettagli dell'Appuntamento:");
								Console.WriteLine($"ID: {reader["ID"]}");
								Console.WriteLine($"Paziente ID: {reader["paziente_id"]}");
								Console.WriteLine($"Personale ID: {reader["personale_id"]}");
								Console.WriteLine($"Data: {((DateTime)reader["data"]).ToString("yyyy-MM-dd")}");
								Console.WriteLine($"Ora: {((TimeSpan)reader["ora"]).ToString(@"hh\:mm")}");
								Console.WriteLine($"Motivo: {reader["motivo"]}");
								Console.WriteLine($"Stato: {reader["stato"]}");
							}
							else
							{
								Console.WriteLine("Appuntamento non trovato.");
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la visualizzazione dei dettagli dell'appuntamento: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void AggiornaAppuntamento()
		{
			Console.Write("Inserisci l'ID dell'appuntamento da aggiornare: ");
			string inputId = Console.ReadLine().Trim();
			int appuntamentoId;

			while (!int.TryParse(inputId, out appuntamentoId))
			{
				Console.WriteLine("ID non valido. Inserisci un numero intero.");
				inputId = Console.ReadLine().Trim();
			}

			DateTime? dataAppuntamento = null;
			bool dataValida = false;
			do
			{
				Console.Write("Inserisci la nuova data dell'appuntamento (YYYY-MM-DD, lascia vuoto per non modificare): ");
				string inputData = Console.ReadLine().Trim();
				if (string.IsNullOrEmpty(inputData))
				{
					dataValida = true;
				}
				else if (DateTime.TryParse(inputData, out DateTime tempData))
				{
					dataAppuntamento = tempData;
					dataValida = true;
				}
				else
				{
					Console.WriteLine("Formato data non valido.");
				}
			} while (!dataValida);

			TimeSpan? oraAppuntamento = null;
			bool oraValida = false;
			do
			{
				Console.Write("Inserisci la nuova ora dell'appuntamento (HH:MM, lascia vuoto per non modificare): ");
				string inputOra = Console.ReadLine().Trim();
				if (string.IsNullOrEmpty(inputOra))
				{
					oraValida = true;
				}
				else if (TimeSpan.TryParse(inputOra, out TimeSpan tempOra))
				{
					oraAppuntamento = tempOra;
					oraValida = true;
				}
				else
				{
					Console.WriteLine("Formato ora non valido.");
				}
			} while (!oraValida);

			Console.Write("Inserisci il nuovo motivo dell'appuntamento (lascia vuoto per non modificare): ");
			string motivo = Console.ReadLine().Trim();

			string stato;
			do
			{
				Console.Write("Inserisci il nuovo stato (PROGRAMMATO/COMPLETATO/ANNULLATO, lascia vuoto per non modificare): ");
				stato = Console.ReadLine().Trim().ToUpper();
				if (string.IsNullOrEmpty(stato) || stato == "PROGRAMMATO" || stato == "COMPLETATO" || stato == "ANNULLATO")
				{
					break;
				}
				else
				{
					Console.WriteLine("Stato non valido.");
				}
			} while (true);

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"
                UPDATE Appuntamenti SET 
                    data = COALESCE(@Data, data),
                    ora = COALESCE(@Ora, ora),
                    motivo = COALESCE(NULLIF(@Motivo, ''), motivo),
                    stato = COALESCE(NULLIF(@Stato, ''), stato)
                WHERE ID = @ID";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@ID", appuntamentoId);
						cmd.Parameters.AddWithValue("@Data", dataAppuntamento.HasValue ? (object)dataAppuntamento.Value : DBNull.Value);
						cmd.Parameters.AddWithValue("@Ora", oraAppuntamento.HasValue ? (object)oraAppuntamento.Value : DBNull.Value);
						cmd.Parameters.AddWithValue("@Motivo", string.IsNullOrEmpty(motivo) ? (object)DBNull.Value : motivo);
						cmd.Parameters.AddWithValue("@Stato", string.IsNullOrEmpty(stato) ? (object)DBNull.Value : stato);

						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected > 0)
						{
							Console.WriteLine("Appuntamento aggiornato correttamente.");
						}
						else
						{
							Console.WriteLine("Nessun appuntamento trovato con l'ID specificato.");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante l'aggiornamento dell'appuntamento: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void AnnullaAppuntamento()
		{
			Console.Write("Inserisci l'ID dell'appuntamento da annullare: ");
			string inputId = Console.ReadLine().Trim();
			int appuntamentoId;

			while (!int.TryParse(inputId, out appuntamentoId))
			{
				Console.WriteLine("ID non valido. Inserisci un numero intero.");
				inputId = Console.ReadLine().Trim();
			}

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = "UPDATE Appuntamenti SET stato = 'ANNULLATO' WHERE ID = @ID";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@ID", appuntamentoId);

						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected > 0)
						{
							Console.WriteLine("Appuntamento annullato correttamente.");
						}
						else
						{
							Console.WriteLine("Nessun appuntamento trovato con l'ID specificato.");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante l'annullamento dell'appuntamento: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void CompletaAppuntamento()
		{
			Console.Write("Inserisci l'ID dell'appuntamento da completare: ");
			string inputId = Console.ReadLine().Trim();
			int appuntamentoId;

			while (!int.TryParse(inputId, out appuntamentoId))
			{
				Console.WriteLine("ID non valido. Inserisci un numero intero.");
				inputId = Console.ReadLine().Trim();
			}

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = "UPDATE Appuntamenti SET stato = 'COMPLETATO' WHERE ID = @ID";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@ID", appuntamentoId);

						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected > 0)
						{
							Console.WriteLine("Appuntamento completato correttamente.");
						}
						else
						{
							Console.WriteLine("Nessun appuntamento trovato con l'ID specificato.");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante il completamento dell'appuntamento: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void VisualizzaStoricoAppuntamenti()
		{
			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"
                    SELECT ID, paziente_id, personale_id, data, ora, motivo, stato 
                    FROM storico_appuntamenti 
                    ORDER BY data DESC, ora DESC";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						Console.WriteLine("Storico degli Appuntamenti Completati:");
						Console.WriteLine("ID\tPaziente ID\tPersonale ID\tData\t\tOra\t\tMotivo\t\tStato");

						while (reader.Read())
						{
							Console.WriteLine($"{reader["ID"]}\t{reader["paziente_id"]}\t\t{reader["personale_id"]}\t\t" +
											  $"{((DateTime)reader["data"]).ToString("yyyy-MM-dd")}\t" +
											  $"{((TimeSpan)reader["ora"]).ToString(@"hh\:mm")}\t" +
											  $"{reader["motivo"]}\t{reader["stato"]}");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la visualizzazione dello storico degli appuntamenti: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void RicercaFullTextMotiviAppuntamenti()
		{
			Console.Write("Inserisci i termini di ricerca per i motivi degli appuntamenti: ");
			string terminiRicerca = Console.ReadLine().Trim();

			while (string.IsNullOrEmpty(terminiRicerca))
			{
				Console.WriteLine("Il campo di ricerca non può essere vuoto.");
				terminiRicerca = Console.ReadLine().Trim();
			}

			// Creazione della query di ricerca full-text
			terminiRicerca = terminiRicerca.Replace(" ", " & "); // PostgreSQL usa & per "AND" nei termini di ricerca full-text

			using (NpgsqlConnection conn = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=admin;Database=hospital_management"))
			{
				try
				{
					conn.Open();
					string query = @"
                SELECT ID, paziente_id, personale_id, data, ora, motivo, stato
                FROM Appuntamenti
                WHERE to_tsvector('italian', motivo) @@ to_tsquery('italian', @TerminiRicerca)";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@TerminiRicerca", terminiRicerca);

						using (NpgsqlDataReader reader = cmd.ExecuteReader())
						{
							Console.WriteLine("Risultati della Ricerca nei Motivi degli Appuntamenti:");
							Console.WriteLine("ID\tPaziente ID\tPersonale ID\tData\t\tOra\t\tMotivo\t\tStato");

							if (!reader.HasRows)
							{
								Console.WriteLine("Nessun risultato trovato per i termini di ricerca specificati.");
							}

							while (reader.Read())
							{
								Console.WriteLine($"{reader["ID"]}\t{reader["paziente_id"]}\t\t{reader["personale_id"]}\t\t" +
												  $"{((DateTime)reader["data"]).ToString("yyyy-MM-dd")}\t" +
												  $"{((TimeSpan)reader["ora"]).ToString(@"hh\:mm")}\t" +
												  $"{reader["motivo"]}\t{reader["stato"]}");
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la ricerca full-text nei motivi degli appuntamenti: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void ContaAppuntamentiPerStato()
		{
			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"
                    SELECT stato, COUNT(ID) AS NumeroAppuntamenti
                    FROM Appuntamenti
                    GROUP BY stato";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						Console.WriteLine("Conteggio degli Appuntamenti per Stato:");
						Console.WriteLine("Stato\t\tNumero Appuntamenti");

						while (reader.Read())
						{
							Console.WriteLine($"{reader["stato"]}\t\t{reader["NumeroAppuntamenti"]}");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante il conteggio degli appuntamenti per stato: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void CreaPrescrizione()
		{
			Console.Write("Inserisci l'ID della cartella clinica: ");
			string inputCartellaClinicaId = Console.ReadLine().Trim();
			int cartellaClinicaId;

			while (!int.TryParse(inputCartellaClinicaId, out cartellaClinicaId))
			{
				Console.WriteLine("ID cartella clinica non valido. Inserisci un numero intero.");
				inputCartellaClinicaId = Console.ReadLine().Trim();
			}

			Console.Write("Inserisci il nome del farmaco: ");
			string farmaco = Console.ReadLine().Trim();

			while (string.IsNullOrEmpty(farmaco))
			{
				Console.WriteLine("Il nome del farmaco non può essere vuoto.");
				farmaco = Console.ReadLine().Trim();
			}

			Console.Write("Inserisci il dosaggio del farmaco: ");
			string dosaggio = Console.ReadLine().Trim();

			while (string.IsNullOrEmpty(dosaggio))
			{
				Console.WriteLine("Il dosaggio non può essere vuoto.");
				dosaggio = Console.ReadLine().Trim();
			}

			Console.Write("Inserisci la frequenza del farmaco: ");
			string frequenza = Console.ReadLine().Trim();

			while (string.IsNullOrEmpty(frequenza))
			{
				Console.WriteLine("La frequenza non può essere vuota.");
				frequenza = Console.ReadLine().Trim();
			}

			Console.Write("Inserisci la durata della prescrizione in giorni (lascia vuoto se non specificato): ");
			string inputDurata = Console.ReadLine().Trim();
			int? durata = null;

			if (!string.IsNullOrEmpty(inputDurata))
			{
				if (int.TryParse(inputDurata, out int durataValida) && durataValida > 0)
				{
					durata = durataValida;
				}
				else
				{
					Console.WriteLine("Durata non valida. Procedo senza specificare una durata.");
				}
			}

			Console.Write("Inserisci eventuali note (lascia vuoto se non specificato): ");
			string note = Console.ReadLine().Trim();

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"
                    INSERT INTO Prescrizioni (cartella_clinica_id, farmaco, dosaggio, frequenza, durata, note)
                    VALUES (@CartellaClinicaId, @Farmaco, @Dosaggio, @Frequenza, @Durata, @Note)";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@CartellaClinicaId", cartellaClinicaId);
						cmd.Parameters.AddWithValue("@Farmaco", farmaco);
						cmd.Parameters.AddWithValue("@Dosaggio", dosaggio);
						cmd.Parameters.AddWithValue("@Frequenza", frequenza);
						cmd.Parameters.AddWithValue("@Durata", (object)durata ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@Note", string.IsNullOrEmpty(note) ? (object)DBNull.Value : note);

						cmd.ExecuteNonQuery();
						Console.WriteLine("Prescrizione creata correttamente.");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la creazione della prescrizione: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void VisualizzaPrescrizioni()
		{
			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"
                    SELECT ID, cartella_clinica_id, farmaco, dosaggio, frequenza, durata, note 
                    FROM Prescrizioni 
                    ORDER BY ID DESC";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						Console.WriteLine("Elenco delle Prescrizioni:");
						Console.WriteLine("ID\tCartella Clinica ID\tFarmaco\t\tDosaggio\tFrequenza\tDurata\t\tNote");

						while (reader.Read())
						{
							Console.WriteLine($"{reader["ID"]}\t{reader["cartella_clinica_id"]}\t\t{reader["farmaco"]}\t" +
											  $"{reader["dosaggio"]}\t{reader["frequenza"]}\t" +
											  $"{(reader["durata"] != DBNull.Value ? reader["durata"].ToString() : "N/D")}\t" +
											  $"{(reader["note"] != DBNull.Value ? reader["note"] : "N/D")}");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la visualizzazione delle prescrizioni: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void VisualizzaDettagliPrescrizione()
		{
			Console.Write("Inserisci l'ID della prescrizione da visualizzare: ");
			string inputId = Console.ReadLine().Trim();
			int prescrizioneId;

			while (!int.TryParse(inputId, out prescrizioneId))
			{
				Console.WriteLine("ID non valido. Inserisci un numero intero.");
				inputId = Console.ReadLine().Trim();
			}

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = "SELECT * FROM Prescrizioni WHERE ID = @ID";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@ID", prescrizioneId);

						using (NpgsqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								Console.WriteLine("Dettagli della Prescrizione:");
								Console.WriteLine($"ID: {reader["ID"]}");
								Console.WriteLine($"Cartella Clinica ID: {reader["cartella_clinica_id"]}");
								Console.WriteLine($"Farmaco: {reader["farmaco"]}");
								Console.WriteLine($"Dosaggio: {reader["dosaggio"]}");
								Console.WriteLine($"Frequenza: {reader["frequenza"]}");
								Console.WriteLine($"Durata: {(reader["durata"] != DBNull.Value ? reader["durata"] : "N/D")}");
								Console.WriteLine($"Note: {(reader["note"] != DBNull.Value ? reader["note"] : "N/D")}");
							}
							else
							{
								Console.WriteLine("Prescrizione non trovata.");
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la visualizzazione dei dettagli della prescrizione: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void AggiornaPrescrizione()
		{
			Console.Write("Inserisci l'ID della prescrizione da aggiornare: ");
			string inputId = Console.ReadLine().Trim();
			int prescrizioneId;

			while (!int.TryParse(inputId, out prescrizioneId))
			{
				Console.WriteLine("ID non valido. Inserisci un numero intero.");
				inputId = Console.ReadLine().Trim();
			}

			Console.Write("Inserisci il nuovo farmaco (lascia vuoto per non modificare): ");
			string farmaco = Console.ReadLine().Trim();

			Console.Write("Inserisci il nuovo dosaggio (lascia vuoto per non modificare): ");
			string dosaggio = Console.ReadLine().Trim();

			Console.Write("Inserisci la nuova frequenza (lascia vuoto per non modificare): ");
			string frequenza = Console.ReadLine().Trim();

			Console.Write("Inserisci la nuova durata in giorni (lascia vuoto per non modificare): ");
			string inputDurata = Console.ReadLine().Trim();
			int? durata = null;

			if (!string.IsNullOrEmpty(inputDurata) && int.TryParse(inputDurata, out int durataValida) && durataValida > 0)
			{
				durata = durataValida;
			}

			Console.Write("Inserisci le nuove note (lascia vuoto per non modificare): ");
			string note = Console.ReadLine().Trim();

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"
                    UPDATE Prescrizioni SET 
                        farmaco = COALESCE(NULLIF(@Farmaco, ''), farmaco),
                        dosaggio = COALESCE(NULLIF(@Dosaggio, ''), dosaggio),
                        frequenza = COALESCE(NULLIF(@Frequenza, ''), frequenza),
                        durata = COALESCE(@Durata, durata),
                        note = COALESCE(NULLIF(@Note, ''), note)
                    WHERE ID = @ID";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@ID", prescrizioneId);
						cmd.Parameters.AddWithValue("@Farmaco", string.IsNullOrEmpty(farmaco) ? (object)DBNull.Value : farmaco);
						cmd.Parameters.AddWithValue("@Dosaggio", string.IsNullOrEmpty(dosaggio) ? (object)DBNull.Value : dosaggio);
						cmd.Parameters.AddWithValue("@Frequenza", string.IsNullOrEmpty(frequenza) ? (object)DBNull.Value : frequenza);
						cmd.Parameters.AddWithValue("@Durata", (object)durata ?? DBNull.Value);
						cmd.Parameters.AddWithValue("@Note", string.IsNullOrEmpty(note) ? (object)DBNull.Value : note);

						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected > 0)
						{
							Console.WriteLine("Prescrizione aggiornata correttamente.");
						}
						else
						{
							Console.WriteLine("Nessuna prescrizione trovata con l'ID specificato.");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante l'aggiornamento della prescrizione: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void EliminaPrescrizione()
		{
			Console.Write("Inserisci l'ID della prescrizione da eliminare: ");
			string inputId = Console.ReadLine().Trim();
			int prescrizioneId;

			while (!int.TryParse(inputId, out prescrizioneId))
			{
				Console.WriteLine("ID non valido. Inserisci un numero intero.");
				inputId = Console.ReadLine().Trim();
			}

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = "DELETE FROM Prescrizioni WHERE ID = @ID";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@ID", prescrizioneId);

						int rowsAffected = cmd.ExecuteNonQuery();
						if (rowsAffected > 0)
						{
							Console.WriteLine("Prescrizione eliminata correttamente.");
						}
						else
						{
							Console.WriteLine("Nessuna prescrizione trovata con l'ID specificato.");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante l'eliminazione della prescrizione: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void RicercaPrescrizioniPerFarmaco()
		{
			Console.Write("Inserisci il nome del farmaco da cercare: ");
			string farmacoRicerca = Console.ReadLine().Trim();

			while (string.IsNullOrEmpty(farmacoRicerca))
			{
				Console.WriteLine("Il campo di ricerca non può essere vuoto.");
				farmacoRicerca = Console.ReadLine().Trim();
			}

			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = @"
                    SELECT ID, cartella_clinica_id, farmaco, dosaggio, frequenza, durata, note
                    FROM Prescrizioni
                    WHERE farmaco LIKE @FarmacoRicerca";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@FarmacoRicerca", "%" + farmacoRicerca + "%");

						using (NpgsqlDataReader reader = cmd.ExecuteReader())
						{
							Console.WriteLine("Risultati della Ricerca per Farmaco:");
							Console.WriteLine("ID\tCartella Clinica ID\tFarmaco\t\tDosaggio\tFrequenza\tDurata\t\tNote");

							if (!reader.HasRows)
							{
								Console.WriteLine("Nessuna prescrizione trovata per il farmaco specificato.");
							}

							while (reader.Read())
							{
								Console.WriteLine($"{reader["ID"]}\t{reader["cartella_clinica_id"]}\t\t{reader["farmaco"]}\t" +
												  $"{reader["dosaggio"]}\t{reader["frequenza"]}\t" +
												  $"{(reader["durata"] != DBNull.Value ? reader["durata"].ToString() : "N/D")}\t" +
												  $"{(reader["note"] != DBNull.Value ? reader["note"] : "N/D")}");
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la ricerca delle prescrizioni per farmaco: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void VisualizzaLogEventi()
		{
			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = "SELECT ID, evento, descrizione, data_evento FROM log_eventi ORDER BY data_evento DESC";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						Console.WriteLine("Log degli Eventi:");
						Console.WriteLine("ID\tEvento\t\tDescrizione\t\t\tData Evento");

						while (reader.Read())
						{
							Console.WriteLine($"{reader["ID"]}\t{reader["evento"]}\t\t{reader["descrizione"]}\t" +
											  $"{((DateTime)reader["data_evento"]).ToString("yyyy-MM-dd HH:mm:ss")}");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la visualizzazione del log eventi: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}
		public static void VisualizzaLogModifiche()
		{
			

			using (NpgsqlConnection conn = new NpgsqlConnection(CONNECTION_STRING))
			{
				try
				{
					conn.Open();
					string query = "SELECT ID, cartella_clinica_id, modifica, data_modifica FROM log_modifiche ORDER BY data_modifica DESC";

					using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
					using (NpgsqlDataReader reader = cmd.ExecuteReader())
					{
						Console.WriteLine("Log delle Modifiche:");
						Console.WriteLine("ID\tCartella Clinica ID\tModifica\t\t\tData Modifica");

						while (reader.Read())
						{
							Console.WriteLine($"{reader["ID"]}\t{reader["cartella_clinica_id"]}\t\t{reader["modifica"]}\t" +
											  $"{((DateTime)reader["data_modifica"]).ToString("yyyy-MM-dd HH:mm:ss")}");
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Errore durante la visualizzazione del log modifiche: " + ex.Message);
				}
			}
			Console.WriteLine("\nPremere un tasto per continuare...");
			Console.ReadKey();
		}


	}
}