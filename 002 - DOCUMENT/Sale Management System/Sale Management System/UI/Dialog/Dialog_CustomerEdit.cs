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
    public partial class SForm_CustomerEdit : Form
    {
        // Store the Id of its parent form
        public Form_CustomerManagement ParentFormId;

        // Store information of a Customer to fill in all missing fields
        public Customer CustomerId;

        #region Constructors

        public SForm_CustomerEdit()
        {
            InitializeComponent();

            // Set its parent form to null
            // In case we update information to wrong forms
            ParentFormId = null;


            // Reset info to null incase we update information to wrong customer 
            ParentFormId = null;
            CustomerId = null;

        }

        #endregion

        #region Buttons Events

        // This event is fired when Cornfirmation button is clicked
        private void BTN_CustomerInfoSave_Click(object sender, EventArgs e)
        {
            // If they didn't fill anything in Customer Code text field
            TXT_CustomerCode.Text.Replace(" ", "");

            if (TXT_CustomerCode.Text.Length < 1)
            {
                // Display warning message
                
                MessageBox.Show("Mã khách hàng phải được điền đầy đủ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Focus on Product code text field
                TXT_CustomerCode.Focus();

                return;
            }

            // If they didn't fill anything in Customer Name text field
            TXT_CustomerName.Text.Trim();

            if (TXT_CustomerName.Text.Length < 1)
            {
                // Display warning message
                MessageBox.Show("Tên khách hàng phải được điền đầy đủ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Focus on Product name text field
                TXT_CustomerName.Focus();

                return;
            }


            // If they didn't fill anything in Customer balance TextField
            TXT_CustomerBalance.Text.Trim();

            // Store information of price
            float flCustomerBalance;

            if (TXT_CustomerBalance.Text.Length < 1)
            {
                // Display warning message
                MessageBox.Show("Số tiền khách trả phải được điền đầy đủ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Focus on Product name text field
                TXT_CustomerBalance.Focus();

                return;
            }

            try
            {
                // Convert string of price to float number
                flCustomerBalance = float.Parse(TXT_CustomerBalance.Text);
            }
            catch
            {
                // If an error happens
                // Display error diaglog and make user input again
                MessageBox.Show("Dữ liệu không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Focus on Product price
                TXT_CustomerBalance.Focus();

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
            // Querry if Product Code already existed
            // Command : "SELECT * FROM " + TABLE_NAME + " WHERE COLUMN_PRODUCT_CODE = '{PRODUCT_CODE}'";
            szCommand = String.Format("SELECT * FROM {0} WHERE {1} = '{2}'",
                                        GlobalConstants.TABLE_PRODUCT_MANAGEMENT,
                                        GlobalConstants.COLUMN_PRODUCT_CODE,
                                        TXT_CustomerCode.Text);

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

            // If the result sent back has one or more row ?
            // Cancel adding
            if (SqlDataReaderId == null || (SqlDataReaderId.HasRows && !TXT_CustomerCode.Text.Equals(CustomerId.CODE)))
            {
                // Free SqlReader for safety
                SqlDataReaderId.Close();

                // Close connection for safety
                SqlConnectionId.Close();

                // Display error dialog
                MessageBox.Show("Mã khách hàng đã tồn tại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Close SqlDataReader
            SqlDataReaderId.Close();
            //////////////////////////////////////////////////////////////////////////
            szCommand = "";
            szCommand += "UPDATE " + GlobalConstants.TABLE_CUSTOMER_MANAGEMENT + " ";
            szCommand += "SET ";
            szCommand += GlobalConstants.COLUMN_CUSTOMER_CODE + " = '" + TXT_CustomerCode.Text + "' ";
            szCommand += ", " + GlobalConstants.COLUMN_CUSTOMER_NAME + " = N'" + TXT_CustomerName.Text + "' ";
            szCommand += ", " + GlobalConstants.COLUMN_CUSTOMER_AGE + " = '" + NUD_CustomerAge.Value.ToString() + "' ";
            szCommand += ", " + GlobalConstants.COLUMN_CUSTOMER_BILL_QUANTITY + " = '" + NUD_CustomerAge.Value.ToString() + "' ";
            szCommand += ", " + GlobalConstants.COLUMN_CUSTOMER_BALANCE + " = '" + TXT_CustomerBalance.Text + "' ";
            szCommand += ", " + GlobalConstants.COLUMN_CUSTOMER_EMAIL + " = '" + TXT_CustomerEmail.Text + "' ";
            szCommand += ", " + GlobalConstants.COLUMN_CUSTOMER_ADDRESS + " = N'" + TXT_CustomerAddress.Text + "' ";
            szCommand += ", " + GlobalConstants.COLUMN_NOTE + " = N'" + RTB_CustomerNote.Text + "' ";
            szCommand += "WHERE " + GlobalConstants.COLUMN_CUSTOMER_CODE + " = '" + CustomerId.CODE + "'";

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
                return;
            }

            // Adding a product is success ?
            // Display a message box to let user know
            MessageBox.Show("Thông tin được chỉnh sửa thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // If this form's parent is available ?
            // Update it new information
            if (ParentFormId != null)
                ParentFormId.fnUpdateResultTable();

            // Close SqlConnection
            SqlConnectionId.Close();

            //////////////////////////////////////////////////////////////////////////
        }

        // This event is fired when user press Clear button to clear all information from all TextFields
        private void BTN_CustomerInfoClear_Click(object sender, EventArgs e)
        {
            // TextFields
            TXT_CustomerAddress.Text = "";
            TXT_CustomerBalance.Text = "";
            TXT_CustomerName.Text = "";

            // Rich Text Box
            RTB_CustomerNote.Text = "";

            // Numeric Up Down
            NUD_CustomerAge.Value = NUD_CustomerAge.Minimum;
            NUD_CustomerBillQuantity.Value = NUD_CustomerBillQuantity.Minimum;

            // Focus on Customer Code
            TXT_CustomerCode.Focus();
        }

        #endregion

        #region TextBox Events

        // This event is fired when user input date to Customer Code TextField
        private void TXT_CustomerCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If user press Space button
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
        }

        // This event is fired when user input data to Customer balance TextField
        private void TXT_CustomerBalance_KeyPress(object sender, KeyPressEventArgs e)
        {
            char cCharacter = e.KeyChar;

            // More than 1 floating point
            if (cCharacter == '.' && TXT_CustomerBalance.Text.IndexOf('.') != -1)
            {
                e.Handled = true;
                return;
            }

            // More than 1 minus point 
            if (cCharacter == '-' && TXT_CustomerBalance.Text.IndexOf('-') != -1)
            {
                e.Handled = true;
                return;
            }

            if ((!char.IsDigit(cCharacter)) && (cCharacter != '-') && (cCharacter != '.') && (cCharacter != 8))
            {
                e.Handled = true;
                return;
            }
        }

        #endregion

        #region Interal Functions

        // This function helps us to fill information to all missing fields
        public void fnFillInformation()
        {
            if (CustomerId == null)
                return;

            // Text Fields
            TXT_CustomerAddress.Text = CustomerId.ADDRESS;
            TXT_CustomerBalance.Text = CustomerId.BALANCE.ToString();
            TXT_CustomerCode.Text = CustomerId.CODE;
            TXT_CustomerName.Text = CustomerId.NAME;
            TXT_CustomerEmail.Text = CustomerId.EMAIL;

            // Numeric up down
            NUD_CustomerAge.Value = (decimal)CustomerId.AGE;
            NUD_CustomerBillQuantity.Value = (decimal)CustomerId.BILL_QUANTITY;

            // Rich Text Box
            RTB_CustomerNote.Text = CustomerId.NOTE;
        }

        #endregion
    }
}
