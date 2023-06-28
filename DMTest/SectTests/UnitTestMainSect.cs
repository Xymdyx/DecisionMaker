namespace DMTest;
/// <summary>
/// dotnet test .\DMTest.csproj 
/// </summary>
[TestClass]
public class UnitTestMainSect
{
    [TestMethod]
    public void testDir()
    {
        Assert.IsFalse(HS.checkAndInitDir());
    }
}