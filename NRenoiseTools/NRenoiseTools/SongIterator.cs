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
using Song = NRenoiseTools.RenoiseSong;
using Instrument = NRenoiseTools.RenoiseInstrument;

namespace NRenoiseTools
{
    /// <summary>
    /// Structures used by <see cref="SongIterator.Iterate"/> and <see cref="SongIterator.IteratePattern"/>.
    /// This structure provides callbacks through delegates.
    /// </summary>
    public struct SongIteratorEvent
    {
        public delegate void OnIteratorEvent();

        public OnIteratorEvent OnBeginPattern;
        public OnIteratorEvent OnEndPattern;
        public OnIteratorEvent OnBeginTrack;
        public OnIteratorEvent OnEndTrack;
        public OnIteratorEvent OnBeginLine;
        public OnIteratorEvent OnEndLine;
        public OnIteratorEvent OnBeginSong;
        public OnIteratorEvent OnEndSong;
        public OnIteratorEvent OnNote;
        public OnIteratorEvent OnEffect;
    }
  
    /// <summary>
    /// Convenient class helper to iterate on a Renoise song. Subclass this class and use Iterate with delegates to catch events.
    /// <code>
    /// public class MyRenoiseIterator : SongIterator {
    ///     public MyRenoiseIterator(Song song) : base(song) {}
    /// 
    ///     public void DoMyConvertion() {
    ///         Iterate(new RenoiseSongEvent()
    ///         {
    ///             OnNote = delegate
    ///             {
    ///                 // do whatever you want to do here.
    ///                 // Look at Pattern, Track, Line, NoteColumn properties... to get the current pattern, track, line...etc
    ///             },
    ///             OnBeginPattern = delegate { Console.WriteLine("New pattern {0}", Pattern.Name); }
    ///         });
    ///     }
    /// </code>
    /// </summary>
    public class SongIterator
    {
        private Song song;
        private int patternSequenceIndex;
        private int patternIndex;
        private Pattern pattern;
        private int firstLineIndexOfCurrentPatternInSong;
        private int trackIndex;
        private Track track;
        private int lineIndex;
        private int lineIndexInTrack;
        private int lineIndexInSong;
        private PatternTrackLineNode line;
        private int columnIndex;
        private PatternTrackNoteColumnNode noteColumn;
        private PatternTrackEffectColumnNode effectColumn;

        private Stack<SongIterator> stateStackList;

        private SongTempoMarker[] nextTempoForPattern;

        /// <summary>
        /// Initializes a new instance of the <see cref="SongIterator"/> class.
        /// </summary>
        /// <param name="song">The renoise song.</param>
        public SongIterator(Song song)
        {
            this.song = song;
            patternSequenceIndex = 0;
            stateStackList = new Stack<SongIterator>();
        }

        /// <summary>
        /// Gets the song being processed.
        /// </summary>
        /// <value>The song.</value>
        public Song Song
        {
            get { return song; }
        }

        /// <summary>
        /// Gets the index of the current pattern.
        /// </summary>
        /// <value>The index of the pattern.</value>
        public int PatternIndex
        {
            get { return patternIndex; }
        }

        /// <summary>
        /// Gets the current pattern.
        /// </summary>
        /// <value>The pattern.</value>
        public Pattern Pattern
        {
            get { return pattern; }
        }

        /// <summary>
        /// Gets the index of the current track.
        /// </summary>
        /// <value>The index of the track.</value>
        public int TrackIndex
        {
            get { return trackIndex; }
        }

        /// <summary>
        /// Gets the current track.
        /// </summary>
        /// <value>The track.</value>
        public Track Track
        {
            get { return track; }
        }

        /// <summary>
        /// Gets the line index in the current track.
        /// </summary>
        /// <value>The line index in track.</value>
        public int LineIndexInTrack
        {
            get { return lineIndexInTrack; }
        }

        /// <summary>
        /// Gets the absolute line index in song (starting from the first pattern)
        /// </summary>
        /// <value>The line index in song.</value>
        public int LineIndexInSong
        {
            get { return lineIndexInSong; }
        }

        /// <summary>
        /// Gets the current line.
        /// </summary>
        /// <value>The line.</value>
        public PatternTrackLineNode Line
        {
            get { return line; }
        }

        /// <summary>
        /// Gets the current index of the column.
        /// </summary>
        /// <value>The index of the column.</value>
        public int ColumnIndex
        {
            get { return columnIndex; }
        }

        /// <summary>
        /// Gets the current note column.
        /// </summary>
        /// <value>The note column.</value>
        public PatternTrackNoteColumnNode NoteColumn
        {
            get { return noteColumn; }
        }

        /// <summary>
        /// Gets the current effect column.
        /// </summary>
        /// <value>The effect column.</value>
        public PatternTrackEffectColumnNode EffectColumn
        {
            get { return effectColumn; }
        }

        /// <summary>
        /// Gets the first line index of current pattern in song (absolute position).
        /// </summary>
        /// <value>The first line index of current pattern in song.</value>
        public int FirstLineIndexOfCurrentPatternInSong
        {
            get { return firstLineIndexOfCurrentPatternInSong; }
        }

        /// <summary>
        /// Pushes the state of this iterator.
        /// </summary>
        private void PushState()
        {
            stateStackList.Push(Clone());
        }

        /// <summary>
        /// Pops the state of this iterator.
        /// </summary>
        private void PopState()
        {
            SongIterator savedState = stateStackList.Pop();
            ResetToState(savedState);
        }

        /// <summary>
        /// Resets the pattern level.
        /// </summary>
        private void ResetPatternLevel()
        {
            // Set to Null
            trackIndex = -1;
            track = null;
            line = null;
            lineIndex = 0;
            lineIndexInTrack = -1;
            lineIndexInSong = firstLineIndexOfCurrentPatternInSong;
            columnIndex = -1;
            noteColumn = null;
            effectColumn = null;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        private SongIterator Clone()
        {
            return (SongIterator)MemberwiseClone();            
        }

        /// <summary>
        /// Copy states from an iterator to the current instance.
        /// </summary>
        /// <param name="copyState">State to copy.</param>
        private void ResetToState(SongIterator copyState)
        {           
            song = copyState.song;
            patternSequenceIndex = copyState.patternSequenceIndex;
            patternIndex = copyState.patternIndex;
            pattern = copyState.pattern;
            firstLineIndexOfCurrentPatternInSong = copyState.firstLineIndexOfCurrentPatternInSong;
            trackIndex = copyState.trackIndex;
            track = copyState.track;
            lineIndex = copyState.lineIndex;
            lineIndexInTrack = copyState.lineIndexInTrack;
            lineIndexInSong = copyState.lineIndexInSong;
            line = copyState.line;
            columnIndex = copyState.columnIndex;
            noteColumn = copyState.noteColumn;
            effectColumn = copyState.effectColumn;
        }

        /// <summary>
        /// Iterates on the song using the RenoiseSongEvent callbacks.
        /// </summary>
        /// <param name="it">It.</param>
        public void Iterate(SongIteratorEvent it)
        {
            PushState();
            if (it.OnBeginSong != null) it.OnBeginSong();
            firstLineIndexOfCurrentPatternInSong = 0;
                // Iterate on PatternSequence and retrieve the instruments used
            patternSequenceIndex = 0;
            for (; patternSequenceIndex < song.PatternSequence.SequenceEntries.SequenceEntry.Length; patternSequenceIndex++)
            {
                patternIndex = song.PatternSequence.SequenceEntries.SequenceEntry[patternSequenceIndex].Pattern;
                IteratePatternInternal(patternIndex, it);                
            }
            if (it.OnEndSong != null) it.OnEndSong(); 
            PopState();
        }

        /// <summary>
        /// Iterates on a specific pattern using the RenoiseSongEvent callbacks.
        /// </summary>
        /// <param name="patternIndexArg">The pattern index arg.</param>
        /// <param name="it">It.</param>
        public void IteratePattern(int patternIndexArg, SongIteratorEvent it)
        {
            PushState();
            IteratePatternInternal(patternIndexArg, it);
            PopState();
        }

        /// <summary>
        /// Internal method that iterates the pattern and calls the callbacks.
        /// </summary>
        /// <param name="patternIndexArg">The pattern index arg.</param>
        /// <param name="it">It.</param>
        private void IteratePatternInternal(int patternIndexArg, SongIteratorEvent it)
        {
            ResetPatternLevel();
            patternIndex = patternIndexArg;
            pattern = song.PatternPool.Patterns.Pattern[patternIndex];

            // Update Current Line Index
            lineIndexInSong = firstLineIndexOfCurrentPatternInSong;

            // Notify Event On Begin Pattern
            if (it.OnBeginPattern != null) it.OnBeginPattern();       

            trackIndex = 0;
            // Iterate on Pattern Tracks
            for (int i = 0; i < pattern.Tracks.PatternTrack.Length; i++, trackIndex++)
            {
                track = pattern.Tracks.PatternTrack[i];
                IterateTrack(it);
            }

            // Iterate on Pattern SendTracks
            if ( pattern.Tracks.PatternSendTrack != null )
                for (int i = 0; i < pattern.Tracks.PatternSendTrack.Length; i++, trackIndex++)
                {
                    track = pattern.Tracks.PatternSendTrack[i];
                    IterateTrack(it);
                }

            // Iterate on Pattern MasterTracks
            for (int i = 0; i < pattern.Tracks.PatternMasterTrack.Length; i++, trackIndex++)
            {
                track = pattern.Tracks.PatternMasterTrack[i];
                IterateTrack(it);
            }

            // Notify event On End Pattern
            if (it.OnEndPattern != null) it.OnEndPattern();

            firstLineIndexOfCurrentPatternInSong += pattern.NumberOfLines;
            lineIndexInSong = firstLineIndexOfCurrentPatternInSong;
            lineIndexInTrack = 0;
        }

        private void IterateTrack(SongIteratorEvent it)
        {
            lineIndex = 0;
            lineIndexInTrack = 0;
            line = null;
            columnIndex = 0;
            noteColumn = null;
            effectColumn = null;
            // Notify event
            if (it.OnBeginTrack != null) it.OnBeginTrack();

            // Iterate on lines
            if (track.Lines != null)
                for (lineIndex = 0; lineIndex < track.Lines.Length; lineIndex++)
                {
                    line = track.Lines[lineIndex];
                    if (line.index >= pattern.NumberOfLines)
                    {
                        break;
                    }
                    lineIndexInTrack = line.index;
                    lineIndexInSong = firstLineIndexOfCurrentPatternInSong + lineIndexInTrack;

                    // Notify event
                    if (it.OnBeginLine != null) it.OnBeginLine();

                    // Iterate on Note columns
                    if (line.NoteColumns != null && it.OnNote != null) 
                    {
                        effectColumn = null;
                        for (columnIndex = 0; columnIndex < line.NoteColumns.NoteColumn.Length; columnIndex++)
                        {
                            noteColumn = line.NoteColumns.NoteColumn[columnIndex];
                            it.OnNote();
                        }
                    }

                    // Iterate on Effect columns
                    if (line.EffectColumns != null && it.OnEffect != null) 
                    {
                        noteColumn = null;
                        for (columnIndex = 0; columnIndex < line.EffectColumns.EffectColumn.Length; columnIndex++)
                        {
                            effectColumn = line.EffectColumns.EffectColumn[columnIndex];
                            // Notify event
                            it.OnEffect();
                        }
                    }
                    if (it.OnEndLine != null) it.OnEndLine();
                }
            
            // Notify event
            if (it.OnEndTrack != null) it.OnEndTrack();
        }


        /// <summary>
        /// Internal tempo event used to parse effect commands on a line.
        /// </summary>
        private class InternalTempoEvent : IComparable
        {
            public int pos;
            public int col;
            public float bpm;
            public int lpb;
            public InternalTempoEvent(int col, int pos, float bpm, int lpb)
            {
                this.col = col;
                this.pos = pos;
                this.bpm = bpm;
                this.lpb = lpb;
            }

            public int CompareTo(object obj)
            {
                InternalTempoEvent against = (InternalTempoEvent)obj;
                int value = pos - against.pos;
                // If same line, then order by column
                if (value == 0)
                {
                    value = col - against.col;
                }
                return value;
            }
        }

        private void prepareTempoForSong()
        {
            // Initialize first tempo marker with global settings
            // TODO: CONVERT Beat from int to float in SongTempoMarker
            SongTempoMarker currentTempo = new SongTempoMarker(Song.GlobalSongData.BeatsPerMin,
                                                                           Song.GlobalSongData.LinesPerBeat, 0, 0, 0);

            List<InternalTempoEvent> tempos = new List<InternalTempoEvent>();

            // Iterate on all effects of current pattern
            Iterate(new SongIteratorEvent
            {
                OnEffect = delegate
                {
                    // Fake colNumber to sort correctly
                    int colNumber = TrackIndex * 1000 + ColumnIndex;
                    if (EffectColumn.Number == "00")
                    {
                        return;
                    }
                    int effectNumber = Convert.ToInt32(EffectColumn.Number, 16);
                    int effectValue = Convert.ToInt32(EffectColumn.Value, 16);
                    InternalTempoEvent newInternalTempo = null;

                    switch (effectNumber)
                    {
                        // New BPM
                        case 0xF0:
                            if (effectValue >= 0x20 && effectValue <= 0xFF)
                            {

                                newInternalTempo = new InternalTempoEvent(
                                        colNumber,
                                        LineIndexInSong,
                                        effectValue, -1);
                            }
                            break;
                        // New Lpb
                        case 0xF1:

                            if (effectValue > 0x00 && effectValue <= 0x1F)
                            {
                                newInternalTempo = new InternalTempoEvent(
                                        colNumber,
                                        LineIndexInSong, -1,
                                        effectValue);
                            }
                            break;
                    }
                    if (newInternalTempo != null) tempos.Add(newInternalTempo);
                }
            });

            List<SongTempoMarker> temposForCurrentSong = new List<SongTempoMarker>();
            // Add the current tempo at the beginning of the new pattern
            temposForCurrentSong.Add(currentTempo);

            // We have tempo in the song
            if (tempos.Count > 0)
            {
                tempos.Sort();
                SongTempoMarker lastTempo = currentTempo;
                for (int i = 0; i < tempos.Count; i++)
                {
                    InternalTempoEvent internalTempoEvent = tempos[i];
                    InternalTempoEvent bpmEvent = null;
                    InternalTempoEvent lpbEvent = null;
                    // Iterate on same line
                    for (int j = i; j < tempos.Count && tempos[j].pos == internalTempoEvent.pos; j++, i++)
                    {
                        // Change in Bpm?
                        if (tempos[j].bpm > 0)
                        {
                            bpmEvent = tempos[j];
                        }
                        else
                        {
                            // Change in lpb
                            lpbEvent = tempos[j];
                        }
                    }
                    i--;
                    float bpm = (bpmEvent != null) ? bpmEvent.bpm : lastTempo.Bpm;
                    int lpb = (lpbEvent != null) ? lpbEvent.lpb : lastTempo.Lpb;

                    // Get time for this marker (use last tempo marker to get the time offset)
                    SongTempoMarker lastTempoMarker = temposForCurrentSong[temposForCurrentSong.Count - 1];

                    double newPositionTime = lastTempoMarker.GetTimeFromOffset(internalTempoEvent.pos);
                    double newPositionBeat = lastTempoMarker.GetBeatFromOffset(internalTempoEvent.pos);


                    SongTempoMarker newTempo = new SongTempoMarker(bpm, lpb,
                                                                                 internalTempoEvent.pos, newPositionTime, newPositionBeat);
      
                    // May be applied to first tempo in song
                    if (lastTempo.Position == newTempo.Position)
                    {
                        temposForCurrentSong[temposForCurrentSong.Count - 1] = newTempo;
                    }
                    else
                    {
                        temposForCurrentSong.Add(newTempo);
                    }
                    lastTempo = newTempo;
                }
            }
            nextTempoForPattern = temposForCurrentSong.ToArray();
        }


        /// <summary>
        /// Gets the current tempo.
        /// </summary>
        /// <value>The tempo.</value>
        public SongTempoMarker Tempo
        {
            get
            {
                return FindTempoMarkerFromPosition(LineIndexInSong);
            }
        }


        /// <summary>
        /// Gets the current position in beat.
        /// </summary>
        /// <value>The position beat.</value>
        public double PositionBeat
        {
            get
            {
                return Tempo.GetBeatFromOffset(LineIndexInSong);
            }
        }

        /// <summary>
        /// Gets the current position in time (ms).
        /// </summary>
        /// <value>The position time.</value>
        public double PositionTime
        {
            get
            {
                return Tempo.GetTimeFromOffset(LineIndexInSong);
            }
        }

        /// <summary>
        /// Finds the tempo marker from an absolute position in the song.
        /// This method must be called after prepareTempoForPattern() for the current pattern.
        /// </summary>
        /// <param name="positionInSong">The position inf the song</param>
        /// <returns>The tempo marker that affects this position</returns>
        private SongTempoMarker FindTempoMarkerFromPosition(int positionInSong)
        {
            // If first time we ask for tempo, then, get all tempos for the song
            if ( nextTempoForPattern == null )
            {
                prepareTempoForSong();
            }
            int i = 0;
            for (; i < nextTempoForPattern.Length; i++)
            {
                SongTempoMarker tempo = nextTempoForPattern[i];
                if (positionInSong < tempo.Position)
                {
                    break;
                }
            }
            return nextTempoForPattern[Math.Max(i - 1, 0)];
        }

        /// <summary>
        /// Gets all the tempos in the song.
        /// </summary>
        /// <value>The tempos.</value>
        public SongTempoMarker[] Tempos
        {
            get
            {
                if (nextTempoForPattern == null)
                {
                    prepareTempoForSong();
                }
                return nextTempoForPattern;
            }
        }

    }
}

