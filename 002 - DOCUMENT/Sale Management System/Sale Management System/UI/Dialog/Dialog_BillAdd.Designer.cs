namespace Sale_Management_System
{
    partial class Dialog_BillAdd
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
            this.LBL_BillCode = new System.Windows.Forms.Label();
            this.TXT_BillCode = new System.Windows.Forms.TextBox();
            this.LBL_CustomerCode = new System.Windows.Forms.Label();
            this.LBL_CustomerName = new System.Windows.Forms.Label();
            this.TXT_CustomerCode = new System.Windows.Forms.TextBox();
            this.TXT_CustomerName = new System.Windows.Forms.TextBox();
            this.LBL_ProductCode = new System.Windows.Forms.Label();
            this.TXT_ProductCode = new System.Windows.Forms.TextBox();
            this.LBL_ProductName = new System.Windows.Forms.Label();
            this.TXT_ProductName = new System.Windows.Forms.TextBox();
            this.DGV_ProductList = new System.Windows.Forms.DataGridView();
            this.TXT_ProductQuantity = new System.Windows.Forms.TextBox();
            this.LBL_ProductQuantity = new System.Windows.Forms.Label();
            this.LBL_ProductRemain = new System.Windows.Forms.Label();
            this.TXT_ProductRemain = new System.Windows.Forms.TextBox();
            this.LBL_TotalBillPrice = new System.Windows.Forms.Label();
            this.TXT_BillPrice = new System.Windows.Forms.TextBox();
            this.RTB_BillNote = new System.Windows.Forms.RichTextBox();
            this.LBL_BillNote = new System.Windows.Forms.Label();
            this.LBL_CustomerPaidMoney = new System.Windows.Forms.Label();
            this.TXT_CustomerPaidMoney = new System.Windows.Forms.TextBox();
            this.LBL_CurrentBalance = new System.Windows.Forms.Label();
            this.TXT_CurrentBalance = new System.Windows.Forms.TextBox();
            this.GBX_CustomerInformation = new System.Windows.Forms.GroupBox();
            this.GBX_ProductInformation = new System.Windows.Forms.GroupBox();
            this.PNL_CustomerInformation = new System.Windows.Forms.Panel();
            this.TPL_BillAddMain = new System.Windows.Forms.TableLayoutPanel();
            this.TPL_ProductListAndPrice = new System.Windows.Forms.TableLayoutPanel();
            this.PNL_PaidPrice = new System.Windows.Forms.Panel();
            this.PNL_ProductInformation = new System.Windows.Forms.Panel();
            this.BTN_ProductMinus = new System.Windows.Forms.Button();
            this.BTN_ProductPlus = new System.Windows.Forms.Button();
            this.BTN_SaveBill = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_ProductList)).BeginInit();
            this.GBX_CustomerInformation.SuspendLayout();
            this.GBX_ProductInformation.SuspendLayout();
            this.PNL_CustomerInformation.SuspendLayout();
            this.TPL_BillAddMain.SuspendLayout();
            this.TPL_ProductListAndPrice.SuspendLayout();
            this.PNL_PaidPrice.SuspendLayout();
            this.PNL_ProductInformation.SuspendLayout();
            this.SuspendLayout();
            // 
            // LBL_BillCode
            // 
            this.LBL_BillCode.AutoSize = true;
            this.LBL_BillCode.ForeColor = System.Drawing.Color.Black;
            this.LBL_BillCode.Location = new System.Drawing.Point(12, 13);
            this.LBL_BillCode.Name = "LBL_BillCode";
            this.LBL_BillCode.Size = new System.Drawing.Size(65, 13);
            this.LBL_BillCode.TabIndex = 0;
            this.LBL_BillCode.Text = "Mã hóa đơn";
            // 
            // TXT_BillCode
            // 
            this.TXT_BillCode.Location = new System.Drawing.Point(15, 29);
            this.TXT_BillCode.Name = "TXT_BillCode";
            this.TXT_BillCode.Size = new System.Drawing.Size(185, 20);
            this.TXT_BillCode.TabIndex = 1;
            // 
            // LBL_CustomerCode
            // 
            this.LBL_CustomerCode.AutoSize = true;
            this.LBL_CustomerCode.ForeColor = System.Drawing.Color.Black;
            this.LBL_CustomerCode.Location = new System.Drawing.Point(8, 21);
            this.LBL_CustomerCode.Name = "LBL_CustomerCode";
            this.LBL_CustomerCode.Size = new System.Drawing.Size(82, 13);
            this.LBL_CustomerCode.TabIndex = 3;
            this.LBL_CustomerCode.Text = "Mã khách hàng";
            // 
            // LBL_CustomerName
            // 
            this.LBL_CustomerName.AutoSize = true;
            this.LBL_CustomerName.ForeColor = System.Drawing.Color.Black;
            this.LBL_CustomerName.Location = new System.Drawing.Point(8, 66);
            this.LBL_CustomerName.Name = "LBL_CustomerName";
            this.LBL_CustomerName.Size = new System.Drawing.Size(86, 13);
            this.LBL_CustomerName.TabIndex = 6;
            this.LBL_CustomerName.Text = "Tên khách hàng";
            // 
            // TXT_CustomerCode
            // 
            this.TXT_CustomerCode.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.TXT_CustomerCode.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.TXT_CustomerCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.TXT_CustomerCode.Location = new System.Drawing.Point(9, 37);
            this.TXT_CustomerCode.Name = "TXT_CustomerCode";
            this.TXT_CustomerCode.Size = new System.Drawing.Size(185, 20);
            this.TXT_CustomerCode.TabIndex = 10;
            this.TXT_CustomerCode.TextChanged += new System.EventHandler(this.TXT_CustomerCode_TextChanged);
            // 
            // TXT_CustomerName
            // 
            this.TXT_CustomerName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.TXT_CustomerName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.TXT_CustomerName.Location = new System.Drawing.Point(9, 82);
            this.TXT_CustomerName.Name = "TXT_CustomerName";
            this.TXT_CustomerName.Size = new System.Drawing.Size(185, 20);
            this.TXT_CustomerName.TabIndex = 11;
            // 
            // LBL_ProductCode
            // 
            this.LBL_ProductCode.AutoSize = true;
            this.LBL_ProductCode.ForeColor = System.Drawing.Color.Black;
            this.LBL_ProductCode.Location = new System.Drawing.Point(6, 21);
            this.LBL_ProductCode.Name = "LBL_ProductCode";
            this.LBL_ProductCode.Size = new System.Drawing.Size(71, 13);
            this.LBL_ProductCode.TabIndex = 12;
            this.LBL_ProductCode.Text = "Mã sản phẩm";
            // 
            // TXT_ProductCode
            // 
            this.TXT_ProductCode.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.TXT_ProductCode.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.TXT_ProductCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.TXT_ProductCode.Location = new System.Drawing.Point(6, 37);
            this.TXT_ProductCode.Name = "TXT_ProductCode";
            this.TXT_ProductCode.Size = new System.Drawing.Size(177, 20);
            this.TXT_ProductCode.TabIndex = 13;
            this.TXT_ProductCode.TextChanged += new System.EventHandler(this.TXT_ProductCode_TextChanged);
            // 
            // LBL_ProductName
            // 
            this.LBL_ProductName.AutoSize = true;
            this.LBL_ProductName.ForeColor = System.Drawing.Color.Black;
            this.LBL_ProductName.Location = new System.Drawing.Point(6, 66);
            this.LBL_ProductName.Name = "LBL_ProductName";
            this.LBL_ProductName.Size = new System.Drawing.Size(75, 13);
            this.LBL_ProductName.TabIndex = 14;
            this.LBL_ProductName.Text = "Tên sản phẩm";
            // 
            // TXT_ProductName
            // 
            this.TXT_ProductName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.TXT_ProductName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.TXT_ProductName.Location = new System.Drawing.Point(6, 82);
            this.TXT_ProductName.Name = "TXT_ProductName";
            this.TXT_ProductName.Size = new System.Drawing.Size(177, 20);
            this.TXT_ProductName.TabIndex = 15;
            this.TXT_ProductName.TextChanged += new System.EventHandler(this.TXT_ProductName_TextChanged);
            // 
            // DGV_ProductList
            // 
            this.DGV_ProductList.AllowUserToAddRows = false;
            this.DGV_ProductList.AllowUserToDeleteRows = false;
            this.DGV_ProductList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DGV_ProductList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_ProductList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGV_ProductList.Location = new System.Drawing.Point(3, 3);
            this.DGV_ProductList.MultiSelect = false;
            this.DGV_ProductList.Name = "DGV_ProductList";
            this.DGV_ProductList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGV_ProductList.Size = new System.Drawing.Size(327, 189);
            this.DGV_ProductList.TabIndex = 16;
            // 
            // TXT_ProductQuantity
            // 
            this.TXT_ProductQuantity.Location = new System.Drawing.Point(6, 130);
            this.TXT_ProductQuantity.Name = "TXT_ProductQuantity";
            this.TXT_ProductQuantity.Size = new System.Drawing.Size(177, 20);
            this.TXT_ProductQuantity.TabIndex = 21;
            this.TXT_ProductQuantity.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TXT_ProductQuantity_KeyPress);
            // 
            // LBL_ProductQuantity
            // 
            this.LBL_ProductQuantity.AutoSize = true;
            this.LBL_ProductQuantity.ForeColor = System.Drawing.Color.Black;
            this.LBL_ProductQuantity.Location = new System.Drawing.Point(6, 114);
            this.LBL_ProductQuantity.Name = "LBL_ProductQuantity";
            this.LBL_ProductQuantity.Size = new System.Drawing.Size(98, 13);
            this.LBL_ProductQuantity.TabIndex = 22;
            this.LBL_ProductQuantity.Text = "Số lượng sản phẩm";
            // 
            // LBL_ProductRemain
            // 
            this.LBL_ProductRemain.AutoSize = true;
            this.LBL_ProductRemain.ForeColor = System.Drawing.Color.Black;
            this.LBL_ProductRemain.Location = new System.Drawing.Point(6, 162);
            this.LBL_ProductRemain.Name = "LBL_ProductRemain";
            this.LBL_ProductRemain.Size = new System.Drawing.Size(97, 13);
            this.LBL_ProductRemain.TabIndex = 23;
            this.LBL_ProductRemain.Text = "Số lượng trong kho";
            // 
            // TXT_ProductRemain
            // 
            this.TXT_ProductRemain.Enabled = false;
            this.TXT_ProductRemain.Location = new System.Drawing.Point(6, 178);
            this.TXT_ProductRemain.Name = "TXT_ProductRemain";
            this.TXT_ProductRemain.Size = new System.Drawing.Size(177, 20);
            this.TXT_ProductRemain.TabIndex = 24;
            // 
            // LBL_TotalBillPrice
            // 
            this.LBL_TotalBillPrice.AutoSize = true;
            this.LBL_TotalBillPrice.ForeColor = System.Drawing.Color.Black;
            this.LBL_TotalBillPrice.Location = new System.Drawing.Point(3, 11);
            this.LBL_TotalBillPrice.Name = "LBL_TotalBillPrice";
            this.LBL_TotalBillPrice.Size = new System.Drawing.Size(103, 13);
            this.LBL_TotalBillPrice.TabIndex = 25;
            this.LBL_TotalBillPrice.Text = "Tổng giá trị hóa đơn";
            // 
            // TXT_BillPrice
            // 
            this.TXT_BillPrice.Enabled = false;
            this.TXT_BillPrice.Location = new System.Drawing.Point(3, 27);
            this.TXT_BillPrice.Name = "TXT_BillPrice";
            this.TXT_BillPrice.Size = new System.Drawing.Size(185, 20);
            this.TXT_BillPrice.TabIndex = 26;
            // 
            // RTB_BillNote
            // 
            this.RTB_BillNote.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RTB_BillNote.Location = new System.Drawing.Point(6, 213);
            this.RTB_BillNote.MaxLength = 1024;
            this.RTB_BillNote.Name = "RTB_BillNote";
            this.RTB_BillNote.Size = new System.Drawing.Size(219, 117);
            this.RTB_BillNote.TabIndex = 27;
            this.RTB_BillNote.Text = "";
            // 
            // LBL_BillNote
            // 
            this.LBL_BillNote.AutoSize = true;
            this.LBL_BillNote.ForeColor = System.Drawing.Color.Black;
            this.LBL_BillNote.Location = new System.Drawing.Point(14, 185);
            this.LBL_BillNote.Name = "LBL_BillNote";
            this.LBL_BillNote.Size = new System.Drawing.Size(44, 13);
            this.LBL_BillNote.TabIndex = 28;
            this.LBL_BillNote.Text = "Ghi chú";
            // 
            // LBL_CustomerPaidMoney
            // 
            this.LBL_CustomerPaidMoney.AutoSize = true;
            this.LBL_CustomerPaidMoney.ForeColor = System.Drawing.Color.Black;
            this.LBL_CustomerPaidMoney.Location = new System.Drawing.Point(3, 103);
            this.LBL_CustomerPaidMoney.Name = "LBL_CustomerPaidMoney";
            this.LBL_CustomerPaidMoney.Size = new System.Drawing.Size(103, 13);
            this.LBL_CustomerPaidMoney.TabIndex = 29;
            this.LBL_CustomerPaidMoney.Text = "Tiền khách hàng trả";
            // 
            // TXT_CustomerPaidMoney
            // 
            this.TXT_CustomerPaidMoney.Location = new System.Drawing.Point(3, 118);
            this.TXT_CustomerPaidMoney.Name = "TXT_CustomerPaidMoney";
            this.TXT_CustomerPaidMoney.Size = new System.Drawing.Size(185, 20);
            this.TXT_CustomerPaidMoney.TabIndex = 30;
            this.TXT_CustomerPaidMoney.TextChanged += new System.EventHandler(this.TXT_CustomerPaidMoney_TextChanged);
            this.TXT_CustomerPaidMoney.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TXT_CustomerPaidMoney_KeyPress);
            // 
            // LBL_CurrentBalance
            // 
            this.LBL_CurrentBalance.AutoSize = true;
            this.LBL_CurrentBalance.ForeColor = System.Drawing.Color.Black;
            this.LBL_CurrentBalance.Location = new System.Drawing.Point(3, 61);
            this.LBL_CurrentBalance.Name = "LBL_CurrentBalance";
            this.LBL_CurrentBalance.Size = new System.Drawing.Size(70, 13);
            this.LBL_CurrentBalance.TabIndex = 31;
            this.LBL_CurrentBalance.Text = "Số tiền dư nợ";
            // 
            // TXT_CurrentBalance
            // 
            this.TXT_CurrentBalance.Enabled = false;
            this.TXT_CurrentBalance.Location = new System.Drawing.Point(3, 77);
            this.TXT_CurrentBalance.Name = "TXT_CurrentBalance";
            this.TXT_CurrentBalance.Size = new System.Drawing.Size(185, 20);
            this.TXT_CurrentBalance.TabIndex = 32;
            // 
            // GBX_CustomerInformation
            // 
            this.GBX_CustomerInformation.Controls.Add(this.LBL_CustomerCode);
            this.GBX_CustomerInformation.Controls.Add(this.TXT_CustomerCode);
            this.GBX_CustomerInformation.Controls.Add(this.LBL_CustomerName);
            this.GBX_CustomerInformation.Controls.Add(this.TXT_CustomerName);
            this.GBX_CustomerInformation.ForeColor = System.Drawing.Color.Black;
            this.GBX_CustomerInformation.Location = new System.Drawing.Point(6, 66);
            this.GBX_CustomerInformation.Name = "GBX_CustomerInformation";
            this.GBX_CustomerInformation.Size = new System.Drawing.Size(219, 109);
            this.GBX_CustomerInformation.TabIndex = 33;
            this.GBX_CustomerInformation.TabStop = false;
            this.GBX_CustomerInformation.Text = "Thông tin khách hàng";
            // 
            // GBX_ProductInformation
            // 
            this.GBX_ProductInformation.BackColor = System.Drawing.Color.Transparent;
            this.GBX_ProductInformation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.GBX_ProductInformation.Controls.Add(this.LBL_ProductName);
            this.GBX_ProductInformation.Controls.Add(this.TXT_ProductName);
            this.GBX_ProductInformation.Controls.Add(this.LBL_ProductQuantity);
            this.GBX_ProductInformation.Controls.Add(this.TXT_ProductQuantity);
            this.GBX_ProductInformation.Controls.Add(this.LBL_ProductRemain);
            this.GBX_ProductInformation.Controls.Add(this.TXT_ProductRemain);
            this.GBX_ProductInformation.Controls.Add(this.TXT_ProductCode);
            this.GBX_ProductInformation.Controls.Add(this.LBL_ProductCode);
            this.GBX_ProductInformation.ForeColor = System.Drawing.Color.Black;
            this.GBX_ProductInformation.Location = new System.Drawing.Point(6, 9);
            this.GBX_ProductInformation.Name = "GBX_ProductInformation";
            this.GBX_ProductInformation.Size = new System.Drawing.Size(189, 213);
            this.GBX_ProductInformation.TabIndex = 34;
            this.GBX_ProductInformation.TabStop = false;
            this.GBX_ProductInformation.Text = "Thông tin sản phẩm";
            // 
            // PNL_CustomerInformation
            // 
            this.PNL_CustomerInformation.BackColor = System.Drawing.Color.Transparent;
            this.PNL_CustomerInformation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.PNL_CustomerInformation.Controls.Add(this.TXT_BillCode);
            this.PNL_CustomerInformation.Controls.Add(this.GBX_CustomerInformation);
            this.PNL_CustomerInformation.Controls.Add(this.RTB_BillNote);
            this.PNL_CustomerInformation.Controls.Add(this.LBL_BillCode);
            this.PNL_CustomerInformation.Controls.Add(this.LBL_BillNote);
            this.PNL_CustomerInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PNL_CustomerInformation.Location = new System.Drawing.Point(3, 3);
            this.PNL_CustomerInformation.Name = "PNL_CustomerInformation";
            this.PNL_CustomerInformation.Size = new System.Drawing.Size(234, 345);
            this.PNL_CustomerInformation.TabIndex = 0;
            // 
            // TPL_BillAddMain
            // 
            this.TPL_BillAddMain.BackColor = System.Drawing.Color.Transparent;
            this.TPL_BillAddMain.ColumnCount = 3;
            this.TPL_BillAddMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.TPL_BillAddMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 204F));
            this.TPL_BillAddMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.TPL_BillAddMain.Controls.Add(this.PNL_CustomerInformation, 0, 0);
            this.TPL_BillAddMain.Controls.Add(this.TPL_ProductListAndPrice, 2, 0);
            this.TPL_BillAddMain.Controls.Add(this.PNL_ProductInformation, 1, 0);
            this.TPL_BillAddMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TPL_BillAddMain.Location = new System.Drawing.Point(0, 0);
            this.TPL_BillAddMain.Name = "TPL_BillAddMain";
            this.TPL_BillAddMain.RowCount = 1;
            this.TPL_BillAddMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TPL_BillAddMain.Size = new System.Drawing.Size(783, 351);
            this.TPL_BillAddMain.TabIndex = 36;
            // 
            // TPL_ProductListAndPrice
            // 
            this.TPL_ProductListAndPrice.BackColor = System.Drawing.Color.Transparent;
            this.TPL_ProductListAndPrice.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.TPL_ProductListAndPrice.ColumnCount = 1;
            this.TPL_ProductListAndPrice.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TPL_ProductListAndPrice.Controls.Add(this.DGV_ProductList, 0, 0);
            this.TPL_ProductListAndPrice.Controls.Add(this.PNL_PaidPrice, 0, 1);
            this.TPL_ProductListAndPrice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TPL_ProductListAndPrice.Location = new System.Drawing.Point(447, 3);
            this.TPL_ProductListAndPrice.Name = "TPL_ProductListAndPrice";
            this.TPL_ProductListAndPrice.RowCount = 2;
            this.TPL_ProductListAndPrice.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TPL_ProductListAndPrice.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.TPL_ProductListAndPrice.Size = new System.Drawing.Size(333, 345);
            this.TPL_ProductListAndPrice.TabIndex = 35;
            // 
            // PNL_PaidPrice
            // 
            this.PNL_PaidPrice.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.PNL_PaidPrice.Controls.Add(this.BTN_SaveBill);
            this.PNL_PaidPrice.Controls.Add(this.LBL_TotalBillPrice);
            this.PNL_PaidPrice.Controls.Add(this.TXT_BillPrice);
            this.PNL_PaidPrice.Controls.Add(this.TXT_CustomerPaidMoney);
            this.PNL_PaidPrice.Controls.Add(this.TXT_CurrentBalance);
            this.PNL_PaidPrice.Controls.Add(this.LBL_CustomerPaidMoney);
            this.PNL_PaidPrice.Controls.Add(this.LBL_CurrentBalance);
            this.PNL_PaidPrice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PNL_PaidPrice.Location = new System.Drawing.Point(3, 198);
            this.PNL_PaidPrice.Name = "PNL_PaidPrice";
            this.PNL_PaidPrice.Size = new System.Drawing.Size(327, 144);
            this.PNL_PaidPrice.TabIndex = 17;
            // 
            // PNL_ProductInformation
            // 
            this.PNL_ProductInformation.BackColor = System.Drawing.Color.Transparent;
            this.PNL_ProductInformation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.PNL_ProductInformation.Controls.Add(this.BTN_ProductPlus);
            this.PNL_ProductInformation.Controls.Add(this.BTN_ProductMinus);
            this.PNL_ProductInformation.Controls.Add(this.GBX_ProductInformation);
            this.PNL_ProductInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PNL_ProductInformation.Location = new System.Drawing.Point(243, 3);
            this.PNL_ProductInformation.Name = "PNL_ProductInformation";
            this.PNL_ProductInformation.Size = new System.Drawing.Size(198, 345);
            this.PNL_ProductInformation.TabIndex = 36;
            // 
            // BTN_ProductMinus
            // 
            this.BTN_ProductMinus.Location = new System.Drawing.Point(6, 313);
            this.BTN_ProductMinus.Name = "BTN_ProductMinus";
            this.BTN_ProductMinus.Size = new System.Drawing.Size(75, 23);
            this.BTN_ProductMinus.TabIndex = 37;
            this.BTN_ProductMinus.Text = "-";
            this.BTN_ProductMinus.UseVisualStyleBackColor = true;
            this.BTN_ProductMinus.Click += new System.EventHandler(this.BTN_DeleteProduct_Click);
            // 
            // BTN_ProductPlus
            // 
            this.BTN_ProductPlus.Location = new System.Drawing.Point(114, 313);
            this.BTN_ProductPlus.Name = "BTN_ProductPlus";
            this.BTN_ProductPlus.Size = new System.Drawing.Size(75, 23);
            this.BTN_ProductPlus.TabIndex = 38;
            this.BTN_ProductPlus.Text = "+";
            this.BTN_ProductPlus.UseVisualStyleBackColor = true;
            this.BTN_ProductPlus.Click += new System.EventHandler(this.BTN_AddProduct_Click);
            // 
            // BTN_SaveBill
            // 
            this.BTN_SaveBill.Location = new System.Drawing.Point(246, 115);
            this.BTN_SaveBill.Name = "BTN_SaveBill";
            this.BTN_SaveBill.Size = new System.Drawing.Size(75, 23);
            this.BTN_SaveBill.TabIndex = 33;
            this.BTN_SaveBill.Text = "Ghi hóa đơn";
            this.BTN_SaveBill.UseVisualStyleBackColor = true;
            this.BTN_SaveBill.Click += new System.EventHandler(this.BTN_SaveBill_Click);
            // 
            // Dialog_BillAdd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(783, 351);
            this.Controls.Add(this.TPL_BillAddMain);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Dialog_BillAdd";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thêm hóa đơn";
            ((System.ComponentModel.ISupportInitialize)(this.DGV_ProductList)).EndInit();
            this.GBX_CustomerInformation.ResumeLayout(false);
            this.GBX_CustomerInformation.PerformLayout();
            this.GBX_ProductInformation.ResumeLayout(false);
            this.GBX_ProductInformation.PerformLayout();
            this.PNL_CustomerInformation.ResumeLayout(false);
            this.PNL_CustomerInformation.PerformLayout();
            this.TPL_BillAddMain.ResumeLayout(false);
            this.TPL_ProductListAndPrice.ResumeLayout(false);
            this.PNL_PaidPrice.ResumeLayout(false);
            this.PNL_PaidPrice.PerformLayout();
            this.PNL_ProductInformation.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LBL_BillCode;
        private System.Windows.Forms.TextBox TXT_BillCode;
        private System.Windows.Forms.Label LBL_CustomerCode;
        private System.Windows.Forms.Label LBL_CustomerName;
        private System.Windows.Forms.TextBox TXT_CustomerCode;
        private System.Windows.Forms.TextBox TXT_CustomerName;
        private System.Windows.Forms.Label LBL_ProductCode;
        private System.Windows.Forms.TextBox TXT_ProductCode;
        private System.Windows.Forms.Label LBL_ProductName;
        private System.Windows.Forms.TextBox TXT_ProductName;
        private System.Windows.Forms.DataGridView DGV_ProductList;
        private System.Windows.Forms.TextBox TXT_ProductQuantity;
        private System.Windows.Forms.Label LBL_ProductQuantity;
        private System.Windows.Forms.Label LBL_ProductRemain;
        private System.Windows.Forms.TextBox TXT_ProductRemain;
        private System.Windows.Forms.Label LBL_TotalBillPrice;
        private System.Windows.Forms.TextBox TXT_BillPrice;
        private System.Windows.Forms.RichTextBox RTB_BillNote;
        private System.Windows.Forms.Label LBL_BillNote;
        private System.Windows.Forms.Label LBL_CustomerPaidMoney;
        private System.Windows.Forms.TextBox TXT_CustomerPaidMoney;
        private System.Windows.Forms.Label LBL_CurrentBalance;
        private System.Windows.Forms.TextBox TXT_CurrentBalance;
        private System.Windows.Forms.GroupBox GBX_CustomerInformation;
        private System.Windows.Forms.GroupBox GBX_ProductInformation;
        private System.Windows.Forms.Panel PNL_CustomerInformation;
        private System.Windows.Forms.TableLayoutPanel TPL_BillAddMain;
        private System.Windows.Forms.TableLayoutPanel TPL_ProductListAndPrice;
        private System.Windows.Forms.Panel PNL_PaidPrice;
        private System.Windows.Forms.Panel PNL_ProductInformation;
        private System.Windows.Forms.Button BTN_SaveBill;
        private System.Windows.Forms.Button BTN_ProductPlus;
        private System.Windows.Forms.Button BTN_ProductMinus;
    }
}