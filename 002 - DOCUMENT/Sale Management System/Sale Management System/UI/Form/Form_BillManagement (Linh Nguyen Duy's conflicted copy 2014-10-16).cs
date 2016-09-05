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

            // Update titles to all components
            fnUpdateTitle();

            // Update bill list
            fnUpdateResultTable();


            #region PictureBoxes Images

            if (GlobalFunctions.fn_SetPictureBoxImage(PBX_BillAdd, GlobalConstants.DIRECTORY_ICON_IMAGE, GlobalConstants.ICON_ADD))
                PBX_BillAdd.BackColor = Color.Transparent;

            if (GlobalFunctions.fn_SetPictureBoxImage(PBX_BillPrinting, GlobalConstants.DIRECTORY_ICON_IMAGE, GlobalConstants.ICON_PRINT))
                PBX_BillPrinting.BackColor = Color.Transparent;

            if (GlobalFunctions.fn_SetPictureBoxImage(PBX_Back, GlobalConstants.DIRECTORY_ICON_IMAGE, GlobalConstants.ICON_BACK))
                PBX_Back.BackColor = Color.Transparent;

            #endregion

            #region Form Background

            GlobalFunctions.fn_SetFormBackground(this, "BillManagement.jpg");

            #endregion

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
        
        // Fired when Print Bill is pressed
        private void BTN_PrintBill_Click(object sender, EventArgs e)
        {
            // Select more or less than 1 row ?
            if (DGV_BillInfo.SelectedRows.Count != 1)
            {
                // Clear DataGridView
                DGV_ProductList.Rows.Clear();
                DGV_ProductList.Refresh();
                return;
            }

            // Find bill and print it by searching its code
            string szBillCode    = DRVR_CurrentRow.Cells[GlobalConstants.COLUMN_BILL_CODE].Value.ToString();
            string szCustomerName = DRVR_CurrentRow.Cells[GlobalConstants.COLUMN_CUSTOMER_NAME].Value.ToString();
            string szBillMoney   = DRVR_CurrentRow.Cells[GlobalConstants.COLUMN_BILL_MONEY].Value.ToString();
            string szPaidMoney = DRVR_CurrentRow.Cells[GlobalConstants.COLUMN_PAID_MONEY].Value.ToString();
            string szCreatedDate = DRVR_CurrentRow.Cells[GlobalConstants.COLUMN_BILL_CREATED_DATE].Value.ToString();
            
            Util_Printing.fnPrintBillByCode(szBillCode, szCustomerName, szBillMoney, szPaidMoney, szCreatedDate);

            szBillCode = null;
            szCustomerName = null;
            szBillMoney = null;
            szPaidMoney = null;
            szCreatedDate = null;
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
            if (MessageBox.Show(GlobalFunctions.fnGetValidLanguage("MESSAGE_CONFIRMATION_CLOSE"), "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
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

        public void fnUpdateTitle()
        {
            // Font Preparation
            Font FontId = new Font(GlobalConstants.PROGRAM_FONT, GlobalConstants.PROGRAM_FONT_SIZE);

            // Update Menu Title
            this.Text = GlobalFunctions.fnGetValidLanguage("TITLE_BILL_MANAGEMENT_MENU");

            #region Labels

            LBL_BillCode.Text = GlobalFunctions.fnGetValidLanguage("TITLE_FILTER_BILL_CODE");
            LBL_CustomerCode.Text = GlobalFunctions.fnGetValidLanguage("TITLE_FILTER_CUSTOMER_CODE");
            LBL_CustomerName.Text = GlobalFunctions.fnGetValidLanguage("TITLE_FILTER_CUSTOMER_NAME");
            LBL_FromDate.Text = GlobalFunctions.fnGetValidLanguage("TITLE_DATE_START");
            LBL_EndDate.Text = GlobalFunctions.fnGetValidLanguage("TITLE_DATE_END");
            LBL_ProductList.Text = GlobalFunctions.fnGetValidLanguage("TITLE_BOUGHT_PRODUCTS_LIST");

            LBL_BillCode.Font = FontId;
            LBL_CustomerCode.Font = FontId;
            LBL_CustomerName.Font = FontId;
            LBL_FromDate.Font = FontId;
            LBL_EndDate.Font = FontId;
            LBL_ProductList.Font = FontId;
            #endregion

            // Check Box
            CHX_FilterByDate.Text = GlobalFunctions.fnGetValidLanguage("TITLE_DATE_RANGE");
            CHX_FilterByDate.Font = FontId;

            // Clear DataGridView
            DGV_ProductList.Columns.Clear();
            DGV_ProductList.Refresh();

            // Create columns for Product DataGridView
            string[] szColumnList = { GlobalConstants.COLUMN_PRODUCT_CODE, GlobalConstants.COLUMN_PRODUCT_NAME, GlobalConstants.COLUMN_PRODUCT_QUANTITY };

            foreach (string szInfo in szColumnList)
            {
                DGV_ProductList.Columns.Add(szInfo, GlobalFunctions.fnGetValidLanguage(szInfo));
                DGV_ProductList.Columns[szInfo].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

        }

        // This function help us to update filtered Bills
        public void fnUpdateResultTable()
        {
            DGV_BillInfo.Columns.Clear();
            DGV_BillInfo.Rows.Clear();
            DGV_BillInfo.Refresh();

            // Not valid Data Source
            if (UserSettings.SQL_DATASOURCE.Length < 1 || UserSettings.SQL_DATASOURCE == null)
            {
                MessageBox.Show(GlobalFunctions.fnGetValidLanguage("WARN_NOT_VALID_DATASOURCE"));
                return;
            }

            // Not valid Catalog
            if (UserSettings.SQL_CATALOG.Length < 1 || UserSettings.SQL_CATALOG == null)
            {
                MessageBox.Show(GlobalFunctions.fnGetValidLanguage("WARN_NOT_VALID_CATALOG"));
                return;
            }

            // SQL Connection object
            SqlConnection SqlConnectionId = util_Sql.fnConnectToDataBase(UserSettings.SQL_DATASOURCE, UserSettings.SQL_CATALOG, UserSettings.SQL_AUTHENTICATION_METHOD, UserSettings.SQL_USER_NAME, UserSettings.SQL_PASSWORD);

            if (SqlConnectionId == null)
            {
                MessageBox.Show(GlobalFunctions.fnGetValidLanguage("WARN_CONNECTION_FAIL"));
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

            
            // Rename all columns
            //string [] szColumnList = {GlobalConstants.COLUMN_PRODUCT_CODE, GlobalConstants.COLUMN_PRODUCT_NAME,
            //                        GlobalConstants.COLUMN_PRODUCT_QUANTITY,
            //                        GlobalConstants.COLUMN_PRODUCT_PRICE, GlobalConstants.COLUMN_NOTE};

            foreach (DataGridViewColumn ColumnId in DGV_BillInfo.Columns)
                ColumnId.HeaderText = GlobalFunctions.fnGetValidLanguage(ColumnId.HeaderText);  
            
            DGV_BillInfo.Columns[GlobalConstants.COLUMN_BILL_MONEY].DefaultCellStyle.Format = "N3";

            // Update data grid view table
            DGV_BillInfo.Refresh();

            // Close SQL Connection and SQL Reader for safety
            SqlDataReaderId.Close();
            SqlConnectionId.Close();
        }

        #endregion

    }
}
