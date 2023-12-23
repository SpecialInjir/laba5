using AForge.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml.Linq;
using Лаба_5.DatabaseModule;
using Лаба_5.ImgAnalyzerModule;
using Лаба_5.ImgPrepareModule;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace CG_Lab5
{
    public partial class GrafficResultForm : Form
    {
        private Bitmap _bitmap;
        private Graphics _graphics;
        private Color _bgColor = Color.White;

        private ImagePreparator _imagePreparator;
        private DbModule _db = new();
        private ImageAnalyzer _analyzer = new();

        private int _matrixSize = 1;
        private int _monochromeBound = 128;

        public GrafficResultForm()
        {
            InitializeComponent();

            _imagePreparator = new(_matrixSize, _monochromeBound);

            InitCanvas();
        }

        private void InitCanvas()
        {
            Rectangle rectangle = pictureBox1.ClientRectangle;
            _bitmap = new Bitmap(rectangle.Width, rectangle.Height);
            _graphics = Graphics.FromImage(_bitmap);
            pictureBox1.Image = _bitmap;
            _graphics.Clear(_bgColor);
        }

        private void choosePhotoBtn_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadImage(dialog.FileName);
            }
        }

        private void LoadImage(string fileName)
        {
            try
            {
                var loadedBitmap = new Bitmap(fileName);
                LoadBitmap(loadedBitmap);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке изображения: {ex.Message}");
            }
        }

        private void grayscaleBtn_Click(object sender, EventArgs e)
        {
            var newBitmap = _imagePreparator.ConvertToGrayscale(_bitmap);

            LoadBitmap(newBitmap);
        }

        private void LoadBitmap(Bitmap newBitmap)
        {
            _bitmap = newBitmap;
            pictureBox1.Image = _bitmap;
            _graphics = Graphics.FromImage(_bitmap);
        }

        private void LoadBitmap2(Bitmap newBitmap)
        {
            _bitmap = newBitmap;
            pictureBox2.Image = _bitmap;
            _graphics = Graphics.FromImage(_bitmap);
        }

        private void LoadBitmap3(Bitmap newBitmap)
        {
            _bitmap = newBitmap;
            pictureBox3.Image = _bitmap;
            _graphics = Graphics.FromImage(_bitmap);
        }

        private void LoadBitmap4(Bitmap newBitmap)
        {
            _bitmap = newBitmap;
            pictureBox4.Image = _bitmap;
            _graphics = Graphics.FromImage(_bitmap);
        }


        private void addImageBtn_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var addedImage = new Bitmap(dialog.FileName);
                _db.AddImage(dialog.SafeFileName, addedImage);
            }
            _db.SaveHashesToFile("C:\\Users\\uzden\\Documents\\CG_Lab5\\Resources\\Хэши.bin");
        }

        private void loadImagesBtn_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _db.LoadHashesFromFile(dialog.FileName);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using SaveFileDialog dialog = new();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _db.SaveHashesToFile(dialog.FileName);
            }
        }

        private void analyzeBtn_Click(object sender, EventArgs e)
        {
            _db.LoadHashesFromFile("C:\\Users\\uzden\\Documents\\lab5\\Хэши.bin");
            var newb = _imagePreparator.ConvertToGrayscale(_bitmap);
            LoadBitmap(newb);
            var n = _imagePreparator.ConvertToMonochrome(_bitmap);
            LoadBitmap(n);
            //var nc = _imagePreparator.MorphologicalDilationF(_bitmap);
            //LoadBitmap(nc);

            var results = _analyzer.Analyze(n, _db);
            int k = 0;
            int percent = 65;
            while (results == null && k <= 3)
            {
               
                results = _analyzer.Analyze(n, _db, percent);
                k++;
                percent -= 10;
            }
            var pen = new Pen(Color.Red, 3);

            var brush = new SolidBrush(Color.Red);
            var img = results.OrderByDescending(x => x.SimularPercent).FirstOrDefault();
            var img2 = results.OrderByDescending(x => x.SimularPercent).FirstOrDefault(x => x.ImgName != img.ImgName);
            var img3 = results.OrderByDescending(x => x.SimularPercent).FirstOrDefault(x => x.ImgName != img.ImgName && x.ImgName != img2.ImgName);
            int offset = 0;
            foreach (var res in results.Distinct())
            {
                //Rectangle rect = new Rectangle(res.x, res.y, res.Bitmap.Width, res.Bitmap.Height);
                //_graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                //_graphics.DrawString(
                //    res.ImgName + " " + res.SimularPercent + "%",
                //    DefaultFont,
                //    brush,
                //    new Point(rect.Right, rect.Bottom)
                //);
                int x = res.Blob.Rectangle.Right -20;
                int y = res.Blob.Rectangle.Bottom + offset;
                _graphics.DrawRectangle(pen, res.Blob.Rectangle);
                _graphics.DrawString(
                    res.ImgName + " " + res.SimularPercent + "%",
                    DefaultFont,
                    brush,
                    new Point(x, y)
                ) ;
               // offset += 20;
            }

            LoadBitmap(n);
            if (img is not null)
            {
                Bitmap image = (Bitmap)System.Drawing.Image.FromFile($"C:\\Users\\uzden\\Documents\\lab5\\Resources\\Существующие образы\\" + img.ImgName);
                LoadBitmap2(image);
            }
            if (img2 is not null)
            {
                Bitmap image2 = (Bitmap)System.Drawing.Image.FromFile($"C:\\Users\\uzden\\Documents\\lab5\\Resources\\Существующие образы\\" + img2.ImgName);
                LoadBitmap3(image2);
            }
            if (img3 is not null)
            {
                Bitmap image3 = (Bitmap)System.Drawing.Image.FromFile($"C:\\Users\\uzden\\Documents\\lab5\\Resources\\Существующие образы\\" + img3.ImgName);
                LoadBitmap4(image3);
            }
            if (results.Any())
            {
                textBox4.Text = string.Join(" ", results.OrderByDescending(x => x.SimularPercent)
                                      .Where(x => x.SimularPercent >= 75)
                                      .Select(x => x.ImgName + " " + x.SimularPercent + "%" + " ")
                                      .Distinct());
            }
            else
            {
                textBox4.Text = "Ничего не найдено, попробуйте с другими параметрами!";
            }

            //foreach (var result in results)
            //{
            //    ListViewItem item = new ListViewItem(result.ImageName);
            //    item.SubItems.Add(result.SimilarityPercentage.ToString());
            //    listView1.Items.Add(item);
            //}

        }


        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }
    }
}