namespace TreeWriterWF
{
    partial class Find
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
            System.Windows.Forms.Panel filterPanel;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Find));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.filterBox = new System.Windows.Forms.TextBox();
            this.clearFilterButton = new System.Windows.Forms.Button();
            this.refreshFilterButton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            filterPanel = new System.Windows.Forms.Panel();
            filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // filterPanel
            // 
            filterPanel.Controls.Add(this.filterBox);
            filterPanel.Controls.Add(this.clearFilterButton);
            filterPanel.Controls.Add(this.refreshFilterButton);
            filterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            filterPanel.Location = new System.Drawing.Point(0, 0);
            filterPanel.MinimumSize = new System.Drawing.Size(2, 26);
            filterPanel.Name = "filterPanel";
            filterPanel.Padding = new System.Windows.Forms.Padding(3);
            filterPanel.Size = new System.Drawing.Size(475, 26);
            filterPanel.TabIndex = 3;
            // 
            // filterBox
            // 
            this.filterBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filterBox.Location = new System.Drawing.Point(3, 3);
            this.filterBox.Name = "filterBox";
            this.filterBox.Size = new System.Drawing.Size(429, 20);
            this.filterBox.TabIndex = 1;
            // 
            // clearFilterButton
            // 
            this.clearFilterButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("clearFilterButton.BackgroundImage")));
            this.clearFilterButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.clearFilterButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.clearFilterButton.FlatAppearance.BorderSize = 0;
            this.clearFilterButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearFilterButton.Location = new System.Drawing.Point(432, 3);
            this.clearFilterButton.MaximumSize = new System.Drawing.Size(20, 20);
            this.clearFilterButton.Name = "clearFilterButton";
            this.clearFilterButton.Size = new System.Drawing.Size(20, 20);
            this.clearFilterButton.TabIndex = 2;
            this.clearFilterButton.UseVisualStyleBackColor = true;
            this.clearFilterButton.Click += new System.EventHandler(this.clearFilterButton_Click);
            // 
            // refreshFilterButton
            // 
            this.refreshFilterButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("refreshFilterButton.BackgroundImage")));
            this.refreshFilterButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.refreshFilterButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.refreshFilterButton.FlatAppearance.BorderSize = 0;
            this.refreshFilterButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshFilterButton.Location = new System.Drawing.Point(452, 3);
            this.refreshFilterButton.MaximumSize = new System.Drawing.Size(20, 20);
            this.refreshFilterButton.Name = "refreshFilterButton";
            this.refreshFilterButton.Size = new System.Drawing.Size(20, 20);
            this.refreshFilterButton.TabIndex = 3;
            this.refreshFilterButton.UseVisualStyleBackColor = true;
            this.refreshFilterButton.Click += new System.EventHandler(this.refreshFilterButton_Click);
            // 
            // dataGridView1
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ColumnHeadersVisible = false;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 26);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(475, 414);
            this.dataGridView1.TabIndex = 4;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Find
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 440);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(filterPanel);
            this.Name = "Find";
            this.Text = "#settings";
            filterPanel.ResumeLayout(false);
            filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox filterBox;
        private System.Windows.Forms.Button clearFilterButton;
        private System.Windows.Forms.Button refreshFilterButton;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
    }
}