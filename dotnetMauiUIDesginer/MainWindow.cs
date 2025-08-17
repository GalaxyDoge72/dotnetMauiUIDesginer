using FastColoredTextBoxNS;
using FastColoredTextBoxNS.Text;
using FastColoredTextBoxNS.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace dotnetMauiUIDesginer
{
    public partial class MainWindow : Form
    {
        private Control? currentControl;
        private TextBox nameTextBox;
        private TextBox xAxisBox;
        private FastColoredTextBox codeEditor;
        private FastColoredTextBox xamlEditor;
        private Canvas _canvasForm;
        private TabControl codeTabs;

        // Mapping from Buttons to their click handler names
        private Dictionary<Button, string> buttonHandlers = new();

        // XAML Formatting Styles
        private readonly TextStyle attributeStyle = new TextStyle(Brushes.DarkRed, null, FontStyle.Regular);
        private readonly TextStyle tagStyle = new TextStyle(Brushes.Blue, null, FontStyle.Bold);
        private readonly TextStyle commentXAMLStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);

        // C# Code Styling
        private readonly TextStyle classStyle = new TextStyle(Brushes.Purple, null, FontStyle.Regular);
        private readonly TextStyle commentStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        private readonly TextStyle typeStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);

        public MainWindow(Canvas canvasForm)
        {
            InitializeComponent();
            _canvasForm = canvasForm;

            _canvasForm.ControlSelectedForProperties += ctrl =>
            {
                LoadControlProperties(ctrl);

                if (ctrl != null)
                    xAxisBox.Text = $"X={ctrl.Location.X}";
                else
                    xAxisBox.Text = string.Empty;

                GenerateButtonClickHandler(ctrl);
            };

            _canvasForm.canvasChanged += () => updateXamlLayout();

            SetupControls();
            SetupEditors();
            SetupTabs();
        }

        private void SetupControls()
        {
            // Label for control name
            Label nameLabel = new Label
            {
                Location = new Point(10, 25),
                Text = "Name:",
                AutoSize = true,
                BackColor = this.BackColor
            };
            Controls.Add(nameLabel);

            // Name TextBox
            nameTextBox = new TextBox { Location = new Point(60, 22), Width = 200 };
            nameTextBox.TextChanged += NameTextBox_TextChanged;
            Controls.Add(nameTextBox);

            // X-axis TextBox
            xAxisBox = new TextBox { Location = new Point(60, 50), Width = 200 };
            Controls.Add(xAxisBox);

            // Keep label background consistent
            this.BackColorChanged += (s, e) => { nameLabel.BackColor = this.BackColor; };
        }

        private void NameTextBox_TextChanged(object? sender, EventArgs e)
        {
            if (currentControl == null) return;

            string newName = nameTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(newName) && !_canvasForm.IsControlNameDuplicateWithOther(currentControl, newName))
            {
                string oldName = currentControl.Name;
                currentControl.Name = newName;

                // Update button handler name if it's a button
                if (currentControl is Button btn)
                {
                    UpdateButtonClickHandlerName(btn, oldName);
                }

                updateXamlLayout();
            }
        }

        private void SetupTabs()
        {
            codeTabs = new TabControl
            {
                Location = new Point(10, 80),
                Size = new Size(620, 500)
            };
            codeTabs.SelectedIndexChanged += updateXamlLayout;
            Controls.Add(codeTabs);

            TabPage codeEditorPage = new TabPage("Code Editor");
            TabPage xamlEditorPage = new TabPage("XAML Code");
            codeTabs.Controls.Add(codeEditorPage);
            codeTabs.Controls.Add(xamlEditorPage);

            codeEditorPage.Controls.Add(codeEditor);
            xamlEditorPage.Controls.Add(xamlEditor);
        }

        private void SetupEditors()
        {
            codeEditor = new FastColoredTextBox
            {
                Size = new Size(610, 490),
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

            xamlEditor = new FastColoredTextBox
            {
                Dock = DockStyle.Fill,
                ShowLineNumbers = true,
                Font = new Font("Consolas", 10)
            };
            xamlEditor.TextChanged += xamlEditor_TextChanged;
        }

        public void LoadControlProperties(Control? control)
        {
            currentControl = control;
            nameTextBox.Text = control?.Name ?? string.Empty;
        }

        private void updateXamlLayout(object? sender, EventArgs e) => updateXamlLayout();

        private void updateXamlLayout()
        {
            if (codeTabs.SelectedTab?.Text != "XAML Code") return;

            string generatedXaml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n" +
                                   genXAMLLayout(_canvasForm.canvasPanel, 0);
            xamlEditor.Text = generatedXaml;
        }

        private void CodeEditor_TextChanged(object? sender, TextChangedEventArgs e)
        {
            e.ChangedRange.ClearStyle(StyleIndex.All);
            e.ChangedRange.SetStyle(classStyle, @"\b(private|public|class)\b");
            e.ChangedRange.SetStyle(typeStyle, @"\b(void|string|int|double|float)\b");
            e.ChangedRange.SetStyle(commentStyle, @"//.*$", RegexOptions.Multiline);
        }

        private void xamlEditor_TextChanged(object? sender, TextChangedEventArgs e)
        {
            e.ChangedRange.ClearStyle(StyleIndex.All);
            e.ChangedRange.SetStyle(tagStyle, @"<[/]?[A-Za-z0-9\.:]+", RegexOptions.Singleline);
            e.ChangedRange.SetStyle(attributeStyle, @"\b[A-Za-z0-9_]+\s*=\s*""[^""]*""");
            e.ChangedRange.SetStyle(commentXAMLStyle, @"", RegexOptions.Singleline);
        }

        private string genXAMLLayout(Control ctrl, int indentLvl)
        {
            var sb = new StringBuilder();
            string indent = new string(' ', indentLvl * 4);
            int x = ctrl.Left;
            int y = ctrl.Top;

            string typeName = ctrl.GetType().Name;
            string controlName = ctrl.Name;

            if (string.IsNullOrEmpty(controlName) || _canvasForm.IsControlNameDuplicate(ctrl))
            {
                int typeCount = _canvasForm.CountControlsOfType(ctrl.GetType());
                controlName = $"{typeName}{typeCount + 1}";
                ctrl.Name = controlName;
            }

            if (ctrl is Panel)
            {
                sb.AppendLine($"{indent}<StackLayout>");
                foreach (Control child in ctrl.Controls)
                    sb.Append(genXAMLLayout(child, indentLvl + 1));
                sb.AppendLine($"{indent}</StackLayout>");
            }
            else if (ctrl is Button)
            {
                string handlerName = buttonHandlers.ContainsKey((Button)ctrl)
                    ? buttonHandlers[(Button)ctrl]
                    : controlName + "_Click";

                sb.AppendLine($"{indent}<Button x:Name=\"{controlName}\" Text=\"{ctrl.Text}\" Margin=\"{x}, {y}, 0, 0\" Clicked=\"{handlerName}\" />");
            }
            else if (ctrl is Label)
            {
                sb.AppendLine($"{indent}<Label x:Name=\"{controlName}\" Text=\"{ctrl.Text}\" Margin=\"{x}, {y}, 0, 0\" />");
            }

            return sb.ToString();
        }

        private void GenerateButtonClickHandler(Control ctrl)
        {
            if (ctrl is not Button button) return;

            string handlerName = buttonHandlers.ContainsKey(button)
                ? buttonHandlers[button]
                : button.Name + "_Click";

            if (!codeEditor.Text.Contains($"void {handlerName}("))
            {
                codeEditor.AppendText($"\nprivate void {handlerName}(object sender, EventArgs e)\n{{\n    // TODO: Add logic for {button.Name}\n}}\n");
                buttonHandlers[button] = handlerName;
            }
        }

        private void UpdateButtonClickHandlerName(Button button, string oldName)
        {
            if (!buttonHandlers.TryGetValue(button, out string oldHandlerName))
            {
                oldHandlerName = oldName + "_Click";
            }

            string newHandlerName = button.Name + "_Click";

            if (oldHandlerName == newHandlerName) return;

            // Replace the old handler name in the code editor
            if (!string.IsNullOrEmpty(oldHandlerName) && codeEditor.Text.Contains(oldHandlerName))
            {
                codeEditor.Text = codeEditor.Text.Replace(oldHandlerName, newHandlerName);
            }

            // Update the mapping
            buttonHandlers[button] = newHandlerName;

            // If no stub exists, create it
            if (!codeEditor.Text.Contains($"void {newHandlerName}("))
            {
                codeEditor.AppendText($"\nprivate void {newHandlerName}(object sender, EventArgs e)\n{{\n    // TODO: Add logic for {button.Name}\n}}\n");
            }
        }
    }
}
