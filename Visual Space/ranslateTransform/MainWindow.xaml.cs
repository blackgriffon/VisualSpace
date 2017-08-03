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

namespace ranslateTransform
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

        Rectangle movedRectangle = new Rectangle();

        private void TranslateTransformSample()

        {

            Rectangle originalRectangle = new Rectangle();

            originalRectangle.Width = 200;

            originalRectangle.Height = 50;

            originalRectangle.Fill = Brushes.Yellow;

            LayoutRoot.Children.Add(originalRectangle);



            
            movedRectangle.Width = 200;

            movedRectangle.Height = 50;

            movedRectangle.Fill = Brushes.Blue;

            movedRectangle.Opacity = 0.5;

            TranslateTransform translateTransform1 = new TranslateTransform(50, 20);

            movedRectangle.RenderTransform = translateTransform1;



            LayoutRoot.Children.Add(movedRectangle);

        }

        int i = 50;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            i = i - 20;
            TranslateTransform translateTransform1 = new TranslateTransform(i, 20);



            bluerec.RenderTransform = translateTransform1;

            
        }
    }
}
