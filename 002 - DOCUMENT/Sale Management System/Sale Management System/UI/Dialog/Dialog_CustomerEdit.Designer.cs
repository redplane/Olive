namespace Sale_Management_System
{
    partial class SForm_CustomerEdit
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
            this.LBL_CustomerCode = new System.Windows.Forms.Label();
            this.TXT_CustomerCode = new System.Windows.Forms.TextBox();
            this.LBL_CustomerName = new System.Windows.Forms.Label();
            this.TXT_CustomerName = new System.Windows.Forms.TextBox();
            this.LBL_CustomerAge = new System.Windows.Forms.Label();
            this.NUD_CustomerAge = new System.Windows.Forms.NumericUpDown();
            this.LBL_CustomerBillQuantity = new System.Windows.Forms.Label();
            this.NUD_CustomerBillQuantity = new System.Windows.Forms.NumericUpDown();
            this.LBL_CustomerBalance = new System.Windows.Forms.Label();
            this.TXT_CustomerBalance = new System.Windows.Forms.TextBox();
            this.LBL_CustomerAddress = new System.Windows.Forms.Label();
            this.TXT_CustomerAddress = new System.Windows.Forms.TextBox();
            this.LBL_CustomerNote = new System.Windows.Forms.Label();
            this.RTB_CustomerNote = new System.Windows.Forms.RichTextBox();
            this.LBL_CustomerEmail = new System.Windows.Forms.Label();
            this.TXT_CustomerEmail = new System.Windows.Forms.TextBox();
            this.BTN_Clear = new System.Windows.Forms.Button();
            this.BTN_Edit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_CustomerAge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_CustomerBillQuantity)).BeginInit();
            this.SuspendLayout();
            // 
            // LBL_CustomerCode
            // 
            this.LBL_CustomerCode.AutoSize = true;
            this.LBL_CustomerCode.BackColor = System.Drawing.Color.Transparent;
            this.LBL_CustomerCode.ForeColor = System.Drawing.Color.Black;
            this.LBL_CustomerCode.Location = new System.Drawing.Point(12, 15);
            this.LBL_CustomerCode.Name = "LBL_CustomerCode";
            this.LBL_CustomerCode.Size = new System.Drawing.Size(82, 13);
            this.LBL_CustomerCode.TabIndex = 0;
            this.LBL_CustomerCode.Text = "Mã khách hàng";
            // 
            // TXT_CustomerCode
            // 
            this.TXT_CustomerCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.TXT_CustomerCode.Enabled = false;
            this.TXT_CustomerCode.Location = new System.Drawing.Point(12, 31);
            this.TXT_CustomerCode.MaxLength = 32;
            this.TXT_CustomerCode.Name = "TXT_CustomerCode";
            this.TXT_CustomerCode.Size = new System.Drawing.Size(207, 20);
            this.TXT_CustomerCode.TabIndex = 1;
            this.TXT_CustomerCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TXT_CustomerCode_KeyPress);
            // 
            // LBL_CustomerName
            // 
            this.LBL_CustomerName.AutoSize = true;
            this.LBL_CustomerName.BackColor = System.Drawing.Color.Transparent;
            this.LBL_CustomerName.ForeColor = System.Drawing.Color.Black;
            this.LBL_CustomerName.Location = new System.Drawing.Point(12, 63);
            this.LBL_CustomerName.Name = "LBL_CustomerName";
            this.LBL_CustomerName.Size = new System.Drawing.Size(86, 13);
            this.LBL_CustomerName.TabIndex = 2;
            this.LBL_CustomerName.Text = "Tên khách hàng";
            // 
            // TXT_CustomerName
            // 
            this.TXT_CustomerName.Location = new System.Drawing.Point(12, 77);
            this.TXT_CustomerName.MaxLength = 64;
            this.TXT_CustomerName.Name = "TXT_CustomerName";
            this.TXT_CustomerName.Size = new System.Drawing.Size(207, 20);
            this.TXT_CustomerName.TabIndex = 3;
            // 
            // LBL_CustomerAge
            // 
            this.LBL_CustomerAge.AutoSize = true;
            this.LBL_CustomerAge.BackColor = System.Drawing.Color.Transparent;
            this.LBL_CustomerAge.ForeColor = System.Drawing.Color.Black;
            this.LBL_CustomerAge.Location = new System.Drawing.Point(12, 107);
            this.LBL_CustomerAge.Name = "LBL_CustomerAge";
            this.LBL_CustomerAge.Size = new System.Drawing.Size(28, 13);
            this.LBL_CustomerAge.TabIndex = 4;
            this.LBL_CustomerAge.Text = "Tuổi";
            // 
            // NUD_CustomerAge
            // 
            this.NUD_CustomerAge.Location = new System.Drawing.Point(12, 123);
            this.NUD_CustomerAge.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NUD_CustomerAge.Name = "NUD_CustomerAge";
            this.NUD_CustomerAge.Size = new System.Drawing.Size(120, 20);
            this.NUD_CustomerAge.TabIndex = 5;
            // 
            // LBL_CustomerBillQuantity
            // 
            this.LBL_CustomerBillQuantity.AutoSize = true;
            this.LBL_CustomerBillQuantity.BackColor = System.Drawing.Color.Transparent;
            this.LBL_CustomerBillQuantity.ForeColor = System.Drawing.Color.Black;
            this.LBL_CustomerBillQuantity.Location = new System.Drawing.Point(12, 151);
            this.LBL_CustomerBillQuantity.Name = "LBL_CustomerBillQuantity";
            this.LBL_CustomerBillQuantity.Size = new System.Drawing.Size(154, 13);
            this.LBL_CustomerBillQuantity.TabIndex = 6;
            this.LBL_CustomerBillQuantity.Text = "Số lượng hóa đơn đã giao dịch";
            // 
            // NUD_CustomerBillQuantity
            // 
            this.NUD_CustomerBillQuantity.Location = new System.Drawing.Point(12, 167);
            this.NUD_CustomerBillQuantity.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NUD_CustomerBillQuantity.Name = "NUD_CustomerBillQuantity";
            this.NUD_CustomerBillQuantity.Size = new System.Drawing.Size(120, 20);
            this.NUD_CustomerBillQuantity.TabIndex = 7;
            // 
            // LBL_CustomerBalance
            // 
            this.LBL_CustomerBalance.AutoSize = true;
            this.LBL_CustomerBalance.BackColor = System.Drawing.Color.Transparent;
            this.LBL_CustomerBalance.ForeColor = System.Drawing.Color.Black;
            this.LBL_CustomerBalance.Location = new System.Drawing.Point(12, 199);
            this.LBL_CustomerBalance.Name = "LBL_CustomerBalance";
            this.LBL_CustomerBalance.Size = new System.Drawing.Size(36, 13);
            this.LBL_CustomerBalance.TabIndex = 8;
            this.LBL_CustomerBalance.Text = "Dư nợ";
            // 
            // TXT_CustomerBalance
            // 
            this.TXT_CustomerBalance.Location = new System.Drawing.Point(12, 215);
            this.TXT_CustomerBalance.Name = "TXT_CustomerBalance";
            this.TXT_CustomerBalance.Size = new System.Drawing.Size(207, 20);
            this.TXT_CustomerBalance.TabIndex = 9;
            this.TXT_CustomerBalance.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TXT_CustomerBalance_KeyPress);
            // 
            // LBL_CustomerAddress
            // 
            this.LBL_CustomerAddress.AutoSize = true;
            this.LBL_CustomerAddress.BackColor = System.Drawing.Color.Transparent;
            this.LBL_CustomerAddress.ForeColor = System.Drawing.Color.Black;
            this.LBL_CustomerAddress.Location = new System.Drawing.Point(313, 14);
            this.LBL_CustomerAddress.Name = "LBL_CustomerAddress";
            this.LBL_CustomerAddress.Size = new System.Drawing.Size(40, 13);
            this.LBL_CustomerAddress.TabIndex = 10;
            this.LBL_CustomerAddress.Text = "Địa chỉ";
            // 
            // TXT_CustomerAddress
            // 
            this.TXT_CustomerAddress.Location = new System.Drawing.Point(317, 30);
            this.TXT_CustomerAddress.MaxLength = 128;
            this.TXT_CustomerAddress.Name = "TXT_CustomerAddress";
            this.TXT_CustomerAddress.Size = new System.Drawing.Size(207, 20);
            this.TXT_CustomerAddress.TabIndex = 11;
            // 
            // LBL_CustomerNote
            // 
            this.LBL_CustomerNote.AutoSize = true;
            this.LBL_CustomerNote.BackColor = System.Drawing.Color.Transparent;
            this.LBL_CustomerNote.ForeColor = System.Drawing.Color.Black;
            this.LBL_CustomerNote.Location = new System.Drawing.Point(313, 107);
            this.LBL_CustomerNote.Name = "LBL_CustomerNote";
            this.LBL_CustomerNote.Size = new System.Drawing.Size(44, 13);
            this.LBL_CustomerNote.TabIndex = 12;
            this.LBL_CustomerNote.Text = "Ghi chú";
            // 
            // RTB_CustomerNote
            // 
            this.RTB_CustomerNote.Location = new System.Drawing.Point(316, 123);
            this.RTB_CustomerNote.MaxLength = 256;
            this.RTB_CustomerNote.Name = "RTB_CustomerNote";
            this.RTB_CustomerNote.Size = new System.Drawing.Size(208, 106);
            this.RTB_CustomerNote.TabIndex = 13;
            this.RTB_CustomerNote.Text = "";
            // 
            // LBL_CustomerEmail
            // 
            this.LBL_CustomerEmail.AutoSize = true;
            this.LBL_CustomerEmail.BackColor = System.Drawing.Color.Transparent;
            this.LBL_CustomerEmail.ForeColor = System.Drawing.Color.Black;
            this.LBL_CustomerEmail.Location = new System.Drawing.Point(314, 63);
            this.LBL_CustomerEmail.Name = "LBL_CustomerEmail";
            this.LBL_CustomerEmail.Size = new System.Drawing.Size(32, 13);
            this.LBL_CustomerEmail.TabIndex = 16;
            this.LBL_CustomerEmail.Text = "Email";
            // 
            // TXT_CustomerEmail
            // 
            this.TXT_CustomerEmail.Location = new System.Drawing.Point(317, 79);
            this.TXT_CustomerEmail.Name = "TXT_CustomerEmail";
            this.TXT_CustomerEmail.Size = new System.Drawing.Size(207, 20);
            this.TXT_CustomerEmail.TabIndex = 17;
            // 
            // BTN_Clear
            // 
            this.BTN_Clear.Location = new System.Drawing.Point(316, 276);
            this.BTN_Clear.Name = "BTN_Clear";
            this.BTN_Clear.Size = new System.Drawing.Size(75, 23);
            this.BTN_Clear.TabIndex = 18;
            this.BTN_Clear.Text = "Xóa";
            this.BTN_Clear.UseVisualStyleBackColor = true;
            this.BTN_Clear.Click += new System.EventHandler(this.BTN_CustomerInfoClear_Click);
            // 
            // BTN_Edit
            // 
            this.BTN_Edit.Location = new System.Drawing.Point(449, 276);
            this.BTN_Edit.Name = "BTN_Edit";
            this.BTN_Edit.Size = new System.Drawing.Size(75, 23);
            this.BTN_Edit.TabIndex = 19;
            this.BTN_Edit.Text = "Sửa";
            this.BTN_Edit.UseVisualStyleBackColor = true;
            this.BTN_Edit.Click += new System.EventHandler(this.BTN_CustomerInfoSave_Click);
            // 
            // SForm_CustomerEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 311);
            this.Controls.Add(this.BTN_Edit);
            this.Controls.Add(this.BTN_Clear);
            this.Controls.Add(this.TXT_CustomerEmail);
            this.Controls.Add(this.LBL_CustomerEmail);
            this.Controls.Add(this.RTB_CustomerNote);
            this.Controls.Add(this.LBL_CustomerNote);
            this.Controls.Add(this.TXT_CustomerAddress);
            this.Controls.Add(this.LBL_CustomerAddress);
            this.Controls.Add(this.TXT_CustomerBalance);
            this.Controls.Add(this.LBL_CustomerBalance);
            this.Controls.Add(this.NUD_CustomerBillQuantity);
            this.Controls.Add(this.LBL_CustomerBillQuantity);
            this.Controls.Add(this.NUD_CustomerAge);
            this.Controls.Add(this.LBL_CustomerAge);
            this.Controls.Add(this.TXT_CustomerName);
            this.Controls.Add(this.LBL_CustomerName);
            this.Controls.Add(this.TXT_CustomerCode);
            this.Controls.Add(this.LBL_CustomerCode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SForm_CustomerEdit";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sửa khách hàng";
            ((System.ComponentModel.ISupportInitialize)(this.NUD_CustomerAge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_CustomerBillQuantity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LBL_CustomerCode;
        private System.Windows.Forms.TextBox TXT_CustomerCode;
        private System.Windows.Forms.Label LBL_CustomerName;
        private System.Windows.Forms.TextBox TXT_CustomerName;
        private System.Windows.Forms.Label LBL_CustomerAge;
        private System.Windows.Forms.NumericUpDown NUD_CustomerAge;
        private System.Windows.Forms.Label LBL_CustomerBillQuantity;
        private System.Windows.Forms.NumericUpDown NUD_CustomerBillQuantity;
        private System.Windows.Forms.Label LBL_CustomerBalance;
        private System.Windows.Forms.TextBox TXT_CustomerBalance;
        private System.Windows.Forms.Label LBL_CustomerAddress;
        private System.Windows.Forms.TextBox TXT_CustomerAddress;
        private System.Windows.Forms.Label LBL_CustomerNote;
        private System.Windows.Forms.RichTextBox RTB_CustomerNote;
        private System.Windows.Forms.Label LBL_CustomerEmail;
        private System.Windows.Forms.TextBox TXT_CustomerEmail;
        private System.Windows.Forms.Button BTN_Clear;
        private System.Windows.Forms.Button BTN_Edit;
    }
}