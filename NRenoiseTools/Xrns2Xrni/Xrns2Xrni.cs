// Copyright 2008 Alexandre Mutel
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.IO;
using NRenoiseTools;

namespace NRenoiseTools.Xrns2XrniApp
{
    class Xrns2Xrni
    {
        public Xrns2Xrni()
        {
            log = Console.Out;
        }

        private TextWriter log;

        public TextWriter Log
        {
            get { return log; }
            set { log = value; }
        }

        public void Extract(string xrnsFile)
        {
            Song song = new Song();
            try
            {
                Log.WriteLine("Open XRNS song file <{0}> for extraction:", xrnsFile);
                song.Load(xrnsFile);

                string songFileNameWithoutExt = (song.FileName == null)
                                                    ? (song.GlobalSongData.SongName ?? "RenoiseSong")
                                                    : Path.GetFileNameWithoutExtension(song.FileName);

                for (int i = 0; i < song.Instruments.Length; i++)
                {
                    Instrument instrument = song.Instruments[i];
                    string instrumentName = string.Format("{0}-Inst{1:D2} ({2}).xrni", songFileNameWithoutExt, i,
                                                          instrument.Name);
                    Log.WriteLine("\tExtract instrument and save XRNI to <{0}>", instrumentName);
                    instrument.Save(instrumentName);
                }

            }
            catch (ArgumentException ex)
            {
                Log.WriteLine("Unable to load XRNS song from: <{0}>", xrnsFile);
            }
        }

        public void Extract(string[] xrnsFiles)
        {
            Log.WriteLine("Begin XRNI extraction from XRNS files");
            foreach (string xrnsFile in xrnsFiles)
                Extract(xrnsFile);
            Log.WriteLine("End XRNI extraction from XRNS files");
        }
    }
}
