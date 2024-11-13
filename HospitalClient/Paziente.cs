using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalClient
{
	internal class Paziente
	{
		static void MenuPazienti()
		{
			bool back = false;
			while (!back)
			{
				Console.Clear();
				Console.WriteLine("Gestione Pazienti:");
				Console.WriteLine("1. Crea Paziente");
				Console.WriteLine("2. Visualizza Pazienti");
				Console.WriteLine("3. Visualizza Dettagli Paziente");
				Console.WriteLine("4. Aggiorna Paziente");
				Console.WriteLine("5. Elimina Paziente");
				Console.WriteLine("6. Visualizza Analisi Mensile Pazienti");
				Console.WriteLine("7. Torna al Menu Principale");
				Console.Write("Seleziona un'opzione: ");
				var choice = Console.ReadLine();

				switch (choice)
				{
					case "1":
						CreaPaziente();
						break;
					case "2":
						VisualizzaPazienti();
						break;
					case "3":
						Console.Write("Inserisci ID del paziente: ");
						int id = int.Parse(Console.ReadLine());
						VisualizzaDettagliPaziente(id);
						break;
					case "4":
						Console.Write("Inserisci ID del paziente: ");
						id = int.Parse(Console.ReadLine());
						AggiornaPaziente(id);
						break;
					case "5":
						Console.Write("Inserisci ID del paziente: ");
						id = int.Parse(Console.ReadLine());
						EliminaPaziente(id);
						break;
					case "6":
						VisualizzaAnalisiMensilePazienti();
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
	}
}
