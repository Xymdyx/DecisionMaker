namespace DMTest;
[TestClass]
public class UnitTestPersonality
{
    private const string DEFAULT_NAME = "Stranger";
    private const string DEFAULT_GREETING = "Welcome!";
    private const string DEFAULT_DEPARTING = "Come back any time!";
    [TestMethod]
    public void testDefInit()
    {
        PER def = new();
        Assert.IsNotNull(def);
    }

    [TestMethod]
    public void testDefInitAfterDelete()
    {
        string relativePerPath = DmCt.DM_RELATIVE_PATH + PSC.DEFAULT_PROF_DIR;
        Directory.CreateDirectory(relativePerPath);
        DmCt.clearADir(relativePerPath);
        Assert.IsFalse(Directory.Exists(relativePerPath));

        PER def = new();
        Assert.IsNotNull(def);
        Assert.IsFalse(def.isDisplayNameCustom());
        Assert.IsFalse(def.isExitCustom());
        Assert.IsFalse(def.isGreetCustom());
    }

    [TestMethod]
    public void testFullInit()
    {
        PER custom = new(DEFAULT_GREETING, DEFAULT_DEPARTING, DEFAULT_NAME);
        Assert.IsNotNull(custom);
        Assert.IsTrue(custom.isDisplayNameCustom());
        Assert.IsTrue(custom.isExitCustom());
        Assert.IsTrue(custom.isDisplayNameCustom());
    }

    [TestMethod]
    public void testInputtedDefault()
    {
        PER inDefault = new(PER.DEFAULT_GREETING, PER.DEFAULT_EXITING, PER.DEFAULT_DISPLAY_NAME);
        Assert.IsNotNull(inDefault);
        
        Assert.IsFalse(inDefault.isDisplayNameCustom());
        Assert.IsFalse(inDefault.isExitCustom());
        Assert.IsFalse(inDefault.isGreetCustom());
    }
}