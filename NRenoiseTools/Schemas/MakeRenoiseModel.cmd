setlocal
set SCHEMA="C:\Program Files (x86)\Renoise 2.7.1\Schemas"
..\Bin\XsdRenoiseParser %SCHEMA%\RenoiseSong30.xsd /out:RenoiseModel /classes
pause