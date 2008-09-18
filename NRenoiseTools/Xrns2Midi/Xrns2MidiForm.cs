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

namespace NRenoiseTools.Xrns2MidiApp
{
    public partial class Xrns2MidiForm : Form
    {
        public Xrns2MidiForm()
        {
            InitializeComponent();
        }

        private void ButtonSelectXrnsFile_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = openFileDialogXnrs.ShowDialog();
            if ( dialogResult == System.Windows.Forms.DialogResult.OK )
            {
                textBoxXrnsFileName.Text = openFileDialogXnrs.FileName;

                textBoxMidiFileName.Text =
                    openFileDialogXnrs.FileName.Substring(0, openFileDialogXnrs.FileName.Length - ".xrns".Length) +
                    ".mid";
                saveFileDialogMidi.FileName = textBoxMidiFileName.Text;                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            logTextBox.Text = "";
            TextBoxWriter textBoxWriter = new TextBoxWriter(logTextBox);
            if (Xrns2Midi.ConvertFile(textBoxXrnsFileName.Text, textBoxMidiFileName.Text, textBoxWriter))
            {
                MessageBox.Show(this, "Convert successfull", "Convertion result", MessageBoxButtons.OK);
            } else
            {
                MessageBox.Show(this, "Convertion error", "Convertion error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonSelectMidiFile_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = saveFileDialogMidi.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                textBoxMidiFileName.Text = saveFileDialogMidi.FileName;
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
