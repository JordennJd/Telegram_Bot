using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Bot.Domain.Entities;
using Bot.MessageExchange.Imperative;
using Bot.Domain.Interfaces;
using Bot.Infrastructure.DataBaseCore;
using Bot.Domain.TimeTableCore;


namespace Bot.Controllers
{
    static class BotUpdateController
    {
        private static IOutputHandler Output;
        private static IInputHandler Input;

        public static void Initialize(IMessageExchangeManager messageExchangeManager){

            messageExchangeManager.UpdateEvent+=Update; //����������� ������� ����������

            Output = messageExchangeManager.GetOutputHandler();
            Input = messageExchangeManager.GetInputHandler();
        }
        private static void PushButton(Update update){
            IEnumerable<IEnumerable<Button>> buttons;
            if (update.Message.Chat is Chat somechat) 
            {    
                buttons=somechat.Buttons;
                foreach(Button[] buts in buttons){
                    foreach(Button but in buts){
                    if (but.Text == update.Message.Text)
                        but.PushButton(new ForFunctionEventArgs(update));
                    }
            }
            }
        }

        public static void Update(IUpdate update){
        //��� ���� ���������

            PushButton(new Update(update, new Chat(update.Message.Chat,
                new Button[][]{
                    new Button[]{new Button("/start", Start)},
                    new Button[]{new Button("Новый Предмет", AddLesson)}
                })
            )); //������� ������� ������� ������� � ����������� �� ����������� ������
        }

        public static async Task Start(object sender, ForFunctionEventArgs e){
        await Output.RequestMessageSending(e.update.Message.Chat, "Hellow world!", e.update.Message.Chat.Buttons); 
        // await Output.RequestMessageSending(e.update.Message.Chat,".", new string [][]{ new string[]{"/start"}});
        DataBaseHandler.AddUser(e.update.Message.User);

        }
    
    public static async Task AddLesson(object sender, ForFunctionEventArgs e){
        await Output.RequestMessageSending(e.update.Message.Chat,
             "Напишите ифнормацию о предмете(название,аудитория,преподаватель)", ButtonsButton: null);

        string Info = await Input.RequestMessageReceiving(e.update.Message.Chat);

        await Output.RequestMessageSending(e.update.Message.Chat,
             "Напишите номер дня недели(1-7)",new string[][] {new string[]{"Понедельник","Вторник","Среда","Четверг","Пятница","Суббота"}} );
        string DayOfWeek = await Input.RequestMessageReceiving(e.update.Message.Chat);
        //Функция проверки дня недели
        await Output.RequestMessageSending(e.update.Message.Chat,
             "Напишите номер пары(1-7)",new string[][] {new string[]{"1","2","3","4","5","6","7"} });
        string PairNumber = await Input.RequestMessageReceiving(e.update.Message.Chat);
        //Функция проверки номера пары
        await Output.RequestMessageSending(e.update.Message.Chat,
             "Выбирите модификацию пары", new string[][] {new string[]{"all","red","blue"}} );
        string Modification = await Input.RequestMessageReceiving(e.update.Message.Chat);

        

        DataBaseHandler.AddLesson(new Lesson(Info,DayOfWeek,PairNumber,Modification));
        await Output.RequestMessageSending(e.update.Message.Chat, "Предмет добавлен", ButtonsButton: e.update.Message.Chat.Buttons );

    }

        
    }
}