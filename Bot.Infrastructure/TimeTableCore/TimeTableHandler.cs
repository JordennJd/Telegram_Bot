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
        int Today = (int)DateTime.Now.DayOfWeek+1; //Получение текущей даты
        List<Lesson> PairsInfo = Bot.Infrastructure.DataBaseCore.DataBaseHandler.GetAllPairs();
        string CorrentTimeTable = daysOfWeek[Today];
        CorrentTimeTable += "\n\n";
        foreach (var Pair in PairsInfo)
        {
            if (Pair.DayOfWeek == daysOfWeek[Today])
            {
                for (int i = 1; i < 8; i++)
                {
                    if (Pair.PairNumber == i.ToString())
                        CorrentTimeTable += $"{Pair.Info} {pairsTime[i]} \n";
                }
            }
        }
        return CorrentTimeTable;
    }
}