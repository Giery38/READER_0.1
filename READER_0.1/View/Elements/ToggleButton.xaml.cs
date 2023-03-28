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
        public static readonly DependencyProperty ToggledProperty = DependencyProperty.Register("Toggled", typeof(bool), typeof(ToggleButton));
        public bool Toggled
        {
            get { return (bool)GetValue(ToggledProperty); }
            set { SetValue(ToggledProperty, value); }
        }

        private Thickness LeftSide = new Thickness(-39, 0, 0, 0);
        private Thickness RightSide = new Thickness(0, 0, -39, 0);
        private SolidColorBrush Off = new SolidColorBrush(Color.FromRgb(160, 160, 160));
        private SolidColorBrush On = new SolidColorBrush(Color.FromRgb(130, 190, 125));        

        public ToggleButton()
        {
            InitializeComponent();
            Back.Fill = Off;
            Toggled = false;
            Dot.Margin = LeftSide;            
        }             

        private void Dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Toggled)
            {
                Back.Fill = On;
                Toggled = true;
                Dot.Margin = RightSide;
            }
            else
            {
                Back.Fill = Off;
                Toggled = false;
                Dot.Margin = LeftSide;
            }
            /*
            if (Command != null && Command.CanExecute(true) == true)
            {
                Command.Execute(true);                
            }
            */
        }

        private void Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Toggled)
            {
                Back.Fill = On;
                Toggled = true;
                Dot.Margin = RightSide;
            }
            else
            {
                Back.Fill = Off;
                Toggled = false;
                Dot.Margin = LeftSide;
            }
            /*
            if (Command != null && Command.CanExecute(false) == true)
            {
                Command.Execute(false);
            }
            */
        }
    }
}
