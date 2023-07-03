using System.Collections;
namespace DMTest;

[TestClass]
public class UnitTestDecSect
{
    [TestMethod]
    public void testDir()
    {
        Assert.IsTrue(DS.checkAndInitDir());
    }

    [TestMethod]
    public void testformatDcPath()
    {
        const string result = DSC.DEFAULT_DC_DIRECTORY + "expected" + TU.TXT;
        Assert.IsTrue(DS.formatDcPath("expected") == result);

        const string same = "\t\n\n\n";
        Assert.IsTrue(DS.formatDcPath(same) == same);

        Assert.IsTrue(DS.formatDcPath(TU.BLANK) == TU.BLANK);
    }

    [TestMethod]
    public void testAlreadyAccepted()
    {
        DS ds = new();
        List<string> fullChoices = DmCt.FULL_DC.CatChoices;
        const string cand = "candidate";
        Assert.IsFalse(ds.isItemAlreadyAccepted(cand, fullChoices));

        foreach (string c in fullChoices)
        {
            Assert.IsTrue(ds.isItemAlreadyAccepted(c, fullChoices));
            Assert.IsTrue(ds.isItemAlreadyAccepted(c.ToUpper(), fullChoices));
            Assert.IsTrue(ds.isItemAlreadyAccepted(c.ToLower(), fullChoices));
        }

        List<string> none = DmCt.CHOICELESS_DC.CatChoices;
        Assert.IsFalse(ds.isItemAlreadyAccepted(cand, none));
        foreach(string c in DmCt.TEST_DC_CHOICES)
            Assert.IsFalse(ds.isItemAlreadyAccepted(c, none));
    }

    [TestMethod]
    public void testBoundsTuple()
    {
        DS ds = new();

        (int, int) revBounds = ds.returnBoundsTuple(DmCt.MAX_OPT, DmCt.MIN_OPT);
        (int, int) bounds = ds.returnBoundsTuple(DmCt.MIN_OPT, DmCt.MAX_OPT);
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
        DS ds = giveDsWithoutDcs();
        Assert.IsFalse(ds.decideForUser(DC.EmptyDc));

        DmCt.CHOICELESS_DC.saveFile();
        Assert.IsFalse(ds.decideForUser(DmCt.CHOICELESS_DC));
        DmCt.CHOICELESS_DC.deleteFile();
        Assert.IsFalse(ds.decideForUser(DmCt.CHOICELESS_DC));

        DmCt.PASS_DC.saveFile();
        Assert.IsTrue(ds.decideForUser(DmCt.PASS_DC));
    }

    [TestMethod]
    public void testRemoveAllDcsInMap()
    {
        DS ds = giveDsWithDcs();
        ds.removeDcsFromMapNotInDir();
        Assert.IsTrue(ds.DcMap.Count == 0);
    }

    [TestMethod]
    public void testRemoveNoDcsInMap()
    {
        DS ds = giveDsWithDcs();
        int initCount = ds.DcMap.Count;

        ds.saveAllDcsInMap();
        ds.removeDcsFromMapNotInDir();
        int rmCount = ds.DcMap.Count;

        Assert.IsTrue(initCount == rmCount);
    }

    [TestMethod]
    public void testRemoveSomeDcsInMap()
    {
        DS ds = giveDsWithDcs();
        int initCount = ds.DcMap.Count;

        ds.saveAllDcsInMap();
        ds.DcMap[DmCt.PETS_DC.CatName].deleteFile();
        ds.DcMap[DmCt.PASS_DC.CatName].deleteFile();
        ds.removeDcsFromMapNotInDir();
        int rmCount = ds.DcMap.Count;

        Assert.IsTrue(initCount > rmCount);
    }

    [TestMethod]
    public void testAddNoDcsToMapFromDir()
    {
        DS ds = giveDsWithDcs();
        int initCount = ds.DcMap.Count;

        ds.addNewDcsToMapFromDir();
        int addCount = ds.DcMap.Count;
        Assert.IsTrue(addCount == initCount);
    }

    [TestMethod]
    public void testAddADcToMapFromDir()
    {
        DS ds = giveDsWithDcs();
        int initCount = ds.DcMap.Count;

        DmCt.HOBBIES_DC.saveFile();
        ds.addNewDcsToMapFromDir();
        int addCount = ds.DcMap.Count;

        Assert.IsTrue(addCount == initCount + 1);
    }

    [TestMethod]
    public void testAddSomeADcsToMapFromDir()
    {
        DS ds = giveDsWithDcs();
        int initCount = ds.DcMap.Count;

        DC[] addDcs = { DmCt.HOBBIES_DC, DmCt.GAME_TYPES_DC, DmCt.FULL_DC };
        foreach(DC d in addDcs)
            d.saveFile();

        ds.addNewDcsToMapFromDir();
        int addCount = ds.DcMap.Count;
        Assert.IsTrue(addCount == initCount + addDcs.Length);
    }

    [TestMethod]
    public void testIsChoiceExistingDc()
    {
        DS ds = giveDsWithDcs();
        ds.saveAllDcsInMap();

        int dcCount = ds.DcMap.Count;
        for (int i = MU.MENU_START; i <= dcCount; i++)
            Assert.IsTrue(ds.isChoiceExistingDc(i));

        for (int neg = DmCt.MIN_OPT; neg < MU.MENU_START; neg++)
            Assert.IsFalse(ds.isChoiceExistingDc(neg));

        for (int pos = DmCt.MAX_OPT; pos > dcCount; pos--)
            Assert.IsFalse(ds.isChoiceExistingDc(pos));
    }

    [TestMethod]
    public void testIsDcAction()
    {
        DS ds = giveDsWithoutDcs();
        int dcActCount = DSC.dcActions.Count;
        for (int i = MU.MENU_START; i <= dcActCount; i++)
            Assert.IsTrue(ds.isChoiceDcAction(i));

        for (int neg = DmCt.MIN_OPT; neg < MU.MENU_START; neg++)
            Assert.IsFalse(ds.isChoiceExistingDc(neg));

        for (int pos = DmCt.MAX_OPT; pos > dcActCount; pos--)
            Assert.IsFalse(ds.isChoiceExistingDc(pos));
    }

    [TestMethod]
    public void testGetDcItemFromChoice()
    {
        DS ds = giveDsWithoutDcs();
        initDsDcMap(ds);

        int dcCount = ds.DcMap.Count;
        for (int i = MU.MENU_START; i <= dcCount; i++)
        {
            DC expected = ds.DcMap.ElementAt(i - 1).Value;
            DC actual = ds.getDcFromMenuChoice(i);
            Assert.IsTrue(expected == actual);
        }

        for (int neg = DmCt.MIN_OPT; neg < MU.MENU_START; neg++)
        {
            DC actual = ds.getDcFromMenuChoice(neg);
            Assert.IsTrue(actual == DC.EmptyDc);
        }

        for (int pos = DmCt.MAX_OPT; pos > dcCount; pos--)
        {
            DC actual = ds.getDcFromMenuChoice(pos);
            Assert.IsTrue(actual == DC.EmptyDc);
        }
    }

    [TestMethod]
    public void testSaveAndAddToEmptyMap()
    {
        DS ds = giveDsWithoutDcs();
        Assert.IsFalse(ds.DcMap.ContainsKey(DmCt.FULL_DC.CatName));
        Assert.IsTrue(ds.saveAndAddDcToMap(DmCt.FULL_DC));
        Assert.IsTrue(DmCt.FULL_DC.checkFileExists());
        Assert.IsTrue(ds.DcMap.ContainsKey(DmCt.FULL_DC.CatName));
    }

    [TestMethod]
    public void testSaveAndAddToMap()
    {
        DS ds = giveDsWithDcs();
        Assert.IsFalse(ds.DcMap.ContainsKey(DmCt.HOBBIES_DC.CatName));
        Assert.IsTrue(ds.saveAndAddDcToMap(DmCt.HOBBIES_DC));
        Assert.IsTrue(DmCt.HOBBIES_DC.checkFileExists());
        Assert.IsTrue(ds.DcMap.ContainsKey(DmCt.HOBBIES_DC.CatName));
    }

    [TestMethod]
    public void testSaveAndAddSavedToMap()
    {
        DS ds = giveDsWithDcs();
        Assert.IsTrue(ds.DcMap.ContainsKey(DmCt.CHOICELESS_DC.CatName));
        Assert.IsFalse(ds.saveAndAddDcToMap(DmCt.CHOICELESS_DC));
        Assert.IsTrue(DmCt.CHOICELESS_DC.checkFileExists());
        Assert.IsTrue(ds.DcMap.ContainsKey(DmCt.CHOICELESS_DC.CatName));
    }

    [TestMethod]
    public void delAndRmDcFromEmptyMap()
    {
        DS ds = giveDsWithoutDcs();
        Assert.IsFalse(ds.DcMap.ContainsKey(DmCt.PASS_DC.CatName));
        Assert.IsFalse(ds.deleteAndRemoveDcFromMap(DmCt.PASS_DC));
        Assert.IsFalse(ds.DcMap.ContainsKey(DmCt.PASS_DC.CatName));
    }

    [TestMethod]
    public void delAndRmDcFromMap()
    {
        DS ds = giveDsWithDcs();
        Assert.IsTrue(ds.DcMap.ContainsKey(DmCt.PASS_DC.CatName));
        Assert.IsTrue(ds.deleteAndRemoveDcFromMap(DmCt.PASS_DC));
        Assert.IsFalse(DmCt.PASS_DC.checkFileExists());
        Assert.IsFalse(ds.DcMap.ContainsKey(DmCt.PASS_DC.CatName));
    }

    [TestMethod]
    public void delAndRmUnsavedDcFromMap()
    {
        DS ds = giveDsWithDcs();
        Assert.IsFalse(ds.DcMap.ContainsKey(DmCt.FULL_DC.CatName));
        Assert.IsFalse(ds.deleteAndRemoveDcFromMap(DmCt.FULL_DC));
        Assert.IsFalse(ds.DcMap.ContainsKey(DmCt.FULL_DC.CatName));
    }

    [TestMethod]
    public void testAddNothingToDecSummary()
    {
        DS ds = giveDsWithDcs();
        Assert.IsFalse(ds.tryAddDecisionToSummary(DmCt.PETS_DC, TU.BLANK));
    }

    [TestMethod]
    public void testAddDecisionToDecSummary()
    {
        DS ds = giveDsWithDcs();
        Assert.IsTrue(ds.tryAddDecisionToSummary(DmCt.PETS_DC, DmCt.PETS_DC.CatChoices[0]));
    }

    [TestMethod]
    public void testAddFewGoodToDecSummary()
    {
        DS ds = giveDsWithDcs();
        addFewGoodtoDecSummary(ds);
    }

    private void addFewGoodtoDecSummary(DS ds)
    {
        for (int i = 0; i < DmCt.PETS_DC.getChoicesCount(); i++)
            Assert.IsTrue(ds.tryAddDecisionToSummary(DmCt.PETS_DC, DmCt.PETS_DC.CatChoices[i]));

        for (int i = 0; i < DmCt.PASS_DC.getChoicesCount(); i++)
            Assert.IsTrue(ds.tryAddDecisionToSummary(DmCt.PASS_DC, DmCt.PASS_DC.CatChoices[i]));
    }

    [TestMethod]
    public void testAddBadInputsToDecSummary()
    {
        DS ds = giveDsWithDcs();
        addFewBadInputsToDecSummary(ds);
    }

    private void addFewBadInputsToDecSummary(DS ds)
    {
        Assert.IsFalse(ds.tryAddDecisionToSummary(DmCt.PETS_DC, TU.BLANK));
        Assert.IsFalse(ds.tryAddDecisionToSummary(DmCt.PETS_DC, DmCt.PASS_DC.CatChoices[0]));
        Assert.IsFalse(ds.tryAddDecisionToSummary(DmCt.PASS_DC, "\t\n\r"));
        Assert.IsFalse(ds.tryAddDecisionToSummary(DmCt.PETS_DC, ""));
        Assert.IsFalse(ds.tryAddDecisionToSummary(DmCt.PASS_DC, DmCt.PETS_DC.CatChoices[0]));
    }

    [TestMethod]
    public void testAddMixedInputsToDecSummary()
    {
        DS ds = giveDsWithDcs();
        addFewGoodtoDecSummary(ds);
        addFewBadInputsToDecSummary(ds);
    }

    [TestMethod]
    public void testSaveEmptyDecSummary()
    {
        DS ds = giveDsWithDcs();
        Assert.IsFalse(ds.showAndSaveDecSummary());
    }

    [TestMethod]
    public void testAddDecisionAndSaveDecSummary()
    {
        DS ds = giveDsWithDcs();
        ds.decideForUser(DmCt.PETS_DC);
        Assert.IsTrue(ds.showAndSaveDecSummary());
    }

    [TestMethod]
    public void testAddFewGoodAndSaveDecSummary()
    {
        DS ds = giveDsWithDcs();
        for (int i = 0; i < DmCt.PETS_DC.getChoicesCount(); i++)
            ds.decideForUser(DmCt.PETS_DC);
        for (int i = 0; i < DmCt.PASS_DC.getChoicesCount(); i++)
            ds.decideForUser(DmCt.PASS_DC);
        Assert.IsTrue(ds.showAndSaveDecSummary());
    }

    [TestMethod]
    public void testSaveDoneDcsToWip()
    {
        DS ds = giveDsWithDcs();
        foreach(DC dc in ds.DcMap.Values)
            Assert.IsTrue(ds.saveUnfinishedDcToWipCat(dc));

        Assert.IsTrue(ds.saveUnfinishedDcToWipCat(DmCt.GAME_TYPES_DC));
        Assert.IsTrue(ds.saveUnfinishedDcToWipCat(DmCt.HOBBIES_DC));
        Assert.IsTrue(ds.saveUnfinishedDcToWipCat(DmCt.FULL_DC));
    }

    [TestMethod]
    public void testSaveChoicelessDcToWip()
    {
        DS ds = giveDsWithoutDcs();
        Assert.IsTrue(ds.saveUnfinishedDcToWipCat(DmCt.CHOICELESS_DC));
    }

    [TestMethod]
    public void testSaveNamedDcToWip()
    {
        DS ds = giveDsWithoutDcs();
        DC named = new DC();
        
        const string JUST_NAMED = "justNamedDc";
        named.CatName = JUST_NAMED;
        Assert.IsTrue(ds.saveUnfinishedDcToWipCat(named));
    }       

    [TestMethod]
    public void testSaveDefaultDcToWip()
    {
        DS ds = giveDsWithoutDcs();
        DC def = new DC();
        Assert.IsFalse(ds.saveUnfinishedDcToWipCat(def));
    }    

    [TestMethod]
    public void testSaveEmptyDcToWip()
    {
        DS ds = giveDsWithoutDcs();
        Assert.IsFalse(ds.saveUnfinishedDcToWipCat(DC.EmptyDc));
    }      

    [TestInitialize]
    public void TestInitialize()
    {
        clearDir();
        FS.tryDeleteWipFile();
    }

    private DS giveDsWithDcs()
    {
        DS ds = giveDsWithoutDcs();
        initDsDcMap(ds);
        return ds;
    }

    private DS giveDsWithoutDcs()
    {
        return new DS();
    }

    private void clearDir()
    {
        DmCt.clearADir(DSC.DEFAULT_DECISIONS_DIRECTORY);
    }

    private void initDsDcMap(DS ds)
    {

        ds.DcMap.Add(DmCt.CHOICELESS_DC.CatName, DmCt.CHOICELESS_DC);

        DmCt.PETS_DC.CatChoices = new(DmCt.PETS_CHOICES);
        ds.DcMap.Add(DmCt.PETS_DC.CatName, DmCt.PETS_DC);

        DmCt.PASS_DC.CatChoices = new(DmCt.TEST_DC_CHOICES_FEW);
        ds.DcMap.Add(DmCt.PASS_DC.CatName, DmCt.PASS_DC);
    }
}