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
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace NRenoiseTools.XsdRenoiseParserApp
{
    /// <summary>
    /// Main class to merge XSD and apply the following modifications to the final xsd :
    /// - Make RenoiseInstrument (from RenoiseInstrumentY.xsd) inherit from Instrument type (from RenoiseSongX.xsd)
    /// - Create a Track type that has the same structure than PatternTrack, PatternSendTrack and PatternMasterTrack, and inherit them from Track.
    /// - Create a type Song and inherit RenoiseSong from Song (to make it similar to RenoiseInstrument->Instrument)
    /// 
    /// TODO: doc explanation of this code
    /// </summary>
    class XSDRenoiseParser
    {
        private MapComplexTypeToElement complexTypeToElements = new MapComplexTypeToElement();
        private Dictionary<string, MapComplexTypeToElement> nameToElementComplexTypes = new Dictionary<string, MapComplexTypeToElement>();
        private Dictionary<string, XmlSchemaComplexType> nameToComplexType = new Dictionary<string, XmlSchemaComplexType>();
        private TextWriter log;
        private TextWriter ConsoleLog;

        public XSDRenoiseParser()
        {
            ConsoleLog = Console.Out;
        }


        /// <summary>
        /// A class that group together an element, a complex type, a stackname (the path names to this element), and the name of the final created complex type.
        /// </summary>
        class XmlSchemaComplexTypeElement
        {
            public XmlSchemaElement Element;
            public XmlSchemaComplexType ComplexType;
            public XmlRenoiseStackName xmlRenoiseStackName;
            public string Name;
        }

        /// <summary>
        /// A collection of <see cref="XmlSchemaComplexTypeElement"/>
        /// </summary>
        class XmlSchemaComplexTypeElementCollection : List<XmlSchemaComplexTypeElement>
        {
        }

        /// <summary>
        /// A dictionary of ComplexType -> XmlSchemaComplexTypeElementCollection
        /// </summary>
        class MapComplexTypeToElement : Dictionary<XmlSchemaComplexType, XmlSchemaComplexTypeElementCollection>
        {
        }

        /// <summary>
        /// Adds to dictionary.
        /// </summary>
        /// <param name="complexTypeMap">The complex type map.</param>
        /// <param name="complexType">Type of the complex.</param>
        /// <param name="element">The element.</param>
        void AddToDictionary(MapComplexTypeToElement complexTypeMap, XmlSchemaComplexType complexType, XmlSchemaElement element)
        {
            XmlSchemaComplexTypeElementCollection elementComplexType = null;

            // Find Existing equivalent ComplexType
            XmlSchemaCompareMode mode = new XmlSchemaCompareMode();
            mode.IsComparingDefaultValues = true;
            foreach (KeyValuePair<XmlSchemaComplexType, XmlSchemaComplexTypeElementCollection> pair in complexTypeMap)
            {
                if (XmlSchemaObjectComparator.CompareType(pair.Key, complexType, mode))
                {
                    elementComplexType = pair.Value;
                }
            }

            // If not found, add new complex type
            if (elementComplexType == null)
            {
                elementComplexType = new XmlSchemaComplexTypeElementCollection();
                complexTypeMap.Add(complexType, elementComplexType);
            }

            // Add root Element to complex type association
            XmlSchemaComplexTypeElement xmlSchemaComplexTypeElement = new XmlSchemaComplexTypeElement();
            xmlSchemaComplexTypeElement.Element = element;
            xmlSchemaComplexTypeElement.ComplexType = element.ElementSchemaType as XmlSchemaComplexType;
            xmlSchemaComplexTypeElement.xmlRenoiseStackName = new XmlRenoiseStackName(element);
            elementComplexType.Add(xmlSchemaComplexTypeElement);
        }

        /// <summary>
        /// Associates the type of the complex.
        /// </summary>
        /// <param name="complexType">Type of the complex.</param>
        /// <param name="element">The element.</param>
        void AssociateComplexType(XmlSchemaComplexType complexType, XmlSchemaElement element)
        {

            AddToDictionary(complexTypeToElements, complexType, element);

            MapComplexTypeToElement elementToComplexTypes;


            string name = XmlRenoiseStackName.StripName(element.Name);

            nameToElementComplexTypes.TryGetValue(name, out elementToComplexTypes);
            if (elementToComplexTypes == null)
            {
                elementToComplexTypes = new MapComplexTypeToElement();
                nameToElementComplexTypes.Add(name, elementToComplexTypes);
            }
            AddToDictionary(elementToComplexTypes, complexType, element);

            // Remove the associated complex type from the element
            element.SchemaType = null;
            element.SchemaTypeName = null;
        }

        /// <summary>
        /// Processes the schema.
        /// </summary>
        /// <param name="xs">The xs.</param>
        void ProcessSchema(XmlSchema xs)
        {
            int level = 0;
            XmlSchemaComplexType complexType;
            foreach (XmlSchemaType type in xs.SchemaTypes.Values)
            {
                complexType = type as XmlSchemaComplexType;
                if (complexType != null)
                    ProcessParticle(complexType.ContentTypeParticle, level);
            }

            foreach (XmlSchemaElement el in xs.Elements.Values)
                ProcessParticle(el, level);
        }

        /// <summary>
        /// Processes the particle.
        /// </summary>
        /// <param name="particle">The particle.</param>
        /// <param name="level">The level.</param>
        void ProcessParticle(XmlSchemaParticle particle, int level)
        {
            level++;
            if (particle is XmlSchemaElement)
            {
                XmlSchemaElement elem = particle as XmlSchemaElement;
                //                log.WriteLine(Shift(level) + "XmlElement: {0}", elem.Name);

                if (elem.RefName.IsEmpty)
                {
                    XmlSchemaType type = elem.ElementSchemaType;
                    XmlSchemaComplexType complexType = type as XmlSchemaComplexType;
                    if (complexType != null && complexType.Name == null)
                    {
                        ProcessParticle(complexType.ContentTypeParticle, level);
                        if (complexType.ContentTypeParticle is XmlSchemaAll)
                        {
                            AssociateComplexType(complexType, elem);

                        }
                    }
                }
            }
            else if (particle is XmlSchemaGroupBase)
            { //xs:all, xs:choice, xs:sequence
                XmlSchemaGroupBase baseParticle = particle as XmlSchemaGroupBase;

                //              log.WriteLine(Shift(level) + "XmlGroup: {0}", particle);
                foreach (XmlSchemaParticle subParticle in baseParticle.Items)
                    ProcessParticle(subParticle, level);
            }
        }

        /// <summary>
        /// Copies the schema elements.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        static void CopySchemaElements(XmlSchema from, XmlSchema to)
        {
            for (int j = 0; j < from.Items.Count; j++)
            {
                XmlSchemaObject schemaObject = from.Items[j];
                schemaObject.Parent = to;
                to.Items.Add(schemaObject);
            }
        }

        /// <summary>
        /// Validates the XSD event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Xml.Schema.ValidationEventArgs"/> instance containing the event data.</param>
        static void ValidateXSDEventHandler(Object sender, ValidationEventArgs e)
        {
            Console.Out.WriteLine(e.Message);
        }


        /// <summary>
        /// Generates the specified XSD files.
        /// </summary>
        /// <param name="xsdFiles">The XSD files.</param>
        /// <param name="outputNamespace">The output namespace.</param>
        /// <param name="outputXSDPrefixName">Name of the output XSD prefix.</param>
        /// <param name="isGeneratingCSharpCode">if set to <c>true</c> [is generating C sharp code].</param>
        /// <param name="isGeneratingSerializers">if set to <c>true</c> [is generating serializers].</param>
        /// <returns></returns>
        public bool Generate(String[] xsdFiles, string outputNamespace, string outputXSDPrefixName, bool isGeneratingCSharpCode, bool isGeneratingSerializers)
        {
            //AppDomainSetup domaininfo = new AppDomainSetup();
            //domaininfo.ShadowCopyFiles = "true";
            //domaininfo.ApplicationBase = Environment.CurrentDirectory;
            //domaininfo.ApplicationName = "XSD-SGEN-App";
            //AppDomain externalAppDomain = AppDomain.CreateDomain("externalAppDomain", null, domaininfo);

            //Assembly xsdAssembly = externalAppDomain.Load("xsd.exe");
            //Assembly sgenAssembly = externalAppDomain.Load(Properties.Resources.sgen);
            //Assembly xsdAssembly = Assembly.LoadFrom(directory + "\\xsd.exe");
            //Assembly sgenAssembly = Assembly.LoadFrom(directory + "\\sgen.exe");

            // Merge all XmlSchema to a single schema
            XmlSchema xs = new XmlSchema();
            List<string> xsdFileUsed = new List<string>();
            for (int i = 0; i < xsdFiles.Length; i++)
            {
                if (!xsdFiles[i].StartsWith("/"))
                {
                    ConsoleLog.WriteLine("Load and merge XSD file from <{0}>",xsdFiles[i]);
                    FileStream inputStream = new FileStream(xsdFiles[i], FileMode.Open);
                    xsdFileUsed.Add(Path.GetFileName(xsdFiles[i]));
                    XmlSchema tempSchema = XmlSchema.Read(inputStream, ValidateXSDEventHandler);
                    CopySchemaElements(tempSchema, xs);
                    inputStream.Close();
                }
            }

            // Compile the schema and print errors if any
            XmlSchemaSet xsSet = new XmlSchemaSet();
            xsSet.Add(xs);
            xsSet.ValidationEventHandler += ValidateXSDEventHandler;
            xsSet.Compile();

            string outputConsoleName = outputXSDPrefixName + ".log";
            log = new StreamWriter(outputConsoleName);

            ConsoleLog.WriteLine("Process Renoise XSD ");
            // Process the schema
            ProcessSchema(xs);


            // Display type and equivalence in log
            foreach (KeyValuePair<XmlSchemaComplexType, XmlSchemaComplexTypeElementCollection> pair in complexTypeToElements)
            {
                log.Write("ComplexType {0}: ", pair.Key.LineNumber);
                foreach (XmlSchemaComplexTypeElement element in pair.Value)
                {
                    log.Write("{0} {1},", element.Element.Name, element.Element.LineNumber);
                }
                log.WriteLine("");
            }

            log.WriteLine("-----------------------------------------------------");

            Dictionary<string, int> nameToCounter = new Dictionary<string, int>();

            foreach (KeyValuePair<string, MapComplexTypeToElement> pair in nameToElementComplexTypes)
            {
                log.WriteLine("ComplexType Name {0}", pair.Key);
                MapComplexTypeToElement complexTypeMap = pair.Value;

                bool isMultipleNames = complexTypeMap.Keys.Count > 1;
                foreach (KeyValuePair<XmlSchemaComplexType, XmlSchemaComplexTypeElementCollection> types in complexTypeMap)
                {
                    XmlSchemaComplexType complexType = types.Key;
                    log.WriteLine("\tComplexType {0}: ", complexType.LineNumber);
                    List<string> newNames = new List<string>();

                    // Make new name
                    for (int i = 0; i < types.Value.Count; i++)
                    {
                        string finalName = isMultipleNames ? types.Value[i].xmlRenoiseStackName.BuildNewName(pair.Key) : pair.Key;
                        types.Value[i].Name = finalName;
                        if (!newNames.Contains(finalName))
                        {
                            newNames.Add(finalName);
                        }
                    }

                    // Check that new name doesn't have any conflicts to any previous name
                    for (int i = 0; i < newNames.Count; i++)
                    {
                        string name = newNames[i];
                        if (nameToCounter.ContainsKey(name))
                        {
                            nameToCounter[name]++;
                            // New Name + Counter
                            string newName = name + nameToCounter[name];
                            log.WriteLine("\t\tWarning, need to create indexed name {0}", name);
                            newNames[i] = newName;
                            for (int j = 0; j < types.Value.Count; j++)
                            {
                                if (types.Value[j].Name == name)
                                {
                                    types.Value[j].Name = newName;
                                }
                            }
                        }
                        else
                        {
                            nameToCounter.Add(name, 0);
                        }
                    }

                    // Associate new name to complextype
                    for (int i = 0; i < newNames.Count; i++)
                    {
                        string name = newNames[i];
                        for (int j = 0; j < types.Value.Count; j++)
                        {
                            if (types.Value[j].Name == name)
                            {
                                XmlSchemaComplexType newComplexType = types.Value[j].ComplexType;
                                newComplexType.Name = name;
                                newComplexType.Parent = null;

                                nameToComplexType.Add(name, newComplexType);
                                break;
                            }
                        }
                    }

                    // Associate Schema Element to complex type
                    for (int i = 0; i < types.Value.Count; i++)
                    {
                        XmlSchemaComplexTypeElement element = types.Value[i];

                        string complexTypeName = element.Name;
                        element.Element.SchemaTypeName = new XmlQualifiedName(complexTypeName);
                        // Patch Envelope property to EnvelopeData
                        log.WriteLine("\t\t{0} {1} => {2}", element.Element.LineNumber, element.xmlRenoiseStackName, complexTypeName);
                    }

                    log.WriteLine("");
                }
            }


            // Postprocess Complex Types (and add new inheritance...etc.)
            PostProcessComplexType();

            // Add all complexType to final XSD
            foreach (KeyValuePair<string, XmlSchemaComplexType> pair in nameToComplexType)
            {
                xs.Items.Add(pair.Value);
            }

            // Revalidate the XSD
            xsSet.Reprocess(xs);

            // Flush the log
            log.Flush();

            // Output XSD

            string outputFileName = outputXSDPrefixName + ".xsd";

            FileStream output = new FileStream(outputFileName, FileMode.Create);
            XmlTextWriter writer = new XmlTextWriter(output, null);
            writer.Formatting = Formatting.Indented;

            // Write Header Comment
            string xmlHeaderComment =
                "***********************************************************************************************************";
            writer.WriteComment(xmlHeaderComment);
            writer.WriteComment(
                MakeComment(
                    string.Format("{0}.xsd generated automatically with XsdRenoiseParser. Generated on: {1}",
                                  outputXSDPrefixName, DateTime.Now), xmlHeaderComment.Length));
            writer.WriteComment(MakeComment("File used to generate this XSD:", xmlHeaderComment.Length));
            foreach (string fileUsed in xsdFileUsed)
            {
                writer.WriteComment(MakeComment(string.Format("    {0}", fileUsed),xmlHeaderComment.Length));
            }
            writer.WriteComment(MakeComment("Check http://www.codeplex.com/nrenoisetools",xmlHeaderComment.Length));
            writer.WriteComment(xmlHeaderComment);

            // Write XSD
            xs.Write(writer);
            output.Flush();
            output.Close();


            if ( ! isGeneratingCSharpCode && isGeneratingSerializers )
            {
                isGeneratingCSharpCode = true;
            }

            // Generate CSharp code?
            if (isGeneratingCSharpCode)
            {
                // Call assembly XSD.EXE : generate .cs from XSD
                ConsoleLog.WriteLine("Generate classes from generated XSD <{0}>", outputFileName);
                int xsdResult = XsdTool.Xsd.Main(new string[]
                                         {
                                             outputFileName, "/classes",
                                             "/namespace:" + outputNamespace,
                                             "/nologo"
                                         }
                        );

                // If .cs ok, generate serializers?
                if (xsdResult == 0 && isGeneratingSerializers)
                {
                    string csFile = Path.GetFileNameWithoutExtension(outputFileName) + ".cs";
                    ConsoleLog.WriteLine("Generate XML serializers from generated classes <{0}> ", csFile);

                    CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                    CompilerParameters parameters = new CompilerParameters();
                    parameters.ReferencedAssemblies.Add("System.dll");
                    parameters.ReferencedAssemblies.Add("System.Data.dll");
                    parameters.ReferencedAssemblies.Add("System.Xml.dll");

                    parameters.GenerateInMemory = false;
                    string outputTempAssemblyName = outputNamespace + ".dll";
                    parameters.OutputAssembly = outputTempAssemblyName;

                    CompilerResults results = provider.CompileAssemblyFromFile(parameters, csFile);

                    if (results.Errors.HasErrors)
                    {
                        Console.WriteLine(results.Errors);
                        return false;
                    }

                    // Generate 
                    xsdResult = SgenTool.Sgen.Main(new string[] {outputTempAssemblyName, "/f", "/nologo"});
                    if (xsdResult != 0)
                    {
                        return false;
                    }
                }
            }

            log.Close();
            return true;
        }

        private static string MakeComment(string comment, int length)
        {
            if ( comment.Length < length )
            {
                StringBuilder builder = new StringBuilder(comment);
                for (int i = 0; i < (length - comment.Length); i++)
                    builder.Append(' ');
                comment = builder.ToString();
            }
            return comment;
        }



        // ----------------------------------------------------------------------------------------------------------
        #region XSD Post Process of RenoiseInstrument, RenoiseSong, Track... etc.
        // ----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Pres the type of the process complex.
        /// </summary>
        void PostProcessComplexType()
        {
            ConsoleLog.WriteLine("Apply PostProcess to Renoise XSD");
            MakeRenoiseInstrumentInheritFromInstrument();
            MakeTrackInheritance();
            MakeSongInheritance();
        }

        /// <summary>
        /// Makes the song inheritance. Create Song inherit from new type SongBase, and inherit RenoiseSong from Song.
        /// </summary>
        void MakeSongInheritance()
        {
            ConsoleLog.WriteLine("--> Make RenoiseSong inheritance");
            XmlSchemaComplexType songType;
            nameToComplexType.TryGetValue("RenoiseSong", out songType);
            if (songType != null)
            {
                // Create a complex type SongBase without the attributes
                XmlSchemaComplexType newSong = new XmlSchemaComplexType();
                newSong.Particle = songType.Particle;
                newSong.Name = "SongBase";
                newSong.IsAbstract = true;
                nameToComplexType.Add(newSong.Name, newSong);

                // Add complex type Song that inherits from SongBaes
                newSong = new XmlSchemaComplexType();
                newSong.Particle = null;
                newSong.Name = "Song";
                newSong.IsAbstract = false;
                nameToComplexType.Add(newSong.Name, newSong);
                MakeInheritance(newSong, "SongBase");

                // Make RenoiseSong inherit from Song
                MakeInheritance(songType, "Song");
            }
        }

        /// <summary>
        /// Makes the track inheritance. Create Track type and make PatternTrack, PatternMasterTrack and PatternSendTrack inherit from Track.
        /// </summary>
        void MakeTrackInheritance()
        {
            ConsoleLog.WriteLine("--> Make Track inheritance");
            XmlSchemaComplexType patternTrack;
            XmlSchemaComplexType patternMasterTrack;
            XmlSchemaComplexType patternSendTrack;
            nameToComplexType.TryGetValue("PatternTrack", out patternTrack);
            nameToComplexType.TryGetValue("PatternMasterTrack", out patternMasterTrack);
            nameToComplexType.TryGetValue("PatternSendTrack", out patternSendTrack);

            if (patternTrack != null && patternMasterTrack != null && patternSendTrack != null &&
                IsInstanceOfWithoutAttributes(patternMasterTrack, patternTrack)
                && IsInstanceOfWithoutAttributes(patternSendTrack, patternTrack))
            {
                XmlSchemaComplexType track = new XmlSchemaComplexType();
                track.Particle = patternTrack.Particle;
                track.Name = "Track";
                track.IsAbstract = true;
                nameToComplexType.Add(track.Name, track);

                MakeInheritance(patternTrack, "Track");
                MakeInheritance(patternMasterTrack, "Track");
                MakeInheritance(patternSendTrack, "Track");
            }
        }


        /// <summary>
        /// Makes the renoise instrument inherit from instrument. Inherit RenoiseInstrument (from RenoiseInstrument.xsd) inherit from Instrument (from RenoiseSong.xsd)
        /// </summary>
        void MakeRenoiseInstrumentInheritFromInstrument()
        {
            ConsoleLog.WriteLine("--> Make RenoiseInstrument inheritance");
            XmlSchemaComplexType instrumentComplexType;
            XmlSchemaComplexType renoiseInstrumentComplexType;
            nameToComplexType.TryGetValue("Instrument", out instrumentComplexType);
            nameToComplexType.TryGetValue("RenoiseInstrument", out renoiseInstrumentComplexType);
            if (instrumentComplexType == null || renoiseInstrumentComplexType == null)
                return;

            XmlSchemaObjectCollection attributes = renoiseInstrumentComplexType.Attributes;
            if (attributes.Count == 1)
            {
                XmlSchemaAttribute docVersionAttribute = (XmlSchemaAttribute)attributes[0];
                if (docVersionAttribute.Name == "doc_version")
                {
                    if (IsInstanceOfWithoutAttributes(renoiseInstrumentComplexType, instrumentComplexType))
                    {
                        MakeInheritance(renoiseInstrumentComplexType, "Instrument");
                    }
                }
            }
        }

        /// <summary>
        /// Removes the attributes from a schema complex type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        static XmlSchemaObjectCollection RemoveAttributes(XmlSchemaComplexType type)
        {
            XmlSchemaObjectCollection attributes = Clone(type.Attributes);
            type.Attributes.Clear();
            return attributes;
        }

        /// <summary>
        /// Determines whether [is instance of without attributes] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="baseTypeToTest">The base type to test.</param>
        /// <returns>
        /// 	<c>true</c> if [is instance of without attributes] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        static bool IsInstanceOfWithoutAttributes(XmlSchemaComplexType type, XmlSchemaComplexType baseTypeToTest)
        {
            XmlSchemaObjectCollection attributesFromBase = RemoveAttributes(baseTypeToTest);
            XmlSchemaObjectCollection attributes = RemoveAttributes(type);
            bool isOk = XmlSchemaObjectComparator.CompareType(type, baseTypeToTest, new XmlSchemaCompareMode());
            Copy(attributesFromBase, baseTypeToTest.Attributes);
            Copy(attributes, type.Attributes);
            return isOk;
        }

        /// <summary>
        /// Makes the inheritance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="baseType">Type of the base.</param>
        static void MakeInheritance(XmlSchemaComplexType type, string baseType)
        {
            XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
            XmlSchemaComplexContentExtension complexContentExtension = new XmlSchemaComplexContentExtension();
            complexContent.Parent = type;
            complexContent.Content = complexContentExtension;
            complexContentExtension.Parent = complexContent;
            // Inherit from Instrument
            complexContentExtension.BaseTypeName = new XmlQualifiedName(baseType);

            // Copy attributes from type to extension
            Copy(type.Attributes, complexContentExtension.Attributes);
            // Clear attributes from type
            type.Attributes.Clear();
            // Associate our content model
            type.ContentModel = complexContent;
            // Remove old xs:all
            type.Particle = null;
        }

        /// <summary>
        /// Copies the specified from.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        static void Copy(XmlSchemaObjectCollection from, XmlSchemaObjectCollection to)
        {
            IEnumerator it = from.GetEnumerator();
            while (it.MoveNext())
                to.Add((XmlSchemaObject)it.Current);
        }

        /// <summary>
        /// Clones the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        static XmlSchemaObjectCollection Clone(XmlSchemaObjectCollection collection)
        {
            XmlSchemaObjectCollection copy = new XmlSchemaObjectCollection();
            Copy(collection, copy);
            return copy;
        }
        #endregion
        // ----------------------------------------------------------------------------------------------------------

    }
}
