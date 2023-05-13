using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Telegram.Bot;

//Зависимости архитектуры
using Bot.MessageExchange.Imperative;
using Bot.Domain.Interfaces;

namespace Bot.MessageExchange.TelegramMesExc
{
    public partial class TelegramMessageExchangeManager : IMessageExchangeManager
    {
        private static List<Chat> chats;
        private class Update : IUpdate
        {
            private Message _message;
            public IMessage Message{get{return _message;}}
            public Update(Telegram.Bot.Types.Update update){
                _message=new Message(update.Message);
            }
        }
        private class Message: IMessage
        {
            private User _user;
            private Chat _chat;
            public string Text{get;}  

            public IUser User{get{return _user;}}
            public IChat Chat{get{return _chat;}}
            
            public Message(User user, Chat chat, string text){
                _user = user;
                _chat = chat;
                Text = text;
            }
            public Message(Telegram.Bot.Types.Message message){

                // Chat findChat = chats.Find(x => x.Id==message.Chat.Id);
                // if(findChat != null){
                //  _chat = findChat;
                // }
                // else{
                //     _chat = new Chat(message.Chat);
                //     chats.Add(_chat);
                // }
                _chat = new Chat(message.Chat);
                _user = new User(message.From);
                Text = message.Text;
            }
        }
        private class User : IUser
        {
            public long Id{get;}
            public string FirstName{get; set;}
            public User(long id, string firstName){
                Id = id;
                FirstName = firstName;
            }
            public User(Telegram.Bot.Types.User user){
                Id=user.Id;
                FirstName = user.FirstName;
            }
        }
        private class Chat:IChat
        {
            public AutoResetEvent GetStringEvent{get;} //Заглушка потока для функции TelegramRequestString
            public long Id{get;}
            public string Title{get;}
            public string BuferString{get; set;}
            public Chat(long id, string title){
                Id = id;
                Title = title;
                GetStringEvent = new AutoResetEvent(false);
            }
            public Chat(Telegram.Bot.Types.Chat chat){
                Id=chat.Id;
                Title = chat.Title;
                GetStringEvent = new AutoResetEvent(false);
            }

            public Chat(IChat chat){
                Id=chat.Id;
                Title=chat.Title;
                GetStringEvent = new AutoResetEvent(false);
            }
        }

        private TelegramInputHandler _inputHandler;
        private TelegramOutputHandler _outputHandler;
        static private TelegramBotClient botClient;
        //6023689671:AAFTPEPJpWXSmJDXZF4lQZlvYvIciDAS1HI
        //6237061864:AAGDYW6Q7kjln3VQeVgPvrQUzzmLcP5Fv5Q
        private string telegramBotToken = "6023689671:AAFTPEPJpWXSmJDXZF4lQZlvYvIciDAS1HI";

        event IMessageExchangeManager.UpdateEventHungler UpdateEvent;

        object objectLock = new Object();
        
        event IMessageExchangeManager.UpdateEventHungler IMessageExchangeManager.UpdateEvent
        {
            add
            {
                lock (objectLock)
                {
                    UpdateEvent += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    UpdateEvent -= value;
                }
            }
        }
        

        /// <summary>
        /// Подключается к боту в телеграмме.
        /// </summary>
        public void Initialize()
        {
            _inputHandler = new TelegramInputHandler();
            _outputHandler = new TelegramOutputHandler();
            botClient = new TelegramBotClient(telegramBotToken);
            botClient.StartReceiving(telegrameUpdate, error);
        }
        private async Task telegrameUpdate(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken token){
            if(update.Message!= null && update.Message.Text!= null)
                if(_inputHandler.FindChatAndSetEventWait(new Chat(update.Message.Chat), update.Message.Text))
                    return;

                    
            UpdateEvent?.Invoke(new Update(update)); 
         
        }
        private Task error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3) //Обработка каких то ошибок
        {
            Console.WriteLine(arg2.ToString());
            throw new NotImplementedException();
            
        }

       

        public IInputHandler GetInputHandler()
        {
            return _inputHandler;
        }

        public IOutputHandler GetOutputHandler()
        {
            return _outputHandler;
        }
    }
}
