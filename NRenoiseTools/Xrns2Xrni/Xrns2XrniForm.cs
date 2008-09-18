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
using System.Windows.Forms;
using NRenoiseTools;

namespace NRenoiseTools.Xrns2XrniApp
{
    public partial class Xrns2XrniForm : Form
    {
        private Xrns2Xrni xrns2Xrni;

        public Xrns2XrniForm()
        {
            InitializeComponent();
            xrns2Xrni = new Xrns2Xrni();
            xrns2Xrni.Log = new TextBoxWriter(LogTextBox);
        }

        private void ButtonSelectXrnsFile_Click(object sender, EventArgs e)
        {
            if ( openFileXRNSDialog.ShowDialog(this) == DialogResult.OK )
            {
                textBoxXrnsFileName.Text = openFileXRNSDialog.FileName;
            }
        }

        private void ConvertButton_Click(object sender, EventArgs e)
        {
            xrns2Xrni.Extract(textBoxXrnsFileName.Text);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
