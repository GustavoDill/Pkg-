namespace Pkg__
{
    partial class Main
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
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smoothButton1 = new CSharpExtendedCommands.UI.SmoothButton();
            this.list = new CSharpExtendedCommands.UI.ControlListBox();
            this.smoothButton2 = new CSharpExtendedCommands.UI.SmoothButton();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            // 
            // smoothButton1
            // 
            this.smoothButton1.ActiveColor = System.Drawing.Color.RoyalBlue;
            this.smoothButton1.BackColor = System.Drawing.Color.DodgerBlue;
            this.smoothButton1.BorderRadius = 8;
            this.smoothButton1.ForeColor = System.Drawing.Color.White;
            this.smoothButton1.HoverColor = System.Drawing.Color.RoyalBlue;
            this.smoothButton1.Location = new System.Drawing.Point(14, 24);
            this.smoothButton1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.smoothButton1.Name = "smoothButton1";
            this.smoothButton1.NormalColor = System.Drawing.Color.DodgerBlue;
            this.smoothButton1.Size = new System.Drawing.Size(233, 37);
            this.smoothButton1.TabIndex = 5;
            this.smoothButton1.Text = "Load Solution";
            this.smoothButton1.UseVisualStyleBackColor = false;
            this.smoothButton1.Click += new System.EventHandler(this.smoothButton1_Click);
            // 
            // list
            // 
            this.list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.list.AutoScroll = true;
            this.list.BackColor = System.Drawing.Color.White;
            this.list.Font = new System.Drawing.Font("Miriam Libre", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.list.ItemSize = new System.Drawing.Size(0, 0);
            this.list.Location = new System.Drawing.Point(14, 84);
            this.list.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.list.Name = "list";
            this.list.Size = new System.Drawing.Size(905, 446);
            this.list.SpaceFirstItem = false;
            this.list.Spacing = 1;
            this.list.TabIndex = 4;
            this.list.Xoffset = 0;
            // 
            // smoothButton2
            // 
            this.smoothButton2.ActiveColor = System.Drawing.Color.RoyalBlue;
            this.smoothButton2.BackColor = System.Drawing.Color.DodgerBlue;
            this.smoothButton2.BorderRadius = 8;
            this.smoothButton2.ForeColor = System.Drawing.Color.White;
            this.smoothButton2.HoverColor = System.Drawing.Color.RoyalBlue;
            this.smoothButton2.Location = new System.Drawing.Point(427, 24);
            this.smoothButton2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.smoothButton2.Name = "smoothButton2";
            this.smoothButton2.NormalColor = System.Drawing.Color.DodgerBlue;
            this.smoothButton2.Size = new System.Drawing.Size(233, 37);
            this.smoothButton2.TabIndex = 6;
            this.smoothButton2.Text = "Load Solution";
            this.smoothButton2.UseVisualStyleBackColor = false;
            this.smoothButton2.Click += new System.EventHandler(this.smoothButton2_Click_2);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 554);
            this.Controls.Add(this.smoothButton2);
            this.Controls.Add(this.smoothButton1);
            this.Controls.Add(this.list);
            this.Font = new System.Drawing.Font("Miriam Libre", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pkg++";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private CSharpExtendedCommands.UI.SmoothButton smoothButton1;
        private CSharpExtendedCommands.UI.ControlListBox list;
        private CSharpExtendedCommands.UI.SmoothButton smoothButton2;
    }
}

