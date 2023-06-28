namespace DMTest;

/// <summary>
/// dotnet test .\DMTest.csproj 
/// </summary>
[TestClass]
public class UnitTestMenuUtils
{
    [TestMethod]
    public void testIsExit()
    {
       Assert.IsTrue(MU.isChoiceMenuExit(MU.EXIT_CODE));

        for (int i = DmCt.MAX_OPT; i > MU.EXIT_CODE; i--)
            Assert.IsFalse(MU.isChoiceMenuExit(i));

        for (int i = DmCt.MIN_OPT; i < MU.EXIT_CODE; i++)
            Assert.IsFalse(MU.isChoiceMenuExit(i));           
    }

    [TestMethod]
    public void testIsBinaryChoice()
    {
       Assert.IsTrue(MU.isBinaryChoice(MU.YES_CODE));
       Assert.IsTrue(MU.isBinaryChoice(MU.NO_CODE));
       Assert.IsTrue(MU.isBinaryChoice(MU.EXIT_CODE));

        for (int i = DmCt.MAX_OPT; i > MU.NO_CODE; i--)
            Assert.IsFalse(MU.isBinaryChoice(i));

        for (int i = DmCt.MIN_OPT; i < MU.EXIT_CODE; i++)
            Assert.IsFalse(MU.isBinaryChoice(i));
    }

    [TestMethod]
    public void testIsBinaryYes()
    {
       Assert.IsTrue(MU.isChoiceYes(MU.YES_CODE));

       Assert.IsFalse(MU.isChoiceYes(MU.NO_CODE));
       Assert.IsFalse(MU.isChoiceYes(MU.EXIT_CODE));
        for (int i = DmCt.MAX_OPT; i > MU.NO_CODE; i--)
            Assert.IsFalse(MU.isChoiceYes(i));

        for (int i = DmCt.MIN_OPT; i < MU.EXIT_CODE; i++)
            Assert.IsFalse(MU.isChoiceYes(i));        
    }

    [TestMethod]
    public void testIsBinaryNo()
    {
       Assert.IsTrue(MU.isChoiceNo(MU.NO_CODE));

       Assert.IsFalse(MU.isChoiceNo(MU.YES_CODE));
       Assert.IsFalse(MU.isChoiceNo(MU.EXIT_CODE));
        for (int i = DmCt.MAX_OPT; i > MU.NO_CODE; i--)
            Assert.IsFalse(MU.isChoiceNo(i));

        for (int i = DmCt.MIN_OPT; i < MU.EXIT_CODE; i++)
            Assert.IsFalse(MU.isChoiceNo(i));            
    }

    [TestMethod]
    public void testisBinaryInputExit()
    {
       Assert.IsTrue(MU.isBinaryInputExit(MU.YES_CODE));
       Assert.IsTrue(MU.isBinaryInputExit(MU.EXIT_CODE));

       Assert.IsFalse(MU.isBinaryInputExit(MU.NO_CODE));
        for (int i = DmCt.MAX_OPT; i > MU.NO_CODE; i--)
            Assert.IsFalse(MU.isBinaryInputExit(i));

        for (int i = DmCt.MIN_OPT; i < MU.EXIT_CODE; i++)
            Assert.IsFalse(MU.isBinaryInputExit(i));
    }
}