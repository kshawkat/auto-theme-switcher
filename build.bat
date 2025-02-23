@echo off
echo Building Auto Theme Switcher...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
echo.
echo Build complete! 
echo.
pause