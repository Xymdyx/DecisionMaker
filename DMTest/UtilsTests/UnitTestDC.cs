namespace DMTest;
using System.Linq;
[TestClass]
public class UnitTestDC1
{
    [TestMethod]
    public void test2ParamInit()
    {
        DC partial = new(DmUtConsts.TEST_DC_NAME, DmUtConsts.TEST_DC_DESC);
        Assert.IsNotNull(partial);
        Assert.IsTrue(partial.IsValidDc());
        Assert.IsFalse(partial.hasChoices());
        Assert.AreEqual(partial.CatName, DmUtConsts.TEST_DC_NAME);
        Assert.AreEqual(partial.CatDesc, DmUtConsts.TEST_DC_DESC);
    }

    [TestMethod]
    public void testFullInit()
    {
        DC full = new(DmUtConsts.TEST_DC_NAME, DmUtConsts.TEST_DC_DESC, DmUtConsts.TEST_DC_CHOICES);
        Assert.IsNotNull(full);

        Assert.IsTrue(full.IsValidDc());
        Assert.IsTrue(full.hasChoices());
        Assert.AreEqual(full.CatName, DmUtConsts.TEST_DC_NAME);
        Assert.AreEqual(full.CatDesc, DmUtConsts.TEST_DC_DESC);
        Assert.AreEqual(full.CatChoices, DmUtConsts.TEST_DC_CHOICES);
        Assert.IsTrue(full.checkFileExists());
    }

    [TestMethod]
    public void testEmptyInit()
    {
        DC blank = new(TU.BLANK, TU.BLANK);
        Assert.IsFalse(blank.IsValidDc());
        Assert.AreEqual(blank.CatName, TU.BLANK);
        Assert.AreEqual(blank.CatDesc, TU.BLANK);
    }

    [TestMethod]
    public void testSave()
    {
        DC toSave = new(DmUtConsts.TEST_DC_DESC, DmUtConsts.TEST_DC_NAME);
        Assert.IsTrue(toSave.saveFile());
        Assert.IsTrue(toSave.checkFileExists());
    }

    [TestMethod]
    public void testDelete()
    {
        DC toDelete = new(DmUtConsts.TEST_DC_DESC, DmUtConsts.TEST_DC_NAME);
        Assert.IsTrue(toDelete.deleteFile());
        Assert.IsFalse(toDelete.checkFileExists());
    }

    [TestMethod]
    public void testEmptyDC()
    {
        Assert.IsNotNull(DC.EmptyDc);
        Assert.IsFalse(DC.EmptyDc.IsNotEmptyDc());
        Assert.IsFalse(DC.EmptyDc.IsValidDc());
    }

    [TestMethod]
    public void testStringifyChoices()
    {
        DmCt.resetTestDcs();
        const string emptyResult = "No choices in \n";
        Assert.AreEqual(DC.EmptyDc.stringifyChoicesOnALine(), emptyResult);

        assertAllDcStringify(DC.EmptyDc);
        assertAllDcStringify(DmCt.CHOICELESS_DC);
        assertAllDcStringify(DmCt.FULL_DC);
        assertAllDcStringify(DmCt.GAME_TYPES_DC);
        assertAllDcStringify(DmCt.HOBBIES_DC);
        assertAllDcStringify(DmCt.PASS_DC);
        assertAllDcStringify(DmCt.PETS_DC);

    }

    private void assertAllDcStringify(DC dc)
    {
        assertStringifyOneLineForm(dc);
        assertStringifyChoicePerLineForm(dc);
        assertStringifyManyChoicesPerLineForm(dc);
    }

    private void assertStringifyOneLineForm(DC dc)
    {
        const int linesExpected = 1;
        int commasExpected = Math.Max(0,dc.getChoicesCount() - 1);
        string result = dc.stringifyChoicesOnALine();
        assertStringifyForm(result, linesExpected, commasExpected);
    }
    private void assertStringifyChoicePerLineForm(DC dc)
    {
        int linesExpected = Math.Max(0, dc.getChoicesCount() - 1);
        const int commasExpected = 0;
        string result = dc.stringifyChoicePerLine();
        assertStringifyForm(result, linesExpected, commasExpected);
    }

    private void assertStringifyManyChoicesPerLineForm(DC dc)
    {
        float newLinesF = float.Ceiling((float)dc.getChoicesCount()/ DC.CHOICES_PER_MANY_LINE);
        int linesExpected = (int) newLinesF;
        int commasExpected = Math.Max(0,dc.getChoicesCount() - 1);
        string result = dc.stringifyManyChoicesPerLine();
        assertStringifyForm(result, linesExpected, commasExpected);
    }

    private void assertStringifyForm(string result, int LinesExpected, int commasExpected)
    {
        int actualLines = result.Count(c => c == '\n');
        int actualCommas = result.Count(c => c == ',');

        Console.WriteLine($"Expected newlines: {LinesExpected}\n Actual newlines: {actualLines}");
        Console.WriteLine($"Expected commas: {commasExpected}\n Actual newlines: {actualCommas}");
        Console.WriteLine(result);

        Assert.IsTrue( actualLines == LinesExpected);
        Assert.IsTrue( actualCommas == commasExpected);
    }

}