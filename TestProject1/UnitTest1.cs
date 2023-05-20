using TimeTableCore;
namespace TestProject1;

[TestClass]
public class UnitTest1
{
    private List<Lesson> lessons;
    private string day;
    [TestInitialize]
    public void dsa()
    {
        int Today = (int)DateTime.Now.DayOfWeek;
        string day = InfoStorage.daysOfWeek[Today];
        Lesson Math = new Lesson("Math", day, "5", "all");
        Lesson Philo = new Lesson("Philo", day, "1", "all");
        Lesson Programming = new Lesson("Programming", day, "2", "all");
        lessons = new List<Lesson> { Math, Philo, Programming };
    }
    [TestMethod]
    public void TestMethod1()
    {
        string expected = $"{day}\n\nPhilo 9:30-11:00\nProgramming 11:10–12:40\nMath 16:40–18:10\n";
            
        string actual = TimeTableHandler.GetCorrentTimeTable(lessons);
        Assert.AreEqual(expected,actual);

    }
}