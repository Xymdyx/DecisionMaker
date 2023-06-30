# Purpose:
This program is a CLI program meant to help indecisive souls make a decision through picking a random choice from a provided list.\
I often find myself knowing what category of thing I want to do but not a specific activity within said category. Hence this program.

# Compiling
dotnet and csc are required to compile the DecisionMaker.dll...\
If you don't use the vscode builder, you can build the app as follows.\
<br>
For the dll :\
dotnet build .\your\path\to\DecisionMaker\DecisionMaker.csproj\
<br>
Outputs this dll (which you can run directly):\
.\DM-App\DecisionMaker\bin\Debug\net7.0\DecisionMaker.dll\
<br>
# Usage Notes:
This takes no command line arguments.\
Run it with .\your\path\to\DecisionMaker.dll \
on Windows or the other OS equivalents.\
<br>
One can create Decision Category files before running the program but must be of proper form...\
Decision Category Minimum Format:\
[\
SampleDcName \
SampleDcDescription\
]

However, adding files while the program runs won't do any thing. Manipulating generated files or directories is strongly discouraged\
and disregarded where possible.

## Decision Category Files:
Foremost, "Decision Category" files are files created by this program for future use.\
These are stored in the Decision\Categories directory and scanned on future runs of the program\
so users don't have to retype previous Decision Categories every time.

### Decision Category format:
[\
SampleDcName\
SampleDcDescription\
OptionalChoice1\
.\
.\
OptionalChoiceN\
]\
That is, the category name on the first line, the description of the 2nd line, and the rest containing category options.

## Profile Files:
These are stored in the ProfileStorage directory, these are scanned on bootup like Decision Category files\
but are much simpler as profile customization is simple for now.

### Profile files format:
[\
    one-line-of-text\
]
