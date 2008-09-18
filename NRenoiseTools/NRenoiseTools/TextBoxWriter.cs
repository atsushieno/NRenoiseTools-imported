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
//
// This class TextBoxWriter is taken from  http://kasperbirch.wordpress.com/2007/12/12/redirecting-datacontextlog-to-textbox/
// Thanks to Kaspers.
using System;
using System.Text;
using System.Windows.Forms;

namespace NRenoiseTools
{
    public class TextBoxWriter : System.IO.TextWriter
    {
        private Encoding encoding;
        private TextBox textBox;

        public TextBoxWriter(TextBox textBox)
        {
            if (textBox == null)
                throw new NullReferenceException();
            this.textBox = textBox;
        }
        public override Encoding Encoding
        {
            get
            {
                if (this.encoding == null)
                {
                    this.encoding = new UnicodeEncoding(false, false);
                }
                return encoding;
            }
        }
        public override void Write(string value)
        {
            textBox.AppendText(value);
        }

        public override void Write(char[] buffer)
        {
            Write(new string(buffer));
        }
        public override void Write(char[] buffer, int index, int count)
        {
            Write(new string(buffer, index, count));
        }
    }  
}
