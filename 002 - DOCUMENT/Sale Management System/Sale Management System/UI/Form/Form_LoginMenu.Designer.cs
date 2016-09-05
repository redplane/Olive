namespace Sale_Management_System
{
    partial class Form_LoginMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_LoginMenu));
            this.TXT_DataSource = new System.Windows.Forms.TextBox();
            this.LBL_DataSource = new System.Windows.Forms.Label();
            this.BTN_Login = new System.Windows.Forms.Button();
            this.LBL_Catalog = new System.Windows.Forms.Label();
            this.TXT_Catalog = new System.Windows.Forms.TextBox();
            this.MS_MenuStripId = new System.Windows.Forms.MenuStrip();
            this.TSP_MenuOption = new System.Windows.Forms.ToolStripMenuItem();
            this.TSP_ProgramExit = new System.Windows.Forms.ToolStripMenuItem();
            this.MS_MenuStripId.SuspendLayout();
            this.SuspendLayout();
            // 
            // TXT_DataSource
            // 
            this.TXT_DataSource.Location = new System.Drawing.Point(305, 40);
            this.TXT_DataSource.Name = "TXT_DataSource";
            this.TXT_DataSource.Size = new System.Drawing.Size(179, 20);
            this.TXT_DataSource.TabIndex = 0;
            this.TXT_DataSource.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextField_OnKeyPress);
            // 
            // LBL_DataSource
            // 
            this.LBL_DataSource.AutoSize = true;
            this.LBL_DataSource.BackColor = System.Drawing.Color.Transparent;
            this.LBL_DataSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBL_DataSource.ForeColor = System.Drawing.Color.White;
            this.LBL_DataSource.Location = new System.Drawing.Point(12, 39);
            this.LBL_DataSource.Name = "LBL_DataSource";
            this.LBL_DataSource.Size = new System.Drawing.Size(64, 18);
            this.LBL_DataSource.TabIndex = 1;
            this.LBL_DataSource.Text = "Máy chủ";
            // 
            // BTN_Login
            // 
            this.BTN_Login.BackColor = System.Drawing.Color.WhiteSmoke;
            this.BTN_Login.Location = new System.Drawing.Point(389, 94);
            this.BTN_Login.Name = "BTN_Login";
            this.BTN_Login.Size = new System.Drawing.Size(95, 24);
            this.BTN_Login.TabIndex = 6;
            this.BTN_Login.Text = "Đăng nhập";
            this.BTN_Login.UseVisualStyleBackColor = false;
            this.BTN_Login.Click += new System.EventHandler(this.BTN_Login_Click);
            // 
            // LBL_Catalog
            // 
            this.LBL_Catalog.AutoSize = true;
            this.LBL_Catalog.BackColor = System.Drawing.Color.Transparent;
            this.LBL_Catalog.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBL_Catalog.ForeColor = System.Drawing.Color.White;
            this.LBL_Catalog.Location = new System.Drawing.Point(12, 66);
            this.LBL_Catalog.Name = "LBL_Catalog";
            this.LBL_Catalog.Size = new System.Drawing.Size(95, 18);
            this.LBL_Catalog.TabIndex = 9;
            this.LBL_Catalog.Text = "Cơ sở dữ liệu";
            // 
            // TXT_Catalog
            // 
            this.TXT_Catalog.Location = new System.Drawing.Point(305, 67);
            this.TXT_Catalog.Name = "TXT_Catalog";
            this.TXT_Catalog.Size = new System.Drawing.Size(179, 20);
            this.TXT_Catalog.TabIndex = 1;
            this.TXT_Catalog.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextField_OnKeyPress);
            // 
            // MS_MenuStripId
            // 
            this.MS_MenuStripId.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSP_MenuOption});
            this.MS_MenuStripId.Location = new System.Drawing.Point(0, 0);
            this.MS_MenuStripId.Name = "MS_MenuStripId";
            this.MS_MenuStripId.Size = new System.Drawing.Size(490, 24);
            this.MS_MenuStripId.TabIndex = 11;
            // 
            // TSP_MenuOption
            // 
            this.TSP_MenuOption.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSP_ProgramExit});
            this.TSP_MenuOption.Name = "TSP_MenuOption";
            this.TSP_MenuOption.Size = new System.Drawing.Size(69, 20);
            this.TSP_MenuOption.Text = "Tùy chọn";
            // 
            // TSP_ProgramExit
            // 
            this.TSP_ProgramExit.Name = "TSP_ProgramExit";
            this.TSP_ProgramExit.Size = new System.Drawing.Size(152, 22);
            this.TSP_ProgramExit.Text = "Thoát";
            this.TSP_ProgramExit.Click += new System.EventHandler(this.TSP_ProgramExit_Click);
            // 
            // Form_LoginMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 130);
            this.Controls.Add(this.LBL_Catalog);
            this.Controls.Add(this.BTN_Login);
            this.Controls.Add(this.LBL_DataSource);
            this.Controls.Add(this.TXT_DataSource);
            this.Controls.Add(this.TXT_Catalog);
            this.Controls.Add(this.MS_MenuStripId);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MS_MenuStripId;
            this.MaximizeBox = false;
            this.Name = "Form_LoginMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login Menu";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Event_OnFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Event_OnFormClosed);
            this.MS_MenuStripId.ResumeLayout(false);
            this.MS_MenuStripId.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TXT_DataSource;
        private System.Windows.Forms.Label LBL_DataSource;
        private System.Windows.Forms.Button BTN_Login;
        private System.Windows.Forms.Label LBL_Catalog;
        private System.Windows.Forms.TextBox TXT_Catalog;
        private System.Windows.Forms.MenuStrip MS_MenuStripId;
        private System.Windows.Forms.ToolStripMenuItem TSP_MenuOption;
        private System.Windows.Forms.ToolStripMenuItem TSP_ProgramExit;
    }
}