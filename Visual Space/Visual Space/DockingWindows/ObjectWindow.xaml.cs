﻿using System;
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


            createObjList(); //서버에서 받아올 리스트. 서버가 아직 없음으로 내가 임의로 리스트를 만든다. 만든 리스트 정보는 ObjectList에 추가된다.(총 6개)
            createbed(); //임시로 침대 때려박음. 위의 것과 동일.

            // appendImageToExpender();





            appendImageToExpender(ObjectList);  //ObjectList를 매개변수로 넣으면 리스트 tag에 있는 ObjectType을 보고 switch case문으로 분류해서 알맞는 익스팬더에 집어넣게 됨. 




        }

        public List<ObjectInfo> ObjectList = new List<ObjectInfo>(); //서버에서 받아온 정보를 이곳에다 넣는다.
       

        //서버에서 받아올 리스트. 서버가 아직 없음으로 내가 임의로 리스트를 만든다. 이걸 받아서 여기에 있는 정보로 익스펜더 안의 랩패널의 자식으로 추가
        public void createObjList()
        {            
            for (int i = 0; i < 6; i++)
            {
                ObjectInfo objInfo = new ObjectInfo();
                objInfo.ObjectType = "chairs";
                objInfo.VisualName = $"이케아의자{i}";
                objInfo.FilePath = $"../../pictures/chairs/chair{i}.png";

                

                ObjectList.Add(objInfo);

              
            }             
        }

        public void createbed()
        {

            for (int i = 0; i < 2; i++)
            {
                ObjectInfo objInfo = new ObjectInfo();
                objInfo.ObjectType = "beds";
                objInfo.VisualName = $"이케아침대{i}";
                objInfo.FilePath = $"../../pictures/beds/bed{i}.png";



                ObjectList.Add(objInfo);





            }
        }



        ////드롭된 정보를 캔버스에서 도면 이미지로 변환하고 그것에 대한 정보를 기억.
        //public class ObjConvertImageInfo
        //{
        //    string VisualName; //사용자가 변경할 수 있는 이름.
        //    string convertFilePath; //변환시켜줄 이미지
        //    string ObjectName; //같은 오브젝트를 여러개 놓는다면 그걸 구분하기 위한 이름이 필요.
        //    Point canvasPoint; //캔버스 좌표
        //    RotateTransform rotationPoint; //시계방향으로 회전하기 위한 2차원 x,y 정보 좌표계.         
        //}

        // ObjectList를 매개변수로 넣어주자.
        public void appendImageToExpender(List<ObjectInfo> _ObjectList)
        {

            
            foreach (var obj_List in _ObjectList)
            {

                switch (obj_List.ObjectType)
                {
                    case "chairs":

                        Categori cate_chairs = new Categori(obj_List.ObjectType, obj_List.FilePath, obj_List.VisualName);
                        Expd_chairs.Children.Add(cate_chairs);


                        break;




                    case "beds":

                        Categori cate_beds = new Categori(obj_List.ObjectType, obj_List.FilePath, obj_List.VisualName);
                        Expd_beds.Children.Add(cate_beds);


                        break;



                }




             //   Categori cate2 = new Categori("/pictures/chair0.PNG", "chair0");  //<<<<파일경로, 오브젝트타입

            //    Expd_temp.Children.Add(cate2);
            }


        }


        public BitmapImage convertImageToBitmap(string FilePath) //이미지 파일을 비트맵이미지로 바꿔줌. 이러면 바로 image.source에 넣어주면 됨.
        {            
            BitmapImage Bitmap = new BitmapImage();
            Bitmap.BeginInit();
            Bitmap.UriSource = new Uri(FilePath, UriKind.Relative);
            Bitmap.EndInit();
            return Bitmap;
        }






        #region 이전 로직

        //어떤 정보를 리스트로 받아야 될지 고민해보자.
        //List<string> strs = new List<string>
        //    {
        //        "chair0.png",
        //        "chair1.png",
        //        "chair2.png",
        //        "chair3.png",
        //        "chair4.png",

        //    };

        // ObjectList를 매개변수로 넣어주자.
        //  public void appendImageToExpender()
        //  {


        //          //Categori cate = new Categori();
        //          //var tempCanvas = cate.Content as Grid;
        //          //var t = tempCanvas.Children.OfType<Image>().ToArray();


        //      int i = 0;
        //      foreach(var str in strs)
        //      {
        //          Image myImage3 = new Image();
        //          BitmapImage bi3 = new BitmapImage();
        //          Categori cate = new Categori();
        //          bi3.BeginInit();
        //          bi3.UriSource = new Uri($"../../pictures/{str}", UriKind.Relative);                
        //          bi3.EndInit();
        //          myImage3.Stretch = Stretch.UniformToFill;
        //          myImage3.Source = bi3;
        //          myImage3.Name = $"chair{i++}";
        //          myImage3.Width = 60;
        //          myImage3.Height = 60;



        //          Button btn = new Button();
        //          btn.Width = 60;
        //          btn.Height = 60;
        //          btn.Background = Brushes.Transparent;

        //          cate.Content = btn;
        //          cate.Content = myImage3;


        //          Expd_temp.Children.Add(cate);
        //      }

        //      //Categori cate2 = new Categori();
        // //     Categori cate2 = new Categori("/pictures/chair0.PNG", "chair0");  //<<<<파일경로, 오브젝트타입

        //     //cate2.Name = "test";
        ////      Categori cate3 = new Categori("/pictures/chair1.PNG", "chair1");

        //      // cate2.MouseMove += OnMouseMove;             
        //      //OnGiveFeedback
        //      //   OnDrop
        //      // OnDragLeave
        //      //OnDragEnter
        //      //            OnDragOver



        //   //   Expd_temp.Children.Add(cate2);
        //   //   Expd_temp.Children.Add(cate3);
        //      return;


        //          //for (int j = 0; j < t.Length; j++)
        //          //{

        //          //    tempCanvas.Children.Remove(t[j]);

        //          //    t[j].Name = $"chair{j}";
        //          //    // Byte[] result_ByteImage = GetBytesFromImageFile($"../../pictures/{image.Name}.png");
        //          //    // string str = Convert.ToString(result_ByteImage);


        //          //    BitmapImage bitimg = convertImageToBitmap(t[j], t[j].Name);

        //          //    t[j].Source = bitimg;
        //          //    t[j].Stretch = Stretch.Uniform;



        //          //    Wrap_BathRoom.Children.Add(t[j]);
        //          //}



        //          ////  foreach (Image myImage in myContent.Children.OfType<Image>())              
        //          //foreach (  var image in tempCanvas.Children.OfType<Image>())
        //          //{

        //          //    image.Name = $"chair{i++}";
        //          //    // Byte[] result_ByteImage = GetBytesFromImageFile($"../../pictures/{image.Name}.png");
        //          //    // string str = Convert.ToString(result_ByteImage);


        //          //    BitmapImage bitimg = convertImageToBitmap(image, image.Name);

        //          //    image.Source = bitimg;
        //          //    image.Stretch = Stretch.Uniform;


        //          //    tempCanvas.Children.Remove(image);
        //          //    Wrap_BathRoom.Children.Add(image);


        //          //    /*
        //          //   _parent.Children.Remove(_element);
        //          //            _panel.Children.Add(_element);
        //          //            // set the value to return to the DoDragDrop call
        //          //            e.Effects = DragDropEffects.Move;    
        //          // */


        //          //}





        //      }

        #endregion

        //   pack://application:,,,/Resources/MyImage.png


        /*
            Image myImage3 = new Image();
                        BitmapImage bi3 = new BitmapImage();
                        bi3.BeginInit();
                        bi3.UriSource = new Uri($"../../pictures/{txt}.PNG", UriKind.Relative);
                        bi3.EndInit();
                        myImage3.Stretch = Stretch.Uniform;
                        myImage3.Source = bi3;
         */








        #region 마우스 드래그

        //        protected override void OnMouseMove(MouseEventArgs e)
        //        {
        //            base.OnMouseMove(e);
        //            if (e.LeftButton == MouseButtonState.Pressed)
        //            {
        //                // Package the data.
        //                DataObject data = new DataObject();
        //                //  data.SetData(DataFormats.StringFormat, ractangleUI.Fill.ToString());
        //                //   data.SetData("Double", ractangleUI.Height);
        //                data.SetData("Object", this);

        //                // Inititate the drag-and-drop operation.
        //                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);


        //                /*
        //                 재정의된 이 OnMouseMove는 다음 작업을 수행합니다.

        //    마우스 왼쪽 단추를 누른 채 마우스를 이동하고 있는지 여부를 확인합니다.

        //    Circle 데이터를 DataObject에 패키지합니다. 이 예제의 경우 Circle 컨트롤은 세 개의 데이터 항목, 즉 채우기 색의 문자열 표현, 높이의 double 형식 표현 및 컨트롤 자체의 복사본을 패키지합니다.

        //    정적 DragDrop.DoDragDrop 메서드를 호출하여 끌어서 놓기 작업을 시작합니다. DoDragDrop 메서드에는 다음 세 개의 매개 변수를 전달합니다.

        //        dragSource - 이 컨트롤에 대한 참조입니다.

        //        data - 이전 코드에서 만든 DataObject입니다.

        //        allowedEffects - 허용되는 끌어서 놓기 작업으로, Copy 또는 Move입니다.




        //            }
        //        }



        //        //놓기 대상의 DragOver 이벤트 처리기에 설정된 Effects 값을 확인합니다.
        //        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        //        {
        //            base.OnGiveFeedback(e);
        //            // These Effects values are set in the drop target's
        //            // DragOver event handler.
        //            if (e.Effects.HasFlag(DragDropEffects.Copy))
        //            {
        //                Mouse.SetCursor(Cursors.Cross);
        //            }
        //            else if (e.Effects.HasFlag(DragDropEffects.Move))
        //            {
        //                Mouse.SetCursor(Cursors.Pen);
        //            }
        //            else
        //            {
        //                Mouse.SetCursor(Cursors.No);
        //            }
        //            e.Handled = true;


        //            /*
        //             재정의된 이 OnGiveFeedback는 다음 작업을 수행합니다.

        //    놓기 대상의 DragOver 이벤트 처리기에 설정된 Effects 값을 확인합니다.

        //    Effects 값에 따라 사용자 지정 커서를 설정합니다. 이 커서는 데이터를 놓을 때의 결과에 대한 시각적 피드백을 사용자에게 제공하기 위한 것입니다.

        //             */

        //            }
        //        }




        //        protected override void OnDrop(DragEventArgs e)
        //        {
        //            base.OnDrop(e);

        //            // If the DataObject contains string data, extract it.
        //            if (e.Data.GetDataPresent(DataFormats.StringFormat))
        //            {
        //                string dataString = (string)e.Data.GetData(DataFormats.StringFormat);

        //                // If the string can be converted into a Brush, 
        //                // convert it and apply it to the ellipse.
        //                BrushConverter converter = new BrushConverter();
        //                if (converter.IsValid(dataString))
        //                {
        //                    Brush newFill = (Brush)converter.ConvertFromString(dataString);
        //                    //      ractangleUI.Fill = newFill;

        //                    // Set Effects to notify the drag source what effect
        //                    // the drag-and-drop operation had.
        //                    // (Copy if CTRL is pressed; otherwise, move.)
        //                    if (e.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
        //                    {
        //                        e.Effects = DragDropEffects.Copy;
        //                    }
        //                    else
        //                    {
        //                        e.Effects = DragDropEffects.Move;
        //                    }
        //                }
        //            }
        //            e.Handled = true;


        //            /*
        //             재정의된 이 OnDrop는 다음 작업을 수행합니다.

        //    GetDataPresent 메서드를 사용하여 끌어온 데이터에 문자열 개체가 포함되어 있는지 확인합니다.

        //    문자열 데이터가 있으면 GetData 메서드를 사용하여 이 데이터를 추출합니다.

        //    BrushConverter를 사용하여 문자열을 Brush로 변환합니다.

        //    변환에 성공하면 Circle 컨트롤의 UI를 제공하는 Ellipse의 Fill에 브러시를 적용합니다.

        //    Drop 이벤트를 처리된 이벤트로 표시합니다. 이 이벤트를 받는 다른 요소에 Circle 사용자 정의 컨트롤이 해당 이벤트를 처리했음을 알리려면 놓기 이벤트를 처리된 이벤트로 표시해야 합니다.

        //             */


        //        }



        //        protected override void OnDragOver(DragEventArgs e)
        //        {
        //            base.OnDragOver(e);
        //            e.Effects = DragDropEffects.None;

        //            // If the DataObject contains string data, extract it.
        //            if (e.Data.GetDataPresent(DataFormats.StringFormat))
        //            {
        //                string dataString = (string)e.Data.GetData(DataFormats.StringFormat);

        //                // If the string can be converted into a Brush, allow copying or moving.
        //                BrushConverter converter = new BrushConverter();
        //                if (converter.IsValid(dataString))
        //                {
        //                    // Set Effects to notify the drag source what effect
        //                    // the drag-and-drop operation will have. These values are 
        //                    // used by the drag source's GiveFeedback event handler.
        //                    // (Copy if CTRL is pressed; otherwise, move.)
        //                    if (e.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
        //                    {
        //                        e.Effects = DragDropEffects.Copy;
        //                    }
        //                    else
        //                    {
        //                        e.Effects = DragDropEffects.Move;
        //                    }
        //                }
        //            }
        //            e.Handled = true;

        //            /*
        //             재정의된 이 OnDragOver는 다음 작업을 수행합니다.

        //    Effects 속성을 None로 설정합니다.

        //    OnDrop 메서드에서 수행되는 것과 동일한 확인 작업을 수행하여 Circle 사용자 정의 컨트롤이 끌어온 데이터를 처리할 수 있는지 여부를 확인합니다.

        //    사용자 정의 컨트롤이 데이터를 처리할 수 있으면 Effects 속성을 Copy 또는 Move로 설정합니다.

        //F5 키를 눌러 응용 프로그램을 빌드 및 실행합니다.

        //TextBox에서 gre라는 텍스트를 선택합니다.

        //이 텍스트를 Circle 컨트롤로 끕니다. gre는 올바른 색이 아니므로 이번에는 커서가 놓기가 허용되지 않음을 나타내는 커서로 바뀝니다.
        //             */
        //        }



        //        protected override void OnDragEnter(DragEventArgs e)
        //        {
        //            base.OnDragEnter(e);
        //            // Save the current Fill brush so that you can revert back to this value in DragLeave.
        //            //  _previousFill = ractangleUI.Fill;

        //            // If the DataObject contains string data, extract it.
        //            if (e.Data.GetDataPresent(DataFormats.StringFormat))
        //            {
        //                string dataString = (string)e.Data.GetData(DataFormats.StringFormat);

        //                // If the string can be converted into a Brush, convert it.
        //                BrushConverter converter = new BrushConverter();
        //                if (converter.IsValid(dataString))
        //                {
        //                    Brush newFill = (Brush)converter.ConvertFromString(dataString.ToString());
        //                    //       ractangleUI.Fill = newFill;
        //                }
        //            }


        //            /*
        //             재정의된 이 OnDragEnter는 다음 작업을 수행합니다.

        //    Ellipse의 Fill 속성을 _previousFill 변수에 저장합니다.

        //    OnDrop 메서드에서 수행되는 것과 동일한 확인 작업을 수행하여 데이터를 Brush로 변환할 수 있는지 여부를 확인합니다.

        //    데이터가 올바른 Brush로 변환되면 Ellipse의 Fill에 이 브러시를 적용합니다.
        //             */


        //        }


        //        protected override void OnDragLeave(DragEventArgs e)
        //        {
        //            base.OnDragLeave(e);
        //            // Undo the preview that was applied in OnDragEnter.
        //            //    ractangleUI.Fill = _previousFill;

        //            /*
        //             재정의된 이 OnDragLeave는 다음 작업을 수행합니다.

        //    Circle 사용자 정의 컨트롤의 UI를 제공하는 Ellipse의 Fill에 _previousFill 변수에 저장된 Brush를 적용합니다.
        //             */

        //        }

        #endregion










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
