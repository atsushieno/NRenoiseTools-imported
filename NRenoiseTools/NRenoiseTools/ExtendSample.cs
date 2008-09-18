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
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace NRenoiseTools
{
    /// <summary>
    /// Sample class. This class to create and manipulate Renoise Song.
    /// </summary>
    public partial class Sample
    {
        private byte[] buffer;

        /// <summary>
        /// Gets or sets the sample buffer.
        /// </summary>
        /// <value>The sample buffer.</value>
        public byte[] Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }

        /// <summary>
        /// Saves the buffer a ZipEntry.
        /// </summary>
        /// <param name="zipFile">The zip file.</param>
        /// <param name="sampleNameEntry">The sample name entry.</param>
        internal void SaveBufferToZip(ZipFile zipFile, string sampleNameEntry)
        {
            MemoryStream sampleStream = new MemoryStream(Buffer);
            zipFile.Add(new ZipEntryStreamSource(sampleStream), sampleNameEntry);
        }

        /// <summary>
        /// Loads the buffer from a ZipEntry.
        /// </summary>
        /// <param name="zipFile">The zip file.</param>
        /// <param name="sampleEntry">The sample entry.</param>
        internal void LoadBufferFromZip(ZipFile zipFile, ZipEntry sampleEntry)
        {
            string sampleName = Path.GetFileName(sampleEntry.Name);
            int sampleIndex = int.Parse(sampleName.Substring("Sample".Length, 3));
            long size = sampleEntry.Size;
            buffer = new byte[size];
            Stream sampleStream = zipFile.GetInputStream(sampleEntry);
            sampleStream.Read(buffer, 0, (int)size);
            // Assert readSize == size
            sampleStream.Close();
        }

    }
}