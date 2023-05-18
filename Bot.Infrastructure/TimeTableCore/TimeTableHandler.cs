using Bot.Infrastructure.DataBaseCore;
using Bot.Domain.Interfaces;

namespace TimeTableCore;

class TimeTableHandler
{
    private static string[] daysOfWeek = { "", "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
    private static string[] pairsTime = { "","9:30 - 11:00 ", "11:10–12:40 ", "13:00–14:30 ",
    "15:00–16:30 ", "16:40–18:10 ", "18:30–20:00" };

    public static string GetCorrentTimeTable()
    {
        int Today = (int)DateTime.Now.DayOfWeek; //Получение текущей даты
        List<Lesson> PairsInfo = DataBaseHandler.GetAllPairs();
        
        string CorrentTimeTable = daysOfWeek[Today] + "\n\n";

        foreach (var Pair in PairsInfo)
        {
            if (isSuitablePlaceForPair(Pair) && Pair.DayOfWeek == daysOfWeek[Today])
            {
                CorrentTimeTable += $"{Pair.Info} {pairsTime[Convert.ToInt32(Pair.PairNumber)]} \n";
            }
        }
        return CorrentTimeTable;
    }
    private static bool isSuitablePlaceForPair(ILesson Pair)
    {
        for (int i = 1; i < 7; i++)
            if (Pair.PairNumber == i.ToString()) return true;

        return false;
    }
    
    public static bool isInputCorrect(string input, int Case)
    {
        switch (Case)
        {
            case(1):
                return input.Length > 3;
            
            case(2):
                return daysOfWeek.Contains(input);
            
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
                


