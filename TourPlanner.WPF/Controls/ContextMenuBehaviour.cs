using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace TourPlanner.WPF.Views
{

    // Taken and adapted from
    // https://stackoverflow.com/questions/555252/show-contextmenu-on-left-click-using-only-xaml/29123964#29123964
    public static class ContextMenuBehaviour
    {
        public static bool GetIsLeftClickEnabled(DependencyObject obj)
        {
            var nullableValue = obj.GetValue(IsLeftClickEnabledProperty) as bool?;

            if (nullableValue is null)
            {
                return false;
            }

            bool value = (bool) nullableValue;
            return value;
        }

        public static void SetIsLeftClickEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLeftClickEnabledProperty, value);
        }

        public static readonly DependencyProperty IsLeftClickEnabledProperty = DependencyProperty
            .RegisterAttached("IsLeftClickEnabled", 
                              typeof(bool), 
                              typeof(ContextMenuBehaviour),
                              new UIPropertyMetadata(false, OnIsLeftClickEnabledChanged));

        private static void OnIsLeftClickEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not UIElement element)
            {
                return;
            }

            bool IsEnabled = e.NewValue is bool enabled && enabled;
            var button = element as ButtonBase;

            if (IsEnabled)
            {
                if (button is not null)
                {
                    button.Click += OnMouseLeftButtonUp;
                }
                else
                {
                    element.MouseLeftButtonUp += OnMouseLeftButtonUp;
                }
            }
            else
            {
                if (button is not null)
                {
                    button.Click -= OnMouseLeftButtonUp;
                } 
                else
                {
                    element.MouseLeftButtonUp -= OnMouseLeftButtonUp;
                }
            }
        }

        private static void OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement element)
            {
                return;
            }

            // if we use binding in our context menu, then it's DataContext won't be set when we show the menu on left click
            // (it seems setting DataContext for ContextMenu is hardcoded in WPF when user right clicks on a control, although I'm not sure)
            // so we have to set up ContextMenu.DataContext manually here
            if (element.ContextMenu.DataContext is null)
            {
                element.ContextMenu.SetBinding(FrameworkElement.DataContextProperty, new Binding { Source = element.DataContext });
            }

            element.ContextMenu.IsOpen = true;
        }

    }
}
