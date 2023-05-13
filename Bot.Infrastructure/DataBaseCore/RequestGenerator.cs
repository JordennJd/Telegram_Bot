using System;
using Bot.MessageExchange.Imperative;
using MySql.Data.MySqlClient;

//Архитектурные зависимости
using Bot.Domain.TimeTableCore;
using Bot.Domain.Interfaces;

namespace Bot.Infrastructure.DataBaseCore{

static class DataBaseHandler
{
public static void AddUser(IUser user)
{
    if (!IsUserInDB(user))
    {
        RequestGenerator.INSERT(GetStringForINSERT(user), "users(id, name)");
    }
}

//Нужно реализовать изменение информации о пользователях в БД
private static string GetStringForINSERT(IUser user)
{
    if (user != null)
        return $"{user.Id},'{user.FirstName}'";

    return null;
}

public static void AddLesson(ILesson lesson)
{
    RequestGenerator.INSERT(GetStringForINSERT(lesson),"TimeTable(Info,DayOfWeek,PairNumber,Modification)");
}
private static string GetStringForINSERT(ILesson lesson = null)
{
    if (lesson != null)
        return $"'{lesson.Info}','{lesson.DayOfWeek}','{lesson.PairNumber}','{lesson.Modification}'";

    return null;
}

private static bool IsUserInDB(IUser user)
{
    return RequestGenerator.SELECT("id","users").Contains(user.Id.ToString());
}

}


file sealed class RequestGenerator
{

private static string ConnectionSTR = "server=localhost;user=root;database=lol;password=lfybk2000";
private static MySqlConnection conn = new MySqlConnection(ConnectionSTR);

public static void INSERT(string value, string TABLE_INFO)
{
    conn.Open();
        new MySqlCommand($"INSERT INTO {TABLE_INFO} VALUES ({value})", conn).ExecuteScalar();
    conn.Close();
}

public static List<string> SELECT(string value, string table, string WHERE = "")
{
    List<string> values = new List<string>();

    conn.Open();

    MySqlCommand SelectAllId = new MySqlCommand($"SELECT {value} FROM {table} {WHERE}", conn);
    MySqlDataReader reader = SelectAllId.ExecuteReader();

    while (reader.Read())
    {
        values.Add(reader[0].ToString());
    }
    conn.Close();

    return values;
}

}
}
