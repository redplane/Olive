namespace Sale_Management_System
{
    partial class Form_ProductManagement
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_ProductManagement));
            this.DGV_ProductInfo = new System.Windows.Forms.DataGridView();
            this.TXT_ProductCode = new System.Windows.Forms.TextBox();
            this.LBL_ProductCode = new System.Windows.Forms.Label();
            this.LBL_ProductName = new System.Windows.Forms.Label();
            this.TXT_ProductName = new System.Windows.Forms.TextBox();
            this.BTN_ProductAdd = new System.Windows.Forms.Button();
            this.BTN_ProductEdit = new System.Windows.Forms.Button();
            this.BTN_DeleteProduct = new System.Windows.Forms.Button();
            this.BTN_Back = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_ProductInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // DGV_ProductInfo
            // 
            this.DGV_ProductInfo.AllowUserToAddRows = false;
            this.DGV_ProductInfo.AllowUserToDeleteRows = false;
            this.DGV_ProductInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DGV_ProductInfo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DGV_ProductInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_ProductInfo.Location = new System.Drawing.Point(289, 23);
            this.DGV_ProductInfo.MultiSelect = false;
            this.DGV_ProductInfo.Name = "DGV_ProductInfo";
            this.DGV_ProductInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGV_ProductInfo.Size = new System.Drawing.Size(544, 249);
            this.DGV_ProductInfo.TabIndex = 0;
            this.DGV_ProductInfo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DGV_ProductInfo_KeyDown);
            // 
            // TXT_ProductCode
            // 
            this.TXT_ProductCode.Location = new System.Drawing.Point(12, 41);
            this.TXT_ProductCode.Name = "TXT_ProductCode";
            this.TXT_ProductCode.Size = new System.Drawing.Size(225, 20);
            this.TXT_ProductCode.TabIndex = 5;
            this.TXT_ProductCode.TextChanged += new System.EventHandler(this.TXT_ProductCode_TextChanged);
            // 
            // LBL_ProductCode
            // 
            this.LBL_ProductCode.AutoSize = true;
            this.LBL_ProductCode.BackColor = System.Drawing.Color.Transparent;
            this.LBL_ProductCode.ForeColor = System.Drawing.Color.Black;
            this.LBL_ProductCode.Location = new System.Drawing.Point(13, 22);
            this.LBL_ProductCode.Name = "LBL_ProductCode";
            this.LBL_ProductCode.Size = new System.Drawing.Size(83, 13);
            this.LBL_ProductCode.TabIndex = 6;
            this.LBL_ProductCode.Text = "Lọc theo mã SP";
            // 
            // LBL_ProductName
            // 
            this.LBL_ProductName.AutoSize = true;
            this.LBL_ProductName.BackColor = System.Drawing.Color.Transparent;
            this.LBL_ProductName.ForeColor = System.Drawing.Color.Black;
            this.LBL_ProductName.Location = new System.Drawing.Point(13, 76);
            this.LBL_ProductName.Name = "LBL_ProductName";
            this.LBL_ProductName.Size = new System.Drawing.Size(84, 13);
            this.LBL_ProductName.TabIndex = 7;
            this.LBL_ProductName.Text = "Lọc theo tên SP";
            // 
            // TXT_ProductName
            // 
            this.TXT_ProductName.Location = new System.Drawing.Point(12, 92);
            this.TXT_ProductName.Name = "TXT_ProductName";
            this.TXT_ProductName.Size = new System.Drawing.Size(225, 20);
            this.TXT_ProductName.TabIndex = 8;
            this.TXT_ProductName.TextChanged += new System.EventHandler(this.TXT_ProductName_TextChanged);
            // 
            // BTN_ProductAdd
            // 
            this.BTN_ProductAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BTN_ProductAdd.Location = new System.Drawing.Point(12, 220);
            this.BTN_ProductAdd.Name = "BTN_ProductAdd";
            this.BTN_ProductAdd.Size = new System.Drawing.Size(109, 23);
            this.BTN_ProductAdd.TabIndex = 27;
            this.BTN_ProductAdd.Text = "Thêm sản phẩm";
            this.BTN_ProductAdd.UseVisualStyleBackColor = true;
            this.BTN_ProductAdd.Click += new System.EventHandler(this.BTN_ProductAdd_Click);
            // 
            // BTN_ProductEdit
            // 
            this.BTN_ProductEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BTN_ProductEdit.Location = new System.Drawing.Point(128, 220);
            this.BTN_ProductEdit.Name = "BTN_ProductEdit";
            this.BTN_ProductEdit.Size = new System.Drawing.Size(109, 23);
            this.BTN_ProductEdit.TabIndex = 28;
            this.BTN_ProductEdit.Text = "Sửa sản phẩm";
            this.BTN_ProductEdit.UseVisualStyleBackColor = true;
            this.BTN_ProductEdit.Click += new System.EventHandler(this.BTN_ProductEdit_Click);
            // 
            // BTN_DeleteProduct
            // 
            this.BTN_DeleteProduct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BTN_DeleteProduct.Location = new System.Drawing.Point(12, 249);
            this.BTN_DeleteProduct.Name = "BTN_DeleteProduct";
            this.BTN_DeleteProduct.Size = new System.Drawing.Size(109, 23);
            this.BTN_DeleteProduct.TabIndex = 29;
            this.BTN_DeleteProduct.Text = "Xóa sản phẩm";
            this.BTN_DeleteProduct.UseVisualStyleBackColor = true;
            this.BTN_DeleteProduct.Click += new System.EventHandler(this.BTN_ProductDelete_Click);
            // 
            // BTN_Back
            // 
            this.BTN_Back.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BTN_Back.Location = new System.Drawing.Point(128, 249);
            this.BTN_Back.Name = "BTN_Back";
            this.BTN_Back.Size = new System.Drawing.Size(109, 23);
            this.BTN_Back.TabIndex = 30;
            this.BTN_Back.Text = "Quay lại";
            this.BTN_Back.UseVisualStyleBackColor = true;
            this.BTN_Back.Click += new System.EventHandler(this.BTN_Back_Click);
            // 
            // Form_ProductManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(852, 284);
            this.Controls.Add(this.BTN_Back);
            this.Controls.Add(this.BTN_DeleteProduct);
            this.Controls.Add(this.BTN_ProductEdit);
            this.Controls.Add(this.BTN_ProductAdd);
            this.Controls.Add(this.TXT_ProductName);
            this.Controls.Add(this.LBL_ProductName);
            this.Controls.Add(this.LBL_ProductCode);
            this.Controls.Add(this.TXT_ProductCode);
            this.Controls.Add(this.DGV_ProductInfo);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_ProductManagement";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản lý sản phẩm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Event_OnFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Event_OnFormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_ProductInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DGV_ProductInfo;
        private System.Windows.Forms.TextBox TXT_ProductCode;
        private System.Windows.Forms.Label LBL_ProductCode;
        private System.Windows.Forms.Label LBL_ProductName;
        private System.Windows.Forms.TextBox TXT_ProductName;
        private System.Windows.Forms.Button BTN_ProductAdd;
        private System.Windows.Forms.Button BTN_ProductEdit;
        private System.Windows.Forms.Button BTN_DeleteProduct;
        private System.Windows.Forms.Button BTN_Back;
    }
}