using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Bot.MessageExchange
{
    public partial class TelegramMessageExchangeManager
    {
        private class TelegramOutputHandler: IOutputHandler
        {
            public async Task RequestMessageSending(IChat chat, string str)
            {
                await botClient.SendTextMessageAsync(chat.Id,str);
                // throw new NotImplementedException(
                
                // );
                
            }
        }
    }
}
