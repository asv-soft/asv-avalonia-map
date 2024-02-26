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

(for %%p in (%projects%) do (
	cd src\%%p\bin\Release\
	dotnet nuget push %%p.%VERSION%.nupkg --source https://api.nuget.org/v3/index.json
	dotnet nuget push %%p.%VERSION%.nupkg --source https://nuget.pkg.github.com/asv-soft/index.json
	cd ../../../../
)) 





