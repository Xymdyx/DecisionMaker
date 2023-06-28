namespace DMTest;

/// <summary>
/// dotnet test .\DMTest.csproj 
/// </summary>
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
}