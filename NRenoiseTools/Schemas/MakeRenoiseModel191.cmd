setlocal
set SCHEMA="C:\Program Files\Renoise 1.9.1\Schemas"
..\Bin\XsdRenoiseParser %SCHEMA%\RenoiseSong10.xsd %SCHEMA%\RenoiseInstrument6.xsd %SCHEMA%\RenoiseDeviceChain6.xsd /out:RenoiseModel191 /classes
pause