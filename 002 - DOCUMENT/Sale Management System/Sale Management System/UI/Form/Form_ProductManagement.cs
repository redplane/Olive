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
    public partial class Form_ProductManagement : System.Windows.Forms.Form
    {
        #region Constructor 

        public Form_ProductManagement()
        {
            InitializeComponent();

            // Set font for DataGridView
            DGV_ProductInfo.Font = new Font(GlobalConstants.PROGRAM_FONT, 10);
            
            // Update data to DataGridView table
            fnUpdateResultTable();

        }

        #endregion

        /*                                  EVENT                                  */

        #region Button Events

        // This event is fired when Back button is clicked
        private void BTN_Back_Click(object sender, EventArgs e)
        {
            // Back to Main Menu
            Form_MainMenu Form_MainMenu = new Form_MainMenu();

            // Close current windows
            this.Dispose();

            // Display Main Menu at the center of the screen
            Form_MainMenu.StartPosition = FormStartPosition.CenterScreen;

            // Show Main Menu
            Form_MainMenu.Show();
        }
        
        // This event is fired when Edit button is clicked
        private void BTN_ProductEdit_Click(object sender, EventArgs e)
        {
            // No row is selected
            if (DGV_ProductInfo.SelectedRows.Count < 1)
            {

                // Display warning message
                MessageBox.Show("Bạn phải chọn ít nhất một dòng", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }
            foreach (DataGridViewRow RowId in DGV_ProductInfo.SelectedRows)
            {
                Product ProductId = new Product();
                
                ProductId.CODE = RowId.Cells[GlobalConstants.COLUMN_PRODUCT_CODE].Value.ToString().Trim();
                ProductId.NAME = RowId.Cells[GlobalConstants.COLUMN_PRODUCT_NAME].Value.ToString().Trim();
                ProductId.QUANTITY = float.Parse(RowId.Cells[GlobalConstants.COLUMN_PRODUCT_QUANTITY].Value.ToString());
                ProductId.PRICE = float.Parse(RowId.Cells[GlobalConstants.COLUMN_PRODUCT_PRICE].Value.ToString());
                ProductId.UNIT = RowId.Cells[GlobalConstants.COLUMN_PRODUCT_UNIT].Value.ToString().Trim();
                ProductId.NOTE = RowId.Cells[GlobalConstants.COLUMN_NOTE].Value.ToString().Trim();


                // Create an object of SForm_PoductEdit
                Dialog_ProductEdit FormId = new Dialog_ProductEdit();

                // Set information to Edit form
                FormId.ProductId = ProductId;

                // Update to Product edit form
                FormId.fnFillInformation();

                // Set its parent form to this form
                FormId.ParentFormId = this;

                // Display Edit form
                FormId.ShowDialog();

                
            }

        }

        // Product add button is clicked
        private void BTN_ProductAdd_Click(object sender, EventArgs e)
        {
            // Create an object of SFrom_ProductAdd
            Dialog_ProductAdd FormId = new Dialog_ProductAdd();

            // Make SubForm understands that this form is its parent
            FormId.FormParentId = this;

            FormId.ShowDialog();
        }

        // This event is fired when Delete button is clicked
        private void BTN_ProductDelete_Click(object sender, EventArgs e)
        {
            // Delete them all
            fnDeleteProductInfo();
        }

        #endregion

        #region Form Events

        // This event is fired when this form is being closed
        private void Event_OnFormClosing(object sender, FormClosingEventArgs e)
        {
            // Display confirmation message
            if (MessageBox.Show("Bạn có muốn đóng cửa sổ ?", "Cảnh báo", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                e.Cancel = true;
        }

        // This event is fired when this form is close (X Button)
        private void Event_OnFormClosed(object sender, FormClosedEventArgs e)
        {
            // Terminate whole application when close this form
            Application.Exit();
            
        }

        #endregion

        #region DataGridView Events

        // This event is fired when user press a key in Data Grid View form
        void DGV_ProductInfo_KeyDown(object sender, KeyEventArgs e)
        {
            // If the pressed key is not Delete key
            if (e.KeyCode != Keys.Delete)
                return;

            fnDeleteProductInfo();
        }

        #endregion

        #region TextBox Events

        // This event is fired when user press a character key in Product Name filter
        private void TXT_ProductName_TextChanged(object sender, EventArgs e)
        {
            fnUpdateResultTable();
        }

        // This event is fired when user enter some text in Product Code filter
        private void TXT_ProductCode_TextChanged(object sender, EventArgs e)
        {
            fnUpdateResultTable();
        }

        #endregion

        #region Internal Functions

        // Delete product information by rows in DataGridView and Sql Table
        private void fnDeleteProductInfo()
        {
            // No row is selected
            if (DGV_ProductInfo.SelectedRows.Count < 1)
            {
                // Display warning message
                MessageBox.Show("Bạn phải chọn một dòng", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create connection to database
            SqlConnection SqlConnectionId = util_Sql.fnConnectToDataBase(UserSettings.SQL_DATASOURCE, UserSettings.SQL_CATALOG, UserSettings.SQL_AUTHENTICATION_METHOD, UserSettings.SQL_USER_NAME, UserSettings.SQL_PASSWORD);

            if (SqlConnectionId == null)
            {
                MessageBox.Show("Kết nối đến cơ sở dữ liệu bị lỗi", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Show the confirmation dialog box
            DialogResult DialogResultId = MessageBox.Show("Bạn có muốn xóa dữ liệu ?", "Cảnh báo" ,MessageBoxButtons.YesNo);
            
            // User chose NO
            if (DialogResultId == DialogResult.No)
            {
                return;
            }

            // Create an object of SqlCommand to execute delete command
            SqlCommand SqlCommandId = new SqlCommand();

            // Create command line

            foreach (DataGridViewRow RowId in DGV_ProductInfo.SelectedRows)
            {
                
            
                string szSqlCommand = "DELETE FROM " + GlobalConstants.TABLE_PRODUCT_MANAGEMENT +
                                    " WHERE " + GlobalConstants.COLUMN_PRODUCT_CODE +
                                    " = ";

                szSqlCommand += "'";
                szSqlCommand += RowId.Cells[GlobalConstants.COLUMN_PRODUCT_CODE].Value;
                szSqlCommand += "'";

                DGV_ProductInfo.Rows.Remove(RowId);

                // Update information to SqlCommand Object 
                SqlCommandId.CommandText = szSqlCommand;
                SqlCommandId.Connection = SqlConnectionId;

                // Execute delete command
                SqlCommandId.ExecuteNonQuery();
            }

            // Close connection for safety
            SqlConnectionId.Close();

            // Update product information table
            fnUpdateResultTable();
        }

        // Update DataGridView Table
        public void fnUpdateResultTable()
        {
            DGV_ProductInfo.Columns.Clear();
            DGV_ProductInfo.Rows.Clear();
            DGV_ProductInfo.Refresh();

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
                MessageBox.Show("Kết nối bị lỗi", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create an object of SqlCommand to store information of querry command
            SqlCommand SqlCommandId = new SqlCommand();

            // Build the querry command
            string szCommand = "SELECT * FROM " + GlobalConstants.TABLE_PRODUCT_MANAGEMENT + " ";

            // We use this flag to make sure that there are 2 more querries at a time to prevent using WHERE many times
            bool bHasPrefix = false;

            // If the Code Filter TextField is valid
            if (TXT_ProductCode.Text != null && TXT_ProductCode.Text.Length > 0)
            {
                bHasPrefix = true;
                szCommand += "WHERE ";
                szCommand += GlobalConstants.COLUMN_PRODUCT_CODE + " LIKE '%" + TXT_ProductCode.Text + "%' ";
            }

            // If the Name Filter TextField is valid
            if (TXT_ProductName.Text != null && TXT_ProductName.Text.Length > 0)
            {
                if (bHasPrefix)
                    szCommand += "AND ";
                else
                {
                    szCommand += "WHERE ";
                    bHasPrefix = true;
                }

                szCommand += GlobalConstants.COLUMN_PRODUCT_NAME + " LIKE N'%" + TXT_ProductName.Text + "%' ";
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
            DGV_ProductInfo.DataSource = BindingSourceId;
            
            DGV_ProductInfo.Columns[GlobalConstants.COLUMN_PRODUCT_PRICE].DefaultCellStyle.Format = "N3";

            DGV_ProductInfo.Columns[GlobalConstants.COLUMN_PRODUCT_CODE].SortMode = DataGridViewColumnSortMode.Automatic;
            DGV_ProductInfo.Columns[GlobalConstants.COLUMN_PRODUCT_NAME].SortMode = DataGridViewColumnSortMode.Automatic;
            DGV_ProductInfo.Columns[GlobalConstants.COLUMN_PRODUCT_PRICE].SortMode = DataGridViewColumnSortMode.Automatic;
            DGV_ProductInfo.Columns[GlobalConstants.COLUMN_PRODUCT_QUANTITY].SortMode = DataGridViewColumnSortMode.Automatic;



            DGV_ProductInfo.Columns[GlobalConstants.COLUMN_PRODUCT_CODE].HeaderText = "Mã sản phẩm";
            DGV_ProductInfo.Columns[GlobalConstants.COLUMN_PRODUCT_NAME].HeaderText = "Tên sản phẩm";
            DGV_ProductInfo.Columns[GlobalConstants.COLUMN_PRODUCT_UNIT].HeaderText = "Đơn vị sản phẩm";
            DGV_ProductInfo.Columns[GlobalConstants.COLUMN_PRODUCT_PRICE].HeaderText = "Giá sản phẩm";
            DGV_ProductInfo.Columns[GlobalConstants.COLUMN_NOTE].HeaderText = "Ghi chú";

            DGV_ProductInfo.Columns[GlobalConstants.COLUMN_PRODUCT_QUANTITY].HeaderText = "Số lượng";
            // Update data grid view table
            DGV_ProductInfo.Refresh();

            // Close SQL Connection and SQL Reader for safety
            SqlDataReaderId.Close();
            SqlConnectionId.Close();
        }

        #endregion
    }
}
