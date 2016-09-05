using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

using System.Resources;
using System.Globalization;

namespace Sale_Management_System
{
    public partial class Form_LoginMenu : Form
    {

        // Configuration file name
        string szConfigFile = String.Format(@"{0}\{1}", Path.GetDirectoryName(Application.ExecutablePath), GlobalConstants.FILE_USER_SETTING) ;
        SqlConnection SqlConnectionId = null;

        #region Constructor
        
        public Form_LoginMenu()
        {
            InitializeComponent();

            // Load configuration file
            fnLoadConfigFile();

        }

        #endregion

        #region Events

        /*****************************************************************************************************/

        #region TextField KeyPress Events

        // This event is fired when user Press a key while his/her pointer is in a Text Field
        private void TextField_OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (sender != TXT_DataSource && sender != TXT_Catalog)
                return;

            // The pressed key is not Enter
            if (e.KeyChar != (char)13)
                return;

            fnDoLogin();

        }

        #endregion

        /*****************************************************************************************************/

        #region Menu ToolStrip Event

        // This event is fired when user click Exit button
        private void TSP_ProgramExit_Click(object sender, EventArgs e)
        {
            // Terminate this program
            Application.Exit();
        }

        #endregion

        /*****************************************************************************************************/

        #region Button Events

        // This event is fired when user clicks login button
        private void BTN_Login_Click(object sender, EventArgs e)
        {
            fnDoLogin();
        }

        #endregion

        /*****************************************************************************************************/

        #region Form Events

        // This event is fired when form has been alread closed
        private void Event_OnFormClosed(object sender, FormClosedEventArgs e)
        {
            // Terminate the program
            Application.Exit();
        }

        // This event is fired when this form is being closed
        private void Event_OnFormClosing(object sender, FormClosingEventArgs e)
        {
            // Display confirmation dialog
            if (MessageBox.Show("Bạn có thực sự muốn thoát ?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                e.Cancel = true;
        }

        #endregion

        /*****************************************************************************************************/

        #endregion

        #region Internal Function

        // Help user to login
        private void fnDoLogin()
        {

            #region Sql Stuff

            // Open a connection to Sale Management Database
            SqlConnectionId = util_Sql.fnConnectToDataBase(TXT_DataSource.Text, TXT_Catalog.Text, false, UserSettings.SQL_USER_NAME, UserSettings.SQL_PASSWORD);

            // Connection failed
            if (SqlConnectionId == null)
            {
                // Display error message box
                MessageBox.Show("Kết nối đến CSDL thất bại", "Lỗi", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;
            }

            #region Successful Connection | Create Database Tables

            util_Sql.fnCreateTableProduct(SqlConnectionId);
            util_Sql.fnCreateTableCustomer(SqlConnectionId);
            util_Sql.fnCreateTableBillGeneral(SqlConnectionId);
            util_Sql.fnCreateTableBillDetailed(SqlConnectionId);

            #endregion

            // Close this connection
            SqlConnectionId.Close();

            #endregion

            // Save information to configuration file when your loading is successful
            fnSaveConfigFile();

            // Display a message to let user know that 
            // The loading is successful

            // Update Datasource and Catalog constant
            UserSettings.SQL_DATASOURCE = TXT_DataSource.Text;
            UserSettings.SQL_CATALOG = TXT_Catalog.Text;

            // Load neccessary data
            fnLoadNeccessaryData();

            #region Redirect to Main Menu

            Form_MainMenu FormId = new Form_MainMenu();

            // Display Main Menu
            FormId.Show();

            // Hide Login Menu window
            this.Dispose();


            #endregion

        }

        // Load neccessary data
        private void fnLoadNeccessaryData()
        {
            #region StreamReader Initialization

            // Create an object of StreamReader to read data from files
            StreamReader StreamReaderId = null;

            try
            {
                StreamReaderId = new StreamReader(string.Format(@"{0}\{1}", Path.GetDirectoryName(Application.ExecutablePath), GlobalConstants.FILE_COMPANY_INFO));
            }
            catch
            {
               // Reading fail ?
                StreamReaderId = null;
            }

            #endregion

            #region Read File | Bind Lines to a List

            // If we can open that neccessary file ?
            if (StreamReaderId != null)
            {
                // Information of a line
                string szLineInfo = null;

                // Read the file line by line
                while ((szLineInfo = StreamReaderId.ReadLine()) != null)
                {
                    // Add to the list
                    UserSettings.LinesList_CompanyInfo.Add(szLineInfo);
                }

                // Close StreamReader for safety
                StreamReaderId.Close();

            }

            #endregion

        }

        // Load data from configuration file
        private void fnLoadConfigFile()
        {
            // Create a StreamReader to read configuration file
            StreamReader StreamReaderId = null;

            try
            {
                // Open file
                StreamReaderId = new StreamReader(szConfigFile);
            }
            catch
            {
                return;
            }

            // Store information of a line
            string szLine;

            // Read file line by line
            while ((szLine = StreamReaderId.ReadLine()) != null)
            {
                // An empty line ?
                if (szLine.Length < 1)
                    continue;

                if (!szLine.Contains('='))
                    continue;

                string[] szInfo = szLine.Split('=');

                if (szInfo[0].Equals(GlobalConstants.KEY_LANGUAGE))
                {
                    GlobalConstants.PROGRAM_LANGUAGE = szInfo[1];
                    GlobalConstants.PROGRAM_LANGUAGE.Trim();
                }
                else if (szInfo[0].Equals(GlobalConstants.KEY_SQL_DATABASE))
                    TXT_DataSource.Text = szInfo[1];
                else if (szInfo[0].Equals(GlobalConstants.KEY_SQL_CATALOG))
                    TXT_Catalog.Text = szInfo[1];
                else if (szInfo[0].Equals(GlobalConstants.KEY_PRINTER_NAME))
                    UserSettings.PRINTER_NAME = szInfo[1];
                else if (szInfo[0].Equals(GlobalConstants.KEY_PAPER_HEIGHT))
                    UserSettings.PAPER_HEIGHT = int.Parse(szInfo[1].Replace(" ", ""));
                else if (szInfo[0].Equals(GlobalConstants.KEY_PAPER_WIDTH))
                    UserSettings.PAPER_WIDTH = int.Parse(szInfo[1].Replace(" ", ""));
            }

            StreamReaderId.Close();
        }

        // Save information to configuration file when loading has completed !
        private void fnSaveConfigFile()
        {
            // Create a list of lines for reading from and writing data to file
            List<string> ListLineOld = new List<string>();
            List<string> ListLineNew = new List<string>();

            // Read data from file
            GlobalFunctions.fnFileReadAllLines(szConfigFile, ListLineOld);

            // Write data to list
            ListLineNew.Add(string.Format("{0}={1}", GlobalConstants.KEY_LANGUAGE, GlobalConstants.PROGRAM_LANGUAGE));
            ListLineNew.Add(string.Format("{0}={1}", GlobalConstants.KEY_SQL_DATABASE, TXT_DataSource.Text.Trim()));
            ListLineNew.Add(string.Format("{0}={1}", GlobalConstants.KEY_SQL_CATALOG, TXT_Catalog.Text.Trim()));

            foreach (string szLine in ListLineOld)
            {
                string[] szTokens = szLine.Trim().Split('=');

                // Skip old values
                if (szTokens[0].Trim().Equals(GlobalConstants.KEY_LANGUAGE))
                    continue;

                if (szTokens[0].Trim().Equals(GlobalConstants.KEY_SQL_DATABASE))
                    continue;

                if (szTokens[0].Trim().Equals(GlobalConstants.KEY_SQL_CATALOG))
                    continue;

                // Rewrite data (if they are not the data which has to be changed in this window)
                ListLineNew.Add(szLine.Trim());
            }
       
            // Rewrite file
            GlobalFunctions.fnFileWriteAllLines(szConfigFile, ListLineNew);
   
            // Destroy 2 list to save memory
            ListLineOld = null;
            ListLineNew = null;
        }

        #endregion

        /*****************************************************************************************************/
        

    }
}
