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

        public ChangesArgsForCoreUpdate(string RoleInUser=null, Buttons ButtonsInChat = null){
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
        public static CoreChat CreateInstance(Chat chat, string directory , List<functionForPushButton> functions){
            Buttons buttons = directoryToButtons(directory, functions);
            if(buttons != null)
                return new CoreChat(chat, new ChangesArgsForCoreUpdate(null, buttons));
            else 
                return null;
            
        }
        public void ChangeButtons(Buttons buttons){
            Buttons = buttons;
            setDirectory(buttons);
        }
        private void setDirectory(Buttons buttons){
            Directory ="";
            foreach(IEnumerable<Button> rowButtons in buttons){
                foreach(Button button in rowButtons){
                    Directory+= $"{button.Text}&{button.functionForPushButton.Method.Name}|";
                }
                Directory+="||";
            }
        }

        private static Buttons directoryToButtons(string directory, List<functionForPushButton> functions){
            string[] rowsStrButtons = directory.Split("||");
            List<List<Button>> buttons = new List<List<Button>>();
            foreach(string rowStrButtons in rowsStrButtons){

                List<Button> rowButtons = new List<Button>();
                string[] strButtons = rowStrButtons.Split("|");

                foreach(string strButton in strButtons){
                    string [] strButtonArgs = strButton.Split("&");
                    if(strButtonArgs.Count()>1){
                        functionForPushButton function = functions.Find((function)=> function.Method.Name== strButtonArgs[1]);
                        if(function!=null)
                            rowButtons.Add(new Button(strButtonArgs[0], function));
                        else return null;

                    }
                    
                }
                if(rowButtons.Count>0)
                    buttons.Add(rowButtons);
            }
            return new Buttons{buttons};
        }

    }
}