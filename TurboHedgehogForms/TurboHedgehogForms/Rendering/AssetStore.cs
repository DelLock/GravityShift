using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace TurboHedgehogForms.Rendering
{
    public sealed class AssetStore
    {
        private readonly Dictionary<string, Image> _cache = new();

        public Image Get(string relativePath)
        {
            if (_cache.TryGetValue(relativePath, out var img)) return img;

            // путь относительно exe
            string path = Path.Combine(System.AppContext.BaseDirectory, relativePath);
            img = Image.FromFile(path);
            _cache[relativePath] = img;
            return img;
        }
    }
}