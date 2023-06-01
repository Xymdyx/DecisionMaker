/**
* auth: Sam Ford
* desc: struct to represent the personality of the user's decision maker.
*/

public struct Personality
{
    private string? _mainGreetingMsg;
    private string? _mainExitMsg;
    private string? _displayName;

    public string? mainGreeting { get => _mainGreetingMsg;}
    public string? mainExit { get => _mainExitMsg; }
    public string? displayName { get => _displayName; }

    private bool isGreetCustom()
    {
        return isPartCustom(this._mainGreetingMsg!);
    }

    public bool isExitCustom()
    {
        return isPartCustom(this._mainExitMsg!);
    }

    private bool isDisplayNameCustom()
    {
        return isPartCustom(this._displayName!);
    }

    private bool isPartCustom(string field)
    {
        return (field != "") && (field != null);
    }

    public Personality(string greeting, string exit, string displayName)
    {
        this._mainGreetingMsg = greeting;
        this._mainExitMsg = exit;
        this._displayName = displayName;
    }

    public override string ToString()
    {
        return $"Personality: greeting = \"{_mainGreetingMsg}\", exiting = \"{_mainExitMsg}\", displayName = \"{_displayName}\"";
    }

}