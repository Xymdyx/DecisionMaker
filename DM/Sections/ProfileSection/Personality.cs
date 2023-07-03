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
        private const string PERS_INFO_INTRO = "Personality.cs:";

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

        // syncs the profile to what's inside the profile directory
        internal void applyFileChangesToPersonality()
        {
            try
            {
                _mainGreetingMsg = TU.readWholeFileAndTrim(PSC.PROF_GREETING_PATH);
                _mainExitMsg = TU.readWholeFileAndTrim(PSC.PROF_EXITING_PATH);
                _displayName = TU.readWholeFileAndTrim(PSC.PROF_DISPLAY_NAME_PATH);

                fillInBlankFields();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{PERS_INFO_INTRO} failed to update personality parts...");
                TU.logErrorMsg(e);
            }
        }

        /// sets personality parts to stored defaults if there's nothing in theie corresponding files
        private void fillInBlankFields()
        {
            if (!isGreetCustom() || !File.Exists(PSC.PROF_GREETING_PATH))
                this._mainGreetingMsg = DEFAULT_GREETING;
            if (!isExitCustom() || !File.Exists(PSC.PROF_EXITING_PATH))
                this._mainExitMsg = DEFAULT_EXITING;
            if (!isDisplayNameCustom() || !File.Exists(PSC.PROF_DISPLAY_NAME_PATH))
                this._displayName = DEFAULT_DISPLAY_NAME;
        }

        internal bool isGreetCustom()
        {
            return isPartCustom(_mainGreetingMsg!, DEFAULT_GREETING);
        }

        internal bool isExitCustom()
        {
            return isPartCustom(_mainExitMsg!, DEFAULT_EXITING);
        }

        internal bool isDisplayNameCustom()
        {
            return isPartCustom(_displayName!, DEFAULT_DISPLAY_NAME);
        }

        private bool isPartCustom(string field, string def)
        {
            return TU.isInputAcceptable(field) && field != def;
        }

        public override string ToString()
        {
            return $"{PERS_INFO_INTRO}: greeting = \"{_mainGreetingMsg}\", exiting = \"{_mainExitMsg}\", displayName = \"{_displayName}\"";
        }
    }
}