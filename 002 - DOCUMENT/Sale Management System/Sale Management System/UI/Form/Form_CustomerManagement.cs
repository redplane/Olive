using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Resources;
using System.Data.SqlClient;

namespace Sale_Management_System
{
    public partial class Form_CustomerManagement : Form
    {
        #region Constructor

        public Form_CustomerManagement()
        {
            InitializeComponent();

            // Update data grid view
            fnUpdateResultTable();

        }

        #endregion

        /**********************************************************************************/

        #region Button Events

        // Buttons
        // This event is fired when Back button is clicked
        private void BTN_Back_Click(object sender, EventArgs e)
        {
            // Turn back to Main Menu
            Form_MainMenu FormId = new Form_MainMenu();

            // Hide this windows
            this.Dispose();

            // Display it !
            FormId.Show();
        }

        // This event is fired when user click Add customer
        private void BTN_AddCustomer_Click(object sender, EventArgs e)
        {
            // Create an object of SFrom_CustomerAdd
            Dialog_CustomerAdd FormId = new Dialog_CustomerAdd();

            // Make its parent form to be this form
            FormId.ParentFormId = this;

            // Show it to the screen
            FormId.ShowDialog();
        }

        // This event is fired when user press Delete button to delete a Customer from the database
        private void BTN_DeleteCustomer_Click(object sender, EventArgs e)
        {
            fnDeleteCustomerInfo();
        }

        // This event is fired when user click Edit button to edit information of a customer
        private void BTN_EditCustomer_Click(object sender, EventArgs e)
        {
            // No row is selected
            if (DGV_CustomerInfo.SelectedRows.Count < 1)
            {
                // Display warning message
                MessageBox.Show("Bạn phải chọn một dòng", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }
            foreach (DataGridViewRow RowId in DGV_CustomerInfo.SelectedRows)
            {
                // Create an object of Customer to store information to it
                Customer CustomerId = new Customer();

                // Store information
                CustomerId.CODE = RowId.Cells[GlobalConstants.COLUMN_CUSTOMER_CODE].Value.ToString().Trim();
                CustomerId.NAME = RowId.Cells[GlobalConstants.COLUMN_CUSTOMER_NAME].Value.ToString().Trim();
                CustomerId.AGE = int.Parse(RowId.Cells[GlobalConstants.COLUMN_CUSTOMER_AGE].Value.ToString().Replace(" ", ""));
                CustomerId.BILL_QUANTITY = int.Parse(RowId.Cells[GlobalConstants.COLUMN_CUSTOMER_BILL_QUANTITY].Value.ToString().Replace(" ", ""));
                CustomerId.BALANCE = float.Parse(RowId.Cells[GlobalConstants.COLUMN_CUSTOMER_BALANCE].Value.ToString().Replace(" ", ""));
                CustomerId.EMAIL = RowId.Cells[GlobalConstants.COLUMN_CUSTOMER_EMAIL].Value.ToString();
                CustomerId.ADDRESS = RowId.Cells[GlobalConstants.COLUMN_CUSTOMER_ADDRESS].Value.ToString().Trim();
                CustomerId.NOTE = RowId.Cells[GlobalConstants.COLUMN_NOTE].Value.ToString().Trim();


                // Create an object of SForm_PoductEdit
                SForm_CustomerEdit FormId = new SForm_CustomerEdit();

                // Set information to Edit form
                FormId.CustomerId = CustomerId;

                // Update to Product edit form
                FormId.fnFillInformation();

                // Set its parent form to this form
                FormId.ParentFormId = this;

                // Display Edit form
                FormId.ShowDialog();


            }
        }

        #endregion

        #region TextBox Events

        // This event is fired when user input Text to Text Field
        private void TXT_TextChanged(object sender, EventArgs e)
        {
            fnUpdateResultTable();
        }

        #endregion

        #region Rich TextBox Event

        // This event is fired when user click a radio button
        private void RBN_Click(object sender, EventArgs e)
        {
            fnUpdateResultTable();
        }

        #endregion

        #region Form Events

        // This event is fired when this form has been closed
        private void Event_OnFormClosed(object sender, FormClosedEventArgs e)
        {
            // Terminate the whole program
            Application.Exit();
        }

        // This event is fired when this form is being closed
        private void Event_OnFormClosing(object sender, FormClosingEventArgs e)
        {
            // Display confirmation message
            if (MessageBox.Show("Bạn muốn thoát chương trình ?", "Cảnh báo", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                e.Cancel = true;
        }

        #endregion

        #region DataGridView Events

        // This event is fired when user press Delete key after he/she selected a row
        private void DGV_CustomerInfo_KeyDown(object sender, KeyEventArgs e)
        {
            // If the pressed key is not Delete key
            if (e.KeyCode != Keys.Delete)
                return;

            fnDeleteCustomerInfo();
        }

        #endregion

        /**********************************************************************************/

        #region Internal Functions

        // This function help us to delete a customer from database
        private void fnDeleteCustomerInfo()
        {
            // No row is selected
            if (DGV_CustomerInfo.SelectedRows.Count < 1)
            {
                // Display warning message
                MessageBox.Show("Bạn phải chọn một dòng", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create connection to database
            SqlConnection SqlConnectionId = util_Sql.fnConnectToDataBase(UserSettings.SQL_DATASOURCE, UserSettings.SQL_CATALOG, UserSettings.SQL_AUTHENTICATION_METHOD, UserSettings.SQL_USER_NAME, UserSettings.SQL_PASSWORD);

            if (SqlConnectionId == null)
            {
                MessageBox.Show("Máy chủ dữ liệu không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // User chose NO
            if (MessageBox.Show("Bạn có muốn xóa dữ liệu ?", "Cảnh báo", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            // Create an object of SqlCommand to execute delete command
            SqlCommand SqlCommandId = new SqlCommand();

            // Create command line

            foreach (DataGridViewRow RowId in DGV_CustomerInfo.SelectedRows)
            {


                string szSqlCommand = "DELETE FROM " + GlobalConstants.TABLE_CUSTOMER_MANAGEMENT +
                                    " WHERE " + GlobalConstants.COLUMN_CUSTOMER_CODE +
                                    " = ";

                szSqlCommand += "'";
                szSqlCommand += RowId.Cells[GlobalConstants.COLUMN_CUSTOMER_CODE].Value;
                szSqlCommand += "'";

                DGV_CustomerInfo.Rows.Remove(RowId);

                // Update information to SqlCommand Object 
                SqlCommandId.CommandText = szSqlCommand;
                SqlCommandId.Connection = SqlConnectionId;

                // Execute delete command
                SqlCommandId.ExecuteNonQuery();
            }

            // Close connection for safety
            SqlConnectionId.Close();

            // Update Customer information table
            fnUpdateResultTable();
        }

        // Update the Data Grid View
        public void fnUpdateResultTable()
        {
            // Clear Customer data grid view
            DGV_CustomerInfo.Columns.Clear();
            DGV_CustomerInfo.Rows.Clear();
            DGV_CustomerInfo.Refresh();

            // Not valid Data Source
            if (UserSettings.SQL_DATASOURCE.Length < 1 || UserSettings.SQL_DATASOURCE == null)
            {
                MessageBox.Show("Máy chủ không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Not valid Catalog
            if (UserSettings.SQL_CATALOG.Length < 1 || UserSettings.SQL_CATALOG == null)
            {
                MessageBox.Show("Cơ sở dữ liệu không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // SQL Connection object
            SqlConnection SqlConnectionId = util_Sql.fnConnectToDataBase(UserSettings.SQL_DATASOURCE, UserSettings.SQL_CATALOG, UserSettings.SQL_AUTHENTICATION_METHOD, UserSettings.SQL_USER_NAME, UserSettings.SQL_PASSWORD);

            if (SqlConnectionId == null)
            {
                MessageBox.Show("Kết nối CSDL bị lỗi", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create an object of SqlCommand to store information of querry command
            SqlCommand SqlCommandId = new SqlCommand();

            // Build the querry command
            string szCommand = "SELECT * FROM " + GlobalConstants.TABLE_CUSTOMER_MANAGEMENT + " ";

            // We use this flag to make sure that there are 2 more querries at a time to prevent using WHERE many times
            bool bHasPrefix = false;

            // If the Code Filter TextField is valid
            if (TXT_CustomerCode.Text != null && TXT_CustomerCode.Text.Length > 0)
            {
                bHasPrefix = true;
                szCommand += "WHERE ";
                szCommand += GlobalConstants.COLUMN_CUSTOMER_CODE + " LIKE '%" + TXT_CustomerCode.Text + "%' ";
            }

            // If the Name Filter TextField is valid
            if (TXT_CustomerName.Text != null && TXT_CustomerName.Text.Length > 0)
            {
                if (bHasPrefix)
                    szCommand += "AND ";
                else
                {
                    szCommand += "WHERE ";
                    bHasPrefix = true;
                }

                szCommand += GlobalConstants.COLUMN_CUSTOMER_NAME + " LIKE N'%" + TXT_CustomerName.Text + "%' ";
            }

            // Attach this command to SQL Command object
            SqlCommandId.CommandText = szCommand;

            // Bind SQL Connection ID to SQL Command ID
            SqlCommandId.Connection = SqlConnectionId;

            // Create an object of SqlReader to read querry data
            SqlDataReader SqlDataReaderId;

            // Receive querry information
            SqlDataReaderId = SqlCommandId.ExecuteReader();

            // If the result we received doesnt have any row ?
            if (!SqlDataReaderId.HasRows)
                return;

            // Create object of BindingSource
            BindingSource BindingSourceId = new BindingSource();

            // Bind all information read from SQL Table to BindingSourceId
            BindingSourceId.DataSource = SqlDataReaderId;

            // Bring read database to DataGridView
            DGV_CustomerInfo.DataSource = BindingSourceId;

            DGV_CustomerInfo.Columns[GlobalConstants.COLUMN_CUSTOMER_BALANCE].DefaultCellStyle.Format = "N3";


            DGV_CustomerInfo.Columns[GlobalConstants.COLUMN_CUSTOMER_CODE].HeaderText = "Mã khách hàng";
            DGV_CustomerInfo.Columns[GlobalConstants.COLUMN_CUSTOMER_NAME].HeaderText = "Tên khách hàng";
            DGV_CustomerInfo.Columns[GlobalConstants.COLUMN_CUSTOMER_AGE].HeaderText = "Tuổi khách hàng";
            DGV_CustomerInfo.Columns[GlobalConstants.COLUMN_CUSTOMER_BILL_QUANTITY].HeaderText = "Số lượng hóa đơn";
            DGV_CustomerInfo.Columns[GlobalConstants.COLUMN_CUSTOMER_BALANCE].HeaderText = "Dư nợ";
            DGV_CustomerInfo.Columns[GlobalConstants.COLUMN_CUSTOMER_EMAIL].HeaderText = "Email";
            DGV_CustomerInfo.Columns[GlobalConstants.COLUMN_CUSTOMER_ADDRESS].HeaderText = "Địa chỉ";
            DGV_CustomerInfo.Columns[GlobalConstants.COLUMN_NOTE].HeaderText = "Ghi chú";

            // Update data grid view table
            DGV_CustomerInfo.Refresh();

            // Close SQL Connection and SQL Reader for safety
            SqlDataReaderId.Close();
            SqlConnectionId.Close();
        }

        #endregion
    }
}
