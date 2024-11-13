using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalClient
{
	internal class Utils
	{
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
	}
}
