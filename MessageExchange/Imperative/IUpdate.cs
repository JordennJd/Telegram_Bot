using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.MessageExchange
{
    public interface IUpdate
    {
        public IMessage Message{get;} 
    }
    public interface IMessage{
        public IUser User{get;}

        public IChat Chat{get;}
        public string Text{get;}

    }
    public interface IChat{
        public long Id{get;}
        public string Title{get;}
        public Button[][] Buttons{get; protected set;}
        public void ChangeButtons(Button[][] buttons){
            Buttons = buttons;
        }

    }
    public interface IUser{
        public long Id{get;}
        public string FirstName{get; protected set;}
        
        public void ChangeName(string FirstName){
            this.FirstName = FirstName;
           
        }

    }
    
}