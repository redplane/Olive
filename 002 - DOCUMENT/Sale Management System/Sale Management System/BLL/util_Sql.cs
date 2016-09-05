using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Sale_Management_System
{
    public static class util_Sql
    {
        // Execute a connection to Database
        public static SqlConnection fnConnectToDataBase(string szDataSource, string szCatalog, bool SqlAuthentication, string szUserName, string szPassword)
        {
            // Connection command to database
            //string szConnectionCommand = "Data Source=" + szDataSource + ";Initial Catalog=" + szCatalog + "; Integrated Security=True";
            
            // Create an object of SqlConnection to help program connect to Database
            SqlConnection SqlConnectionId = new SqlConnection();

            // Create an object of SqlConnectionStringBuilder to help us build a connection command
            SqlConnectionStringBuilder SqlConnectionStringBuilderId = new SqlConnectionStringBuilder();
            
            // Build the command
            SqlConnectionStringBuilderId.DataSource = szDataSource;
            SqlConnectionStringBuilderId.InitialCatalog = szCatalog;

            // Do we want to use Sql Login Authentication ?
            if (SqlAuthentication)
            {
                SqlConnectionStringBuilderId.UserID = szUserName;
                SqlConnectionStringBuilderId.Password = szPassword;
                SqlConnectionStringBuilderId.IntegratedSecurity = false;
            }
            else
            {
                SqlConnectionStringBuilderId.IntegratedSecurity = true;
            }

            // Set connection time out to 5 sec to help us not to waste much time waiting for server response
            // In case database doesn't exist
            SqlConnectionStringBuilderId.ConnectTimeout = 5000;

            // Bind login command to SqlConnectionId
            SqlConnectionId.ConnectionString = SqlConnectionStringBuilderId.ConnectionString;
            
            // Open connection to database
            try
            {
                SqlConnectionId.Open();
            }
            catch
            {
                // Return null if this connection causes error
                return null;
            }

            // Return Connection ID
            return SqlConnectionId;
        }

        // Check if a value already existed in a column or not
        public static bool fnIsInDatabase(SqlConnection SqlConnectionId, string szTableName, string szColumnName, string szValue)
        {
            // Connection to database is fail
            if (SqlConnectionId == null || SqlConnectionId.State == System.Data.ConnectionState.Closed || SqlConnectionId.State == System.Data.ConnectionState.Broken)
                return true;

            // Store command information to szCommand
            string szCommand;

            // Create an object of SqlCommand to store Querry command information
            SqlCommand SqlCommandId = new SqlCommand();

            // Create an object of SqlReader to store read data from Database
            SqlDataReader SqlDataReaderId;

            //////////////////////////////////////////////////////////////////////////
            // Querry if Product Code already existed
            // Command : "SELECT * FROM " + TABLE_NAME + " WHERE COLUMN = '{VALUE}'";

            szCommand = String.Format("SELECT * FROM {0} WHERE {1} = '{2}'",
                                        szTableName,
                                        szColumnName,
                                        szValue);

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

            if (SqlDataReaderId == null)
                return false;
            if (SqlDataReaderId.HasRows)
            {
                // Free SqlReader for safety
                SqlDataReaderId.Close();

                return true;
            }

            SqlDataReaderId.Close();
            return false;
        }

        #region DataTable Creation

        // Create Product Management Table if it doesn't exist
        public static void fnCreateTableProduct(SqlConnection SqlConnectionId)
        {
            if (SqlConnectionId == null)
                return;

            // Create SqlCommand's object
            SqlCommand SqlCommandId = new SqlCommand();

            if (SqlCommandId == null)
                return;

            #region Column Settings

            string DTYPE_PRODUCT_CODE = "CHAR(32) NOT NULL PRIMARY KEY";
            string DTYPE_PRODUCT_NAME = "NCHAR(64) NOT NULL";
            string DTYPE_PRODUCT_QUANTITY = "FLOAT";
            string DTYPE_PRODUCT_UNIT = "NCHAR(32) NOT NULL";
            string DTYPE_PRODUCT_PRICE = "FLOAT";
            string DTYPE_PRODUCT_NOTE = "NCHAR(255)";

            #endregion

            // Store creation command here
            string szCommand;

            #region Table Creation

            // Product Table
            szCommand = "IF NOT EXISTS ";
            szCommand += "( ";
            szCommand += "SELECT [name] ";
            szCommand += "FROM sys.tables ";
            szCommand += " WHERE [name] = '" + GlobalConstants.TABLE_PRODUCT_MANAGEMENT + "' ";
            szCommand += ") ";
            szCommand += "CREATE TABLE " + GlobalConstants.TABLE_PRODUCT_MANAGEMENT;
            szCommand += "( ";
            szCommand += GlobalConstants.COLUMN_PRODUCT_CODE + " " + DTYPE_PRODUCT_CODE + ", "; // Column : Product Code
            szCommand += GlobalConstants.COLUMN_PRODUCT_NAME + " " + DTYPE_PRODUCT_NAME + ", "; // Column : Product Name
            szCommand += GlobalConstants.COLUMN_PRODUCT_QUANTITY + " " + DTYPE_PRODUCT_QUANTITY + ", "; // Column : Product Quantity
            szCommand += GlobalConstants.COLUMN_PRODUCT_UNIT + " " + DTYPE_PRODUCT_UNIT + ", "; // Column : Product Unit
            szCommand += GlobalConstants.COLUMN_PRODUCT_PRICE + " " + DTYPE_PRODUCT_PRICE + ", "; // Column : Product Price
            szCommand += GlobalConstants.COLUMN_NOTE + " " + DTYPE_PRODUCT_NOTE;
            szCommand += ")";


            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            // Incase this table exists, but it misses some columns
            // Store the name of the checking columns
            string szCheckingColumn;

            // Type of data of the checking column
            string szDataType;

            #region Column : Product Code
            
            szCheckingColumn = GlobalConstants.COLUMN_PRODUCT_CODE;
            szDataType = DTYPE_PRODUCT_CODE;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_PRODUCT_MANAGEMENT, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_PRODUCT_MANAGEMENT, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Product Name

            szCheckingColumn = GlobalConstants.COLUMN_PRODUCT_NAME;
            szDataType = DTYPE_PRODUCT_NAME;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_PRODUCT_MANAGEMENT, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_PRODUCT_MANAGEMENT, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Product Quantity

            szCheckingColumn = GlobalConstants.COLUMN_PRODUCT_QUANTITY;
            szDataType = DTYPE_PRODUCT_QUANTITY;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_PRODUCT_MANAGEMENT, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_PRODUCT_MANAGEMENT, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Product Unit

            szCheckingColumn = GlobalConstants.COLUMN_PRODUCT_UNIT;
            szDataType = DTYPE_PRODUCT_UNIT;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_PRODUCT_MANAGEMENT, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_PRODUCT_MANAGEMENT, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Product Price 

            szCheckingColumn = GlobalConstants.COLUMN_PRODUCT_PRICE;
            szDataType = DTYPE_PRODUCT_PRICE;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_PRODUCT_MANAGEMENT, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_PRODUCT_MANAGEMENT, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Product Note

            szCheckingColumn = GlobalConstants.COLUMN_NOTE;
            szDataType = DTYPE_PRODUCT_NOTE;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_PRODUCT_MANAGEMENT, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_PRODUCT_MANAGEMENT, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Data Releasing

            DTYPE_PRODUCT_CODE = null;
            DTYPE_PRODUCT_NAME = null;
            DTYPE_PRODUCT_NOTE = null;
            DTYPE_PRODUCT_PRICE = null;
            DTYPE_PRODUCT_QUANTITY = null;

            szCheckingColumn = null;
            szCommand = null;
            szDataType = null;

            #endregion
        }

        // Create Product Management Table if it doesn't exist
        public static void fnCreateTableCustomer(SqlConnection SqlConnectionId)
        {
            if (SqlConnectionId == null)
                return;

            // Create SqlCommand's object
            SqlCommand SqlCommandId = new SqlCommand();

            if (SqlCommandId == null)
                return;

            #region Columns Settings 

            string DTYPE_CUSTOMER_CODE = "CHAR(32) NOT NULL PRIMARY KEY";
            string DTYPE_CUSTOMER_NAME = "NCHAR(64) NOT NULL";
            string DTYPE_CUSTOMER_AGE = "INT";
            string DTYPE_CUSTOMER_BILL_QUANTITY = "INT";
            string DTYPE_CUSTOMER_BALANCE = "FLOAT";
            string DTYPE_CUSTOMER_EMAIL = "NCHAR(128)";
            string DTYPE_CUSTOMER_ADDRESS = "NCHAR(128)";
            string DTYPE_CUSTOMER_NOTE = "NCHAR(256)";

            #endregion

            // Store command information
            string szCommand;

            #region Customer Table Creation

            szCommand = "";
            szCommand = "IF NOT EXISTS ";
            szCommand += "( ";
            szCommand += "SELECT [name] ";
            szCommand += "FROM sys.tables ";
            szCommand += " WHERE [name] = '" + GlobalConstants.TABLE_CUSTOMER_MANAGEMENT + "' ";
            szCommand += ") ";
            szCommand += "CREATE TABLE " + GlobalConstants.TABLE_CUSTOMER_MANAGEMENT;
            szCommand += "( ";
            szCommand += GlobalConstants.COLUMN_CUSTOMER_CODE + " " + DTYPE_CUSTOMER_CODE + ", "; // Column : Customer Code
            szCommand += GlobalConstants.COLUMN_CUSTOMER_NAME + " " + DTYPE_CUSTOMER_NAME + ", "; // Column : Customer Name
            szCommand += GlobalConstants.COLUMN_CUSTOMER_AGE + " " + DTYPE_CUSTOMER_AGE + ", "; // Column : Customer Age
            szCommand += GlobalConstants.COLUMN_CUSTOMER_BILL_QUANTITY + " " + DTYPE_CUSTOMER_BILL_QUANTITY + ", "; // Column : Customer Bill Quantity
            szCommand += GlobalConstants.COLUMN_CUSTOMER_BALANCE + " " + DTYPE_CUSTOMER_BILL_QUANTITY + ", ";
            szCommand += GlobalConstants.COLUMN_CUSTOMER_EMAIL + " " + DTYPE_CUSTOMER_EMAIL + ", ";
            szCommand += GlobalConstants.COLUMN_CUSTOMER_ADDRESS + " " + DTYPE_CUSTOMER_ADDRESS + ", ";
            szCommand += GlobalConstants.COLUMN_NOTE + " " + DTYPE_CUSTOMER_NOTE;
            szCommand += ")";

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            // Incase this table exists, but it misses some columns
            // Store the name of the checking columns
            string szCheckingColumn;

            // Type of data of the checking column
            string szDataType;

            #region Column : Customer Code

            szCheckingColumn = GlobalConstants.COLUMN_CUSTOMER_CODE;
            szDataType = DTYPE_CUSTOMER_CODE;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Customer Name

            szCheckingColumn = GlobalConstants.COLUMN_CUSTOMER_NAME;
            szDataType = DTYPE_CUSTOMER_NAME;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Customer Age

            szCheckingColumn = GlobalConstants.COLUMN_CUSTOMER_AGE;
            szDataType = DTYPE_CUSTOMER_AGE;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Bill Quantity

            szCheckingColumn = GlobalConstants.COLUMN_CUSTOMER_BILL_QUANTITY;
            szDataType = DTYPE_CUSTOMER_BILL_QUANTITY;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Customer Balance

            szCheckingColumn = GlobalConstants.COLUMN_CUSTOMER_BALANCE;
            szDataType = DTYPE_CUSTOMER_BALANCE;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Customer Email

            szCheckingColumn = GlobalConstants.COLUMN_CUSTOMER_EMAIL;
            szDataType = DTYPE_CUSTOMER_EMAIL;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Customer Address

            szCheckingColumn = GlobalConstants.COLUMN_CUSTOMER_ADDRESS;
            szDataType = DTYPE_CUSTOMER_ADDRESS;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Customer Note

            szCheckingColumn = GlobalConstants.COLUMN_NOTE;
            szDataType = DTYPE_CUSTOMER_NOTE;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_CUSTOMER_MANAGEMENT, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Variables Release

            DTYPE_CUSTOMER_ADDRESS = null;
            DTYPE_CUSTOMER_AGE = null;
            DTYPE_CUSTOMER_BALANCE = null;
            DTYPE_CUSTOMER_BILL_QUANTITY = null;
            DTYPE_CUSTOMER_CODE = null;
            DTYPE_CUSTOMER_NAME = null;
            DTYPE_CUSTOMER_NOTE = null;

            szCheckingColumn = null;
            szCommand = null;
            szDataType = null;

            #endregion

        }

        // Create Product Management Table if it doesn't exist
        public static void fnCreateTableBillGeneral(SqlConnection SqlConnectionId)
        {
            if (SqlConnectionId == null)
                return;

            // Create SqlCommand's object
            SqlCommand SqlCommandId = new SqlCommand();

            if (SqlCommandId == null)
                return;

            #region Columns Settings

            string DTYPE_BILL_CODE = "CHAR(32) NOT NULL PRIMARY KEY";
            string DTYPE_CUSTOMER_CODE = "CHAR(32)";
            string DTYPE_CUSTOMER_NAME = "NCHAR(64)";
            string DTYPE_BILL_PRICE = "FLOAT NOT NULL";
            string DTYPE_PAID_MONEY = "FLOAT NOT NULL";
            string DTYPE_CREATED_DATE = "DATETIME";
            string DTYPE_BILL_NOTE = "NCHAR(1024)";

            #endregion

            // Store command information
            string szCommand;

            #region Table [Customer] Creation

            szCommand = "";
            szCommand = "IF NOT EXISTS ";
            szCommand += "( ";
            szCommand += "SELECT [name] ";
            szCommand += "FROM sys.tables ";
            szCommand += " WHERE [name] = '" + GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL + "' ";
            szCommand += ") ";
            szCommand += "CREATE TABLE " + GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL;
            szCommand += "( ";
            szCommand += GlobalConstants.COLUMN_BILL_CODE + " " + DTYPE_BILL_CODE + ", ";
            szCommand += GlobalConstants.COLUMN_CUSTOMER_CODE + " " + DTYPE_CUSTOMER_CODE + ", ";
            szCommand += GlobalConstants.COLUMN_CUSTOMER_NAME + " " + DTYPE_CUSTOMER_NAME + ", ";
            szCommand += GlobalConstants.COLUMN_BILL_MONEY + " " + DTYPE_BILL_PRICE + ", ";
            szCommand += GlobalConstants.COLUMN_PAID_MONEY + " " + DTYPE_PAID_MONEY + ", ";
            szCommand += GlobalConstants.COLUMN_BILL_CREATED_DATE + " " + DTYPE_CREATED_DATE + ", ";
            szCommand += GlobalConstants.COLUMN_NOTE + " " + DTYPE_BILL_NOTE + " ";
            szCommand += ")";

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            // Incase this table exists, but it misses some columns
            // Store the name of the checking columns
            string szCheckingColumn;

            // Type of data of the checking column
            string szDataType;

            #region Column : Bill Code

            szCheckingColumn = GlobalConstants.COLUMN_BILL_CODE;
            szDataType = DTYPE_BILL_CODE;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Customer Code

            szCheckingColumn = GlobalConstants.COLUMN_CUSTOMER_CODE;
            szDataType = DTYPE_CUSTOMER_CODE;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Customer Name

            szCheckingColumn = GlobalConstants.COLUMN_CUSTOMER_NAME;
            szDataType = GlobalConstants.COLUMN_CUSTOMER_NAME;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Bill Price

            szCheckingColumn = GlobalConstants.COLUMN_BILL_MONEY;
            szDataType = DTYPE_BILL_PRICE;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Paid Money

            szCheckingColumn = GlobalConstants.COLUMN_PAID_MONEY;
            szDataType = DTYPE_PAID_MONEY;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Bill Created Date

            szCheckingColumn = GlobalConstants.COLUMN_BILL_CREATED_DATE;
            szDataType = DTYPE_CREATED_DATE;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Bill Note

            szCheckingColumn = GlobalConstants.COLUMN_NOTE;
            szDataType = DTYPE_BILL_NOTE;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_BILL_MANAGEMENT_GENERAL, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Variables Releasing

            DTYPE_BILL_CODE = null;
            DTYPE_CUSTOMER_CODE = null;
            DTYPE_CUSTOMER_NAME = null;
            DTYPE_BILL_PRICE = null;
            DTYPE_PAID_MONEY = null;
            DTYPE_CREATED_DATE = null;
            DTYPE_BILL_NOTE = null;

            szCheckingColumn = null;
            szCommand = null;
            szDataType = null;

            #endregion
        }

        // Create Product Table Bill Detailed
        public static void fnCreateTableBillDetailed(SqlConnection SqlConnectionId)
        {
            if (SqlConnectionId == null)
                return;

            // Create SqlCommand's object
            SqlCommand SqlCommandId = new SqlCommand();

            if (SqlCommandId == null)
                return;

            #region Column Settings

            string DTYPE_BILL_CODE = "CHAR(32) NOT NULL";
            string DTYPE_CUSTOMER_CODE = "CHAR(32)";
            string DTYPE_CUSTOMER_NAME = "NCHAR(64)";
            string DTYPE_PRODUCT_CODE = "CHAR(32) NOT NULL";
            string DTYPE_PRODUCT_NAME = "NCHAR(128) NOT NULL";
            string DTYPE_PRODUCT_QUANTITY = "FLOAT NOT NULL";
            string DTYPE_PRODUCT_UNIT = "NCHAR(32) NOT NULL";
            string DTYPE_PRODUCT_TOTAL_PRICE = "FLOAT NOT NULL";
            string DTYPE_BILL_CREATED_DATE = "DATETIME";

            #endregion

            #region Table [Bill Detailed] Building

            // Store command information
            string szCommand;

            // Customer Table
            szCommand = "";
            szCommand = "IF NOT EXISTS ";
            szCommand += "( ";
            szCommand += "SELECT [name] ";
            szCommand += "FROM sys.tables ";
            szCommand += " WHERE [name] = '" + GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED + "' ";
            szCommand += ") ";
            szCommand += "CREATE TABLE " + GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED;
            szCommand += "( ";
            szCommand += GlobalConstants.COLUMN_BILL_CODE + " " + DTYPE_BILL_CODE + ", ";
            szCommand += GlobalConstants.COLUMN_CUSTOMER_CODE + " " + DTYPE_CUSTOMER_CODE + ", ";
            szCommand += GlobalConstants.COLUMN_CUSTOMER_NAME + " " + DTYPE_CUSTOMER_NAME + ", ";
            szCommand += GlobalConstants.COLUMN_PRODUCT_CODE + " " + DTYPE_PRODUCT_CODE + ", ";
            szCommand += GlobalConstants.COLUMN_PRODUCT_NAME + " " + DTYPE_PRODUCT_NAME + ", ";
            szCommand += GlobalConstants.COLUMN_PRODUCT_QUANTITY + " " + DTYPE_PRODUCT_QUANTITY + ", ";
            szCommand += GlobalConstants.COLUMN_PRODUCT_UNIT + " " + DTYPE_PRODUCT_UNIT + ", ";
            szCommand += GlobalConstants.COLUMN_PRODUCT_TOTAL_PRICE + " " + DTYPE_PRODUCT_TOTAL_PRICE + ", ";
            szCommand += GlobalConstants.COLUMN_BILL_CREATED_DATE + " " + DTYPE_BILL_CREATED_DATE + " ";
            szCommand += ")";


            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            // Incase this table exists, but it misses some columns
            // Store the name of the checking columns
            string szCheckingColumn;

            // Type of data of the checking column
            string szDataType;

            #region Column : Bill Code

            szCheckingColumn = GlobalConstants.COLUMN_BILL_CODE;
            szDataType = DTYPE_BILL_CODE;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Customer Code

            szCheckingColumn = GlobalConstants.COLUMN_CUSTOMER_CODE;
            szDataType = DTYPE_CUSTOMER_CODE;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Customer Name

            szCheckingColumn = GlobalConstants.COLUMN_CUSTOMER_NAME;
            szDataType = GlobalConstants.COLUMN_CUSTOMER_NAME;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Product Code

            szCheckingColumn = GlobalConstants.COLUMN_PRODUCT_CODE;
            szDataType = DTYPE_PRODUCT_CODE;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Product Name

            szCheckingColumn = GlobalConstants.COLUMN_PRODUCT_NAME;
            szDataType = DTYPE_PRODUCT_NAME;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Product Quantity

            szCheckingColumn = GlobalConstants.COLUMN_PRODUCT_QUANTITY;
            szDataType = DTYPE_PRODUCT_QUANTITY;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED, szCheckingColumn, szDataType);

            #endregion

            #region Product Unit

            szCheckingColumn = GlobalConstants.COLUMN_PRODUCT_UNIT;
            szDataType = DTYPE_PRODUCT_UNIT;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED, szCheckingColumn, szDataType);

            #endregion

            #region Column : Column Product Total Price

            szCheckingColumn = GlobalConstants.COLUMN_PRODUCT_TOTAL_PRICE;
            szDataType = DTYPE_PRODUCT_TOTAL_PRICE;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Column : Bill Created Date

            szCheckingColumn = GlobalConstants.COLUMN_BILL_CREATED_DATE;
            szDataType = DTYPE_BILL_CREATED_DATE;

            szCommand = "IF NOT EXISTS";
            szCommand += "(";
            szCommand += "SELECT * FROM syscolumns ";
            szCommand += String.Format("WHERE id=object_id('{0}') AND name='{1}' ", GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED, szCheckingColumn);
            szCommand += ")";
            szCommand += String.Format("ALTER TABLE {0} ADD {1} {2}", GlobalConstants.TABLE_BILL_MANAGEMENT_DETAILED, szCheckingColumn, szDataType);

            SqlCommandId.CommandText = szCommand;
            SqlCommandId.Connection = SqlConnectionId;
            SqlCommandId.ExecuteNonQuery();

            #endregion

            #region Variables Releasing

            DTYPE_BILL_CODE = null;
            DTYPE_CUSTOMER_CODE = null;
            DTYPE_CUSTOMER_NAME = null;
            DTYPE_PRODUCT_CODE = null;
            DTYPE_PRODUCT_NAME = null;
            DTYPE_PRODUCT_QUANTITY = null;
            DTYPE_PRODUCT_TOTAL_PRICE = null;

            szCheckingColumn = null;
            szCommand = null;
            szDataType = null;

            #endregion
        }

        #endregion
    }
}