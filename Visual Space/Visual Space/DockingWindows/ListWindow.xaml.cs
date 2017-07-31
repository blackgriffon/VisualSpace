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

namespace Nollan.Visual_Space.DockingWindows
{
    /// <summary>
    /// ListWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ListWindow : DockingLibrary.DockableContent
    {
        public ListWindow()
        {
            InitializeComponent();
        }



        void OnSelectionChanged(object sender, RoutedEventArgs args)
        {
            string str = list.SelectedItem.ToString();
            MessageBox.Show(str);
        }

    }
}
