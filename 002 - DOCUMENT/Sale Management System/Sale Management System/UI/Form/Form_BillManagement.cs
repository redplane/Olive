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
    public partial class Form_BillManagement : Form
    {
        private DataGridViewRow DRVR_CurrentRow = null;

        #region Constructor

        public Form_BillManagement()
        {
            InitializeComponent();

            // Set date time picker to current time (today)
            DTP_FromDate.Value = DateTime.Today;
            DTP_EndDate.Value = DateTime.Today;

            // Update bill list
            fnUpdateResultTable();

        }

        #endregion

        #region Button Events

        // Fired when Back button is pressed
        private void BTN_Back_Click(object sender, EventArgs e)
        {
                // Create object of Main Menu
                Form_MainMenu FormId = new Form_MainMenu();

                // Hide this Menu Context
                this.Dispose();

                // Display Main Menu
                FormId.Show();
        }

        // Fired when Add bill button is pressed
        private void BTN_AddBill_Click(object sender, EventArgs e)
        {
            // Create an object of SForm_BillAdd
            Dialog_BillAdd FormId = new Dialog_BillAdd();

            // Set its parent is this window
            FormId.ParentFormId = this;
            // Display it to screen
            FormId.ShowDialog();
        }
        
        #endregion

        #region TextBox Events

        // This event is fired when user input any characters to Text Fields
        private void TextBox_OnTextChanged(object sender, EventArgs e)
        {
            // Filter Bill Management Table
            fnUpdateResultTable();
        }

        #endregion

        #region Form Events

        // This event is fired when this form has been closed
        private void Event_OnFormClosed(object sender, FormClosedEventArgs e)
        {
            // Terminate program
            Application.Exit();
        }

        // This event is fired when this form is being closed
        private void Event_OnFormClosing(object sender, FormClosingEventArgs e)
        {
            // Display confirmation message
            if (MessageBox.Show("Bạn có muốn đóng cửa sổ ?", "Cảnh báo", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                e.Cancel = true;
        }

        #endregion

        #region CheckBox Events

        // This event is fired when Check Box is changed its state
        private void CHX_FilterByDate_CheckStateChanged(object sender, EventArgs e)
        {
            if (CHX_FilterByDate.CheckState != CheckState.Checked)
            {
                // Disable DateTimePicker objects
                DTP_FromDate.Enabled = false;
                DTP_EndDate.Enabled = false;
            }
            else
            {
                // Enable DateTimePicker objects
                DTP_FromDate.Enabled = true;
                DTP_EndDate.Enabled = true;
            }
            // Filter Bill Management Table
            fnUpdateResultTable();
        }

        #endregion

        #region DateTimePicker Events

        // This event is fired when user choose the started date
        private void DTP_FromDate_ValueChanged(object sender, EventArgs e)
        {
            if (CHX_FilterByDate.CheckState != CheckState.Checked)
                return;

            // Update result table
            fnUpdateResultTable();
        }

        // This event is fired when user choose the finished date
        private void DTP_EndDate_ValueChanged(object sender, EventArgs e)
        {
            if (CHX_FilterByDate.CheckState != CheckState.Checked)
                return;

            // Update result table
            fnUpdateResultTable();
        }

        #endregion

        #region DataGridView Events

        // This event is fired when user select a row in DataGridView
        private void DGV_BillInfo_SelectionChanged(object sender, EventArgs e)
        {
            // Select more or less than 1 row ?
            if (DGV_BillInfo.SelectedRows.Count != 1)
            {
                // Clear DataGridView
                DGV_ProductList.Rows.Clear();
                DGV_ProductList.Refresh();
                return;
            }

            // Select a row more than 1 time?
            if (DGV_BillInfo.CurrentRow == DRVR_CurrentRow)
                return;

            // Clear Data Grid View


            // Update current row ID
            DRVR_CurrentRow = DGV_BillInfo.CurrentRow;
            
            // Open Sql Connection
            SqlConnection SqlConnectionId = util_Sql.fnConnectToDataBase(UserSettings.SQL_DATASOURCE, UserSettings.SQL_CATALOG, UserSettings.SQL_AUTHENTICATION_METHOD, UserSettings.SQL_USER_NAME, UserSettings.SQL_PASSWORD);

            // Connection is fail ?
            if (SqlConnectionId == null)
                return;
            
            // Create an object of SqlCommand to Execute querry commands
            SqlCommand SqlCommanId = new SqlCommand();

            // Create an object of string to build querry command
            string szCommand;

            // Build querry command
            szCommand = String.Format("SELECT * FROM {0} WHERE {1} = '{2}'",
                                        GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED,
                                        GlobalConstants.COLUMN_BILL_CODE,
                                        DRVR_CurrentRow.Cells[GlobalConstants.COLUMN_BILL_CODE].Value.ToString());

            // Bind connection and command text to SqlCommand
            SqlCommanId.CommandText = szCommand;
            SqlCommanId.Connection = SqlConnectionId;

            // Execute command and store it to SqlDataReader
            SqlDataReader SqlDataReaderId;

            try
            {
                SqlDataReaderId = SqlCommanId.ExecuteReader();
            }
            catch
            {
                return;
            }

            // Reading successfully
            // The returned result is blank ?
            if (!SqlDataReaderId.HasRows)
            {
                // Close SqlDataReader and SqlConnection
                SqlDataReaderId.Close();
                SqlConnectionId.Close();

                return;
            }

            
            // Clear rows
            DGV_ProductList.Rows.Clear();

            // Read returned result
            while (SqlDataReaderId.Read())
            {
                DGV_ProductList.Rows.Add(SqlDataReaderId[GlobalConstants.COLUMN_PRODUCT_CODE].ToString().Trim(),
                                         SqlDataReaderId[GlobalConstants.COLUMN_PRODUCT_NAME].ToString().Trim(),
                                         SqlDataReaderId[GlobalConstants.COLUMN_PRODUCT_QUANTITY].ToString().Trim());
            }

            
            // Close SqlDataReader and SqlConnection
            SqlDataReaderId.Close();
            SqlConnectionId.Close();
        }

        #endregion

        #region Internal Functions

        // This function help us to update filtered Bills
        public void fnUpdateResultTable()
        {
            DGV_BillInfo.Columns.Clear();
            DGV_BillInfo.Rows.Clear();
            DGV_BillInfo.Refresh();

            // Not valid Data Source
            if (UserSettings.SQL_DATASOURCE.Length < 1 || UserSettings.SQL_DATASOURCE == null)
            {
                MessageBox.Show("Máy chủ dữ liệu không hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Kết nối dữ liệu bị lỗi", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create an object of SqlCommand to store information of querry command
            SqlCommand SqlCommandId = new SqlCommand();

            // Build the querry command
            string szCommand = "SELECT * FROM " + GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL + " ";

            // We use this flag to make sure that there are 2 more querries at a time to prevent using WHERE many times
            bool bHasPrefix = false;

            // If the Bill Code Filter TextField is valid
            if (TXT_BillCode.Text != null && TXT_BillCode.Text.Trim().Length > 0)
            {
                bHasPrefix = true;
                szCommand += "WHERE ";
                szCommand += GlobalConstants.COLUMN_BILL_CODE + " LIKE '%" + TXT_BillCode.Text + "%' ";
            }

            // If the Customer Code Filter TextField is valid
            if (TXT_CustomerCode.Text != null && TXT_CustomerCode.Text.Trim().Length > 0)
            {
                if (bHasPrefix)
                    szCommand += "AND ";
                else
                {
                    szCommand += "WHERE ";
                    bHasPrefix = true;
                }

                szCommand += GlobalConstants.COLUMN_CUSTOMER_CODE + " LIKE N'%" + TXT_CustomerCode.Text + "%' ";
            }

            // If the Customer Name Filter TextField is valid
            // If the Name Filter TextField is valid
            if (TXT_CustomerName.Text != null && TXT_CustomerName.Text.Trim().Length > 0)
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
            
            
            // Filter by date
            if (CHX_FilterByDate.CheckState == CheckState.Checked)
            {
                if (bHasPrefix)
                    szCommand += "AND ";
                else
                {
                    szCommand += "WHERE ";
                    bHasPrefix = true;
                }

                szCommand += GlobalConstants.COLUMN_BILL_CREATED_DATE + " BETWEEN ";
                szCommand += "'" + DTP_FromDate.Value.ToShortDateString() + "'" + " AND ";
                szCommand += "'" + DTP_EndDate.Value.ToShortDateString() + " " + "23:59:59.999'";

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
            DGV_BillInfo.DataSource = BindingSourceId;

            DGV_BillInfo.Columns[GlobalConstants.COLUMN_BILL_MONEY].DefaultCellStyle.Format = "N3";

            // Update data grid view table
            DGV_BillInfo.Refresh();

            // Close SQL Connection and SQL Reader for safety
            SqlDataReaderId.Close();
            SqlConnectionId.Close();
        }

        #endregion

        private void DGV_BillInfo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }
}
