using Bot.Controllers;
using Bot.Domain.Entities;
using Bot.MessageExchange.Imperative;
namespace Telegram_Bot.Bot.Infrastructure.InputHelper;

public class RegularButtons
{
    private static IOutputHandler Output = BotUpdateController.Output;
    private static IInputHandler Input = BotUpdateController.Input;

    public static async Task GotoMenu(object sender, ForFunctionEventArgs e)
    {
        Buttons menu = new Buttons
        {
            new Button[] { new Button("/start", BotUpdateController.Start) },
            new Button[] { new Button("Расписание на сегодня", BotUpdateController.GetTimeTable) },
            new Button[] { new Button("Настройки", BotUpdateController.Settings) }

        };
        e.update.Message.Chat.ChangeButtons(menu);
        await Output.RequestMessageSending(e.update.Message.Chat, "Меню", menu);
        
    }

}