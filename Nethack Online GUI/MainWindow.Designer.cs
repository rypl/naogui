using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Nethack_Online_GUI
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.Messages = new System.Windows.Forms.TextBox();
            this.Status = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Messages
            // 
            this.Messages.Cursor = System.Windows.Forms.Cursors.Default;
            this.Messages.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Messages.Location = new System.Drawing.Point(10, 9);
            this.Messages.Margin = new System.Windows.Forms.Padding(0);
            this.Messages.Multiline = true;
            this.Messages.Name = "Messages";
            this.Messages.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Messages.Size = new System.Drawing.Size(1280, 90);
            this.Messages.TabIndex = 0;
            // 
            // Status
            // 
            this.Status.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Status.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Status.Font = new System.Drawing.Font("Arial", 12F);
            this.Status.Location = new System.Drawing.Point(10, 508);
            this.Status.Multiline = true;
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(1280, 45);
            this.Status.TabIndex = 1;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1302, 567);
            this.Controls.Add(this.Status);
            this.Controls.Add(this.Messages);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "Nethack Online GUI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Messages;
        private System.Windows.Forms.TextBox Status;
    }
}