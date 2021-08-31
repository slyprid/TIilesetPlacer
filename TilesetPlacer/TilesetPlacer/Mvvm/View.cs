using System;
using System.Windows;

namespace TilesetPlacer.Mvvm
{
    public abstract class View<T>
        : FrameworkElement, IView<T>
        where T : IViewModel
    {
        public T ViewModel { get; set; }

        protected View()
        {
            ViewModel = (T)Activator.CreateInstance(typeof(T));
            ViewModel.Owner = this;
            DataContext = ViewModel;
        }
    }
}