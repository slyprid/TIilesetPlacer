using System.Windows;
using System.Windows.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using TilesetPlacer.Extensions;
using TilesetPlacer.Models;
using Color = Microsoft.Xna.Framework.Color;

namespace TilesetPlacer.Scenes
{
    public class OutputTilesetScene 
        : WpfGame
    {
        private IGraphicsDeviceService _graphicsDeviceManager;
        private WpfKeyboard _keyboard;
        private WpfMouse _mouse;
        private SpriteBatch _spriteBatch;

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

        #endregion

        protected override void Initialize()
        {
            // must be initialized. required by Content loading and rendering (will add itself to the Services)
            // note that MonoGame requires this to be initialized in the constructor, while WpfInterop requires it to
            // be called inside Initialize (before base.Initialize())
            _graphicsDeviceManager = new WpfGraphicsDeviceService(this);

            // wpf and keyboard need reference to the host control in order to receive input
            // this means every WpfGame control will have it's own keyboard & mouse manager which will only react if the mouse is in the control
            _keyboard = new WpfKeyboard(this);
            _mouse = new WpfMouse(this);

            // must be called after the WpfGraphicsDeviceService instance was created
            base.Initialize();

            // content loading now possible
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Device = GraphicsDevice;
        }

        protected override void Update(GameTime time)
        {
            // every update we can now query the keyboard & mouse for our WpfGame
            var mouseState = _mouse.GetState();
            var keyboardState = _keyboard.GetState();
        }

        protected override void Draw(GameTime time)
        {
            GraphicsDevice.Clear(new Color(Background.Color.R, Background.Color.G, Background.Color.B));

            if (TileWidth <= 0 || TileHeight <= 0) return;

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            for (var y = 0; y < ActualHeight; y += TileHeight)
            {
                _spriteBatch.DrawDashedLine(0f, y, (float)ActualWidth, y, Color.White.WithOpacity(0.5f));   
            }

            for (var x = 0; x < ActualWidth; x += TileWidth)
            {
                _spriteBatch.DrawDashedLine(x, 0f, x, (float)ActualHeight, Color.White.WithOpacity(0.5f));
            }

            _spriteBatch.End();
        }
    }
}