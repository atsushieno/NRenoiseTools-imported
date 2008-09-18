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
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;

namespace NRenoiseTools.XsdRenoiseParserApp
{
    /// <summary>
    /// This class provide a stack of names in the XML path based on an XmlSchemaElement.
    /// </summary>
    class XmlRenoiseStackName
    {
        public List<string> ElementNames = new List<string>();

        public XmlRenoiseStackName(XmlSchemaElement element)
        {
            ElementNames.Add(element.Name);
            XmlSchemaObject xmlObj = element;
            while (xmlObj != null && xmlObj.Parent != null)
            {
                XmlSchemaObject parent = xmlObj.Parent;
                if (parent is XmlSchemaElement)
                {
                    ElementNames.Add((parent as XmlSchemaElement).Name);
                }
                xmlObj = parent;
            }
        }

        public string BuildNewName(string rootName)
        {

            bool isPreset = false;
            bool isFilterDevice = false;
            string filterDeviceName = null;

            if (ElementNames.Count > 4)
            {
                // In case of 
                // RenoiseSong.PatternPool.Patterns.Pattern.Tracks.PatternTrack.Automations.Envelopes.Envelope.Envelope
                // RenoiseSong.PatternPool.Patterns.Pattern.Tracks.PatternMasterTrack.Automations.Envelopes.Envelope.Envelope
                // RenoiseSong.PatternPool.Patterns.Pattern.Tracks.PatternSendTrack.Automations.Envelopes.Envelope.Envelope
                // Map to EnvelopeData
                if (ElementNames[0] == "Envelope")
                {
                    if (ElementNames[1] == "Envelope"
                        && ElementNames[2] == "Envelopes"
                        && ElementNames[3] == "Automations")
                    {
                        return "EnvelopeData";
                    }

                    if (ElementNames[1] == "Envelopes"
                        && ElementNames[2] == "Automations")
                    {
                        return "EnvelopeItem";
                    }
                }
            }

            for (int i = 0; i < ElementNames.Count; i++)
            {
                string name = ElementNames[i];
                if (i > 0 && (name == "RunTimePresetA" || name == "RunTimePresetB"))
                {
                    isPreset = true;
                }
                if ((name == "FilterDevices" || name == "RenoiseDeviceChain") && i > 0 && ElementNames[i - 1] == "Devices")
                {
                    isFilterDevice = true;
                    filterDeviceName = ElementNames[i - 2];
                }
            }
            if (ElementNames[ElementNames.Count - 1] == "FilterDevicePreset")
            {
                isFilterDevice = true;
                filterDeviceName = "FilterDevicePreset";
            }
            StringBuilder newName = new StringBuilder();
            if (isFilterDevice)
            {
                // 
                // FilterDevices.Devices.SequencerTrackDevice (3).RunTimePresetA (2).DeviceSlot (1).Volume (0)
                if (isPreset)
                {
                    newName.Append(filterDeviceName).Append(rootName).Append("RunTimePreset");
                }
                else
                {
                    // RenoiseSong.Tracks.SequencerTrack.FilterDevices.Devices.SequencerTrackDevice (1).Volume (0)
                    newName.Append(filterDeviceName).Append(rootName);
                }
            }
            else if (isPreset)
            {
                newName.Append(rootName).Append("RunTimePreset");
            }
            else
            {
                if (ElementNames.Count >= 2)
                {
                    newName.Append(ElementNames[1]).Append(rootName);
                }
                else
                {
                    newName.Append(rootName);
                }
            }
            return newName.ToString();
        }

        // Reverse order Name
        public override string ToString()
        {
            StringBuilder nameStr = new StringBuilder();
            for (int i = ElementNames.Count - 1; i >= 0; i--)
            {
                nameStr.Append(ElementNames[i]);
                if (i > 0)
                {
                    nameStr.Append('.');
                }
            }
            return nameStr.ToString();
        }

        public static string StripName(string name)
        {
            if (name == "RunTimePresetA" || name == "RunTimePresetB")
            {
                return "RunTimePreset";
            } 
            
            if ( name == "PatternSequence")
            {
                return "PatternSequenceData";                
            }
            StringBuilder newName = new StringBuilder(name.Length);
            for (int i = 0; i < name.Length; i++)
            {
                if (!(name[i] >= '0' && name[i] <= '9'))
                {
                    newName.Append(name[i]);
                }
            }
            return newName.ToString();
        }
    }
}