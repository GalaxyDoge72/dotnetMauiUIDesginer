using FastColoredTextBoxNS;
using FastColoredTextBoxNS.Text;
using FastColoredTextBoxNS.Types;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace dotnetMauiUIDesginer
{
    public partial class MainWindow : Form
    {
        private Control? currentControl;
        private TextBox nameTextBox;
        private Button applyButton;
        private TextBox xAxisBox;
        private FastColoredTextBox codeEditor;

        private readonly TextStyle classStyle = new TextStyle(Brushes.Purple, null, FontStyle.Regular);
        private readonly TextStyle commentStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        private readonly TextStyle typeStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        private readonly TextStyle conditionalStyle = new TextStyle(Brushes.Yellow, null, FontStyle.Regular);


        public MainWindow()
        {
            InitializeComponent();

            // Label for control name
            Label nameLabel = new Label
            {
                Location = new Point(10, 25),
                Text = "Name:",
                AutoSize = true,
                BackColor = this.BackColor
            };
            Controls.Add(nameLabel);

            // TextBox for editing control name
            nameTextBox = new TextBox
            {
                Location = new Point(60, 22),
                Width = 200
            };
            Controls.Add(nameTextBox);

            // Apply button to set the control's name and text
            applyButton = new Button
            {
                Text = "Apply",
                Location = new Point(270, 22)
            };
            applyButton.Click += ApplyButton_Click;
            Controls.Add(applyButton);

            // Additional text box placeholder (adjust or remove as needed)
            xAxisBox = new TextBox
            {
                Text = "",
                Location = new Point(60, 50),
                Width = 200
            };
            Controls.Add(xAxisBox);

            // Keep label background consistent when form background changes
            this.BackColorChanged += (s, e) =>
            {
                nameLabel.BackColor = this.BackColor;
            };

            // Setup FastColoredTextBox editor
            codeEditor = new FastColoredTextBox
            {
                Location = new Point(10, 90),
                Size = new Size(480, 300),
                Language = Language.CSharp,
                ShowLineNumbers = true,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Consolas", 10),
                AutoIndent = true,
                AutoIndentChars = true,
                PaddingBackColor = Color.LightGray
            };

            codeEditor.TextChanged += CodeEditor_TextChanged;

            Controls.Add(codeEditor);

            // Disable apply button until a control is selected
            applyButton.Enabled = false;
        }

        // Loads properties of the selected control into the UI
        public void LoadControlProperties(Control? control)
        {
            currentControl = control;

            if (control == null)
            {
                nameTextBox.Text = string.Empty;
                applyButton.Enabled = false;
            }
            else
            {
                nameTextBox.Text = control.Name ?? string.Empty;
                applyButton.Enabled = true;
            }
        }

        // Applies the name change to the currently selected control
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

        // FastColoredTextBox syntax highlighting - only in changed range!
        private void CodeEditor_TextChanged(object? sender, TextChangedEventArgs e)
        {
            // Clear previous styles only in changed range
            e.ChangedRange.ClearStyle(StyleIndex.All);

            // C# keywords regex (expand as needed)
            string classKeywords = @"\b(private|public|class)\b";
            string typeKeywords = @"\b(void|string|int|double|float)";
            string conditionalKeywords = @"\b(if|else|then|for|foreach|try|except|finally)\b";

            e.ChangedRange.SetStyle(classStyle, classKeywords);
            e.ChangedRange.SetStyle(typeStyle, typeKeywords);
            e.ChangedRange.SetStyle(commentStyle, @"//.*$", RegexOptions.Multiline);
        }

    }
}
