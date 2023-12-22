namespace Лаба_5.DatabaseModule
{
    public class DbModule
    {
        public Dictionary<string, ulong> ImageHashes = new();

        public void AddImage(string imageName, Bitmap image)
        {
            ulong perceptualHash = HashCalculator.Calculate(image);
            ImageHashes[imageName] = perceptualHash;
        }

        public (string, ulong)? GetHash(string imageName)
        {
            if (ImageHashes.ContainsKey(imageName))
            {
                return (imageName, ImageHashes[imageName]);
            }
            else
            {
                Console.WriteLine($"Образ с именем {imageName} не найден в базе данных.");
                return null;
            }
        }

        public (string, ulong)? GetByHash(ulong hash)
        {
            if (ImageHashes.ContainsValue(hash))
            {
                string key = ImageHashes.First(x => x.Value == hash).Key;
                return (key, ImageHashes[key]);
            }
            else
            {
                Console.WriteLine($"Образ с хэшем {hash} не найден в базе данных.");
                return null;
            }
        }

        public void SaveHashesToFile(string filePath)
        {
            using BinaryWriter writer = new(File.Open(filePath, FileMode.Create));
            foreach (var entry in ImageHashes)
            {
                writer.Write(entry.Key);    // записываем имя образа
                writer.Write(entry.Value);  // записываем хеш
            }
        }

        public void LoadHashesFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                using BinaryReader reader = new(File.Open(filePath, FileMode.Open));
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    string imageName = reader.ReadString(); // читаем имя образа
                    ulong hash = reader.ReadUInt64();       // читаем хеш
                    ImageHashes[imageName] = hash;
                }
            }
            else
            {
                Console.WriteLine($"Файл {filePath} не найден.");
            }
        }
    }
}
