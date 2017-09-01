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
using System.Windows.Shapes;
using MahApps.Metro;
using MahApps.Metro.Controls;
using System.IO;

namespace Nollan.Visual_Space.DockingWindows
{
    /// <summary>
    /// HelpWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HelpWindow : MetroWindow
    {
        public HelpWindow()
        {
            InitializeComponent();
            this.Loaded += HelpWindow_Loaded;
           
           // texttest();
        }

        private void HelpWindow_Loaded(object sender, RoutedEventArgs e)
        {
            tv_Home_Selected(sender, e); //처음 로딩할 때 시작화면 선택한 상태가 디폴트 상태로 하기 위해.
        }

        public void PopUp()
        {
            Name_helpwindow.Show();
        }

        //public void texttest()
        //{
        //    string str = null;
        //    for (int i = 0; i < 30; i++)
        //    {
        //        str += $"{i}\r\n";

        //    }

        //    tbk_explain.Text = str;

        //}



        string bg_homeFilePath = @"\Help\home.png";
        string exp_homeFilePath = @"\Help\background1.png";
        private void tv_Home_Selected(object sender, RoutedEventArgs e)
        {
            //  string img_helpPath = @"D:\winshare\mainproject\VisualSpace\Visual Space\Visual Space\Help\home.png";



            grid_img.Background = GetBrushFromPath(bg_homeFilePath);
            grid_exp.Background = GetBrushFromPath(exp_homeFilePath);


        }

        string bg_OneFilePath = @"\Help\one_bg.png";
        string exp_OneFilePath = @"\Help\one_exp.png";
        private void tv_One_Selected(object sender, RoutedEventArgs e)
        {
            grid_img.Background = GetBrushFromPath(bg_OneFilePath);
            grid_exp.Background = GetBrushFromPath(exp_OneFilePath);
        }

        string bg_TwoFilePath = @"\Help\two_bg.png";
        string exp_TwoFilePath = @"\Help\two_exp.png";
        private void tv_Two_Selected(object sender, RoutedEventArgs e)
        {
            grid_img.Background = GetBrushFromPath(bg_TwoFilePath);
            grid_exp.Background = GetBrushFromPath(exp_TwoFilePath);
        }

        string bg_ThreeFilePath = @"\Help\three_bg.png";
        string exp_ThreeFilePath = @"\Help\three_exp.png";
        private void tv_Three_Selected(object sender, RoutedEventArgs e)
        {
            grid_img.Background = GetBrushFromPath(bg_ThreeFilePath);
            grid_exp.Background = GetBrushFromPath(exp_ThreeFilePath);
        }

        string bg_FourFilePath = @"\Help\four_bg.png";
        string exp_FourFilePath = @"\Help\four_exp.png";
        private void tv_Four_Selected(object sender, RoutedEventArgs e)
        {
            grid_img.Background = GetBrushFromPath(bg_FourFilePath);
            grid_exp.Background = GetBrushFromPath(exp_FourFilePath);
        }

        string bg_FiveFilePath = @"\Help\five_bg.png";
        string exp_FiveFilePath = @"\Help\five_exp.png";
        private void tv_Five_Selected(object sender, RoutedEventArgs e)
        {
            grid_img.Background = GetBrushFromPath(bg_FiveFilePath);
            grid_exp.Background = GetBrushFromPath(exp_FiveFilePath);
        }



        public ImageBrush GetBrushFromPath(string Path)
        {  
        

#if DEBUG

            string dir = Directory.GetCurrentDirectory();
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir + Path;

#else
            string dir = Directory.GetCurrentDirectory();
            dir = dir +  Path;    
#endif


            BitmapImage bit = new BitmapImage();
            bit.BeginInit();
            bit.UriSource = new Uri(dir, UriKind.RelativeOrAbsolute);
            bit.EndInit();

            ImageBrush brush = new ImageBrush(bit);

            return brush;
        }



    }
}
