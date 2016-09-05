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
    public partial class Dialog_CustomerAdd : Form
    {
        // Store the Id of its parent form
        public Form_CustomerManagement ParentFormId;

        #region Constructor

        public Dialog_CustomerAdd()
        {
            InitializeComponent();

            // Set its parent form to null
            // In case we update information to wrong forms
            ParentFormId = null;

        }

        #endregion

        #region Button Events

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

            // Create an object of SqlCommand to execute Sql Commands
            SqlCommand SqlCommandId = new SqlCommand();

            ///////////////////////////////////////////////////////////////////
            if (util_Sql.fnIsInDatabase(SqlConnectionId, GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, GlobalConstants.COLUMN_CUSTOMER_CODE, TXT_CustomerCode.Text))
            {
                // Close connection for safety
                SqlConnectionId.Close();

                // Display error dialog
                MessageBox.Show("Mã khách hàng đã tồn tại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //////////////////////////////////////////////////////////////////////////
            // Product code doesn't exist before ?
            // Insert it to Database
            // Command :  "INSERT INTO {TABLE_NAME} (CODE, NAME, QUANTITY, PRICE, NOTE) VALUES (..., ..., ..., ..., ...)
            
            szCommand = String.Format("INSERT INTO {0} ({1}, {2}, {3}, {4}, {5} , {6}, {7}, {8} ) VALUES ('{9}', N'{10}', '{11}', '{12}', '{13}', '{14}', N'{15}', N'{16}')",
                // COLUMNS NAME
                                        GlobalConstants.TABLE_CUSTOMER_MANAGEMENT,
                                        GlobalConstants.COLUMN_CUSTOMER_CODE,
                                        GlobalConstants.COLUMN_CUSTOMER_NAME,
                                        GlobalConstants.COLUMN_CUSTOMER_AGE,
                                        GlobalConstants.COLUMN_CUSTOMER_BILL_QUANTITY,
                                        GlobalConstants.COLUMN_CUSTOMER_BALANCE,
                                        GlobalConstants.COLUMN_CUSTOMER_EMAIL,
                                        GlobalConstants.COLUMN_CUSTOMER_ADDRESS,
                                        GlobalConstants.COLUMN_NOTE,
                // THEIR VALUES
                                        TXT_CustomerCode.Text,
                                        TXT_CustomerName.Text,
                                        (int)NUD_CustomerAge.Value,
                                        (int)NUD_CustomerBillQuantity.Value,
                                        flCustomerBalance,
                                        TXT_CustomerEmail.Text,
                                        TXT_CustomerAddress.Text,
                                        RTB_CustomerNote.Text);
            
            // Start adding customer information to Database
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
                //MessageBox.Show(EX.ToString());
                // End adding
                return;
            }

            // Adding a product is success ?
            // Display a message box to let user know
            MessageBox.Show("Đã thêm khách hàng thành công");

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
            TXT_CustomerCode.Text = "";
            TXT_CustomerName.Text = "";

            // Rich Text Box
            RTB_CustomerNote.Text = "";

            // Numeric Up Down
            NUD_CustomerAge.Value = NUD_CustomerAge.Minimum;
            NUD_CustomerBillQuantity.Value = NUD_CustomerBillQuantity.Minimum;

            // Focus on Customer Code
            TXT_CustomerCode.Focus();
        }

        // This event is fired when user clicks Generate button to automatically generate a code string for a customer
        private void BTN_CustomerCodeCreate_Click(object sender, EventArgs e)
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

            for (; iTriedTime < 5; iTriedTime ++)
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

                    // Terminate function
                    return;
                }
            }

            // Close Sql Connection
            SqlConnectionId.Close();


            // Set user to focus on Customer code text field
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

    }
}
