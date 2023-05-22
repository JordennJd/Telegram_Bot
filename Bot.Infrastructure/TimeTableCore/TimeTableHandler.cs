using System.Runtime.CompilerServices;
using Bot.Domain.Interfaces;

[assembly: InternalsVisibleTo("TestProject1")]
namespace TimeTableCore;

class InfoStorage
{
    public static string[] daysOfWeek = { "", "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
    public static string[] pairsTime = { "","9:30-11:00", "11:10–12:40", "13:00–14:30",
        "15:00–16:30", "16:40–18:10", "18:30–20:00" };
}

class TimeTableHandler
{


    public static string GetCorrentTimeTable(List<Lesson> lessons)
    {
        int Today = (int)DateTime.Now.DayOfWeek; //Получение текущей даты
        List<Lesson> PairsInfo = lessons;
        
        string StringForUOutput = InfoStorage.daysOfWeek[Today] + "\n\n";
        Lesson[] CorrentTimeTable = new Lesson[6];
        foreach (var Pair in PairsInfo)
        {
            if (isSuitablePlaceForPair(Pair) && Pair.DayOfWeek == InfoStorage.daysOfWeek[Today])
            {
                CorrentTimeTable[Convert.ToInt32(Pair.PairNumber)] = Pair;
            }
        }
        foreach (Lesson Pair in CorrentTimeTable)
        {
            if(Pair!=null) 
                StringForUOutput += $"{Pair.Info} {InfoStorage.pairsTime[Convert.ToInt32(Pair.PairNumber)]}\n";
            
        }
        return StringForUOutput;

    }
    private static bool isSuitablePlaceForPair(ILesson Pair)
    {
        for (int i = 1; i < 7; i++)
            if (Pair.PairNumber == i.ToString()) return true;

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
                


