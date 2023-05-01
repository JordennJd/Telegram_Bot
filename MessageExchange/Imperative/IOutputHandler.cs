namespace Bot.MessageExchange
{
    /// <summary>
    /// Реализует обработку вывода сообщения пользователем.
    /// </summary>
    public interface IOutputHandler
    {
        /// <summary>
        /// Отправка сообщения пользователью
        /// Ichat chat - чат пользователя,
        /// string messageName - сообщение пользователю,
        /// IEnumerable<IEnumerable<string>> ButtonsString - клавиатура для пользователя
        /// Чтобы удалить клавиатуру задайте в параметр функции ButtonsButton: null
        /// </summary> 
        public Task RequestMessageSending(IChat chat, string messageName, IEnumerable<IEnumerable<string>> ButtonsString = null);

        /// <summary>
        /// Отправка сообщения пользователью
        /// Ichat chat - чат пользователя,
        /// string messageName - сообщение пользователю,
        /// IEnumerable<IEnumerable<Button>> ButtonsButton - клавиатура для пользователя
        /// Чтобы удалить клавиатуру задайте в параметр функции ButtonsButton: null
        /// </summary> 
        public Task RequestMessageSending(IChat chat, string messageName, IEnumerable<IEnumerable<Button>> ButtonsButton);
        
    }
}
