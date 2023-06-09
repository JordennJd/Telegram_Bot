using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Bot.Domain.Interfaces
{
    public interface IUpdate
    {
        public abstract Message Message { get; }
    }
    public abstract class Message
    {
        public virtual User User { get; }

        public virtual Chat Chat { get; }
        public virtual string Text { get; }
    }

    public abstract class Chat
    {
        public virtual long Id { get; }
        public virtual string Title { get; }
    }

    public abstract class User
    {
        public virtual long Id { get; }
        public virtual string FirstName { get; protected set; }

        public virtual void ChangeName(string FirstName)
        {
            this.FirstName = FirstName;
        }
    }
    public interface ILesson
    {
        public string Info { get; }
        public string DayOfWeek { get; }
        public string LessonNumber { get; }
        public string Modification { get; }
    }

}