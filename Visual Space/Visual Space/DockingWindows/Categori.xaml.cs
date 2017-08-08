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

namespace Nollan.Visual_Space.DockingWindows
{
    /// <summary>
    /// Categori.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Categori : UserControl
    {
        public Categori()
        {
            InitializeComponent();
           
        }
    


        ///pictures/chair.PNG
        public Categori(string _ObjectType, string _FilePath, string _VisualName) //오브젝트 타입, 경로, 비쥬얼네임.
        {
            InitializeComponent();

            ObjectInfo NewInfo = new ObjectInfo();
            NewInfo.ObjectType = _ObjectType;
            NewInfo.FilePath = _FilePath;
            NewInfo.VisualName = _VisualName;
            

            //Image myImage3 = new Image();
            //상대 파일경로의 png를 bitmap으로 변환.
            BitmapImage bitimage = new BitmapImage();
            bitimage.BeginInit();
            bitimage.UriSource = new Uri(_FilePath, UriKind.Relative);
            bitimage.EndInit();


            //bitmap으로 바꾼 걸 카테고리의 이미지.source에 대입.
            img_cate.Stretch = Stretch.UniformToFill;
            img_cate.Source = bitimage;
            img_cate.Width = 60;
            img_cate.Height = 60;
            img_cate.Tag = NewInfo; //태그 정보 입력

            img_cate.ToolTip = NewInfo.VisualName; //비주얼 이름으로 툴팁 팝업


        }




        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Package the data.
                DataObject data = new DataObject();
                //  data.SetData(DataFormats.StringFormat, ractangleUI.Fill.ToString());
                //  data.SetData("Double", ractangleUI.Height);
                data.SetData("Object", this);

                // Inititate the drag-and-drop operation.
                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);


                /*
                 재정의된 이 OnMouseMove는 다음 작업을 수행합니다.

    마우스 왼쪽 단추를 누른 채 마우스를 이동하고 있는지 여부를 확인합니다.

    Circle 데이터를 DataObject에 패키지합니다. 이 예제의 경우 Circle 컨트롤은 세 개의 데이터 항목, 즉 채우기 색의 문자열 표현, 높이의 double 형식 표현 및 컨트롤 자체의 복사본을 패키지합니다.

    정적 DragDrop.DoDragDrop 메서드를 호출하여 끌어서 놓기 작업을 시작합니다. DoDragDrop 메서드에는 다음 세 개의 매개 변수를 전달합니다.

        dragSource - 이 컨트롤에 대한 참조입니다.

        data - 이전 코드에서 만든 DataObject입니다.

        allowedEffects - 허용되는 끌어서 놓기 작업으로, Copy 또는 Move입니다.
             
             
             */

            }
        }



        //놓기 대상의 DragOver 이벤트 처리기에 설정된 Effects 값을 확인합니다.
        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);
            // These Effects values are set in the drop target's
            // DragOver event handler.
            if (e.Effects.HasFlag(DragDropEffects.Copy))
            {
                Mouse.SetCursor(Cursors.Cross);
            }
            else if (e.Effects.HasFlag(DragDropEffects.Move))
            {
                Mouse.SetCursor(Cursors.Pen);
            }
            else
            {
                Mouse.SetCursor(Cursors.No);
            }
            e.Handled = true;


            /*
             재정의된 이 OnGiveFeedback는 다음 작업을 수행합니다.

    놓기 대상의 DragOver 이벤트 처리기에 설정된 Effects 값을 확인합니다.

    Effects 값에 따라 사용자 지정 커서를 설정합니다. 이 커서는 데이터를 놓을 때의 결과에 대한 시각적 피드백을 사용자에게 제공하기 위한 것입니다.
             
             */

        }





        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            // If the DataObject contains string data, extract it.
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                string dataString = (string)e.Data.GetData(DataFormats.StringFormat);

                // If the string can be converted into a Brush, 
                // convert it and apply it to the ellipse.
                BrushConverter converter = new BrushConverter();
                if (converter.IsValid(dataString))
                {
                    Brush newFill = (Brush)converter.ConvertFromString(dataString);
              //      ractangleUI.Fill = newFill;

                    // Set Effects to notify the drag source what effect
                    // the drag-and-drop operation had.
                    // (Copy if CTRL is pressed; otherwise, move.)
                    if (e.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
                    {
                        e.Effects = DragDropEffects.Copy;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                }
            }
            e.Handled = true;


            /*
             재정의된 이 OnDrop는 다음 작업을 수행합니다.

    GetDataPresent 메서드를 사용하여 끌어온 데이터에 문자열 개체가 포함되어 있는지 확인합니다.

    문자열 데이터가 있으면 GetData 메서드를 사용하여 이 데이터를 추출합니다.

    BrushConverter를 사용하여 문자열을 Brush로 변환합니다.

    변환에 성공하면 Circle 컨트롤의 UI를 제공하는 Ellipse의 Fill에 브러시를 적용합니다.

    Drop 이벤트를 처리된 이벤트로 표시합니다. 이 이벤트를 받는 다른 요소에 Circle 사용자 정의 컨트롤이 해당 이벤트를 처리했음을 알리려면 놓기 이벤트를 처리된 이벤트로 표시해야 합니다.
             
             */


        }




        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            e.Effects = DragDropEffects.None;

            // If the DataObject contains string data, extract it.
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                string dataString = (string)e.Data.GetData(DataFormats.StringFormat);

                // If the string can be converted into a Brush, allow copying or moving.
                BrushConverter converter = new BrushConverter();
                if (converter.IsValid(dataString))
                {
                    // Set Effects to notify the drag source what effect
                    // the drag-and-drop operation will have. These values are 
                    // used by the drag source's GiveFeedback event handler.
                    // (Copy if CTRL is pressed; otherwise, move.)
                    if (e.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
                    {
                        e.Effects = DragDropEffects.Copy;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                }
            }
            e.Handled = true;

            /*
             재정의된 이 OnDragOver는 다음 작업을 수행합니다.

    Effects 속성을 None로 설정합니다.

    OnDrop 메서드에서 수행되는 것과 동일한 확인 작업을 수행하여 Circle 사용자 정의 컨트롤이 끌어온 데이터를 처리할 수 있는지 여부를 확인합니다.

    사용자 정의 컨트롤이 데이터를 처리할 수 있으면 Effects 속성을 Copy 또는 Move로 설정합니다.

F5 키를 눌러 응용 프로그램을 빌드 및 실행합니다.

TextBox에서 gre라는 텍스트를 선택합니다.

이 텍스트를 Circle 컨트롤로 끕니다. gre는 올바른 색이 아니므로 이번에는 커서가 놓기가 허용되지 않음을 나타내는 커서로 바뀝니다.
             */
        }




        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            // Save the current Fill brush so that you can revert back to this value in DragLeave.
          //  _previousFill = ractangleUI.Fill;

            // If the DataObject contains string data, extract it.
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                string dataString = (string)e.Data.GetData(DataFormats.StringFormat);

                // If the string can be converted into a Brush, convert it.
                BrushConverter converter = new BrushConverter();
                if (converter.IsValid(dataString))
                {
                    Brush newFill = (Brush)converter.ConvertFromString(dataString.ToString());
             //       ractangleUI.Fill = newFill;
                }
            }


            /*
             재정의된 이 OnDragEnter는 다음 작업을 수행합니다.

    Ellipse의 Fill 속성을 _previousFill 변수에 저장합니다.

    OnDrop 메서드에서 수행되는 것과 동일한 확인 작업을 수행하여 데이터를 Brush로 변환할 수 있는지 여부를 확인합니다.

    데이터가 올바른 Brush로 변환되면 Ellipse의 Fill에 이 브러시를 적용합니다.
             */


        }


        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);
            // Undo the preview that was applied in OnDragEnter.
        //    ractangleUI.Fill = _previousFill;

            /*
             재정의된 이 OnDragLeave는 다음 작업을 수행합니다.

    Circle 사용자 정의 컨트롤의 UI를 제공하는 Ellipse의 Fill에 _previousFill 변수에 저장된 Brush를 적용합니다.
             */

        }












    }
}
