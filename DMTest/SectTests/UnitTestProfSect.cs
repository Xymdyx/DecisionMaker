namespace DMTest;
/// <summary>
/// dotnet test .\DMTest.csproj 
/// </summary>
[TestClass]
public class UnitTestProfSect
{
    private const string FAIL_DC_PATH = "Comics.txt";
    private const string FAIL_FS_PATH = "failure.txt";
    private const string FAIL_PS_PATh = "screenname.txt";

    [TestMethod]
    public void testDir()
    {
        clearDir();
        Assert.IsTrue(PS.checkAndInitDir());
    }

    [TestMethod]
    public void testSaveProfilePart()
    {
        PS ps = new();
        makeCustomProfile(ps);

        Assert.IsFalse(ps.trySaveProfilePart(DSC.DEFAULT_DC_DIRECTORY + FAIL_DC_PATH, DmUtConsts.TEST_DN));
        Assert.IsFalse(ps.trySaveProfilePart(FSC.DEFAULT_FILES_DIR + FAIL_FS_PATH, DmUtConsts.TEST_GREET));
        Assert.IsFalse(ps.trySaveProfilePart(PSC.PROFILE_GREETING_PATH + FAIL_PS_PATh, DmUtConsts.TEST_DEPART));

        Directory.Delete(PSC.DEFAULT_PROFILE_DIR, true);
        clearDir();
    }

    private void makeCustomProfile(PS ps)
    {
        Assert.IsTrue(ps.trySaveProfilePart(PSC.PROFILE_DISPLAY_NAME_PATH, DmUtConsts.TEST_DN));
        Assert.IsTrue(ps.trySaveProfilePart(PSC.PROFILE_GREETING_PATH, DmUtConsts.TEST_GREET));
        Assert.IsTrue(ps.trySaveProfilePart(PSC.PROFILE_EXITING_PATH, DmUtConsts.TEST_DEPART));
    }

    [TestMethod]
    public void testSaveProfile()
    {
        PS ps = new();
        Assert.IsTrue(ps.saveEntireProfile());
        
        clearDir();
        Assert.IsTrue(ps.saveEntireProfile());

        makeCustomProfile(ps);
        Assert.IsTrue(ps.saveEntireProfile());
    }

    private void clearDir()
    {
        try
        {
            Directory.Delete(PSC.DEFAULT_PROFILE_DIR, true);
        }
        catch(Exception e)
        {
            DmUtConsts.logPreProcessingFail(e);
        }        
    }

}