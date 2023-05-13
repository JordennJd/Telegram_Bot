using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Bot.Domain.Entities
{
    
    public class Button{
        public string Text;
        public delegate Task functionForPushButton(object sender, ForFunctionEventArgs e); //������� ������� ���������� ��� ������� �������
        public event functionForPushButton puchButtonEvent; //����� ������� ������
        public Button(string text, functionForPushButton function)
        {
            puchButtonEvent += function;
            Text =text;
        }
        public void PushButton(ForFunctionEventArgs e)
        {
            puchButtonEvent?.Invoke(this, e);

        }
    }
}