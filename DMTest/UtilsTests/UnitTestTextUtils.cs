namespace DMTest;

/// <summary>
/// dotnet test .\DMTest.csproj 
/// </summary>
[TestClass]
public class UnitTestTextUtils
{
    private const string ALPHABET = "abcdefghijklmnopqrstuvwxyz";
    private const string MY_NAME = "Xymdyx";
    private const string NAME_MIX = "XyMdYx";
    private const string ALPHANUMERIC_GAME = "Nine hours 9 PeRsOnS 9 DOORS";
    private const string ZERO_TO_NINE = "0123456789";
    private const string FAV_NUM = "892";
    private const string LUCKY_NUM = "7777777";
    private const string BEST_SINGLE_DIGIT = "2";
    private const string ALPHABET_0_TO_9 = ALPHABET + ZERO_TO_NINE;
    private const string SPEECH = "Look at me!";
    private const string NUMBERED_SPEECH = "100K at Me!";

    private readonly List<string> empty = new();
     private readonly List<string> one = new() { "Sam" };
     private readonly List<string> few = new() { "Sam", "Josh", "Austin", "Andrew", "Misty", "Ethan", "Ronnie", "Will", "Tim" };

    [TestMethod]
    public void testIsAlpha()
    {
        Assert.IsTrue(TU.isAlpha(ALPHABET));
        Assert.IsTrue(TU.isAlpha(ALPHABET.ToUpper()));
        Assert.IsTrue(TU.isAlpha(NAME_MIX));
        Assert.IsTrue(TU.isAlpha(MY_NAME));

        Assert.IsFalse(TU.isAlpha(ZERO_TO_NINE));
        Assert.IsFalse(TU.isAlpha(ALPHABET_0_TO_9));
        Assert.IsFalse(TU.isAlpha(ALPHANUMERIC_GAME));
        Assert.IsFalse(TU.isAlpha(NUMBERED_SPEECH));
        Assert.IsFalse(TU.isAlpha(SPEECH));
    }

    [TestMethod]
    public void testIsNumeric()
    {
        Assert.IsTrue(TU.isNumeric(ZERO_TO_NINE));
        Assert.IsTrue(TU.isNumeric(FAV_NUM));
        Assert.IsTrue(TU.isNumeric(BEST_SINGLE_DIGIT));
        Assert.IsTrue(TU.isNumeric(LUCKY_NUM));
        Assert.IsTrue(TU.isNumeric(Int32.MaxValue.ToString()));

        Assert.IsFalse(TU.isNumeric(ALPHABET));
        Assert.IsFalse(TU.isNumeric(ALPHABET_0_TO_9));
        Assert.IsFalse(TU.isNumeric(ALPHANUMERIC_GAME));
        Assert.IsFalse(TU.isNumeric(SPEECH));
        Assert.IsFalse(TU.isNumeric(NUMBERED_SPEECH));
    }

    [TestMethod]
    public void testPrettyStringfiy()
    {
        Assert.AreEqual(TU.prettyStringifyList(empty), TU.BLANK);
        Assert.AreEqual(TU.prettyStringifyList(one), "Sam");

        string fewExpected = String.Join(", ", few);
        Assert.AreEqual(TU.prettyStringifyList(few), fewExpected);
    }

    [TestMethod]
    public void testIsStringListEmpty()
    {
        Assert.IsTrue(TU.isStringListEmpty(empty));
        Assert.IsFalse(TU.isStringListEmpty(one));
        Assert.IsFalse(TU.isStringListEmpty(few));
    }

    [TestMethod]
    public void testIsStop()
    {
        foreach(string w in TU.stopWords)
            Assert.IsTrue(TU.isInputStopCommand(w));
    }

    [TestMethod]
    public void testConvertMenuInput()
    {
        for (int i = DmCt.MIN_OPT; i < DmCt.MAX_OPT; i++)
            Assert.IsTrue(TU.convertMenuInputToInt(i.ToString()) == i);
    }

    [TestMethod]
    public void textConvertTextToInt32()
    {
        for (int i = DmCt.MIN_OPT; i < DmCt.MAX_OPT; i++)
            Assert.IsTrue(TU.convertTextToInt32(i.ToString(), out int j));        
    }
}