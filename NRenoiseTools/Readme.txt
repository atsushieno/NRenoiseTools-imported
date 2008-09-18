NRenoiseTools provides a collection of tools to easily manipulate XRNS Renoise Song and XRNI Renoise Instrument File Formats.  This project is composed of :
-	A common library NRenoiseTools assembly that contains almost everything to easily manipulate XRNS and XRNI files.
-	A Xrns2Midi converter able to convert a XRNS song to a Midi AMF 1/2 File format.
-	A Xrns2Xrni extractor able to extract all the instruments from a song and save them to independent XRNI files.
-	The XsdRenoiseParser specially developed to build an optimized version of the original XSD Renoise Model.

Features
-	An optimized model extracted from Renoise XSD Files. The generated model in NRenoiseTools is 10 times smaller than the original XSD model  from Renoise.
-	Original Renoise Model is slightly extended while still being compatible to provide additional inheritance between XML schema complex types.
-	Easily manipulates Renoise Songs  and Renoise Instruments : Load and Save on Song and instruments.
-	With the use of partial class feature on C# language, Renoise Model is extended with additional method to facilitate its use.
-	A SongIterator to iterate more easily in a Song (on Patterns, Tracks, Notes, Effects…Etc) and facilitate the conversion process of a RenoiseSong.

Future Plans
You are welcome to vote for the following features (or any new features) or even contribute to one of them if you have a good experience in C# and in the following domains:
-	Improve the Xrns2Midi converter (support for xml,  panning of notes…etc.)
-	Add methods on the model to easily Add/Remove Patterns, Tracks, Notes, Effects..etc.
-	Add other useful tools (XM converter… etc.)

** Project Home and contact **
Go to http://www.codeplex.com/nrenoisetools to have more information
Contact author: alexandre_mutel [at] yahoo [dot] fr
Contact on Renoise Forum: alx
