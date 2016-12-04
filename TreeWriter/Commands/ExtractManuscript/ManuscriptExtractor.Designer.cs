namespace TreeWriterWF.Commands.Extract
{
    partial class ManuscriptExtractor
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
            this.components = new System.ComponentModel.Container();
            this.buttonExtract = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.extractionSettings = new System.Windows.Forms.PropertyGrid();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonExtract
            // 
            this.buttonExtract.AutoSize = true;
            this.buttonExtract.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonExtract.Location = new System.Drawing.Point(400, 0);
            this.buttonExtract.Name = "buttonExtract";
            this.buttonExtract.Size = new System.Drawing.Size(75, 26);
            this.buttonExtract.TabIndex = 4;
            this.buttonExtract.Text = "Extract";
            this.buttonExtract.UseVisualStyleBackColor = true;
            this.buttonExtract.Click += new System.EventHandler(this.buttonExtract_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonExtract);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 414);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(475, 26);
            this.panel1.TabIndex = 5;
            // 
            // extractionSettings
            // 
            this.extractionSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.extractionSettings.Location = new System.Drawing.Point(0, 0);
            this.extractionSettings.Name = "extractionSettings";
            this.extractionSettings.Size = new System.Drawing.Size(475, 414);
            this.extractionSettings.TabIndex = 6;
            // 
            // ManuscriptExtractor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 440);
            this.Controls.Add(this.extractionSettings);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ManuscriptExtractor";
            this.Text = "Extract Manuscript";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NoteList_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonExtract;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PropertyGrid extractionSettings;



    }
}