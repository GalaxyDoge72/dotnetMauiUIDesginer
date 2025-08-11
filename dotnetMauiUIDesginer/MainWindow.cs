namespace dotnetMauiUIDesginer
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public Point getMouseRelativeToApp()
        {
            Point screenPos = Cursor.Position;

            Point formPos = this.PointToClient(screenPos);

            return formPos;
        }



        // Timer Events //
        private void oneMSTimer_Tick(object sender, EventArgs e)
        {
            Point formPos = getMouseRelativeToApp();

            if (this.ClientRectangle.Contains(formPos))
            {
                int x = formPos.X;
                int y = formPos.Y;

                string mouseLocationMSG = $"X: {x} | Y: {y}";
                mouseLocationLabel.Text = mouseLocationMSG;
            }
            else
            {
                mouseLocationLabel.Text = "Invalid Location";
            }
        }

        
    }
}
