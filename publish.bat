cd /d %~dp0
rd /Q /S app
dotnet publish -c release -r win-x64 --self-contained
md app
move bin\release\netcoreapp2.2\win-x64\publish app\bin

pause