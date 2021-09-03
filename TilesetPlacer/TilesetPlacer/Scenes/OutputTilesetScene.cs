using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using TilesetPlacer.Extensions;
using TilesetPlacer.Graphics;
using TilesetPlacer.Models;
using Color = Microsoft.Xna.Framework.Color;

namespace TilesetPlacer.Scenes
{
    public class OutputTilesetScene 
        : WpfGame
    {
        private Vector2 _mousePosition;
        private float _mx;
        private float _my;
        private ColorEx _cursorColor;
        private Tweener _tweener;
        private IGraphicsDeviceService _graphicsDeviceManager;
        private WpfKeyboard _keyboard;
        private WpfMouse _mouse;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;
        private SpriteBatch _spriteBatch;
        private float _scaleX;
        private float _scaleY;
        
        #region Dependency Properties 

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(SolidColorBrush), typeof(OutputTilesetScene), new PropertyMetadata(default(SolidColorBrush)));
        public SolidColorBrush Background
        {
            get => (SolidColorBrush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public static readonly DependencyProperty TileWidthProperty = DependencyProperty.Register("TileWidth", typeof(int), typeof(OutputTilesetScene), new PropertyMetadata(default(int)));
        public int TileWidth
        {
            get => (int) GetValue(TileWidthProperty);
            set => SetValue(TileWidthProperty, value);
        }

        public static readonly DependencyProperty TileHeightProperty = DependencyProperty.Register("TileHeight", typeof(int), typeof(OutputTilesetScene), new PropertyMetadata(default(int)));
        public int TileHeight
        {
            get => (int) GetValue(TileHeightProperty);
            set => SetValue(TileHeightProperty, value);
        }

        public static readonly DependencyProperty DeviceProperty = DependencyProperty.Register("Device", typeof(GraphicsDevice), typeof(OutputTilesetScene), new PropertyMetadata(default(GraphicsDevice)));
        public GraphicsDevice Device
        {
            get => (GraphicsDevice)GetValue(DeviceProperty);
            set => SetValue(DeviceProperty, value);
        }

        public static readonly DependencyProperty SelectedTilesetProperty = DependencyProperty.Register("SelectedTileset", typeof(Tileset), typeof(OutputTilesetScene), new PropertyMetadata(default(Tileset)));
        public Tileset SelectedTileset
        {
            get => (Tileset) GetValue(SelectedTilesetProperty);
            set => SetValue(SelectedTilesetProperty, value);
        }

        public static readonly DependencyProperty SelectedTilesProperty = DependencyProperty.Register("SelectedTiles", typeof(ObservableCollection<Vector2>), typeof(OutputTilesetScene), new PropertyMetadata(default(ObservableCollection<Vector2>)));
        public ObservableCollection<Vector2> SelectedTiles
        {
            get => (ObservableCollection<Vector2>)GetValue(SelectedTilesProperty);
            set => SetValue(SelectedTilesProperty, value);
        }

        public static readonly DependencyProperty OriginalTileWidthProperty = DependencyProperty.Register("OriginalTileWidth", typeof(int), typeof(OutputTilesetScene), new PropertyMetadata(default(int)));
        public int OriginalTileWidth
        {
            get => (int) GetValue(OriginalTileWidthProperty);
            set => SetValue(OriginalTileWidthProperty, value);
        }

        public static readonly DependencyProperty OriginalTileHeightProperty = DependencyProperty.Register("OriginalTileHeight", typeof(int), typeof(OutputTilesetScene), new PropertyMetadata(default(int)));
        public int OriginalTileHeight
        {
            get => (int) GetValue(OriginalTileHeightProperty);
            set => SetValue(OriginalTileHeightProperty, value);
        }

        public static readonly DependencyProperty TilesProperty = DependencyProperty.Register("Tiles", typeof(ObservableCollection<Tile>), typeof(OutputTilesetScene), new PropertyMetadata(default(ObservableCollection<Tile>)));
        public ObservableCollection<Tile> Tiles
        {
            get => (ObservableCollection<Tile>) GetValue(TilesProperty);
            set => SetValue(TilesProperty, value);
        }

        #endregion

        protected override void Initialize()
        {
            _graphicsDeviceManager = new WpfGraphicsDeviceService(this);

            _keyboard = new WpfKeyboard(this);
            _mouse = new WpfMouse(this);

            base.Initialize();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Device = GraphicsDevice;

            SelectedTiles = new ObservableCollection<Vector2>();
            Tiles = new ObservableCollection<Tile>();

            _cursorColor = new ColorEx(Color.Yellow);
            _tweener = new Tweener();
            _tweener.TweenTo(_cursorColor, x => x.Value3, new Vector3(1f, 0, 0f), 0.25f, 0.025f).RepeatForever(0.2f).AutoReverse().Easing(EasingFunctions.Linear);
        }

        protected override void Update(GameTime gameTime)
        {
            _previousMouseState = _currentMouseState;
            _currentMouseState = _mouse.GetState();
            var keyboardState = _keyboard.GetState();

            _mousePosition = new Vector2(_currentMouseState.X, _currentMouseState.Y);
            _mx = (int)(_mousePosition.X / TileWidth) * TileWidth;
            _my = (int)(_mousePosition.Y / TileHeight) * TileHeight;

            _scaleX = (float) TileWidth / (float) OriginalTileWidth;
            _scaleY = (float)TileHeight / (float)OriginalTileHeight;

            _tweener.Update(gameTime.GetElapsedSeconds());

            if (_currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released && SelectedTileset != null)
            {
                var minX = SelectedTiles.Min(tile => tile.X);
                var minY = SelectedTiles.Min(tile => tile.Y);
                var maxX = SelectedTiles.Max(tile => tile.X);
                var maxY = SelectedTiles.Max(tile => tile.Y);
                var dx = ((maxX - minX) / OriginalTileWidth) + 1;
                var dy = ((maxY - minY) / OriginalTileHeight) + 1 ;
                var idx = 0;
                for (var y = 0; y < dy; y++)
                {
                    for (var x = 0; x < dx; x++)
                    {
                        var selectedTile = SelectedTiles[idx];
                        var tile = new Tile
                        {
                            X = (int)(_mx + (x * TileWidth)),
                            Y = (int)(_my + (y * TileHeight)),
                            Width = TileWidth,
                            Height = TileHeight,
                            SourceX = (int)selectedTile.X,
                            SourceY = (int)selectedTile.Y,
                            SourceWidth = OriginalTileWidth,
                            SourceHeight = OriginalTileHeight,
                            Texture = SelectedTileset.OutputTexture,
                            TilesetId = SelectedTileset.Id
                        };
                        Tiles.Add(tile);
                        idx++;
                    }
                }
            }

            if (_currentMouseState.RightButton == ButtonState.Pressed)
            {
                foreach(var tile in Tiles.ToList())
                {
                    if(tile.X == (int)_mx && tile.Y == (int)_my)
                    {
                        Tiles.Remove(tile);
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(Background.Color.R, Background.Color.G, Background.Color.B));

            if (TileWidth <= 0 || TileHeight <= 0) return;

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp);

            foreach (var tile in Tiles)
            {
                tile.Draw(_spriteBatch);
            }

            for (var y = 0; y < ActualHeight; y += TileHeight)
            {
                _spriteBatch.DrawDashedLine(0f, y, (float)ActualWidth, y, Color.White.WithOpacity(0.5f));   
            }

            for (var x = 0; x < ActualWidth; x += TileWidth)
            {
                _spriteBatch.DrawDashedLine(x, 0f, x, (float)ActualHeight, Color.White.WithOpacity(0.5f));
            }

            _spriteBatch.DrawRectangle(new RectangleF(_mx, _my, TileWidth, TileHeight), _cursorColor.Color);

            _spriteBatch.End();
        }
    }
}