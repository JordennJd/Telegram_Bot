using Bot.MessageExchange;
using DataBaseCore;
using TimeTableCore;
using System;
using Telegram.Bot.Types;

public class Program
{
    static IMessageExchangeManager messageExchangeManager;
    static IOutputHandler Output;
    static IInputHandler Input;
    private static void Main()
    {
        Console.WriteLine("Бот запущен.");

        messageExchangeManager = new TelegramMessageExchangeManager();
        Button[][] meinMenu = new Button[][]{
            new Button[]{new Button("/start", Start)},
            new Button[]{new Button("Расписание на Сегодня", GetTimeTable)},
            new Button[]{new Button("Настройки", GetSettings)},
            new Button[]{new Button("Добавить/Заменить предмет в расписании", AddLesson, "Admin")}
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
        await Output.RequestMessageSending(e.update.Message.Chat, "Hellow world!", DataBaseCore.DataBaseHandler.GetUserRole(e.update.Message.User), e.update.Message.Chat.Buttons);
        DataBaseCore.DataBaseHandler.AddUser(e.update.Message.User);

    }
    public static async Task GetSettings(object sender, ForFunctionEventArgs e)
    {
        
    }
    public static async Task GetTimeTable(object sender, ForFunctionEventArgs e)
    {

        await Output.RequestMessageSending(e.update.Message.Chat,
            TimeTableCore.TimeTableHandler.doTimeTableBuild(), DataBaseCore.DataBaseHandler.GetUserRole(e.update.Message.User), e.update.Message.Chat.Buttons);

    }
    public static async Task AddLesson(object sender, ForFunctionEventArgs e){
        await Output.RequestMessageSending(e.update.Message.Chat,
             "Напишите ифнормацию о предмете(название,аудитория,преподаватель)", DataBaseCore.DataBaseHandler.GetUserRole(e.update.Message.User), ButtonsButton: null);

        string Info = await Input.RequestMessageReceiving(e.update.Message.Chat);

        await Output.RequestMessageSending(e.update.Message.Chat,
             "Напишите номер дня недели(1-7)",DataBaseCore.DataBaseHandler.GetUserRole(e.update.Message.User),new string[][] {new string[]{"Понедельник","Вторник","Среда","Четверг","Пятница","Суббота"}});
        string DayOfWeek = await Input.RequestMessageReceiving(e.update.Message.Chat);
        //Функция проверки дня недели
        await Output.RequestMessageSending(e.update.Message.Chat,
             "Напишите номер пары(1-7)", DataBaseCore.DataBaseHandler.GetUserRole(e.update.Message.User), new string[][] {new string[]{"1","2","3","4","5","6","7"} });
        string PairNumber = await Input.RequestMessageReceiving(e.update.Message.Chat);
        //Функция проверки номера пары
        await Output.RequestMessageSending(e.update.Message.Chat,
             "Выбирите модификацию пары", DataBaseCore.DataBaseHandler.GetUserRole(e.update.Message.User), new string[][] {new string[]{"all","red","blue"}} );
        string Modification = await Input.RequestMessageReceiving(e.update.Message.Chat);
        Lesson lesson = new Lesson(Info, DayOfWeek, PairNumber, Modification);
        if (!DataBaseCore.DataBaseHandler.IsLessonExist(lesson))
        {
            DataBaseCore.DataBaseHandler.AddLesson(lesson);
            await Output.RequestMessageSending(e.update.Message.Chat, "Предмет добавлен", DataBaseCore.DataBaseHandler.GetUserRole(e.update.Message.User), ButtonsButton: e.update.Message.Chat.Buttons);
        }
        else
        {
            await Output.RequestMessageSending(e.update.Message.Chat,
                "Предмет этой парой в этот день уже существует, хотите заменить?", DataBaseCore.DataBaseHandler.GetUserRole(e.update.Message.User), new string[][] { new string[] { "Да", "Нет" } });
            string answer = await Input.RequestMessageReceiving(e.update.Message.Chat);
            if (answer == "Да")
            {
                DataBaseCore.DataBaseHandler.DeleteLesson(lesson);
                DataBaseCore.DataBaseHandler.AddLesson(lesson);
                await Output.RequestMessageSending(e.update.Message.Chat, "Предмет заменен", DataBaseCore.DataBaseHandler.GetUserRole(e.update.Message.User), ButtonsButton: e.update.Message.Chat.Buttons);
            }
            else
            {
                await Output.RequestMessageSending(e.update.Message.Chat, "Ничего не изменено", DataBaseCore.DataBaseHandler.GetUserRole(e.update.Message.User), ButtonsButton: e.update.Message.Chat.Buttons);

            }
        }

    }

}
