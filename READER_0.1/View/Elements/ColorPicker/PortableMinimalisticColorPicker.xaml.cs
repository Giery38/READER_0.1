using ColorPicker.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace ColorPicker
{
    public partial class PortableMinimalisticColorPicker : DualPickerControlBase
    {
        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register(nameof(SmallChange), typeof(double), typeof(PortableMinimalisticColorPicker),
                new PropertyMetadata(1.0));
        public double SmallChange
        {
            get => (double)GetValue(SmallChangeProperty);
            set => SetValue(SmallChangeProperty, value);
        }

        public static readonly DependencyProperty ShowAlphaProperty =
            DependencyProperty.Register(nameof(ShowAlpha), typeof(bool), typeof(PortableMinimalisticColorPicker),
                new PropertyMetadata(true));
        public bool ShowAlpha
        {
            get => (bool)GetValue(ShowAlphaProperty);
            set => SetValue(ShowAlphaProperty, value);
        }

        public static readonly DependencyProperty PickerTypeProperty
            = DependencyProperty.Register(nameof(PickerType), typeof(PickerType), typeof(PortableMinimalisticColorPicker),
                new PropertyMetadata(PickerType.HSV));            
        public PickerType PickerType
        {
            get => (PickerType)GetValue(PickerTypeProperty);
            set => SetValue(PickerTypeProperty, value);
        }
        private static Dictionary<string, Color> standardColors = new Dictionary<string, Color>()
        {
            { "Темно-красный", new Color() { R = 151, G = 51, B = 0, A = 255 } },
            { "Красный", new Color() { R = 255, G = 0, B = 0, A = 255 } },
            { "Оранжевый", new Color() { R = 255, G = 102, B = 0, A = 255 } },
            { "Желтый", new Color() { R = 255, G = 255, B = 0, A = 255 } },
            { "Светло-зеленый", new Color() { R = 146, G = 208, B = 80, A = 255 } },
            { "Зеленый", new Color() { R = 0, G = 176, B = 80, A = 255 } },
            { "Светло-синий", new Color() { R = 0, G = 176, B = 240, A = 255 } },
            { "Синий", new Color() { R = 0, G = 112, B = 192, A = 255 } },
            { "Темно-синий", new Color() { R = 0, G = 32, B = 96, A = 255 } },
            { "Сиреневый", new Color() { R = 112, G = 48, B = 160, A = 255 } }
        };
        public static readonly DependencyProperty StandardColorsProperty =
         DependencyProperty.Register(nameof(StandardColors), typeof(Dictionary<string, Color>), typeof(PortableMinimalisticColorPicker), new PropertyMetadata(standardColors));
        public Dictionary<string, Color> StandardColors
        {
            get { return (Dictionary<string, Color>)GetValue(StandardColorsProperty); }
            set { SetValue(StandardColorsProperty, value); }
        }

        public static readonly DependencyProperty RecentColorsProperty =
         DependencyProperty.Register(nameof(RecentColors), typeof(ObservableCollection<Color>), typeof(PortableMinimalisticColorPicker));
        public ObservableCollection<Color> RecentColors
        {
            get { return (ObservableCollection<Color>)GetValue(RecentColorsProperty); }
            set { SetValue(RecentColorsProperty, value); }

        }

        public static readonly DependencyProperty WidthWindowProperty =
           DependencyProperty.Register(nameof(WidthWindow), typeof(double), typeof(PortableMinimalisticColorPicker));
               
        public double WidthWindow
        {
            get => (double)GetValue(WidthWindowProperty);
            set => SetValue(WidthWindowProperty, value);
        }

        public static readonly DependencyProperty MinHeightWindowProperty =
          DependencyProperty.Register(nameof(MinHeightWindow), typeof(double), typeof(PortableMinimalisticColorPicker));              
        public double MinHeightWindow
        {
            get => (double)GetValue(MinHeightWindowProperty);
            set => SetValue(MinHeightWindowProperty, value);
        }
        public static readonly DependencyProperty BackgroundWindowProperty =
        DependencyProperty.Register(nameof(BackgroundWindow), typeof(Brush), typeof(PortableMinimalisticColorPicker));
        public Brush BackgroundWindow
        {
            get => (Brush)GetValue(MinHeightWindowProperty);
            set => SetValue(MinHeightWindowProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusWindowProperty =
        DependencyProperty.Register(nameof(CornerRadiusWindow), typeof(CornerRadius), typeof(PortableMinimalisticColorPicker));
        public CornerRadius CornerRadiusWindow
        {
            get { return (CornerRadius)GetValue(CornerRadiusWindowProperty); }
            set { SetValue(CornerRadiusWindowProperty, value); }

        }
        public PortableMinimalisticColorPicker()
        {
            InitializeComponent();
        }
    }
}
