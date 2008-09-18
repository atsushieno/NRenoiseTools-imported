File Extension & XSD definitions:
---------------------------------

XRNS (Extended Renoise Song) 
  -> RenoiseSongXX.xsd

XRNI (Extended Renoise Instrument) 
  -> RenoiseInstrumentXX.xsd

XRNT (Extended Renoise Track) 
  -> RenoiseDeviceChainsXX.xsd

XRDP (Extended Renoise Device Preset) 
  -> FilterDevicePresetXX.xsd


Note: XRNS, XRNI and XRNTs are zip containers (you can open them with 
any PKZip compatible tool). The main XML file is then in the root of the
Archive. XRDP is a plain XML file.
  

  
Renoise Releases & XSD definition versions:
-------------------------------------------

Renoise 1.5.0 (and below)
  -> N/A: Used different "closed" binary formats
  
Renoise 1.8.0
  -> RenoiseSong 4
  -> RenoiseInstrument 2
  -> RenoiseDeviceChain 3
  -> (no FilterDevicePresets)
  
Renoise 1.9.0
  -> RenoiseSong: 9
  -> RenoiseInstrument: 5
  -> RenoiseDeviceChain: 5
  -> FilterDevicePreset: 1

Renoise 1.9.1 (latest)
  -> RenoiseSong: 10
  -> RenoiseInstrument: 6
  -> RenoiseDeviceChain: 6
  -> FilterDevicePreset: 1
  
Note: all remaining intermediate XSD definition versions (e.g. 
RenoiseSong 5-8) versions can safely be ignored. These are only used in 
"private" alpha/beta releases which never went public.

