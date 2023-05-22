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
        
    }
    private static async void PushButton(CoreUpdate update)
    {
        Button button = update.Message.Chat.Buttons.FindButtonForText(update.Message.Text);
        if(button != null)
            button.PushButton(new ForFunctionEventArgs(update));
        else{
            await Output.RequestMessageSending(update.Message.Chat, "Неверный запрос!", update.Message.Chat.Buttons);
        }
    }

    private static void ChangeButtons(CoreChat chat , Buttons buttons){
        chat.ChangeButtons(buttons);
    }

    public static void Update(IUpdate update)
    {
        Sessia curentSessia = Sessias.GetOrAddSessia(update.Message.User.Id, (functions)=>{
            CoreChat coreChat = null;
            CoreUser coreUser;
            //id==1202179202 || 1047654455
            Predicate<long> isAdmin = (long id) =>  id==1047654455;

            if(DataBaseHandler.IsUserInDB(update.Message.User)){
                coreUser = new CoreUser(update.Message.User.Id, DataBaseHandler.GetUserName(update.Message.User), DataBaseHandler.GetUserRole(update.Message.User));
            }else{
                ChangesArgsForCoreUpdate args;
                if(isAdmin(update.Message.User.Id))
                    args = new ChangesArgsForCoreUpdate(RoleInUser: "admin");
                else
                    args = new ChangesArgsForCoreUpdate(RoleInUser: "Tvar drozhashchaya");
                coreUser = new CoreUser(update.Message.User, args);
                DataBaseHandler.AddUser(coreUser);
            }
            


            bool chatInDb =ChatHandler.IsChatInDB(update.Message.Chat);
            if(chatInDb){
                coreChat = CoreChat.CreateInstance(update.Message.Chat, ChatHandler.GetChatDirectory(update.Message.Chat), functions);
            }
            if(coreChat == null){
                ChangesArgsForCoreUpdate args;
                if(isAdmin(update.Message.User.Id))
                    args = new ChangesArgsForCoreUpdate(ButtonsInChat: new Buttons{
                        new Button[] { new Button("/start", Start) },
                        new Button[] { new Button("Новый Предмет", AddLesson) , new Button("Расписание на сегодня", GetTimeTable)},
                        new Button[] { new Button("MenuTest", MenuTest) }
                    });
                else{
                    args = new ChangesArgsForCoreUpdate(ButtonsInChat: new Buttons{
                        new Button[] { new Button("/start", Start) },
                        new Button[] { new Button("Расписание на сегодня", GetTimeTable)},
                        new Button[] { new Button("MenuTest", MenuTest) }
                        });
                    
                }
                coreChat = new CoreChat(update.Message.Chat, args);  
            }
            if(!chatInDb)
                    ChatHandler.AddChat(coreChat);
                    
            return (coreUser, coreChat);
        });
        
        curentSessia.ResetTimer();
        PushButton(new CoreUpdate(new CoreMessage(curentSessia.User, curentSessia.Chat, update.Message.Text)));
    }

    public static async Task MenuTest(object sender, ForFunctionEventArgs e)
    {
        Buttons menu = new Buttons{new Button[]{new Button("Кнопка в меню",ButtonOnMenu)}};
        e.update.Message.Chat.ChangeButtons(menu);
        await Output.RequestMessageSending(e.update.Message.Chat, "Менюшка", menu);

    }
    public static async Task ButtonOnMenu(object sender, ForFunctionEventArgs e)
    {
        Buttons menu = new Buttons{
            new Button[] { new Button("/start", Start) },
            new Button[] { new Button("Новый Предмет", AddLesson) , new Button("Расписание на сегодня", GetTimeTable)},
            new Button[] { new Button("MenuTest", MenuTest)}
        };
        e.update.Message.Chat.ChangeButtons(menu);
        
        await Output.RequestMessageSending(e.update.Message.Chat, "Ты в менюшке был", menu);
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

