using System.Runtime.CompilerServices;
using Bot.Domain.Interfaces;
using Bot.Infrastructure.DataBaseCore;

[assembly: InternalsVisibleTo("TestProject1")]
namespace TimeTableCore;

class InfoStorage
{
    public static readonly string[] daysOfWeek = { "", "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
    public static readonly string[] LessonsTime = { "","9:30-11:00", "11:10–12:40", "13:00–14:30",
        "15:00–16:30", "16:40–18:10", "18:30–20:00" };
}

class TimeTableHandler
{
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
        return StringForUOutput;

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

class InputCheker 
{
    
    public static bool isInputCorrect(string input, int Case)
    {
        switch (Case)
        {
            case(1):
                return input.Length > 3;
            
            case(2):
                return InfoStorage.daysOfWeek.Contains(input);
            
            case(3):
                try
                {
                    int Input = Convert.ToInt32(input);
                    return (Input >= 1 & Input <= 6);
                }
                catch
                {
                    return false;
                }
                    
            case(4):
                return input.Contains("all") || input.Contains("red") || input.Contains("blue");
        }

        return false;
    }
}
                


