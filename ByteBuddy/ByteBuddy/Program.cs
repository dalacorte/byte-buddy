using ByteBuddy.DLL;
using ByteBuddy.Utils;

namespace ByteBuddy
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            EnableConsole();
            DatabaseUtils.CreateDatabase();
            DatabaseUtils.CreateTable();
            //DatabaseUtils.InitialPayload();
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

        private static void EnableConsole()
        {
#if DEBUG
            Win32.AllocConsole();
#endif
        }
    }
}