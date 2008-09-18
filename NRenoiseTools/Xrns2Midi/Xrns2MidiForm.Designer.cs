namespace NRenoiseTools.Xrns2MidiApp
{
    partial class Xrns2MidiForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Xrns2MidiForm));
            this.textBoxXrnsFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonSelectXrnsFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxMidiFileName = new System.Windows.Forms.TextBox();
            this.ButtonSelectMidiFile = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.openFileDialogXnrs = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogMidi = new System.Windows.Forms.SaveFileDialog();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxXrnsFileName
            // 
            this.textBoxXrnsFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxXrnsFileName.Location = new System.Drawing.Point(101, 6);
            this.textBoxXrnsFileName.Name = "textBoxXrnsFileName";
            this.textBoxXrnsFileName.Size = new System.Drawing.Size(384, 20);
            this.textBoxXrnsFileName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "XRNS Input File";
            // 
            // ButtonSelectXrnsFile
            // 
            this.ButtonSelectXrnsFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonSelectXrnsFile.Image = global::NRenoiseTools.Xrns2MidiApp.Properties.Resources.drafts_open;
            this.ButtonSelectXrnsFile.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ButtonSelectXrnsFile.Location = new System.Drawing.Point(491, 4);
            this.ButtonSelectXrnsFile.Name = "ButtonSelectXrnsFile";
            this.ButtonSelectXrnsFile.Size = new System.Drawing.Size(99, 23);
            this.ButtonSelectXrnsFile.TabIndex = 2;
            this.ButtonSelectXrnsFile.Text = "Select...";
            this.ButtonSelectXrnsFile.UseVisualStyleBackColor = true;
            this.ButtonSelectXrnsFile.Click += new System.EventHandler(this.ButtonSelectXrnsFile_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "MIDI Output File";
            // 
            // textBoxMidiFileName
            // 
            this.textBoxMidiFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMidiFileName.Location = new System.Drawing.Point(101, 38);
            this.textBoxMidiFileName.Name = "textBoxMidiFileName";
            this.textBoxMidiFileName.Size = new System.Drawing.Size(384, 20);
            this.textBoxMidiFileName.TabIndex = 4;
            // 
            // ButtonSelectMidiFile
            // 
            this.ButtonSelectMidiFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonSelectMidiFile.Image = global::NRenoiseTools.Xrns2MidiApp.Properties.Resources.drafts_open;
            this.ButtonSelectMidiFile.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ButtonSelectMidiFile.Location = new System.Drawing.Point(491, 36);
            this.ButtonSelectMidiFile.Name = "ButtonSelectMidiFile";
            this.ButtonSelectMidiFile.Size = new System.Drawing.Size(99, 23);
            this.ButtonSelectMidiFile.TabIndex = 5;
            this.ButtonSelectMidiFile.Text = "Select...";
            this.ButtonSelectMidiFile.UseVisualStyleBackColor = true;
            this.ButtonSelectMidiFile.Click += new System.EventHandler(this.ButtonSelectMidiFile_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Image = global::NRenoiseTools.Xrns2MidiApp.Properties.Resources.go;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(491, 64);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(99, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Convert";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // openFileDialogXnrs
            // 
            this.openFileDialogXnrs.Filter = "XRNS Files|*.xrns";
            this.openFileDialogXnrs.Title = "Open Input XRNS file to convert";
            // 
            // saveFileDialogMidi
            // 
            this.saveFileDialogMidi.DefaultExt = "mid";
            this.saveFileDialogMidi.Filter = "MIDI Files|*.mid";
            this.saveFileDialogMidi.Title = "Save converted Midi file to";
            // 
            // logTextBox
            // 
            this.logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.logTextBox.Location = new System.Drawing.Point(15, 93);
            this.logTextBox.MaxLength = 327670;
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logTextBox.Size = new System.Drawing.Size(575, 415);
            this.logTextBox.TabIndex = 7;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(515, 514);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 8;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // Xrns2MidiForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 541);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.ButtonSelectMidiFile);
            this.Controls.Add(this.textBoxMidiFileName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ButtonSelectXrnsFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxXrnsFileName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Xrns2MidiForm";
            this.Text = "XRNS To Midi Converter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxXrnsFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonSelectXrnsFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxMidiFileName;
        private System.Windows.Forms.Button ButtonSelectMidiFile;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.OpenFileDialog openFileDialogXnrs;
        private System.Windows.Forms.SaveFileDialog saveFileDialogMidi;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.Button closeButton;
    }
}

