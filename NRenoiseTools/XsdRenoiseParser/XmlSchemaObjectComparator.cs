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
using System.Reflection;
using System.Xml.Schema;

namespace NRenoiseTools.XsdRenoiseParserApp
{
    /// <summary>
    /// A parameter used in the comparison mode for methods CompareType.
    /// </summary>
    class XmlSchemaCompareMode
    {
        public bool IsComparingDefaultValues = true;
    }

    /// <summary>
    /// A simple class to perform a deep comparison of two XmlSchemaObject. This class is very limited and expect to have 
    /// elements, attributes...etc declared in the same order.
    /// </summary>
    internal class XmlSchemaObjectComparator
    {

        /// <summary>
        /// Compares two xml elements.
        /// </summary>
        /// <returns>true if types are equals</returns>
        public static bool CompareType(XmlSchemaGroupBase from, XmlSchemaGroupBase against, XmlSchemaCompareMode mode)
        {
            return CompareType(from.Items, against.Items, mode);
        }

        /// <summary>
        /// Compares two xml elements.
        /// </summary>
        /// <returns>true if types are equals</returns>
        public static bool CompareType(XmlSchemaSimpleType from, XmlSchemaSimpleType against, XmlSchemaCompareMode mode)
        {
            bool isSameType = false;
            if (from.Content.GetType() == against.Content.GetType())
            {
                isSameType = true;
            }
            return isSameType;
        }

        /// <summary>
        /// Compares two xml elements.
        /// </summary>
        /// <returns>true if types are equals</returns>
        public static bool CompareType(XmlSchemaObjectCollection from, XmlSchemaObjectCollection against, XmlSchemaCompareMode mode)
        {
            bool isSameType = false;
            if (from.Count == against.Count)
            {
                isSameType = true;
                for (int i = 0; i < from.Count; i++)
                {
                    XmlSchemaObject fromObj = from[i];
                    XmlSchemaObject againstObj = against[i];
                    isSameType = CompareType(fromObj, againstObj, mode);
                    if (!isSameType)
                        break;
                }
            }
            return isSameType;
        }

        /// <summary>
        /// Compares two xml elements.
        /// </summary>
        /// <returns>true if types are equals</returns>
        public static bool CompareType(XmlSchemaAttribute from, XmlSchemaAttribute against, XmlSchemaCompareMode mode)
        {

            if (from.Name != against.Name
                || from.Use != against.Use
                || from.SchemaTypeName != against.SchemaTypeName)
            {
                return false;
            }
            if (mode.IsComparingDefaultValues)
            {
                if (from.FixedValue != against.FixedValue)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Compares two xml elements.
        /// </summary>
        /// <returns>true if types are equals</returns>
        public static bool CompareType(XmlSchemaComplexType from, XmlSchemaComplexType against, XmlSchemaCompareMode mode)
        {
            bool isSameType = CompareType(from.ContentTypeParticle, against.ContentTypeParticle, mode);

            if (isSameType)
            {
                isSameType = CompareType(from.Attributes, against.Attributes, mode);
            }
            return isSameType;
        }

        /// <summary>
        /// Compares two xml elements.
        /// </summary>
        /// <returns>true if types are equals</returns>
        public static bool CompareType(XmlSchemaElement from, XmlSchemaElement against, XmlSchemaCompareMode mode)
        {
            if (from.Name != against.Name)
            {
                return false;
            }
            if (mode.IsComparingDefaultValues)
            {
                if (from.DefaultValue != against.DefaultValue
                    || from.MinOccurs != against.MinOccurs
                    || from.MaxOccurs != against.MaxOccurs)
                {
                    return false;
                }
            }

            return CompareType(from.ElementSchemaType, against.ElementSchemaType, mode);
        }

        /// <summary>
        /// Compares two xml elements.
        /// </summary>
        /// <returns>true if types are equals</returns>
        public static bool CompareType(XmlSchemaObject from, XmlSchemaObject against, XmlSchemaCompareMode mode)
        {
            bool isSameType = false;
            // Check Types
            if (from == null && against == null)
            {
                return true;
            }
            if (from == null || against == null)
            {
                return false;
            }
            if (from.GetType() != against.GetType())
            {
                return false;
            }

            Type fromType = from.GetType();
            MethodInfo info = typeof(XmlSchemaObjectComparator).GetMethod("CompareType", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] { fromType, fromType, typeof(XmlSchemaCompareMode) }, null);

            if (info != null && info.GetParameters()[0].ParameterType != typeof(XmlSchemaObject))
            {
                isSameType = (bool)info.Invoke(null, new object[] { from, against, mode });
            }
            else
            {
                throw new ArgumentException(String.Format("ERROR in code, no comparer for type {0}", fromType));
            }
            return isSameType;
        }
    }

}
