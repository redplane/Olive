namespace Sale_Management_System
{
    partial class Form_BillManagement
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_BillManagement));
            this.DGV_BillInfo = new System.Windows.Forms.DataGridView();
            this.TXT_BillCode = new System.Windows.Forms.TextBox();
            this.LBL_BillCode = new System.Windows.Forms.Label();
            this.LBL_CustomerName = new System.Windows.Forms.Label();
            this.TXT_CustomerName = new System.Windows.Forms.TextBox();
            this.LBL_CustomerCode = new System.Windows.Forms.Label();
            this.TXT_CustomerCode = new System.Windows.Forms.TextBox();
            this.DTP_FromDate = new System.Windows.Forms.DateTimePicker();
            this.LBL_FromDate = new System.Windows.Forms.Label();
            this.LBL_EndDate = new System.Windows.Forms.Label();
            this.DTP_EndDate = new System.Windows.Forms.DateTimePicker();
            this.LBL_ProductList = new System.Windows.Forms.Label();
            this.CHX_FilterByDate = new System.Windows.Forms.CheckBox();
            this.DGV_ProductList = new System.Windows.Forms.DataGridView();
            this.PBX_BillAdd = new System.Windows.Forms.PictureBox();
            this.PBX_BillPrinting = new System.Windows.Forms.PictureBox();
            this.PBX_Back = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_BillInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_ProductList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBX_BillAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBX_BillPrinting)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBX_Back)).BeginInit();
            this.SuspendLayout();
            // 
            // DGV_BillInfo
            // 
            this.DGV_BillInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DGV_BillInfo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DGV_BillInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_BillInfo.Location = new System.Drawing.Point(296, 12);
            this.DGV_BillInfo.MultiSelect = false;
            this.DGV_BillInfo.Name = "DGV_BillInfo";
            this.DGV_BillInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGV_BillInfo.Size = new System.Drawing.Size(544, 267);
            this.DGV_BillInfo.TabIndex = 2;
            this.DGV_BillInfo.SelectionChanged += new System.EventHandler(this.DGV_BillInfo_SelectionChanged);
            // 
            // TXT_BillCode
            // 
            this.TXT_BillCode.Location = new System.Drawing.Point(15, 25);
            this.TXT_BillCode.Name = "TXT_BillCode";
            this.TXT_BillCode.Size = new System.Drawing.Size(235, 20);
            this.TXT_BillCode.TabIndex = 8;
            this.TXT_BillCode.TextChanged += new System.EventHandler(this.TextBox_OnTextChanged);
            // 
            // LBL_BillCode
            // 
            this.LBL_BillCode.AutoSize = true;
            this.LBL_BillCode.BackColor = System.Drawing.Color.Transparent;
            this.LBL_BillCode.ForeColor = System.Drawing.Color.White;
            this.LBL_BillCode.Location = new System.Drawing.Point(12, 9);
            this.LBL_BillCode.Name = "LBL_BillCode";
            this.LBL_BillCode.Size = new System.Drawing.Size(104, 13);
            this.LBL_BillCode.TabIndex = 9;
            this.LBL_BillCode.Text = "LABEL_BILL_CODE";
            // 
            // LBL_CustomerName
            // 
            this.LBL_CustomerName.AutoSize = true;
            this.LBL_CustomerName.BackColor = System.Drawing.Color.Transparent;
            this.LBL_CustomerName.ForeColor = System.Drawing.Color.White;
            this.LBL_CustomerName.Location = new System.Drawing.Point(12, 110);
            this.LBL_CustomerName.Name = "LBL_CustomerName";
            this.LBL_CustomerName.Size = new System.Drawing.Size(144, 13);
            this.LBL_CustomerName.TabIndex = 10;
            this.LBL_CustomerName.Text = "LABEL_CUSTOMER_NAME";
            // 
            // TXT_CustomerName
            // 
            this.TXT_CustomerName.Location = new System.Drawing.Point(15, 126);
            this.TXT_CustomerName.Name = "TXT_CustomerName";
            this.TXT_CustomerName.Size = new System.Drawing.Size(235, 20);
            this.TXT_CustomerName.TabIndex = 11;
            this.TXT_CustomerName.TextChanged += new System.EventHandler(this.TextBox_OnTextChanged);
            // 
            // LBL_CustomerCode
            // 
            this.LBL_CustomerCode.AutoSize = true;
            this.LBL_CustomerCode.BackColor = System.Drawing.Color.Transparent;
            this.LBL_CustomerCode.ForeColor = System.Drawing.Color.White;
            this.LBL_CustomerCode.Location = new System.Drawing.Point(12, 61);
            this.LBL_CustomerCode.Name = "LBL_CustomerCode";
            this.LBL_CustomerCode.Size = new System.Drawing.Size(143, 13);
            this.LBL_CustomerCode.TabIndex = 12;
            this.LBL_CustomerCode.Text = "LABEL_CUSTOMER_CODE";
            // 
            // TXT_CustomerCode
            // 
            this.TXT_CustomerCode.Location = new System.Drawing.Point(15, 77);
            this.TXT_CustomerCode.Name = "TXT_CustomerCode";
            this.TXT_CustomerCode.Size = new System.Drawing.Size(235, 20);
            this.TXT_CustomerCode.TabIndex = 13;
            this.TXT_CustomerCode.TextChanged += new System.EventHandler(this.TextBox_OnTextChanged);
            // 
            // DTP_FromDate
            // 
            this.DTP_FromDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.DTP_FromDate.Enabled = false;
            this.DTP_FromDate.Location = new System.Drawing.Point(50, 189);
            this.DTP_FromDate.Name = "DTP_FromDate";
            this.DTP_FromDate.Size = new System.Drawing.Size(200, 20);
            this.DTP_FromDate.TabIndex = 14;
            this.DTP_FromDate.ValueChanged += new System.EventHandler(this.DTP_FromDate_ValueChanged);
            // 
            // LBL_FromDate
            // 
            this.LBL_FromDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.LBL_FromDate.AutoSize = true;
            this.LBL_FromDate.BackColor = System.Drawing.Color.Transparent;
            this.LBL_FromDate.ForeColor = System.Drawing.Color.White;
            this.LBL_FromDate.Location = new System.Drawing.Point(57, 173);
            this.LBL_FromDate.Name = "LBL_FromDate";
            this.LBL_FromDate.Size = new System.Drawing.Size(98, 13);
            this.LBL_FromDate.TabIndex = 15;
            this.LBL_FromDate.Text = "LBL_FROM_DATE";
            // 
            // LBL_EndDate
            // 
            this.LBL_EndDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.LBL_EndDate.AutoSize = true;
            this.LBL_EndDate.BackColor = System.Drawing.Color.Transparent;
            this.LBL_EndDate.ForeColor = System.Drawing.Color.White;
            this.LBL_EndDate.Location = new System.Drawing.Point(57, 223);
            this.LBL_EndDate.Name = "LBL_EndDate";
            this.LBL_EndDate.Size = new System.Drawing.Size(90, 13);
            this.LBL_EndDate.TabIndex = 16;
            this.LBL_EndDate.Text = "LBL_END_DATE";
            // 
            // DTP_EndDate
            // 
            this.DTP_EndDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.DTP_EndDate.Enabled = false;
            this.DTP_EndDate.Location = new System.Drawing.Point(50, 239);
            this.DTP_EndDate.Name = "DTP_EndDate";
            this.DTP_EndDate.Size = new System.Drawing.Size(200, 20);
            this.DTP_EndDate.TabIndex = 17;
            this.DTP_EndDate.ValueChanged += new System.EventHandler(this.DTP_EndDate_ValueChanged);
            // 
            // LBL_ProductList
            // 
            this.LBL_ProductList.AutoSize = true;
            this.LBL_ProductList.BackColor = System.Drawing.Color.Transparent;
            this.LBL_ProductList.ForeColor = System.Drawing.Color.White;
            this.LBL_ProductList.Location = new System.Drawing.Point(12, 266);
            this.LBL_ProductList.Name = "LBL_ProductList";
            this.LBL_ProductList.Size = new System.Drawing.Size(114, 13);
            this.LBL_ProductList.TabIndex = 19;
            this.LBL_ProductList.Text = "LBL_PRODUCT_LIST";
            // 
            // CHX_FilterByDate
            // 
            this.CHX_FilterByDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CHX_FilterByDate.AutoSize = true;
            this.CHX_FilterByDate.BackColor = System.Drawing.Color.Transparent;
            this.CHX_FilterByDate.ForeColor = System.Drawing.Color.White;
            this.CHX_FilterByDate.Location = new System.Drawing.Point(15, 153);
            this.CHX_FilterByDate.Name = "CHX_FilterByDate";
            this.CHX_FilterByDate.Size = new System.Drawing.Size(135, 17);
            this.CHX_FilterByDate.TabIndex = 20;
            this.CHX_FilterByDate.Text = "TITLE_DATE_RANGE";
            this.CHX_FilterByDate.UseVisualStyleBackColor = false;
            this.CHX_FilterByDate.CheckedChanged += new System.EventHandler(this.CHX_FilterByDate_CheckStateChanged);
            // 
            // DGV_ProductList
            // 
            this.DGV_ProductList.AllowUserToAddRows = false;
            this.DGV_ProductList.AllowUserToDeleteRows = false;
            this.DGV_ProductList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.DGV_ProductList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_ProductList.Location = new System.Drawing.Point(15, 282);
            this.DGV_ProductList.Name = "DGV_ProductList";
            this.DGV_ProductList.Size = new System.Drawing.Size(250, 49);
            this.DGV_ProductList.TabIndex = 21;
            // 
            // PBX_BillAdd
            // 
            this.PBX_BillAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PBX_BillAdd.BackColor = System.Drawing.SystemColors.ControlDark;
            this.PBX_BillAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PBX_BillAdd.ErrorImage = null;
            this.PBX_BillAdd.InitialImage = null;
            this.PBX_BillAdd.Location = new System.Drawing.Point(296, 294);
            this.PBX_BillAdd.Name = "PBX_BillAdd";
            this.PBX_BillAdd.Size = new System.Drawing.Size(64, 64);
            this.PBX_BillAdd.TabIndex = 22;
            this.PBX_BillAdd.TabStop = false;
            this.PBX_BillAdd.Click += new System.EventHandler(this.BTN_AddBill_Click);
            // 
            // PBX_BillPrinting
            // 
            this.PBX_BillPrinting.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.PBX_BillPrinting.BackColor = System.Drawing.SystemColors.ControlDark;
            this.PBX_BillPrinting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PBX_BillPrinting.ErrorImage = null;
            this.PBX_BillPrinting.InitialImage = null;
            this.PBX_BillPrinting.Location = new System.Drawing.Point(548, 294);
            this.PBX_BillPrinting.Name = "PBX_BillPrinting";
            this.PBX_BillPrinting.Size = new System.Drawing.Size(64, 64);
            this.PBX_BillPrinting.TabIndex = 23;
            this.PBX_BillPrinting.TabStop = false;
            this.PBX_BillPrinting.Click += new System.EventHandler(this.BTN_PrintBill_Click);
            // 
            // PBX_Back
            // 
            this.PBX_Back.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.PBX_Back.BackColor = System.Drawing.SystemColors.ControlDark;
            this.PBX_Back.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PBX_Back.ErrorImage = null;
            this.PBX_Back.InitialImage = null;
            this.PBX_Back.Location = new System.Drawing.Point(776, 294);
            this.PBX_Back.Name = "PBX_Back";
            this.PBX_Back.Size = new System.Drawing.Size(64, 64);
            this.PBX_Back.TabIndex = 24;
            this.PBX_Back.TabStop = false;
            this.PBX_Back.Click += new System.EventHandler(this.BTN_Back_Click);
            // 
            // Form_BillManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(852, 370);
            this.Controls.Add(this.PBX_Back);
            this.Controls.Add(this.PBX_BillPrinting);
            this.Controls.Add(this.PBX_BillAdd);
            this.Controls.Add(this.DGV_ProductList);
            this.Controls.Add(this.CHX_FilterByDate);
            this.Controls.Add(this.LBL_ProductList);
            this.Controls.Add(this.DTP_EndDate);
            this.Controls.Add(this.LBL_EndDate);
            this.Controls.Add(this.LBL_FromDate);
            this.Controls.Add(this.DTP_FromDate);
            this.Controls.Add(this.TXT_CustomerCode);
            this.Controls.Add(this.LBL_CustomerCode);
            this.Controls.Add(this.TXT_CustomerName);
            this.Controls.Add(this.LBL_CustomerName);
            this.Controls.Add(this.LBL_BillCode);
            this.Controls.Add(this.TXT_BillCode);
            this.Controls.Add(this.DGV_BillInfo);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_BillManagement";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TITLE_BILL_MANAGEMENT";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Event_OnFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Event_OnFormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_BillInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_ProductList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBX_BillAdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBX_BillPrinting)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PBX_Back)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DGV_BillInfo;
        private System.Windows.Forms.TextBox TXT_BillCode;
        private System.Windows.Forms.Label LBL_BillCode;
        private System.Windows.Forms.Label LBL_CustomerName;
        private System.Windows.Forms.TextBox TXT_CustomerName;
        private System.Windows.Forms.Label LBL_CustomerCode;
        private System.Windows.Forms.TextBox TXT_CustomerCode;
        private System.Windows.Forms.DateTimePicker DTP_FromDate;
        private System.Windows.Forms.Label LBL_FromDate;
        private System.Windows.Forms.Label LBL_EndDate;
        private System.Windows.Forms.DateTimePicker DTP_EndDate;
        private System.Windows.Forms.Label LBL_ProductList;
        private System.Windows.Forms.CheckBox CHX_FilterByDate;
        private System.Windows.Forms.DataGridView DGV_ProductList;
        private System.Windows.Forms.PictureBox PBX_BillAdd;
        private System.Windows.Forms.PictureBox PBX_BillPrinting;
        private System.Windows.Forms.PictureBox PBX_Back;
    }
}