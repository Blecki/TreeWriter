namespace TreeWriterWF
{
    partial class DistractionFreeEditor
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
            this.leftSplit = new System.Windows.Forms.SplitContainer();
            this.rightSplit = new System.Windows.Forms.SplitContainer();
            this.textEditor = new TreeWriterWF.TextEditor();
            ((System.ComponentModel.ISupportInitialize)(this.leftSplit)).BeginInit();
            this.leftSplit.Panel2.SuspendLayout();
            this.leftSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightSplit)).BeginInit();
            this.rightSplit.Panel2.SuspendLayout();
            this.rightSplit.SuspendLayout();
            this.SuspendLayout();
            // 
            // leftSplit
            // 
            this.leftSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.leftSplit.Location = new System.Drawing.Point(0, 0);
            this.leftSplit.Name = "leftSplit";
            // 
            // leftSplit.Panel2
            // 
            this.leftSplit.Panel2.Controls.Add(this.rightSplit);
            this.leftSplit.Size = new System.Drawing.Size(597, 490);
            this.leftSplit.SplitterDistance = 40;
            this.leftSplit.TabIndex = 1;
            // 
            // rightSplit
            // 
            this.rightSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.rightSplit.Location = new System.Drawing.Point(0, 0);
            this.rightSplit.Name = "rightSplit";
            // 
            // rightSplit.Panel1
            // 
            this.rightSplit.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            // 
            // rightSplit.Panel2
            // 
            this.rightSplit.Panel2.Controls.Add(this.textEditor);
            this.rightSplit.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.rightSplit.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.rightSplit.Size = new System.Drawing.Size(553, 490);
            this.rightSplit.SplitterDistance = 40;
            this.rightSplit.TabIndex = 0;
            // 
            // textEditor
            // 
            this.textEditor.AnnotationVisible = ScintillaNET.Annotation.Boxed;
            this.textEditor.AutomaticFold = ((ScintillaNET.AutomaticFold)(((ScintillaNET.AutomaticFold.Show | ScintillaNET.AutomaticFold.Click) 
            | ScintillaNET.AutomaticFold.Change)));
            this.textEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditor.HScrollBar = false;
            this.textEditor.IdleStyling = ScintillaNET.IdleStyling.ToVisible;
            this.textEditor.Location = new System.Drawing.Point(0, 0);
            this.textEditor.Name = "textEditor";
            this.textEditor.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.textEditor.Size = new System.Drawing.Size(509, 490);
            this.textEditor.TabIndex = 0;
            this.textEditor.Text = "Test Text";
            this.textEditor.WrapMode = ScintillaNET.WrapMode.Word;
            // 
            // DistractionFreeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 490);
            this.Controls.Add(this.leftSplit);
            this.Name = "DistractionFreeEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.leftSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.leftSplit)).EndInit();
            this.leftSplit.ResumeLayout(false);
            this.rightSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rightSplit)).EndInit();
            this.rightSplit.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TextEditor textEditor;
        private System.Windows.Forms.SplitContainer leftSplit;
        private System.Windows.Forms.SplitContainer rightSplit;
    }
}