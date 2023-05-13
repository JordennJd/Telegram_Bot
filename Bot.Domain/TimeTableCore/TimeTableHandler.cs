namespace Bot.Domain.TimeTableCore;
class Lesson : ILesson
{
        public string Info{get ;}
        public string DayOfWeek{get;}
        public string PairNumber{get;}
        public string Modification{get;}
        
        public Lesson(string info, string dayOfWeek, string pairNumber, string modification){
            Info = info;
            DayOfWeek = dayOfWeek;
            PairNumber = pairNumber;
            Modification = modification;
        }
}