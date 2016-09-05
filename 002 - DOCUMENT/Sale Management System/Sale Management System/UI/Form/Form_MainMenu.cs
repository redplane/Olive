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

namespace Sale_Management_System
{
    public partial class Form_MainMenu : Form
    {
        #region Constructor

        public Form_MainMenu()
        {
            // Install all components
            InitializeComponent();

        }

        #endregion

        /********************************************************************************************************/

        #region Button Events

        //  This event is fired when user click Customer Management
        private void BTN_FormCustomer_Click(object sender, EventArgs e)
        {
            // Display Customer Management Form
            Form_CustomerManagement FormId = new Form_CustomerManagement();

            // Hide Main Menu window
            this.Dispose();

            // Show the menu
            FormId.Show();

            
        }
        
        // This event is fired when use click Product Mangement button
        private void BTN_FormProduct_Click(object sender, EventArgs e)
        {
            Form_ProductManagement FormId = new Form_ProductManagement();

            // Hide Main Menu window
            this.Dispose();

            // Display Product Management Form
            FormId.Show();
        }

        // This event is fired when user click Bill Management button
        private void BTN_FormBill_Click(object sender, EventArgs e)
        {
            // Create an object of Bill Management form
            Form_BillManagement FormId = new Form_BillManagement();

            // Hide current form
            this.Dispose();

            // Show Bill Management window
            FormId.Show();
        }

        // This event is fired when user clicks Back button
        private void BTN_BackClicks(object sender, EventArgs e)
        {
            // Create an object of Font
            Font FontId = new Font(GlobalConstants.PROGRAM_FONT, GlobalConstants.PROGRAM_FONT_SIZE);

            // Display confirmation dialog
            if (MessageBox.Show("Bạn có muốn đăng xuất ?", "Cảnh báo", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;

            // Reset User Setting
            UserSettings.SQL_AUTHENTICATION_METHOD = false;
            UserSettings.SQL_CATALOG = null;
            UserSettings.SQL_DATASOURCE = null;
            UserSettings.SQL_PASSWORD = null;
            UserSettings.SQL_USER_NAME = null;

            // Create an instance of Login Form
            Form_LoginMenu FormId = new Form_LoginMenu();

            // Make it appear at the center of user's screen
            FormId.StartPosition = FormStartPosition.CenterScreen;

            // Dispose this form
            this.Dispose();

            // Show Login Form
            FormId.Show();
        }


        #endregion

        /********************************************************************************************************/

        #region MenuToolStrip Events
        
        // This event is fired when user click EXIT button in Menu Strip toolbar
        private void TSP_MenuExit_Click(object sender, EventArgs e)
        {
            // Close this windows
            this.Close();
        }

        #endregion

        /********************************************************************************************************/

        #region Form Events

        private void Event_OnFormClosing(object sender, FormClosingEventArgs e)
        {
            // Display confirmation message
            if (MessageBox.Show("Bạn có muốn đóng chương trình ?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                e.Cancel = true;
        }

        // This event is fired when Main Menu is closed
        private void Event_OnFormClosed(object sender, FormClosedEventArgs e)
        {
            // Terminate this program
            Application.Exit();
        }

        #endregion

        /********************************************************************************************************/

    }
}
