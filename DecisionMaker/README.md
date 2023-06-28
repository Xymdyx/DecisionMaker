Purpose:
This program is a CLI program meant to help indecisive souls make a decision through picking a random choice from a provided list.
I often find myself knowing what category of thing I want to do but not a specific activity within said category. Hence this program.

Usage Notes:
This takes no command line arguments. Run it with .\DecisionMaker.exe on Windows or the other OS equivalents.
One can create Decision Category files before running the program but must be of proper form.

Decision Category Minimum Format:
[
SampleDcName
SampleDcDescription
]

However, adding files while the program runs won't do any thing. Manipulating generated files or directories is strongly discouraged
and disregarded where possible.

Decision Category Files:
Foremost, "Decision Category" files are files created by this program for future use.
These are stored in the Decision\Categories directory and scanned on future runs of the program
so users don't have to retype Decision Categories eveery time.

Decision Category format:
[
SampleDcName
SampleDcDescription
OptionalChoice1
.
.
.
OptionalChoiceN
]
That is, the category name on the first line, the description of the 2nd line, and the rest containing category options.


Profile Files:
These are stored in the ProfileStorage directory, these are scanned on bootup like Decision Category files
but are much simpler as profile customization is simple for now.

Profile files format:
[
    one-line-of-text
]