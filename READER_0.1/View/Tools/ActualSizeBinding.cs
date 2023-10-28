using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace READER_0._1.View.Tools
{
    public static class ActualSizeBinding // https://stackoverflow.com/questions/63860555/canvas-actualwidth-and-actualheight-passed-into-viewmodel-in-mvvm-way
    {

        public static readonly DependencyProperty ActiveProperty = DependencyProperty.RegisterAttached(
            "Active",
            typeof(bool),
            typeof(ActualSizeBinding),
            new FrameworkPropertyMetadata(OnActualSizePropertyChanged));

        public static bool GetActive(FrameworkElement frameworkElement)
        {
            return (bool)frameworkElement.GetValue(ActiveProperty);
        }

        public static void SetActive(FrameworkElement frameworkElement, bool active)
        {
            frameworkElement.SetValue(ActiveProperty, active);
        }

        public static readonly DependencyProperty BoundActualWidthProperty = DependencyProperty.RegisterAttached(
            "BoundActualWidth",
            typeof(double),
            typeof(ActualSizeBinding));

        public static double GetBoundActualWidth(FrameworkElement frameworkElement)
        {
            return (double)frameworkElement.GetValue(BoundActualWidthProperty);
        }

        public static void SetBoundActualWidth(FrameworkElement frameworkElement, double width)
        {
            frameworkElement.SetValue(BoundActualWidthProperty, width);
        }

        public static readonly DependencyProperty BoundActualHeightProperty = DependencyProperty.RegisterAttached(
            "BoundActualHeight",
            typeof(double),
            typeof(ActualSizeBinding));

        public static double GetBoundActualHeight(FrameworkElement frameworkElement)
        {
            return (double)frameworkElement.GetValue(BoundActualHeightProperty);
        }

        public static void SetBoundActualHeight(FrameworkElement frameworkElement, double height)
        {
            frameworkElement.SetValue(BoundActualHeightProperty, height);
        }

        private static void OnActualSizePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (!(dependencyObject is FrameworkElement frameworkElement))
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                frameworkElement.SizeChanged += OnFrameworkElementSizeChanged;
                UpdateObservedSizesForFrameworkElement(frameworkElement);
            }
            else
            {
                frameworkElement.SizeChanged -= OnFrameworkElementSizeChanged;
            }
        }

        private static void OnFrameworkElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is FrameworkElement frameworkElement)
            {
                UpdateObservedSizesForFrameworkElement(frameworkElement);
            }
        }

        private static void UpdateObservedSizesForFrameworkElement(FrameworkElement frameworkElement)
        {
            frameworkElement.SetCurrentValue(BoundActualWidthProperty, frameworkElement.ActualWidth);
            frameworkElement.SetCurrentValue(BoundActualHeightProperty, frameworkElement.ActualHeight);
        }
    }
}
