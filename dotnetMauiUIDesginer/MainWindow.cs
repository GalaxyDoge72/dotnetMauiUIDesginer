namespace dotnetMauiUIDesginer
{
    public partial class MainWindow : Form
    {
        private Control? currentControl;
        private TextBox nameTextBox;
        private Button applyButton;
        private Panel canvasPanel;

        public MainWindow()
        {
            InitializeComponent();

            Label nameLabel = new Label
            {
                Location = new Point(10, 45),
                Text = "Name:",
                AutoSize = true,
                BackColor = this.BackColor
            };
            Controls.Add(nameLabel);

            nameTextBox = new TextBox
            {
                Location = new Point(60, 42),
                Width = 200
            };
            Controls.Add(nameTextBox);

            applyButton = new Button
            {
                Text = "Apply",
                Location = new Point(270, 42)
            };
            applyButton.Click += ApplyButton_Click;
            Controls.Add(applyButton);

            this.BackColorChanged += (s, e) =>
            {
                nameLabel.BackColor = this.BackColor;
            };
        }

        public void LoadControlProperties(Control? control)
        {
            currentControl = control;

            if (control == null)
            {
                nameTextBox.Text = string.Empty;
                applyButton.Enabled = false;  // disable apply button or other UI elements
            }
            else
            {
                nameTextBox.Text = control.Name ?? string.Empty;
                applyButton.Enabled = true;
            }
        }

        private void ApplyButton_Click(object? sender, EventArgs e)
        {
            if (currentControl != null)
            {
                currentControl.Name = nameTextBox.Text;
                currentControl.Text = nameTextBox.Text;
                MessageBox.Show($"Control name set to: {currentControl.Name}");
            }
            else
            {
                MessageBox.Show("No control selected.");
            }
        }
    }
}