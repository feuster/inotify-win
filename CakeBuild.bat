rem ============================================
rem Set environment
rem ============================================
@echo off
cd "%~dp0"
set appname=InotifyWait
set versiondefault=1.10.0.0

rem ============================================
rem DO NOT EDIT THE SECTION BEYOND THIS LINE!
rem ============================================
cls
echo ****************************************
echo Starting publishing...
echo ****************************************
echo.
echo ========================================
echo Version information
echo ========================================
set /P version=Please enter version information (default: %versiondefault%): || set version=%versiondefault%
set artifact="%~dp0publish\%appname%_%version%.exe"
set zipartifact="%~dp0publish\%appname%_%version%.zip"
echo.
echo ========================================
echo CakeBuild powershell
echo ========================================
set buildps="%~dp0CakeBuild.ps1"
if not exist %buildps% (
	echo Creating %buildps%
	echo dotnet run --project build/Build.csproj -- $args>%buildps%
	echo exit $LASTEXITCODE;>>%buildps%
	echo.>>%buildps%
) else (
	echo %buildps% already created
)
echo.
echo ========================================
echo dotnet restore
echo ========================================
dotnet restore
echo.
echo ========================================
echo Starting Cake build...
echo ========================================
Powershell.exe -executionpolicy remotesigned -File %buildps% --configuration=Release --framework=net9.0 --runtime=win-x64 --publishdir=../publish --publishexename="%appname%" --publishversion="%version%"
echo.
echo ========================================
echo Published files
echo ========================================
if not exist %artifact% (
	echo %appname% publishing with error finished. Binary can not be found in subfolder:
	echo  %artifact%
) else (
	echo %appname% publishing finished. Binary can be found in subfolder:
	echo  %artifact%
)
if not exist %zipartifact% (
	echo Zip archive publishing with error finished. Zip archive can not be found in subfolder:
	echo  %zipartifact%
) else (
	echo Zip archive publishing finished. Zip archive can be found in subfolder:
	echo  %zipartifact%
)
echo.

rem ============================================
rem Comment the pause out for script auto-close
rem ============================================
pause