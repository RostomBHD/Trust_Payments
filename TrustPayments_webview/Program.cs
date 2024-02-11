namespace TrustPayments_webview
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            //string sConnectionString = (String)Environment.GetCommandLineArgs().GetValue(1);

            //Console.WriteLine("connection string is " + sConnectionString);
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}