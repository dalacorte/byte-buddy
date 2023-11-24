namespace ByteBuddy.Entities
{
    public class Buddy
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public string Path { get; private set; }
        public string ThumbnailPath { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Frames { get; private set; }
        public byte[] Bytes { get; private set; }

        public Buddy() { }

        public Buddy(string id, string name, string type, string path, string thumbnailPath, int width, int height, int frames, byte[] bytes)
        {
            Id = id;
            Name = name;
            Type = type;
            Path = path;
            ThumbnailPath = thumbnailPath;
            Width = width;
            Height = height;
            Frames = frames;
            Bytes = bytes;
        }
    }
}
