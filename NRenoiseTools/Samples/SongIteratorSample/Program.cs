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

namespace SongIteratorSample
{
    class Program
    {
        public class DisplaySong : SongIterator
        {
            public DisplaySong(Song song) : base(song) {}

            public void Display(TextWriter log)
            {
                Iterate(new SongIteratorEvent()
                            {
                                OnBeginSong = delegate
                                                  {
                                                      // Access to current Song
                                                      log.WriteLine("Begin Parse Song Name: <{0}>",
                                                                    Song.GlobalSongData.SongName);
                                                  },
                                OnEndSong = delegate
                                                {
                                                    // Access to current Song
                                                    log.WriteLine("End Parse Song");
                                                },
                                OnBeginPattern = delegate
                                                     {
                                                         // Access to current Pattern
                                                         log.WriteLine("Begin Parse Pattern {0}", PatternIndex);
                                                     },
                                OnEndPattern = delegate
                                                   {
                                                       // Access to current Pattern
                                                       log.WriteLine("End Parse Pattern {0}", PatternIndex);
                                                   },
                                OnBeginTrack = delegate
                                                   {
                                                       // Access to current Track, Pattern
                                                       log.WriteLine("\tBegin Parse Track {0} in Pattern {1}",
                                                                     TrackIndex, PatternIndex);
                                                   },
                                OnEndTrack = delegate
                                                 {
                                                     // Access to current Track, Pattern
                                                     log.WriteLine("\tEnd Parse Track {0} in Pattern {1}",
                                                                   TrackIndex, PatternIndex);
                                                 },
                                OnBeginLine = delegate
                                                  {
                                                      // Access to current Line, Track, Pattern
                                                      log.WriteLine(
                                                          "\t\tBegin Parse Line {0} in Pattern {1} in Track {2}",
                                                          Line.index,
                                                          PatternIndex, TrackIndex);
                                                  },
                                OnEndLine = delegate
                                                {
                                                    // Access to current Line, Track, Pattern
                                                    log.WriteLine("\t\tEnd Parse Line {0} in Pattern {1} in Track {2}",
                                                                  Line.index,
                                                                  PatternIndex, TrackIndex);
                                                },
                                OnNote = delegate
                                             {
                                                 // Access to current NoteColumn, Line, Track, Pattern
                                                 log.WriteLine("\t\t  - Pattern {0} Track {1} Note {2} Instrument {3}",
                                                               PatternIndex,
                                                               TrackIndex, NoteColumn.Note, NoteColumn.Instrument);
                                             },
                                OnEffect = delegate
                                               {
                                                   // Access to current EffectColumn, Line, Track, Pattern
                                                   log.WriteLine(
                                                       "\t\t  - Pattern {0} Track {1} TrackType {2} Effect {3}={4} ",
                                                       PatternIndex,
                                                       TrackIndex, Track.GetType().Name, EffectColumn.Number, EffectColumn.Value);
                                               },
                            });
            }
        }

        static void Main(string[] args)
        {
            Song song = new Song();
            // Load a XRNS Song from a file
            song.Load("DemoSong - Diggin for Gold.xrns");

            DisplaySong displaySongy= new DisplaySong(song);
            displaySongy.Display(Console.Out);

            Console.WriteLine("Press any key");
            Console.ReadKey(true);
        }
    }
}
