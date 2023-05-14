using Telegram.Bot;

//Зависимости архитектуры
using Bot.MessageExchange.Imperative;
using Bot.Domain.Interfaces;


namespace Bot.MessageExchange.TelegramMesExc
{
    public partial class TelegramMessageExchangeManager 
    {
        private class TelegramInputHandler : IInputHandler
        {
            private List<TelegramChat> awaitChats;

            public TelegramInputHandler()
            {
                awaitChats = new List<TelegramChat>();
            }

            public async Task<string> RequestMessageReceiving(Chat chat)
            {   
                TelegramChat awaitChat = new TelegramChat(chat);
                awaitChats.Add(awaitChat);
                await Task.Run(() => {awaitChat.GetStringEvent.WaitOne();});
                return awaitChat.BuferString;
            }

            public bool FindChatAndSetEventWait(TelegramChat chat, string Text)
            {  
                int Index = awaitChats.FindIndex(0, awaitChats.Count, x=> x.Id ==chat.Id);

                if(Index!=-1)
                {
                    awaitChats[Index].BuferString=Text;
                    awaitChats[Index].GetStringEvent.Set();
                    awaitChats.RemoveAt(Index);
                    return true;
                }
                return false;
            }

        
        }

    }
}
