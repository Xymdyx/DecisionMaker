# TLDR:
This is the supporting Unit Tests for the DecisionMaker app.\
It isn't all encompassing and only tests for scenarios that are likely when
the program is executing (i.e. not a lot of nulls where nulls aren't likely to happen while the program runs)

# How to Compile:
This was made via this tutorial:\
https://learn.microsoft.com/en-us/dotnet/core/tutorials/testing-library-with-visual-studio-code?pivots=dotnet-7-0
<br>
As such, it requires dotnet, csc compiler, and mstest.

## Running the tests:
I recommend running the following command from the top-level of the DMTest dir:\
dotnet test .\DMTest.csproj
<br>
This will do the whole shebang.\
Use VsCode/Visual Studio to run individual tests as needed.
<br>
https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-test

## Bug Reporting:
If any failures are found with the provided tests (in the provided version of the code), feel free to reach out with details.
