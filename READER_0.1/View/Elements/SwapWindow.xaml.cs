using READER_0._1.View.Tools;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static READER_0._1.View.Elements.SwapWindow;

namespace READER_0._1.View.Elements
{
    /// <summary>
    /// Логика взаимодействия для SwapWindow.xaml
    /// </summary>
    public partial class SwapWindow : UserControl
    {
        public SwapWindow()
        {
            InitializeComponent();           
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetSwapOrientation(SwapOrientation);
            state = ShowState.Hide;
            IsHitTestVisible = false;
            FrameworkElement parent = Parent as FrameworkElement;
            if (parent != null && HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                Content.Width = parent.ActualWidth;
                Width = parent.ActualWidth;
            }
            else
            {
                Width = Content.ActualWidth;
            }          
            if (parent != null && VerticalAlignment == VerticalAlignment.Stretch)
            {               
                Content.Height = parent.ActualHeight;
                Height = parent.ActualHeight;
            }
            else
            {
                Height = Content.ActualHeight;
            }            
            if (VerticalAlignment == VerticalAlignment.Stretch || HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                parent.SizeChanged += Parent_SizeChanged;
            }
            Swap.SwapOrientation = SwapOrientation;
            Swap.State = ShowState.Hide;
            subSwapWindow = Helper.FindChild<SwapWindow>(this);            
        }
     
        private List<SwapWindow> subSwapWindow;
        private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged == true && HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                Content.Width = e.NewSize.Width;
                Width = Content.Width;
            }
            if (e.HeightChanged == true && VerticalAlignment == VerticalAlignment.Stretch)
            {
                Content.Height = e.NewSize.Height;
                Height = Content.Height;
            }
        }

        public static readonly DependencyProperty CustomContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(SwapWindow));
        public object CustomContent
        {
            get
            {
                return (object)GetValue(CustomContentProperty);
            }
            set
            {
                SetValue(CustomContentProperty, value);
            }
        }        
        public static readonly DependencyProperty SwapOrientationProperty =
          DependencyProperty.Register(nameof(SwapOrientation), typeof(SwapOrientations),
          typeof(SwapWindow), new PropertyMetadata(SwapOrientations.Bottom));
        public SwapOrientations SwapOrientation
        {
            get
            {
                return (SwapOrientations)GetValue(SwapOrientationProperty);
            }
            set
            {                
                SetSwapOrientation(value);
                SetValue(SwapOrientationProperty, value); 
            }
        }
                
        private void SetSwapOrientation(SwapOrientations orientation)
        {
            Storyboard storyboardShow = (Storyboard)UserControl.Resources["Show"];
            DoubleAnimation doubleAnimationShow = storyboardShow.Children.First() as DoubleAnimation;
            Storyboard storyboardHide = (Storyboard)UserControl.Resources["Hide"];
            DoubleAnimation doubleAnimationHide = storyboardHide.Children.First() as DoubleAnimation;
            switch (orientation)
            {
                case SwapOrientations.Left:
                    Storyboard.SetTargetProperty(storyboardShow, new PropertyPath("(Canvas.Right)"));
                    Storyboard.SetTargetProperty(storyboardHide, new PropertyPath("(Canvas.Right)"));
                    doubleAnimationShow.From = -Swap.ActualWidth;
                    doubleAnimationHide.To = -Swap.ActualWidth;
                    Canvas.SetRight(Swap, -Swap.ActualWidth);                    
                    break;
                case SwapOrientations.Right:
                    Storyboard.SetTargetProperty(storyboardShow, new PropertyPath("(Canvas.Left)"));
                    Storyboard.SetTargetProperty(storyboardHide, new PropertyPath("(Canvas.Left)"));
                    doubleAnimationShow.From = -Swap.ActualWidth;
                    doubleAnimationHide.To = -Swap.ActualWidth;
                    Canvas.SetLeft(Swap, -Swap.ActualWidth);
                    break;
                case SwapOrientations.Top:
                    Storyboard.SetTargetProperty(storyboardShow, new PropertyPath("(Canvas.Bottom)"));
                    Storyboard.SetTargetProperty(storyboardHide, new PropertyPath("(Canvas.Bottom)"));
                    doubleAnimationShow.From = -Swap.ActualHeight;
                    doubleAnimationHide.To = -Swap.ActualHeight;
                    Canvas.SetBottom(Swap, -Swap.ActualHeight);                    
                    break;
                case SwapOrientations.Bottom:
                    Storyboard.SetTargetProperty(storyboardShow, new PropertyPath("(Canvas.Top)"));
                    Storyboard.SetTargetProperty(storyboardHide, new PropertyPath("(Canvas.Top)"));
                    doubleAnimationShow.From = -Swap.ActualHeight;
                    doubleAnimationHide.To = -Swap.ActualHeight;
                    Canvas.SetTop(Swap, -Swap.ActualHeight);
                    break;
            }
        }
        public static readonly DependencyProperty DurationProperty =
         DependencyProperty.Register(nameof(Duration), typeof(Duration),
         typeof(SwapWindow), new PropertyMetadata(new Duration(new TimeSpan(0,0,0))));
        public Duration Duration
        {
            get
            {
                return (Duration)GetValue(DurationProperty);
            }
            set
            {
                SetValue(DurationProperty, value);
            }
        }
        public void Show()
        {
            state = ShowState.Opening;
            Storyboard storyboardShow = (Storyboard)UserControl.Resources["Show"];
            storyboardShow.Begin();            
        }
        private void Show_Completed(object sender, EventArgs e)
        {
            if (State != ShowState.Hiding)
            {
                state = ShowState.Open;             
                if (MouseLeaveHide == true && IsMouseOver == false && GetOpenChild().Count == 0)
                {
                    Hide();
                }
            }           
        }       
        public void Hide()
        {
            state = ShowState.Hiding;            
            Storyboard storyboardHide = (Storyboard)UserControl.Resources["Hide"];
            DoubleAnimation doubleAnimationHide = storyboardHide.Children.First() as DoubleAnimation;      
            double newValue = 0;
            double newValue2 = Canvas.GetLeft(Swap);
            double newValue3 = Canvas.GetRight(Swap);
            double newValue4 = Canvas.GetTop(Swap);
            double newValue5 = Canvas.GetBottom(Swap);
            switch (SwapOrientation)
            {
                case SwapOrientations.Left:
                    newValue = Canvas.GetRight(Swap);
                    break;
                case SwapOrientations.Right:
                    newValue = Canvas.GetLeft(Swap);
                    break;
                case SwapOrientations.Top:
                    newValue = Canvas.GetBottom(Swap);
                    break;
                case SwapOrientations.Bottom:
                    newValue = Canvas.GetTop(Swap);
                    break;
            }
            doubleAnimationHide.From = newValue;
            storyboardHide.Begin();            
        }
        private void Hide_Completed(object sender, EventArgs e)
        {
            if (State != ShowState.Opening)
            {
                Storyboard storyboardHide = (Storyboard)UserControl.Resources["Hide"];
                DoubleAnimation doubleAnimationHide = storyboardHide.Children.First() as DoubleAnimation;
                if (doubleAnimationHide.From != 0)
                {
                    doubleAnimationHide.From = 0;
                }
                state = ShowState.Hide;
                subSwapWindow.ForEach(item => item.Hide());
            }            
        }        
        public static readonly DependencyProperty StateProperty =
        DependencyProperty.Register(nameof(State), typeof(ShowState), typeof(SwapWindow), new PropertyMetadata(ShowState.Hide));
        private ShowState state
        {
            get
            {
                return (ShowState)GetValue(StateProperty);
            }
            set
            {
                ShowState oldValue = (ShowState)GetValue(StateProperty);
                if (oldValue != value)
                {                    
                    switch (value)
                    {
                        case ShowState.Hide:
                            IsHitTestVisible = false;
                            break;
                        case ShowState.Hiding:
                            IsHitTestVisible = false;
                            break;
                        case ShowState.Open:
                            IsHitTestVisible = true;
                            break;
                        case ShowState.Opening:
                            IsHitTestVisible = true;
                            break;
                    }
                    SetValue(StateProperty, value);
                    OnPropertyChanged(new DependencyPropertyChangedEventArgs(StateProperty, oldValue, value));
                    ShowStateChange?.Invoke(this, value);
                }               
            }
        }
        /// <summary>
        /// Read only value
        /// </summary>
        public ShowState State
        {
            get
            {
                return (ShowState)GetValue(StateProperty);
            }
            set
            {
                throw new Exception("Read only value");
            }
        }
        public delegate void RoutedEvent(object sender, ShowState e);
        public event RoutedEvent ShowStateChange;
        public static readonly DependencyProperty MouseLeaveHideProperty =
       DependencyProperty.Register(nameof(MouseLeaveHide), typeof(bool), typeof(SwapWindow));
        public bool MouseLeaveHide
        {
            get
            {
                return (bool)GetValue(MouseLeaveHideProperty);
            }
            set
            {                
                SetValue(MouseLeaveHideProperty, value);
            }
        }      
        public enum ShowState
        {
            Hide,
            Hiding,
            Open,
            Opening
        }
        public enum SwapOrientations
        {
            Left,
            Right,
            Top,
            Bottom            
        }

        #region SetReadOnly
        
        public new Brush Background
        {
            get
            {
                return (Brush)GetValue(BackgroundProperty);             
            }
            private set
            {
                SetValue(BackgroundProperty, value);
            }
        }     
        public new bool IsHitTestVisible
        {
            get
            {
              return (bool)GetValue(IsHitTestVisibleProperty);
            }
            private set
            {
                SetValue(IsHitTestVisibleProperty, value);
            }
        }
        public new double Width
        {
            get
            {
                return (double)GetValue(WidthProperty);
            }
            private set
            {
                SetValue(WidthProperty, value);
            }
        }
        public new double MinWidth
        {
            get
            {
                return (double)GetValue(MinWidthProperty);
            }
            private set
            {
                SetValue(MinWidthProperty, value);
            }
        }
        public new double Height
        {
            get
            {
                return (double)GetValue(HeightProperty);
            }
            private set
            {
                SetValue(HeightProperty, value);
            }
        }
        public new double MinHeight
        {
            get
            {
                return (double)GetValue(MinHeightProperty);
            }
            private set
            {
                SetValue(MinHeightProperty, value);
            }
        }
        #endregion

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if ((State == ShowState.Open || State == ShowState.Opening) && MouseLeaveHide == true &&
                 GetOpenChild().Count == 0)
            {
                Hide();
            }
        }
        private List<SwapWindow> GetOpenChild()
        {
            return subSwapWindow.FindAll(item => item.State == ShowState.Opening || item.State == ShowState.Open).ToList();
        }
    }
}
