using System.Windows;

namespace TilesetPlacer.Mvvm
{
    public interface IViewModel
    {
        FrameworkElement Owner { get; set; }
    }
}