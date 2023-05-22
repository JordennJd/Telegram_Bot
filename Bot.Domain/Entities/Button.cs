using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Bot.Domain.Entities
{
    public delegate Task functionForPushButton(object sender, ForFunctionEventArgs e);
    public class Button{
        public string Text {get;}
        public functionForPushButton functionForPushButton{get;}
        public event functionForPushButton puchButtonEvent; 
        public Button(string text, functionForPushButton function)
        {
            puchButtonEvent += function;
            Text =text;
            functionForPushButton = function;
        }
        public void PushButton(ForFunctionEventArgs e)
        {
            puchButtonEvent?.Invoke(this, e);

        }
    }

    public class Buttons{
        public IEnumerable<IEnumerable<Button>> buttons{get;}
        public Buttons(IEnumerable<IEnumerable<Button>> buttons){
            this.buttons = buttons;
        }
        public Button FindButtonForText(string TextButton){
            foreach(IEnumerable<Button> rowButton in this.buttons){
                foreach(Button button in rowButton){
                    if(button.Text == TextButton)
                        return button;
                }
            }
            return null;
        }
        public Button FindButtonForFunction(string TextFunction){
            foreach(IEnumerable<Button> rowButton in this.buttons){
                foreach(Button button in rowButton){
                    if(button.functionForPushButton.Method.Name == TextFunction)
                        return button;
                }
            }
            return null;
        }
    }
}