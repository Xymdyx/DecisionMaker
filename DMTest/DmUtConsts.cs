public static class DmUtConsts
{
    internal const string DM_RELATIVE_PATH = @"..\..\..\..\DM";
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

 // In initialize DS with DCs
    internal static readonly DC CHOICELESS_DC = new("Category", "A category with no choices");
    internal static readonly DC PASS_DC = new(DmCt.TEST_DC_NAME, DmCt.TEST_DC_DESC, new(DmCt.TEST_DC_CHOICES_FEW));

    internal static readonly List<string> PETS_CHOICES = new() { "Dog", "Cat", "Hamster", "Snake", "Parrot" };
    internal static readonly DC PETS_DC = new("Pets To Get", "Pets I want to adopt", new(PETS_CHOICES));

    // Unbound from initialized DS
    internal static readonly DC FULL_DC = new("name", "desc", new(DmCt.TEST_DC_CHOICES_FEW));

    internal static readonly List<string> GAME_TYPES_CHOICES = new() 
    { "Puzzle", "FPS", "RPG", "Platformer", "Simulator", "Sandbox", "Open World", "Mystery", "Visual Novel", "Rhythmn", "Bullet Hell", "MOBA", "MMO", "Puzzle", "RTS", "Horror" };
    internal static readonly DC GAME_TYPES_DC = new(
        "Game Genres",
        "Which game genre I should play now",
        new(GAME_TYPES_CHOICES)
     );

    internal static readonly List<string> HOBBIES_CHOICES = new()
    { "Cooking", "Play a video game", "Program something", "Exercise", "Watch YouTube", "Contemplate", "Study a language", "Clean", "Hang out with friend(s)" };
    internal static readonly DC HOBBIES_DC = new(
       "Hobbies",
       "What to do with my free time",
       new(HOBBIES_CHOICES)
    );    

    internal static void clearADir(string dir)
    {
        try
        {
            Directory.Delete(dir, true);
        }
        catch(Exception e)
        {
            logPreProcessingFail(e);
        }
    }

    internal static void logPreProcessingFail(Exception e)
    {
        Console.WriteLine($"{UT_INFO_HEADER}: pre-processing failed for test, failing now!");
        Console.WriteLine(e.Message + "\n");
    }

    internal static void resetTestDcs()
    {
        DmCt.FULL_DC.CatChoices = DmCt.TEST_DC_CHOICES_FEW;
        DmCt.PASS_DC.CatChoices = DmCt.TEST_DC_CHOICES_FEW;
        DmCt.HOBBIES_DC.CatChoices = DmCt.HOBBIES_CHOICES;
        DmCt.PETS_DC.CatChoices = DmCt.PETS_CHOICES;
        DmCt.GAME_TYPES_DC.CatChoices = DmCt.GAME_TYPES_CHOICES;
        DmCt.CHOICELESS_DC.CatChoices = new();
    }
}