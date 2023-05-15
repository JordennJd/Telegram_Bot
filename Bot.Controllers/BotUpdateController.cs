using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Bot.Domain.Entities;
using Bot.MessageExchange.Imperative;
using Bot.Domain.Interfaces;
using Bot.Infrastructure.DataBaseCore;
using TimeTableCore;

namespace Bot.Controllers;

static class BotUpdateController
{
    private static IOutputHandler Output;
    private static IInputHandler Input;

    private static Buttons AllButtonsInController;

    public static void Initialize(IMessageExchangeManager messageExchangeManager)
    {

        messageExchangeManager.UpdateEvent += Update;

        Output = messageExchangeManager.GetOutputHandler();
        Input = messageExchangeManager.GetInputHandler();

        AllButtonsInController  = new Buttons(){
                new Button[] { new Button("/start", Start) },
                new Button[] { new Button("Новый Предмет", AddLesson) , new Button("Расписание на сегодня", GetTimeTable)}
        };
            
    }

    public static void Update(IUpdate update)
    {
        CoreUpdate newCoreUpdate = new CoreUpdate(update, AllButtonsInController);
        if(update.Message.User.Id==1202179202){ //Пока так, мой айдишник тут, тип админ
            newCoreUpdate = new CoreUpdate(update, AllButtonsInController);
        }
        else{
            newCoreUpdate = new CoreUpdate(update, new Buttons(){
                new Button[] { new Button("/start", Start) },
                new Button[] { new Button("Расписание на сегодня", GetTimeTable)}
        });
        }
        if(!Bot.Infrastructure.DataBaseCore.DataBaseHandler.AddUser(update.Message.User)){
            //Тут логика если есть в бд юзер
        }
        if(!ChatHandler.AddChat(newCoreUpdate.Message.Chat)){
            newCoreUpdate.Message.Chat.ChangeButtons(ChatHandler.GetChatDirectory(update.Message.Chat), AllButtonsInController); 

        }
        PushButton(newCoreUpdate);
    }

    private static void PushButton(CoreUpdate update)
    {
        update.Message.Chat.Buttons.FindButtonForText(update.Message.Text)?.PushButton(new ForFunctionEventArgs(update));

    }


    public static async Task Start(object sender, ForFunctionEventArgs e)
    {
        
        await Output.RequestMessageSending(e.update.Message.Chat, "Hellow world!", e.update.Message.Chat.Buttons);

    }

    public static async Task GetTimeTable(object sender, ForFunctionEventArgs e)
    {

        await Output.RequestMessageSending(e.update.Message.Chat, TimeTableCore.TimeTableHandler.GetCorrentTimeTable(), e.update.Message.Chat.Buttons);

    }

    public static async Task AddLesson(object sender, ForFunctionEventArgs e)
    {
        await Output.RequestMessageSending(e.update.Message.Chat,
            "Напишите ифнормацию о предмете(название,аудитория,преподаватель)",ButtonsButton: null);
        string Info = await Input.RequestMessageReceiving(e.update.Message.Chat);
        
        await Output.RequestMessageSending(e.update.Message.Chat,
            "Напишите номер дня недели(1-7)",
            new string[][] { new string[] { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" } });
        string DayOfWeek = await Input.RequestMessageReceiving(e.update.Message.Chat);
        
        //Функция проверки дня недели
        await Output.RequestMessageSending(e.update.Message.Chat,
            "Напишите номер пары(1-7)", 
            new string[][] { new string[] { "1", "2", "3", "4", "5", "6" } });
        string PairNumber = await Input.RequestMessageReceiving(e.update.Message.Chat);
        
        //Функция проверки номера пары
        await Output.RequestMessageSending(e.update.Message.Chat,
            "Выбирите модификацию пары", 
            new string[][] { new string[] { "all", "red", "blue" } });
        string Modification = await Input.RequestMessageReceiving(e.update.Message.Chat);
        
        Lesson lesson = new Lesson(Info, DayOfWeek, PairNumber, Modification);
        
        if (!Bot.Infrastructure.DataBaseCore.DataBaseHandler.IsLessonExist(lesson))
        {
            Bot.Infrastructure.DataBaseCore.DataBaseHandler.AddLesson(lesson);
            await Output.RequestMessageSending(e.update.Message.Chat, "Предмет добавлен",
                ButtonsButton: e.update.Message.Chat.Buttons);
        }

        else
        {
            await Output.RequestMessageSending(e.update.Message.Chat,
                "Предмет этой парой в этот день уже существует, хотите заменить?",
                new string[][] { new string[] { "Да", "Нет" } });
            string answer = await Input.RequestMessageReceiving(e.update.Message.Chat);
            
            if (answer == "Да")
            {
                Bot.Infrastructure.DataBaseCore.DataBaseHandler.DeleteLesson(lesson);
                Bot.Infrastructure.DataBaseCore.DataBaseHandler.AddLesson(lesson);
                await Output.RequestMessageSending(e.update.Message.Chat, "Предмет заменен",
                    ButtonsButton: e.update.Message.Chat.Buttons);
            }

            else
            {
                await Output.RequestMessageSending(e.update.Message.Chat, "Ничего не изменено",
                    ButtonsButton: e.update.Message.Chat.Buttons);
            }
        }
    }

}

