namespace TilesetPlacer.Models
{
    public class Project
    {
        public bool IsDirty { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int OutputTileWidth { get; set; }
        public int OutputTileHeight { get; set; }
        
        public Project()
        {
            OutputTileWidth = 32;
            OutputTileHeight = 32;
            IsDirty = true;
        }
    }
}