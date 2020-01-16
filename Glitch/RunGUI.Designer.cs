using System.ComponentModel;

namespace Glitch
{
    partial class RunGUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(RunGUI));
            this.btnPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // btnPanel
            // 
            this.btnPanel.BackColor = System.Drawing.SystemColors.Window;
            this.btnPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPanel.Location = new System.Drawing.Point(0, 0);
            this.btnPanel.Name = "btnPanel";
            this.btnPanel.Size = new System.Drawing.Size(721, 209);
            this.btnPanel.TabIndex = 0;
            // 
            // RunGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 209);
            this.Controls.Add(this.btnPanel);
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.Name = "RunGUI";
            this.Text = "CC24 Glitch - Payloads are enabled. Press LShift+Esc to toggle";
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel btnPanel;
    }
}