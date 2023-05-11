using System;
using Bot.MessageExchange;
using MySql.Data.MySqlClient;
using TimeTableCore;
using DataBaseCore;

namespace DataBaseCore;

internal sealed partial class DataBaseHandler
{
    public static void AddUser(IUser user)
    {
        if (!IsUserInDB(user))
        {
            RequestGenerator.INSERT(GetStringForINSERT(user), "users(id, name)");
        }
    }
    public static string GetUserRole(IUser user)
    {
        return RequestGenerator.SELECT("role", "users",$"WHERE id = '{user.Id}'")[0][0];

    }

    //Нужно реализовать изменение информации о пользователях в БД
    private static string GetStringForINSERT(IUser user)
    {
        if (user != null)
            return $"{user.Id},'{user.FirstName}'";

        return null;
    }

    private static bool IsUserInDB(IUser user)
    {
        return RequestGenerator.SELECT("id", "users")[0].Contains(user.Id.ToString());
    }

}

