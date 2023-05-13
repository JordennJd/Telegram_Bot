using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq;

using Bot.Domain.Interfaces;

namespace Bot.Domain.Entities
{
    class Update : IUpdate
    {
        private Message _message;
        public IMessage Message{get{return _message;}}

        public Update(Message message){
            _message = message;
        }
    }
    class Message: IMessage
    {
        private User _user;
        private Chat _chat;
        public string Text{get;}  

        public IUser User{get{return _user;}}
        public IChat Chat{get{return _chat;}}
        
        public Message(User user, Chat chat, string text){
            _user = user;
            _chat = chat;
            Text = text;
        }
        
    }
    class User : IUser
    {
        public long Id{get;}
        public string FirstName{get; set;}
        public User(long id, string firstName){
            Id = id;
            FirstName = firstName;
        }
        
    }
    class Chat:IChat
    {
        public long Id{get;}
        public string Title{get;}
        public IEnumerable<IEnumerable<Button>> Buttons{get;}
        public Chat(long id, string title, IEnumerable<IEnumerable<Button>> buttons){
            Id = id;
            Title = title;
            Buttons =buttons;
        }
        
    }
}