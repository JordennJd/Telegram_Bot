using Bot.Infrastructure.DataBaseCore.InnerRealisation;
using Bot.Domain.Interfaces;
using Bot.Domain.Entities;

namespace Bot.Infrastructure.DataBaseCore;

internal sealed partial class DataBaseHandler
{
    public static void AddUser(CoreUser user)
    {
            RequestGenerator.INSERT(GetStringForINSERT(user), "users(id, name, role)");
    }
    public static void UpdateUser(CoreUser user){
        RequestGenerator.UPDATE($"name = '{user.FirstName}', role = '{user.Role}'","users", $"id = '{user.Id}'");
    }
    
    public static string GetUserRole(User user)
    {
        return RequestGenerator.SELECT("role", "users", $"WHERE id = '{user.Id}'")[0][0];
    }
     public static string GetUserName(User user)
    {
        return RequestGenerator.SELECT("name", "users", $"WHERE id = '{user.Id}'")[0][0];
    }

    //TODO Нужно реализовать изменение информации о пользователях в БД
    private static string GetStringForINSERT(CoreUser user)
    {
        return $"{user.Id},'{user.FirstName}'";
    }

    public static bool IsUserInDB(User user)
    {
        List<string [] > request = RequestGenerator.SELECT("id", "users");
        if(request.Count == 0){
            return false;
        }
        else return request[0][0].Contains(user.Id.ToString()); 
    }

}

