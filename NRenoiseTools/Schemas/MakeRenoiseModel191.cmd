setlocal
set SCHEMA=.
..\Bin\XsdRenoiseParser %SCHEMA%\RenoiseSong10.xsd %SCHEMA%\RenoiseInstrument6.xsd %SCHEMA%\RenoiseDeviceChain6.xsd /out:RenoiseModel191 /classes /serializers
pause