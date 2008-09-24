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
    /// Main Renoise Instrument class. Use this class to create and manipulate Renoise Instrument.
    /// </summary>
    partial class Instrument
    {
        private string fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Instrument"/> class.
        /// </summary>
        public Instrument()
        {
            MethodBase methodBase = new StackTrace().GetFrame(1).GetMethod();
            // If 
            if (!methodBase.IsConstructor || !(methodBase.DeclaringType == typeof(RenoiseInstrument)))
            {
                Load(new MemoryStream(Properties.Resources.EmptyInstrument));
            }
        }

        /// <summary>
        /// Copies this instrument to another instrument.
        /// </summary>
        /// <param name="toInstrument">instrument to copy to.</param>
        public void Copy(Instrument toInstrument)
        {
            Util.Copy(this, toInstrument);
        }

        /// <summary>
        /// Gets or sets the name of the file used to store the RenoiseInstrument. This property is set when
        /// loading song from a file <see cref="Load(string)"/>.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        /// <summary>
        /// Loads a xrni Renoise instrument from a specified file name.
        /// </summary>
        /// <param name="fileNameArg">Name of the xrni Renoise file.</param>
        public void Load(string fileNameArg)
        {
            fileName = fileNameArg;
            Load(new FileStream(fileNameArg, FileMode.Open, FileAccess.Read));
        }

        /// <summary>
        /// Loads a xrni Renoise instrument from a specified stream.
        /// </summary>
        /// <param name="xrniInputStream">The XRNI input stream.</param>
        public void Load(Stream xrniInputStream)
        {
            // Open ZipFile
            ZipFile zipFile = Util.OpenZip(xrniInputStream);

            // Get Instrument.xml from xrni archive
            ZipEntry zipEntry = zipFile.GetEntry("Instrument.xml");
            if (zipEntry == null)
                throw new ArgumentException("Invalid XRNI instrument file. No Instrument.xml in the archive");

            StreamReader instrumentStream = new StreamReader(zipFile.GetInputStream(zipEntry));

            // Use generated Serializer
            XmlSerializer xSerializer = RenoiseXmlSerializerFactory.Find(typeof(RenoiseInstrument));

            // Deserialize Instrument.xml
            RenoiseInstrument instrument = (RenoiseInstrument)xSerializer.Deserialize(instrumentStream);

            // Close the stream from the zip
            instrumentStream.Close();

            // Copy the properties to this instance
            string savedFileName = FileName;
            instrument.Copy(this);
            FileName = savedFileName;

            // Set all buffer Samples to null
            foreach (Sample sample in instrument.Samples)
            {
                sample.Buffer = null;
            }

            IEnumerator enumSamples = zipFile.GetEnumerator();
            while (enumSamples.MoveNext())
            {
                ZipEntry sampleEntry = (ZipEntry)enumSamples.Current;
                if (sampleEntry.IsFile && sampleEntry.Name.StartsWith("SampleData"))
                {
                    string sampleName = Path.GetFileName(sampleEntry.Name);
                    int sampleIndex = int.Parse(sampleName.Substring("Sample".Length, 3));
                    Samples[sampleIndex].LoadBufferFromZip(zipFile, sampleEntry);
                }
            }
        }

        /// <summary>
        /// Writes this xrni Renoise instrument to the default filename file.
        /// </summary>
        public void Save()
        {
            Save(FileName);
        }

        /// <summary>
        /// Writes this xrni Renoise instrument to a specified output file.
        /// </summary>
        /// <param name="fileNameArg">Name of the file.</param>
        public void Save(string fileNameArg)
        {
            Save(new FileStream(fileNameArg, FileMode.Create));
        }

        /// <summary>
        /// Writes this xrni Renoise instrument from a specified output stream.
        /// </summary>
        /// <param name="xrniOutputStream">The XRNI output stream.</param>
        public void Save(Stream xrniOutputStream)
        {
            // Open ZipFile
            ZipFile zipFile = ZipFile.Create(xrniOutputStream);

            // Begin ZipUpdate
            zipFile.BeginUpdate();

            // Serializer Instrument
            // Get Instrument.xml from xrni archive
            XmlSerializer instrumentSerializer = RenoiseXmlSerializerFactory.Find(typeof (RenoiseInstrument));
            // XmlSerializer instrumentSerializer= new XmlSerializer(typeof(RenoiseInstrument));
            // RenoiseInstrumentSerializer instrumentSerializer = new RenoiseInstrumentSerializer();
            MemoryStream instrumentStream = new MemoryStream();

            // Use RenoiseInstrument : it has the doc_version attribute
            RenoiseInstrument instrument = new RenoiseInstrument();
            Copy(instrument);
            instrumentSerializer.Serialize(instrumentStream, instrument);

            // Seek back to Position = 0
            instrumentStream.Position = 0;
            zipFile.Add(new ZipEntryStreamSource(instrumentStream), "Instrument.xml");

            if (instrument.Samples != null)
            {
                for (int j = 0; j < instrument.Samples.Length; j++)
                {
                    Sample sample = instrument.Samples[j];
                    if (sample.Buffer != null && sample.Buffer.Length > 0)
                    {
                        int index = "Sample00 ".Length;
                        string fileName = sample.FileName == null ? "Undefined" : Path.GetFileName(sample.FileName);
                        string sampleName = fileName.Substring(index, fileName.Length - index);
                        string sampleNameEntry = String.Format(
                            "SampleData/Sample{0:D2} {1}", j, sampleName);

                        sample.SaveBufferToZip(zipFile, sampleNameEntry);
                    }
                }
            }
            // Commit Zip
            zipFile.CommitUpdate();

            // Close Zip
            zipFile.Close();
            // Close output file
            xrniOutputStream.Close();
        }
    }
}
