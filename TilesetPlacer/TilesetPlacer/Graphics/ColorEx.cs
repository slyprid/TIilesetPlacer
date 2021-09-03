using Microsoft.Xna.Framework;

namespace TilesetPlacer.Graphics
{
    public class ColorEx
    {
        public Color Color { get; private set; }

        public Vector3 Value3
        {
            get => Color.ToVector3();
            set => Color = new Color(value);
        }

        public Vector4 Value4
        {
            get => Color.ToVector4();
            set => Color = new Color(value);
        }


        public ColorEx()
        {
            Color = Color.White;
        }

        public ColorEx(Color color)
        {
            Color = color;
        }

        public static Color FromRGB(byte red, byte green, byte blue)
        {
            return new Color(red, green, blue);
        }

        public static Color FromRGBA(byte red, byte green, byte blue, byte alpha)
        {
            return new Color(red, green, blue, alpha);
        }
    }
}