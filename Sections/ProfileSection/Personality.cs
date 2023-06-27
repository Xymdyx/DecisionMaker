/**
* auth: Sam Ford
* desc: struct to represent the personality of the user's decision maker.
*/

namespace DecisionMaker
{
    internal class Personality
    {
        internal const string DEFAULT_GREETING = "We will ask you what you want a decision for shortly...";
        internal const string DEFAULT_EXITING = "Thanks for consulting us!";
        internal const string DEFAULT_DISPLAY_NAME = "friend";
        private const string PERSONALITY_ERR_HEADER = "Personality.cs:";

        private string? _mainGreetingMsg;
        private string? _mainExitMsg;
        private string? _displayName;

        internal string? mainGreeting { get => _mainGreetingMsg; }
        internal string? mainExit { get => _mainExitMsg; }
        internal string? displayName { get => _displayName; }

        internal Personality()
        {
            applyFileChangesToPersonality();
        }

        internal Personality(string greeting, string exit, string displayName)
        {
            this._mainGreetingMsg = greeting;
            this._mainExitMsg = exit;
            this._displayName = displayName;
        }

        internal void applyFileChangesToPersonality()
        {
            try
            {
                if (File.Exists(PSC.PROFILE_GREETING_PATH))
                    _mainGreetingMsg = File.ReadAllText(PSC.PROFILE_GREETING_PATH);
                if (File.Exists(PSC.PROFILE_EXITING_PATH))
                    _mainExitMsg = File.ReadAllText(PSC.PROFILE_EXITING_PATH);
                if (File.Exists(PSC.PROFILE_DISPLAY_NAME_PATH))
                    _displayName = File.ReadAllText(PSC.PROFILE_DISPLAY_NAME_PATH);

                fillInBlankFields();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{PERSONALITY_ERR_HEADER} failed to update personality parts...");
                TU.logErrorMsg(e);
            }
        }

        private void fillInBlankFields()
        {
            if (!isGreetCustom() || !File.Exists(PSC.PROFILE_GREETING_PATH))
                this._mainGreetingMsg = DEFAULT_GREETING;
            if (!isExitCustom() || !File.Exists(PSC.PROFILE_EXITING_PATH))
                this._mainExitMsg = DEFAULT_EXITING;
            if (!isDisplayNameCustom() || !File.Exists(PSC.PROFILE_DISPLAY_NAME_PATH))
                this._displayName = DEFAULT_DISPLAY_NAME;
        }

        internal bool isGreetCustom()
        {
            return isPartCustom(_mainGreetingMsg!) && _mainGreetingMsg != DEFAULT_GREETING;
        }

        internal bool isExitCustom()
        {
            return isPartCustom(_mainExitMsg!) && _mainExitMsg != DEFAULT_EXITING;
        }

        internal bool isDisplayNameCustom()
        {
            return isPartCustom(_displayName!) && _displayName != DEFAULT_DISPLAY_NAME;
        }

        private bool isPartCustom(string field)
        {
            return TU.isInputAcceptable(field);
        }

        public override string ToString()
        {
            return $"Personality: greeting = \"{_mainGreetingMsg}\", exiting = \"{_mainExitMsg}\", displayName = \"{_displayName}\"";
        }
    }
}