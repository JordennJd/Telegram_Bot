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
        public CoreUpdate(IUpdate update, IEnumerable<IEnumerable<Button>> buttons)
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
        public CoreMessage(Message message, IEnumerable<IEnumerable<Button>> buttons)
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
        public IEnumerable<IEnumerable<Button>> Buttons;
        public CoreChat(long id, string title, IEnumerable<IEnumerable<Button>> buttons)
        {
            Id = id;
            Title = title;
            Buttons = buttons;
        }
        public CoreChat(Chat chat, IEnumerable<IEnumerable<Button>> buttons)
        {
            Id = chat.Id;
            Title = chat.Title;
            Buttons = buttons;
        }

    }
}