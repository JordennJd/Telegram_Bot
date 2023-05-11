using System;
using MySql.Data.MySqlClient;

namespace DataBaseCore
{
    public sealed class RequestGenerator
    {

        private static string ConnectionSTR = "server=localhost;user=root;database=lol;password=lfybk2000";
        private static MySqlConnection conn = new MySqlConnection(ConnectionSTR);

        public static void INSERT(string value, string TABLE_INFO)
        {
            conn.Open();
            new MySqlCommand($"INSERT INTO {TABLE_INFO} VALUES ({value})", conn).ExecuteScalar();
            conn.Close();
        }

        public static void DELETE(string value, string TABLE_INFO)
        {
            conn.Open();
            new MySqlCommand($"DELETE FROM {TABLE_INFO} WHERE {value}", conn).ExecuteScalar();
            conn.Close();
        }

        public static List<string[]> SELECT(string value, string table, string WHERE = "")
        {
            List<string[]> values = new List<string[]>();

            conn.Open();

            MySqlCommand SelectAllId = new MySqlCommand($"SELECT {value} FROM {table} {WHERE}", conn);
            MySqlDataReader reader = SelectAllId.ExecuteReader();
            if(reader.HasRows)
            while (reader.Read())
            {
                string[] pair = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    pair[i] = reader[i].ToString();
                }
                values.Add(pair);
            }
            else
            {
                values.Add(new string[] { "VOID" });
            }
            conn.Close();



            return values;
        }

    }
}

