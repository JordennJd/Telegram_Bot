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

    private static Sessias Sessias;

    public static void Initialize(IMessageExchangeManager messageExchangeManager)
    {
        messageExchangeManager.UpdateEvent += Update;

        Output = messageExchangeManager.GetOutputHandler();
        Input = messageExchangeManager.GetInputHandler();

        Sessias = new Sessias();

        //Сюда надо добавлять все кнопки которые есть для работы в этом контроллере.
        AllButtonsInController  = new Buttons(new Button[][]{
                new Button[] { new Button("/start", Start) },
                new Button[] { new Button("Новый Предмет", AddLesson) , new Button("Расписание на сегодня", GetTimeTable)},
                new Button[] { new Button("d", ButtonOnMenu) },
                new Button[] { new Button("d", MenuTest) },
        });
        
        
        Sessias.AddButtonInListAllButtons(AllButtonsInController);
    }
    private static void PushButton(CoreUpdate update)
    {
        update.Message.Chat.Buttons.FindButtonForText(update.Message.Text)?.PushButton(new ForFunctionEventArgs(update));

    }

    private static void ChangeButtons(CoreChat chat , Buttons buttons){
        chat.ChangeButtons(buttons);
    }

    private static void Update(IUpdate update)
    {
        Sessia curentSessia = Sessias.GetOrAddSessia(update.Message.User.Id, ()=>{
            CoreChat coreChat;
            CoreUser coreUser;
            //id==1202179202 || 1047654455
            Predicate<long> isAdmin = (long id) =>  id==1047654455;

            if(DataBaseHandler.IsUserInDB(update.Message.User)){
                coreUser = new CoreUser(update.Message.User.Id, DataBaseHandler.GetUserName(update.Message.User), DataBaseHandler.GetUserRole(update.Message.User));
            }else{
                ChangesArgsForCoreUpdate args;
                if(isAdmin(update.Message.User.Id))
                    args = new ChangesArgsForCoreUpdate("admin",null);
                else
                    args = new ChangesArgsForCoreUpdate("Tvar drozhashchaya",null);
                coreUser = new CoreUser(update.Message.User, args);
                DataBaseHandler.AddUser(coreUser);
            }
            
            if(ChatHandler.IsChatInDB(update.Message.Chat)){
                coreChat = new CoreChat(update.Message.Chat, ChatHandler.GetChatDirectory(update.Message.Chat), AllButtonsInController);
            }else{
                ChangesArgsForCoreUpdate args;
                if(isAdmin(update.Message.User.Id))
                    args = new ChangesArgsForCoreUpdate(null, new Buttons(new Button[][]{
                        new Button[] { new Button("/start", Start) },
                        new Button[] { new Button("Новый Предмет", AddLesson) , new Button("Расписание на сегодня", GetTimeTable)},
                        new Button[] { new Button("MenuTest", MenuTest) }
                    }));
                else{
                    args = new ChangesArgsForCoreUpdate(null, new Buttons(new Button[][]{
                        new Button[] { new Button("/start", Start) },
                        new Button[] { new Button("Расписание на сегодня", GetTimeTable)},
                        new Button[] { new Button("MenuTest", MenuTest) }
                        }));
                    
                }
                coreChat = new CoreChat(update.Message.Chat, args);
                ChatHandler.AddChat(coreChat);
            }
            return (coreUser, coreChat);
        });
        
        curentSessia.ResetTimer();
        PushButton(new CoreUpdate(new CoreMessage(curentSessia.User, curentSessia.Chat, update.Message.Text)));
    }

    public static async Task MenuTest(object sender, ForFunctionEventArgs e)
    {
        Buttons menu = new Buttons(new Button[][]{new Button[]{new Button("Кнопка в меню",ButtonOnMenu)}});
        e.update.Message.Chat.ChangeButtons(menu);
        await Output.RequestMessageSending(e.update.Message.Chat, "Менюшка", menu);

    }
    public static async Task ButtonOnMenu(object sender, ForFunctionEventArgs e)
    {
        Buttons menu = new Buttons(new Button[][]{
                new Button[] { new Button("/start", Start) },
                new Button[] { new Button("Новый Предмет", AddLesson) , new Button("Расписание на сегодня", GetTimeTable)},
                new Button[] { new Button("MenuTest", MenuTest) }}
            );
        e.update.Message.Chat.ChangeButtons(menu);
        await Output.RequestMessageSending(e.update.Message.Chat, "Ты в менюшке был", menu);
    }


    public static async Task Start(object sender, ForFunctionEventArgs e)
    {
        
        await Output.RequestMessageSending(e.update.Message.Chat, "Hellow world!", e.update.Message.Chat.Buttons);

    }

    public static async Task GetTimeTable(object sender, ForFunctionEventArgs e)
    {

        await Output.RequestMessageSending(e.update.Message.Chat,
            TimeTableHandler.GetCorrentTimeTable(DataBaseHandler.GetAllPairs()),
            e.update.Message.Chat.Buttons);

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

