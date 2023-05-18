using Bot.Infrastructure.DataBaseCore;
using System;
using Bot.Domain.Interfaces;

namespace TimeTableCore;

class TimeTableHandler
{
    private static string[] daysOfWeek = { "", "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
    private static string[] pairsTime = { "","9:30 - 11:00 ", "11:10–12:40 ", "13:00–14:30 ",
    "15:00–16:30 ", "16:40–18:10 ", "18:30–20:00" };

    public static string GetCorrentTimeTable()
    {
        List<Lesson> PairsInfo = Bot.Infrastructure.DataBaseCore.DataBaseHandler.GetAllPairs();
        return BuildTimeTable(PairsInfo);
    }

    private static string BuildTimeTable(List<Lesson> PairsInfo)
    {
        int Today = (int)DateTime.Now.DayOfWeek; //Получение текущей даты
        string CorrentTimeTable = daysOfWeek[Today] + "\n\n";
        Lesson[] SortedPairs = new Lesson[7];
        
        foreach (var Pair in PairsInfo)
        {
            if (isSuitablePlaceForPair(Pair) && Pair.DayOfWeek == daysOfWeek[Today])
            {
                SortedPairs[Convert.ToInt32(Pair.PairNumber)] = Pair;
            }
        }
        
        foreach (Lesson pair in SortedPairs)
        {
            if(pair!=null) CorrentTimeTable += $"{pair.Info} {pairsTime[Convert.ToInt32(pair.PairNumber)]} \n";
        }
        return CorrentTimeTable;

    }

    private static bool isSuitablePlaceForPair(Lesson Pair)
    {
        for (int i = 1; i < 8; i++)
            if (Pair.PairNumber == i.ToString()) return true;

        return false;
    }
    
}