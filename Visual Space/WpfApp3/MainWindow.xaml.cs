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

namespace WpfApp3
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


        ChildWindow cw = null; //차일드 윈도우 선언
        public void button1_Click(object sender, RoutedEventArgs e)
        {
            if (cw == null)
            {
                cw = new ChildWindow(); //자식 인스턴스 생성
                cw.OnChildTextInputEvent += Cw_OnChildTextInputEvent; //차일드윈도우의 OnChildTextInputEvent 이벤트가 발생하면 
                                           //부모에서는 Cw_OnChildTextInputEvent 이벤트를 발생시킬 준비한다. 차일드윈도우에서 발생한 이벤트가 가진 스트링 값을 가지고
                                           //Cw_OnChildTextInputEvent가 발생하게 된다. 텍스트블록의 텍스트에 파라미터 값이 들어오게 되고 자식 창은 닫은 후에 이벤트를 삭제한다.
                cw.Show();
            }
        }

        private void Cw_OnChildTextInputEvent(string Parameters)
        {
            textBlock1.Text = Parameters;
            if (cw != null)
            {
                cw.Close();
                cw.OnChildTextInputEvent -= Cw_OnChildTextInputEvent;                
                cw = null;
            }
        }



    }
    

}
