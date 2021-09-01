using TilesetPlacer.Mvvm;
using TilesetPlacer.ViewModels;

namespace TilesetPlacer.Views
{
    public partial class MainWindow 
        : WindowView<MainViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}