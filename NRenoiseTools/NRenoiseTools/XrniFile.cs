using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;

namespace NRenoiseTools
{
    public class XrniFile
    {
        private RenoiseInstrument instrument;
        private InternalSample[] samples = new InternalSample[32];


       public XrniFile()
        {
            // Create an empty Renoise song
            Load(new MemoryStream(Properties.Resources.EmptySong));
        }

       public XrniFile(RenoiseInstrument instrument)
        {
            this.instrument = instrument;
        }

       public XrniFile(Instrument instrument, bool isInstrumentToBind)
       {
           this.instrument = new RenoiseInstrument();
           if ( ! isInstrumentToBind )
           {
               instrument = (Instrument)Util.DeepClone(instrument);
           }
           CopyInstrumentToXrni(instrument, this.instrument);
       }

        public static void CopyInstrumentToXrni(Instrument fromInstrument, RenoiseInstrument toInstrument)
        {
            toInstrument.Envelopes = fromInstrument.Envelopes;
            toInstrument.MidiProperties = fromInstrument.MidiProperties;
            toInstrument.Name = fromInstrument.Name;
            toInstrument.PluginProperties = fromInstrument.PluginProperties;
            toInstrument.Samples = fromInstrument.Samples;
            toInstrument.SplitMap = fromInstrument.SplitMap;
        }
       
        public RenoiseInstrument Instrument
        {
            get { return instrument; }
            set { instrument = value; }
        }

        public byte[] GetSample(int sampleIndex)
        {
            if ( sampleIndex >= instrument.Samples.Length )
            {
                throw new ArgumentException(String.Format("sampleIndex {0} must be < {1}", sampleIndex, instrument.Samples.Length));
            }
            return samples[sampleIndex].samples;
        }

        public void SetSample(int sampleIndex, byte[] sample, string sampleFileName)
        {
            if (sampleIndex >= instrument.Samples.Length)
            {
                throw new ArgumentException(String.Format("sampleIndex {0} must be < {1}", sampleIndex, instrument.Samples.Length));
            }
            InternalSample internalSample = samples[sampleIndex];

            // update sample file name
            if (sampleFileName != null && !sampleFileName.StartsWith("("))
            {
                sampleFileName = "(" + Path.GetFileNameWithoutExtension(sampleFileName) + ")." +
                                 Path.GetExtension(sampleFileName);
            }

            if (internalSample == null)
            {
                internalSample = new InternalSample(sampleFileName, sample);
                samples[sampleIndex] = internalSample;
            }
            internalSample.fullSampleName = sampleFileName;
            internalSample.samples = sample;
            instrument.Samples[sampleIndex].FileName = sampleFileName;
        }

        public void SetSample( int sampleIndex, byte[] sample)
        {
            SetSample(sampleIndex, sample, null);
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
            ZipEntry zipEntry = zipFile.GetEntry("Instrument.xml");
            StreamReader instrumentStream = new StreamReader(zipFile.GetInputStream(zipEntry));

            // Deserialize XRNS song. Use builtin RenoiseSongSerializer for faster serialization (without the cost of creating it)
            // RenoiseInstrumentSerializer xSerializer = new RenoiseInstrumentSerializer();
            XmlSerializer xSerializer = new XmlSerializer(typeof(RenoiseInstrument));
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
            instrument = (RenoiseInstrument)xSerializer.Deserialize(instrumentStream);
            instrumentStream.Close();

            for (int j = 0; j < instrument.Samples.Length; j++)
            {
                SetSample(j,null);
            }

            IEnumerator enumSamples = zipFile.GetEnumerator();
            while (enumSamples.MoveNext())
            {
                ZipEntry sampleEntry = (ZipEntry)enumSamples.Current;
                if (sampleEntry.IsFile && sampleEntry.Name.StartsWith("SampleData") )
                {
                    string[] pathNames = sampleEntry.Name.Split('/');
                    string sampleName = pathNames[1];
                    int sampleIndex = int.Parse(sampleName.Substring("Sample".Length, 3));
                    long size = sampleEntry.Size;
                    byte[] buffer = new byte[size];
                    Stream sampleStream = zipFile.GetInputStream(sampleEntry);
                    int readSize = sampleStream.Read(buffer, 0, (int)size);
                    sampleStream.Close();
                    // Set Sample Data with correct name
                    int indexOfSampleName = "Sample00 ".Length;
                    SetSample(sampleIndex, buffer, sampleName.Substring(indexOfSampleName, sampleName.Length - indexOfSampleName));
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
            // RenoiseInstrumentSerializer instrumentSerializer = new RenoiseInstrumentSerializer();
            XmlSerializer instrumentSerializer = new XmlSerializer(typeof(RenoiseInstrument));

            MemoryStream  instrumentStream = new MemoryStream();
            instrumentSerializer.Serialize(instrumentStream, instrument);
            // Seek back to Position = 0
            instrumentStream.Position = 0;
            zipFile.Add(new ZipEntryStreamSource(instrumentStream), "Instrument.xml");


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
                    else if (samples[j] != null &&
                             samples[j].fullSampleName != null)
                    {
                        sampleName = samples[j].fullSampleName;
                    }
                    if (sampleName != null)
                    {
                        string sampleNameEntry = String.Format("SampleData/Sample{0:D2} {1}",j, sampleName);

                        MemoryStream sampleStream = new MemoryStream(GetSample(j));
                        zipFile.Add(new ZipEntryStreamSource(sampleStream), sampleNameEntry);
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
