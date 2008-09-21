setlocal
call "C:\Program Files\Microsoft Visual Studio 9.0\VC\vcvarsall.bat" x86
set DXROOT=C:\Program Files\Sandcastle
set PATH=..\nant\bin;%PATH%
REM msbuild /t:Build /p:Configuration=Release NetAsm.sln
nant %1 %2 %3 %4 %5 %6
pause