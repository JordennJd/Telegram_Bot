using Bot.Domain.Entities;
using Bot.MessageExchange.Imperative;
using Bot.Domain.Interfaces;
using Bot.Infrastructure.DataBaseCore;
using Telegram_Bot.Bot.Infrastructure.InputHelper;
using TimeTableCore;

namespace Bot.Controllers;

static class BotUpdateController
{
    public static IOutputHandler Output;
    public static IInputHandler Input;

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

    private static void Update(IUpdate update)
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
                        new Button[] { new Button("Расписание на сегодня", GetTimeTable)},
                        new Button[] { new Button("Настройки", Settings)}
                    });
                else{
                    args = new ChangesArgsForCoreUpdate(ButtonsInChat: new Buttons{
                        new Button[] { new Button("/start", Start) },
                        new Button[] { new Button("Расписание на сегодня", GetTimeTable)}
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

    public static async Task Settings(object sender, ForFunctionEventArgs e)
    {
        Buttons settings = new Buttons{new Button[]
            {new Button("Кнопка в меню",ButtonOnMenu),new Button("Добавить новый предмет/изменить старый",AddLesson),
                new Button("Удалить пару из расписания",DeleteLesson)}};
        e.update.Message.Chat.ChangeButtons(settings);

        await Output.RequestMessageSending(e.update.Message.Chat, "Настройки", settings);

    }
    public static async Task DeleteLesson(object sender, ForFunctionEventArgs e)
    {
        string[] Information = new string[4];
        
        await InputHandler.DoCorrectInputDayOfWeek(sender, e, Information);
        await InputHandler.DoCorrectInputPairNumber(sender, e, Information);
        await InputHandler.DoCorrectInputModification(sender, e, Information);
        
        Lesson lesson = new Lesson("Nomatter", Information[1],Information[2], Information[3]);
        if (DataBaseHandler.IsLessonExist(lesson))
        {
            DataBaseHandler.DeleteLesson(lesson);
            await Output.RequestMessageSending(e.update.Message.Chat, "Пара удалена");
        }
        else
        {
            await Output.RequestMessageSending(e.update.Message.Chat, "Такой пары нету, ничего не изменилось");
        }
        
        RegularButtons.GotoMenu(sender,e);;
        
        

    }
    public static async Task ButtonOnMenu(object sender, ForFunctionEventArgs e)
    {
        RegularButtons.GotoMenu(sender,e);
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
        string[] Information = new string[4];
        
        await InputHandler.DoCorrectInputInfo(sender, e, Information);
        await InputHandler.DoCorrectInputDayOfWeek(sender, e, Information);
        await InputHandler.DoCorrectInputPairNumber(sender, e, Information);
        await InputHandler.DoCorrectInputModification(sender, e, Information);

        
        Lesson lesson = new Lesson(Information[0], Information[1],Information[2], Information[3]);
        
        if (!DataBaseHandler.IsLessonExist(lesson))
        {
            DataBaseHandler.AddLesson(lesson);
        }

        else
        {
            InputHandler.ReplaceLesson(sender,e,lesson);
        }
        await RegularButtons.GotoMenu(sender,e);
    }

    
}

