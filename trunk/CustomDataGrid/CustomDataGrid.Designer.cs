namespace CustomDataGrid
{
    partial class CustomDataGrid
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.vSB = new System.Windows.Forms.VScrollBar();
            this.SuspendLayout();
            // 
            // vSB
            // 
            this.vSB.Location = new System.Drawing.Point(114, 25);
            this.vSB.Name = "vSB";
            this.vSB.Size = new System.Drawing.Size(17, 80);
            this.vSB.TabIndex = 0;
            this.vSB.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vSB_Scroll);
            // 
            // CustomDataGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.vSB);
            this.Name = "CustomDataGrid";
            this.Size = new System.Drawing.Size(148, 148);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CustomDataGrid_MouseMove);
            this.Leave += new System.EventHandler(this.CustomDataGrid_Leave);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CustomDataGrid_MouseClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CustomDataGrid_MouseDown);
            this.Resize += new System.EventHandler(this.CustomDataGrid_Resize);
            this.Enter += new System.EventHandler(this.CustomDataGrid_Enter);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CustomDataGrid_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.VScrollBar vSB;

    }
}
