namespace Sale_Management_System
{
    partial class Dialog_ProductEdit
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
            this.LBL_ProductNote = new System.Windows.Forms.Label();
            this.RTB_ProductNote = new System.Windows.Forms.RichTextBox();
            this.TXT_ProductPrice = new System.Windows.Forms.TextBox();
            this.LBL_ProductPrice = new System.Windows.Forms.Label();
            this.TXT_ProductQuantity = new System.Windows.Forms.TextBox();
            this.LBL_ProductQuantity = new System.Windows.Forms.Label();
            this.TXT_ProductName = new System.Windows.Forms.TextBox();
            this.LBL_ProductName = new System.Windows.Forms.Label();
            this.TXT_ProductCode = new System.Windows.Forms.TextBox();
            this.LBL_ProductCode = new System.Windows.Forms.Label();
            this.LBL_ProductUnit = new System.Windows.Forms.Label();
            this.TXT_ProductUnit = new System.Windows.Forms.TextBox();
            this.BTN_Edit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LBL_ProductNote
            // 
            this.LBL_ProductNote.AutoSize = true;
            this.LBL_ProductNote.BackColor = System.Drawing.Color.Transparent;
            this.LBL_ProductNote.ForeColor = System.Drawing.Color.Black;
            this.LBL_ProductNote.Location = new System.Drawing.Point(280, 62);
            this.LBL_ProductNote.Name = "LBL_ProductNote";
            this.LBL_ProductNote.Size = new System.Drawing.Size(44, 13);
            this.LBL_ProductNote.TabIndex = 21;
            this.LBL_ProductNote.Text = "Ghi chú";
            // 
            // RTB_ProductNote
            // 
            this.RTB_ProductNote.Location = new System.Drawing.Point(285, 78);
            this.RTB_ProductNote.MaxLength = 255;
            this.RTB_ProductNote.Name = "RTB_ProductNote";
            this.RTB_ProductNote.Size = new System.Drawing.Size(229, 221);
            this.RTB_ProductNote.TabIndex = 17;
            this.RTB_ProductNote.Text = "";
            // 
            // TXT_ProductPrice
            // 
            this.TXT_ProductPrice.Location = new System.Drawing.Point(12, 186);
            this.TXT_ProductPrice.MaxLength = 32;
            this.TXT_ProductPrice.Name = "TXT_ProductPrice";
            this.TXT_ProductPrice.Size = new System.Drawing.Size(229, 20);
            this.TXT_ProductPrice.TabIndex = 16;
            this.TXT_ProductPrice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TXT_ProductPrice_KeyPress);
            // 
            // LBL_ProductPrice
            // 
            this.LBL_ProductPrice.AutoSize = true;
            this.LBL_ProductPrice.BackColor = System.Drawing.Color.Transparent;
            this.LBL_ProductPrice.ForeColor = System.Drawing.Color.Black;
            this.LBL_ProductPrice.Location = new System.Drawing.Point(9, 170);
            this.LBL_ProductPrice.Name = "LBL_ProductPrice";
            this.LBL_ProductPrice.Size = new System.Drawing.Size(23, 13);
            this.LBL_ProductPrice.TabIndex = 19;
            this.LBL_ProductPrice.Text = "Giá";
            // 
            // TXT_ProductQuantity
            // 
            this.TXT_ProductQuantity.Location = new System.Drawing.Point(12, 132);
            this.TXT_ProductQuantity.MaxLength = 32;
            this.TXT_ProductQuantity.Name = "TXT_ProductQuantity";
            this.TXT_ProductQuantity.Size = new System.Drawing.Size(229, 20);
            this.TXT_ProductQuantity.TabIndex = 14;
            this.TXT_ProductQuantity.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TXT_ProductQuantity_KeyPress);
            // 
            // LBL_ProductQuantity
            // 
            this.LBL_ProductQuantity.AutoSize = true;
            this.LBL_ProductQuantity.BackColor = System.Drawing.Color.Transparent;
            this.LBL_ProductQuantity.ForeColor = System.Drawing.Color.Black;
            this.LBL_ProductQuantity.Location = new System.Drawing.Point(9, 116);
            this.LBL_ProductQuantity.Name = "LBL_ProductQuantity";
            this.LBL_ProductQuantity.Size = new System.Drawing.Size(49, 13);
            this.LBL_ProductQuantity.TabIndex = 15;
            this.LBL_ProductQuantity.Text = "Số lượng";
            // 
            // TXT_ProductName
            // 
            this.TXT_ProductName.Location = new System.Drawing.Point(12, 78);
            this.TXT_ProductName.MaxLength = 64;
            this.TXT_ProductName.Name = "TXT_ProductName";
            this.TXT_ProductName.Size = new System.Drawing.Size(229, 20);
            this.TXT_ProductName.TabIndex = 12;
            // 
            // LBL_ProductName
            // 
            this.LBL_ProductName.AutoSize = true;
            this.LBL_ProductName.BackColor = System.Drawing.Color.Transparent;
            this.LBL_ProductName.ForeColor = System.Drawing.Color.Black;
            this.LBL_ProductName.Location = new System.Drawing.Point(9, 62);
            this.LBL_ProductName.Name = "LBL_ProductName";
            this.LBL_ProductName.Size = new System.Drawing.Size(75, 13);
            this.LBL_ProductName.TabIndex = 13;
            this.LBL_ProductName.Text = "Tên sản phẩm";
            // 
            // TXT_ProductCode
            // 
            this.TXT_ProductCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.TXT_ProductCode.Enabled = false;
            this.TXT_ProductCode.Location = new System.Drawing.Point(12, 29);
            this.TXT_ProductCode.MaxLength = 32;
            this.TXT_ProductCode.Name = "TXT_ProductCode";
            this.TXT_ProductCode.ReadOnly = true;
            this.TXT_ProductCode.Size = new System.Drawing.Size(229, 20);
            this.TXT_ProductCode.TabIndex = 11;
            // 
            // LBL_ProductCode
            // 
            this.LBL_ProductCode.AutoSize = true;
            this.LBL_ProductCode.BackColor = System.Drawing.Color.Transparent;
            this.LBL_ProductCode.ForeColor = System.Drawing.Color.Black;
            this.LBL_ProductCode.Location = new System.Drawing.Point(9, 13);
            this.LBL_ProductCode.Name = "LBL_ProductCode";
            this.LBL_ProductCode.Size = new System.Drawing.Size(71, 13);
            this.LBL_ProductCode.TabIndex = 10;
            this.LBL_ProductCode.Text = "Mã sản phẩm";
            // 
            // LBL_ProductUnit
            // 
            this.LBL_ProductUnit.AutoSize = true;
            this.LBL_ProductUnit.BackColor = System.Drawing.Color.Transparent;
            this.LBL_ProductUnit.ForeColor = System.Drawing.Color.Black;
            this.LBL_ProductUnit.Location = new System.Drawing.Point(280, 13);
            this.LBL_ProductUnit.Name = "LBL_ProductUnit";
            this.LBL_ProductUnit.Size = new System.Drawing.Size(38, 13);
            this.LBL_ProductUnit.TabIndex = 22;
            this.LBL_ProductUnit.Text = "Đơn vị";
            // 
            // TXT_ProductUnit
            // 
            this.TXT_ProductUnit.Location = new System.Drawing.Point(283, 29);
            this.TXT_ProductUnit.Name = "TXT_ProductUnit";
            this.TXT_ProductUnit.Size = new System.Drawing.Size(229, 20);
            this.TXT_ProductUnit.TabIndex = 23;
            // 
            // BTN_Edit
            // 
            this.BTN_Edit.Location = new System.Drawing.Point(12, 276);
            this.BTN_Edit.Name = "BTN_Edit";
            this.BTN_Edit.Size = new System.Drawing.Size(75, 23);
            this.BTN_Edit.TabIndex = 24;
            this.BTN_Edit.Text = "Sửa";
            this.BTN_Edit.UseVisualStyleBackColor = true;
            this.BTN_Edit.Click += new System.EventHandler(this.BTN_ProductSave_Click);
            // 
            // Dialog_ProductEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 311);
            this.Controls.Add(this.BTN_Edit);
            this.Controls.Add(this.TXT_ProductUnit);
            this.Controls.Add(this.LBL_ProductUnit);
            this.Controls.Add(this.LBL_ProductNote);
            this.Controls.Add(this.RTB_ProductNote);
            this.Controls.Add(this.TXT_ProductPrice);
            this.Controls.Add(this.LBL_ProductPrice);
            this.Controls.Add(this.TXT_ProductQuantity);
            this.Controls.Add(this.LBL_ProductQuantity);
            this.Controls.Add(this.TXT_ProductName);
            this.Controls.Add(this.LBL_ProductName);
            this.Controls.Add(this.TXT_ProductCode);
            this.Controls.Add(this.LBL_ProductCode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Dialog_ProductEdit";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sửa sản phẩm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LBL_ProductNote;
        private System.Windows.Forms.RichTextBox RTB_ProductNote;
        private System.Windows.Forms.TextBox TXT_ProductPrice;
        private System.Windows.Forms.Label LBL_ProductPrice;
        private System.Windows.Forms.TextBox TXT_ProductQuantity;
        private System.Windows.Forms.Label LBL_ProductQuantity;
        private System.Windows.Forms.TextBox TXT_ProductName;
        private System.Windows.Forms.Label LBL_ProductName;
        private System.Windows.Forms.TextBox TXT_ProductCode;
        private System.Windows.Forms.Label LBL_ProductCode;
        private System.Windows.Forms.Label LBL_ProductUnit;
        private System.Windows.Forms.TextBox TXT_ProductUnit;
        private System.Windows.Forms.Button BTN_Edit;
    }
}