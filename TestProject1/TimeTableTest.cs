using TimeTableCore;
namespace TimeTableTest;

[TestClass]
public class TimeTableTest
{
    private List<Lesson> lessons;
    private string day;
    [TestInitialize]
    public void TimeTableTestInit()
    {
        int Today = (int)DateTime.Now.DayOfWeek;
        day = InfoStorage.daysOfWeek[Today];
        Lesson Math = new Lesson("Math", day, "5", "all");
        Lesson Philo = new Lesson("Philo", day, "1", "all");
        Lesson Programming = new Lesson("Programming", day, "2", "all");
        lessons = new List<Lesson> { Math, Philo, Programming };
    }
    [TestMethod]
    public void TimeTableTest_CorrectOutputOrder()
    {
        string expected = $"{day}\n\nPhilo 9:30-11:00\nProgramming 11:10–12:40\nMath 16:40–18:10\n";
            
        string actual = TimeTableHandler.GetCorrentTimeTable(lessons);
        Assert.AreEqual(expected,actual);

    }
}