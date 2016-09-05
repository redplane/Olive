namespace Sale_Management_System
{
    partial class Form_MainMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_MainMenu));
            this.MS_MenuStripId = new System.Windows.Forms.MenuStrip();
            this.TSP_MenuOption = new System.Windows.Forms.ToolStripMenuItem();
            this.TSP_MenuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.STS_StatusStripId = new System.Windows.Forms.StatusStrip();
            this.STS_StatusLabelId = new System.Windows.Forms.ToolStripStatusLabel();
            this.BTN_ProductManagement = new System.Windows.Forms.Button();
            this.BTN_CustomerManagement = new System.Windows.Forms.Button();
            this.BTN_BillManagement = new System.Windows.Forms.Button();
            this.BTN_Back = new System.Windows.Forms.Button();
            this.MS_MenuStripId.SuspendLayout();
            this.STS_StatusStripId.SuspendLayout();
            this.SuspendLayout();
            // 
            // MS_MenuStripId
            // 
            this.MS_MenuStripId.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSP_MenuOption});
            this.MS_MenuStripId.Location = new System.Drawing.Point(0, 0);
            this.MS_MenuStripId.Name = "MS_MenuStripId";
            this.MS_MenuStripId.Size = new System.Drawing.Size(350, 24);
            this.MS_MenuStripId.TabIndex = 6;
            this.MS_MenuStripId.Text = "menuStrip1";
            // 
            // TSP_MenuOption
            // 
            this.TSP_MenuOption.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSP_MenuExit});
            this.TSP_MenuOption.Name = "TSP_MenuOption";
            this.TSP_MenuOption.Size = new System.Drawing.Size(69, 20);
            this.TSP_MenuOption.Text = "Tùy chọn";
            // 
            // TSP_MenuExit
            // 
            this.TSP_MenuExit.Name = "TSP_MenuExit";
            this.TSP_MenuExit.Size = new System.Drawing.Size(105, 22);
            this.TSP_MenuExit.Text = "Thoát";
            this.TSP_MenuExit.Click += new System.EventHandler(this.TSP_MenuExit_Click);
            // 
            // STS_StatusStripId
            // 
            this.STS_StatusStripId.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.STS_StatusLabelId});
            this.STS_StatusStripId.Location = new System.Drawing.Point(0, 179);
            this.STS_StatusStripId.Name = "STS_StatusStripId";
            this.STS_StatusStripId.Size = new System.Drawing.Size(350, 22);
            this.STS_StatusStripId.TabIndex = 7;
            // 
            // STS_StatusLabelId
            // 
            this.STS_StatusLabelId.Name = "STS_StatusLabelId";
            this.STS_StatusLabelId.Size = new System.Drawing.Size(0, 17);
            // 
            // BTN_ProductManagement
            // 
            this.BTN_ProductManagement.Location = new System.Drawing.Point(115, 27);
            this.BTN_ProductManagement.Name = "BTN_ProductManagement";
            this.BTN_ProductManagement.Size = new System.Drawing.Size(120, 23);
            this.BTN_ProductManagement.TabIndex = 13;
            this.BTN_ProductManagement.Text = "Quản lý sản phẩm";
            this.BTN_ProductManagement.UseVisualStyleBackColor = true;
            this.BTN_ProductManagement.Click += new System.EventHandler(this.BTN_FormProduct_Click);
            // 
            // BTN_CustomerManagement
            // 
            this.BTN_CustomerManagement.Location = new System.Drawing.Point(115, 56);
            this.BTN_CustomerManagement.Name = "BTN_CustomerManagement";
            this.BTN_CustomerManagement.Size = new System.Drawing.Size(120, 23);
            this.BTN_CustomerManagement.TabIndex = 14;
            this.BTN_CustomerManagement.Text = "Quản lý khách hàng";
            this.BTN_CustomerManagement.UseVisualStyleBackColor = true;
            this.BTN_CustomerManagement.Click += new System.EventHandler(this.BTN_FormCustomer_Click);
            // 
            // BTN_BillManagement
            // 
            this.BTN_BillManagement.Location = new System.Drawing.Point(115, 85);
            this.BTN_BillManagement.Name = "BTN_BillManagement";
            this.BTN_BillManagement.Size = new System.Drawing.Size(120, 23);
            this.BTN_BillManagement.TabIndex = 15;
            this.BTN_BillManagement.Text = "Quản lý đơn hàng";
            this.BTN_BillManagement.UseVisualStyleBackColor = true;
            this.BTN_BillManagement.Click += new System.EventHandler(this.BTN_FormBill_Click);
            // 
            // BTN_Back
            // 
            this.BTN_Back.Location = new System.Drawing.Point(134, 143);
            this.BTN_Back.Name = "BTN_Back";
            this.BTN_Back.Size = new System.Drawing.Size(82, 23);
            this.BTN_Back.TabIndex = 17;
            this.BTN_Back.Text = "Quay lại";
            this.BTN_Back.UseVisualStyleBackColor = true;
            this.BTN_Back.Click += new System.EventHandler(this.BTN_BackClicks);
            // 
            // Form_MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(350, 201);
            this.Controls.Add(this.BTN_Back);
            this.Controls.Add(this.BTN_BillManagement);
            this.Controls.Add(this.BTN_CustomerManagement);
            this.Controls.Add(this.BTN_ProductManagement);
            this.Controls.Add(this.STS_StatusStripId);
            this.Controls.Add(this.MS_MenuStripId);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MS_MenuStripId;
            this.MaximizeBox = false;
            this.Name = "Form_MainMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sale Management System";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Event_OnFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Event_OnFormClosed);
            this.MS_MenuStripId.ResumeLayout(false);
            this.MS_MenuStripId.PerformLayout();
            this.STS_StatusStripId.ResumeLayout(false);
            this.STS_StatusStripId.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MS_MenuStripId;
        private System.Windows.Forms.ToolStripMenuItem TSP_MenuOption;
        private System.Windows.Forms.ToolStripMenuItem TSP_MenuExit;
        private System.Windows.Forms.StatusStrip STS_StatusStripId;
        private System.Windows.Forms.ToolStripStatusLabel STS_StatusLabelId;
        private System.Windows.Forms.Button BTN_ProductManagement;
        private System.Windows.Forms.Button BTN_CustomerManagement;
        private System.Windows.Forms.Button BTN_BillManagement;
        private System.Windows.Forms.Button BTN_Back;
    }
}

