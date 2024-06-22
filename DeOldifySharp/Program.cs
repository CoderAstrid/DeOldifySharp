//*************************************************************************************************
//* (C) ColorfulSoft corp., 2021 - 2022. All Rights reserved.
//*************************************************************************************************

using System;
using System.Windows.Forms;

namespace DeOldifySharp
{

    /// <summary>
    /// Main class.
    /// </summary>
    public static class Program
    {

        /// <summary>
        /// Entry point.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            // Reduce application CPU priority to avoid freezes during image processing
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass =
                System.Diagnostics.ProcessPriorityClass.BelowNormal;
            //DeOldify.Initialize();
            if(args.Length > 0)
            {
                return;
            }
            Application.EnableVisualStyles();
            try
            {
                //Application.Run(new MainForm());
                Application.Run(new DeOldifySharp.FrmMain());
            }
            catch
            {
            }
        }

    }

}
