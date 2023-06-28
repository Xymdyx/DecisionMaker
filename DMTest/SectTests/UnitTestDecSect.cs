using System.Collections;
namespace DMTest;

/// <summary>
/// dotnet test .\DMTest.csproj
/// </summary>
[TestClass]
public class UnitTestDecSect
{
    internal readonly DC FULL_DC = new("name", "desc", DmUtConsts.TEST_DC_CHOICES_FEW);
    internal readonly DC CHOICELESS_DC = new("Category", "A category with no choices");
    internal readonly DC PASS_DC = new(DmUtConsts.TEST_DC_NAME, DmUtConsts.TEST_DC_DESC, DmUtConsts.TEST_DC_CHOICES_FEW);
    internal readonly DC PETS_DC = new("Pets To Get", "Pets I want to adopt", new() { "Dog", "Cat", "Hamster", "Snake", "Parrot" });
    internal readonly DC GAME_TYPES_DC = new(
        "Game Genres",
        "Which game genre I should play now",
        new() { "Puzzle", "FPS", "RPG", "Platformer", "Simulator", "Sandbox", "Open World", "Mystery", "Visual Novel" }
     );

    internal readonly DC HOBBIES_DC = new(
       "Hobbies",
       "What to do with my free time",
       new() { "Cooking", "Play a video game", "Program something", "Exercise", "Watch YouTube", "Contemplate", "Study a language", "Clean", "Hang out with friend(s)" }
    );

    [TestMethod]
    public void testDir()
    {
        Assert.IsTrue(DS.checkAndInitDir());
    }

    [TestMethod]
    public void testFormatDcPath()
    {
        const string result = DSC.DEFAULT_DC_DIRECTORY + "expected" + TU.TXT;
        Assert.IsTrue(DS.formatDCPath("expected") == result);

        const string same = "\t\n\n\n";
        Assert.IsTrue(DS.formatDCPath(same) == same);

        Assert.IsTrue(DS.formatDCPath(TU.BLANK) == TU.BLANK);
    }

    [TestMethod]
    public void testAlreadyAccepted()
    {
        DS ds = new();
        List<string> fullChoices = FULL_DC.CatChoices;
        const string cand = "candidate";
        Assert.IsFalse(ds.isItemAlreadyAccepted(cand, fullChoices));

        foreach (string c in fullChoices)
        {
            Assert.IsTrue(ds.isItemAlreadyAccepted(c, fullChoices));
            Assert.IsTrue(ds.isItemAlreadyAccepted(c.ToUpper(), fullChoices));
            Assert.IsTrue(ds.isItemAlreadyAccepted(c.ToLower(), fullChoices));
        }

        List<string> none = CHOICELESS_DC.CatChoices;
        Assert.IsFalse(ds.isItemAlreadyAccepted(cand, none));
        foreach(string c in DmUtConsts.TEST_DC_CHOICES)
            Assert.IsFalse(ds.isItemAlreadyAccepted(c, none));

    }

    [TestMethod]
    public void testTryRemove()
    {
        DS ds = new();
        List<string> fullChoices = FULL_DC.CatChoices;
        removeChoiceBlankUb(1, fullChoices);
        removeChoiceBlankLb(fullChoices.Count, fullChoices);

        while (fullChoices.Count > 0)
            Assert.AreNotEqual(ds.tryRemoveChoice(1, fullChoices), TU.BLANK);

        List<string> none = CHOICELESS_DC.CatChoices;
        removeChoiceBlankLb(0, none);
        removeChoiceBlankUb(0, none);
    }

    private void removeChoiceBlankLb(int lb, List<string> opts)
    {
        DS ds = new();
        int i = DmUtConsts.MAX_OPT;
        while (i > lb)
        {
            Assert.AreEqual(ds.tryRemoveChoice(i, opts), TU.BLANK);
            --i;
        }
    }

    private void removeChoiceBlankUb(int ub, List<string> opts)
    {
        DS ds = new();
        int j = DmUtConsts.MIN_OPT;
        while (j < ub)
        {
            Assert.AreEqual(ds.tryRemoveChoice(j, opts), TU.BLANK);
            ++j;
        }
    }

    [TestMethod]
    public void testGetDcActions()
    {
        DS ds = new();
        string[] dcActKeys = ds.getDCActionKeys();
        foreach (string a in dcActKeys)
            Assert.IsTrue(DSC.dcActions.Contains(a));
    }

    [TestMethod]
    public void testGetDcTermVals()
    {
        DS ds = new();
        bool[] dcTermVals = ds.getDCActionTerminateVals();
        int i = 0;
        foreach (DictionaryEntry de in DSC.dcActions)
        {
            Assert.IsTrue( (bool) de.Value! == dcTermVals[i]);
            i++;
        }
    }

    [TestMethod]
    public void testBoundsTuple()
    {
        DS ds = new();

        (int, int) revBounds = ds.returnBoundsTuple(DmUtConsts.MAX_OPT, DmUtConsts.MIN_OPT);
        (int, int) bounds = ds.returnBoundsTuple(DmUtConsts.MIN_OPT, DmUtConsts.MAX_OPT);
        Assert.AreEqual(revBounds, bounds);

        (int, int) expected = (1, 1);
        Assert.AreEqual(expected, ds.returnBoundsTuple(1, 1));

        (int, int) int32Extents = (Int32.MinValue, Int32.MaxValue);
        Assert.AreEqual(ds.returnBoundsTuple(Int32.MaxValue, Int32.MinValue), int32Extents);
        Assert.AreEqual(ds.returnBoundsTuple(Int32.MinValue, Int32.MaxValue), int32Extents);
    }

    [TestMethod]
    public void testDecideForUser()
    {
        clearDir();
        DS ds = new();
        Assert.IsFalse(ds.decideForUser(DC.EmptyDc));

        CHOICELESS_DC.saveFile();
        Assert.IsFalse(ds.decideForUser(CHOICELESS_DC));
        CHOICELESS_DC.deleteFile();
        Assert.IsFalse(ds.decideForUser(CHOICELESS_DC));

        PASS_DC.saveFile();
        Assert.IsFalse(ds.decideForUser(PASS_DC));
    }

    [TestMethod]
    public void testRemoveAllDcsInMap()
    {
        clearDir();
        DS ds = new();
        initDsDcMap(ds);

        ds.removeDcsFromMapNotInDir();
        Assert.IsTrue(ds.DcMap.Count == 0);
    }

    [TestMethod]
    public void testRemoveNoDcsInMap()
    {
        clearDir();
        DS ds = new();
        initDsDcMap(ds);
        int initCount = ds.DcMap.Count;

        ds.saveAllDcsInMap();
        ds.removeDcsFromMapNotInDir();
        int rmCount = ds.DcMap.Count;

        Assert.IsTrue(initCount == rmCount);
    }

    [TestMethod]
    public void testRemoveSomeDcsInMap()
    {
        clearDir();
        DS ds = new();
        initDsDcMap(ds);
        int initCount = ds.DcMap.Count;

        ds.saveAllDcsInMap();
        ds.DcMap[PETS_DC.CatName].deleteFile();
        ds.DcMap[PASS_DC.CatName].deleteFile();
        ds.removeDcsFromMapNotInDir();
        int rmCount = ds.DcMap.Count;

        Assert.IsTrue(initCount > rmCount);
    }

    private void initDsDcMap(DS ds)
    {
        ds.DcMap.Add(CHOICELESS_DC.CatName, CHOICELESS_DC);
        ds.DcMap.Add(PETS_DC.CatName, PETS_DC);
        ds.DcMap.Add(PASS_DC.CatName, PASS_DC);
    }

    [TestMethod]
    public void testAddNoDcsToMapFromDir()
    {
        clearDir();
        DS ds = new();
        initDsDcMap(ds);
        int initCount = ds.DcMap.Count;

        ds.addNewCategoriesToMapFromDir();
        int addCount = ds.DcMap.Count;
        Assert.IsTrue(addCount == initCount);
    }

    [TestMethod]
    public void testAddADcToMapFromDir()
    {
        clearDir();
        DS ds = new();
        initDsDcMap(ds);
        int initCount = ds.DcMap.Count;

        HOBBIES_DC.saveFile();
        ds.addNewCategoriesToMapFromDir();
        int addCount = ds.DcMap.Count;

        Assert.IsTrue(addCount == initCount + 1);
    }

    [TestMethod]
    public void testAddSomeADcsToMapFromDir()
    {
        clearDir();
        DS ds = new();
        initDsDcMap(ds);
        int initCount = ds.DcMap.Count;

        DC[] addDcs = { HOBBIES_DC, GAME_TYPES_DC, FULL_DC };
        foreach(DC d in addDcs)
            d.saveFile();

        ds.addNewCategoriesToMapFromDir();
        int addCount = ds.DcMap.Count;
        Assert.IsTrue(addCount == initCount + addDcs.Length);
    }

    [TestMethod]
    public void testIsChoiceExistingDc()
    {
        clearDir();
        DS ds = new();
        initDsDcMap(ds);
        ds.saveAllDcsInMap();

        int dcCount = ds.DcMap.Count;
        for (int i = MU.MENU_START; i <= dcCount; i++)
            Assert.IsTrue(ds.isChoiceExistingDc(i));

        for (int neg = DmUtConsts.MIN_OPT; neg < MU.MENU_START; neg++)
            Assert.IsFalse(ds.isChoiceExistingDc(neg));

        for (int pos = DmUtConsts.MAX_OPT; pos > dcCount; pos--)
            Assert.IsFalse(ds.isChoiceExistingDc(pos));
    }

    [TestMethod]
    public void testIsDcAction()
    {
        clearDir();
        DS ds = new();
        int dcActCount = DSC.dcActions.Count;
        for (int i = MU.MENU_START; i <= dcActCount; i++)
            Assert.IsTrue(ds.isChoiceDCAction(i));

        for (int neg = DmUtConsts.MIN_OPT; neg < MU.MENU_START; neg++)
            Assert.IsFalse(ds.isChoiceExistingDc(neg));

        for (int pos = DmUtConsts.MAX_OPT; pos > dcActCount; pos--)
            Assert.IsFalse(ds.isChoiceExistingDc(pos));
    }

    [TestMethod]
    public void testGetDcNameFromChoice()
    {
        clearDir();
        DS ds = new();
        initDsDcMap(ds);

        int dcCount = ds.DcMap.Count;
        for (int i = MU.MENU_START; i <= dcCount; i++)
        {
            string expected = ds.DcMap.ElementAt(i - 1).Key;
            string actual = ds.getDCNameFromMenuChoice(i);
            Assert.IsTrue(expected == actual);
        }

        for (int neg = DmUtConsts.MIN_OPT; neg < MU.MENU_START; neg++)
        {
            string actual = ds.getDCNameFromMenuChoice(neg);
            Assert.IsTrue(actual == TU.BLANK);
        }

        for (int pos = DmUtConsts.MAX_OPT; pos > dcCount; pos--)
        {
            string actual = ds.getDCNameFromMenuChoice(pos);
            Assert.IsTrue(actual == TU.BLANK);
        }
    }


    [TestMethod]
    public void testGetDcItemFromChoice()
    {
        clearDir();
        DS ds = new();
        initDsDcMap(ds);

        int dcCount = ds.DcMap.Count;
        for (int i = MU.MENU_START; i <= dcCount; i++)
        {
            DC expected = ds.DcMap.ElementAt(i - 1).Value;
            DC actual = ds.getDCFromMenuChoice(i);
            Assert.IsTrue(expected == actual);
        }

        for (int neg = DmUtConsts.MIN_OPT; neg < MU.MENU_START; neg++)
        {
            DC actual = ds.getDCFromMenuChoice(neg);
            Assert.IsTrue(actual == DC.EmptyDc);
        }

        for (int pos = DmUtConsts.MAX_OPT; pos > dcCount; pos--)
        {
            DC actual = ds.getDCFromMenuChoice(pos);
            Assert.IsTrue(actual == DC.EmptyDc);
        }    }

    private void clearDir()
    {
        try
        {
            Directory.Delete(DSC.DEFAULT_DC_DIRECTORY, true);
        }
        catch(Exception e)
        {
            DmUtConsts.logPreProcessingFail(e);
        }
    }
}