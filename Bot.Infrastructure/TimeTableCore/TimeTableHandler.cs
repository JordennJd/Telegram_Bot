using Bot.Infrastructure.DataBaseCore;

namespace TimeTableCore;

class TimeTableHandler
{
    private static string[] daysOfWeek = { "", "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
    private static string[] LessonsTime = { "","9:30 - 11:00 ", "11:10–12:40 ", "13:00–14:30 ",
    "15:00–16:30 ", "16:40–18:10 ", "18:30–20:00" };

    public static string GetCorrentTimeTable()
    {
        List<Lesson> Lessons = DataBaseHandler.GetAllPairs();
        return BuildTimeTable(Lessons);
    }

    private static string BuildTimeTable(List<Lesson> Lessons)
    {
        string CorrentTimeTable = daysOfWeek[(int)DateTime.Now.DayOfWeek] + "\n\n";

        Lessons = GetSortedLessonList(Lessons);
        
        foreach (Lesson lesson in Lessons)
        {
            if(lesson!=null) CorrentTimeTable += $"{lesson.Info} {LessonsTime[Convert.ToInt32(lesson.LessonNumber)]}({lesson.LessonNumber} Пара) \n";
        }
        return CorrentTimeTable;

    }

    private static List<Lesson> GetSortedLessonList(List<Lesson> lessons)
    {
        List<Lesson> sortedLessons = new List<Lesson>(6){null,null,null,null,null,null};

        foreach (var lesson in lessons)
        {
            
            if (isSuitablePlaceForPair(lesson) && lesson.DayOfWeek == daysOfWeek[(int)DateTime.Now.DayOfWeek])
            {
                sortedLessons.Insert(Convert.ToInt32(lesson.LessonNumber),lesson);
            }
        }

        return sortedLessons;
    }

    private static bool isSuitablePlaceForPair(Lesson Lesson)
    {
        for (int i = 1; i < 8; i++)
            if (Lesson.LessonNumber == i.ToString()) return true;

        return false;
    }
    
}