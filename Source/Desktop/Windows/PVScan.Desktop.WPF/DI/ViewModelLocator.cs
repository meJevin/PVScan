using PVScan.Desktop.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PVScan.Desktop.WPF.DI
{
    public static class ViewModelLocator
    {
        static ViewModelLocator()
        {

        }

        public static readonly DependencyProperty AutoWireViewModelProperty =
            DependencyProperty.RegisterAttached(
                "AutoWireViewModel",
                typeof(bool),
                typeof(ViewModelLocator),
                new PropertyMetadata(default(bool), OnAutoWireViewModelChanged));

        public static bool GetAutoWireViewModel(UIElement bindable) =>
            (bool)bindable.GetValue(AutoWireViewModelProperty);

        public static void SetAutoWireViewModel(UIElement bindable, bool value) =>
            bindable.SetValue(AutoWireViewModelProperty, value);

        private static void OnAutoWireViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement view))
            {
                return;
            }

            var viewType = view.GetType();
            var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var viewModelName = $"{viewName}ViewModel, {viewAssemblyName}";

            var viewModelType = Type.GetType(viewModelName);
            if (viewModelType != null)
            {
                var viewModel = Resolver.Resolve(viewModelType) as BaseViewModel;

                view.DataContext = viewModel;
            }
            else
            {

            }
        }
    }
}
