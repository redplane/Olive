using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Resources;
using System.Globalization;
using System.Data.SqlClient;

namespace Sale_Management_System
{
    public static class GlobalConstants
    {
        #region Language

        public const string LANGUAGE_ENGLISH = "en-us";
        public const string LANGUAGE_VIETNAMESE = "vi-vn";

        #endregion

        #region INI Key

        public const string KEY_SQL_DATABASE =  "[DATA_SOURCE]";
        public const string KEY_SQL_CATALOG =   "[CATALOG]";
        public const string KEY_LANGUAGE = "[LANGUAGE]";

        public const string KEY_PRINTER_NAME = "[PRINTER_NAME]";
        public const string KEY_PAPER_NAME = "[PAPER_NAME]";
        public const string KEY_PAPER_HEIGHT = "[PAPER_HEIGHT]";
        public const string KEY_PAPER_WIDTH = "[PAPER_WIDTH]";

        #endregion

        #region Key Length

        public const int LENGTH_PRODUCT_CODE = 16;
        public const int LENGTH_PRODUCT_NAME = 32;
        public const int LENGTH_PRODUCT_QUANTITY = 10;
        public const int LENGTH_PRODUCT_UNIT = 10;
        
        #endregion

        #region Config File

        public const string FILE_USER_SETTING = "UserSettings.ini";
        public const string FILE_COMPANY_INFO = "CompanyInfo.txt";

        #endregion

        #region Resources

        public static CultureInfo InfoCulture = null;

        public static string PROGRAM_LANGUAGE = LANGUAGE_ENGLISH;

        public const string PROGRAM_RESOURCE = "Sale_Management_System.Lang.MyResource";
        public static string PROGRAM_IMAGE_LOGIN_BACKGROUND = Path.GetDirectoryName(Application.ExecutablePath) + @"\background_img.png";

        #endregion

        #region Icons

        public const string ICON_BACK = "Back.png";
        public const string ICON_ADD = "Add.png";
        public const string ICON_EDIT = "Edit.png";
        public const string ICON_REMOVE = "Remove.png";
        public const string ICON_PRINT = "Printer.png";
        public const string ICON_REFRESH = "Refresh.png";
        public const string ICON_CLEAR = "Clear.png";
        public const string ICON_SAVE = "Save.png";
        public const string ICON_GENERATE = "Generate.png";

        public const string ICON_PRINT_PREVIEW = "PrintPreview.png";

        #endregion

        public const string PROGRAM_FONT = "Times New Roman";
        public const int PROGRAM_FONT_SIZE = 9;

        #region Directory

        public const string DIRECTORY_BACKGROUND_IMAGE = @"Resources\BackgroundImages";
        public const string DIRECTORY_ICON_IMAGE = @"Resources\Icons";

        #endregion

        #region Sql Tables Name

        public const string TABLE_PRODUCT_MANAGEMENT = "ProductManagement";
        public const string TABLE_CUSTOMER_MANAGEMENT = "CustomerManagement";
        public const string TABLE_BILL_MANAGEMENT_GENERAL = "BillManagementGeneral";
        public const string TABLE_BILL_MANAGEMENT_DETAILED = "BillManagementDetailed";

        #endregion
        
        #region List of Sql Tables Columns

        #region Product Management

        public const string COLUMN_PRODUCT_CODE = "COLUMN_PRODUCT_CODE";
        public const string COLUMN_PRODUCT_NAME = "COLUMN_PRODUCT_NAME";
        public const string COLUMN_PRODUCT_QUANTITY = "COLUMN_PRODUCT_QUANTITY";
        public const string COLUMN_PRODUCT_UNIT = "COLUMN_PRODUCT_UNIT";
        public const string COLUMN_PRODUCT_PRICE = "COLUMN_PRODUCT_PRICE";
        public const string COLUMN_NOTE = "COLUMN_NOTE";

        #endregion

        #region Customer Management

        public const string PREFIX_CUSTOMER = "CUS_";
        public const string COLUMN_CUSTOMER_CODE = "COLUMN_CUSTOMER_CODE";
        public const string COLUMN_CUSTOMER_NAME = "COLUMN_CUSTOMER_NAME";
        public const string COLUMN_CUSTOMER_AGE = "COLUMN_CUSTOMER_AGE";
        public const string COLUMN_CUSTOMER_BILL_QUANTITY = "COLUMN_CUSTOMER_BILL_QUANTITY";
        public const string COLUMN_CUSTOMER_BALANCE = "COLUMN_CUSTOMER_BALANCE";
        public const string COLUMN_CUSTOMER_EMAIL = "COLUMN_CUSTOMER_EMAIL";
        public const string COLUMN_CUSTOMER_ADDRESS = "COLUMN_CUSTOMER_ADDRESS";

        #endregion

        #region Bill Management Detailed

        // Columns of TABLE_BILL_MANAGEMENT_DETAILED
        public const string PREFIX_BILL = "BILL_";
        public const string COLUMN_BILL_CODE = "COLUMN_BILL_CODE";
        //public const string COLUMN_PRODUCT_CODE = "COLUMN_PRODUCT_CODE";
        //public const string COLUMN_PRODUCT_NAME = "COLUMN_PRODUCT_NAME";
        //public const string COLUMN_PRODUCT_QUANTITY = "COLUMN_PRODUCT_QUANTITY";
        public const string COLUMN_PRODUCT_TOTAL_PRICE = "COLUMN_PRODUCT_TOTAL_PRICE";
        //public const string COLUMN_BILL_CREATED_DATE = "COLUMN_BILL_CREATED_DATE";

        #endregion

        #region Bill Management General

        // Columns of TABLE_BILL_MANAGEMENT_GENERAL
        //public const string COLUMN_BILL_CODE = "COLUMN_BILL_CODE";
        //public const string COLUMN_CUSTOMER_CODE = "COLUMN_CUSTOMER_CODE";
        //public const string COLUMN_CUSTOMER_NAME = "COLUMN_CUSTOMER_NAME";
        public const string COLUMN_BILL_MONEY = "COLUMN_BILL_MONEY";
        public const string COLUMN_PAID_MONEY = "COLUMN_PAID_MONEY";
        public const string COLUMN_BILL_CREATED_DATE = "COLUMN_BILL_CREATED_DATE";

        #endregion

        #endregion

    }
}
