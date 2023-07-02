namespace DMTest;

[TestClass]
public class UnitTestFileSect
{
    private readonly string[] FAIL_PATHS =
    {
        @"..\DecisionsFun\funny.txt",
        DmUtConsts.DM_RELATIVE_PATH + @"FunnyLand\DNE\doesntexist.txt",
        DmUtConsts.DM_RELATIVE_PATH + @"\Help\README\reads.txt"
     };

    private const string PASS_DIR = @".\Pass\";
    private readonly string[] PASS_PATHS =
    {
        PASS_DIR + @".\passDC.txt",
        PASS_DIR + @".\greeting.txt",
        PASS_DIR + @".\departing.txt",
        PASS_DIR + @".\displayname.txt"
    };

    private readonly string[] PASS_DC_CONTENTS = { "passDC", "A sample dc for unit tests", "Passing", "FunPass", "FunFunFun" };
    private const string PASS_TEXT = "PASS";

    [TestMethod]
    public void testDir()
    {
        clearDir();
        Assert.IsTrue(FS.checkAndInitDir());
    }

    [TestMethod]
    public void testViewNonExistentFile()
    {
        if (ensureFailFilesDontExist())
        {
            for (int i = 0; i < FAIL_PATHS.Length; i++)
                Assert.IsTrue(FS.viewFileContents(FAIL_PATHS[i]) == TU.BLANK);
        }
        else
            Assert.IsTrue(false);
    }

    [TestMethod]
    public void testViewPassFiles()
    {
        if(ensurePassFilesExist())
        {
            for (int i = 0; i < PASS_PATHS.Length; i++)
                Assert.IsTrue(FS.viewFileContents(PASS_PATHS[i]) != TU.BLANK);
            deletePassDir();
        }
        else
            Assert.IsTrue(false);
    }

    [TestMethod]
    public void testDeleteNonExistentFiles()
    {
        if (ensureFailFilesDontExist())
        {
            for (int i = 0; i < FAIL_PATHS.Length; i++)
                Assert.IsTrue(FS.deleteManageableFile(FAIL_PATHS[i]));
        }
        else
            Assert.IsTrue(false);
    }

    [TestMethod]
    public void testDeleteExistingFiles()
    {
        if (ensurePassFilesExist())
        {
            for (int i = 0; i < PASS_PATHS.Length; i++)
                Assert.IsTrue(FS.deleteManageableFile(PASS_PATHS[i]));
            deletePassDir();
        }
        else
            Assert.IsTrue(false);
    }

    [TestMethod]
    public void testSaveFilesOnExit()
    {
        FS fs = new(new(), new());
        Assert.IsTrue(fs.saveFilesBeforeExit());
    }

    private bool ensureFailFilesDontExist()
    {
        try
        {
            for (int i = 0; i < FAIL_PATHS.Length; i++)
            {
                if(File.Exists(FAIL_PATHS[i]))
                    File.Delete(FAIL_PATHS[i]);
            }
        }
        catch(Exception e)
        {
            DmUtConsts.logPreProcessingFail(e);
            return false;
        }
        return true;
    }

    private bool ensurePassFilesExist()
    {
        try
        {
            Directory.CreateDirectory(PASS_DIR);
            int dcIdx = PASS_PATHS.Length - 1;
            for (int i = 0; i < dcIdx; i++)
                File.WriteAllText(PASS_PATHS[i], PASS_TEXT);

            File.WriteAllLines(PASS_PATHS[dcIdx], PASS_DC_CONTENTS);
        }
        catch(Exception e)
        {
            Console.WriteLine(DmUtConsts.UT_INFO_HEADER + $" failed to ensure all pass files exist...\n{e.Message}\n");
            return false;
        }
        return true;
    }

    [TestInitialize]
    public void TestInitialize()
    {
        DmUtConsts.clearADir(PASS_DIR);
        deletePassDir();
    }

    private void clearDir()
    {
        DmUtConsts.clearADir(FSC.DEFAULT_FILES_DIR);
    }

    private void deletePassDir()
    {
        DmUtConsts.clearADir(PASS_DIR);
    }
}