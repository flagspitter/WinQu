namespace WinQu
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createdNew;
            const string mutexName = "WinQuMutex";

            using( var mutex = new Mutex( true, mutexName, out createdNew ) )
            {
                if( createdNew )
                {
                    // To customize application configuration such as set high DPI settings or default font,
                    // see https://aka.ms/applicationconfiguration.
                    ApplicationConfiguration.Initialize();
                    Application.Run(new WinQu());
                }
	            else
	            {
	                MessageBox.Show( "WinQu is already running.", "WinQu" );
	            }
            }
        }
    }
}
