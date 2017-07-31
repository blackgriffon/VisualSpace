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
        }



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
