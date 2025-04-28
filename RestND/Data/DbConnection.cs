using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows; // For MessageBox

namespace RestND.Data
{
    public class DbConnection
    {
        public MySqlConnection Connection;

        public DbConnection(string server, string database, string userId, string password)
        {
            string connectionString = $"Server={server};Port=3306;Database={database};User ID={userId};Password={password};";
            Connection = new MySqlConnection(connectionString);
        }

        public void OpenConnection()
        {
            if (Connection.State == System.Data.ConnectionState.Closed)
            {
                Connection.Open();
                Console.WriteLine("Database connection opened.");
            }
        }

        public void CloseConnection()
        {
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                Connection.Close();
                Console.WriteLine("Database connection closed.");
            }
        }
    }

    public class DatabaseOperations : DbConnection
    {
        public DatabaseOperations(string server, string database, string userId, string password)
            : base(server, database, userId, password) { }

        // Regular ExecuteReader (without transaction)
        public List<Dictionary<string, object>> ExecuteReader(string query, params MySqlParameter[] parameters)
        {
            var results = new List<Dictionary<string, object>>();

            try
            {
                OpenConnection();

                using var command = new MySqlCommand(query, Connection);
                command.Parameters.AddRange(parameters);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader[i];
                    }
                    results.Add(row);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL Error (Reader): {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General Error (Reader): {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                CloseConnection();
            }

            return results;
        }

        // ExecuteReader with transaction support
        public List<Dictionary<string, object>> ExecuteReader(string query, MySqlConnection conn, MySqlTransaction transaction, params MySqlParameter[] parameters)
        {
            var results = new List<Dictionary<string, object>>();

            try
            {
                using var command = new MySqlCommand(query, conn, transaction);
                command.Parameters.AddRange(parameters);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader[i];
                    }
                    results.Add(row);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL Error (Reader-Transaction): {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General Error (Reader-Transaction): {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return results;
        }

        // Regular ExecuteNonQuery (without transaction)
        public int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
        {
            try
            {
                OpenConnection();

                using var command = new MySqlCommand(query, Connection);
                command.Parameters.AddRange(parameters);

                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} row(s) affected.");
                return rowsAffected;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL Error (NonQuery): {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General Error (NonQuery): {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
            finally
            {
                CloseConnection();
            }
        }

        // ExecuteNonQuery with transaction support
        public int ExecuteNonQuery(string query, MySqlConnection conn, MySqlTransaction transaction, params MySqlParameter[] parameters)
        {
            try
            {
                using var command = new MySqlCommand(query, conn, transaction);
                command.Parameters.AddRange(parameters);

                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} row(s) affected.");
                return rowsAffected;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL Error (NonQuery-Transaction): {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General Error (NonQuery-Transaction): {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        // NEW: ExecuteScalar with transaction support (to get single value like LAST_INSERT_ID)
        public object ExecuteScalar(string query, MySqlConnection conn, MySqlTransaction transaction, params MySqlParameter[] parameters)
        {
            try
            {
                using var command = new MySqlCommand(query, conn, transaction);
                command.Parameters.AddRange(parameters);
                return command.ExecuteScalar();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL Error (Scalar-Transaction): {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General Error (Scalar-Transaction): {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
