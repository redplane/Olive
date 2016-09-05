namespace Sale_Management_System
{
    partial class Form_CustomerManagement
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_CustomerManagement));
            this.DGV_CustomerInfo = new System.Windows.Forms.DataGridView();
            this.TXT_CustomerName = new System.Windows.Forms.TextBox();
            this.LBL_NameFilter = new System.Windows.Forms.Label();
            this.LBL_CodeFilter = new System.Windows.Forms.Label();
            this.TXT_CustomerCode = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_CustomerInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // DGV_CustomerInfo
            // 
            this.DGV_CustomerInfo.AllowUserToDeleteRows = false;
            this.DGV_CustomerInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DGV_CustomerInfo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DGV_CustomerInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_CustomerInfo.Location = new System.Drawing.Point(296, 24);
            this.DGV_CustomerInfo.Name = "DGV_CustomerInfo";
            this.DGV_CustomerInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGV_CustomerInfo.Size = new System.Drawing.Size(544, 251);
            this.DGV_CustomerInfo.TabIndex = 16;
            this.DGV_CustomerInfo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DGV_CustomerInfo_KeyDown);
            // 
            // TXT_CustomerName
            // 
            this.TXT_CustomerName.Location = new System.Drawing.Point(12, 91);
            this.TXT_CustomerName.Name = "TXT_CustomerName";
            this.TXT_CustomerName.Size = new System.Drawing.Size(253, 20);
            this.TXT_CustomerName.TabIndex = 25;
            this.TXT_CustomerName.TextChanged += new System.EventHandler(this.TXT_TextChanged);
            // 
            // LBL_NameFilter
            // 
            this.LBL_NameFilter.AutoSize = true;
            this.LBL_NameFilter.BackColor = System.Drawing.Color.Transparent;
            this.LBL_NameFilter.Location = new System.Drawing.Point(9, 75);
            this.LBL_NameFilter.Name = "LBL_NameFilter";
            this.LBL_NameFilter.Size = new System.Drawing.Size(127, 13);
            this.LBL_NameFilter.TabIndex = 21;
            this.LBL_NameFilter.Text = "Lọc theo tên khách hàng";
            // 
            // LBL_CodeFilter
            // 
            this.LBL_CodeFilter.AutoSize = true;
            this.LBL_CodeFilter.BackColor = System.Drawing.Color.Transparent;
            this.LBL_CodeFilter.ForeColor = System.Drawing.Color.Black;
            this.LBL_CodeFilter.Location = new System.Drawing.Point(9, 24);
            this.LBL_CodeFilter.Name = "LBL_CodeFilter";
            this.LBL_CodeFilter.Size = new System.Drawing.Size(126, 13);
            this.LBL_CodeFilter.TabIndex = 29;
            this.LBL_CodeFilter.Text = "Lọc theo mã khách hàng";
            // 
            // TXT_CustomerCode
            // 
            this.TXT_CustomerCode.Location = new System.Drawing.Point(12, 41);
            this.TXT_CustomerCode.MaxLength = 32;
            this.TXT_CustomerCode.Name = "TXT_CustomerCode";
            this.TXT_CustomerCode.Size = new System.Drawing.Size(253, 20);
            this.TXT_CustomerCode.TabIndex = 30;
            this.TXT_CustomerCode.TextChanged += new System.EventHandler(this.TXT_TextChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(12, 223);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(114, 23);
            this.button1.TabIndex = 38;
            this.button1.Text = "Thêm khách hàng";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.BTN_AddCustomer_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(151, 223);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(114, 23);
            this.button2.TabIndex = 39;
            this.button2.Text = "Sửa khách hàng";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.BTN_EditCustomer_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button3.Location = new System.Drawing.Point(11, 252);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(114, 23);
            this.button3.TabIndex = 40;
            this.button3.Text = "Xóa khách hàng";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.BTN_DeleteCustomer_Click);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button4.Location = new System.Drawing.Point(151, 252);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(114, 23);
            this.button4.TabIndex = 41;
            this.button4.Text = "Quay lại";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.BTN_Back_Click);
            // 
            // Form_CustomerManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(852, 289);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.TXT_CustomerCode);
            this.Controls.Add(this.LBL_CodeFilter);
            this.Controls.Add(this.TXT_CustomerName);
            this.Controls.Add(this.LBL_NameFilter);
            this.Controls.Add(this.DGV_CustomerInfo);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_CustomerManagement";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản lý khách hàng";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Event_OnFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Event_OnFormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_CustomerInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DGV_CustomerInfo;
        private System.Windows.Forms.TextBox TXT_CustomerName;
        private System.Windows.Forms.Label LBL_NameFilter;
        private System.Windows.Forms.Label LBL_CodeFilter;
        private System.Windows.Forms.TextBox TXT_CustomerCode;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}