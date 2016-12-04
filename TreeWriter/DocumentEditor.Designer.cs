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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.duplicateViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wordCountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveDocumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.duplicateViewToolStripMenuItem,
            this.wordCountToolStripMenuItem,
            this.saveDocumentToolStripMenuItem,
            this.closeEditorToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(158, 114);
            // 
            // duplicateViewToolStripMenuItem
            // 
            this.duplicateViewToolStripMenuItem.Name = "duplicateViewToolStripMenuItem";
            this.duplicateViewToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.duplicateViewToolStripMenuItem.Text = "Duplicate View";
            this.duplicateViewToolStripMenuItem.Click += new System.EventHandler(this.duplicateViewToolStripMenuItem_Click);
            // 
            // wordCountToolStripMenuItem
            // 
            this.wordCountToolStripMenuItem.Name = "wordCountToolStripMenuItem";
            this.wordCountToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.wordCountToolStripMenuItem.Text = "Word Count";
            this.wordCountToolStripMenuItem.Click += new System.EventHandler(this.wordCountToolStripMenuItem_Click);
            // 
            // saveDocumentToolStripMenuItem
            // 
            this.saveDocumentToolStripMenuItem.Name = "saveDocumentToolStripMenuItem";
            this.saveDocumentToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.saveDocumentToolStripMenuItem.Text = "Save Document";
            this.saveDocumentToolStripMenuItem.Click += new System.EventHandler(this.saveDocumentToolStripMenuItem_Click);
            // 
            // closeEditorToolStripMenuItem
            // 
            this.closeEditorToolStripMenuItem.Name = "closeEditorToolStripMenuItem";
            this.closeEditorToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.closeEditorToolStripMenuItem.Text = "Close Editor";
            this.closeEditorToolStripMenuItem.Click += new System.EventHandler(this.closeEditorToolStripMenuItem_Click);
            // 
            // DocumentEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 440);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DocumentEditor";
            this.TabPageContextMenuStrip = this.contextMenuStrip1;
            this.Text = "DocumentEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DocumentEditor_FormClosing);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem duplicateViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wordCountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveDocumentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeEditorToolStripMenuItem;

    }
}