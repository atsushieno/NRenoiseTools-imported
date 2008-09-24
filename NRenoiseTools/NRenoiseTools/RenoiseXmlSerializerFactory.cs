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
using System.Xml.Serialization;

namespace NRenoiseTools
{
    /// <summary>
    /// Class to load a XmlSerializer from NRenoiseTools.XmlSerializers if available else use default (and slow) method.
    /// </summary>
    class RenoiseXmlSerializerFactory
    {
        private static object lockObject = new object();
        private static Assembly serializerAssembly = null;

        /// <summary>
        /// Returns a xml serializer for the specified type. 
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>a xml serializer</returns>
        public static XmlSerializer Find(Type type)
        {
            Assembly assembly = null;
            XmlSerializer result = null;
            lock (lockObject)
            {
                if ( serializerAssembly == null )
                {
                    try
                    {
                        serializerAssembly = Assembly.Load("NRenoiseTools.XmlSerializers");
                    }
                    catch (FileNotFoundException ex1)
                    {
                        try
                        {
                            serializerAssembly = Assembly.LoadFrom("NRenoiseTools.XmlSerializers.dll");
                        }
                        catch (FileNotFoundException ex2)
                        {
                            try
                            {
                                serializerAssembly = Assembly.Load(
                                    "NRenoiseTools.XmlSerializers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=5fc9b172611661e4");
                            }
                            catch (FileNotFoundException ex3)
                            {
                            }
                        }
                    }
                }
                assembly = serializerAssembly;
            }

            if (assembly != null)
            {

                string serialClassName = string.Format("Microsoft.Xml.Serialization.GeneratedAssembly.{0}Serializer",
                                                       type.Name);

                result = (XmlSerializer)assembly.CreateInstance(serialClassName);
            }  else
            {
                result = new XmlSerializer(type);
            }
            return result;
        }
    }
}
