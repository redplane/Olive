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

namespace Sale_Management_System
{
    public partial class Dialog_ProductEdit : Form
    {
        // Store information of a product
        public Product ProductId = null;

        // Store ID of parent form
        public Form_ProductManagement ParentFormId = null;

        #region Constructor

        public Dialog_ProductEdit()
        {
            InitializeComponent();


            // Reset product information to null
            // To prevent we edit wrong product
            ProductId = null;

            // Set parent form to null
            ParentFormId = null;

        }

        #endregion

        #region TextBox Events

        // This event is fired when user enter a key in Product quantity text field
        private void TXT_ProductQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            char cCharacter = e.KeyChar;

            // More than 1 floating point
            if (cCharacter == '.' && TXT_ProductQuantity.Text.IndexOf('.') != -1)
            {
                e.Handled = true;
                return;
            }

            if ((!char.IsDigit(cCharacter)) && (cCharacter != '.') && (cCharacter != 8))
            {
                e.Handled = true;
                return;
            }
        }

        // This event is fired when user enter a key in Product price text field
        private void TXT_ProductPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            char cCharacter = e.KeyChar;

            // More than 1 floating point
            if (cCharacter == '.' && TXT_ProductPrice.Text.IndexOf('.') != -1)
            {
                e.Handled = true;
                return;
            }

            if ((!char.IsDigit(cCharacter)) && (cCharacter != '.') && (cCharacter != 8))
            {
                e.Handled = true;
                return;
            }
        }

        #endregion

        #region Button Events

        // This event is fired when user click Save button to confirm his/her product edition
        private void BTN_ProductSave_Click(object sender, EventArgs e)
        {
            
            // If they didn't fill anything inProduct code text field
            TXT_ProductCode.Text.Trim();

            if (TXT_ProductCode.Text.Length < 1)
            {
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
                // Display warning message
                MessageBox.Show("Tên sản phẩm phải được điền đầy đủ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Focus on Product name text field
                TXT_ProductName.Focus();

                return;
            }

            // If they didn't fill anything in product quantity
            TXT_ProductQuantity.Text.Trim();

            // Store information of product quantity
            float flProductQuantity;

            if (TXT_ProductQuantity.Text.Length < 1)
            {
                // Display warning message
                MessageBox.Show("Số lượng sản phẩm phải được điền đầy đủ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Focus on Product name text field
                TXT_ProductQuantity.Focus();

                return;
            }

            try
            {
                // Convert string of price to float number
                flProductQuantity = float.Parse(TXT_ProductQuantity.Text);
            }
            catch
            {
                // If an error happens
                // Display error diaglog and make user input again
                MessageBox.Show("Dữ liệu không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Focus on Product price
                TXT_ProductQuantity.Focus();

                return;
            }

            // If they didn't fill anything in product price
            TXT_ProductPrice.Text.Trim();

            // Store information of price
            float flProductPrice;

            if (TXT_ProductPrice.Text.Length < 1)
            {
                // Display warning message
                MessageBox.Show("Giá sản phẩm phải được điền đầy đủ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Focus on Product name text field
                TXT_ProductPrice.Focus();

                return;
            }


            try
            {
                // Convert string of price to float number
                flProductPrice = float.Parse(TXT_ProductPrice.Text);
            }
            catch
            {
                // If an error happens
                // Display error diaglog and make user input again
                MessageBox.Show("Dữ liệu không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Focus on Product price
                TXT_ProductPrice.Focus();

                return;
            }

            // Trim Product Unit
            TXT_ProductUnit.Text.Trim();

            // Unfilled product unit?
            if (TXT_ProductUnit.Text.Length < 1)
            {
                // Focus on TXT_ProductUnit
                TXT_ProductUnit.Focus();

                // Display warning message
                MessageBox.Show("Chưa điền đơn vị sản phẩm", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            // Create SqlConnection to Databas
            SqlConnection SqlConnectionId = util_Sql.fnConnectToDataBase(UserSettings.SQL_DATASOURCE, UserSettings.SQL_CATALOG, UserSettings.SQL_AUTHENTICATION_METHOD, UserSettings.SQL_USER_NAME, UserSettings.SQL_PASSWORD);

            // Connection is fail
            if (SqlConnectionId == null || SqlConnectionId.State == ConnectionState.Broken || SqlConnectionId.State == ConnectionState.Closed)
            {
                // Display an error message box
                MessageBox.Show("Kết nối dữ liệu không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create a command string to store querry command information
            string szCommand;

            // Create an object of SqlDataReader to check if Product code already existed
            SqlDataReader SqlDataReaderId;

            // Create an object of SqlCommand to execute Sql Commands
            SqlCommand SqlCommandId = new SqlCommand();

            //////////////////////////////////////////////////////////////////////////
            // Querry if new product code already existed
            // Command : "SELECT * FROM " + TABLE_NAME + " WHERE COLUMN_PRODUCT_CODE = '{PRODUCT_CODE}'";
            szCommand = String.Format("SELECT * FROM {0} WHERE {1} = '{2}'",
                                        GlobalConstants.TABLE_PRODUCT_MANAGEMENT,
                                        GlobalConstants.COLUMN_PRODUCT_CODE,
                                        ProductId.CODE);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;

            // Execute reader command
            try
            {
                SqlDataReaderId = SqlCommandId.ExecuteReader();
            }
            catch
            {
                SqlDataReaderId = null;
            }

            // If the result sent back has more than one row?
            // Cancel editing
            if (SqlDataReaderId == null || (SqlDataReaderId.HasRows && !TXT_ProductCode.Text.Equals(ProductId.CODE)))
            {
                // Free SqlReader for safety
                SqlDataReaderId.Close();

                // Close connection for safety
                SqlConnectionId.Close();

                // Display error dialog
                MessageBox.Show("Mã sản phẩm đã tồn tại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Dispose this Dialog
                this.Dispose();

                return;
            }

            // Close SqlDataReader
            SqlDataReaderId.Close();

            //////////////////////////////////////////////////////////////////////////
            // Product is already in Database
            // Update new data to it
            // Command :   "UPDATE {TABLE_NAME} 
            //              SET CODE = '' , 
            //              NAME = '', 
            //              QUANTITY = '',
            //              PRICE = '',
            //              NOTE = '',
            //              WHERE CODE = ''  
              
            szCommand = "";
            szCommand += "UPDATE " + GlobalConstants.TABLE_PRODUCT_MANAGEMENT + " ";
            szCommand += "SET ";
            szCommand += GlobalConstants.COLUMN_PRODUCT_CODE + " = '" + TXT_ProductCode.Text + "' ";
            szCommand += ", " + GlobalConstants.COLUMN_PRODUCT_NAME + " = N'" + TXT_ProductName.Text + "' ";
            szCommand += ", " + GlobalConstants.COLUMN_PRODUCT_QUANTITY + " = '" + TXT_ProductQuantity.Text + "' ";
            szCommand += ", " + GlobalConstants.COLUMN_PRODUCT_PRICE + " = '" + TXT_ProductPrice.Text + "' ";
            szCommand += ", " + GlobalConstants.COLUMN_PRODUCT_UNIT + " = N'" + TXT_ProductUnit.Text.Trim() + "' ";
            szCommand += ", " + GlobalConstants.COLUMN_NOTE + " = N'" + RTB_ProductNote.Text + "' ";
            szCommand += "WHERE " + GlobalConstants.COLUMN_PRODUCT_CODE + " = '" + ProductId.CODE + "'";

            // Start adding product information to Database
            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;

            try
            {
                SqlCommandId.ExecuteNonQuery();
            }
            catch
            {
                // An eror occurs ?
                // Close connection
                SqlConnectionId.Close();

                // Display error message
                MessageBox.Show("Lỗi không xác định", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // End adding
                this.Dispose();

                return;
            }

            // Adding a product is success ?
            // Display a message box to let user know
            MessageBox.Show("Đã sửa thông tin sản phẩm");

            // If this form's parent is available ?
            // Update it new information
            if (ParentFormId != null)
                ParentFormId.fnUpdateResultTable();

            // Close SqlConnection
            SqlConnectionId.Close();

            // Dispose current form
            this.Dispose();

            //////////////////////////////////////////////////////////////////////////
        }

        #endregion

        #region Internal Functions


        // This function help us to fill information to Product edit form
        public void fnFillInformation()
        {
            if (ProductId == null)
                return;

            // Text Field
            TXT_ProductCode.Text = ProductId.CODE;
            TXT_ProductName.Text = ProductId.NAME;
            TXT_ProductQuantity.Text = ProductId.QUANTITY.ToString();
            TXT_ProductPrice.Text = ProductId.PRICE.ToString();
            TXT_ProductUnit.Text = ProductId.UNIT;
            RTB_ProductNote.Text = ProductId.NOTE;

        }

        #endregion
    }
}
