namespace CustomDataGrid
{
    partial class MainForm
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
            this.dgExample = new System.Windows.Forms.DataGrid();
            this.dgCustom = new CustomDataGrid();
            ((System.ComponentModel.ISupportInitialize)(this.dgExample)).BeginInit();
            this.SuspendLayout();
            // 
            // dgExample
            // 
            this.dgExample.DataMember = "";
            this.dgExample.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgExample.Location = new System.Drawing.Point(12, 12);
            this.dgExample.Name = "dgExample";
            this.dgExample.Size = new System.Drawing.Size(500, 180);
            this.dgExample.TabIndex = 1;
            // 
            // dgCustom
            // 
            this.dgCustom.AutoScroll = true;
            this.dgCustom.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.dgCustom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dgCustom.CaptionBackColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgCustom.CaptionFont = new System.Drawing.Font("Microsoft Sans Serif", 10.25F);
            this.dgCustom.CaptionForeColor = System.Drawing.SystemColors.ControlText;
            this.dgCustom.CaptionText = "";
            this.dgCustom.HeaderBackColor = System.Drawing.SystemColors.Control;
            this.dgCustom.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgCustom.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgCustom.Location = new System.Drawing.Point(12, 207);
            this.dgCustom.Name = "dgCustom";
            this.dgCustom.PreferredColumnWidth = 75;
            this.dgCustom.PreferredRowHeight = 16;
            this.dgCustom.ReadOnly = true;
            this.dgCustom.RowHeadersVisible = false;
            this.dgCustom.RowHeaderWidth = 35;
            this.dgCustom.Size = new System.Drawing.Size(500, 180);
            this.dgCustom.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 398);
            this.Controls.Add(this.dgExample);
            this.Controls.Add(this.dgCustom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "CustomDataGrid Example";
            ((System.ComponentModel.ISupportInitialize)(this.dgExample)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private CustomDataGrid dgCustom;
        private System.Windows.Forms.DataGrid dgExample;
    }
}

