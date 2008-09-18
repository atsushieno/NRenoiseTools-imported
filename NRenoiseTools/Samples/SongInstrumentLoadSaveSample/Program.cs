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
using NRenoiseTools;

namespace SongInstrumentLoadSaveSample
{
    /// <summary>
    /// This is a simple example loading and saving Renoise Song and instrument files.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Song song = new Song();

            // Load a XRNS Song from a file
            song.Load("DemoSong - Diggin for Gold.xrns");

            // Save the first instrument of this song to a file
            song.Instruments[0].Save("MyInstrument.xrni");

            // Load previously instrument 
            Instrument instrument = new Instrument();
            instrument.Load("MyInstrument.xrni");

            // Replace the instrument in the song
            song.Instruments[0] = instrument;

            // Save the Song to another XRNS file
            song.Save("NewDemoSong.xrns");
        }
    }
}
