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
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;

namespace NRenoiseTools
{
    /// <summary>
    /// Main Renoise Song class. Use this class to create and manipulate Renoise Song.
    /// </summary>
    partial class Song
    {
        private string fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Song"/> class.
        /// </summary>
        public Song()
        {
            MethodBase methodBase = new StackTrace().GetFrame(1).GetMethod();
            // If 
            if ( ! methodBase.IsConstructor || ! (methodBase.DeclaringType == typeof(RenoiseSong)) )
            {
                Load(new MemoryStream(Properties.Resources.EmptySong));        
            }            
        }

        /// <summary>
        /// Gets or sets the name of the file used to store the RenoiseSong. This property is set when
        /// loading song from a file <see cref="Load(string)"/>.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        /// <summary>
        /// Copies this song to another song.
        /// </summary>
        /// <param name="toSong">song to copy to.</param>
        public void Copy(Song toSong)
        {
            Util.Copy(this, toSong);
        }

        /// <summary>
        /// Loads a xrns Renoise song from a specified file name.
        /// </summary>
        /// <param name="fileNameArg">Name of the xrns Renoise file.</param>
        public void Load(string fileNameArg)
        {
            fileName = fileNameArg;
            Load(new FileStream(fileName, FileMode.Open, FileAccess.Read));
        }

        /// <summary>
        /// Loads a xrns Renoise song from a specified stream.
        /// </summary>
        /// <param name="xrnsInputStream">The XRNS input stream.</param>
        public void Load(Stream xrnsInputStream)
        {
            // Open ZipFile
            ZipFile zipFile = Util.OpenZip(xrnsInputStream);

            // Get Song.xml from xrns archive
            ZipEntry zipEntry = zipFile.GetEntry("Song.xml");
            if (zipEntry == null)
                throw new ArgumentException("Invalid XRNS song file. No Song.xml in the archive");

            StreamReader songStream = new StreamReader(zipFile.GetInputStream(zipEntry));

            // Use generated Serializer
            XmlSerializer xSerializer = new Microsoft.Xml.Serialization.GeneratedAssembly.RenoiseSongSerializer();

            // Deserialize Song.xml
            RenoiseSong song = (RenoiseSong)xSerializer.Deserialize(songStream);

            // Close the stream from the zip
            songStream.Close();

            // Copy the properties to this instance
            string savedFileName = FileName;
            song.Copy(this);
            FileName = savedFileName;

            // Set all buffer Samples to null
            foreach (Instrument instrument in Instruments) {
                foreach (Sample sample in instrument.Samples)
                {
                    sample.Buffer = null;
                }
            }

            IEnumerator enumSamples = zipFile.GetEnumerator();
            while (enumSamples.MoveNext())
            {
                ZipEntry sampleEntry = (ZipEntry)enumSamples.Current;
                if (sampleEntry.IsFile && sampleEntry.Name.StartsWith("SampleData"))
                {
                    string[] pathNames = sampleEntry.Name.Split('/');
                    string instrumentName = pathNames[1];
                    string sampleName = pathNames[2];
                    int instrumentIndex = int.Parse(instrumentName.Substring("Instrument".Length, 3));
                    int sampleIndex = int.Parse(sampleName.Substring("Sample".Length, 3));
                    Instruments[instrumentIndex].Samples[sampleIndex].LoadBufferFromZip(zipFile, sampleEntry);
                }
            }
            xrnsInputStream.Close();
        }

        /// <summary>
        /// Writes this xrns Renoise song to a specified output file.
        /// </summary>
        public void Save()
        {
            Save(fileName);
        }

        /// <summary>
        /// Writes this xrns Renoise song to a specified output file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void Save(string fileName)
        {
            Save(new FileStream(fileName, FileMode.Create));
        }

        /// <summary>
        /// Writes this xrns Renoise song from a specified output stream.
        /// </summary>
        /// <param name="xrnsOutputStream">The XRNS output stream.</param>
        public void Save(Stream xrnsOutputStream)
        {
            // Open ZipFile
            ZipFile zipFile = ZipFile.Create(xrnsOutputStream);

            // Begin ZipUpdate
            zipFile.BeginUpdate();

            // Serializer Song
            // Get Song.xml from xrns archive
            XmlSerializer songSerializer = new Microsoft.Xml.Serialization.GeneratedAssembly.RenoiseSongSerializer();
            // XmlSerializer songSerializer= new XmlSerializer(typeof(RenoiseSong));
            // RenoiseSongSerializer songSerializer = new RenoiseSongSerializer();
            MemoryStream songStream = new MemoryStream();

            // Use RenoiseSong : it has the doc_version attribute
            RenoiseSong song = new RenoiseSong();
            Copy(song);
            songSerializer.Serialize(songStream, song);

            // Seek back to Position = 0
            songStream.Position = 0;
            zipFile.Add(new ZipEntryStreamSource(songStream), "Song.xml");

            // Add Samples to archive
            if (Instruments != null)
            {
                for (int i = 0; i < Instruments.Length; i++)
                {
                    Instrument instrument = Instruments[i];
                    if (instrument.Samples != null)
                    {
                        for (int j = 0; j < instrument.Samples.Length; j++)
                        {
                            Sample sample = instrument.Samples[j];
                            int index = "Sample00 ".Length;
                            string fileName = sample.FileName == null ? "Undefined" : Path.GetFileName(sample.FileName);
                            string sampleName = fileName.Substring(index, fileName.Length - index);
                            string sampleNameEntry = String.Format(
                                "SampleData/Instrument{0:D2} ({1})/Sample{2:D2} {3}", i,
                                instrument.Name,
                                j, sampleName);
                            sample.SaveBufferToZip(zipFile, sampleNameEntry);
                        }
                    }
                }
            }
            // Commit Zip
            zipFile.CommitUpdate();

            // Close Zip
            zipFile.Close();

            xrnsOutputStream.Close();
        }
    }
}
