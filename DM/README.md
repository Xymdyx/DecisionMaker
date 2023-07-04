# Purpose:
This program is a CLI program meant to help indecisive souls make a decision through picking a random choice from a provided list.\
I often find myself knowing what category of thing I want to do but not a specific activity within said category. Hence this program.

# Running
dotnet and csc are required to run/compile the DM.dll...\
To run the app, run any of these from the top-level of the DM folder:
<br><br>
**.\your\path\to\DM.exe** (Release exe works alone on 64-bit windows. Building your own needs all files made by dotnet)
<br>
**dotnet .\your\path\to\generated\DM.dll**
<br>
**dotnet run**
<br><br>
# Building/Compiling
If you're not a building wizard, you can build the app as follows:
<br>
For a dll, exe, etc run:\
**dotnet build .\your\path\to\DM\DM.csproj**
<br><br>
" -c Release" may be appended to optimize the code,\
then you must subsitute "Debug" with "Release" in the following path:

<br><br>
.\DM\bin\Debug\net7.0
<br><br>
Outputs a dll, exe, and their dependencies (which can be run per the "Running" section "):
<br>

# Usage Notes:
This takes no command line arguments.\
<br>
One can create Decision Category files before running the program, but they must be in proper form...\
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
]\
