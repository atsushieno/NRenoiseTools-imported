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

namespace NRenoiseTools.XsdRenoiseParserApp
{
    /// <summary>
    /// XsdRenoiseParser. Convert Renoise XSD files to an optimized single XSD file.
    /// Usage: XsdRenoiseParser.exe RenoiseSongX.xsd  RenoiseInstrumentY.xsd  RenoiseDeviceChainZ.xsd [/out:{0}] [/ns:{1}] [/classes] [/serializers]", DefaultXSDPrefixName, DefaultNamespace
    /// 
    /// Example for 1.9.1:
    /// XsdRenoiseParser RenoiseSong10.xsd RenoiseInstrument6.xsd RenoiseDeviceChain6.xsd /out:RenoiseModel191 /classes /serializers
    /// </summary>
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0) goto usage;

            XSDRenoiseParser parser = new XSDRenoiseParser();

            const string DefaultXSDPrefixName = "RenoiseModel";
            const string DefaultNamespace = "NRenoiseTools";
            string outputXSDPrefixName = DefaultXSDPrefixName;
            string outputNamespace = DefaultNamespace;
            bool isGeneratingClasses = false;
            bool isGeneratingSerializers = false;

            bool isArgumentsOk = true;
            foreach (string arg in args)
            {
                if ( arg.StartsWith("/out:"))
                {
                    string temp = arg.Split(':')[1];
                    outputXSDPrefixName = (!string.IsNullOrEmpty(temp)) ? temp : outputXSDPrefixName;
                } else if ( arg.StartsWith("/ns:"))
                {
                    string temp = arg.Split(':')[1];
                    outputNamespace = (!string.IsNullOrEmpty(temp)) ? temp : outputNamespace;
                } else if ( arg == "/classes")
                {
                    isGeneratingClasses = true;   
                } else if ( arg == "/serializers")
                {
                    isGeneratingSerializers = true;
                } else if ( arg.StartsWith("/"))
                {
                    Console.WriteLine("Invalid argument <{0}>. Check usage", arg);
                    isArgumentsOk = false;
                }
            }

            if (isArgumentsOk)
            {
                return parser.Generate(args, outputNamespace, outputXSDPrefixName, isGeneratingClasses, isGeneratingSerializers) ? 0 : 1;
            }
usage:
            Console.WriteLine("Usage: XsdRenoiseParser.exe RenoiseSongX.xsd  RenoiseInstrumentY.xsd  RenoiseDeviceChainZ.xsd [/out:{0}] [/ns:{1}] [/classes] [/serializers]", DefaultXSDPrefixName, DefaultNamespace);
            Console.WriteLine("Press any key to end the program");
            Console.ReadKey(true);
            return -1;
        }


    }
}
