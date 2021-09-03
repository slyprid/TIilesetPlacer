using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace TilesetPlacer.Models
{
    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int SourceX { get; set; }
        public int SourceY { get; set; }
        public int SourceWidth { get; set; }
        public int SourceHeight { get; set; }
        public Guid TilesetId { get; set; }

        [JsonIgnore]
        public Texture2D Texture { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            var srcRect = new Rectangle(SourceX, SourceY, SourceWidth, SourceHeight);
            var destRect = new Rectangle(X, Y, Width, Height);
            spriteBatch.Draw(Texture, destRect, srcRect, Color.White);
        }
    }
}