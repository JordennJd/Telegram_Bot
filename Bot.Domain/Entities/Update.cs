using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq;

using Bot.Domain.Interfaces;

namespace Bot.Domain.Entities
{
    public class ChangesArgsForCoreUpdate{
        public string RoleInUser{get;}
        public Buttons ButtonsInChat {get;}

        public ChangesArgsForCoreUpdate(string RoleInUser, Buttons ButtonsInChat){
            this.RoleInUser= RoleInUser;
            this.ButtonsInChat = ButtonsInChat;
        }
    }
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
        public CoreUpdate(IUpdate update, ChangesArgsForCoreUpdate args)
        {
            Message = new CoreMessage(update.Message, args);
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
        public CoreMessage(Message message, ChangesArgsForCoreUpdate args )
        {
            User = new CoreUser(message.User, args);
            Chat = new CoreChat(message.Chat, args);
            Text = message.Text;
        }

    }
    public class CoreUser : User
    {
        public override long Id { get; }
        public override string FirstName { get; protected set;}
        public string Role{get; private set;}
        public CoreUser(long id, string firstName, string role)
        {
            Id = id;
            FirstName = firstName;
            Role = role;
        }
        public CoreUser(User user, ChangesArgsForCoreUpdate args)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            Role=args.RoleInUser;
        }
        public void ChangeRole(string newRole){
            Role = newRole;
        }
    }
    public class CoreChat : Chat
    {
        public override long Id { get; }
        public override string Title { get; }
        public Buttons Buttons{get; private set;}

        public string Directory {get; private set;}
        public CoreChat(long id, string title, Buttons buttons)
        {
            Id = id;
            Title = title;
            Buttons = buttons;
            Directory ="";
            setDirectory(buttons);
        }
        public CoreChat(Chat chat, ChangesArgsForCoreUpdate args)
        {
            Id = chat.Id;
            Title = chat.Title;
            Buttons = args.ButtonsInChat;
            Directory ="";
            setDirectory(args.ButtonsInChat);
        }
        public CoreChat(Chat chat, string directory , Buttons functions){
            Id = chat.Id;
            Title = chat.Title;
            Directory=directory;
            ChangeButtons(directory, functions);
        }
        public void ChangeButtons(Buttons buttons){
            Buttons = buttons;
            setDirectory(buttons);
        }
        private void setDirectory(Buttons buttons){
            Directory ="";
            foreach(IEnumerable<Button> rowButtons in Buttons.buttons){
                foreach(Button button in rowButtons){
                    Directory+= $"{button.Text}&{button.functionForPushButton.Method.Name}|";
                }
                Directory+="||";
            }
        }

        private bool ChangeButtons(string directory, Buttons functions){
            string[] rowsStrButtons = directory.Split("||");
            List<List<Button>> buttons = new List<List<Button>>();
            foreach(string rowStrButtons in rowsStrButtons){

                List<Button> rowButtons = new List<Button>();
                string[] strButtons = rowStrButtons.Split("|");

                foreach(string strButton in strButtons){
                    string [] strButtonArgs = strButton.Split("&");
                    if(strButtonArgs.Count()>1){
                        Button button = functions.FindButtonForFunction(strButtonArgs[1]);
                        if(button!=null)
                            rowButtons.Add(new Button(strButtonArgs[0], button.functionForPushButton));
                        else return false;
                    }
                    
                }
                if(rowButtons.Count>0)
                    buttons.Add(rowButtons);
            }
            Buttons = new Buttons(buttons);
            return true;
        }

    }
}