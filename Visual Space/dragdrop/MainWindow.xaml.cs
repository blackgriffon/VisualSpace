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


namespace dragdrop
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }




        


















        // Global variables used to keep track of the // mouse position and whether the object is captured 
        // by the mouse. 

        bool isMouseCaptured;
            double mouseVerticalPosition;
            double mouseHorizontalPosition;

            public void Handle_MouseDown(object sender, MouseEventArgs args)
            {
                Rectangle item = sender as Rectangle;
                mouseVerticalPosition = args.GetPosition(null).Y;
                mouseHorizontalPosition = args.GetPosition(null).X; isMouseCaptured = true;
                item.CaptureMouse();
            }

            public void Handle_MouseMove(object sender, MouseEventArgs args)
            {
                Rectangle item = sender as Rectangle;

                if (isMouseCaptured)
                {
                    // Calculate the current position of the object. 
                    double deltaV = args.GetPosition(null).Y - mouseVerticalPosition;
                    double deltaH = args.GetPosition(null).X - mouseHorizontalPosition;
                    double newTop = deltaV + (double)item.GetValue(Canvas.TopProperty);
                    double newLeft = deltaH + (double)item.GetValue(Canvas.LeftProperty);
                    // Set new position of object. 
                    item.SetValue(Canvas.TopProperty, newTop);
                    item.SetValue(Canvas.LeftProperty, newLeft);
                    // Update position global variables.
                    mouseVerticalPosition = args.GetPosition(null).Y;
                    mouseHorizontalPosition = args.GetPosition(null).X;
                }
            }

            public void Handle_MouseUp(object sender, MouseEventArgs args)
            {
                Rectangle item = sender as Rectangle;
                isMouseCaptured = false;
                item.ReleaseMouseCapture();
                mouseVerticalPosition = -1;
                mouseHorizontalPosition = -1;
            }






        //버튼

        public void Button_MouseDown(object sender, MouseEventArgs args)
        {
            Button item = sender as Button;
            mouseVerticalPosition = args.GetPosition(null).Y;
            mouseHorizontalPosition = args.GetPosition(null).X; isMouseCaptured = true;
            item.CaptureMouse();
        }

        public void Button_MouseMove(object sender, MouseEventArgs args)
        {
            Button item = sender as Button;

            if (isMouseCaptured)
            {
                // Calculate the current position of the object. 
                double deltaV = args.GetPosition(null).Y - mouseVerticalPosition;
                double deltaH = args.GetPosition(null).X - mouseHorizontalPosition;
                double newTop = deltaV + (double)item.GetValue(Canvas.TopProperty);
                double newLeft = deltaH + (double)item.GetValue(Canvas.LeftProperty);
                // Set new position of object. 
                item.SetValue(Canvas.TopProperty, newTop);
                item.SetValue(Canvas.LeftProperty, newLeft);
                // Update position global variables.
                mouseVerticalPosition = args.GetPosition(null).Y;
                mouseHorizontalPosition = args.GetPosition(null).X;
            }
        }

        public void Button_MouseUp(object sender, MouseEventArgs args)
        {
            Button item = sender as Button;
            isMouseCaptured = false;
            item.ReleaseMouseCapture();
            mouseVerticalPosition = -1;
            mouseHorizontalPosition = -1;
        }




    }
    }

