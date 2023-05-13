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
            private List<Chat> awaitChats;
            public TelegramInputHandler(){
                awaitChats = new List<Chat>();
            }
            public async Task<string> RequestMessageReceiving(IChat chat)
            {   
                Chat awaitChat = new Chat(chat);
                awaitChats.Add(awaitChat);
                await Task.Run(() => {awaitChat.GetStringEvent.WaitOne();});
                return awaitChat.BuferString;
            }

             
            public bool FindChatAndSetEventWait(Chat chat, string Text){  
                int i = awaitChats.FindIndex(0, awaitChats.Count, x=> x.Id ==chat.Id);
                if(i!=-1){
                    awaitChats[i].BuferString=Text;
                    awaitChats[i].GetStringEvent.Set();
                    awaitChats.RemoveAt(i);
                    return true;
                }
                else{
                    return false;
                }
            }

        
        }

    }
}
