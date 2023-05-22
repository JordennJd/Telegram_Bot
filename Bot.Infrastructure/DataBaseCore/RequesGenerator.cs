using MySql.Data.MySqlClient;
namespace Bot.Infrastructure.DataBaseCore.InnerRealisation;
public sealed class RequestGenerator
{
    private static string ConnectionSTR;
    private static MySqlConnection conn;
    static RequestGenerator(){
        switch (Environment.MachineName)
        {
            case ("DESKTOP-AI7DA69"):
                ConnectionSTR = "server=localhost;user=root;database=lol;password=123123";
                break;
            case ("MacBook-Air-Danil"): 
                ConnectionSTR = "server=localhost;user=root;database=lol;password=lfybk2000";
                break;
            
        }
        conn= new MySqlConnection(ConnectionSTR);
    }


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

    //UPDATE users SET role = 'admin', name = 'danil' WHERE id = '1047654455';
    public static void UPDATE(string value, string TABLE_INFO, string WHERE){
        conn.Open();
        new MySqlCommand($"UPDATE {TABLE_INFO} SET {value} WHERE {WHERE}", conn).ExecuteScalar();
        conn.Close();
    }
    public static List<string[]> SELECT(string value, string table, string WHERE = "")
    {
        conn.Open();
        MySqlCommand SelectAllId = new MySqlCommand($"SELECT {value} FROM {table} {WHERE}", conn);
        MySqlDataReader reader = SelectAllId.ExecuteReader();
        List<string[]> Values = ReaderHandler(reader);
        conn.Close();
        return Values;
    }

    private static List<string[]> ReaderHandler(MySqlDataReader reader)
    {
        List<string[]> values = new List<string[]>();
        while (reader.Read())
        {
            string[] Row = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
            {
                Row[i] = reader[i].ToString();
            }
            values.Add(Row);
        }
        return values;
    }


    }

