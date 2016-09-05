using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Resources;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing;

namespace Sale_Management_System
{
    class GlobalFunctions
    {
        #region Code Generation

        // Auto generate code
        // This function help us to automatically generate a Code for customer
        public static string fnGenerateCode(string szPrefix)
        {
            // Standard : PREFIX_XXX-YYY
            // XXX : Characters (form A -> Z)
            // YYY : Number from 1 -> 9999

            Random GeneratorId = new Random();
            int iResult = GeneratorId.Next(1, 9999);

            string szResult = szPrefix;
            szResult += String.Format("{0}{1}{2}-", (char)GeneratorId.Next(65, 90), (char)GeneratorId.Next(65, 90), (char)GeneratorId.Next(65, 90));

            szResult += iResult;
            return szResult;
        }

        #endregion

        #region File Reading / Writing Stuffs

        // This function helps us to Read all line from a file
        // And put them to a list
        public static void fnFileReadAllLines(string szFileName, List<string> ListLines)
        {
            try
            {
                // Create an object of StreamReader to read data from file
                StreamReader StreamReaderId = new StreamReader(szFileName, Encoding.Unicode);

                // Create an object of string to read file line by line
                string szLine = null;

                while ((szLine = StreamReaderId.ReadLine()) != null)
                    ListLines.Add(szLine);

                // Close StreamReader for safety
                StreamReaderId.Close();
            }
            catch
            {
                // Reading fail
                return;
            }

        }

        // This function helps us to Write all line to a file from a list of strings
        public static void fnFileWriteAllLines(string szFileName, List<string> ListLines)
        {
            try
            {
                // Create an object of StreamWriter to write data to a file
                StreamWriter StreamWriterId = new StreamWriter(new FileStream(szFileName, FileMode.Create, FileAccess.Write), Encoding.Unicode);

                if (ListLines.Capacity < 1)
                {
                    StreamWriterId.Write("");
                }
                else
                {
                    foreach (string szLine in ListLines)
                    {
                        if (szLine == null)
                            continue;

                        StreamWriterId.WriteLine(szLine);
                    }
                }
                // Close StreamWriter for safety
                StreamWriterId.Close();
            }
            catch
            {
                return;
            }
        }

        #endregion

        #region Form Utilities

        // This function helps us to set background to a form
        public static bool fn_SetFormBackground(Form FormId, string szImages)
        {
            string szImageFilePath = Path.GetDirectoryName(Application.ExecutablePath) + @"\" + GlobalConstants.DIRECTORY_BACKGROUND_IMAGE + @"\" + szImages;

            // File doesn't exist
            if (!File.Exists(szImageFilePath))
                return false;
            
            try
            {
                // Set image to form
                FormId.BackgroundImage = Image.FromFile(szImageFilePath);
                FormId.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch
            {
            }

            return true;
        }

        #endregion

        #region Button Utilities

        public static bool fn_SetButtonBackgroundImage(Button BTN_ButtonId, string szImage)
        {
            string szImageFilePath = Path.GetDirectoryName(Application.ExecutablePath) + @"\" + GlobalConstants.DIRECTORY_BACKGROUND_IMAGE + @"\" + szImage;

            // File doesn't exist
            if (!File.Exists(szImageFilePath))
                return false;

            try
            {
                // Set image to form
                BTN_ButtonId.BackgroundImage = Image.FromFile(szImageFilePath);
                BTN_ButtonId.BackgroundImageLayout = ImageLayout.Stretch;

                BTN_ButtonId.BackColor = Color.Transparent;
                BTN_ButtonId.FlatStyle = FlatStyle.Flat;

                BTN_ButtonId.FlatAppearance.MouseDownBackColor = Color.Transparent;
                BTN_ButtonId.FlatAppearance.MouseOverBackColor = Color.Transparent;
                
            }
            catch
            {
            }

            return true;
        }
        
        #endregion

        #region PictureBox Utilities

        public static bool fn_SetPictureBoxImage(PictureBox PBX_PictureBoxId, string szDirectory, string szImage)
        {
            string szImageFilePath = Path.GetDirectoryName(Application.ExecutablePath) + @"\" + szDirectory + @"\" + szImage;

            // File doesn't exist
            if (!File.Exists(szImageFilePath))
                return false;

            try
            {
                // Set image to form
                PBX_PictureBoxId.BackgroundImage = Image.FromFile(szImageFilePath);
                PBX_PictureBoxId.BackgroundImageLayout = ImageLayout.Stretch;

            }
            catch
            {
            }

            return true;
        }

        #endregion
    }
}
