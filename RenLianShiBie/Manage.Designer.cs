namespace RenLianShiBie
{
    partial class Manage
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
            this.label1 = new System.Windows.Forms.Label();
            this.SheBeiTianJia = new System.Windows.Forms.Button();
            this.BianJi = new System.Windows.Forms.Button();
            this.ShanChu = new System.Windows.Forms.Button();
            this.SouSuo = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.MenuPanel = new System.Windows.Forms.Panel();
            this.SheBeiViewPanel = new System.Windows.Forms.Panel();
            this.SheBeiView = new System.Windows.Forms.ListView();
            this.ID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TTMachine = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TTIPADDR = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.MenuPanel.SuspendLayout();
            this.SheBeiViewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(140)))), ((int)(((byte)(230)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(47, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(195, 33);
            this.label1.TabIndex = 7;
            this.label1.Text = "人脸识别系统";
            // 
            // SheBeiTianJia
            // 
            this.SheBeiTianJia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(157)))), ((int)(((byte)(249)))));
            this.SheBeiTianJia.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.SheBeiTianJia.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SheBeiTianJia.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SheBeiTianJia.ForeColor = System.Drawing.Color.Snow;
            this.SheBeiTianJia.Location = new System.Drawing.Point(3, 2);
            this.SheBeiTianJia.Name = "SheBeiTianJia";
            this.SheBeiTianJia.Size = new System.Drawing.Size(91, 35);
            this.SheBeiTianJia.TabIndex = 8;
            this.SheBeiTianJia.Text = "添加";
            this.SheBeiTianJia.UseVisualStyleBackColor = false;
            this.SheBeiTianJia.Click += new System.EventHandler(this.SheBeiTianJia_Click);
            // 
            // BianJi
            // 
            this.BianJi.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(157)))), ((int)(((byte)(249)))));
            this.BianJi.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.BianJi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BianJi.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BianJi.ForeColor = System.Drawing.Color.Snow;
            this.BianJi.Location = new System.Drawing.Point(3, 37);
            this.BianJi.Name = "BianJi";
            this.BianJi.Size = new System.Drawing.Size(91, 35);
            this.BianJi.TabIndex = 8;
            this.BianJi.Text = "编辑";
            this.BianJi.UseVisualStyleBackColor = false;
            this.BianJi.Click += new System.EventHandler(this.BianJi_Click);
            // 
            // ShanChu
            // 
            this.ShanChu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(157)))), ((int)(((byte)(249)))));
            this.ShanChu.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.ShanChu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ShanChu.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ShanChu.ForeColor = System.Drawing.Color.Snow;
            this.ShanChu.Location = new System.Drawing.Point(3, 71);
            this.ShanChu.Name = "ShanChu";
            this.ShanChu.Size = new System.Drawing.Size(91, 35);
            this.ShanChu.TabIndex = 8;
            this.ShanChu.Text = "刪除";
            this.ShanChu.UseVisualStyleBackColor = false;
            // 
            // SouSuo
            // 
            this.SouSuo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(157)))), ((int)(((byte)(249)))));
            this.SouSuo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.SouSuo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SouSuo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SouSuo.ForeColor = System.Drawing.Color.Snow;
            this.SouSuo.Location = new System.Drawing.Point(3, 103);
            this.SouSuo.Name = "SouSuo";
            this.SouSuo.Size = new System.Drawing.Size(91, 35);
            this.SouSuo.TabIndex = 8;
            this.SouSuo.Text = "搜索";
            this.SouSuo.UseVisualStyleBackColor = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(140)))), ((int)(((byte)(230)))));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1073, 63);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // MenuPanel
            // 
            this.MenuPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.MenuPanel.Controls.Add(this.SheBeiTianJia);
            this.MenuPanel.Controls.Add(this.SouSuo);
            this.MenuPanel.Controls.Add(this.BianJi);
            this.MenuPanel.Controls.Add(this.ShanChu);
            this.MenuPanel.Location = new System.Drawing.Point(12, 69);
            this.MenuPanel.Name = "MenuPanel";
            this.MenuPanel.Size = new System.Drawing.Size(97, 143);
            this.MenuPanel.TabIndex = 9;
            // 
            // SheBeiViewPanel
            // 
            this.SheBeiViewPanel.Controls.Add(this.SheBeiView);
            this.SheBeiViewPanel.Location = new System.Drawing.Point(117, 69);
            this.SheBeiViewPanel.Name = "SheBeiViewPanel";
            this.SheBeiViewPanel.Size = new System.Drawing.Size(816, 343);
            this.SheBeiViewPanel.TabIndex = 10;
            // 
            // SheBeiView
            // 
            this.SheBeiView.CheckBoxes = true;
            this.SheBeiView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ID,
            this.TTMachine,
            this.TTIPADDR});
            this.SheBeiView.FullRowSelect = true;
            this.SheBeiView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.SheBeiView.Location = new System.Drawing.Point(1, 1);
            this.SheBeiView.Name = "SheBeiView";
            this.SheBeiView.Size = new System.Drawing.Size(809, 337);
            this.SheBeiView.TabIndex = 0;
            this.SheBeiView.UseCompatibleStateImageBehavior = false;
            this.SheBeiView.View = System.Windows.Forms.View.Details;
            // 
            // ID
            // 
            this.ID.Text = "ID";
            this.ID.Width = 80;
            // 
            // TTMachine
            // 
            this.TTMachine.Text = "设备名称";
            this.TTMachine.Width = 266;
            // 
            // TTIPADDR
            // 
            this.TTIPADDR.Text = "IP地址";
            this.TTIPADDR.Width = 316;
            // 
            // Manage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(251)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1136, 622);
            this.Controls.Add(this.SheBeiViewPanel);
            this.Controls.Add(this.MenuPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Manage";
            this.Text = "人臉識別係統";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Manage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.MenuPanel.ResumeLayout(false);
            this.SheBeiViewPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button SheBeiTianJia;
        private System.Windows.Forms.Button BianJi;
        private System.Windows.Forms.Button ShanChu;
        private System.Windows.Forms.Button SouSuo;
        private System.Windows.Forms.Panel MenuPanel;
        private System.Windows.Forms.Panel SheBeiViewPanel;
        private System.Windows.Forms.ListView SheBeiView;
        private System.Windows.Forms.ColumnHeader ID;
        private System.Windows.Forms.ColumnHeader TTMachine;
        private System.Windows.Forms.ColumnHeader TTIPADDR;
    }
}