namespace Bot.MessageExchange
{
    /// <summary>
    /// Реализует обработку вывода сообщения пользователем.
    /// </summary>
    public interface IOutputHandler
    {
        public Task RequestMessageSending(IChat chat, string messageName);
    }
}
