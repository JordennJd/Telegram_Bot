using Bot.Domain.Interfaces;

namespace Bot.MessageExchange.Imperative
{
    /// <summary>
    /// Реализует обработку ввода сообщения пользователем.
    /// </summary>
    public interface IInputHandler
    {
        public Task<string> RequestMessageReceiving(IChat chat);
    }
}
