using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CustomDataGrid
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message, "Error - Unhandled Program Exception");
            }
        }
    }
}