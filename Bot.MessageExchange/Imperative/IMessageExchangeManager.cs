using Bot.Domain.Interfaces;

namespace Bot.MessageExchange.Imperative
{
    /// <summary>
    /// Инициализирует реализации отправки и получение сообщений.
    /// </summary>    
    public interface IMessageExchangeManager
    {
        
        public void Initialize();
        public IInputHandler GetInputHandler();
        public IOutputHandler GetOutputHandler();
        public delegate void UpdateEventHungler(IUpdate update);
        public event UpdateEventHungler UpdateEvent;
        
    }
}
