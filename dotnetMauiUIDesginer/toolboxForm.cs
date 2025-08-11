using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace dotnetMauiUIDesginer
{
    public partial class toolboxForm : Form
    {
        private ListView listView;

        public toolboxForm()
        {
            InitializeComponent();
            SetupToolbox();
        }

        private void SetupToolbox()
        {
            int iconSize = 48;
            ImageList imageList = new ImageList
            {
                ImageSize = new Size(iconSize, iconSize),
                ColorDepth = ColorDepth.Depth32Bit
            };

            string basePath = Path.Combine(Application.StartupPath, "Assets", "Icons");

            imageList.Images.Add("button", LoadAndResizeIcon(Path.Combine(basePath, "button.png"), iconSize));
            imageList.Images.Add("label", LoadAndResizeIcon(Path.Combine(basePath, "label.png"), iconSize));

            listView = new ListView
            {
                View = View.LargeIcon,
                LargeImageList = imageList,
                Dock = DockStyle.Fill
            };

            listView.Items.Add(new ListViewItem("Button") { ImageKey = "button" });
            listView.Items.Add(new ListViewItem("Label") { ImageKey = "label" });

            listView.MouseDown += (s, e) =>
            {
                var hit = listView.HitTest(e.Location);
                if (hit.Item != null)
                {
                    listView.DoDragDrop(hit.Item.Text, DragDropEffects.Copy);
                }
            };

            Controls.Add(listView);
        }

        private Image LoadAndResizeIcon(string filePath, int size)
        {
            if (!File.Exists(filePath)) return null;

            using var original = Image.FromFile(filePath);
            return ResizeImageWithAspect(original, size);
        }

        private Image ResizeImageWithAspect(Image original, int size)
        {
            int originalWidth = original.Width;
            int originalHeight = original.Height;

            float ratio = Math.Min((float)size / originalWidth, (float)size / originalHeight);

            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            Bitmap resized = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(resized))
            {
                g.Clear(Color.Transparent);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                int offsetX = (size - newWidth) / 2;
                int offsetY = (size - newHeight) / 2;

                g.DrawImage(original, offsetX, offsetY, newWidth, newHeight);
            }

            return resized;
        }
    }
}
