using Bot.Domain.Interfaces;
using Bot.Domain.Entities;

namespace Bot.MessageExchange.Imperative
{
    /// <summary>
    /// Реализует обработку вывода сообщения пользователем.
    /// </summary>
    public interface IOutputHandler
    {
        /// <summary>
        /// IEnumerable<IEnumerable<string>> ButtonsString - клавиатура для пользователя
        /// Чтобы удалить клавиатуру задайте в параметр функции ButtonsButton: null
        /// </summary> 
        public Task RequestMessageSending(Chat chat, string messageName, IEnumerable<IEnumerable<string>> ButtonsString = null);
        public Task RequestMessageSending(Chat chat, string messageName, Buttons ButtonsButton);
        
    }
}
