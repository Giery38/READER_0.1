using ColorPicker;
using ColorPicker.Models;
using READER_0._1.View.Elements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;

namespace ColorPicker
{
    /// <summary>
    /// Логика взаимодействия для MinimalisticColorPicker.xaml
    /// </summary>
    public partial class MinimalisticColorPicker : DualPickerControlBase
    {        
        public MinimalisticColorPicker() : base()
        {
            InitializeComponent();                       
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
          DependencyProperty.Register(nameof(StandardColors), typeof(Dictionary<string, Color>), typeof(MinimalisticColorPicker), new PropertyMetadata(standardColors));

        public Dictionary<string, Color> StandardColors
        {
            get { return (Dictionary<string, Color>)GetValue(StandardColorsProperty); }
            set { SetValue(StandardColorsProperty, value); }
        }
        public static readonly DependencyProperty RecentColorsProperty =
         DependencyProperty.Register(nameof(RecentColors), typeof(ObservableCollection<Color>), typeof(MinimalisticColorPicker));       
        public ObservableCollection<Color> RecentColors
        {
            get { return (ObservableCollection<Color>)GetValue(RecentColorsProperty); }
            set { SetValue(RecentColorsProperty, value); }
           
        }
        public static readonly DependencyProperty CornerRadiusProperty =
       DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(MinimalisticColorPicker));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }

        }

        public new static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(MinimalisticColorPicker));
        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        private void BaseColoors_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                KeyValuePair<string, Color> selectedItem = (KeyValuePair<string, Color>)e.AddedItems[0];                
                Color selectedColor = selectedItem.Value;
                SelectedColor = selectedColor;
            }
        }
       
    }
}
