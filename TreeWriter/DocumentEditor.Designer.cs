namespace TreeWriterWF
{
    partial class DocumentEditor
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
            this.textEditor = new ScintillaNET.Scintilla();
            this.SuspendLayout();
            // 
            // textEditor
            // 
            this.textEditor.AutomaticFold = ((ScintillaNET.AutomaticFold)(((ScintillaNET.AutomaticFold.Show | ScintillaNET.AutomaticFold.Click) 
            | ScintillaNET.AutomaticFold.Change)));
            this.textEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditor.Location = new System.Drawing.Point(0, 0);
            this.textEditor.Name = "textEditor";
            this.textEditor.Size = new System.Drawing.Size(475, 440);
            this.textEditor.TabIndex = 0;
            this.textEditor.Text = "scintilla1";
            this.textEditor.WrapMode = ScintillaNET.WrapMode.Word;
            this.textEditor.Zoom = 10;
            this.textEditor.HotspotClick += new System.EventHandler<ScintillaNET.HotspotClickEventArgs>(this.textEditor_HotspotClick);
            this.textEditor.StyleNeeded += new System.EventHandler<ScintillaNET.StyleNeededEventArgs>(this.textEditor_StyleNeeded);
            // 
            // DocumentEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 440);
            this.Controls.Add(this.textEditor);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DocumentEditor";
            this.Text = "DocumentEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DocumentEditor_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private ScintillaNET.Scintilla textEditor;

    }
}