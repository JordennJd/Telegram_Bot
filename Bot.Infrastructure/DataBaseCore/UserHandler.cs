using System;
using Bot.MessageExchange;
using MySql.Data.MySqlClient;
using TimeTableCore;
using Bot.Infrastructure.DataBaseCore;
using Bot.Domain.Interfaces;

namespace Bot.Infrastructure.DataBaseCore;

internal sealed partial class DataBaseHandler
{
    public static void AddUser(User user)
    {
        if (!IsUserInDB(user))
        {
            RequestGenerator.INSERT(GetStringForINSERT(user), "users(id, name)");
        }
    }
    public static string GetUserRole(User user)
    {
        return RequestGenerator.SELECT("role", "users", $"WHERE id = '{user.Id}'")[0][0];
    }

    //TODO Нужно реализовать изменение информации о пользователях в БД
    private static string GetStringForINSERT(User user)
    {
        if (user != null)
            return $"{user.Id},'{user.FirstName}'";

        return null;
    }

    private static bool IsUserInDB(User user)
    {
        return RequestGenerator.SELECT("id", "users")[0][0].Contains(user.Id.ToString());
    }

}

