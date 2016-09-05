using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections;

namespace Sale_Management_System
{
    public partial class Dialog_BillAdd : Form
    {
        // Parent Form
        public Form_BillManagement ParentFormId = null;

        // Store information of Total Bill Price
        private double dTotalBillPrice = 0;

        // Store information of Current Balance
        private double dCurrentBalance = 0;

        #region Constructor

        public Dialog_BillAdd()
        {
            InitializeComponent();
            
            // Reset parent form to null to prevent us from updating information to wrong form
            ParentFormId = null;
            
            // Read Customer Code and Name and fill the read to Text Suggestion
            fnReadCustomer();
            
            // Read product Code and Name and fill the read information to Text Suggestion
            fnReadProduct();

            // Create columns for DataGridView table
            fnCreateTableProduct();


        }

        #endregion

        #region Button Events

        // This event is fired when user clicks Generate button to Generate a random code string for Bill Code Text Field
        private void BTN_BillCodeGenerate_Click(object sender, EventArgs e)
        {
            // Create a variable to hold times the function tried to generate a code string for a customer
            // If time is more than 5 and the generation function can't generate any code strings
            // Display an error message to screen and tell the user to try again
            int iTriedTime = 0;

            // Create a connection to SQL Database to check if Customer code already existed or not
            SqlConnection SqlConnectionId = util_Sql.fnConnectToDataBase(UserSettings.SQL_DATASOURCE, UserSettings.SQL_CATALOG, UserSettings.SQL_AUTHENTICATION_METHOD, UserSettings.SQL_USER_NAME, UserSettings.SQL_PASSWORD);

            // Connection failed
            if (SqlConnectionId == null || SqlConnectionId.State == ConnectionState.Broken || SqlConnectionId.State == ConnectionState.Closed)
            {
                SqlConnectionId = null;
                return;
            }

            for (; iTriedTime < 5; iTriedTime++)
            {
                // Generate customer code
                string szBillCode = GlobalFunctions.fnGenerateCode(GlobalConstants.PREFIX_BILL);

                // If the generated code doesn't exist in databse 
                if (!util_Sql.fnIsInDatabase(SqlConnectionId, GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL, GlobalConstants.COLUMN_BILL_CODE, szBillCode))
                {
                    // Update to Customer Code text field
                    TXT_BillCode.Text = szBillCode;

                    // Set user to focus on Customer code text field
                    TXT_BillCode.Focus();

                    // Display notify message
                    MessageBox.Show("Hóa đơn được tạo thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Close Sql Connection
                    SqlConnectionId.Close();

                    // Terminate function
                    return;
                }
            }

            // Close Sql Connection
            SqlConnectionId.Close();

            // 5 times reached, and we can not generate any new code strings ?
            // Display an error message
            MessageBox.Show("Tạo mã hóa đơn bị lỗi", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // Set user to focus on Customer code text field
            TXT_BillCode.Focus();
        }

        // This event is fired when user clicks Generate button to Generate a random code string for Customer Code Text Field
        private void BTN_CustomerCodeGenerate_Click(object sender, EventArgs e)
        {
            // Create a variable to hold times the function tried to generate a code string for a customer
            // If time is more than 5 and the generation function can't generate any code strings
            // Display an error message to screen and tell the user to try again
            int iTriedTime = 0;

            // Create a connection to SQL Database to check if Customer code already existed or not
            SqlConnection SqlConnectionId = util_Sql.fnConnectToDataBase(UserSettings.SQL_DATASOURCE, UserSettings.SQL_CATALOG, UserSettings.SQL_AUTHENTICATION_METHOD, UserSettings.SQL_USER_NAME, UserSettings.SQL_PASSWORD);

            // Connection failed
            if (SqlConnectionId == null || SqlConnectionId.State == ConnectionState.Broken || SqlConnectionId.State == ConnectionState.Closed)
            {
                SqlConnectionId = null;
                return;
            }

            for (; iTriedTime < 5; iTriedTime++)
            {
                // Generate customer code
                string szCustomerCode = GlobalFunctions.fnGenerateCode(GlobalConstants.PREFIX_CUSTOMER);

                // If the generated code doesn't exist in databse 
                if (!util_Sql.fnIsInDatabase(SqlConnectionId, GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, GlobalConstants.COLUMN_CUSTOMER_CODE, szCustomerCode))
                {
                    // Update to Customer Code text field
                    TXT_CustomerCode.Text = szCustomerCode;

                    // Set user to focus on Customer code text field
                    TXT_CustomerCode.Focus();

                    // Close Sql Connection
                    SqlConnectionId.Close();

                    // Enable Customer Name Text Field
                    TXT_CustomerName.Enabled = true;
                    TXT_CustomerName.Text = "";

                    // Terminate function
                    return;
                }
            }

            // Close Sql Connection
            SqlConnectionId.Close();

            // 5 times reached, and we can not generate any new code strings ?
            // Display an error message
            MessageBox.Show("Quá trình tạo mã bị lỗi", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // Set user to focus on Customer code text field
            TXT_CustomerCode.Focus();

        }

        // This event is fired when user clicks Add product to add product to the list
        private void BTN_AddProduct_Click(object sender, EventArgs e)
        {
            // Connect to Database
            SqlConnection SqlConnectionId = util_Sql.fnConnectToDataBase(UserSettings.SQL_DATASOURCE, UserSettings.SQL_CATALOG, UserSettings.SQL_AUTHENTICATION_METHOD, UserSettings.SQL_USER_NAME, UserSettings.SQL_PASSWORD);

            // Connection is fail
            if (SqlConnectionId == null)
            {
                // Display error message
                MessageBox.Show("Kết nối dữ liệu không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            // If they didn't fill anything inProduct code text field
            TXT_ProductCode.Text.Trim();

            if (TXT_ProductCode.Text.Length < 1)
            {
                // Close Sql connection
                SqlConnectionId.Close();

                // Display warning message
                MessageBox.Show("Mã sản phẩm phải được điền đầy đủ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Focus on Product code text field
                TXT_ProductCode.Focus();

                return;
            }

            // If they didn't fill anything in product name text field
            TXT_ProductName.Text.Trim();

            if (TXT_ProductName.Text.Length < 1)
            {
                // Close Sql connection
                SqlConnectionId.Close();

                // Display warning message
                MessageBox.Show("Tên sản phẩm phải được điền đầy đủ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Focus on Product name text field
                TXT_ProductName.Focus();

                return;
            }

            // If they didn't fill anything in product quantity
            TXT_ProductQuantity.Text.Replace(" ", "");

            // Store information of product quantity
            double dProductQuantity;

            if (TXT_ProductQuantity.Text.Length < 1)
            {
                // Close Sql connection
                SqlConnectionId.Close();

                // Display warning message
                MessageBox.Show("Số lượng sản phẩm phải được điền đầy đủ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Focus on Product name text field
                TXT_ProductQuantity.Focus();
                
                return;
            }

            try
            {
                // Convert string of price to float number
                dProductQuantity = double.Parse(TXT_ProductQuantity.Text);
            }
            catch
            {
                // Close Sql connection
                SqlConnectionId.Close();

                // If an error happens
                // Display error diaglog and make user input again
                MessageBox.Show("Dữ liệu không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Focus on Product quantity
                TXT_ProductQuantity.Focus();

                return;
            }

            // Quantity must be greater than 0
            if (dProductQuantity <= 0)
            {
                // Close Sql connection
                SqlConnectionId.Close();

                // If an error happens
                // Display error diaglog and make user input again
                MessageBox.Show("Dữ liệu không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Focus on Product price
                TXT_ProductQuantity.Focus();

                return;
            }

            // Build querry command
            string szCommand = String.Format("SELECT {0}, {1}, {2} FROM {3} WHERE {4} = '{5}'", GlobalConstants.COLUMN_PRODUCT_PRICE, GlobalConstants.COLUMN_PRODUCT_QUANTITY, GlobalConstants.COLUMN_PRODUCT_UNIT, GlobalConstants.TABLE_PRODUCT_MANAGEMENT,
                                                                            GlobalConstants.COLUMN_PRODUCT_CODE,
                                                                            TXT_ProductCode.Text);

            // Create an object of SqlCommand 
            SqlCommand SqlCommandId = new SqlCommand();
            
            // Bind Datasource to SqlCommand
            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            
            // Execute querry command and save the result to SqlDataReader's object
            SqlDataReader SqlDataReaderId;

            try
            {
                SqlDataReaderId = SqlCommandId.ExecuteReader();
            }
            catch
            {
                // Close Sql connection
                SqlConnectionId.Close();

                // Display error message
                MessageBox.Show("Kết nối dữ liệu không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            // Has no result return ? 
            if (!SqlDataReaderId.HasRows)
            {
                // Close Data Reader
                SqlDataReaderId.Close();

                // Close Sql connection
                SqlConnectionId.Close();

                return;
            }

            double dSqlProductPrice = 0;
            double dSqlProductQuantity = 0;

            // Read information of Product Price and Product Quantity
            while (SqlDataReaderId.Read())
            {
                dSqlProductPrice = double.Parse(SqlDataReaderId[GlobalConstants.COLUMN_PRODUCT_PRICE].ToString());
                dSqlProductQuantity = double.Parse(SqlDataReaderId[GlobalConstants.COLUMN_PRODUCT_QUANTITY].ToString());
                break;
            }
            

            // Customer bought more products than saler has ?
            if (dProductQuantity > dSqlProductQuantity)
            {
                // Close Data Reader
                SqlDataReaderId.Close();

                // Close Sql connection
                SqlConnectionId.Close();

                // Display error message
                MessageBox.Show("Không đủ sản phẩm", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            // This flag turn to true when we found the matched result
            bool bResultFound = false;

            // Find the Row in DataGridView whose Product Code column contains the adding Product Code
            
            foreach (DataGridViewRow RowId in DGV_ProductList.Rows)
            {
                // Meet the matched result
                if (!bResultFound && (RowId.Cells[GlobalConstants.COLUMN_PRODUCT_CODE].Value.ToString().Equals(TXT_ProductCode.Text)))
                {
                    // Total value of Product
                    double dProductTotalPrice = dProductQuantity * dSqlProductPrice;

                    // This product was added before
                    RowId.Cells[GlobalConstants.COLUMN_PRODUCT_QUANTITY].Value = dProductQuantity;
                    RowId.Cells[GlobalConstants.COLUMN_PRODUCT_TOTAL_PRICE].Value = dProductTotalPrice;

                    dTotalBillPrice += dProductTotalPrice;

                    // Make program know that we already found the matched result
                    bResultFound = true;

                }
                else
                {
                    // Retrive Total Product Price
                    double dTotalProductPrice = double.Parse(RowId.Cells[GlobalConstants.COLUMN_PRODUCT_TOTAL_PRICE].Value.ToString());

                    // Increase Total Bill Price
                    dTotalBillPrice += dTotalProductPrice;

                }
            }

            // This is the first time we add this product to the list ?
            if (!bResultFound)
            {
                // Retrieve Total Product Price
                double dTotalProductPrice = dProductQuantity * dSqlProductPrice;

                // Add new product to Product List in Bill Order
                DGV_ProductList.Rows.Add(TXT_ProductCode.Text, TXT_ProductName.Text, dProductQuantity, SqlDataReaderId[GlobalConstants.COLUMN_PRODUCT_UNIT].ToString() ,dSqlProductPrice, dTotalProductPrice.ToString("N3"));

                // Increase the Total Bill Price
                dTotalBillPrice += dTotalProductPrice;

            }   
            
            
            // Display to Bill Price Text Field
            TXT_BillPrice.Text = dTotalBillPrice.ToString("N3");

            // Clear Text Fields
            TXT_ProductCode.Text = "";
            TXT_ProductName.Text = "";
            TXT_ProductQuantity.Text = "";

            // Close Data Reader
            SqlDataReaderId.Close();

            // Close Sql connection
            SqlConnectionId.Close();

        }
        
        // This event is fired when user click Delete Product to delete a product from the
        private void BTN_DeleteProduct_Click(object sender, EventArgs e)
        {
            

            // No row is selected
            if (DGV_ProductList.SelectedRows.Count < 1)
            {
                // Display warning message
                MessageBox.Show("Bạn phải chọn một dòng", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            // Delete selected row
            foreach (DataGridViewRow RowId in DGV_ProductList.SelectedRows)
                DGV_ProductList.Rows.Remove(RowId);

            dTotalBillPrice = 0;

            // Update Bill Total Price
            foreach (DataGridViewRow RowId in DGV_ProductList.Rows)
                dTotalBillPrice += double.Parse(RowId.Cells[GlobalConstants.COLUMN_PRODUCT_TOTAL_PRICE].Value.ToString());

            // Display data to Bill Total Price TextField
            TXT_BillPrice.Text = dTotalBillPrice.ToString("N3", CultureInfo.CurrentCulture);
        }

        // This event is fired when user click Save Product to save a product
        private void BTN_SaveBill_Click(object sender, EventArgs e)
        {
            Hashtable ProductInfoList = new Hashtable();
        
            // No product in Order List
            if (DGV_ProductList.Rows.Count < 1)
            {
                // Display warning message
                MessageBox.Show("Chưa có sản phẩm nào được mua", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            // Blank Bill Code ?
            if (TXT_BillCode.Text.Replace(" ", "").Length < 1)
            {
                // Focus on Bill Code Text Field
                TXT_BillCode.Focus();

                // Display warning message
                MessageBox.Show("Mã hóa đơn phải được điền đầy đủ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (TXT_CustomerPaidMoney.Text.Trim().Length < 1)
            {
                // Display warning message
                MessageBox.Show("Số tiền khách hàng trả phải được điền", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            bool bCustomerAlreadyExisted = false;
            bool bCustomerInfoEnough = false;

            int iCustomerBillNumber = 0;
            double dCustomerBalance = 0;

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            #region SQL CONNECTION
            // Connect to Database
            SqlConnection SqlConnectionId = util_Sql.fnConnectToDataBase(UserSettings.SQL_DATASOURCE, UserSettings.SQL_CATALOG, UserSettings.SQL_AUTHENTICATION_METHOD, UserSettings.SQL_USER_NAME, UserSettings.SQL_PASSWORD);

            // Create SqlCommand's object to execute querry commands
            SqlCommand SqlCommandId = new SqlCommand();

            // Bind connection to SqlCommand
            SqlCommandId.Connection = SqlConnectionId;

            // An object of string to store information of querry command
            string szCommand;

            // An object of SqlDataReader to Store information of returned result
            SqlDataReader SqlDataReaderId;

            // Connection is fail
            if (SqlConnectionId == null)
            {
                // Display warning message
                MessageBox.Show("Kết nối dữ liệu không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            #endregion

            #region CUSTOMER CODE VALIDATION

            // No customer code ?
            if (TXT_CustomerCode.Text.Replace(" ", "").Length < 1 || TXT_CustomerName.Text.Replace(" ", "").Length < 1)
            {
                // Display warning message with Yes/No question
                if (MessageBox.Show("Không có dữ liệu khách hàng. Bạn có muốn tiếp tục ?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                {
                    // User says NO ?
                    // Close Sql Connection
                    SqlConnectionId.Close();
                    return;
                }
                // Yes ?
                // Clear these 2 Text Fields
                TXT_CustomerName.Text = "";
                TXT_CustomerCode.Text = "";
            }
            else
            {
                // Enough information for a Customer
                bCustomerInfoEnough = true;

                szCommand = "SELECT * FROM " + GlobalConstants.TABLE_CUSTOMER_MANAGEMENT + " ";
                szCommand += "WHERE " + GlobalConstants.COLUMN_CUSTOMER_CODE + " = ";
                szCommand += "'" + TXT_CustomerCode.Text + "'";

                SqlCommandId.CommandText = szCommand;

                try
                {
                    SqlDataReaderId = SqlCommandId.ExecuteReader();
                }
                catch
                {
                    // Close Sql Connection
                    SqlConnectionId.Close();

                    // Error happens
                    MessageBox.Show("Lưu hóa đơn bị lỗi", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                if (SqlDataReaderId.HasRows)
                {
                    // Customer already existed before
                    bCustomerAlreadyExisted = true;

                    // Read the returned data
                    SqlDataReaderId.Read();

                    // Read bill number
                    iCustomerBillNumber = int.Parse(SqlDataReaderId[GlobalConstants.COLUMN_CUSTOMER_BILL_QUANTITY].ToString());

                    // Read Customer's balance
                    dCustomerBalance = double.Parse(SqlDataReaderId[GlobalConstants.COLUMN_CUSTOMER_BALANCE].ToString().Replace(",", ""));

                    // Update to Customer Text Field
                    TXT_CustomerName.Text = SqlDataReaderId[GlobalConstants.COLUMN_CUSTOMER_NAME].ToString().Trim();
                }

                // Close SqlDataReader
                SqlDataReaderId.Close();
            }

            #endregion

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            #region BILL CODE VALIDATION

            // Build querry command
            // Validate Bill Code

            // Validate this Bill Code in BillManagementGeneral Table
            szCommand = String.Format("SELECT * FROM {0} WHERE {1} = '{2}'", GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL, GlobalConstants.COLUMN_BILL_CODE, TXT_BillCode.Text);
            
            // Bind connection to SqlCommand
            SqlCommandId.Connection = SqlConnectionId;
            
            // Bind querry command to SqlCommand
            SqlCommandId.CommandText = szCommand;



            // Execute reader command and store the returned result to SqlDataReader
            try
            {
                SqlDataReaderId = SqlCommandId.ExecuteReader();
            }
            catch
            {
                // Reading fail ?
                // Close SqlConnection
                SqlConnectionId.Close();

                // Display error message
                MessageBox.Show("Kế nối dữ liệu lỗi", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            // Close previous Data Reader
            SqlDataReaderId.Close();

            // Validate this Bill Code in BillManagementDetailed Table
            szCommand = String.Format("SELECT * FROM {0} WHERE {1} = '{2}'", GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL, GlobalConstants.COLUMN_BILL_CODE, TXT_BillCode.Text);
            
            // Bind querry command to SqlCommand
            SqlCommandId.CommandText = szCommand;


            // Execute reader command and store the returned result to SqlDataReader
            try
            {
                SqlDataReaderId = SqlCommandId.ExecuteReader();
            }
            catch
            {
                // Reading fail ?
                // Close SqlConnection
                SqlConnectionId.Close();

                // Display error message
                MessageBox.Show("Kết nối dữ liệu không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
               
                return;

            }

            // There is some results returned back
            // That means bill code already existed

            if (SqlDataReaderId.HasRows)
            {
                // Close SqlDataReader
                SqlDataReaderId.Close();

                // Close SqlConnection
                SqlConnectionId.Close();

                // Display warning message
                MessageBox.Show("Mã hóa đơn đã tồn tại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            // Close SqlDataReader to open the new one
            SqlDataReaderId.Close();

            #endregion

            #region PRODUCT VALIDATION

            foreach (DataGridViewRow RowId in DGV_ProductList.Rows)
            {
                // Build new querry to check if Products in the order list is valid or not
                szCommand = String.Format("SELECT * FROM {0} WHERE {1} = '{2}'", GlobalConstants.TABLE_PRODUCT_MANAGEMENT, GlobalConstants.COLUMN_PRODUCT_CODE, RowId.Cells[GlobalConstants.COLUMN_PRODUCT_CODE].Value.ToString());
                
                // Store command to SqlCommand
                SqlCommandId.CommandText = szCommand;

                // Execute querry command and store the returned 
                try
                {
                    SqlDataReaderId = SqlCommandId.ExecuteReader();
                }
                catch
                {
                    // Reading failed
                    // Close SqlConnection
                    SqlConnectionId.Close();

                    // Display error message
                    MessageBox.Show("Kết nối dữ liệu không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MessageBox.Show("Kết nối dữ liệu không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }

                // If there is no product in Product List ?
                if (!SqlDataReaderId.HasRows)
                {
                    // Close SqlDataReader and SqlConnection
                    SqlDataReaderId.Close();
                    SqlConnectionId.Close();

                    // Select the invalid row
                    RowId.Selected = true;

                    // Display warning message
                    
                    MessageBox.Show("Sản phẩm không tồn tại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Read data
                SqlDataReaderId.Read();

                // Product is valid ?
                // Check the quantity
                double dProductQuantity = double.Parse(RowId.Cells[GlobalConstants.COLUMN_PRODUCT_QUANTITY].Value.ToString());
                double dRealProductQuantity = double.Parse(SqlDataReaderId[GlobalConstants.COLUMN_PRODUCT_QUANTITY].ToString());

                if (dRealProductQuantity < dProductQuantity)
                {
                    // Close SqlDataReader and SqlConnection
                    SqlDataReaderId.Close();
                    SqlConnectionId.Close();

                    // Select the invalid row
                    RowId.Selected = true;

                    MessageBox.Show("Sai lệch số sản phẩm", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                // Store remaining products
                ProductInfoList.Add(RowId.Cells[GlobalConstants.COLUMN_PRODUCT_CODE].Value.ToString(), dRealProductQuantity - dProductQuantity);
                
                // Close previous SqlDataReader
                SqlDataReaderId.Close();
            }

            #endregion

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            #region DEBIT VALIDATION

            // Not enough money and Customer information is still missing ?
            if (dCurrentBalance != 0 && !bCustomerInfoEnough)
            {
                // Close Sql Connection
                SqlConnectionId.Close();

                // Display warning message
                MessageBox.Show("Không thể ghi nợ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            
            // Display confirmation dialog
            if (MessageBox.Show("Bạn có muốn lưu hóa đơn ?", "", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                // Close Sql Connection
                SqlConnectionId.Close();

                return;
            }

            #endregion

            #region BILL MANAGEMENT TABLE STRUCTURE

            // Write Data to BillManagementGeneral Table
            // Construction of BILL MANAGEMENT [GENERAL] TABLE
            /*
             * BILL CODE
             * CUSTOMER CODE
             * CUSTOMER NAME
             * MONEY
             * PAID MONEY
             * CREATED DATE
             * NOTE
            */

            #endregion

            #region BILL MANAGEMENT GENERAL [COMMAND BUILD]

            string szCreatedDate = DateTime.Now.ToString();
            szCommand =  string.Format("INSERT INTO {0}( ", GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL);
            szCommand +=        GlobalConstants.COLUMN_BILL_CODE;
            szCommand += ", " + GlobalConstants.COLUMN_CUSTOMER_CODE;
            szCommand += ", " + GlobalConstants.COLUMN_CUSTOMER_NAME;
            szCommand += ", " + GlobalConstants.COLUMN_BILL_MONEY;
            szCommand += ", " + GlobalConstants.COLUMN_PAID_MONEY;
            szCommand += ", " + GlobalConstants.COLUMN_BILL_CREATED_DATE;
            szCommand += ", " + GlobalConstants.COLUMN_NOTE;
            szCommand += ") ";
            szCommand += "VALUES (";
            szCommand +=        string.Format(" '{0}' ", TXT_BillCode.Text);
            szCommand +=        string.Format(", '{0}' ", TXT_CustomerCode.Text);
            szCommand +=        string.Format(", N'{0}' ", TXT_CustomerName.Text);
            szCommand +=        string.Format(", '{0}' ", TXT_BillPrice.Text.ToString().Replace(",", ""));             
            szCommand +=        string.Format(", '{0}' ", float.Parse(TXT_CustomerPaidMoney.Text));                 
            szCommand +=        string.Format(", '{0}' ", float.Parse(szCreatedDate));
            szCommand +=        string.Format(", N'{0}' ", RTB_BillNote.Text);
            szCommand += ")";

            #endregion

            // Bind querry command to SqlCommand
            SqlCommandId.CommandText = szCommand;

            // Execute command
            try
            {
                SqlCommandId.ExecuteNonQuery();
            }
            catch
            {
                // Error occurs ?
                // Close Sql Connection
                SqlConnectionId.Close();

                // Display message
                MessageBox.Show("Lưu dữ liệu không thành công", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                return;
            }


            #region BILL MANAGEMENT DETAILED UPDATE | PRODUCT UPDATE

            foreach (DataGridViewRow RowId in DGV_ProductList.Rows)
            {
                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Add order products to BILL MANAGEMENT TABLE [DETAILED]
                szCommand = "INSERT INTO " + GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED + " ";
                szCommand += "( ";
                szCommand +=        GlobalConstants.COLUMN_BILL_CODE + " ";
                szCommand += ", " + GlobalConstants.COLUMN_CUSTOMER_CODE + " ";
                szCommand += ", " + GlobalConstants.COLUMN_CUSTOMER_NAME + " ";
                szCommand += ", " + GlobalConstants.COLUMN_PRODUCT_CODE + " ";
                szCommand += ", " + GlobalConstants.COLUMN_PRODUCT_NAME + " ";
                szCommand += ", " + GlobalConstants.COLUMN_PRODUCT_QUANTITY + " ";
                szCommand += ", " + GlobalConstants.COLUMN_PRODUCT_UNIT + " ";
                szCommand += ", " + GlobalConstants.COLUMN_PRODUCT_TOTAL_PRICE + " ";
                szCommand += ") ";
                szCommand += "VALUES (";
                szCommand +=        "'" + TXT_BillCode.Text.Replace(" ", "") + "' ";
                szCommand += ", " + "'" + TXT_CustomerCode.Text.Replace(" ", "") + "' ";
                szCommand += ", " + "N'" + TXT_CustomerName.Text.Trim() + "' ";
                szCommand += ", " + "'" + RowId.Cells[GlobalConstants.COLUMN_PRODUCT_CODE].Value.ToString() + "' ";
                szCommand += ", " + "'" + RowId.Cells[GlobalConstants.COLUMN_PRODUCT_NAME].Value.ToString() + "' ";
                szCommand += ", " + "'" + RowId.Cells[GlobalConstants.COLUMN_PRODUCT_QUANTITY].Value.ToString() + "' ";
                szCommand += ", " + "'" + RowId.Cells[GlobalConstants.COLUMN_PRODUCT_UNIT].Value.ToString() + "' ";
                szCommand += ", " + "'" + double.Parse(RowId.Cells[GlobalConstants.COLUMN_PRODUCT_TOTAL_PRICE].Value.ToString()) + "' ";
                szCommand += ")";

                // Execute querry command
                SqlCommandId.CommandText = szCommand;
                SqlCommandId.ExecuteNonQuery();

                /////////////////////////////////////////////////////////////////////////////////////////////////////

                // Insert modify information 
                // Close previous SqlDataReader
                SqlDataReaderId.Close();

                // Update to Product Management
                szCommand = "UPDATE " + GlobalConstants.TABLE_PRODUCT_MANAGEMENT + " ";
                szCommand += "SET ";
                szCommand += String.Format("{0} = '{1}' ", GlobalConstants.COLUMN_PRODUCT_QUANTITY, (double)ProductInfoList[RowId.Cells[GlobalConstants.COLUMN_PRODUCT_CODE].Value.ToString()]);
                szCommand += String.Format("WHERE {0} = '{1}';", GlobalConstants.COLUMN_PRODUCT_CODE, RowId.Cells[GlobalConstants.COLUMN_PRODUCT_CODE].Value.ToString()); 

                // Update to Customer Management

                // Execute Update command
                SqlCommandId.CommandText = szCommand;
                SqlCommandId.ExecuteNonQuery();
            }

            #endregion

            #region CUSTOMER INFORMATION UPDATE

            // Update information to Customer Database
            if (bCustomerAlreadyExisted)
            {
                szCommand = "UPDATE " + GlobalConstants.TABLE_CUSTOMER_MANAGEMENT + " ";
                szCommand += "SET ";
                szCommand += String.Format("{0} = '{1}', {2} = '{3}' ", 
                                    GlobalConstants.COLUMN_CUSTOMER_BILL_QUANTITY, iCustomerBillNumber + 1,
                                    GlobalConstants.COLUMN_CUSTOMER_BALANCE, dCurrentBalance + dCustomerBalance);
                szCommand += String.Format("WHERE {0} = '{1}';", GlobalConstants.COLUMN_CUSTOMER_CODE, TXT_CustomerCode.Text);

                SqlCommandId.CommandText = szCommand;
                SqlCommandId.ExecuteNonQuery();

            }
            else if (bCustomerInfoEnough)
            {
                szCommand =  String.Format("INSERT INTO {0} ({1}, {2}, {3}, {4}) VALUES (", 
                                GlobalConstants.TABLE_CUSTOMER_MANAGEMENT,
                                GlobalConstants.COLUMN_CUSTOMER_CODE,
                                GlobalConstants.COLUMN_CUSTOMER_NAME,
                                GlobalConstants.COLUMN_CUSTOMER_BILL_QUANTITY, 
                                GlobalConstants.COLUMN_CUSTOMER_BALANCE);
                szCommand += String.Format("'{0}', N'{1}', '{2}', '{3}')", TXT_CustomerCode.Text, TXT_CustomerName.Text, iCustomerBillNumber + 1, dCurrentBalance);
                
                SqlCommandId.CommandText = szCommand;
                SqlCommandId.ExecuteNonQuery();
            }

            // Close SqlDataReader
            SqlDataReaderId.Close();

            // Close Sql Connection
            SqlConnectionId.Close();

            #endregion

            #region UPDATE PARENT TABLE
            // If parent form is valid
            // Update information to it and close this current window
            if (ParentFormId != null)
                ParentFormId.fnUpdateResultTable();

            #endregion

            // Display complete message
            MessageBox.Show("Hóa đơn đã được xuất thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Dispose();
            
        }

        #endregion

        #region TextBox Events

        // This event is fired when user enter character into Product Quantity TextField
        private void TXT_ProductQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            // More than 1 floating point ?
            if (e.KeyChar == '.' && TXT_ProductQuantity.Text.IndexOf('.') != -1)
                e.Handled = true;
            
            // Not a number, space , backspace or floating point character ?
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ' ') && (e.KeyChar != (char)8))
                e.Handled = true;
        }

        // This event is fired when user enter character into Customer Paid Money Text Field
        private void TXT_CustomerPaidMoney_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Remove spaces in this text field
            TXT_CustomerPaidMoney.Text.Replace(" ", "");

            if (e.KeyChar == '.' && (TXT_ProductCode.Text.IndexOf('.') != -1))
            {
                e.Handled = true;
                return;
            }

            // Invalid character
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != (char)8))
            {
                e.Handled = true;
                return;
            }
        }

        // This event is fired when user enter character into Product Code TextField
        private void TXT_ProductCode_TextChanged(object sender, EventArgs e)
        {
            if (!TXT_ProductCode.Enabled)
                return;

            if (TXT_ProductCode.TextLength > 0)
            {
                TXT_ProductName.Enabled = false;

                // Connect to Database
                // Create an object of SqlConnection
                SqlConnection SqlConnectionId = util_Sql.fnConnectToDataBase(UserSettings.SQL_DATASOURCE, UserSettings.SQL_CATALOG, UserSettings.SQL_AUTHENTICATION_METHOD, UserSettings.SQL_USER_NAME, UserSettings.SQL_PASSWORD);

                // Connection fail ?
                if (SqlConnectionId == null)
                    return;

                // Create an object of SqlCommand
                SqlCommand SqlCommandId = new SqlCommand();

                // Create an object of string to build querry command
                string szCommand = String.Format("SELECT {0}, {1} FROM {2} WHERE {3} = '{4}'", GlobalConstants.COLUMN_PRODUCT_NAME, GlobalConstants.COLUMN_PRODUCT_QUANTITY, GlobalConstants.TABLE_PRODUCT_MANAGEMENT, GlobalConstants.COLUMN_PRODUCT_CODE, TXT_ProductCode.Text);

                // Bind information to SqlCommandId
                SqlCommandId.CommandText = szCommand;
                SqlCommandId.Connection = SqlConnectionId;

                // Create an object of SqlDataReader
                SqlDataReader SqlDataReaderId;

                try
                {
                    SqlDataReaderId = SqlCommandId.ExecuteReader();
                }
                catch
                {
                    // Reading is fail ?

                    // Close SqlConnection
                    SqlConnectionId.Close();
                    return;
                }

                // If result is available
                // Take the first result

                while (SqlDataReaderId.Read())
                {
                    TXT_ProductName.Text = SqlDataReaderId[GlobalConstants.COLUMN_PRODUCT_NAME].ToString();
                    TXT_ProductRemain.Text = SqlDataReaderId[GlobalConstants.COLUMN_PRODUCT_QUANTITY].ToString();
                    break;

                }

                // Close SqlDataReader
                SqlDataReaderId.Close();

                // Close Sql Connection
                SqlConnectionId.Close();
                return;

            }

            if (!TXT_ProductName.Enabled)
            {
                TXT_ProductName.Enabled = true;

                // Clear Product Name Text Field
                TXT_ProductName.Text = "";
            }
        }

        // This event is fired when user enter character into Product Name TextField
        private void TXT_ProductName_TextChanged(object sender, EventArgs e)
        {
            if (!TXT_ProductName.Enabled)
                return;

            if (TXT_ProductName.TextLength > 0)
            {
                TXT_ProductCode.Enabled = false;

                // Connect to Database
                // Create an object of SqlConnection
                SqlConnection SqlConnectionId = util_Sql.fnConnectToDataBase(UserSettings.SQL_DATASOURCE, UserSettings.SQL_CATALOG, UserSettings.SQL_AUTHENTICATION_METHOD, UserSettings.SQL_USER_NAME, UserSettings.SQL_PASSWORD);

                // Connection fail ?
                if (SqlConnectionId == null)
                    return;

                // Create an object of SqlCommand
                SqlCommand SqlCommandId = new SqlCommand();

                // Create an object of string to build querry command
                string szCommand = String.Format("SELECT {0}, {1} FROM {2} WHERE {3} = '{4}'", GlobalConstants.COLUMN_PRODUCT_CODE, GlobalConstants.COLUMN_PRODUCT_QUANTITY, GlobalConstants.TABLE_PRODUCT_MANAGEMENT, GlobalConstants.COLUMN_PRODUCT_NAME, TXT_ProductName.Text);

                // Bind information to SqlCommandId
                SqlCommandId.CommandText = szCommand;
                SqlCommandId.Connection = SqlConnectionId;

                // Create an object of SqlDataReader
                SqlDataReader SqlDataReaderId;

                try
                {
                    SqlDataReaderId = SqlCommandId.ExecuteReader();
                }
                catch
                {
                    // Reading is fail ?

                    // Close SqlConnection
                    SqlConnectionId.Close();
                    return;
                }

                // If result is available
                // Take the first result

                while (SqlDataReaderId.Read())
                {
                    TXT_ProductCode.Text = SqlDataReaderId[GlobalConstants.COLUMN_PRODUCT_CODE].ToString();
                    TXT_ProductRemain.Text = SqlDataReaderId[GlobalConstants.COLUMN_PRODUCT_QUANTITY].ToString();
                    break;

                }

                // Close SqlDataReader
                SqlDataReaderId.Close();

                // Close Sql Connection
                SqlConnectionId.Close();
                return;

            }

            if (!TXT_ProductCode.Enabled)
            {
                TXT_ProductCode.Enabled = true;

                // Clear Product Code Text Field
                TXT_ProductCode.Text = "";
            }
        }

        // This event is fired when user enter character into Customer Code TextField
        private void TXT_CustomerCode_TextChanged(object sender, EventArgs e)
        {
            if (!TXT_CustomerCode.Enabled)
                return;

            if (TXT_CustomerCode.TextLength > 0)
            {
                // Connect to Database
                // Create an object of SqlConnection
                SqlConnection SqlConnectionId = util_Sql.fnConnectToDataBase(UserSettings.SQL_DATASOURCE, UserSettings.SQL_CATALOG, UserSettings.SQL_AUTHENTICATION_METHOD, UserSettings.SQL_USER_NAME, UserSettings.SQL_PASSWORD);

                // Connection fail ?
                if (SqlConnectionId == null)
                    return;

                // Create an object of SqlCommand
                SqlCommand SqlCommandId = new SqlCommand();

                // Create an object of string to build querry command
                string szCommand = String.Format("SELECT {0} FROM {1} WHERE {2} = '{3}'", GlobalConstants.COLUMN_CUSTOMER_NAME, GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, GlobalConstants.COLUMN_CUSTOMER_CODE, TXT_CustomerCode.Text);

                // Bind information to SqlCommandId
                SqlCommandId.CommandText = szCommand;
                SqlCommandId.Connection = SqlConnectionId;

                // Create an object of SqlDataReader
                SqlDataReader SqlDataReaderId;

                try
                {
                    SqlDataReaderId = SqlCommandId.ExecuteReader();
                }
                catch
                {
                    // Reading is fail ?

                    // Close SqlConnection
                    SqlConnectionId.Close();
                    return;
                }

                // If result is available
                // Take the first result

                while (SqlDataReaderId.Read())
                {
                    TXT_CustomerName.Text = SqlDataReaderId.GetString(0);

                    // Customer is valid . Disable the Customer Name Text Field
                    TXT_CustomerName.Enabled = false;

                    break;

                }

                // Close SqlDataReader
                SqlDataReaderId.Close();

                // Close Sql Connection
                SqlConnectionId.Close();
                return;

            }

            if (!TXT_CustomerName.Enabled)
            {
                TXT_CustomerName.Enabled = true;

                // Clear Product Code Text Field
                TXT_CustomerName.Text = "";
            }
        }

        // This event is fired when TXT_CustomerPaidMoney is changed
        private void TXT_CustomerPaidMoney_TextChanged(object sender, EventArgs e)
        {
            // Remove spaces 
            TXT_CustomerPaidMoney.Text.Replace(" ", "");

            double dCustomerPaidMoney = 0;

            // Purchase nothing ?
            if (dTotalBillPrice == 0)
            {
                // Clear text in Current Balance
                TXT_CurrentBalance.Text = "";
                return;
            }
            if (TXT_CustomerPaidMoney.Text.Length > 0)
            {
                try
                {
                    dCustomerPaidMoney = double.Parse(TXT_CustomerPaidMoney.Text);
                }
                catch
                {
                    // Errors happen
                    // Clear TXT_CurrentBalance
                    TXT_CurrentBalance.Text = "";
                    return;
                }
            }
            else
            {
                // Input nothing ?
                // Hide information in TXT_CurrentBalance
                TXT_CurrentBalance.Text = "";
                return;
            }
           

            // How much else customer has to pay ?
            dCustomerPaidMoney -= dTotalBillPrice;
            dCurrentBalance = dCustomerPaidMoney;

            TXT_CurrentBalance.Text = dCustomerPaidMoney.ToString("N3");
        }

        #endregion

        #region Internal Functions

        // This function help us to read all customer in Customer table
        private bool fnReadCustomer()
        {
            // Create an object of SqlConnection
            SqlConnection SqlConnectionId = new SqlConnection();

            // Create an object of SqlCommand to execute reader command
            SqlCommand SqlCommandId = new SqlCommand();

            // Connect to Database
            SqlConnectionId = util_Sql.fnConnectToDataBase(UserSettings.SQL_DATASOURCE, UserSettings.SQL_CATALOG, UserSettings.SQL_AUTHENTICATION_METHOD, UserSettings.SQL_USER_NAME, UserSettings.SQL_PASSWORD);

            // Connection is fail !
            if (SqlConnectionId == null)
                return false;

            // Store command to szCommand string
            string szCommand = "";



            // Create an object of SqlDataReader to read information from SqlDatabase
            SqlDataReader SqlDataReaderId;

            // Create an object of AutoCompleteStringCollection
            AutoCompleteStringCollection ACSC_CollectionId = new AutoCompleteStringCollection();

            // Build the reader command

            /////////////////////////////////////////////////////////////////////////////////////////////

            // Read all Customers' Code
            szCommand = String.Format("SELECT {0} FROM {1}", GlobalConstants.COLUMN_CUSTOMER_CODE, GlobalConstants.TABLE_CUSTOMER_MANAGEMENT);

            // Bind information to SqlCommand's object
            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;

            // Execute command
            try
            {
                SqlDataReaderId = SqlCommandId.ExecuteReader();
            }
            catch
            {
                // Reading failed !
                return false;
            }

            // Reading completed ?

            // Update result to AutoCompleteStringCollection
            while (SqlDataReaderId.Read())
            {
                ACSC_CollectionId.Add(SqlDataReaderId.GetString(0).Trim());
            }

            // Update Data source to Customer Code TextField suggestion
            TXT_CustomerCode.AutoCompleteCustomSource = ACSC_CollectionId;

            // Free SqlDataReader's object
            SqlDataReaderId.Close();

            // Free AutoCompleteStringSource
            ACSC_CollectionId = new AutoCompleteStringCollection();

            /////////////////////////////////////////////////////////////////////////////////////////////

            // Read all Customers' Name
            szCommand = String.Format("SELECT {0} FROM {1}", GlobalConstants.COLUMN_CUSTOMER_NAME, GlobalConstants.TABLE_CUSTOMER_MANAGEMENT);

            // Bind information to SqlCommand's object
            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;

            // Execute command
            try
            {
                SqlDataReaderId = SqlCommandId.ExecuteReader();
            }
            catch
            {
                // Reading failed !
                return false;
            }

            // Reading completed ?

            // Update result to AutoCompleteStringCollection
            while (SqlDataReaderId.Read())
                ACSC_CollectionId.Add(SqlDataReaderId.GetString(0).Trim());

            // Update Data source to Customer Name TextField suggestion
            TXT_CustomerName.AutoCompleteCustomSource = ACSC_CollectionId;

            // Free SqlDataReader's object
            SqlDataReaderId.Close();

            // Free AutoCompleteStringSource
            ACSC_CollectionId = null;

            // Close connection for safety
            SqlConnectionId.Close();
            return true;

        }

        // This function help us to read all customer in Product Table
        private bool fnReadProduct()
        {
            // Create an object of SqlConnection
            SqlConnection SqlConnectionId = new SqlConnection();

            // Create an object of SqlCommand to execute reader command
            SqlCommand SqlCommandId = new SqlCommand();

            // Connect to Database
            SqlConnectionId = util_Sql.fnConnectToDataBase(UserSettings.SQL_DATASOURCE, UserSettings.SQL_CATALOG, UserSettings.SQL_AUTHENTICATION_METHOD, UserSettings.SQL_USER_NAME, UserSettings.SQL_PASSWORD);

            // Connection is fail !
            if (SqlConnectionId == null)
                return false;

            // Store command to szCommand string
            string szCommand = "";



            // Create an object of SqlDataReader to read information from SqlDatabase
            SqlDataReader SqlDataReaderId;

            // Create an object of AutoCompleteStringCollection
            AutoCompleteStringCollection ACSC_CollectionId = new AutoCompleteStringCollection();

            // Build the reader command

            /////////////////////////////////////////////////////////////////////////////////////////////

            // Read all Customers' Code
            szCommand = String.Format("SELECT {0} FROM {1}", GlobalConstants.COLUMN_PRODUCT_CODE, GlobalConstants.TABLE_PRODUCT_MANAGEMENT);

            // Bind information to SqlCommand's object
            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;

            // Execute command
            try
            {
                SqlDataReaderId = SqlCommandId.ExecuteReader();
            }
            catch
            {
                // Reading failed !
                return false;
            }

            // Reading completed ?

            // Update result to AutoCompleteStringCollection
            while (SqlDataReaderId.Read())
                ACSC_CollectionId.Add(SqlDataReaderId[GlobalConstants.COLUMN_PRODUCT_CODE].ToString().Trim());

            // Update Data source to Customer Code TextField suggestion
            TXT_ProductCode.AutoCompleteCustomSource = ACSC_CollectionId;

            // Free SqlDataReader's object
            SqlDataReaderId.Close();

            // Free AutoCompleteStringSource
            ACSC_CollectionId = new AutoCompleteStringCollection();

            /////////////////////////////////////////////////////////////////////////////////////////////

            // Read all Customers' Name
            szCommand = String.Format("SELECT {0} FROM {1}", GlobalConstants.COLUMN_PRODUCT_NAME, GlobalConstants.TABLE_PRODUCT_MANAGEMENT);

            // Bind information to SqlCommand's object
            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;

            // Execute command
            try
            {
                SqlDataReaderId = SqlCommandId.ExecuteReader();
            }
            catch
            {
                // Reading failed !
                return false;
            }

            // Reading completed ?

            // Update result to AutoCompleteStringCollection
            while (SqlDataReaderId.Read())
                ACSC_CollectionId.Add(SqlDataReaderId.GetString(0).Trim());

            // Update Data source to Customer Name TextField suggestion
            TXT_ProductName.AutoCompleteCustomSource = ACSC_CollectionId;

            // Free SqlDataReader's object
            SqlDataReaderId.Close();

            // Free AutoCompleteStringSource
            ACSC_CollectionId = null;

            // Close connection for safety
            SqlConnectionId.Close();
            return true;

        }

        // This function helps us to create a table of Products list
        private void fnCreateTableProduct()
        {
            // Clear DataGridView
            DGV_ProductList.Columns.Clear();
            DGV_ProductList.Rows.Clear();
            DGV_ProductList.Refresh();

            // Re-create product column list
            string[] szColumnsList = { GlobalConstants.COLUMN_PRODUCT_CODE, GlobalConstants.COLUMN_PRODUCT_NAME, GlobalConstants.COLUMN_PRODUCT_QUANTITY, GlobalConstants.COLUMN_PRODUCT_UNIT, GlobalConstants.COLUMN_PRODUCT_PRICE, GlobalConstants.COLUMN_PRODUCT_TOTAL_PRICE };

            foreach (string szColumnInfo in szColumnsList)
            {
                DGV_ProductList.Columns.Add(szColumnInfo, szColumnInfo);
                DGV_ProductList.Columns[szColumnInfo].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        #endregion
    }
}
