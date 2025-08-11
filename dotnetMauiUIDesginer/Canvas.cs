using System;
using System.Drawing;
using System.Windows.Forms;

namespace dotnetMauiUIDesginer
{
    public partial class Canvas : Form
    {
        private Panel canvasPanel;
        public event Action<Control>? ControlSelectedForProperties;

        private ContextMenuStrip controlContextMenu;
        private Control? contextMenuTargetControl;

        public Canvas()
        {
            InitializeComponent();

            canvasPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                AllowDrop = true
            };
            Controls.Add(canvasPanel);

            canvasPanel.DragEnter += CanvasPanel_DragEnter;
            canvasPanel.DragDrop += CanvasPanel_DragDrop;

            Controls.Add(canvasPanel);
            InitializeContextMenu();
        }

        private void canvasPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)))
                e.Effect = DragDropEffects.Copy;
        }

        private void canvasPanel_DragDrop(object sender, DragEventArgs e)
        {
            string controlType = (string)e.Data.GetData(typeof(string));
            addControlToCanvas(controlType, e.X, e.Y);
        }

        private void addControlToCanvas(string controlType, int screenX, int screenY)
        {
            Control? newControl = controlType switch
            {
                "Button" => new Button { Text = "Button" },
                "Label" => new Label { Text = "Label" },
                _ => null
            };

            if (newControl != null)
            {
                Point clientPoint = canvasPanel.PointToClient(new Point(screenX, screenY));
                newControl.Location = clientPoint;
                newControl.AutoSize = true;
                EnableDragMove(newControl);

                newControl.ContextMenuStrip = canvasContextMenu;
                canvasPanel.Controls.Add(newControl);
            }
        }

        private void enableDragMove(Control control)
        {
            bool dragging = false;
            Point dragStart = Point.Empty;

            control.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    dragging = true;
                    dragStart = e.Location;
                }
            };

            control.MouseMove += (s, e) =>
            {
                if (dragging && e.Button == MouseButtons.Left)
                {
                    control.Left += e.X - dragStart.X;
                    control.Top += e.Y - dragStart.Y;
                }
            };

            control.MouseUp += (s, e) => dragging = false;
        }
        // Drag and drop Functions //

        // Context Menu Functions //
        private void InitializeContextMenu()
        {
            canvasContextMenu = new ContextMenuStrip();
            canvasContextMenu.Items.Add("Delete", null, DeleteMenuItem_Click);
            canvasContextMenu.Items.Add("Properties", null, PropertiesMenuItem_Click);
        }

        private void EditPropertiesItem_Click(object? sender, EventArgs e)
        {
            if (contextMenuTargetControl != null)
            {
                ControlSelectedForProperties?.Invoke(contextMenuTargetControl);
            }
        }

        private void DeleteItem_Click(object? sender, EventArgs e)
        {
            if (contextMenuTargetControl != null)
            {
                Control selected = canvasContextMenu.SourceControl;
                MessageBox.Show($"Properties for {selected.GetType().Name}:\nLocation: {selected.Location}");
            }
        }
    }
}
