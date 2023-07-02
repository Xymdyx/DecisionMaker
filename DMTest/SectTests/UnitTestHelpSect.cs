namespace DMTest;
[TestClass]
public class UnitTestHelpSect
{
    [TestMethod]
    public void testDir()
    {
        Assert.IsFalse(HS.checkAndInitDir());
    }
}