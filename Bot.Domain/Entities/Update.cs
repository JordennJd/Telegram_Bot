using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq;

using Bot.Domain.Interfaces;

namespace Bot.Domain.Entities
{
    public abstract class Update : IUpdate
    {
        public virtual Message Message { get; }
    }

    public class CoreUpdate : Update
    {
        public override CoreMessage Message { get; }

        public CoreUpdate(CoreMessage message)
        {
            Message = message;

        }
        public CoreUpdate(IUpdate update, Buttons buttons)
        {
            Message = new CoreMessage(update.Message, buttons);
        }
    }
    public class CoreMessage : Message
    {
        public override string Text { get; }

        public override CoreUser User { get; }
        public override CoreChat Chat { get; }

        public CoreMessage(CoreUser user, CoreChat chat, string text)
        {
            User = user;
            Chat = chat;
            Text = text;
        }
        public CoreMessage(Message message, Buttons buttons)
        {
            User = new CoreUser(message.User);
            Chat = new CoreChat(message.Chat, buttons);
            Text = message.Text;
        }

    }
    public class CoreUser : User
    {
        public override long Id { get; }
        public override string FirstName { get; set; }
        public CoreUser(long id, string firstName)
        {
            Id = id;
            FirstName = firstName;
        }
        public CoreUser(User user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
        }
    }
    public class CoreChat : Chat
    {
        public override long Id { get; }
        public override string Title { get; }
        public Buttons Buttons;

        public string Directory {get;}
        public CoreChat(long id, string title, Buttons buttons)
        {
            Id = id;
            Title = title;
            Buttons = buttons;
            Directory ="";
            foreach(IEnumerable<Button> rowButtons in Buttons){
                foreach(Button button in rowButtons){
                    Directory+= $"{button.Text}&&{button.functionForPushButton.Method.Name}||";
                }
                Directory+="||";
            }
        }
        public CoreChat(Chat chat, Buttons buttons)
        {
            Id = chat.Id;
            Title = chat.Title;
            Buttons = buttons;
            Directory ="";
            foreach(IEnumerable<Button> rowButtons in Buttons){
                foreach(Button button in rowButtons){
                    Directory+= $"{button.Text}&{button.functionForPushButton.Method.Name}|";
                }
                Directory+="||";
            }
        }
        public CoreChat(Chat chat, string directory , Buttons functions){
            Id = chat.Id;
            Title = chat.Title;
            Directory=directory;
            ChangeButtons(directory, functions);
        }
        public bool ChangeButtons(string directory, Buttons functions){
            Buttons buttons = new Buttons(); 
            string[] rowsStrButtons = directory.Split("||");

            foreach(string rowStrButtons in rowsStrButtons){

                List<Button> rowButtons = new List<Button>();
                string[] strButtons = rowStrButtons.Split("|");

                foreach(string strButton in strButtons){
                    string [] strButtonArgs = strButton.Split("&");
                    
                    Button button = functions.FindButtonForFunction(strButtonArgs[1]);
                    if(button!=null)
                        rowButtons.Add(new Button(strButtonArgs[0], button.functionForPushButton));
                    else return false;
                }
                buttons.Add(rowButtons);
            }
            Buttons = buttons;
            return true;
        }

    }
}