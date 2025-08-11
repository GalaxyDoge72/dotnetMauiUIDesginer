using System.Drawing;
using System.Windows.Forms;

namespace dotnetMauiUIDesginer
{
    public partial class Canvas : Form
    {
        private Panel canvasPanel;

        public Canvas()
        {
            InitializeComponent();

            canvasPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                AllowDrop = true
            };

            canvasPanel.DragEnter += CanvasPanel_DragEnter;
            canvasPanel.DragDrop += CanvasPanel_DragDrop;

            Controls.Add(canvasPanel);
        }

        private void CanvasPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void CanvasPanel_DragDrop(object sender, DragEventArgs e)
        {
            string controlType = (string)e.Data.GetData(typeof(string));
            Point clientPoint = canvasPanel.PointToClient(new Point(e.X, e.Y));
            AddControlToCanvas(controlType, clientPoint);
        }

        private void AddControlToCanvas(string controlType, Point location)
        {
            Control newControl = controlType switch
            {
                "Button" => new Button { Text = "Button" },
                "Label" => new Label { Text = "Label" },
                _ => null
            };

            if (newControl != null)
            {
                newControl.Location = location;
                newControl.AutoSize = true;
                EnableDragMove(newControl);
                canvasPanel.Controls.Add(newControl);
            }
        }

        private void EnableDragMove(Control control)
        {
            bool dragging = false;
            Point dragStart = Point.Empty;

            control.MouseDown += (s, e) =>
            {
                dragging = true;
                dragStart = e.Location;
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
    }
}
