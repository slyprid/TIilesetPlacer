using System.Collections.Generic;
using System.Windows.Documents;
using Newtonsoft.Json;

namespace TilesetPlacer.Models
{
    public class Project
    {
        #region Ignored Properties

        [JsonIgnore]
        public bool IsDirty { get; set; }

        [JsonIgnore]
        public string Path { get; set; }
        
        #endregion
        
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int OutputTileWidth { get; set; }
        public int OutputTileHeight { get; set; }
        public string MetaFilename { get; set; }
        public List<Tileset> Tilesets { get; set; }
        public List<Tile> Tiles { get; set; }
        
        public Project()
        {
            OutputTileWidth = 32;
            OutputTileHeight = 32;
            IsDirty = true;
            MetaFilename = "Tiles.meta";
            Tilesets = new List<Tileset>();
            Tiles = new List<Tile>();
        }
    }
}