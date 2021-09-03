using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
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
    public class SelectedTilesetScene 
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
        private Vector2 _startPoint;

        #region Dependency Properties 

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(SolidColorBrush), typeof(SelectedTilesetScene), new PropertyMetadata(default(SolidColorBrush)));
        public SolidColorBrush Background
        {
            get => (SolidColorBrush) GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public static readonly DependencyProperty TileWidthProperty = DependencyProperty.Register("TileWidth", typeof(int), typeof(SelectedTilesetScene), new PropertyMetadata(default(int)));
        public int TileWidth
        {
            get => (int)GetValue(TileWidthProperty);
            set => SetValue(TileWidthProperty, value);
        }

        public static readonly DependencyProperty TileHeightProperty = DependencyProperty.Register("TileHeight", typeof(int), typeof(SelectedTilesetScene), new PropertyMetadata(default(int)));
        public int TileHeight
        {
            get => (int)GetValue(TileHeightProperty);
            set => SetValue(TileHeightProperty, value);
        }

        public static readonly DependencyProperty DeviceProperty = DependencyProperty.Register("Device", typeof(GraphicsDevice), typeof(SelectedTilesetScene), new PropertyMetadata(default(GraphicsDevice)));
        public GraphicsDevice Device
        {
            get => (GraphicsDevice) GetValue(DeviceProperty);
            set => SetValue(DeviceProperty, value);
        }

        public static readonly DependencyProperty SelectedTilesetProperty = DependencyProperty.Register("SelectedTileset", typeof(Tileset), typeof(SelectedTilesetScene), new PropertyMetadata(default(Tileset)));

        public Tileset SelectedTileset
        {
            get => (Tileset) GetValue(SelectedTilesetProperty);
            set => SetValue(SelectedTilesetProperty, value);
        }

        public static readonly DependencyProperty SelectedTilesProperty = DependencyProperty.Register("SelectedTiles", typeof(ObservableCollection<Vector2>), typeof(SelectedTilesetScene), new PropertyMetadata(default(ObservableCollection<Vector2>)));
        public ObservableCollection<Vector2> SelectedTiles
        {
            get => (ObservableCollection<Vector2>) GetValue(SelectedTilesProperty);
            set => SetValue(SelectedTilesProperty, value);
        }

        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register("IsDirty", typeof(bool), typeof(SelectedTilesetScene), new PropertyMetadata(default(bool)));
        public bool IsDirty
        {
            get => (bool) GetValue(IsDirtyProperty);
            set => SetValue(IsDirtyProperty, value);
        }

        public static readonly DependencyProperty MousePositionProperty = DependencyProperty.Register("MousePosition", typeof(string), typeof(SelectedTilesetScene), new PropertyMetadata(default(string)));
        public string MousePosition
        {
            get => (string) GetValue(MousePositionProperty);
            set => SetValue(MousePositionProperty, value);
        }

        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(string), typeof(SelectedTilesetScene), new PropertyMetadata(default(string)));
        public string SelectedIndex
        {
            get => (string) GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public static readonly DependencyProperty SelectedCurrentIndexProperty = DependencyProperty.Register("SelectedCurrentIndex", typeof(string), typeof(SelectedTilesetScene), new PropertyMetadata(default(string)));
        public string SelectedCurrentIndex
        {
            get => (string) GetValue(SelectedCurrentIndexProperty);
            set => SetValue(SelectedCurrentIndexProperty, value);
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

            _cursorColor = new ColorEx(Color.Yellow);
            _tweener = new Tweener();
            _tweener.TweenTo(_cursorColor, x => x.Value3, new Vector3(1f, 0, 0f), 0.25f, 0.025f).RepeatForever(0.2f).AutoReverse().Easing(EasingFunctions.Linear);

            SelectedTiles = new ObservableCollection<Vector2>();
        }

        protected override void Update(GameTime gameTime)
        {
            _previousMouseState = _currentMouseState;
            _currentMouseState = _mouse.GetState();
            var keyboardState = _keyboard.GetState();

            _mousePosition = new Vector2(_currentMouseState.X, _currentMouseState.Y);
            _mx = (int)(_mousePosition.X / TileWidth) * TileWidth;
            _my = (int)(_mousePosition.Y / TileHeight) * TileHeight;

            MousePosition = $"[{_mx / TileWidth},{_my / TileHeight}]";

            _tweener.Update(gameTime.GetElapsedSeconds());

            if (SelectedTileset == null) return;

            var sx = _startPoint.X;
            var sy = _startPoint.Y;
            var dx = _mx - sx;
            var dy = _my - sy;

            var startIndex = (sy / TileHeight) * (SelectedTileset.SelectedTexture.Width / TileHeight) + (sx / TileWidth);
            var endIndex = (_my / TileHeight) * (SelectedTileset.SelectedTexture.Width / TileWidth) + (_mx / TileWidth);
            var index = (_my / TileHeight) * (SelectedTileset.SelectedTexture.Width / TileWidth) + (_mx / TileWidth);
            SelectedCurrentIndex = $"{index}";

            if (_currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                SelectedTiles.Clear();
                _startPoint = new Vector2(_mx, _my);
                SelectedTiles.Add(_startPoint);
                IsDirty = true;
                SelectedIndex = $"{index}";
            }
            else if (_currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Pressed)
            {
                if (startIndex == endIndex) return;

                SelectedTiles.Clear();
                
                SelectedIndex = $"[{startIndex}->{endIndex}]";

                for (var y = 0; y <= dy; y += TileHeight)
                {
                    for (var x = 0; x <= dx; x += TileWidth)
                    {
                        SelectedTiles.Add(new Vector2(sx + x, sy + y));
                    }
                }
                IsDirty = true;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(Background.Color.R, Background.Color.G, Background.Color.B));

            if (TileWidth <= 0 || TileHeight <= 0) return;

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            if (SelectedTileset != null)
            {
                _spriteBatch.Draw(SelectedTileset.SelectedTexture, Vector2.Zero, Color.White);
            }

            if (SelectedTileset == null)
            {
                for (var y = 0; y < ActualHeight; y += TileHeight)
                {
                    _spriteBatch.DrawDashedLine(0f, y, (float) ActualWidth, y, Color.White.WithOpacity(0.5f));
                }

                for (var x = 0; x < ActualWidth; x += TileWidth)
                {
                    _spriteBatch.DrawDashedLine(x, 0f, x, (float) ActualHeight, Color.White.WithOpacity(0.5f));
                }
            }
            else
            {
                for (var y = 0; y < SelectedTileset.SelectedTexture.Height; y += TileHeight)
                {
                    _spriteBatch.DrawDashedLine(0f, y, (float)SelectedTileset.SelectedTexture.Width, y, Color.White.WithOpacity(0.5f));
                }

                for (var x = 0; x < SelectedTileset.SelectedTexture.Width; x += TileWidth)
                {
                    _spriteBatch.DrawDashedLine(x, 0f, x, (float)SelectedTileset.SelectedTexture.Height, Color.White.WithOpacity(0.5f));
                }

                _spriteBatch.DrawLine(0f, SelectedTileset.SelectedTexture.Height, SelectedTileset.SelectedTexture.Width, SelectedTileset.SelectedTexture.Height, Color.White);
                _spriteBatch.DrawLine(SelectedTileset.SelectedTexture.Width, 0f, SelectedTileset.SelectedTexture.Width, SelectedTileset.SelectedTexture.Height, Color.White);
            }

            foreach (var (x, y) in SelectedTiles)
            {
                _spriteBatch.DrawRectangle(new RectangleF(x, y, TileWidth, TileHeight), Color.Blue);
            }

            _spriteBatch.DrawRectangle(new RectangleF(_mx, _my, TileWidth, TileHeight), _cursorColor.Color);

            _spriteBatch.End();
        }
    }
}