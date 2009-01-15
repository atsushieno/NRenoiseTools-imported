setlocal
set SCHEMA="C:\Program Files\Renoise 2.0.0\Schemas"
..\Bin\XsdRenoiseParser %SCHEMA%\RenoiseSong14.xsd %SCHEMA%\RenoiseInstrument7.xsd %SCHEMA%\RenoiseDeviceChain7.xsd /out:RenoiseModel200 /classes
pause