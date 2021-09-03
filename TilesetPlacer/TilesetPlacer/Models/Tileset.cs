using System;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace TilesetPlacer.Models
{
    public class Tileset
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        [JsonIgnore]
        public Texture2D SelectedTexture { get; set; }

        [JsonIgnore]
        public Texture2D OutputTexture { get; set; }

        public Tileset()
        {
            Id = Guid.NewGuid();
        }
    }
}