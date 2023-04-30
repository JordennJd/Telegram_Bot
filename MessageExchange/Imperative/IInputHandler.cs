namespace Bot.MessageExchange
{
    /// <summary>
    /// Реализует обработку ввода сообщения пользователем.
    /// </summary>
    public interface IInputHandler
    {
        public Task<string> RequestMessageReceiving(IChat chat);
    }
}
