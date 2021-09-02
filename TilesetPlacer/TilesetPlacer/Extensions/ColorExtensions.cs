using System;
using Microsoft.Xna.Framework;

namespace TilesetPlacer.Extensions
{
    public static class ColorExtensions
    {
        public static Color WithOpacity(this Color color, float opacity)
        {
            color.A = (Byte) (opacity * 255);
            return color;
        }
    }
}