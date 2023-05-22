using System;
using Bot.Domain.Interfaces;

namespace TimeTableCore
{
    class Lesson : ILesson
    {
        public string Info { get; }
        public string DayOfWeek { get; }
        public string LessonNumber { get; }
        public string Modification { get; }

        public Lesson(string info, string dayOfWeek, string lessonNumber, string modification)
        {
            Info = info;
            DayOfWeek = dayOfWeek;
            LessonNumber = lessonNumber;
            Modification = modification;
        }
    }
}

