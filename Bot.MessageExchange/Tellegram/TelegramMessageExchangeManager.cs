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

namespace Bot.MessageExchange.TelegramMesExc;

public partial class TelegramMessageExchangeManager : IMessageExchangeManager
{
    private static List<TelegramChat> chats;

    abstract class Update : IUpdate
    {
        public virtual Message Message { get; }
    }

    private class TelegramUpdate : Update
    { 
        public override TelegramMessage Message{get; }
        public TelegramUpdate(Telegram.Bot.Types.Update update){
            Message =new TelegramMessage(update.Message);
        }
    }
    private class TelegramMessage: Message
    {
            
        public override string Text{get;}  
        public override TelegramUser User{ get; }
        public override TelegramChat Chat { get; }
            
        public TelegramMessage(Telegram.Bot.Types.Message message)
        {

            Chat = new TelegramChat(message.Chat);
            User = new TelegramUser(message.From);
            Text = message.Text;
        }
    }
    private class TelegramUser : User
    {
        public override long Id{get;}
        public override string FirstName{get; set;}
        public TelegramUser(Telegram.Bot.Types.User user){
            Id=user.Id;
            FirstName = user.FirstName;
        }
    }
    private class TelegramChat:Chat
    {
        //Внутренняя реализация TelegramChat
        public AutoResetEvent GetStringEvent{get;} //Заглушка потока для функции TelegramRequestString
        public string BuferString { get; set; }


        public override long Id{get;}
        public override string Title{get;}
    
        public TelegramChat(Telegram.Bot.Types.Chat chat)
        {
            Id=chat.Id;
            Title = chat.Title;
            GetStringEvent = new AutoResetEvent(false);
        }

        public TelegramChat(Chat chat)
        {
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

    private async Task telegrameUpdate(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken token)
    {
        if(update.Message!= null && update.Message.Text!= null)
            if(_inputHandler.FindChatAndSetEventWait(new TelegramChat(update.Message.Chat), update.Message.Text))
                return;
     
        UpdateEvent?.Invoke(new TelegramUpdate(update)); 
    }


    public IInputHandler GetInputHandler()
    {
        return _inputHandler;
    }
    public IOutputHandler GetOutputHandler()
    {
        return _outputHandler;
    }

    private Task error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3) //Обработка каких то ошибок
    {
        Console.WriteLine(arg2.ToString());
        throw new NotImplementedException();
    }
}

