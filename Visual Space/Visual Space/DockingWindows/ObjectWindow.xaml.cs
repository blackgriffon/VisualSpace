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
using DockingLibrary;
using SampleExpander;

namespace Nollan.Visual_Space.DockingWindows
{
    /// <summary>
    /// ObjectWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ObjectWindow : DockingLibrary.DockableContent
    {
        public ObjectWindow()
        {
            InitializeComponent();


            


            //   _myCanvas = new Canvas();
            //----------
            //var rect1 = new Rectangle();
            //rect1.Height = rect1.Width = 10;
            //rect1.Fill = Brushes.Blue;

            //Canvas.SetTop(rect1, 8);
            //Canvas.SetLeft(rect1, 8);            

            //_myCanvas.PreviewMouseLeftButtonDown += MyCanvas_PreviewMouseLeftButtonDown;
            //_myCanvas.PreviewMouseMove += MyCanvas_PreviewMouseMove;
            //_myCanvas.PreviewMouseLeftButtonUp += MyCanvas_PreviewMouseLeftButtonUp;
            //PreviewKeyDown += window1_PreviewKeyDown;
            //_myCanvas.Children.Add(rect1);
            //--------
            //    myDockPanel.Children.Add(_myCanvas);
        }

        



        /*
        #region Dragging a WPF user control


        Point anchorPoint;
        Point currentPoint;
        bool isInDrag = false;

        private void root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            anchorPoint = e.GetPosition(null);
            element.CaptureMouse();
            isInDrag = true;
            e.Handled = true;
        }

        private void root_MouseMove(object sender, MouseEventArgs e)
        {
            if (isInDrag)
            {
                var element = sender as FrameworkElement;
                currentPoint = e.GetPosition(null);

                var transform = new TranslateTransform
                {
                    X = (currentPoint.X - anchorPoint.X),
                    Y = (currentPoint.Y - anchorPoint.Y)
                };
                this.RenderTransform = transform;
                anchorPoint = currentPoint;
            }
        }

        private void root_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isInDrag)
            {
                var element = sender as FrameworkElement;
                element.ReleaseMouseCapture();
                isInDrag = false;
                e.Handled = true;
            }
        }

        #endregion

        */


        /*  #region 실험
          
        private bool _isDown;
        private bool _isDragging;
    //    private Canvas _myCanvas;
        private UIElement _originalElement;
        private double _originalLeft;
        private double _originalTop;
        private SimpleCircleAdorner _overlayElement;
        private Point _startPoint;


        private void window1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && _isDragging)
            {
                DragFinished(true);
            }
        }

        private void MyCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDown)
            {
                DragFinished(false);
                e.Handled = true;
            }
        }

        private void DragFinished(bool cancelled)
        {
            Mouse.Capture(null);
            if (_isDragging)
            {
                AdornerLayer.GetAdornerLayer(_overlayElement.AdornedElement).Remove(_overlayElement);

                if (cancelled == false)
                {
                    Canvas.SetTop(_originalElement, _originalTop + _overlayElement.TopOffset);
                    Canvas.SetLeft(_originalElement, _originalLeft + _overlayElement.LeftOffset);
                }
                _overlayElement = null;
            }
            _isDragging = false;
            _isDown = false;
        }

        private void MyCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDown)
            {
                if ((_isDragging == false) &&
                    ((Math.Abs(e.GetPosition(_myCanvas).X - _startPoint.X) >
                      SystemParameters.MinimumHorizontalDragDistance) ||
                     (Math.Abs(e.GetPosition(_myCanvas).Y - _startPoint.Y) >
                      SystemParameters.MinimumVerticalDragDistance)))
                {
                    DragStarted();
                }
                if (_isDragging)
                {
                    DragMoved();
                }
            }
        }

        private void DragStarted()
        {
            _isDragging = true;
            _originalLeft = Canvas.GetLeft(_originalElement);
            _originalTop = Canvas.GetTop(_originalElement);

            _overlayElement = new SimpleCircleAdorner(_originalElement);
            var layer = AdornerLayer.GetAdornerLayer(_originalElement);
            layer.Add(_overlayElement);
        }

        private void DragMoved()
        {
            var currentPosition = Mouse.GetPosition(_myCanvas);

            _overlayElement.LeftOffset = currentPosition.X - _startPoint.X;
            _overlayElement.TopOffset = currentPosition.Y - _startPoint.Y;

            
         //   TranslateTransform ttf = new TranslateTransform();
         
        }

        private void MyCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == _myCanvas)
            {
            }
            else
            {
                _isDown = true;
                _startPoint = e.GetPosition(_myCanvas);
                _originalElement = e.Source as UIElement;
                _myCanvas.CaptureMouse();
                e.Handled = true;
            }
        }
       
#endregion */




        //public class MultiplyConverter : IMultiValueConverter
        //{


        //    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        double result = 1.0;
        //        for (int i = 0; i < values.Length; i++)
        //        {
        //            if (values[i] is double)
        //                result *= (double)values[i];
        //        }

        //        return result;
        //    }



        //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        //    {
        //        throw new Exception("Not implemented");
        //    }
        //}




    }
}
