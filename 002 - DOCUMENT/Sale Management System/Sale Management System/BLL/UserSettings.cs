using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sale_Management_System
{
    public static class UserSettings
    {
        #region Sql Configuration

        // SQL CONNECTION CONFIGURATION
        public static bool SQL_AUTHENTICATION_METHOD = false;
        public static string SQL_USER_NAME = "";
        public static string SQL_PASSWORD = "";

        public static string SQL_DATASOURCE = "";
        public static string SQL_CATALOG = "";

        #endregion

        #region Printing Configuration
        
        // Paper Settings
        public static int PAPER_WIDTH = 0;
        public static int PAPER_HEIGHT = 0;
        public static string PAPER_NAME = "";

        // Printer
        public static string PRINTER_NAME = "";

        // List of Lines stored information of a company
        public static List<string> LinesList_CompanyInfo = new List<string>();

        
        #endregion
    }
}
