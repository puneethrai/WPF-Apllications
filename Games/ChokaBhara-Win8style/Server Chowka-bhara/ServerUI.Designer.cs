namespace Server_Chowka_bhara
{
    partial class ServerUI
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
            this.ServerMessage = new System.Windows.Forms.TextBox();
            this.ConnectedUser = new System.Windows.Forms.StatusStrip();
            this.Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.ServerTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Rooms = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.RefreshBtn = new System.Windows.Forms.Button();
            this.Room_1 = new System.Windows.Forms.CheckedListBox();
            this.ConnectedUser.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ServerMessage
            // 
            this.ServerMessage.Location = new System.Drawing.Point(11, 12);
            this.ServerMessage.Multiline = true;
            this.ServerMessage.Name = "ServerMessage";
            this.ServerMessage.Size = new System.Drawing.Size(325, 20);
            this.ServerMessage.TabIndex = 0;
            // 
            // ConnectedUser
            // 
            this.ConnectedUser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Status,
            this.ServerTime});
            this.ConnectedUser.Location = new System.Drawing.Point(0, 277);
            this.ConnectedUser.Name = "ConnectedUser";
            this.ConnectedUser.Size = new System.Drawing.Size(357, 22);
            this.ConnectedUser.TabIndex = 1;
            this.ConnectedUser.Text = "ConectedUser";
            // 
            // Status
            // 
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(112, 17);
            this.Status.Text = "No. of connection:0";
            // 
            // ServerTime
            // 
            this.ServerTime.Name = "ServerTime";
            this.ServerTime.Size = new System.Drawing.Size(230, 17);
            this.ServerTime.Spring = true;
            this.ServerTime.Text = "Server UPtime:00:00:00";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(13, 60);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(322, 214);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Room Information";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Rooms);
            this.groupBox3.Location = new System.Drawing.Point(156, 29);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(156, 179);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Room Detail";
            // 
            // Rooms
            // 
            this.Rooms.FormattingEnabled = true;
            this.Rooms.Location = new System.Drawing.Point(6, 24);
            this.Rooms.Name = "Rooms";
            this.Rooms.Size = new System.Drawing.Size(144, 147);
            this.Rooms.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.RefreshBtn);
            this.groupBox2.Controls.Add(this.Room_1);
            this.groupBox2.Location = new System.Drawing.Point(11, 29);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(133, 179);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Room No";
            // 
            // Refresh
            // 
            this.RefreshBtn.Location = new System.Drawing.Point(29, 150);
            this.RefreshBtn.Name = "Refresh";
            this.RefreshBtn.Size = new System.Drawing.Size(77, 23);
            this.RefreshBtn.TabIndex = 2;
            this.RefreshBtn.Text = "Refresh";
            this.RefreshBtn.UseVisualStyleBackColor = true;
            this.RefreshBtn.Click += new System.EventHandler(this.RefreshBtn_Click);
            // 
            // Room_1
            // 
            this.Room_1.FormattingEnabled = true;
            this.Room_1.Location = new System.Drawing.Point(6, 23);
            this.Room_1.Name = "Room_1";
            this.Room_1.Size = new System.Drawing.Size(121, 124);
            this.Room_1.TabIndex = 1;
            this.Room_1.SelectedIndexChanged += new System.EventHandler(this.Room_1_SelectedIndexChanged);
            // 
            // Server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 299);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ConnectedUser);
            this.Controls.Add(this.ServerMessage);
            this.Name = "Server";
            this.Text = "Choka Bhara Server";
            this.ConnectedUser.ResumeLayout(false);
            this.ConnectedUser.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
        private System.Windows.Forms.TextBox ServerMessage;
        private System.Windows.Forms.StatusStrip ConnectedUser;
        private System.Windows.Forms.ToolStripStatusLabel Status;
        private System.Windows.Forms.ToolStripStatusLabel ServerTime;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox Rooms;
        private System.Windows.Forms.CheckedListBox Room_1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button RefreshBtn;
    }
}