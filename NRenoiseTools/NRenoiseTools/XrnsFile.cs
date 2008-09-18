using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;

namespace NRenoiseTools
{
    public class XrnsFile
    {
        private RenoiseSong song;
        // todo: should optimized allocation
        private InternalSample[,] samplesByInstrumentArray = new InternalSample[256, 32];

        public XrnsFile()
        {
            // Create an empty Renoise song
//            Load(new MemoryStream(Properties.Resources.EmptySong));
        }

        public XrnsFile(RenoiseSong song)
        {
            this.song = song;
        }

        public RenoiseSong Song
        {
            get { return song; }
            set { song = value; }
        }

        public byte[] GetInstrumentSample(int instrumentIndex, int sampleIndex)
        {
            if ( instrumentIndex >= song.Instruments.Length )
            {
                throw new ArgumentException(String.Format("instrumentIndex {0} must be < {1}", instrumentIndex, song.Instruments.Length));
            }

            if ( sampleIndex >= song.Instruments[instrumentIndex].Samples.Length )
            {
                throw new ArgumentException(String.Format("sampleIndex {0} must be < {1}", sampleIndex, song.Instruments[instrumentIndex].Samples.Length));
            }
            return samplesByInstrumentArray[instrumentIndex, sampleIndex].samples;
        }

        public void SetInstrumentSample(int instrumentIndex, int sampleIndex, byte[] sample, string sampleFileName)
        {
            if (instrumentIndex >= song.Instruments.Length)
            {
                throw new ArgumentException(String.Format("instrumentIndex {0} must be < {1}", instrumentIndex, song.Instruments.Length));
            }

            if (sampleIndex >= song.Instruments[instrumentIndex].Samples.Length)
            {
                throw new ArgumentException(String.Format("sampleIndex {0} must be < {1}", sampleIndex, song.Instruments[instrumentIndex].Samples.Length));
            }
            InternalSample internalSample = samplesByInstrumentArray[instrumentIndex, sampleIndex];

            // update sample file name
            if (sampleFileName != null && !sampleFileName.StartsWith("("))
            {
                sampleFileName = "(" + Path.GetFileNameWithoutExtension(sampleFileName) + ")." +
                                 Path.GetExtension(sampleFileName);
            }

            if (internalSample == null)
            {
                internalSample = new InternalSample(sampleFileName, sample);
                samplesByInstrumentArray[instrumentIndex, sampleIndex] = internalSample;
            }
            internalSample.fullSampleName = sampleFileName;
            internalSample.samples = sample;
        }

        public void SetInstrumentSample(int instrumentIndex, int sampleIndex, byte[] sample)
        {
            SetInstrumentSample(instrumentIndex, sampleIndex, sample, null);
        }

        public void Load(string fileName)
        {
            Load(new FileStream(fileName, FileMode.Open));
        }
        
        public void Load(Stream xrnsInputStream)
        {
            // Open ZipFile
            ZipFile zipFile = new ZipFile(xrnsInputStream);

            // Get Song.xml from xrns archive
            ZipEntry zipEntry = zipFile.GetEntry("Song.xml");
            StreamReader songStream = new StreamReader(zipFile.GetInputStream(zipEntry));

            // Deserialize XRNS song. Use builtin RenoiseSongSerializer for faster serialization (without the cost of creating it)
            // RenoiseSongSerializer xSerializer = new RenoiseSongSerializer();
            XmlSerializer xSerializer = new Microsoft.Xml.Serialization.GeneratedAssembly.RenoiseSongSerializer();
//            XmlSerializer xSerializer = new XmlSerializer(typeof(RenoiseSong));
            // XmlSerializer xSerializer = new XmlSerializer(typeof(RenoiseSong));
            // To generate the RenoiseSongSerializer : see article http://www.hanselman.com/blog/CategoryView.aspx?category=XmlSerializer
            // Add the folowwing configuration to get the source of RenoiseSongSerializer
            // Application.exe.config
            //<configuration>
            //   <system.diagnostics>
            //      <switches>
            //         <add name="XmlSerialization.Compilation" value="1" />
            //      </switches>
            //   </system.diagnostics>
            //  <system.xml.serialization> 
            //  <xmlSerializer tempFilesLocation="C:\\Code\\Sound\\muziks\\Xrns2Midi\\Xrns2Midi\\bin\\Release\\"/> 
            //  </system.xml.serialization> 
            //</configuration>
            song = (RenoiseSong)xSerializer.Deserialize(songStream);
            songStream.Close();

            // Prepare Samples
            for (int i = 0; i < song.Instruments.Length; i++)
            {
                Instrument instrument = song.Instruments[i];
                for (int j = 0; j < instrument.Samples.Length; j++)
                {
                    SetInstrumentSample(i,j,null);
                }
            }

            IEnumerator enumSamples = zipFile.GetEnumerator();
            while (enumSamples.MoveNext())
            {
                ZipEntry sampleEntry = (ZipEntry)enumSamples.Current;
                if (sampleEntry.IsFile && sampleEntry.Name.StartsWith("SampleData") )
                {
                    string[] pathNames = sampleEntry.Name.Split('/');
                    string instrumentName = pathNames[1];
                    string sampleName = pathNames[2];
                    int instrumentIndex = int.Parse(instrumentName.Substring("Instrument".Length, 3));
                    int sampleIndex = int.Parse(sampleName.Substring("Sample".Length, 3));
                    long size = sampleEntry.Size;
                    byte[] buffer = new byte[size];
                    Stream sampleStream = zipFile.GetInputStream(sampleEntry);
                    int readSize = sampleStream.Read(buffer, 0, (int)size);
                    sampleStream.Close();
                    // Set Sample Data with correct name
                    int indexOfSampleName = "Sample00 ".Length;
                    SetInstrumentSample(instrumentIndex, sampleIndex, buffer, sampleName.Substring(indexOfSampleName, sampleName.Length - indexOfSampleName));

                }
            }
        }

        public void Write(string fileName)
        {
            Write(new FileStream(fileName, FileMode.Create));
        }

        public void Write(Stream xrnsOutputStream)
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
            songSerializer.Serialize(songStream,song);
            // Seek back to Position = 0
            songStream.Position = 0;
            zipFile.Add(new ZipEntryStreamSource(songStream), "Song.xml");

            // Add Samples to archive
            if (song.Instruments != null)
            {
                for (int i = 0; i < song.Instruments.Length; i++)
                {
                    Instrument instrument = song.Instruments[i];
                    if (instrument.Samples != null)
                    {
                        for (int j = 0; j < instrument.Samples.Length; j++)
                        {
                            string fileName = instrument.Samples[j].FileName;
                            string sampleName = null;
                            // If filename is set on instrument, use this fileName
                            if (fileName != null)
                            {
                                fileName = Path.GetFileName(fileName);
                                int index = "Sample00 ".Length;
                                sampleName = fileName.Substring(index, fileName.Length - index);
                            }
                            else if (samplesByInstrumentArray[i, j] != null &&
                                     samplesByInstrumentArray[i, j].fullSampleName != null)
                            {
                                sampleName = samplesByInstrumentArray[i, j].fullSampleName;
                            }
                            if (sampleName != null)
                            {
                                string sampleNameEntry = String.Format(
                                    "SampleData/Instrument{0:D2} ({1})/Sample{2:D2} {3}", i,
                                    instrument.Name,
                                    j, sampleName);

                                MemoryStream sampleStream = new MemoryStream(GetInstrumentSample(i, j));
                                zipFile.Add(new ZipEntryStreamSource(sampleStream), sampleNameEntry);
                            }
                        }
                    }
                }
            }
            // Commit Zip
            zipFile.CommitUpdate();

            // Close Zip
            zipFile.Close();
        }
    }
}
