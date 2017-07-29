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

namespace VisualSpace.UnityViewer
{
    /// <summary>
    /// ExeViewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ExeViewer : UserControl
    {
        public static UnityExe uc = new UnityExe(@"C:\Users\TAEWOO\Desktop\test4.exe");


        public ExeViewer()
        {
            InitializeComponent();
            host.Child = uc;
            Application.Current.Exit += Current_Exit;
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            uc.CloseExe();
        }
    }
}
