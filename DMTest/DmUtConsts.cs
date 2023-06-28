public static class DmUtConsts
{
    internal const string DM_RELATIVE_PATH = @"..\..\..\..\DecisionMaker";
    internal const string UT_INFO_HEADER = "DmUnitTests:";

    internal const int MIN_OPT = -9999;
    internal const int MAX_OPT = 9909;

    internal const string TEST_DN = "Xymdyx!8?9@2$";
    internal const string TEST_GREET = "Hullo there friendliest of friends!";
    internal const string TEST_DEPART = "Goodbye friendliest of friends!";

    internal const string TEST_DC_NAME = "Test";
    internal const string TEST_DC_DESC = "Thing to test";
    internal static readonly List<string> TEST_DC_CHOICES = new() { "Item1", "Item2" };
    internal static readonly List<string> TEST_DC_CHOICES_FEW = new() { "Choice", "option", "thingamajig" };

    internal static void logPreProcessingFail(Exception e)
    {
        Console.WriteLine($"{DmUtConsts.UT_INFO_HEADER}: pre-processing failed for test, failing now!");
        Console.WriteLine(e.Message + "\n");
    }
}