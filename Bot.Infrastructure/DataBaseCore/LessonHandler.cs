using Bot.Infrastructure.DataBaseCore.InnerRealisation;
using TimeTableCore;
using Bot.Domain.Interfaces;

namespace Bot.Infrastructure.DataBaseCore;
internal sealed partial class DataBaseHandler
{

    public static void AddLesson(ILesson lesson)
    {
        RequestGenerator.INSERT(GetStringForINSERT(lesson), "TimeTable(Info,DayOfWeek,PairNumber,Modification)");
    }
    
    public static bool IsLessonExist(ILesson lesson)
    {
        bool isExist = RequestGenerator.SELECT("Info", "TimeTable",
            $"WHERE DayOfWeek = '{lesson.DayOfWeek}' AND PairNumber = '{lesson.PairNumber}' AND Modification = '{lesson.Modification}'").Count != 0;
        return isExist;
    }

    public static void DeleteLesson(ILesson lesson)
    {
        RequestGenerator.DELETE($"DayOfWeek = '{lesson.DayOfWeek}' AND PairNumber = '{lesson.PairNumber}' " +
            $"AND Modification = '{lesson.Modification}'", "TimeTable");
    }


    public static List<Lesson> GetAllPairs()
    {
        return ConvertToClassLesson(RequestGenerator.SELECT("*", "TimeTable"));
    }
    
    private static List<Lesson> ConvertToClassLesson(List<string[]> Lessons)
    {
        List<Lesson> lessons = new List<Lesson>();
        foreach (var lesson in Lessons)
        {
            lessons.Add(new Lesson(lesson[0], lesson[1], lesson[2], lesson[3]));
        }
        return lessons;
    }
    
    private static string GetStringForINSERT(ILesson lesson)
    {
        return $"'{lesson.Info}','{lesson.DayOfWeek}','{lesson.PairNumber}','{lesson.Modification}'";
    }
}

