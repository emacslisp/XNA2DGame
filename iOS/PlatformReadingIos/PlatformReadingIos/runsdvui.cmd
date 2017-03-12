cd /d "D:\dev\XNA2DGame\iOS\PlatformReadingIos\PlatformReadingIos" &msbuild "PlatformReadingIos.csproj" /t:sdvViewer /p:configuration="Debug" /p:platform=iPhone
exit %errorlevel% 