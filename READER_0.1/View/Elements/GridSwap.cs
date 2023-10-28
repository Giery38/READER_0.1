using DocumentFormat.OpenXml.Drawing.Charts;
using READER_0._1.Command.CommandMain;
using READER_0._1.View.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static READER_0._1.View.Elements.SwapWindow;

namespace READER_0._1.View.Elements
{
    public  partial class GridSwap : Grid
    {
        static GridSwap() 
        {            
            Canvas.LeftProperty.OverrideMetadata(typeof(GridSwap), new FrameworkPropertyMetadata((o, e) => ((GridSwap)o).LeftPropertyChanged((double)e.NewValue)));
            Canvas.RightProperty.OverrideMetadata(typeof(GridSwap), new FrameworkPropertyMetadata((o, e) => ((GridSwap)o).RightPropertyChanged((double)e.NewValue)));
            Canvas.TopProperty.OverrideMetadata(typeof(GridSwap), new FrameworkPropertyMetadata((o, e) => ((GridSwap)o).TopPropertyChanged((double)e.NewValue)));
            Canvas.BottomProperty.OverrideMetadata(typeof(GridSwap), new FrameworkPropertyMetadata((o, e) => ((GridSwap)o).BottomPropertyChanged((double)e.NewValue)));            
        }        
        public GridSwap()
        {
            Loaded += GridSwap_Loaded;                      
        }
        private DrawingBrush opacityMask;
        private void GridSwap_Loaded(object sender, RoutedEventArgs e)
        {
            opacityMask = OpacityMask as DrawingBrush;
            if (opacityMask != null)
            {
                switch (SwapOrientation)
                {
                    case SwapOrientations.Left:
                        opacityMask.AlignmentX = AlignmentX.Left;
                        opacityMask.AlignmentY = AlignmentY.Top;
                        break;
                    case SwapOrientations.Right:
                        opacityMask.AlignmentX = AlignmentX.Right;
                        opacityMask.AlignmentY = AlignmentY.Top;
                        break;
                    case SwapOrientations.Top:
                        opacityMask.AlignmentX = AlignmentX.Center;
                        opacityMask.AlignmentY = AlignmentY.Top;
                        break;
                    case SwapOrientations.Bottom:
                        opacityMask.AlignmentX = AlignmentX.Center;
                        opacityMask.AlignmentY = AlignmentY.Bottom;
                        break;
                }
                DrawingGroup drawingGroup = opacityMask.Drawing as DrawingGroup;
                if (drawingGroup != null && drawingGroup.Children.Count > 0)
                {
                    GeometryDrawing geometryDrawing = drawingGroup.Children[0] as GeometryDrawing;

                    if (geometryDrawing != null)
                    {
                        RectangleGeometry rectangleGeometry = geometryDrawing.Geometry as RectangleGeometry;

                        if (rectangleGeometry != null)
                        {
                            rectOpacity = rectangleGeometry;                            
                            if (false == DesignerProperties.GetIsInDesignMode(rectangleGeometry))
                            {
                                rectOpacity.Rect = new Rect(0, 0, 0, 0);
                            }
                            else
                            {
                                rectOpacity.Rect = new Rect(0, 0, 3000, 3000);
                            }                                                    
                        }
                    }
                }
            }
        }       
        public static readonly DependencyProperty StateProperty =
      DependencyProperty.Register(nameof(State), typeof(ShowState), typeof(GridSwap));
        public ShowState State
        {
            get
            {
                return (ShowState)GetValue(StateProperty);
            }
            set
            {
                if ((ShowState)GetValue(StateProperty) != value)
                {                   
                    SetValue(StateProperty, value);                   
                }
            }
        }
        public static readonly DependencyProperty SwapOrientationProperty =
                 DependencyProperty.Register(nameof(SwapOrientation), typeof(SwapOrientations), typeof(GridSwap));
        public SwapOrientations SwapOrientation
        {
            get
            {
                return (SwapOrientations)GetValue(SwapOrientationProperty);
            }
            set
            {               
                SetValue(SwapOrientationProperty, value);
            }
        }
        private RectangleGeometry rectOpacity;
        private void LeftPropertyChanged(double left)
        {
            if (rectOpacity != null)
            {
                double value = ActualWidth + left;
                rectOpacity.Rect = new Rect(0, 0, value, ActualHeight);

            }            
        }
        private void RightPropertyChanged(double right)
        {
            if (rectOpacity != null)
            {
                double value = ActualWidth + right;
                rectOpacity.Rect = new Rect(0, 0, value, ActualHeight);

            }

        }
        private void TopPropertyChanged(double top)
        {
            if (rectOpacity != null)
            {
                double value = ActualHeight + top;
                rectOpacity.Rect = new Rect(0, 0, ActualWidth, value);

            }
        }
        private void BottomPropertyChanged(double bottom)
        {
            if (rectOpacity != null)
            {
                double value = ActualHeight + bottom;
                rectOpacity.Rect = new Rect(0, 0, ActualWidth, value);
            }
        }              
    }
}
