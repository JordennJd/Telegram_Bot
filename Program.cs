using Bot.MessageExchange;
using DataBaseCore;
using TimeTableCore;

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
            new Button[]{new Button("Request", Request)},
            new Button[]{new Button("Новый Предмет", AddLesson)}
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
        // await Output.RequestMessageSending(e.update.Message.Chat,".", new string [][]{ new string[]{"/start"}});
        DataBaseCore.DataBaseHandler.AddUser(e.update.Message.User);

    }
    public static async Task Request(object sender, ForFunctionEventArgs e){

        await Output.RequestMessageSending(e.update.Message.Chat, "Введите строку", ButtonsButton: null); //ButtonsButton: null - удаляет клавиатуру
    }
    public static async Task AddLesson(object sender, ForFunctionEventArgs e){
        await Output.RequestMessageSending(e.update.Message.Chat,
             "Напишите ифнормацию о предмете(название,аудитория,преподаватель)", ButtonsButton: null);

        string Info = await Input.RequestMessageReceiving(e.update.Message.Chat);

        await Output.RequestMessageSending(e.update.Message.Chat,
             "Напишите номер дня недели(1-7)" );
        string DayOfWeek = await Input.RequestMessageReceiving(e.update.Message.Chat);
        //Функция проверки дня недели
        await Output.RequestMessageSending(e.update.Message.Chat,
             "Напишите номер пары(1-7)" );
        string PairNumber = await Input.RequestMessageReceiving(e.update.Message.Chat);
        //Функция проверки номера пары
        await Output.RequestMessageSending(e.update.Message.Chat,
             "Выбирите модификацию пары", new string[][] {new string[]{"all","red","blue"}} );
        string Modification = await Input.RequestMessageReceiving(e.update.Message.Chat);



        DataBaseCore.DataBaseHandler.AddLesson(new Lesson(Info,DayOfWeek,PairNumber,Modification));
        await Output.RequestMessageSending(e.update.Message.Chat, "Предмет добавлен", ButtonsButton: e.update.Message.Chat.Buttons );

    }

}
