namespace NRenoiseTools.Xrns2XrniApp
{
    partial class Xrns2XrniForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Xrns2XrniForm));
            this.ButtonSelectXrnsFile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxXrnsFileName = new System.Windows.Forms.TextBox();
            this.LogTextBox = new System.Windows.Forms.TextBox();
            this.ConvertButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.openFileXRNSDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // ButtonSelectXrnsFile
            // 
            this.ButtonSelectXrnsFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonSelectXrnsFile.Image = global::NRenoiseTools.Xrns2XrniApp.Properties.Resources.drafts_open;
            this.ButtonSelectXrnsFile.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ButtonSelectXrnsFile.Location = new System.Drawing.Point(451, 4);
            this.ButtonSelectXrnsFile.Name = "ButtonSelectXrnsFile";
            this.ButtonSelectXrnsFile.Size = new System.Drawing.Size(99, 23);
            this.ButtonSelectXrnsFile.TabIndex = 5;
            this.ButtonSelectXrnsFile.Text = "Select...";
            this.ButtonSelectXrnsFile.UseVisualStyleBackColor = true;
            this.ButtonSelectXrnsFile.Click += new System.EventHandler(this.ButtonSelectXrnsFile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "XRNS Input File";
            // 
            // textBoxXrnsFileName
            // 
            this.textBoxXrnsFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxXrnsFileName.Location = new System.Drawing.Point(101, 6);
            this.textBoxXrnsFileName.Name = "textBoxXrnsFileName";
            this.textBoxXrnsFileName.Size = new System.Drawing.Size(344, 20);
            this.textBoxXrnsFileName.TabIndex = 3;
            // 
            // LogTextBox
            // 
            this.LogTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.LogTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.LogTextBox.Location = new System.Drawing.Point(12, 32);
            this.LogTextBox.MaxLength = 655350;
            this.LogTextBox.Multiline = true;
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.ReadOnly = true;
            this.LogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogTextBox.Size = new System.Drawing.Size(644, 429);
            this.LogTextBox.TabIndex = 6;
            // 
            // ConvertButton
            // 
            this.ConvertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ConvertButton.Image = global::NRenoiseTools.Xrns2XrniApp.Properties.Resources.go;
            this.ConvertButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ConvertButton.Location = new System.Drawing.Point(556, 4);
            this.ConvertButton.Name = "ConvertButton";
            this.ConvertButton.Size = new System.Drawing.Size(100, 23);
            this.ConvertButton.TabIndex = 7;
            this.ConvertButton.Text = "Convert";
            this.ConvertButton.UseVisualStyleBackColor = true;
            this.ConvertButton.Click += new System.EventHandler(this.ConvertButton_Click);
            // 
            // CloseButton
            // 
            this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseButton.Location = new System.Drawing.Point(581, 467);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 8;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // openFileXRNSDialog
            // 
            this.openFileXRNSDialog.DefaultExt = "xrns";
            this.openFileXRNSDialog.Filter = "XRNS Renoise Song|*.xrns";
            this.openFileXRNSDialog.Title = "Select XRNS Renoise Song to extract XRNI instruments from";
            // 
            // Xrns2XrniForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 495);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.ConvertButton);
            this.Controls.Add(this.LogTextBox);
            this.Controls.Add(this.ButtonSelectXrnsFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxXrnsFileName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Xrns2XrniForm";
            this.Text = "Xrns2XrniForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonSelectXrnsFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxXrnsFileName;
        private System.Windows.Forms.TextBox LogTextBox;
        private System.Windows.Forms.Button ConvertButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.OpenFileDialog openFileXRNSDialog;
    }
}