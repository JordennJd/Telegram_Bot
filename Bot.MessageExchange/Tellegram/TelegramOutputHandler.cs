using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

//Зависимости архитектуры
using Bot.MessageExchange.Imperative;
using Bot.Domain.Interfaces;
using Bot.Domain.Entities;

namespace Bot.MessageExchange.TelegramMesExc;

public partial class TelegramMessageExchangeManager
{
    private class TelegramOutputHandler: IOutputHandler
    {
        public async Task RequestMessageSending(Chat chat, string str, IEnumerable<IEnumerable<string>> buttons = null)
        {
            if(buttons == null)
                await botClient.SendTextMessageAsync(chat.Id,str);

            else
            {
                List<List<KeyboardButton>> telegramButtons= new List<List<KeyboardButton>>();
                foreach(string[] butrow in buttons)
                {
                    List<KeyboardButton> telegramButtonsRow = new List<KeyboardButton>();

                    foreach(string button in butrow)
                        telegramButtonsRow.Add(button);

                    telegramButtons.Add(telegramButtonsRow);

                }
                ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(telegramButtons);
                await botClient.SendTextMessageAsync(chat.Id,str, replyMarkup: replyKeyboardMarkup);
            } 
        }

        public async Task RequestMessageSending(Chat chat, string str, Buttons buttons)
        {
            if(buttons == null)
                await botClient.SendTextMessageAsync(chat.Id,str, replyMarkup: new ReplyKeyboardRemove());

            else
            {
                List<List<KeyboardButton>> telegramButtons= new List<List<KeyboardButton>>();
                foreach(IEnumerable<Button> butrow in buttons)
                {
                    List<KeyboardButton> telegramButtonsRow = new List<KeyboardButton>();

                    foreach(Button button in butrow)
                    {
                        if(button.Text[0]=='/'){
                            break;
                        }
                        telegramButtonsRow.Add(button.Text);
                    }

                    if(telegramButtonsRow.Count>0)
                        telegramButtons.Add(telegramButtonsRow);
                }

                if(telegramButtons.Count>0)
                {
                    ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(telegramButtons);
                    await botClient.SendTextMessageAsync(chat.Id,str, replyMarkup: replyKeyboardMarkup);
                }
            } 
        }
    }
}

