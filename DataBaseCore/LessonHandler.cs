using System;
using Bot.MessageExchange;
using DataBaseCore;
using TimeTableCore;

namespace DataBaseCore
{
    internal sealed partial class DataBaseHandler
    {

        public static void AddLesson(ILesson lesson)
        {
            RequestGenerator.INSERT(GetStringForINSERT(lesson), "TimeTable(Info,DayOfWeek,PairNumber,Modification)");
        }

        private static string GetStringForINSERT(ILesson lesson = null)
        {
            if (lesson != null)
                return $"'{lesson.Info}','{lesson.DayOfWeek}','{lesson.PairNumber}','{lesson.Modification}'";

            return null;
        }
        public static bool IsLessonExist(ILesson lesson)
        {
            return RequestGenerator.SELECT("Info", "TimeTable",
                $"WHERE DayOfWeek = '{lesson.DayOfWeek}' AND PairNumber = '{lesson.PairNumber}' AND Modification = '{lesson.Modification}'")[0][0] !="VOID";
        }

        public static void DeleteLesson(ILesson lesson)
        {
            RequestGenerator.DELETE($"DayOfWeek = '{lesson.DayOfWeek}' AND PairNumber = '{lesson.PairNumber}' " +
                $"AND Modification = '{lesson.Modification}'", "TimeTable");
        }


        public static List<Lesson> GetAllPairs()
        {
            List<Lesson> lessons = new List<Lesson>();
            List<string[]> Lessons = RequestGenerator.SELECT("*", "TimeTable");
            foreach (var lesson in Lessons)
            {
                lessons.Add(new Lesson(lesson[0], lesson[1], lesson[2], lesson[3]));
            }
            return lessons;
        }
    }
}

