@echo off
rem ====== projects ======

set projects=Asv.Avalonia.Map

rem ====== projects ======

rem set git to global PATH
SET BASH_PATH="%SYSTEMDRIVE%\Program Files\Git\bin"
SET PATH=%BASH_PATH%;%PATH%

 rem copy version to text file, then in variable
git describe --tags --abbrev=4 >>version.txt
SET /p VERSION=<version.txt
del version.txt

rem Extracting only X.X.X from the version string
SET VERSION=%VERSION:v=%
for /f "tokens=1 delims=-" %%V in ("%VERSION%") do (
    set "version=%%V"
)
set VERSION=%version%



rem build all projects
(for %%p in (%projects%) do (
  	echo %%p
	dotnet restore ./src/%%p/%%p.csproj
	dotnet build /p:SolutionDir=../;ProductVersion=%VERSION% ./src/%%p/%%p.csproj -c Release
	dotnet pack ./src/%%p/%%p.csproj -c Release
)) 




