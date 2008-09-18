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
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using ICSharpCode.SharpZipLib.Zip;

namespace NRenoiseTools
{
    /// <summary>
    /// Misc utility methods.
    /// </summary>
    public class Util
    {
        /// <summary>
        /// Open a ZipFile from a Stream. If the stream is not a zip file, an ArgumentException is thrown.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="ArgumentException">If stream is not a zip file</exception>
        /// <returns>A Zip file</returns>
        static public ZipFile OpenZip(Stream stream)
        {
            byte[] header = new byte[4];
            bool isZipFile = true;
            if (stream.CanSeek)
            {
                isZipFile = false;
                if (stream.Length > 4)
                {
                    stream.Read(header, 0, 4);
                    isZipFile = header[0] == 0x50 && header[1] == 0x4b && header[2] == 0x03 && header[3] == 0x04;
                }
                // Rewind to beginning of file
                stream.Position = 0;
            }
            ZipFile zipFile = null;
            if ( isZipFile )
            {
                try
                {
                    zipFile = new ZipFile(stream);
                } catch (Exception ex)
                {
                    throw new ArgumentException("Unable to load zip from stream", ex);
                }
            } else
            {
                throw new ArgumentException("Invalid stream. Stream is not a zip file");
            }

            return zipFile;
        }

        /// <summary>
        /// Perform a deep clone of an object.
        /// </summary>
        /// <param name="objToClone">The obj to clone.</param>
        /// <returns>the clone</returns>
        static public object DeepClone(object objToClone)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            binaryFormatter.Serialize(stream, objToClone);
            stream.Position = 0;
            return binaryFormatter.Deserialize(stream);
        }

        /// <summary>
        /// Copies properties from an instance to another
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcInstance">The source instance.</param>
        /// <param name="dstInstance">The destination instance.</param>
        static public void Copy<T>(T srcInstance, T dstInstance)
        {
            foreach (PropertyInfo info in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                info.SetValue(dstInstance, info.GetValue(srcInstance, null), null);
            }
        }
    }
}
