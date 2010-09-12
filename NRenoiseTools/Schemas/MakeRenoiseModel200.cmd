setlocal
set SCHEMA="C:\Program Files (x86)\Renoise 2.5.0\Schemas"
..\Bin\XsdRenoiseParser %SCHEMA%\RenoiseSong21.xsd /out:RenoiseModel250 /classes
pause