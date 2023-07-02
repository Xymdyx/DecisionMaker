
using System.Collections;
using DsUt = DMTest.UnitTestDecSect;
namespace DMTest;

[TestClass]
public class UnitTestDcSect
{
    [TestMethod]
    public void testTryRemove()
    {
        DCSEC dcSec = new();

        List<string> fullChoices = DsUt.FULL_DC.CatChoices;
        removeChoiceBlankUb(1, fullChoices, dcSec);
        removeChoiceBlankLb(fullChoices.Count, fullChoices, dcSec);

        while (fullChoices.Count > 0)
            Assert.AreNotEqual(dcSec.tryRemoveChoice(1, fullChoices), TU.BLANK);

        List<string> none = DsUt.CHOICELESS_DC.CatChoices;
        removeChoiceBlankLb(0, none, dcSec);
        removeChoiceBlankUb(0, none, dcSec);
    }

    private void removeChoiceBlankLb(int lb, List<string> opts, DCSEC dcSec)
    {
        int i = DmUtConsts.MAX_OPT;
        while (i > lb)
        {
            Assert.AreEqual(dcSec.tryRemoveChoice(i, opts), TU.BLANK);
            --i;
        }
    }

    private void removeChoiceBlankUb(int ub, List<string> opts, DCSEC dcSec)
    {
        int j = DmUtConsts.MIN_OPT;
        while (j < ub)
        {
            Assert.AreEqual(dcSec.tryRemoveChoice(j, opts), TU.BLANK);
            ++j;
        }
    }

    [TestMethod]
    public void testGetDcActions()
    {
        DCSEC dcSec = new();
        string[] dcActKeys = dcSec.getDcActionKeys();
        foreach (string a in dcActKeys)
            Assert.IsTrue(DSC.dcActions.Contains(a));
    }

    [TestMethod]
    public void testGetDcTermVals()
    {
        DCSEC dcSec = new();
        bool[] dcTermVals = dcSec.getDcActionTerminateVals();
        int i = 0;
        foreach (DictionaryEntry de in DSC.dcActions)
        {
            Assert.IsTrue( (bool) de.Value! == dcTermVals[i]);
            i++;
        }
    }

    [TestInitialize]
    public void TestInitialize()
    {
        DmUtConsts.clearADir(DSC.DEFAULT_DECISIONS_DIRECTORY);
    }    
}