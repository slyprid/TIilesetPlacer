using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace TilesetPlacer.Extensions
{
    public static class SpriteBatchExtensions
    {
        private static Texture2D _whitePixelTexture;

        private static Texture2D GetTexture(SpriteBatch spriteBatch)
        {
            if (_whitePixelTexture != null) return _whitePixelTexture;
            _whitePixelTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _whitePixelTexture.SetData(new [] { Color.White });
            spriteBatch.Disposing += (sender, args) =>
            {
                _whitePixelTexture?.Dispose();
                _whitePixelTexture = null;
            };
            return _whitePixelTexture;
        }

        public static void DrawDashedLine(
            this SpriteBatch spriteBatch,
            float x1,
            float y1,
            float x2,
            float y2,
            Color color,
            float thickness = 1f,
            float layerDepth = 0.0f)
        {
            spriteBatch.DrawDashedLine(new Vector2(x1, y1), new Vector2(x2, y2), color, thickness, layerDepth);
        }

        public static void DrawDashedLine(
            this SpriteBatch spriteBatch,
            Vector2 point1,
            Vector2 point2,
            Color color,
            float thickness = 1f,
            float layerDepth = 0.0f)
        {
            var length = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2((double)point2.Y - (double)point1.Y, (double)point2.X - (double)point1.X);
            spriteBatch.DrawDashedLine(point1, length, angle, color, thickness, layerDepth);
        }

        public static void DrawDashedLine(
            this SpriteBatch spriteBatch,
            Vector2 point,
            float length,
            float angle,
            Color color,
            float thickness = 1f,
            float layerDepth = 0.0f)
        {
            var origin = new Vector2(0.0f, 0.5f);
            for (var i = 0; i < length; i += 2)
            {
                var nextPoint = point + (angle > 0 ? new Vector2(0f, i) : new Vector2(i, 0f));
                var scale = new Vector2(thickness, thickness);
                spriteBatch.Draw(GetTexture(spriteBatch), nextPoint, new Rectangle?(), color, angle, origin, scale, SpriteEffects.None, layerDepth);
            }
            
        }
    }
}