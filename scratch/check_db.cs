using Microsoft.Data.SqlClient;
using System;

string connectionString = "Server=localhost;Database=IARS_DB;Trusted_Connection=True;TrustServerCertificate=True;";
using (SqlConnection connection = new SqlConnection(connectionString))
{
    connection.Open();
    string query = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'KaizenProposals'";
    using (SqlCommand command = new SqlCommand(query, connection))
    {
        using (SqlDataReader reader = command.ExecuteReader())
        {
            Console.WriteLine("Columns in KaizenProposals (IARS_DB):");
            while (reader.Read())
            {
                Console.WriteLine(reader["COLUMN_NAME"]);
            }
        }
    }
}

connectionString = "Server=localhost;Database=IARS;Trusted_Connection=True;TrustServerCertificate=True;";
try {
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        string query = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'KaizenProposals'";
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            using (SqlDataReader reader = command.ExecuteReader())
            {
                Console.WriteLine("\nColumns in KaizenProposals (IARS):");
                while (reader.Read())
                {
                    Console.WriteLine(reader["COLUMN_NAME"]);
                }
            }
        }
    }
} catch (Exception ex) {
    Console.WriteLine("\nCould not connect to IARS database: " + ex.Message);
}
