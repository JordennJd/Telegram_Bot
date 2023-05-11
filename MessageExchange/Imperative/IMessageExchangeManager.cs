namespace Bot.MessageExchange
{
    public class ForFunctionEventArgs : EventArgs
    {
        public IUpdate update { get; }


        public ForFunctionEventArgs(IUpdate update)
        {
            this.update = update;
        }
    }

    public class Button
    {
        public string Text;
        public delegate Task functionForPushButton(object sender, ForFunctionEventArgs e); //делегат функции вызываемой при нажатии клавиши
        public event functionForPushButton puchButtonEvent; //Евент нажатия кнопки
        public string Role;
        public Button(string text, functionForPushButton function, string role = "public")
        {
            puchButtonEvent += function;
            Text = text;
            Role = role;
            }
        public void PushButton(ForFunctionEventArgs e)
        {
            puchButtonEvent?.Invoke(this, e);

        }
    }

    /// <summary>
    /// Инициализирует реализации отправки и получение сообщений.
    /// </summary>    
    public interface IMessageExchangeManager
    {

        public void Initialize(Button[][] meinMenu);
        public IInputHandler GetInputHandler();
        public IOutputHandler GetOutputHandler();
        public void PushButton(IUpdate update);
        public delegate void UpdateEventHungler(IUpdate update);
        public event UpdateEventHungler UpdateEvent;

    }
}
