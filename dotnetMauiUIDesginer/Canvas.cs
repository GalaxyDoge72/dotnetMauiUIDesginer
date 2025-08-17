using System;
using System.Drawing;
using System.Windows.Forms;

namespace dotnetMauiUIDesginer
{
    public partial class Canvas : Form
    {
        private Panel _canvasPanel;
        public event Action<Control>? ControlSelectedForProperties;
        public event Action? canvasChanged;

        private ContextMenuStrip controlContextMenu;
        private Control? contextMenuTargetControl;

        public Panel canvasPanel { get; private set; }

        public Canvas()
        {
            InitializeComponent();

            _canvasPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                AllowDrop = true
            };

            this.canvasPanel = _canvasPanel;

            Controls.Add(_canvasPanel);

            // Drag & drop events
            _canvasPanel.DragEnter += _canvasPanel_DragEnter;
            _canvasPanel.DragDrop += _canvasPanel_DragDrop;

            // Setup context menu
            controlContextMenu = new ContextMenuStrip();

            var editPropertiesItem = new ToolStripMenuItem("Edit Properties");
            editPropertiesItem.Click += EditPropertiesItem_Click;
            controlContextMenu.Items.Add(editPropertiesItem);

            var deleteItem = new ToolStripMenuItem("Delete");
            deleteItem.Click += DeleteItem_Click;
            controlContextMenu.Items.Add(deleteItem);
        }

        private void _canvasPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)))
                e.Effect = DragDropEffects.Copy;
        }

        private void _canvasPanel_DragDrop(object sender, DragEventArgs e)
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
                Point clientPoint = _canvasPanel.PointToClient(new Point(screenX, screenY));
                newControl.Location = clientPoint;
                newControl.AutoSize = true;

                enableDragMove(newControl);
                AttachSelectionHandler(newControl);
                AttachContextMenu(newControl);

                _canvasPanel.Controls.Add(newControl);
                canvasChanged?.Invoke();
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

            control.MouseUp += (s, e) =>
            {
                dragging = false;

                // Notify that the canvas changed
                canvasChanged?.Invoke();
            };
        }


        private void AttachSelectionHandler(Control ctrl)
        {
            ctrl.Click += (s, e) =>
            {
                // Only left-click triggers selection event
                if (e is MouseEventArgs me && me.Button == MouseButtons.Left)
                {
                    ControlSelectedForProperties?.Invoke(ctrl);
                }
            };
        }

        private void AttachContextMenu(Control ctrl)
        {
            ctrl.MouseUp += (s, e) =>
            {
                if (e is MouseEventArgs me && me.Button == MouseButtons.Right)
                {
                    contextMenuTargetControl = ctrl;
                    controlContextMenu.Show(ctrl, me.Location);
                }
            };
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
                // Remove from parent (_canvasPanel)
                _canvasPanel.Controls.Remove(contextMenuTargetControl);
                canvasChanged?.Invoke();

                // Optionally, reset selection so MainWindow knows no control is selected
                ControlSelectedForProperties?.Invoke(null);


                contextMenuTargetControl = null;
            }
        }

        public int CountControlsOfType(Type type)
        {
            int count = 0;
            foreach (Control c in canvasPanel.Controls)
                if (c.GetType() == type) count++;
            return count;
        }

        public bool IsControlNameDuplicate(Control ctrl)
        {
            foreach (Control c in canvasPanel.Controls)
            {
                if (c != ctrl && c.Name == ctrl.Name)
                    return true;
            }
            return false;
        }

        public bool IsControlNameDuplicateWithOther(Control ctrlToIgnore, string name)
        {
            foreach (Control c in canvasPanel.Controls)
            {
                if (c != ctrlToIgnore && c.Name == name)
                    return true;
            }
            return false;
        }
    }
}
