using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace TilesetPlacer.Helpers
{
    public static class ContentHelper
    {
        public static Texture2D LoadTextureFromFile(GraphicsDevice graphicsDevice, string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                return Texture2D.FromStream(graphicsDevice, stream);
            }
        }
    }
}