using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using TilesetPlacer.Helpers;
using TilesetPlacer.Models;
using TilesetPlacer.Mvvm;
using TilesetPlacer.Views;

namespace TilesetPlacer.ViewModels
{
    public class MainViewModel
        : ViewModel
    {
        #region Dependency Properties

        public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(Project), typeof(MainViewModel), new PropertyMetadata(default(Project)));
        public Project Project
        {
            get => (Project) GetValue(ProjectProperty);
            set => SetValue(ProjectProperty, value);
        }

        public static readonly DependencyProperty TilesetsProperty = DependencyProperty.Register("Tilesets", typeof(ObservableCollection<Tileset>), typeof(MainViewModel), new PropertyMetadata(default(ObservableCollection<Tileset>)));
        public ObservableCollection<Tileset> Tilesets
        {
            get => (ObservableCollection<Tileset>) GetValue(TilesetsProperty);
            set => SetValue(TilesetsProperty, value);
        }

        public static readonly DependencyProperty SelectedTilesetProperty = DependencyProperty.Register("SelectedTileset", typeof(Tileset), typeof(MainViewModel), new PropertyMetadata(default(Tileset)));
        public Tileset SelectedTileset
        {
            get => (Tileset) GetValue(SelectedTilesetProperty);
            set => SetValue(SelectedTilesetProperty, value);
        }

        public static readonly DependencyProperty OutputGraphicsDeviceProperty = DependencyProperty.Register("OutputGraphicsDevice", typeof(GraphicsDevice), typeof(MainViewModel), new PropertyMetadata(default(GraphicsDevice)));
        public GraphicsDevice OutputGraphicsDevice
        {
            get => (GraphicsDevice) GetValue(OutputGraphicsDeviceProperty);
            set => SetValue(OutputGraphicsDeviceProperty, value);
        }

        public static readonly DependencyProperty SelectedGraphicsDeviceProperty = DependencyProperty.Register("SelectedGraphicsDevice", typeof(GraphicsDevice), typeof(MainViewModel), new PropertyMetadata(default(GraphicsDevice)));
        public GraphicsDevice SelectedGraphicsDevice
        {
            get => (GraphicsDevice)GetValue(SelectedGraphicsDeviceProperty);
            set => SetValue(SelectedGraphicsDeviceProperty, value);
        }

        public static readonly DependencyProperty SelectedTilesProperty = DependencyProperty.Register("SelectedTiles", typeof(ObservableCollection<Vector2>), typeof(MainViewModel), new PropertyMetadata(default(ObservableCollection<Vector2>)));
        public ObservableCollection<Vector2> SelectedTiles
        {
            get => (ObservableCollection<Vector2>) GetValue(SelectedTilesProperty);
            set => SetValue(SelectedTilesProperty, value);
        }

        public static readonly DependencyProperty TileWidthProperty = DependencyProperty.Register("TileWidth", typeof(int), typeof(MainViewModel), new PropertyMetadata(default(int), OnTileWidthChange));
        public int TileWidth
        {
            get => (int) GetValue(TileWidthProperty);
            set => SetValue(TileWidthProperty, value);
        }

        public static readonly DependencyProperty TileHeightProperty = DependencyProperty.Register("TileHeight", typeof(int), typeof(MainViewModel), new PropertyMetadata(default(int), OnTileHeightChange));
        public int TileHeight
        {
            get => (int) GetValue(TileHeightProperty);
            set => SetValue(TileHeightProperty, value);
        }

        public static readonly DependencyProperty TilesProperty = DependencyProperty.Register("Tiles", typeof(ObservableCollection<Tile>), typeof(MainViewModel), new PropertyMetadata(default(ObservableCollection<Tile>)));
        public ObservableCollection<Tile> Tiles
        {
            get => (ObservableCollection<Tile>) GetValue(TilesProperty);
            set => SetValue(TilesProperty, value);
        }

        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register("IsDirty", typeof(bool), typeof(MainViewModel), new PropertyMetadata(default(bool), OnIsDirtyChanged));
        public bool IsDirty
        {
            get => (bool) GetValue(IsDirtyProperty);
            set => SetValue(IsDirtyProperty, value);
        }

        public static readonly DependencyProperty SelectedMousePositionProperty = DependencyProperty.Register("SelectedMousePosition", typeof(string), typeof(MainViewModel), new PropertyMetadata(default(string)));
        public string SelectedMousePosition
        {
            get => (string) GetValue(SelectedMousePositionProperty);
            set => SetValue(SelectedMousePositionProperty, value);
        }

        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(string), typeof(MainViewModel), new PropertyMetadata(default(string)));
        public string SelectedIndex
        {
            get => (string) GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public static readonly DependencyProperty SelectedCurrentIndexProperty = DependencyProperty.Register("SelectedCurrentIndex", typeof(string), typeof(MainViewModel), new PropertyMetadata(default(string)));
        public string SelectedCurrentIndex
        {
            get => (string) GetValue(SelectedCurrentIndexProperty);
            set => SetValue(SelectedCurrentIndexProperty, value);
        }
        

        #endregion

        #region Command Properties

        public Command NewProjectCommand { get; set; }
        public Command OpenProjectCommand { get; set; }
        public Command SaveProjectCommand { get; set; }
        public Command ExportCommand { get; set; }
        public Command AddTilesetCommand { get; set; }
        public Command RemoveTilesetCommand { get; set; }
        public Command AddPropertyCommand { get; set; }
        public Command RemovePropertyCommand { get; set; }
        public Command SetPickPlaceModeCommand { get; set; }
        public Command SetPropertyModeCommand { get; set; }
        public Command ConfigureCommand { get; set; }

        #endregion

        public MainViewModel()
        {
            NewProject();
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            NewProjectCommand = new Command(NewProject);
            OpenProjectCommand = new Command(OpenProject);
            SaveProjectCommand = new Command(SaveProject);
            ExportCommand = new Command(Export);
            AddTilesetCommand = new Command(AddTileset);
            RemoveTilesetCommand = new Command(RemoveTileset);
            AddPropertyCommand = new Command(AddProperty);
            RemovePropertyCommand = new Command(RemoveProperty);
            SetPickPlaceModeCommand = new Command(SetPickPlaceMode);
            SetPropertyModeCommand = new Command(SetPropertyMode);
            ConfigureCommand = new Command(Configure);
        }

        #region Actions

        private void NewProject()
        {
            if (Project != null && Project.IsDirty)
            {
                var ret = MessageBox.Show("Are you sure you want to start a new project?", "TilesetPlacer", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (ret == MessageBoxResult.No) return;
            }
            Project = new Project();
            Tilesets = new ObservableCollection<Tileset>();
            Tiles = new ObservableCollection<Tile>();

            SelectedTiles = new ObservableCollection<Vector2>();
            TileWidth = 16;
            TileHeight = 16;
            IsDirty = true;
        }

        private void OpenProject()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "TilesetPlacer Project Files (*.tpproj)|*.tpproj|All files (*.*)|*.*"
            };

            if (!dialog.ShowDialog((MainWindow)Owner).GetValueOrDefault()) return;

            var path = dialog.FileName;

            Project = JsonConvert.DeserializeObject<Project>(File.ReadAllText(path));
            SelectedTileset = null;
            SelectedTiles.Clear();
            Tiles.Clear();
            Tilesets.Clear();

            foreach (var tileset in Project.Tilesets)
            {
                tileset.SelectedTexture = ContentHelper.LoadTextureFromFile(SelectedGraphicsDevice, tileset.Path);
                tileset.OutputTexture = ContentHelper.LoadTextureFromFile(OutputGraphicsDevice, tileset.Path);
            }

            foreach (var tile in Project.Tiles)
            {
                tile.Texture = Project.Tilesets.Single(x => x.Id == tile.TilesetId).OutputTexture;
            }

            Tilesets = new ObservableCollection<Tileset>(Project.Tilesets);
            Tiles = new ObservableCollection<Tile>(Project.Tiles);

            if (Tilesets.Count > 0)
            {
                SelectedTileset = Tilesets.First();
            }
        }

        private void SaveProject()
        {
            if (string.IsNullOrEmpty(Project.Path))
            {
                var dialog = new SaveFileDialog
                {
                    FileName = $"TilesetPlacerProject1.tpproj",
                    Filter = "TilesetPlacer Project Files (*.tpproj)|*.tpproj|All files (*.*)|*.*"
                };

                if (!dialog.ShowDialog((MainWindow) Owner).GetValueOrDefault()) return;

                Project.Path = dialog.FileName;
            }

            Project.Tilesets = Tilesets.ToList();
            Project.Tiles = Tiles.ToList();

            var output = JsonConvert.SerializeObject(Project, Formatting.Indented);

            File.WriteAllText(Project.Path, output);

            IsDirty = false;
        }

        private void Export()
        {

        }

        private void AddTileset()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.gif, *.jpg, *.png, *.tif, *.dds)|*.gif;*.jpg;*.png;*.tif;*.dds|All files (*.*)|*.*"
            };

            if (!dialog.ShowDialog((MainWindow)Owner).GetValueOrDefault()) return;

            var filename = dialog.FileName;
            var tileset = new Tileset
            {
                Name = Path.GetFileNameWithoutExtension(filename),
                Path = filename,
                SelectedTexture = ContentHelper.LoadTextureFromFile(SelectedGraphicsDevice, filename),
                OutputTexture = ContentHelper.LoadTextureFromFile(OutputGraphicsDevice, filename)
            };
            Tilesets.Add(tileset);
            SelectedTileset = tileset;
            IsDirty = true;
        }

        private void RemoveTileset()
        {
            if (SelectedTileset == null)
            {
                MessageBox.Show("No tileset selected", "TilesetPlacer");
                return;
            }

            var id = SelectedTileset.Id;
            Tilesets.Remove(SelectedTileset);
            SelectedTileset = null;

            foreach (var tile in Tiles.ToList())
            {
                if (tile.TilesetId == id)
                {
                    Tiles.Remove(tile);
                }
            }

            IsDirty = true;
        }

        private void AddProperty()
        {
            IsDirty = true;
        }

        private void RemoveProperty()
        {
            IsDirty = true;
        }

        private void SetPickPlaceMode()
        {

        }

        private void SetPropertyMode()
        {

        }

        private void Configure()
        {
            IsDirty = true;
        }

        #endregion

        #region Events

        private static void OnTileWidthChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (MainViewModel) d;
            var tileWidth = (int) e.NewValue;
            viewModel.Project.TileWidth = tileWidth;
            viewModel.SelectedTiles.Clear();
            viewModel.IsDirty = true;
        }

        private static void OnTileHeightChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (MainViewModel)d;
            var tileHeight = (int)e.NewValue;
            viewModel.Project.TileHeight = tileHeight;
            viewModel.SelectedTiles.Clear();
            viewModel.IsDirty = true;
        }

        private static void OnIsDirtyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (MainViewModel)d;
            var value = (bool)e.NewValue;
            viewModel.Project.IsDirty = value;
        }

        #endregion
    }
}