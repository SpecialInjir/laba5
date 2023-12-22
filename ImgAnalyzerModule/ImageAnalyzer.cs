using AForge.Imaging;
using System.Drawing.Imaging;
using System.Reflection.Metadata;
using Лаба_5.DatabaseModule;
using Лаба_5.Models;
using Blob = AForge.Imaging.Blob;

namespace Лаба_5.ImgAnalyzerModule
{
    public class ImageAnalyzer
    {
        private List<Blob> _segments = new();
        private Bitmap _bitmap;
        private List<CompareRes> comparisonResults = new();

        private readonly BlobCounter segmList = new()
        {
            FilterBlobs = true,
            MinHeight = 8,
            MinWidth = 8
        };

        public List<CompareRes> Analyze(Bitmap bitmap, DbModule db)
        {
            _bitmap = (Bitmap)bitmap.Clone();

            var contrastedBmp = AddContrast(_bitmap);

            segmList.ProcessImage(contrastedBmp);

            _segments = segmList.GetObjectsInformation().ToList();

            comparisonResults = CompareSegments(_segments, db, _bitmap);

            return comparisonResults.FindAll(r => r.SimularPercent >= 65);
        }

        public Bitmap AddContrast(Bitmap img)
        {
            Bitmap contrasted = new(img.Width, img.Height);

            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    Color color = img.GetPixel(x, y);
                    Color invertedColor = Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
                    contrasted.SetPixel(x, y, invertedColor);
                }
            }

            return contrasted;
        }


        public static List<CompareRes> CompareSegments(List<Blob> segments, DbModule db, Bitmap source)
        {
            var imageHashes = db.ImageHashes;
            List<CompareRes> similarityResults = new();

            foreach (Blob segment in segments)
            {
                // Получение координат объекта
                int x = segment.Rectangle.X;
                int y = segment.Rectangle.Y;
                int width = segment.Rectangle.Width;
                int height = segment.Rectangle.Height;

                // Выделение соответствующей области на исходном изображении
                var blobBitmap = source.Clone(new Rectangle(x, y, width, height), source.PixelFormat);


                var currentSegmentHash = HashCalculator.Calculate(blobBitmap);

                foreach (var imageHash in imageHashes)
                {
                    double similarity = CompareHashes(currentSegmentHash, imageHash.Value);
                    var result = new CompareRes(segment, similarity, imageHash.Key);

                    similarityResults.Add(result);
                }
            }

            return similarityResults;
        }

        private static double CompareHashes(ulong hash1, ulong hash2)
        {
            // Вычисляем расстояние Хемминга
            ulong xorResult = hash1 ^ hash2;

            // Подсчитываем количество установленных битов в результате XOR
            int setBitsCount = CountSetBits(xorResult);

            // Вычисляем процент соответствия
            double similarity = 1.0 - setBitsCount / 64.0;
            return similarity * 100.0;
        }


        private static int CountSetBits(ulong number)
        {
            int count = 0;
            while (number > 0)
            {
                count += (int)(number & 1);
                number >>= 1;
            }
            return count;
        }
    }
}

