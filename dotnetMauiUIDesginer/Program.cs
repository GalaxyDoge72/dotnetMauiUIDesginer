using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace dotnetMauiUIDesginer
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();

            var canvas = new Canvas();
            var mainWindow = new MainWindow();
            var toolbox = new toolboxForm();

            int toolboxWidth = 250;
            int toolboxHeight = 200;

            int mainWindowWidth = 250;
            int mainWindowHeight = 200;

            canvas.StartPosition = FormStartPosition.Manual;
            canvas.Location = new Point(500, 200);
            canvas.Size = new Size(800, 600);

            toolbox.StartPosition = FormStartPosition.Manual;
            toolbox.Size = new Size(toolboxWidth, toolboxHeight);
            toolbox.Location = new Point(canvas.Left - toolboxWidth, canvas.Top);

            mainWindow.StartPosition = FormStartPosition.Manual;
            mainWindow.Size = new Size(mainWindowWidth, mainWindowHeight);
            mainWindow.Location = new Point(canvas.Left - mainWindowWidth, canvas.Top + toolboxHeight);

            toolbox.Show();
            mainWindow.Show();

            canvas.LocationChanged += (s, e) =>
            {
                toolbox.Location = new Point(canvas.Left - toolboxWidth, canvas.Top);
                mainWindow.Location = new Point(canvas.Left - mainWindowWidth, canvas.Top + toolboxHeight);
            };

            Application.Run(canvas);
        }

    }
}