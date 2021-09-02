using Microsoft.Xna.Framework.Graphics;

namespace TilesetPlacer.Models
{
    public class Tileset
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Texture2D SelectedTexture { get; set; }
        public Texture2D OutputTexture { get; set; }
    }
}