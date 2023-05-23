using Bot.Controllers;
using Bot.Domain.Entities;
using Bot.MessageExchange.Imperative;
using Bot.Domain.Interfaces;
using Bot.Infrastructure.DataBaseCore;
using TimeTableCore;
namespace Telegram_Bot.Bot.Infrastructure.InputHelper;

class InputHandler
    {
        private static IOutputHandler Output = BotUpdateController.Output;
        private static IInputHandler Input = BotUpdateController.Input;
        public static async Task DoCorrectInputInfo(object sender, ForFunctionEventArgs e,string[] Information)
        {
            await Output.RequestMessageSending(e.update.Message.Chat,
                "Напишите ифнормацию о предмете(название,аудитория,преподаватель)",ButtonsButton: null);
            string Info = await Input.RequestMessageReceiving(e.update.Message.Chat);
            do
            {
                if (!InputCheker.isInputCorrectAddLesson(Info, 1))
                {
                    await Output.RequestMessageSending(e.update.Message.Chat,
                        "Длина информации о предмете должна состовлять больше 3 символов", ButtonsButton: null);
                    Info = await Input.RequestMessageReceiving(e.update.Message.Chat);

                }
            } while (!InputCheker.isInputCorrectAddLesson(Info, 1));

            Information[0] = Info;
        }

        public static async Task DoCorrectInputDayOfWeek(object sender, ForFunctionEventArgs e, string[] Information)
        {
            await Output.RequestMessageSending(e.update.Message.Chat,
                "Напишите номер дня недели(1-7)",
                new string[][] { new string[] { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" } });
            string DayOfWeek = await Input.RequestMessageReceiving(e.update.Message.Chat);
            do
            {
                if (!InputCheker.isInputCorrectAddLesson(DayOfWeek, 2))
                {
                    await Output.RequestMessageSending(e.update.Message.Chat,
                        "Выбирите день недели из предложенных вариантов",new string[][] { new string[] { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" } });
                    DayOfWeek = await Input.RequestMessageReceiving(e.update.Message.Chat);

                }
            } while (!InputCheker.isInputCorrectAddLesson(DayOfWeek,2));

            Information[1] = DayOfWeek;
        }

        public static async Task DoCorrectInputPairNumber(object sender, ForFunctionEventArgs e, string[] Information)
        {
            await Output.RequestMessageSending(e.update.Message.Chat,
                "Напишите номер пары(1-6)", 
                new string[][] { new string[] { "1", "2", "3", "4", "5", "6" } });
            string PairNumber = await Input.RequestMessageReceiving(e.update.Message.Chat);
            do
            {
                if (!InputCheker.isInputCorrectAddLesson(PairNumber, 3))
                {
                    await Output.RequestMessageSending(e.update.Message.Chat,
                        "Выбирите число от 1 до 6",new string[][] { new string[] { "1", "2", "3", "4", "5", "6" } });
                    PairNumber = await Input.RequestMessageReceiving(e.update.Message.Chat);

                }
            } while (!InputCheker.isInputCorrectAddLesson(PairNumber,3));

            Information[2] = PairNumber;
        }

        public static async Task DoCorrectInputModification(object sender, ForFunctionEventArgs e, string[] Information)
        {
            await Output.RequestMessageSending(e.update.Message.Chat,
                "Выбирите модификацию пары", 
                new string[][] { new string[] { "all", "red", "blue" } });
            string Modification = await Input.RequestMessageReceiving(e.update.Message.Chat);
            do
            {
                if (!InputCheker.isInputCorrectAddLesson(Modification, 4))
                {
                    await Output.RequestMessageSending(e.update.Message.Chat,
                        "Выбирите из предложенных выриантов",new string[][] { new string[] { "all", "red", "blue" } });
                    Modification = await Input.RequestMessageReceiving(e.update.Message.Chat);

                }
            } while (!InputCheker.isInputCorrectAddLesson(Modification,4));

            Information[3] = Modification;
        }

        public static async Task ReplaceLesson(object sender, ForFunctionEventArgs e, Lesson lesson)
        {
            await Output.RequestMessageSending(e.update.Message.Chat,
                "Предмет этой парой в этот день уже существует, хотите заменить?",
                new string[][] { new string[] { "Да", "Нет" } });
            string answer = await Input.RequestMessageReceiving(e.update.Message.Chat);
            
            if (answer == "Да")
            {
                DataBaseHandler.DeleteLesson(lesson);
                DataBaseHandler.AddLesson(lesson);
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
    
    file class InputCheker 
    {
        enum Cases
        {
            Info = 1,
            DayOfWeek = 2,
            PairNumber = 3,
            Modification = 4
        }
    
        public static bool isInputCorrectAddLesson(string input, int Case)
        {
            switch (Case)
            {
                case((int)Cases.Info):
                    return input.Length > 3;
            
                case((int)Cases.DayOfWeek):
                    return InfoStorage.daysOfWeek.Contains(input);
            
                case((int)Cases.PairNumber):
                    try
                    {
                        int Input = Convert.ToInt32(input);
                        return (Input >= 1 & Input <= 6);
                    }
                    catch
                    {
                        return false;
                    }
                    
                case((int)Cases.Modification):
                    return input.Contains("all") || input.Contains("red") || input.Contains("blue");
            }

            return false;
        }
        
    }