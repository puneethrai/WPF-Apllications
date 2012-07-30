namespace WebBrowserEmulator
{
    partial class WebBowserEmulator
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
                // Release the icon resource.
                trayIcon.Dispose();
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
            this.WebprogressBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.InvokeprogressBar = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // WebprogressBar
            // 
            this.WebprogressBar.Location = new System.Drawing.Point(10, 100);
            this.WebprogressBar.Name = "WebprogressBar";
            this.WebprogressBar.Size = new System.Drawing.Size(263, 22);
            this.WebprogressBar.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Emulation";
            // 
            // InvokeprogressBar
            // 
            
            this.InvokeprogressBar.Location = new System.Drawing.Point(11, 34);
            this.InvokeprogressBar.ForeColor = System.Drawing.Color.FromArgb(0, 255, 0);
                       
            this.InvokeprogressBar.Name = "InvokeprogressBar";
            this.InvokeprogressBar.Size = new System.Drawing.Size(262, 23);
            this.InvokeprogressBar.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Current Emulation";
            // 
            // WebBowserEmulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 134);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.WebprogressBar);
            this.Controls.Add(this.InvokeprogressBar);
            this.Controls.Add(this.label1);
            this.Name = "WebBowserEmulator";
            this.Text = "Web Browser Emulator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar WebprogressBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar InvokeprogressBar;
        private System.Windows.Forms.Label label2;
    }
}

