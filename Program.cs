﻿//using Hackathon_Gazprom.DataBaseCore;
using Bot.MessageExchange;
using DataBaseCore;

public class Program
{
    static IMessageExchangeManager messageExchangeManager;
    static IOutputHandler Output;
    static IInputHandler Input;
    private static void Main(string[] args)
    {
        Console.WriteLine("Бот запущен.");

        messageExchangeManager = new TelegramMessageExchangeManager();
        Button[][] meinMenu = new Button[][]{
            new Button[]{new Button("/start", Start)},
            new Button[]{new Button("Request", Request)}
        };
        messageExchangeManager.Initialize(meinMenu); 
        messageExchangeManager.UpdateEvent+=Update; //Привязываем вход программы

        Output = messageExchangeManager.GetOutputHandler();
        Input = messageExchangeManager.GetInputHandler();
        
        Console.ReadLine();
    }
    public static void Update(IUpdate update){
        //ТУТ ВХОД ПРОГРАММЫ

        messageExchangeManager.PushButton(update); //Функция которая вызовет функции в зависимости от написанного текста
    }

    //Тут функции которые будут вызываться при нажатии кнопки
    public static async Task Start(object sender, ForFunctionEventArgs e){
        await Output.RequestMessageSending(e.update.Message.Chat, "Hellow world!", e.update.Message.Chat.Buttons); 
        DataBaseCore.DataBaseHandler.AddUser(e.update.Message.User);

    }
    public static async Task Request(object sender, ForFunctionEventArgs e){

        await Output.RequestMessageSending(e.update.Message.Chat, "Введите строку", ButtonsButton: null); //ButtonsButton: null - удаляет клавиатуру
        string buf =await Input.RequestMessageReceiving(e.update.Message.Chat);
        
        await Output.RequestMessageSending(e.update.Message.Chat, buf, e.update.Message.Chat.Buttons);
    }

}
