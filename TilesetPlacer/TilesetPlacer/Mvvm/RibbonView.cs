using System;
using System.Windows.Controls.Ribbon;

namespace TilesetPlacer.Mvvm
{
    public abstract class RibbonView<T>
        : RibbonWindow, IView<T>
        where T : IViewModel
    {
        public T ViewModel { get; set; }

        protected RibbonView()
        {
            ViewModel = (T)Activator.CreateInstance(typeof(T));
            ViewModel.Owner = this;
            DataContext = ViewModel;
        }

        protected RibbonView(T viewModel)
        {
            ViewModel = viewModel;
            ViewModel.Owner = this;
            DataContext = ViewModel;
        }
    }
}