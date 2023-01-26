using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BackgroundQuotes
{
    public partial class Form1 : Form
    {
        private const int startTextSize = 100;
        private const string fontName = "Segoe print";
        private Random rnd = new Random();
        private List<string> logs = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void selectImagesDir(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog(this);
            tbImagePath.Text = folderBrowserDialog1.SelectedPath;
        }

        private void btnSelectQuotesFile(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog(this);
            tbQuotesPath.Text = openFileDialog1.FileName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IEnumerable<string> quotes = File.ReadLines(tbQuotesPath.Text);
            DirectoryInfo dirInfo = new DirectoryInfo(tbImagePath.Text);
            FileInfo[] imageFileInfos = dirInfo.GetFiles().Where(file => (file.Attributes & FileAttributes.Hidden) == 0)
                .ToArray();
            int index = 0;
            foreach (string quote in quotes)
            {
                if (imageFileInfos.Length > index)
                {
                    CreateNewBackground(imageFileInfos[index++].Name, quote, rnd);
                }
            }
            if (logs.Count>0) {
                string log = string.Join("\n", logs);
                MessageBox.Show(log);
            }
        }

        void CreateNewBackground(string fileName, string message, Random random)
        { 

            Image image=null;
            Graphics graphics = null;
            try
            {
                 image = Image.FromFile(Path.Combine(tbImagePath.Text, fileName));
                graphics = Graphics.FromImage(image);
                int x = 0;
                int y = 0;
                int width = image.Width;
                int hight = Convert.ToInt32(image.Height * 0.25);
                Font font = getFont(graphics, message, hight, width);

                SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
                graphics.FillRectangle(semiTransBrush, x, y, width, hight);
                graphics.DrawString(message, font,
                    new SolidBrush(Color.FromArgb(random.Next(150, 255), random.Next(150, 255), random.Next(150, 255))),
                    new RectangleF(x, y, width, hight),
                    new StringFormat(StringFormatFlags.LineLimit));
                image.Save(Path.Combine(tbOutputDir.Text, fileName), ImageFormat.Jpeg);
                graphics.Dispose();
                image.Dispose();
            }
            catch (Exception e) {
                logs.Add(fileName + " " + e.Message);
                graphics?.Dispose();
                image?.Dispose();
            }
        }


        Font getFont(Graphics graphics, string message, int hight, int width)
        {
            Font font = new Font(fontName, startTextSize, FontStyle.Regular);
            SizeF stringSize = graphics.MeasureString(message, font);
            while (hight < stringSize.Height * (stringSize.Width / width + 1))
            {
                font = new Font(fontName, font.Size - 1.0F, FontStyle.Regular);
                stringSize = graphics.MeasureString(message, font);
            }

            return font;
        }

        private void btnOutputDir(object sender, EventArgs e)
        {
            folderBrowserDialog2.ShowDialog(this);
            tbOutputDir.Text = folderBrowserDialog2.SelectedPath;
        }
    }
}