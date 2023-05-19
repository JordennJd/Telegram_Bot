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

    public static void Initialize(IMessageExchangeManager messageExchangeManager)
    {

        messageExchangeManager.UpdateEvent += Update;

        Output = messageExchangeManager.GetOutputHandler();
        Input = messageExchangeManager.GetInputHandler();
    }

    public static void Update(IUpdate update)
    {


        PushButton(new CoreUpdate(update,
            new Button[][]
            {
                new Button[] { new Button("/start", Start) },
                new Button[] { new Button("Новый Предмет", AddLesson) , new Button("Расписание на сегодня", GetTimeTable), }
            })
        );
    }

    private static void PushButton(CoreUpdate update)
    {
        foreach (Button[] buts in update.Message.Chat.Buttons)
        {
            foreach (Button but in buts)
            {
                if (but.Text == update.Message.Text)
                    but.PushButton(new ForFunctionEventArgs(update));
            }
        }

    }


    public static async Task Start(object sender, ForFunctionEventArgs e)
    {
        Bot.Infrastructure.DataBaseCore.DataBaseHandler.AddUser(e.update.Message.User);
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
        do
        {
            if (!InputCheker.isInputCorrect(Info, 1))
            {
                await Output.RequestMessageSending(e.update.Message.Chat,
                    "Длина информации о предмете должна состовлять больше 3 символов",ButtonsButton: null);
                Info = await Input.RequestMessageReceiving(e.update.Message.Chat);

            }
        } while (!InputCheker.isInputCorrect(Info,1));
        
        await Output.RequestMessageSending(e.update.Message.Chat,
            "Напишите номер дня недели(1-7)",
            new string[][] { new string[] { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" } });
        string DayOfWeek = await Input.RequestMessageReceiving(e.update.Message.Chat);
        do
        {
            if (!InputCheker.isInputCorrect(DayOfWeek, 2))
            {
                await Output.RequestMessageSending(e.update.Message.Chat,
                    "Выбирите день недели из предложенных вариантов",new string[][] { new string[] { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" } });
                DayOfWeek = await Input.RequestMessageReceiving(e.update.Message.Chat);

            }
        } while (!InputCheker.isInputCorrect(DayOfWeek,2));
        //Функция проверки дня недели
        await Output.RequestMessageSending(e.update.Message.Chat,
            "Напишите номер пары(1-6)", 
            new string[][] { new string[] { "1", "2", "3", "4", "5", "6" } });
        string PairNumber = await Input.RequestMessageReceiving(e.update.Message.Chat);
        do
        {
            if (!InputCheker.isInputCorrect(PairNumber, 3))
            {
                await Output.RequestMessageSending(e.update.Message.Chat,
                    "Выбирите число от 1 до 6",new string[][] { new string[] { "1", "2", "3", "4", "5", "6" } });
                PairNumber = await Input.RequestMessageReceiving(e.update.Message.Chat);

            }
        } while (!InputCheker.isInputCorrect(PairNumber,3));
        //Функция проверки номера пары
        await Output.RequestMessageSending(e.update.Message.Chat,
            "Выбирите модификацию пары", 
            new string[][] { new string[] { "all", "red", "blue" } });
        string Modification = await Input.RequestMessageReceiving(e.update.Message.Chat);
        do
        {
            if (!InputCheker.isInputCorrect(Modification, 4))
            {
                await Output.RequestMessageSending(e.update.Message.Chat,
                    "Выбирите из предложенных выриантов",new string[][] { new string[] { "all", "red", "blue" } });
                Modification = await Input.RequestMessageReceiving(e.update.Message.Chat);

            }
        } while (!InputCheker.isInputCorrect(Modification,4));
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

