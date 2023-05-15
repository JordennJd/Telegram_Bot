using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot.Domain.Entities;
using Bot.Domain.Interfaces;

namespace Bot.Infrastructure.DataBaseCore
{
    public class ChatHandler
    {
        public static bool AddChat(CoreChat chat)
    {
        if (!IsChatInDB(chat))
        {
            RequestGenerator.INSERT(GetStringForINSERT(chat), "chats(id, directory)");
            return true;
        }
        else return false;
    }
    public static string GetChatDirectory(Chat chat)
    {
        return RequestGenerator.SELECT("directory", "chats", $"WHERE id = '{chat.Id}'")[0][0];
    }

    //TODO Нужно реализовать изменение информации о пользователях в БД
    private static string GetStringForINSERT(CoreChat chat)
    {
        if (chat != null)
            return $"{chat.Id},'{chat.Directory}'";

        return null;
    }

    public static bool IsChatInDB(Chat chat)
    {
        List<string []> request = RequestGenerator.SELECT("id", "Chats");
        if(request.Count == 0){
            return false;
        }
        else{
            return request[0][0].Contains(chat.Id.ToString());
        }
        
    }
    }
}