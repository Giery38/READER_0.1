using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace READER_0._1.View.Elements
{
    public partial class ToggleButton : UserControl
    {
        /*
       public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ToggleButton));
       public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(ToggleButton));
       public ICommand Command
       {
           get { return (ICommand)GetValue(CommandProperty); }
           set { SetValue(CommandProperty, value); }
       }

       public object CommandParameter
       {
           get { return (object)GetValue(CommandParameterProperty); }
           set { SetValue(CommandParameterProperty, value); }
       }
       */
        public new static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(ToggleButton));
        public new Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }
        public new static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(ToggleButton));
        public new Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }
        public static readonly DependencyProperty ToggledProperty = DependencyProperty.Register("Toggled", typeof(bool), typeof(ToggleButton));
        public bool Toggled
        {
            get { return (bool)GetValue(ToggledProperty); }
            set 
            {
                if (Toggled != value)
                {
                    SetValue(ToggledProperty, value);                    
                }              
            }
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ToggleButton));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        private Thickness LeftSide = new Thickness(0, 0, 0, 0);
        private Thickness RightSide = new Thickness(0, 0, 0, 0);
        private SolidColorBrush Off = new SolidColorBrush(Color.FromRgb(160, 160, 160));
        private SolidColorBrush On = new SolidColorBrush(Color.FromRgb(130, 190, 125));
        private Brush startBackBorderBrush;

        public ToggleButton()
        {
            InitializeComponent();
            Back.Background = Off;                  
            startBackBorderBrush = Back.BorderBrush;
            Loaded += ToggleButton_Loaded;
        }

        private void ToggleButton_Loaded(object sender, RoutedEventArgs e)
        {
            double dotWidth = (Back.ActualWidth / 2) - Back.BorderThickness.Top - Back.Padding.Top;
            Dot.Width = dotWidth;            
            LeftSide = new Thickness((int)-dotWidth, 0, 0, 0);
            RightSide = new Thickness(0, 0, (int)-dotWidth, 0);
            Dot.Margin = LeftSide;
            SetToggled(Toggled);
        }

        private void Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (!Toggled)
            {
                SetToggled(true);
            }
            else
            {
                SetToggled(false);
            }
        } 
        private void SetToggled(bool toggled)
        {

            if (toggled == true)
            {
                Back.Background = On;
                Toggled = true;
                Dot.Margin = RightSide;
            }
            else
            {
                Back.Background = Off;
                Toggled = false;
                Dot.Margin = LeftSide;
            }
        }
    }
}
