/**
* auth: Sam Ford
* desc: struct to represent the personality of the user's decision maker.
*/

using DPS = DecisionMaker.ProfileSection;

public struct Personality
{
    private const string DEFAULT_GREETING = "We will ask you what you want a decision for shortly...";
    private const string DEFAULT_EXITING = "Thanks for consulting us!";
    private const string DEFAULT_DISPLAY_NAME = "friend";

    private string? _mainGreetingMsg;
    private string? _mainExitMsg;
    private string? _displayName;

    public string? mainGreeting { get => _mainGreetingMsg;}
    public string? mainExit { get => _mainExitMsg;}
    public string? displayName { get => _displayName;}

    public Personality()
    {
        applyFileChangesToPersonality();
    }

    public Personality(string greeting, string exit, string displayName)
    {
        this._mainGreetingMsg = greeting;
        this._mainExitMsg = exit;
        this._displayName = displayName;
    }

    public void applyFileChangesToPersonality()
    {
        if(File.Exists(DPS.PROFILE_GREETING_PATH))
            _mainGreetingMsg = File.ReadAllText(DPS.PROFILE_GREETING_PATH);             
        if(File.Exists(DPS.PROFILE_EXITING_PATH))
            _mainExitMsg = File.ReadAllText(DPS.PROFILE_EXITING_PATH);
        if(File.Exists(DPS.PROFILE_DISPLAY_NAME_PATH))
            _displayName = File.ReadAllText(DPS.PROFILE_DISPLAY_NAME_PATH);

        fillInBlankFields();
    }    

    private void fillInBlankFields()
    {
        if(!isGreetCustom())
            this._mainGreetingMsg = DEFAULT_GREETING;
        if(!isExitCustom())
            this._mainExitMsg = DEFAULT_EXITING;
        if(!isDisplayNameCustom())
            this._displayName = DEFAULT_DISPLAY_NAME;
    }

    public bool isGreetCustom()
    {
        return isPartCustom(this._mainGreetingMsg!);
    }

    public bool isExitCustom()
    {
        return isPartCustom(this._mainExitMsg!);
    }

    public bool isDisplayNameCustom()
    {
        return isPartCustom(this._displayName!);
    }

    private bool isPartCustom(string field)
    {
        return (field != "") && (field != null);
    }

    public override string ToString()
    {
        return $"Personality: greeting = \"{_mainGreetingMsg}\", exiting = \"{_mainExitMsg}\", displayName = \"{_displayName}\"";
    }
}