using System;
using System.Windows;

namespace TilesetPlacer.Mvvm
{
    public abstract class WindowView<T>
        : Window, IView<T>
        where T : IViewModel
    {
        public T ViewModel { get; set; }

        protected WindowView()
        {
            ViewModel = (T)Activator.CreateInstance(typeof(T));
            ViewModel.Owner = this;
            DataContext = ViewModel;
        }

        protected WindowView(T viewModel)
        {
            ViewModel = viewModel;
            ViewModel.Owner = this;
            DataContext = ViewModel;
        }
    }
}