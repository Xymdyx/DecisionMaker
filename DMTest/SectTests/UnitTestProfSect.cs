namespace DMTest;
[TestClass]
public class UnitTestProfSect
{
    private const string FAIL_DC_PATH = "Comics.txt";
    private const string FAIL_FS_PATH = "failure.txt";
    private const string FAIL_PS_PATh = "screenname.txt";

    [TestMethod]
    public void testDir()
    {
        Assert.IsTrue(PS.checkAndInitDir());
    }

    [TestMethod]
    public void testSaveProfilePart()
    {
        PS ps = new();
        makeCustomProfile(ps);

        Assert.IsFalse(ps.trySaveProfilePart(DSC.DEFAULT_DC_DIRECTORY + FAIL_DC_PATH, DmCt.TEST_DN));
        Assert.IsFalse(ps.trySaveProfilePart(FSC.DEFAULT_FILES_DIR + FAIL_FS_PATH, DmCt.TEST_GREET));
        Assert.IsFalse(ps.trySaveProfilePart(PSC.PROF_GREETING_PATH + FAIL_PS_PATh, DmCt.TEST_DEPART));
    }

    private void makeCustomProfile(PS ps)
    {
        Assert.IsTrue(ps.trySaveProfilePart(PSC.PROF_DISPLAY_NAME_PATH, DmCt.TEST_DN));
        Assert.IsTrue(ps.trySaveProfilePart(PSC.PROF_GREETING_PATH, DmCt.TEST_GREET));
        Assert.IsTrue(ps.trySaveProfilePart(PSC.PROF_EXITING_PATH, DmCt.TEST_DEPART));
    }

    [TestMethod]
    public void testSaveEntireProfile()
    {
        PS ps = new();
        Assert.IsTrue(ps.saveEntireProfile());

        clearDir();
        Assert.IsTrue(ps.saveEntireProfile());

        makeCustomProfile(ps);
        Assert.IsTrue(ps.saveEntireProfile());
    }

    [TestInitialize]
    public void TestInitialize()
    {
        clearDir();
    }

    private void clearDir()
    {
        DmCt.clearADir(PSC.DEFAULT_PROF_DIR);
    }

}