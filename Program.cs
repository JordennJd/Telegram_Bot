using Bot.MessageExchange.TelegramMesExc;
using Bot.Controllers;


public class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Бот запущен.");

        var messageExchangeManager = new TelegramMessageExchangeManager();

        messageExchangeManager.Initialize(); 
        BotUpdateController.Initialize(messageExchangeManager);
         //Привязываем вход программы
         
        Console.ReadLine();
    }
}
