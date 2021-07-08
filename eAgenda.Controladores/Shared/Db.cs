using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.SQLite;

namespace eAgenda.Controladores.Shared
{
    public delegate T ConverterDelegate<T>(IDataReader reader);

    public static class Db
    {
        private static readonly string connectionString = "";

        static Db()
        {
            //conversa com appconfig
            connectionString = ConfigurationManager.ConnectionStrings["DBeAgenda"].ConnectionString;
            //SQLiteConnection.ClearAllPools();
        }

        public static int Insert(string sql, Dictionary<string, object> parameters)
        {
            if (connectionString.Contains("SQLite"))
            {
                SQLiteConnection connection = new SQLiteConnection(connectionString);

                SQLiteCommand command = new SQLiteCommand(sql + ";SELECT last_insert_rowid()", connection);

                SetParameterParaSQLite(command, parameters);

                connection.Open();

                int id = Convert.ToInt32(command.ExecuteScalar());

                connection.Close();

                return id;
            }
            else
            {
                SqlConnection connection = new SqlConnection(connectionString);

                SqlCommand command = new SqlCommand(sql.AppendSelectIdentity(), connection);

                command.SetParameters(parameters);

                connection.Open();

                int id = Convert.ToInt32(command.ExecuteScalar());

                connection.Close();

                return id;
            }
            
        }      
        public static void Update(string sql, Dictionary<string, object> parameters = null)
        {
            if (connectionString.Contains("SQLite"))
            {
                SQLiteConnection connection = new SQLiteConnection(connectionString);

                SQLiteCommand command = new SQLiteCommand(sql, connection);
                SetParameterParaSQLite(command, parameters);

                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();
            }
            else
            {
                SqlConnection connection = new SqlConnection(connectionString);

                SqlCommand command = new SqlCommand(sql, connection);

                command.SetParameters(parameters);

                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();
            }
        }
        public static void Delete(string sql, Dictionary<string, object> parameters)
        {
            Update(sql, parameters);
        }
        public static List<T> GetAll<T>(string sql, ConverterDelegate<T> convert, Dictionary<string, object> parameters = null)
        {
            if (connectionString.Contains("SQLite"))
            {
                SQLiteConnection connection = new SQLiteConnection(connectionString);

                SQLiteCommand command = new SQLiteCommand(sql, connection);

                SetParameterParaSQLite(command, parameters);

                connection.Open();

                var list = new List<T>();

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var obj = convert(reader);
                    list.Add(obj);
                }
                reader.Close();
                connection.Close();
                return list;
            }
            else
            {
                SqlConnection connection = new SqlConnection(connectionString);

                SqlCommand command = new SqlCommand(sql, connection);

                command.SetParameters(parameters);

                connection.Open();

                var list = new List<T>();

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var obj = convert(reader);
                    list.Add(obj);
                }

                connection.Close();
                return list;
            }
           
        }
        public static T Get<T>(string sql, ConverterDelegate<T> convert, Dictionary<string, object> parameters)
        {
            if (connectionString.Contains("SQLite"))
            {
                SQLiteConnection connection = new SQLiteConnection(connectionString);

                SQLiteCommand command = new SQLiteCommand(sql, connection);

                SetParameterParaSQLite(command, parameters);

                connection.Open();

                T t = default;

                var reader = command.ExecuteReader();

                if (reader.Read())
                    t = convert(reader);
                reader.Close();
                connection.Close();
                return t;
            }
            else
            {
                SqlConnection connection = new SqlConnection(connectionString);

                SqlCommand command = new SqlCommand(sql, connection);

                command.SetParameters(parameters);

                connection.Open();

                T t = default;

                var reader = command.ExecuteReader();

                if (reader.Read())
                    t = convert(reader);

                connection.Close();
                return t;
            }
            
        }
        public static bool Exists(string sql, Dictionary<string, object> parameters)
        {
            if (connectionString.Contains("SQLite"))
            {
                SQLiteConnection connection = new SQLiteConnection(connectionString);

                SQLiteCommand command = new SQLiteCommand(sql, connection);

                SetParameterParaSQLite(command, parameters);

                connection.Open();

                int numberRows = Convert.ToInt32(command.ExecuteScalar());

                connection.Close();

                return numberRows > 0;
            }
            else
            {
                SqlConnection connection = new SqlConnection(connectionString);

                SqlCommand command = new SqlCommand(sql, connection);

                command.SetParameters(parameters);

                connection.Open();

                int numberRows = Convert.ToInt32(command.ExecuteScalar());

                connection.Close();

                return numberRows > 0;
            }
            
        }         
        
        public static bool IsNullOrEmpty(this object value)
        {
            return (value is string && string.IsNullOrEmpty((string)value)) ||
                    value == null;
        }
        private static void SetParameters(this SqlCommand command, Dictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return;

            foreach (var parameter in parameters)
            {
                string name = parameter.Key;

                object value = parameter.Value.IsNullOrEmpty() ? DBNull.Value : parameter.Value;

                SqlParameter dbParameter = new SqlParameter(name, value);

                command.Parameters.Add(dbParameter);
            }
        }
        private static void SetParameterParaSQLite(this SQLiteCommand command, Dictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return;

            foreach (var parameter in parameters)
            {
                string name = parameter.Key;

                object value = parameter.Value.IsNullOrEmpty() ? DBNull.Value : parameter.Value;

                SQLiteParameter dbParameter = new SQLiteParameter(name, value);

                command.Parameters.Add(dbParameter);
            }
        }
        private static string AppendSelectIdentity(this string sql)
        {
            return sql + ";SELECT SCOPE_IDENTITY()";
        }
    }
}
