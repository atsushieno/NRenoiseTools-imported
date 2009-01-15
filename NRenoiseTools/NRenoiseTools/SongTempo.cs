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

namespace NRenoiseTools
{
    /// <summary>
    /// Tempo marker inside the song with the absolute position (in line and time) in the song.
    /// </summary>
    public class SongTempoMarker : SongTempo
    {
        private int position;
        private double timePosition;
        private double beatPosition;

        public SongTempoMarker(float bpm, int speed, int positionInSong, double timePosition, double beatPosition)
            : base(bpm, speed)
        {
            this.position = positionInSong;
            this.timePosition = timePosition;
            this.beatPosition = beatPosition;
        }

        public double GetTimeFromOffset(int offset)
        {
            return timePosition + (offset - position) * TimePerBeat / Lpb;
        }

        public double GetBeatFromOffset(int offset)
        {
            return beatPosition + (offset - position) * 1.0 / Lpb;
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public int Position
        {
            get { return position; }
        }

        /// <summary>
        /// Gets the beat position of this tempo.
        /// </summary>
        /// <value>The beat position.</value>
        public double BeatPosition
        {
            get { return beatPosition; }
        }

        /// <summary>
        /// Gets the time position in micro seconds of this tempo.
        /// </summary>
        /// <value>The time position.</value>
        public double TimePosition
        {
            get { return timePosition; }
        }
    }

    public class SongTempo
    {
        private float bpm;
        private int speed;
        private int lpb;
        private float realbpm;

        public SongTempo(float bpm, int speed)
        {
            this.bpm = bpm;
            this.speed = speed;
            updateRealBpm();
        }

        public float Bpm
        {
            get { return bpm; }
            set { bpm = value; updateRealBpm(); }
        }

        public int Speed
        {
            get { return speed; }
            set { speed = value; updateRealBpm(); }
        }

        public int Lpb
        {
            get { return lpb; }
        }

        public float Realbpm
        {
            get { return realbpm; }
        }

        private void updateRealBpm()
        {
            realbpm = (speed == 1 || speed == 2 || speed == 3 || speed == 6 || speed == 12) ? bpm : ((int)Math.Round(6.0 / speed * bpm, 0));
            lpb = LPBFromSpeed(speed);
        }

        private static int LPBFromSpeed(int speed)
        {
            int lpb = 0;
            if (speed > 3)
            {
                lpb = 4;
            }
            else
            {
                switch (speed)
                {
                    case 3:
                        lpb = 8;
                        break;
                    case 2:
                        lpb = 12;
                        break;
                    case 1:
                        lpb = 24;
                        break;
                }
            }
            return lpb;
        }

        public double TimePerBeat
        {
            get
            {
                return Math.Floor(60000000.0/Realbpm);
            }
        }
    }
}
