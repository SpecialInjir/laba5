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
        private List<CompareRes2> comparisonResults2 = new();

        private readonly BlobCounter segmList = new()
        {
            FilterBlobs = true,
            MinHeight = 8,
            MinWidth = 8
        };

        public List<CompareRes> Analyze(Bitmap bitmap, DbModule db, int simularPercent = 65)
        {
            _bitmap = (Bitmap)bitmap.Clone();

            var contrastedBmp = AddContrast(_bitmap);

            segmList.ProcessImage(contrastedBmp);

            _segments = segmList.GetObjectsInformation().ToList();

            comparisonResults = CompareSegments(_segments, db, _bitmap);

            return comparisonResults.FindAll(r => r.SimularPercent >= simularPercent);
        }

        public List<CompareRes2> Analyze2(Bitmap bitmap, DbModule db)
        {
            _bitmap = (Bitmap)bitmap.Clone();

            var contrastedBmp = AddContrast(_bitmap);

            var _segments = ExtractConnectedComponents(contrastedBmp);

            //segmList.ProcessImage(contrastedBmp);

            //_segments = segmList.GetObjectsInformation().ToList();

            comparisonResults2 = CompareSegments2(_segments, db, _bitmap);

            return comparisonResults2.FindAll(r => r.SimularPercent >= 65);
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


        //-----------------------------------------------------------

        public static List<CompareRes2> CompareSegments2(List<Bitmap> segments, DbModule db, Bitmap source)
        {
            var imageHashes = db.ImageHashes;
            List<CompareRes2> similarityResults = new();

            foreach (Bitmap segment in segments)
            {
                int x = 0; // координата x левого верхнего угла объекта
                int y = 0; // координата y левого верхнего угла объекта
                int width = segment.Width;
                int height = segment.Height;

                // Нахождение координат объекта
                for (int i = 0; i < source.Width; i++)
                {
                    for (int j = 0; j < source.Height; j++)
                    {
                        if (IsObject(segment, i, j)) // функция IsObject определяет, является ли пиксель частью объекта
                        {
                            x = i;
                            y = j;
                            break;
                        }
                    }
                }

                // Выделение соответствующей области на исходном изображении
                var blobBitmap = source.Clone(new Rectangle(x, y, width, height), source.PixelFormat);

                var currentSegmentHash = HashCalculator.Calculate(blobBitmap);

                foreach (var imageHash in imageHashes)
                {
                    double similarity = CompareHashes(currentSegmentHash, imageHash.Value);
                    var result = new CompareRes2(segment, similarity, x, y, imageHash.Key);

                    similarityResults.Add(result);
                }
            }

            return similarityResults;
        }

        private static bool IsObject(Bitmap segment, int x, int y)
        {
            Color pixelColor = segment.GetPixel(x, y+1);

            // если объект имеет белый фон, то можно проверить, является ли пиксель не белым
            if (pixelColor.R < 240 && pixelColor.G < 240 && pixelColor.B < 240)
            {
                return true; // Пиксель принадлежит объекту
            }
            else
            {
                return false; // Пиксель не принадлежит объекту
            }
        }

        private int[,] ConvertToBinary(Bitmap image)
    {
        var binaryImage = new int[image.Width, image.Height];
        for (int i = 0; i < image.Width; i++)
        {
            for (int j = 0; j < image.Height; j++)
            {
                binaryImage[i, j] = image.GetPixel(i, j).R > 128 ? 1 : 0;
            }
        }

        return binaryImage;
    }
    private int[,] LabelComponents(Bitmap image)
        {
            var binaryImage = ConvertToBinary(image);
            int labelCount = 1;
            int width = binaryImage.GetLength(0);
            int height = binaryImage.GetLength(1);
            int[,] labels = new int[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (binaryImage[x, y] != 0 && labels[x, y] == 0)
                    {
                        FloodFill(binaryImage, labels, x, y, labelCount);
                        labelCount++;
                    }
                }
            }

            return labels;
        }

        private void FloodFill(int[,] binaryImage, int[,] labels, int x, int y, int label)
        {
            if (x >= 0 && y >= 0 && x < binaryImage.GetLength(0) && y < binaryImage.GetLength(1) && binaryImage[x, y] != 0 && labels[x, y] == 0)
            {
                labels[x, y] = label;

                FloodFill(binaryImage, labels, x - 1, y, label);
                FloodFill(binaryImage, labels, x + 1, y, label);
                FloodFill(binaryImage, labels, x, y - 1, label);
                FloodFill(binaryImage, labels, x, y + 1, label);
            }
        }

        private List<Bitmap> ExtractConnectedComponents(Bitmap image)
        {
            var labels =  LabelComponents(image);
            var connectedComponents = new List<Bitmap>();
            var labelCount = labels.Cast<int>().Max();

            for (int i = 1; i <= labelCount; i++)
            {
                var minX = labels.GetLength(0);
                var minY = labels.GetLength(1);
                var maxX = 0;
                var maxY = 0;

                for (int x = 0; x < labels.GetLength(0); x++)
                {
                    for (int y = 0; y < labels.GetLength(1); y++)
                    {
                        if (labels[x, y] == i)
                        {
                            minX = Math.Min(minX, x);
                            minY = Math.Min(minY, y);
                            maxX = Math.Max(maxX, x);
                            maxY = Math.Max(maxY, y);
                        }
                    }
                }

                var width = maxX - minX + 1;
                var height = maxY - minY + 1;
                var component = new Bitmap(width, height);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        component.SetPixel(x, y, image.GetPixel(minX + x, minY + y));
                    }
                }

                connectedComponents.Add(component);
            }

            return connectedComponents;
        }

        //public (Bitmap, string, int, string) FindMostSimilarImage(string hash, Dictionary<Bitmap, DataRecord> hashes)
        //{
        //    int minDistance = int.MaxValue;
        //    Bitmap mostSimilarImage = null;
        //    string mosthash = "";
        //    string s = "";
        //    foreach (var pair in hashes)
        //    {
        //        int distance = HammingDistance(hash, pair.Value.Hash);
        //        if (distance < minDistance)
        //        {
        //            minDistance = distance;
        //            mostSimilarImage = pair.Key;
        //            mosthash = pair.Value.Hash;
        //            s = pair.Value.SemanticInformation;
        //        }
        //    }
        //    return (mostSimilarImage, mosthash, minDistance, s);
        //}

        private int HammingDistance(string hash1, string hash2)
        {
            int distance = 0;
            for (int i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                {
                    distance++;
                }
            }
            return distance;
        }






    }
}

