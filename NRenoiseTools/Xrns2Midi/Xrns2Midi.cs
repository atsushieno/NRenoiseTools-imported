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
using System.Collections.Generic;
using System.IO;
using NAudio.Midi;
using NRenoiseTools;
using Song = NRenoiseTools.RenoiseSong;
using Instrument = NRenoiseTools.RenoiseInstrument;

namespace NRenoiseTools.Xrns2MidiApp
{
    /// <summary>
    /// Main class that performs convertion from XRNS to MIDI files.
    /// See RenoiseMidiSong.ConvertToMidi methods
    /// </summary>
    class Xrns2Midi 
    {
        /// <summary>
        /// Simple association of a midi channel and a patch.
        /// </summary>
        class RenoiseMidiInstrument
        {
            public int channel;
            public int patch;
            public int shiftBaseNote;
        }

        /// <summary>
        /// Main convertion class. Use RenoiseModel.RenoiseSongIterator to facilitate iterations on the song patterns, notes, effects...etc.
        /// </summary>
        class RenoiseMidiSong : SongIterator
        {
            public const int MidiDivision = 96;
            private RenoiseMidiInstrument[] instruments;
            private MidiEventCollection midiEvents;
            private Dictionary<int, NoteOnEvent> lastEvents;
            private TextWriter log;

            public TextWriter Log
            {
                get { return log; }
                set { log = value; }
            }

            private TempoEvent CalculateTempoEvent(SongTempoMarker marker)
            {
                return new TempoEvent((int)marker.TimePerBeat, (int)marker.BeatPosition * MidiDivision);
            }

            private int CalculateMidiTimeForPosition(int position)
            {
                return (int)(Tempo.GetBeatFromOffset(position) * MidiDivision);             
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="RenoiseMidiSong"/> class.
            /// </summary>
            /// <param name="song">The renoise song.</param>
            public RenoiseMidiSong(Song song) : base(song)
            {
                Log = Console.Out;

                // Build a collection of MidiEvents of Midi file type 2.
                midiEvents = new MidiEventCollection(2, MidiDivision);

                // Add midi events for all tempos in the song
                foreach (SongTempoMarker tempo in Tempos)
                {
                    Log.WriteLine("Find tempo Bpm: {0} Lpb: {1} at Absolute Position: {2}", tempo.Bpm, tempo.Lpb, tempo.Position);
                    midiEvents.AddEvent(CalculateTempoEvent(tempo), 0);
                }

                // Calculate the maximum number of column
                int maxColumn = 0;
                foreach (SequencerTrack trackDef in Song.Tracks.SequencerTrack)
                {
                    maxColumn = Math.Max(maxColumn, trackDef.NumberOfVisibleNoteColumns);
                }
                lastEvents = new Dictionary<int, NoteOnEvent>();
            }

            /// <summary>
            /// Assigns the midi instruments. This method iterate all other the song and mark all instruments
            /// used. Then, if the instrument is a midi instrument, it will take the channel and patch from this
            /// instrument otherwhise, it will allocate the first available channel for this instrument.
            /// </summary>
            private void AssignMidiInstruments()
            {
                Log.WriteLine("Assign Instruments to channels");

                instruments = new RenoiseMidiInstrument[Song.Instruments.Instrument.Length];
                // Init channels marker with -1
                int[] channels = new int[16];
                for (int i = 0; i < channels.Length; i++)
                {
                    channels[i] = -1;
                }

                // Iterate on all note in the song to retrieve the instrument used
                Iterate(new SongIteratorEvent()
                {
                    //OnBeginPattern = delegate
                    //                     {
                    //                         String midiPatternName = String.Format("Pattern [{0}] : {1} {2}", PatternIndex, Pattern.Name);
                    //                         TextEvent textEvent = new TextEvent("tt",MetaEventType.SequenceTrackName,0);
                                             
                    //                         //textEvent.Channel
                    //                         //midiEvents.AddEvent( new TextEvent(²"tt",MetaEventType.SequenceTrackName,0) );
                    //                         //$master->addEventArr($lines,0, 0,11,array('type'=>'Marker', 'string'=>"Pattern [$pn]: $patternid $patternName"));
      
                    //                     },
                    OnNote = delegate
                                 {
                                     if (NoteColumn.Instrument != null)
                                     {
                                         int instrumentIndex = Convert.ToInt32(NoteColumn.Instrument, 16);
                                         if (instruments[instrumentIndex] == null)
                                         {
                                             RenoiseMidiInstrument midiInstrument = new RenoiseMidiInstrument();
                                             instruments[instrumentIndex] = midiInstrument;

                                             Instrument instrument = Song.Instruments.Instrument[instrumentIndex];

                                             // If midi instrument, then take the affected channel and patch
                                             if (instrument.MidiOutputProperties.IsActive)
                                             {

                                                 midiInstrument.channel = instrument.MidiOutputProperties.Channel;
                                                 midiInstrument.patch = instrument.MidiOutputProperties.Program;
                                                 Log.WriteLine("\tInstrument Midi N°{0} is affected to channel {1}", instrumentIndex, midiInstrument.channel);

                                                 // Don't know why there is an octave shift on BaseNote for Midi instruments?
                                                 midiInstrument.shiftBaseNote = instrument.MidiOutputProperties.Transpose -
                                                                                NoteHelper.ConvertToNumber("C-4") - 12;
                                                 if (midiInstrument.channel > 0)
                                                 {
                                                     channels[midiInstrument.channel - 1] = instrumentIndex;
                                                 }
                                             }
                                             else
                                             {
                                                 // else leave the instrument unsassigned (may be a VST or a sample?)
                                                 // Assign after this loop
                                                 midiInstrument.channel = -1;
                                                 midiInstrument.patch = 0;
                                                 // TODO: How to select BaseNote on samples?
                                                 midiInstrument.shiftBaseNote = instrument.PluginProperties.Transpose -
                                                                                NoteHelper.ConvertToNumber("C-4");
                                             }
                                         }
                                     }
                                 }
                });

                int cyclicChannel = 0;
                // Affect channels for instrument that are not midi
                for (int i = 0; i < instruments.Length; i++)
                {
                    RenoiseMidiInstrument instrument = instruments[i];
                    if (instrument != null)
                    {
                        if (instrument.channel < 0)
                        {
                            // Look for first available channel
                            for (int j = 0; j < channels.Length; j++)
                            {
                                int channel = channels[j];
                                // Affect first channel available
                                if (channel == -1)
                                {
                                    instrument.channel = j + 1;
                                    channels[j] = j + 1;
                                    Log.WriteLine("\tStandard Instrument N°{0} is affected to channel {1}", i, instrument.channel);
                                    break;
                                }
                            }
                        } 

                        // If instrument channel is still unaffected, there is not enough channel available
                        // Throws an exception
                        if (instrument.channel < 0)
                        {
                            instrument.channel = cyclicChannel + 1;
                            Log.WriteLine("\tWarning, unable to affect a channel for Instrument N°{0} (Name: {1}). Reuse channel N°{2} already used.", i, Song.Instruments.Instrument[i].Name, instrument.channel);
                            cyclicChannel = (cyclicChannel + 1) % 16;
                        }

                        // If patch is affected. Send MIDI Patch Change
                        if (instrument.patch > 0)
                        {
                            Log.WriteLine("\tInstrument N°{0} change patch to {1}", i, instrument.patch);
                            PatchChangeEvent patchChangeEvent = new PatchChangeEvent(0, instrument.channel,
                                                                                     instrument.patch);
                            // Strange, need to affect patch again (bug in NAudio?)
                            patchChangeEvent.Patch = instrument.patch;
                            midiEvents.AddEvent(patchChangeEvent, 0);
                        }
                    }
                }
            }

            /// <summary>
            /// Main method that converts the XRNS to MIDI events.
            /// </summary>
            private void ConvertToMidi()
            {
                // Assign all instruments and channels
                AssignMidiInstruments();

                // Tempo of current line : updated by OnBeginLine delegate
                int timeForCurrentPosition = 0;
                int endTimeOfSong = 0;

                Log.WriteLine("Convert patterns");

                Iterate(new SongIteratorEvent
                {
                    OnEndSong = delegate
                                    {
                                        endTimeOfSong = CalculateMidiTimeForPosition(FirstLineIndexOfCurrentPatternInSong);
                                    },
                    OnBeginPattern = delegate
                                    {
                                        Log.WriteLine("\tConvert Pattern {0}", PatternIndex);
                                    },
                    OnBeginLine = delegate
                                      {
                                          timeForCurrentPosition = CalculateMidiTimeForPosition(LineIndexInSong);
                                      },
                    OnNote = delegate
                                 {
                                     int absColumn = TrackIndex * 1024 + ColumnIndex;
                                     RenoiseMidiInstrument instrument = (NoteColumn.Instrument != null)
                                                                            ?
                                                                                instruments[
                                                                                    Convert.ToInt32(
                                                                                        NoteColumn.
                                                                                            Instrument, 16)]
                                                                            : null;
                                     int channel = (instrument != null) ? instrument.channel : 0;
                                     int noteNumber = NoteHelper.ConvertToNumber(NoteColumn.Note);
                                     if (instrument != null) noteNumber += instrument.shiftBaseNote;

                                     string volumeInHexa = NoteColumn.Volume ?? "7F";
                                     int velocity = Convert.ToInt32(volumeInHexa, 16);

                                     NoteOnEvent lastNoteOnEvent;
                                     lastEvents.TryGetValue(absColumn, out lastNoteOnEvent);

                                     // NoteOff
                                     if (NoteColumn.Note == "OFF")
                                     {
                                         if (lastNoteOnEvent != null)
                                         {
                                             lastNoteOnEvent.NoteLength = (int)(timeForCurrentPosition - lastNoteOnEvent.AbsoluteTime);
                                             midiEvents.AddEvent(lastNoteOnEvent.OffEvent, 0);
                                             lastEvents.Remove(absColumn);
                                         }
                                     }
                                     else if (noteNumber >= 0)
                                     {
                                         NoteOnEvent noteOnEvent;
                                         // NoteOn
                                         if (lastNoteOnEvent == null)
                                         {
                                             // If new note and no older note on the same column
                                             noteOnEvent = new NoteOnEvent(timeForCurrentPosition, channel,
                                                                           noteNumber,
                                                                           velocity, 0);
                                             // Add new event to last note
                                             lastEvents.Add(absColumn, noteOnEvent);
                                         }
                                         else
                                         {
                                             // If new note and there is an older note on the same column
                                             // NoteOff by note
                                             lastNoteOnEvent.NoteLength = (int)(timeForCurrentPosition - lastNoteOnEvent.AbsoluteTime);
                                             // Add Off event for last note
                                             midiEvents.AddEvent(lastNoteOnEvent.OffEvent, 0);

                                             noteOnEvent = new NoteOnEvent(timeForCurrentPosition, channel,
                                                                           noteNumber,
                                                                           velocity, 0);

                                             // Replace last event with new note
                                             lastEvents[absColumn] = noteOnEvent;
                                         }
                                         midiEvents.AddEvent(noteOnEvent, 0);
                                     }
                                 }
                });


                // Send NoteOff on all last midinote opened
                foreach (NoteOnEvent lastNoteOnEvent in lastEvents.Values)
                {
                    lastNoteOnEvent.NoteLength = (int)(endTimeOfSong - lastNoteOnEvent.AbsoluteTime);
                    midiEvents.AddEvent(lastNoteOnEvent.OffEvent, 0);
                }

                // Prepare Midi events for export
                midiEvents.PrepareForExport();
            }

            /// <summary>
            /// Exports the Renoise song to the specified midi file.
            /// </summary>
            /// <param name="midiFile">The midi file.</param>
            public void export(string midiFile)
            {
                Log.WriteLine("Start convertion of RenoiseSong to Midi");
                ConvertToMidi();
                Log.WriteLine("Export to midi file <{0}>", midiFile);
                MidiFile.Export(midiFile, midiEvents);
                Log.WriteLine("End convertion of RenoiseSong to Midi");
            }
        }

        public static bool ConvertFile(String xrnsFile, String midiFile)
        {
            return ConvertFile(xrnsFile, midiFile, null);
        }


        /// <summary>
        /// Main method to covnert XNR FILE TO an output file
        /// </summary>
        /// <param name="xrnsFile">The XRNS file.</param>
        /// <param name="midiFile">The midi file.</param>
        public static bool ConvertFile(String xrnsFile, String midiFile, TextWriter log)
        {
            bool isConversionOk = true;
            try
            {
                Song renoiseSong = new Song();
                renoiseSong.Load(xrnsFile);

                RenoiseMidiSong midiSong = new RenoiseMidiSong(renoiseSong);
                if (log != null)
                {
                    midiSong.Log = log;
                }
                midiSong.export(midiFile);
            } catch (Exception ex)
            {
                isConversionOk = false;
                log.WriteLine("Exception occured {0}", ex);
            }
            return isConversionOk;
        }
    }
}
