using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Sale_Management_System
{
    public partial class SplashScreen : Form
    {
        #region Constant

        private string SPLASH_IMAGE = "SplashScreen.jpg";

        #endregion

        public SplashScreen()
        {
            InitializeComponent();

            #region Set Background Image

            // Build SpashScreen Image path
            string szSplashImageUrl = Path.GetDirectoryName(Application.ExecutablePath) + @"\" + SPLASH_IMAGE;

            // File validation
            if (!File.Exists(szSplashImageUrl))
            {
                // Make this form invisible
                this.Visible = false;

                // Display warning message
                MessageBox.Show(string.Format("Tập tin {0} không tồn tại", szSplashImageUrl), "", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else
            {
                // Set background image
                this.BackgroundImage = Image.FromFile(szSplashImageUrl);
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }

            // Make this form be unable to show in taskbar
            this.ShowInTaskbar = false;

            #endregion
        }

        #region Form Event

        // This event is fired form already shown out
        private void Event_OnFormShown(object sender, EventArgs e)
        {
            // Wait for 5 secs
            Thread.Sleep(2000);

            // Display Start Form
            Form_LoginMenu Form_LoginMenuId = new Form_LoginMenu();

            // Hide current form
            this.Hide();

            // Display Start Form
            Form_LoginMenuId.Show();
        }

        #endregion

    }
}
