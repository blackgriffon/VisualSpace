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

namespace Nollan.Visual_Space
{
    /// <summary>
    /// MyDrag.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MyDrag : UserControl
    {
        public MyDrag()
        {
            InitializeComponent();
        }

        bool flag_Click = false;
        Point beforePositon;

        void DragPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            flag_Click = false;

                if (!flag_Click)
            {
                DragPanel.ReleaseMouseCapture();
                DragPanel.MouseMove -= new MouseEventHandler(DragPanel_MouseMove);
            }
        
        }

        void DragPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //   DragPanel.VerticalAlignment = VerticalAlignment.Top;
            //  DragPanel.HorizontalAlignment = HorizontalAlignment.Left;
            flag_Click = true;
            if (flag_Click)
            {
                DragPanel.CaptureMouse();
                beforePositon = e.GetPosition(LayoutRoot);
                DragPanel.MouseMove += new MouseEventHandler(DragPanel_MouseMove);

            }


        }
        //LayoutRoot

        void DragPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (flag_Click)
            {
                beforePositon.X = e.GetPosition(LayoutRoot).X - beforePositon.X;
                beforePositon.Y = e.GetPosition(LayoutRoot).Y - beforePositon.Y;
                Thickness temp = this.Margin;
                temp.Left += beforePositon.X;
                temp.Top += beforePositon.Y;
                this.Margin = temp;
                beforePositon = e.GetPosition(LayoutRoot);

            }


        }


 


    }
}
