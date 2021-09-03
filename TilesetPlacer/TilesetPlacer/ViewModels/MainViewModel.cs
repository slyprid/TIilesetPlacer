using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        }

        private void OpenProject()
        {

        }

        private void SaveProject()
        {

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
        }

        private void RemoveTileset()
        {

        }

        private void AddProperty()
        {

        }

        private void RemoveProperty()
        {

        }

        private void SetPickPlaceMode()
        {

        }

        private void SetPropertyMode()
        {

        }

        private void Configure()
        {

        }

        #endregion

        #region Events

        private static void OnTileWidthChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (MainViewModel) d;
            var tileWidth = (int) e.NewValue;
            viewModel.Project.TileWidth = tileWidth;
            viewModel.SelectedTiles.Clear();
        }

        private static void OnTileHeightChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (MainViewModel)d;
            var tileHeight = (int)e.NewValue;
            viewModel.Project.TileHeight = tileHeight;
            viewModel.SelectedTiles.Clear();
        }

        #endregion
    }
}