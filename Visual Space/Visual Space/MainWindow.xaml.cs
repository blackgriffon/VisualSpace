//#define RUN_3DVIWER_IN_UNITY_EDITER
#define RUN_3DVIWER


using Nollan.Visual_Space.DockingWindows;

using Nollan.Visual_Space.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using VisualSpace.UnityViewer;

namespace Nollan.Visual_Space 
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>

   public partial class MainWindow : Window , ICloneable
    {
        
                IWpfUnityTCPServer server = null;


        public MainWindow()
        {
            InitializeComponent();

#if RUN_3DVIWER
            server = new WpfUnityTCPServer("127.0.0.1", 9000);
            server.Connect();
            server.OnReceviedCompleted += OnReceviedCompleted;

#else
            server = new NullWpfUnityTCPServer();
#endif
            this.Loaded += MainWindow_Loaded;
        }



        private void OnReceviedCompleted(WpfUnityPacketHeader header)
        {
            switch (header.ObjectType)
            {
                case WpfUnityPacketType.WallInfo:
                    WallInfo wallInfo = (WallInfo)header.Data;

                    switch (wallInfo.Action)
                    {
                        case WallInfo.WallInfoAction.MOVE3D:

                            Dispatcher.Invoke(() =>
                            {
                                foreach (var el in mapCanvas.Children)
                                {
                                    if (el is Line l)
                                    {
                                        if (l.Name == wallInfo.Name)
                                        {

                                            List<int> pos = convert3DPosTo2D(wallInfo);

                                            l.X1 = pos[2];
                                            l.Y1 = pos[3];
                                            l.X2 = pos[0];
                                            l.Y2 = pos[1];
                                            break;
                                        }
                                    }
                                }

                            });
                            break;

                        case WallInfo.WallInfoAction.REMOVE3D:
                            Dispatcher.Invoke(() =>
                            {
                                Line toRemove = null;

                                // foreach 문 안에서 list의 요소를 지우면 에러가 난다.
                                foreach (var el in mapCanvas.Children)
                                {
                                    if (el is Line l)
                                    {
                                        if (l.Name == wallInfo.Name)
                                        {
                                            toRemove = l;
                                            break;
                                        }
                                    }
                                }

                                if (toRemove != null)
                                    mapCanvas.Children.Remove(toRemove as UIElement);
                            });
                            break;


                        case WallInfo.WallInfoAction.SELECT3D:
                            Dispatcher.Invoke(() =>
                            {
                                Line toSelect = null;

                                // foreach 문 안에서 list의 요소를 지우면 에러가 난다.
                                foreach (var el in mapCanvas.Children)
                                {
                                    if (el is Line l)
                                    {
                                        if (l.Name == wallInfo.Name)
                                        {

                                            toSelect = l;
                                            break;
                                        }
                                    }
                                }

                                if (toSelect != null)
                                {

                                    selectedLine = toSelect;
                                    selectedLine.Stroke = Brushes.Red;
                                }

                            });
                            break;

                        case WallInfo.WallInfoAction.DESELECT3D:
                            Dispatcher.Invoke(() =>
                            {
                                if (selectedLine != null)
                                    selectedLine.Stroke = Brushes.Black;
                                selectedLine = null;

                            });
                            break;
                    }
                    break;


            }
        }


        private List<int> convert3DPosTo2D(WallInfo wallInfo)
        {

            int zeroPos = 400;
            // 계산

            // 2D 상의 중심점을 구한다.
            int xc = (int)(wallInfo.PosX * 20 + zeroPos);
            int yc = (int)(wallInfo.PosZ * 20 * -1 + zeroPos);
            int w, h;

            // 2D 상의 길이를 구한다.
            // float이기 때문에 유니티상에서 정확하게 소수점 1자리로 안떨어지고 2.6이면 2.5xxxxx 로 나온다.
            // 따라서 반올림을 해줘서 값을 보정한다.
            if (wallInfo.ScaleX > wallInfo.ScaleZ)
            {
                w = (int)(Math.Round(wallInfo.ScaleX-0.2, 0)) * 20;
                h = 0;
            }
            else
            {
                w = 0;
                h = (int)(Math.Round(wallInfo.ScaleZ-0.2, 0)) * 20;
            }

            // 이만큼 더해주면 x1,x2 빼주면 x2, y2가된다.
            w = w / 2;
            h = h / 2;

            List<int> pos = new List<int>();
            pos.Add(xc - w);
            pos.Add(yc - h);
            pos.Add(xc + w);
            pos.Add(yc + h);

            return pos;
        }




        #region 윈도우 리사이징 시 컨트롤 크기 변경

        double orginalWidth, originalHeight;
        ScaleTransform scale = new ScaleTransform();

        void Window1_SizeChanged(object sender, SizeChangedEventArgs e)

        {

            ChangeSize(e.NewSize.Width, e.NewSize.Height);

        }

        
        void Window1_Loaded()
        {

            orginalWidth = this.Width;
            originalHeight = this.Height;
            
            if (this.WindowState == WindowState.Maximized)
            {
                ChangeSize(this.ActualWidth, this.ActualHeight);
            }
            
            this.SizeChanged += new SizeChangedEventHandler(Window1_SizeChanged);
        }


        private void ChangeSize(double width, double height)
        {
            scale.ScaleX = width / orginalWidth;
            scale.ScaleY = height / originalHeight;

            FrameworkElement rootElement = this.Content as FrameworkElement;
            rootElement.LayoutTransform = scale;
        }        

#endregion


   


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            // 클라이언트를 EXE로 enbedded안하고 unity프로그램으로 돌리거나
            // 3Dviwer를 실행시킬 때만 되도록한다.
#if !RUN_3DVIWER_IN_UNITY_EDITER && RUN_3DVIWER
            host.Child = new UnityExe(AppDomain.CurrentDomain.BaseDirectory+ @"..\..\..\exe\3D Viewer.exe");
#endif

            //-----
            //   Window1_Loaded(); //리사이징 코드
            //----//

           
            checkCanvasWidthCount();
            showSubPanel(); //왼쪽 오른쪽 UI 띄우기
            


        }


        DockingWindows.ListWindow listWindow;
        public void showSubPanel()
        {
            //0731추가-----
            //set window parent for dragging operations
            ListDockManager.ParentWindow = this;
            ExpenderDockManager.ParentWindow = this;

            listWindow = new DockingWindows.ListWindow();

            listWindow.call_mainwindow = this; // 자식폼에서 부모폼을 등록??? 자식에서 부모에게 갑 전달 실험용.


            listWindow.DockManager = ListDockManager;
            listWindow.Show(Dock.Top);


            DockingWindows.ObjectWindow objWindowDocking = new DockingWindows.ObjectWindow();
            objWindowDocking.DockManager = ExpenderDockManager;
            objWindowDocking.Show(Dock.Top);
            //-------//
        }



        public static List<Point> ListRenderPoint = new List<Point>();//각 꼭지점 좌표를 기억하는 리스트

        Line selectedLine; //선택된 선과 선택 안된 선의 컬러를 구분해주기 위해 이전에 선택된 선을 기억하고 있는 변수.
        bool? Flag_HorizontalLinePlusMinus = null;
        bool? Flag_VerticalLinePlusMinus = null;
        bool? Result_separateLine = null;
        int i = 0;
        Image obj_image;
        bool imageClick = false;

        //선 증가 감소 플래그
        public bool linePlusMinus_Check;

        bool bMouseDown = false;
        bool bLine_check = false;
        Point stPoint;
        Point curPoint;
        Line line;
        int _gridSize = 20; //캔버스 격자 간격 변수. 이걸 바꾸면 MapCanvas의 거도 바꿔줘야함.



        



        /*
         고려 사항
         0. 격자선의 꼭지점을 어떻게 알아낼 것인가? 
         --%40 해서 나머지 0인 경우? 0일 때 나머지 0. 40일때 나머지 0, 80일 때 나머지 0
         --x와 y 둘 다 %40이 0일 때 꼭지점이 성립한다?
         --점들에 대한 정보를 리스트 안에 기록 후 >> 
         --foreach를 통해 유저가 입력한 마우스 값과 비교하여 가장 가까운 점을 매칭시켜 그 점을 stPoint로 지정한다.
            

         (0 0)   (40 0)  (80 0)   (120 0)
         (0 40)  (40 40) (80 40)  (120 40)
         (0 80)  (40 80) (80 80)  (120 80) 
         (0 120) (40 120 (80 120) (120 120)
            
         1. 격자선의 꼭지점.(마우스다운 이벤트 시 좌표검사로 가장 가까운 꼭지점 부분으로 이동? 
         2. 선 이동시 x1은 꼭지점에 위치해야 함. 가장 가까운 꼭지점으로 x1 과 x2를 이동시킨다? 예외처리는 어떻게?             
             */

   


        //로그 찍는 함수
        public void writeLog(double linex1, double liney1, double linex2, double liney2, string str)
        {
            StreamWriter writer = new StreamWriter(

            File.Open("../../Log.txt", FileMode.Append));

            TextWriterTraceListener listener = new TextWriterTraceListener(writer);

            Debug.Listeners.Add(listener);

            Debug.WriteLine(string.Format("x1y1({0}, {1}), x2y2({2}, {3}) - {4} : {5}.", linex1, liney1, linex2, liney2, DateTime.Now, str));
            //Debug.WriteLine(string.Format("{0} : Test Log2.", DateTime.Now));
            Debug.Flush();

            writer.Close();

        }

       
       
     
        //마우스다운시
        private void MapCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {


            if (e.OriginalSource.GetType() != typeof(Image)) //클릭했을 시 오리지널소스가 image가 아닌 경우
            {


                //0731추가------
                //선이 그려져 있고, 선택된 선이 있는데 그냥 허공(캔버스)를 선택하는 경우
                if (selectedLine != null)
                {
                    if (e.Source != line)
                    {
                        // 선택해재 정보 전송
                        server.Send(PacketFactory.MakeDeselect(selectedLine.Name));
                        selectedLine.Stroke = Brushes.Black;
                        selectedLine = null; //선택된 선 해제.

                        /*
                    1.선택된 선이 있을 때 선 그릴 경우 일단 캔버스를 클릭하게 되므로 선택해제가 됨. 
                    2.캔버스를 누르면 선택이 취소됨
                    3.선증가감소 체크 버튼이 켜져있을 시 선을 누르면 선택한 것임.
                    4.선이동시(선그리기아니고, 선증감도 아닐 때) 내려놓으면 붉은색으로 남아있음.    

                     */

                    }
                }
                //-----------------//



                //마우스다운시
                if (!bMouseDown && bLine_check && !linePlusMinus_Check)
                {




                    bMouseDown = true;
                    stPoint = TranslatePointToCanvas(e, mapCanvas);
                    remainderToFindVertex(stPoint.X, stPoint.Y); //가장 가까운 꼭지점을 찾는다. 그 값은 튜플형식으로 Result_StartPoint에 담기게됨.
                    line = new Line();
                    line.MouseEnter += control_MouseEnter;
                    line.MouseLeave += control_MouseLeave;
                   
                    line.X1 = Result_StartPoint.Item1;
                    line.X2 = Result_StartPoint.Item1;
                    line.Y1 = Result_StartPoint.Item2;
                    line.Y2 = Result_StartPoint.Item2;


                    line.Stroke = Brushes.Black;
                    line.StrokeThickness = 6;
                    line.Name = $"Line_{i++}";
                    mapCanvas.Children.Add(line);

                }
                // 선 그리기가 아닐때
                else
                {
                    //버블링되서 윈도우가 받게 되는 핸들을 형변환.
                    line = e.OriginalSource as Line;

                    if (line != null)
                    {

                        //선 확대 축소 선 색깔 변경(마우스 다운)
                        selectedLineColorChangeMethod(); //선택된 선 색 변경 메서드                    

                        txtBox.Text = line.Name;

                        //선택된 선을 기억
                        selectedLine = line;
                        stPoint = stPoint = TranslatePointToCanvas(e, mapCanvas);


                        // 선 증가 감소 진입 플래그
                        if (Ch_linePlusMinus.IsChecked == true)
                        {

                            writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스다운-세퍼레이트라인처리 전");
                            Result_separateLine = separateLine(line.X1, line.Y1, line.X2, line.Y2); //수평인지 수직선인지 구별하는 함수
                                                                                                    //수평 ture, 수직 false


                            if (Result_separateLine == true) //ㅡ선. 수평선일 경우. y값은 변경하지 않고 x값만 변경하자.
                            {
                                isHorizontalORVertical_Line();
                                fluctuationFlagisTrueORFalse();

                            }
                            else //l선(수직선)일 경우. x값은 변경하지 않고 y값만 변경하자.
                            {
                                isHorizontalORVertical_Line();
                                fluctuationFlagisTrueORFalse();

                            }
                        }

                    }
                }
            }
            else //클릭했을 때 original source가 image인 경우
            {
                obj_image = e.OriginalSource as Image; //현재 클릭된 이미지를 기억?
                                                       // MessageBox.Show("이미지");
                                                       //  imageStPoint = TranslatePointToCanvas(e, mapCanvas); //클릭한 좌표를 맵캔버스 기준으로 변환해서 반환값을 가져온다.
                imageClick = true;


                txtBox.Text = obj_image.Name;

              //  MessageBox.Show((obj_image.Name).ToString());

            }



        }



        //마우스무브시
        private void MapCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (imageClick == false)
            {

                //선 그리기
                if (bMouseDown && bLine_check)
                {
                    //선 그릴 때 늘어났다 줄어났다를 마우스 다운하는 지점에 맞춰야하니까.
                    curPoint = TranslatePointToCanvas(e, mapCanvas);
                    line.X2 = curPoint.X;
                    line.Y2 = curPoint.Y;

                }
                // 선그리기 가 아닐때
                else
                {
                    curPoint = TranslatePointToCanvas(e, mapCanvas);


                    // 라인 증가 및 축소시킬 때.
                    if (line != null && !bLine_check && linePlusMinus_Check)  //마우스 다운상태고, 라인이 널이 아니고, 선그리기 체크 해제, 증가감소 체크상태일 때
                    {

                        stPoint = curPoint;

                        double movX = Math.Abs(curPoint.X - line.X1);
                        double movY = Math.Abs(curPoint.Y - line.Y1);

                        if (Flag_HorizontalLinePlusMinus != null)
                        {
                            if (Flag_HorizontalLinePlusMinus == true) //ㅡ선(수평선)에서 --값으로 선이 증가할 건지 ++값으로 증가할건지(왼쪽 증가, 오른쪽증가)
                            {
                                moveLtRt_LinePlusMinus(movX);
                                writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스무브-수평선 : x1에 마우스클릭이 더 가까웠을 경우");
                            }
                            else // Flag_HorizontalLinePlusMinus == false 일 때(마우스 클릭이 x2에 더 가까웠을 경우)
                            {
                                moveLtRt_LinePlusMinus(movX);
                                writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스무브-수평선 : x2에 마우스클릭이 더 가까웠을 경우");
                            }
                        }

                        if (Flag_VerticalLinePlusMinus != null)
                        {

                            if (Flag_VerticalLinePlusMinus == true) //l선(수직선) 일 때, --값으로 선이 증가할 건지 ++값으로 증가할건지(위쪽 증가, 밑쪽증가)
                            {
                                moveUpDw_LInePlusMinus(movY);
                                writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스무브-수직선 : y1에 마우스클릭이 더 가까웠을 경우");
                            }
                            else // Flag_VerticalLinePlusMinus == false 일 때(마우스 클릭이 x2에 더 가까웠을 경우)
                            {
                                moveUpDw_LInePlusMinus(movY);
                                writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스무브-수직선 : y2에 마우스클릭이 더 가까웠을 경우");
                            }
                        }
                    }

                    // 선 이동시킬 때(마우스무브)
                    if (line != null && !linePlusMinus_Check)
                    {
                        // 이동할 거리
                        double movX = curPoint.X - stPoint.X;
                        double movY = curPoint.Y - stPoint.Y;
                        stPoint = curPoint; //현재점에서 시작점을 뺏으니 현재점을 다시 시작점으로 바꿔줘야 다음에 다시 재귀했을 때 다시 로직이 돌아감

                        //수평 수직
                        // 거리 이동
                        line.X1 += movX;
                        line.X2 += movX;

                        line.Y1 += movY;
                        line.Y2 += movY;

                    }
                }

            }
            else //imageClick = true 일 때
            {
               
              //  imageCurPoint = TranslatePointToCanvas(e, mapCanvas);
                //   double movX = imageCurPoint.X - imageStPoint.X;
                //  double movY = imageCurPoint.Y - imageStPoint.Y;
                

                Canvas.SetLeft(obj_image, e.GetPosition(mapCanvas).X - obj_image.ActualWidth/2);
                Canvas.SetTop(obj_image, e.GetPosition(mapCanvas).Y - obj_image.ActualHeight/2);

                // obj_image.

            }
        }



        private void MapCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!imageClick) //imageClick == false
            {

                #region 일반적인 선 그리기
                //선 그리기 로직 - 일반적인 선 그리기
                if (bMouseDown && bLine_check)
                {
                    //혹시 선택되서 붉은 선이 그려져 있을 때 선그리기 하면 붉은색 선이 남지 않도록 다 검게 칠해주자.
                    foreach (var child in mapCanvas.Children)
                    {
                        if (child is Line l) //is에다가 변수선언하면서 as 효과까지 주는 c#문법. 라인일 때만 true
                        {
                            if (l.Stroke == Brushes.Red)
                                l.Stroke = Brushes.Black;

                        }
                    }



                    curPoint = TranslatePointToCanvas(e, mapCanvas);
                    remainderToFindVertex(curPoint.X, curPoint.Y); //가장 가까운 꼭지점을 찾는다. 그 값은 튜플형식으로 Result_StartPoint에 담기게됨.
                    line.X2 = Result_StartPoint.Item1;
                    line.Y2 = Result_StartPoint.Item2;


                    //y가 x보다 크면 line.x1의 좌표를 line.x2에 대입한다. 
                    //왜냐면 y값이 변한다는 건 ㅣ선(수직선)이란 것이고 x값은 고정이기 때문이다.
                    if (Math.Abs(line.X1 - curPoint.X) < Math.Abs(line.Y1 - curPoint.Y))      //ㅣ선(수직선) 구분 및 선 보정 + 길이 25 이하 삭제          
                    {

                        line.X2 = line.X1;

                        //선을 그렸을 때 길이가 25보다 작을 경우 그 선은 삭제하는 로직
                        if (Math.Abs(line.Y1 - curPoint.Y) < (_gridSize / 2)) //수직
                        {
                            //Line toDeleteLine = null;

                            //foreach (var el in mapCanvas.Children)
                            //{
                            //    if (el is Line l)
                            //    {
                            //        if (l.Name == line.Name)
                            //        {
                            //            toDeleteLine = l;                                   
                            //            break;
                            //        }
                            //    }
                            //}
                            //if (toDeleteLine != null)
                            //    mapCanvas.Children.Remove(toDeleteLine as UIElement);

                            mapCanvas.Children.Remove(line as UIElement);
                            line = null;
                            bMouseDown = false;
                            return;
                        }

                        // y2가 y1보다 더 큰 경우 y2와 y1의 위치 스왑해준다.y2는 y1이 되고 y1은 y2가 된다.
                        if (line.Y1 < line.Y2)
                        {
                            var temp = line.Y2;
                            line.Y2 = line.Y1;
                            line.Y1 = temp;
                        }

                    }
                    else // ㅡ(수평선) 보정 및 스왑 + 길이 작을 때 삭제.
                    {
                        line.Y2 = line.Y1; //보정


                        //선을 그렸을 때 길이가 25보다 작을 경우 그 선은 삭제하는 로직
                        if (Math.Abs(line.X1 - curPoint.X) < (_gridSize / 2)) //수평
                        {
                            //Line toDeleteLine = null;

                            //foreach (var el in mapCanvas.Children)
                            //{
                            //    if (el is Line l)
                            //    {
                            //        if (l.Name == line.Name)
                            //        {
                            //            toDeleteLine = l;                                  
                            //            break;
                            //        }
                            //    }
                            //}
                            //if (toDeleteLine != null)
                            //    mapCanvas.Children.Remove(toDeleteLine as UIElement);

                            mapCanvas.Children.Remove(line as UIElement);
                            line = null;
                            bMouseDown = false;
                            return;


                        }
                        //x2가 x1보다 더 큰 경우 x2와 x1의 위치 스왑해준다.
                        if (line.X1 < line.X2)
                        {
                            var temp = line.X2;
                            line.X2 = line.X1;
                            line.X1 = temp;

                        }


                    }

                    writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스업-선그리기 로그");


                    WpfUnityPacketHeader header = fillWallInfo(line, WallInfo.WallInfoAction.CREATE);
                    server.Send(header);

                    line = null;
                    bMouseDown = false;


                }
                #endregion

                #region 선 그리기 아닐 때(이동 및 축소확대)
                else // 선 그리기 아닐 때(이동 및 축소확대)
                {
                    curPoint = TranslatePointToCanvas(e, mapCanvas);


                    //선 축소 확대 로직
                    if (linePlusMinus_Check && line != null)
                    {
                        writeLog(line.X1, line.X2, line.Y1, line.Y2, "라인바뀌기 전");

                        if (Flag_HorizontalLinePlusMinus != null)
                        {
                            if (Flag_HorizontalLinePlusMinus == true)  //ㅡ선(수평선), true일 때 성립. 스왑된 x1에서 오른쪽으로 증가
                            {

                                //확대 축소가 끝났으니 다시 바꾼 값 원상복귀하기 위한 스왑.
                                double tempLine = line.X1;
                                line.X1 = line.X2;
                                line.X2 = tempLine;

                                writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스다운-line.x가 가까울 때, 마지막 스왑 전");

                                remainderToFindVertex(curPoint.X, curPoint.Y); //가장 가까운 꼭지점을 찾는다. 그 값은 튜플형식으로 Result_StartPoint에 담기게됨.
                                line.X1 = Result_StartPoint.Item1;

                                if (Math.Abs(line.X1 - line.X2) < _gridSize - 1) //선을 줄 일 때 x1과 x2의 길이가 40미만
                                {
                                    //길이가 40미만이 되면 40으로 고정 시켜준다. 
                                    line.X1 = line.X2 + _gridSize;

                                }

                                double tempY = line.Y1; //수평선의 y값은 동일하므로 다 같은 값 대입
                                line.Y1 = tempY;
                                line.Y2 = tempY;


                            }
                            else //ㅡ선(수평선), false일 때 성립. 
                            {


                                remainderToFindVertex(curPoint.X, curPoint.Y); //가장 가까운 꼭지점을 찾는다. 그 값은 튜플형식으로 Result_StartPoint에 담기게됨.
                                line.X2 = Result_StartPoint.Item1;

                                if (Math.Abs(line.X1 - line.X2) < _gridSize - 1) //선을 줄 일 때 x1과 x2의 길이가 40미만
                                {
                                    //길이가 40미만이 되면 40으로 고정 시켜준다. 
                                    line.X2 = line.X1 - _gridSize;

                                }

                            }

                        }


                        if (Flag_VerticalLinePlusMinus != null)
                        {
                            if (Flag_VerticalLinePlusMinus == true)  //l선(수직선), true일 때 성립. 스왑된 y1에서 아래쪽으로 증가
                            {

                                //확대 축소가 끝났으니 다시 바꾼 값 원상복귀하기 위한 스왑.
                                double tempLine = line.Y1;
                                line.Y1 = line.Y2;
                                line.Y2 = tempLine;

                                writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스다운-line.Y가 가까울 때, 마지막 스왑 전");

                                remainderToFindVertex(curPoint.X, curPoint.Y); //가장 가까운 꼭지점을 찾는다. 그 값은 튜플형식으로 Result_StartPoint에 담기게됨.
                                line.Y1 = Result_StartPoint.Item2;
                                writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스다운-line.Y가 가까울 때, 마지막 스왑 후");



                                if (Math.Abs(line.Y1 - line.Y2) < _gridSize - 1) //선을 줄 일 때 y1과 y2의 길이가 40미만
                                {
                                    //길이가 40미만이 되면 40으로 고정 시켜준다. 
                                    line.Y1 = line.Y2 + _gridSize;
                                }


                                double tempX = line.X1; //수직선의 x값은 동일하므로 같은 값 대입
                                line.X1 = tempX;
                                line.X2 = tempX;
                            }
                            else //l선(수직선), false일 때 성립. 
                            {
                                remainderToFindVertex(curPoint.X, curPoint.Y); //가장 가까운 꼭지점을 찾는다. 그 값은 튜플형식으로 Result_StartPoint에 담기게됨.
                                line.Y2 = Result_StartPoint.Item2;

                                if (Math.Abs(line.Y1 - line.Y2) < _gridSize - 1) //선을 줄 일 때 x1과 x2의 길이가 40미만
                                {
                                    //길이가 40미만이 되면 40으로 고정 시켜준다. 
                                    line.Y2 = line.Y1 - _gridSize;

                                }
                            }
                        }

                        WpfUnityPacketHeader header = fillWallInfo(line, WallInfo.WallInfoAction.MOVE);
                        server.Send(header);
                        // line.Stroke = Brushes.Black;
                        line = null; //이렇게 null로 참조를 끊어줘야 업이벤트가 끝나고 선이 마우스에서 떨어짐.                    
                        bMouseDown = false;
                        Result_StartPoint = null;

                    }

                    //선 이동 로직
                    if (line != null && !bLine_check && !linePlusMinus_Check)
                    {
                        // 이동할 거리
                        double movX = curPoint.X - stPoint.X;
                        double movY = curPoint.Y - stPoint.Y;
                        stPoint = curPoint; //현재점을 시작점으로. 근데 없어도 돌아가긴 함


                        //선의 두 거리 차를 구한다. 그래야 나중에 시작점이 옮겨가도 구해진 거리만큼 더해서 원래의 형태로 선이 그려질 수 있다.
                        double moveDistanceX2 = Math.Abs(line.X1 - line.X2);
                        double moveDistanceY2 = Math.Abs(line.Y1 - line.Y2);

                        writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스업 수평선구분전");
                        bool Result_separateLine = separateLine(line.X1, line.Y1, line.X2, line.Y2); //수평인지 수직선인지 구별하는 함수
                        remainderToFindVertex(line.X1, line.Y1); //선택된 선의 x1, y1의 좌표를 준다. 그러면 가장 가까운 꼭지점이 나올 것.
                        writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스업 가까운꼭지점알아냄");
                        //*******
                        //고려해야 할 거.이동시 x1y1과 x2y2가 0보다 작아질 경우 0보다 작아진 값을 0으로 보정시켜줘야 함.
                        //이동시 x1y1과 x2y2가 actualHeight/ actualWidth보다 커질 경우 커진 값을 -하여 보정시켜줘야 함.



                        moveParallelLIne(Result_separateLine, moveDistanceX2, moveDistanceY2); //선 평행이동 함수


                        //line.Stroke = Brushes.Black;

                        WpfUnityPacketHeader header = fillWallInfo(line, WallInfo.WallInfoAction.MOVE);
                        server.Send(header);
                        bMouseDown = false;
                        line = null; //이렇게 null로 참조를 끊어줘야 업이벤트가 끝나고 선이 마우스에서 떨어짐.


                    }


                    Result_separateLine = null; //수직 수평인지 구분
                    Flag_VerticalLinePlusMinus = null; // 선이 위증가--인지 , 밑으로 증가++인지
                    Flag_HorizontalLinePlusMinus = null; // 선이 왼쪽증가--인지, 오른쪽 증가++인지


                }
                #endregion

            }
            else //imageClick == true
            {
                imageClick = false;

                Canvas.SetLeft(obj_image, e.GetPosition(mapCanvas).X - obj_image.ActualWidth / 2);
                Canvas.SetTop(obj_image, e.GetPosition(mapCanvas).Y - obj_image.ActualHeight / 2);
                server.Send(fillObjectInfo(obj_image, ObjectInfoPacket.ObjectAction.MOVE));

                //obj_image = null;
            }

        }


        public void createObject()
        {

            for (int i = 0; i < 6; i++)
            { 
            Categori cate = new Categori();
            var image = cate.Content as Image;
            image.Name = "chair" + "i";
                // Byte[] result_ByteImage = GetBytesFromImageFile($"../../pictures/{image.Name}.png");
                // string str = Convert.ToString(result_ByteImage);


                BitmapImage bitimg = convertImageToBitmap(image, image.Name);

                image.Source = bitimg;
                image.Stretch = Stretch.Uniform;



            }
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

        }


        public BitmapImage convertImageToBitmap(Image myImage3, string txt) //이미지 파일을 비트맵이미지로 바꿔줌. 이러면 바로 image.source에 넣어주면 됨.
        {

            myImage3 = new Image();
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri($"../../pictures/{txt}.PNG", UriKind.Relative);
            bi3.EndInit();          

            return bi3;

        }


        //로컬에 있는 데이터를 연결하는 방법. image element를 동적생성하여 source를 설정할 때.
        public Byte[] GetBytesFromImageFile(string path)
        {
            //path는 이미지의 경로
            Byte[] array = null;
            FileStream aFile = null;
            try
            {
                if (File.Exists(path))
                {
                    aFile = new FileStream(path, FileMode.Open, FileAccess.Read);
                    array = new Byte[(int)aFile.Length];
                    aFile.Read(array, 0, (int)aFile.Length);
                    aFile.Close();
                    return array;


                }
                else
                {
                    return null;

                }


            }
            catch (Exception ex)
            {


            }

            return null;
            
        }


        #region Byte로 된 이미지를 연결하는 방법
        //Byte로 된 이미지를 연결하는 방법. FileStream과 비슷한 방법으로 사용하면 됨. 대신 MemoryStream을 사용하면 됨
        public void GetCardImage(Byte[] imageBytes, object obj)
        {

            MemoryStream ms = new MemoryStream(imageBytes);

            if (ms != null)
            {
                try
                {

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    Image img = (Image)obj;
                    img.Source = bitmap;
                }
                catch (Exception ex)
                {

                }
            }

            ms.Close();            


        }
#endregion


        private WpfUnityPacketHeader fillObjectInfo(Image img, ObjectInfoPacket.ObjectAction action)
        {
            // 공통적인 작업
            ObjectInfoPacket objectInfo = new ObjectInfoPacket();
            objectInfo.Name = img.Name;//img.Name;
            ObjConvertImageInfo ObjInfo = (ObjConvertImageInfo)img.Tag;
            objectInfo.ObjectType = "bed_1";//임시로 테스트ObjInfo.ObjectType;
            objectInfo.Action = action;

            // del 일때는 굳이 좌표정보가 필요없다.
            switch (action)
            {

                case ObjectInfoPacket.ObjectAction.CREATE:
                case ObjectInfoPacket.ObjectAction.MOVE:

                    int zeroPos = 400;
                    // 계산
                    // 2d 좌표 기준으로 200 / 200 센터로 지정한다.

                    float xc = (float)Canvas.GetLeft(img)- zeroPos + (float)img.ActualWidth / 2;
                    float yc = ((float)Canvas.GetTop(img)- zeroPos + (float)img.ActualHeight / 2) * -1;


                    objectInfo.PosX = xc / 20;
                    objectInfo.PosY = 0f;
                    objectInfo.PosZ = yc / 20;
                    break;

            }

            WpfUnityPacketHeader header = new WpfUnityPacketHeader(WpfUnityPacketType.ObjectInfo, objectInfo);
            return header;
        }


        private WpfUnityPacketHeader fillWallInfo(Line line, WallInfo.WallInfoAction action)
        {

            // 공통적인 작업
            WallInfo wallInfo = new WallInfo();
            wallInfo.Name = line.Name;
            wallInfo.Action = action;

            // del 일때는 굳이 좌표정보가 필요없다.
            switch (action)
            {
                case WallInfo.WallInfoAction.CREATE:
                case WallInfo.WallInfoAction.MOVE:

                    int zeroPos = 400;
                    float wallThickness = 0.2f;
                    // 계산
                    // 2d 좌표 기준으로 200 / 200 센터로 지정한다.

                    float x1 = (float)line.X1 - zeroPos;
                    float x2 = (float)line.X2 - zeroPos;
                    float y1 = (float)line.Y1 - zeroPos;
                    float y2 = (float)line.Y2 - zeroPos;


                    // 중심점을 구한다.
                    float xc = (x1 + x2) / 2;
                    float yc = (y1 + y2) / 2 * -1;

                    // 크기를 구한다.
                    float w = Math.Abs(x1 - x2);
                    float h = Math.Abs(y1 - y2);

                    wallInfo.PosX = xc / 20;
                    wallInfo.PosY = 1f;

                    wallInfo.PosZ = yc / 20;

                    wallInfo.ScaleX = w / 20;
                    wallInfo.ScaleY = 2f;
                    wallInfo.ScaleZ = h / 20;

                    if (wallInfo.ScaleX == 0)
                    { 
                        wallInfo.ScaleX = wallThickness;
                        wallInfo.ScaleZ += wallThickness;
                    }
                    else
                    {
                        wallInfo.ScaleX += wallThickness;
                        wallInfo.ScaleZ = wallThickness;
                    }
                    break;

            }

            WpfUnityPacketHeader header = new WpfUnityPacketHeader(WpfUnityPacketType.WallInfo, wallInfo);
            return header;


        }

     

        public void moveParallelLIne(bool Result_separateLine, double _moveDistanceX2, double _moveDistanceY2)
        {
            if (Result_separateLine) //true면 ㅡ선, 수평선임.
            {


                line.X1 = Result_StartPoint.Item1; //튜플에 저장된 가장 가까운 꼭지점 값
                line.Y1 = Result_StartPoint.Item2;

                line.X2 = Result_StartPoint.Item1 - _moveDistanceX2;
                line.Y2 = Result_StartPoint.Item2 + 0;

                writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스업 수평선구분후");

                ////x1 > x2 의 경우(오른쪽에서 왼쪽으로 그린 경우) 왼쪽 벽 예외 막기
                if (line.X1 < _moveDistanceX2)
                {
                    line.X2 = 0;
                    line.X1 = _moveDistanceX2;
                }

            }
            else //false면 ㅣ선, 수직선임
            {
                line.X1 = Result_StartPoint.Item1; //튜플에 저장된 가장 가까운 꼭지점 값
                line.Y1 = Result_StartPoint.Item2;

                line.X2 = Result_StartPoint.Item1 + 0;
                line.Y2 = Result_StartPoint.Item2 - _moveDistanceY2;

                writeLog(line.X1, line.Y1, line.X2, line.Y2, "크기조절-마우스업 수직선 구분후");

                ////위쪽 벽 예외 막기
                if (line.Y1 < _moveDistanceY2)
                {
                    line.Y2 = 0;
                    line.Y1 = _moveDistanceY2;
                }
            }

        }


        //수평선인지 수직선인지 구별하는 함수
        public bool separateLine(double x1, double y1, double x2, double y2)
        {

            if (y1 == y2) // ㅡ 선. 수평선의 경우 true 반환
            {
                return true;

            }
            else //ㅣ선, 수직선일 경우 
            {
                return false;
            }

        }


        //두 점 사이의 길이를 구하기 위한 피타고라스의 정리
        public double LengthPts(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }




        double rad = 18;
        public bool InEllipse(double x, double y, double mx, double my)
        {
            // 원점에서 마우스 포인터까지의 거리가
            // 반지름보다 작으면 원 안에 위치해 있다.
            if (LengthPts(x, y, mx, my) < rad)
                return true;
            else
                return false;
        }


        public Tuple<double, double> Result_StartPoint; //remainderToFindVertex 메서드를 통해 얻은 가장 가까운 꼭지점 값x,y를 저장시킬 튜플

        public Tuple<double, double> Method_tupleStartPointXY(double x, double y)
        {
            var tupleResult = Tuple.Create(x, y); // == new Tuple<double,double>(x, y); 같은 내용이나 Tuple의 Create 도우미를 통해 쉽게 만들 수 있음.
            return tupleResult;
        }

        //현재 마우스값 stx, sty 이거나 line의 x1, y1의 좌표값이 매개변수로 온다.
        public void remainderToFindVertex(double stx, double sty) //가장 가까운 꼭지점을 찾는다. 그 값은 튜플형식으로 Result_StartPoint에 담기게됨.
        {
            //현 위치점 stx에서 %_gridSize(격자 간격)을 하여 나온 나머지 값
            double minusValueX = stx % _gridSize;
            double minusValueY = sty % _gridSize;

            //현 위치점 stx - 나머지 값 = 맨 왼쪽 상단 좌표(coordinateX) <<기준이 될 왼쪽 상단 좌표이다.
            double coordinateLeftTopX = stx - minusValueX;
            double coordinateLeftTopY = sty - minusValueY;
            double coordinateLeftTopLengthPts = LengthPts(stx, sty, coordinateLeftTopX, coordinateLeftTopY); //현 위치와 왼쪽 상단의 길이를 피타고라스로 구한다


            //현 위치점 coordinateLeftTopX + 40, coordinateLeftTopY+0 = 오른쪽 상단 좌표
            double coordinateRightTopX = coordinateLeftTopX + _gridSize;
            double coordinateRightTopY = coordinateLeftTopY + 0;
            double coordinateRightTopLengthPts = LengthPts(stx, sty, coordinateRightTopX, coordinateRightTopY); //현 위치와 오른쪽 상단의 길이를 피타고라스로 구한다


            //현 위치점 coordinateLeftTopX + 0, coordinateLeftTopY+40 = 왼쪽 하단 좌표
            double coordinateLeftBottomX = coordinateLeftTopX + 0;
            double coordinateLeftBottomY = coordinateLeftTopY + _gridSize;
            double coordinateLeftBottomLengthPts = LengthPts(stx, sty, coordinateLeftBottomX, coordinateLeftBottomY); //현 위치와 왼쪽 하단의 길이를 피타고라스로 구한다


            //왼쪽 상단에서 + x40 y40. 오른쪽 하단 좌표 값
            double coordinateRightBottomX = coordinateLeftTopX + _gridSize;
            double coordinateRightBottomY = coordinateLeftTopY + _gridSize;
            double coordinateRightBottomLengthPts = LengthPts(stx, sty, coordinateRightBottomX, coordinateRightBottomY); //현 위치와 오른쪽 하단의 길이를 피타고라스로 구한다

            //min 값으로 해서 계속 비교하자.
            double minLengthValue = compareLength(coordinateLeftTopLengthPts, coordinateRightTopLengthPts, coordinateLeftBottomLengthPts, coordinateRightBottomLengthPts);


            //가장 현재점에서 가까운 길이값을 알아냈으므로 길이 값을 통해서 가장 가까운 꼭지점의 좌표값을 알아내자.
            double startPointX;
            double startPointY;
            if (minLengthValue == coordinateLeftTopLengthPts)
            {
                startPointX = coordinateLeftTopX;
                startPointY = coordinateLeftTopY;
            }
            else if (minLengthValue == coordinateRightTopLengthPts)
            {
                startPointX = coordinateRightTopX;
                startPointY = coordinateRightTopY;
            }
            else if (minLengthValue == coordinateLeftBottomLengthPts)
            {
                startPointX = coordinateLeftBottomX;
                startPointY = coordinateLeftBottomY;
            }
            else //(minLengthValue==coordinateRightBottomLengthPts)
            {
                startPointX = coordinateRightBottomX;
                startPointY = coordinateRightBottomY;
            }

            //튜플로 값 저장
            Result_StartPoint = new Tuple<double, double>(startPointX, startPointY);

        }

        public void moveLtRt_LinePlusMinus(double _movX)
        {
            if (Flag_HorizontalLinePlusMinus == true) //ㅡ선(수평선) 일 때
            {
                line.X2 = line.X1 + _movX; //line.x1에서 + 시켜줘야 오른쪽 방향으로 선이 움직여야 왼쪽으로 진행됨.
                writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스무브-수평선 : x1에 마우스클릭이 더 가까웠을 경우");
            }
            else // Flag_HorizontalLinePlusMinus == false 일 때(마우스 클릭이 x2에 더 가까웠을 경우)
            {
                line.X2 = line.X1 - _movX; //line.x1에서 - 시켜줘야 왼쪽으로 선이 진행됨.
                writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스무브-수평선 : x2에 마우스클릭이 더 가까웠을 경우");
            }

        }

        public void moveUpDw_LInePlusMinus(double _movY)
        {

            if (Flag_VerticalLinePlusMinus == true) //l선(수직선) 일 때. true이면 ++ 이라서 밑에쪽으로 선 증가
            {
                line.Y2 = line.Y1 + _movY; //line.y1에서 + 시켜줘야 아래 방향으로 선이 움직임.
                writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스무브-수평선 : x1에 마우스클릭이 더 가까웠을 경우");
            }
            else // Flag_VerticalLinePlusMinus == false 일 때(마우스 클릭이 y2에 더 가까웠을 경우) 수평선일 때 -- 위에쪽으로 선 증가
            {
                line.Y2 = line.Y1 - _movY; //line.y1에서 - 시켜줘야 위쪽으로 선이 진행됨.
                writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스무브-수평선 : x2에 마우스클릭이 더 가까웠을 경우");
            }

        }

        public bool compareLength(double ResultPoint1, double ResultPoint2) //둘을 비교하여 더 작은 값이 마우스와 가까운 것.
        {
            //line.x1이 가까우면 true, line.x2가 가까우면 false
            if (ResultPoint1 < ResultPoint2)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        //4개의 꼭지점들 중 가장 마우스클릭한 지점과 가까운 꼭지점 길이 반환
        public double compareLength(double LeftTopPoint, double RightTopPoint, double LeftBottomPoint, double RightBottomPoint)
        {
            double minValue;

            minValue = (LeftTopPoint < RightTopPoint) ? (LeftTopPoint) : (RightTopPoint);
            minValue = (minValue < LeftBottomPoint) ? (minValue) : (LeftBottomPoint);
            minValue = (minValue < RightBottomPoint) ? (minValue) : (RightBottomPoint);

            return minValue;
        }

        public void fluctuationFlagisTrueORFalse()
        {


            //마우스 클릭 시 line.x1이 가까울 때 : x2를 시작위치로 두고, x1을 증가 감소 (true 조건) true일 때 ++. 오른쪽으로 선 증가
            if (Flag_HorizontalLinePlusMinus == true)
            {
                //x1과 x2를 스왑해줘야 함. 왜냐면 이 조건은 마우스클릭과 line.x1이 가깝기 때문에
                //line.x2가 기준이 되고 line.x1 쪽이 늘어나야 하므로 임시로 값을 스왑해준 후에 다시 x1을 원상복귀 시킨다.
                double tempLine = line.X1;
                line.X1 = line.X2;
                line.X2 = tempLine;

                writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스다운-line.x1가 가까울 때");

                //마우스 업일 때 다시 스왑해서 x1이 큰 위치에 가게 해야 된다.
                //마우스 업일 때 Flag_HorizontalLinePlusMinus= null해준다.

            }



            //마우스 클릭 시 line.y1이 가까울 때 : y2를 시작위치로 두고, y1을 증가 (true 조건)
            if (Flag_VerticalLinePlusMinus == true)
            {
                //x1과 x2를 스왑해줘야 함. 왜냐면 이 조건은 마우스클릭과 line.x1이 가깝기 때문에
                //line.x2가 기준이 되고 line.x1 쪽이 늘어나야 하므로 임시로 값을 스왑해준 후에 다시 x1을 원상복귀 시킨다.
                double tempLine = line.Y1;
                line.Y1 = line.Y2;
                line.Y2 = tempLine;

                writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스다운-line.y1가 가까울 때");

                //마우스 업일 때 다시 스왑해서 x1이 큰 위치에 가게 해야 된다.
                //마우스 업일 때 Flag_HorizontalLinePlusMinus= null해준다.

            }

        }


        public void isHorizontalORVertical_Line()
        {
            if (Result_separateLine != null)
            {

                if (Result_separateLine == true) //수평선일 때 true
                {
                    //x1 y1과 마우스 사이의 거리
                    double compareLengthX1ToClickPoint = LengthPts(line.X1, line.Y1, stPoint.X, stPoint.Y);
                    //x2 y2와 마우스 사이의 거리
                    double compareLengthX2ToClickPoint = LengthPts(line.X2, line.Y2, stPoint.X, stPoint.Y);

                    writeLog(compareLengthX1ToClickPoint, compareLengthX2ToClickPoint, 0, 0, "마우스다운-컴페어랭스1,2");

                    Flag_HorizontalLinePlusMinus = compareLength(compareLengthX1ToClickPoint, compareLengthX2ToClickPoint);
                    // MessageBox.Show($"{Flag_HorizontalLinePlusMinus}");

                }
                else //수직선일 때(false)
                {
                    //x1 y1과 마우스 사이의 거리
                    double compareLengthX1ToClickPoint = LengthPts(line.X1, line.Y1, stPoint.X, stPoint.Y);
                    //x2 y2와 마우스 사이의 거리
                    double compareLengthX2ToClickPoint = LengthPts(line.X2, line.Y2, stPoint.X, stPoint.Y);

                    writeLog(compareLengthX1ToClickPoint, compareLengthX2ToClickPoint, 0, 0, "마우스다운-컴페어랭스1,2(수직선)");

                    Flag_VerticalLinePlusMinus = compareLength(compareLengthX1ToClickPoint, compareLengthX2ToClickPoint);
                    // MessageBox.Show($"{Flag_HorizontalLinePlusMinus}");

                }

            }


        }


        public void selectedLineColorChangeMethod()
        {

            if (selectedLine == line) //똑같은 선을 선택했으면
            {

                line.Stroke = Brushes.Red;

            }
            else //똑같은 선을 선택한 게 아니라면
            {

                if (selectedLine != null) //이전에 선택한 선이 있다면. else1 한 번 거치면 무조건 이쪽으로 와야함.
                {
                    selectedLine.Stroke = Brushes.Black;
                    server.Send(PacketFactory.MakeDeselect(selectedLine.Name));

                    line.Stroke = Brushes.Red;
                    server.Send(PacketFactory.MakeSelect(line.Name));

                }
                else //else1. 선을 한 번도 선택을 하지 않았으면 이쪽으로.
                {
                    line.Stroke = Brushes.Red;
                    server.Send(PacketFactory.MakeSelect(line.Name));
                }

            }

        }

        

       

        private void Ch_line_Checked(object sender, RoutedEventArgs e)
        {
            if (bLine_check == false)
            {
                bLine_check = true;
            }
            else //bLine_check == true
            {
                bLine_check = false;
            }

        }

        private void Ch_line_Unchecked(object sender, RoutedEventArgs e)
        {
            bLine_check = false;
        }


        private void Ch_linePlusMinus_Checked(object sender, RoutedEventArgs e)
        {
            if (Ch_line.IsEnabled && Ch_linePlusMinus.IsEnabled)
            {
                Ch_line.IsChecked = false;
            }

            linePlusMinus_Check = true;
            Ch_line.IsEnabled = false;


        }



        public void Ch_linePlusMinus_Unchecked(object sender, RoutedEventArgs e)
        {
            linePlusMinus_Check = false;
            Ch_line.IsEnabled = true;
        }


      


        public void mapCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            //처음 실행시
            double canvasWidth = e.NewSize.Width;
            double canvasHeight = e.NewSize.Height;

            mapCanvas.Width = canvas_TotalWidth;
            mapCanvas.Height = canvas_TotalHeight;

            //x축 그려질 개수 구하기
            var xlineCount = Math.Round(this.ActualWidth / _gridSize);

            //y축 그려질 개수 구하기
            var ylineCount = Math.Round(this.ActualHeight / _gridSize);

            //꼭지점 저장
            for (int y = 0; y < ylineCount + 1; y++)
            {
                for (int x = 0; x < xlineCount + 1; x++)
                {
                    var xPosition = x * _gridSize;
                    var yPosition = y * _gridSize;

                    //(0,0) (40,0)

                    Point rsp = new Point();
                    rsp.X = xPosition;
                    rsp.Y = yPosition;
                    ListRenderPoint.Add(rsp);


                }
            }
        }


        public Point TranslatePointToCanvas(MouseButtonEventArgs e, UIElement mapCanvas) //마우스버튼다운용
        {
            Point _stPoint = this.mainWindow.TranslatePoint(e.GetPosition(null), mapCanvas);

            return _stPoint;
        }

        public Point TranslatePointToCanvas(MouseEventArgs e, UIElement mapCanvas) //마우스 무브용 오버로딩
        {
            Point _stPoint = this.mainWindow.TranslatePoint(e.GetPosition(null), mapCanvas);

            return _stPoint;
        }



        private void stackPanel_objectWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double stackPanelWidth = e.NewSize.Width;
            double stackPanelHeight = e.NewSize.Height;

            ExpenderDockManager.Width = stackPanelWidth;
            ExpenderDockManager.Height = stackPanelHeight;
            //   MessageBox.Show(string.Format("width {0}, height {1}", ExpenderDockManager.Width, ExpenderDockManager.Height));

        }

        private void stackPanel_listWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            double stackPanelWidth = e.NewSize.Width;
            double stackPanelHeight = e.NewSize.Height;

            ListDockManager.Width = stackPanelWidth;
            ListDockManager.Height = stackPanelHeight;


        }

      


        private void ScViewer_canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {


            scViewer_Width = e.NewSize.Width; //현재크기값 저장 스크롤뷰어 위스
            scViewer_Height = e.NewSize.Height;

            // //리사이징 될 때 기억해놓은 액츄얼 사이즈와 변경된 e.width를 비교하여 처음 기억한 값보다 작을 때 대입해준다. 그러면 캔버스가 원래 가지고 있던 값대로 되서 스크롤바 상태가 커질 것.
            //if (e.NewSize.Width <= canvas_TotalWidth)
            // { 

            // mapCanvas.Width = canvas_TotalWidth;
            // mapCanvas.Height = canvas_TotalHeight;




            //     MessageBox.Show(string.Format("width {0}, height {1}", canvas_TotalWidth, canvas_TotalHeight));
            //     return;
            // }

            // double canvasWidth = e.NewSize.Width; //현재 스크롤뷰어의 크기만큼 대입됨.
            // double canvasHeight = e.NewSize.Height;

            // canvas_TotalWidth = canvasWidth;
            // canvas_TotalHeight = canvasHeight;


            // mapCanvas.Width = canvasWidth;
            // mapCanvas.Height = canvasHeight;


            // //리사이즈시 버튼 카운트를 초기화
            // btn_clickWidthCount = 0;
            // btn_clickHeightCount = 0;


        }


        double scViewer_Width; //스크롤뷰어 위스
        double scViewer_Height;
        double canvas_TotalWidth = 1400; //실행시 캔버스가 처음 가지는 넓이로 초기화
        double canvas_TotalHeight = 800; //실행시 캔버스가 처음 가지는 높이로 초기화
        int btn_clickWidthCount = 0;
        int btn_clickHeightCount = 0;
        int plusCellDistance = 80;

        private void btn_PlusWidthHeight_Click(object sender, RoutedEventArgs e)
        {

          

            ++btn_clickWidthCount;
            ++btn_clickHeightCount;


            canvas_TotalWidth = mapCanvas.Width + (plusCellDistance * btn_clickWidthCount);
            canvas_TotalHeight = mapCanvas.Height + (plusCellDistance * btn_clickHeightCount);


            mapCanvas.Width = canvas_TotalWidth;
            mapCanvas.Height = canvas_TotalHeight;

            checkCanvasWidthCount();

            btn_PlusWidthHeight.IsChecked = false;
        }

     

        public void checkCanvasWidthCount()
        {
            if (btn_clickWidthCount == 0)
            {
                btn_MinusWidhtHeight.IsEnabled = false;

            }
            else if(btn_clickWidthCount > 0) //0보다 클 경우
            {
                btn_MinusWidhtHeight.IsEnabled = true;
            }

        }


        private void btn_MinusWidhtHeight_Click(object sender, RoutedEventArgs e)
        {
            
            canvas_TotalWidth = mapCanvas.Width - (plusCellDistance * btn_clickWidthCount);
            canvas_TotalHeight = mapCanvas.Height - (plusCellDistance * btn_clickHeightCount);

            --btn_clickWidthCount;
            --btn_clickHeightCount;

          

            mapCanvas.Width = canvas_TotalWidth;
            mapCanvas.Height = canvas_TotalHeight;

            if (btn_clickWidthCount <= 0 && btn_clickHeightCount <= 0)
            {
                btn_clickWidthCount = 0;
                btn_clickHeightCount = 0;

                checkCanvasWidthCount();
                btn_MinusWidhtHeight.IsEnabled = false;
                btn_MinusWidhtHeight.IsChecked = false;
            }

        }

       
        public void control_MouseEnter(object sender, MouseEventArgs e)
        {
           // timer1.Stop();
            Cursor = Cursors.Cross;
        }

        public void control_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Arrow;
           // timer1.Start();
        }

        private void Convertimg_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void Convertimg_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }
      


        //마우스가 격자 캔버스 밖으로 나가도 다시 캔버스 안으로 돌아오면 마우스 이벤트 다시 연결시킴.
        public void mapCanvas_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if(sender is Line)
            ((Line)sender).ReleaseMouseCapture();
        }



        //-------0802

        private void panel_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Object"))
            {
                // These Effects values are used in the drag source's
                // GiveFeedback event handler to determine which cursor to display.
                if (e.KeyStates == DragDropKeyStates.ControlKey)
                {
                    e.Effects = DragDropEffects.Copy;
                }
                else
                {
                    e.Effects = DragDropEffects.Move;
                }
            }

            /*
             이 DragOver 이벤트의 처리기는 다음 작업을 수행합니다.

    Circle 사용자 정의 컨트롤이 T:system.Windows.DataObject에 패키지하여 DoDragDrop 호출 시 전달한 "개체" 데이터가 끌어온 데이터에 포함되어 있는지 확인합니다.

    "개체" 데이터가 있으면 Ctrl 키가 누른 상태인지 확인합니다.

    Ctrl 키가 누른 상태이면 Effects 속성을 Copy로 설정합니다. 그렇지 않으면 Effects 속성을 Move로 설정합니다.
             */


        }



        public void AppendToChildList(ObjConvertImageInfo ObjConvertimgInfo) // 매개변수를 주고 이 함수를 호출하면 자식의 변수에 값을 넣어주게 된다.
        {

            listWindow.ReceiveAppendedList(ObjConvertimgInfo); //자식 함수에 값 추가.

        }


        public int innerObjName = 0;
        private void panel_Drop(object sender, DragEventArgs e)
        {
            // If an element in the panel has already handled the drop,
            // the panel should not also handle it.
            if (e.Handled == false)
            {
                Canvas _panel = (Canvas)sender; //버블링된 sender(캔버스)를 판넬로 형변환.
                DockingWindows.Categori _element = (DockingWindows.Categori)e.Data.GetData("Object");
               

                if (_panel != null && _element != null)
                {
                    // Get the panel that the element currently belongs to,
                    // then remove it from that panel and add it the Children of
                    // the panel that its been dropped on.
                    WrapPanel _parent = (WrapPanel)VisualTreeHelper.GetParent(_element);



                    if (_parent != null)
                    {
                        if (e.KeyStates == DragDropKeyStates.ControlKey &&
                            e.AllowedEffects.HasFlag(DragDropEffects.Copy)) //컨트롤 눌렀을 때 무브
                        {
                           
                       //     Circle _circle = new Circle((Circle)_element);
                        //    _panel.Children.Add(_circle);
                            // set the value to return to the DoDragDrop call
                        //    e.Effects = DragDropEffects.Copy;
                        }
                        else if (e.AllowedEffects.HasFlag(DragDropEffects.Move)) //컨트롤 안눌렀을 모든 경우 
                        {

                            //if (_element.Content is Image img)
                            //{
                            //    txt = img.Name;  

                            //}

                            //var myContent = _element.Content as Grid;
                            // //string txt = null;
                            // foreach (Image myImage in myContent.Children.OfType<Image>())
                            // {
                            //     txt = myImage.Name;
                            //     break;
                            // }


                            /*

                            string txt = null;
                            foreach (TextBox tb in sp.Children.OfType<TextBox>())
                            {

                                if (tb.Name == "_" + $"{Selectedidx}")
                                {
                                    txt = tb.Text;
                                    break;
                                }
                            }   
                            */


                            Image img = _element.Content as Image;
                            ObjectInfo newInfo = new ObjectInfo();
                            object tag = img.Tag;
                            newInfo =  (ObjectInfo)tag;
                            
                          //  newInfo = (ObjectInfo)img.Tag;
                            string _ObjectType = newInfo.ObjectType;
                            string _FilePath = newInfo.FilePath; //원본 이미지 경로(오른쪽 UI에 있는 이미지 경로)
                            string _VisualName = newInfo.VisualName;


                            Image convertimg = new Image();
                            BitmapImage convertbitmap = new BitmapImage();
                            convertbitmap.BeginInit();
                            convertbitmap.UriSource = new Uri($"../../pictures/{_ObjectType}/convert/{_ObjectType}.png", UriKind.Relative);
                            convertbitmap.EndInit();
                            convertimg.Stretch = Stretch.Uniform;
                            convertimg.Source = convertbitmap;
                            convertimg.MouseEnter += Convertimg_MouseEnter;
                            convertimg.MouseLeave += Convertimg_MouseLeave;


                            //tag에 저장하기 위한 정보들
                            ObjConvertImageInfo ObjConvertimgInfo = new ObjConvertImageInfo();
                            ObjConvertimgInfo.canvasPoint = e.GetPosition(mapCanvas); //캔버스에서 이미지가 위치한 좌표 정보 기억
                            ObjConvertimgInfo.ObjectType = _ObjectType; //오브젝트타입
                            ObjConvertimgInfo.ImgFilePath = _FilePath; //원본 이미지 경로
                            ObjConvertimgInfo.VisualName = _VisualName; //비주얼 네임. 툴팁에 뜨는 이름.

                            ObjConvertimgInfo.convertFilePath = $"../../pictures/{_ObjectType}/convert/{_ObjectType}.png"; //변환 이미지 경로. 혹시 필요할까봐...
                            ObjConvertimgInfo.ObjectName = $"Object{innerObjName}"; //이 이름을 가지고 유니티와 연동해서 양쪽으로 지우고 움직이고 함.
                        //  ObjConvertimgInfo.rotationPoint = null;//회전값을 위한 2차원의 x,y point좌표.    

                            convertimg.Tag = ObjConvertimgInfo; //이미지의 tag에 정보를 기억.

                            Canvas.SetLeft(convertimg, e.GetPosition(mapCanvas).X);
                            Canvas.SetTop(convertimg, e.GetPosition(mapCanvas).Y);
                            

                            AppendToChildList(ObjConvertimgInfo); // 컨버트된 이미지 태그에 등록한 클래스 정보들을 자식의 함수를 통해 넘겨줌.



                            ObjConvertimgList.Add(ObjConvertimgInfo); //왼쪽 UI에서 지울 수 있도록 이미지의 tag에 넣었던 정보를 리스트 안에도 저장. 
                            _panel.Children.Add(convertimg); //캔버스에 이미지 출력하기 위한 자식으로 추가.
                            server.Send(fillObjectInfo(convertimg, ObjectInfoPacket.ObjectAction.CREATE));







                            //트리뷰 노드에 이름 추가.
                            //WrapPanel wrPanel = listWindow as WrapPanel;                            


                            //foreach (TreeView myTreeView in wrPanel.Children.OfType<TreeView>())
                            //{
                            //    txt = myTreeView;
                            //    break;
                            //}








                            innerObjName++;



                            // MessageBox.Show(string.Format("x: {0} || y: {1}",dropPoint.X, dropPoint.Y));


                            /*
                            _parent.Children.Remove(_element); //부모(도킹)
                           _panel.Children.Add(_element);
                           */


                            // set the value to return to the DoDragDrop call
                            //  e.Effects = DragDropEffects.Move;
                            //  e.Effects = DragDropEffects.Copy;


                        }
                    }
                }
            }
        }



        //델리게이트이벤트로값전달
        //public delegate void OnChildTextInputHandler(string Parameters);
        //public event OnChildTextInputHandler OnChildTextInputEvent;

        //public void button1_Click(object sender, RoutedEventArgs e)
        //{
        //    if (OnChildTextInputEvent != null)
        //        OnChildTextInputEvent(textBox1.Text);
        //}
        




        //메인윈도우에서 캔버스에 추가된 이미지에 대한 정보를 가지고 있어야됨. 그래야 왼쪽에서 지우면 그 정보를 가지고 캔버스의 것도 지움.
        public List<ObjConvertImageInfo> ObjConvertimgList = new List<ObjConvertImageInfo>();




        //왼쪽 트리뷰에 추가할 거.
        public void addTreeViewNodes(string _ObjectType ,string _VisualName)
        {


        }
       

        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }

        Point dropPoint;
        //--------------


        public UIElement DeepCopy(UIElement element)

        {
          //  string oriName
            string shapestring = XamlWriter.Save(element);
          //  ((FrameworkElement)element).Name = oriName;
            StringReader stringReader = new StringReader(shapestring);
            XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
            UIElement DeepCopyobject = (UIElement)XamlReader.Load(xmlTextReader);
            return DeepCopyobject;

        }

        private void mainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            MessageBox.Show(e.OriginalSource.ToString());
        }



        private void ScViewer_canvas_KeyDown(object sender, KeyEventArgs e)
        {
          //  MessageBox.Show("들어옴");

            if (e.Key == Key.Delete)
            {
                string toDeleteName = txtBox.Text;
                Line toDeleteLine = null;
                Image toDeleteImage = null;

                foreach (var el in mapCanvas.Children)
                {
                    if (el is Line l)
                    {
                        if (l.Name == toDeleteName)
                        {
                            toDeleteLine = l;                           
                            break;
                        }
                    }
                }
                if (toDeleteLine != null)
                {

                    WpfUnityPacketHeader header = fillWallInfo(toDeleteLine, WallInfo.WallInfoAction.REMOVE);
                    server.Send(header);
                    mapCanvas.Children.Remove(toDeleteLine as UIElement);
                }


                //이미지지우기
                if (obj_image != null) //이미지 선택한 게 있다면
                {
                    foreach (var el in mapCanvas.Children)
                    {
                        if (el is Image l)
                        {
                            if (l.Name == toDeleteName)
                            {
                                toDeleteImage = l;

                                //여기서 지우면 자식에게 값을 줘서 해당 오브젝트네임을 가진 녀석을 지우게 해야함.

                               ObjConvertImageInfo oct = (ObjConvertImageInfo)toDeleteImage.Tag;
                                listWindow.DeleteList(oct);



                                mapCanvas.Children.Remove(toDeleteImage as UIElement);

                               


                                break;
                            }
                        }
                    }
                }




            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string toDeleteName = txtBox.Text;
            Line toDeleteLine = null;
            Image toDeleteImage = null;

            foreach (var el in mapCanvas.Children)
            {
                if (el is Line l)
                {
                    if (l.Name == toDeleteName)
                    {
                        toDeleteLine = l;
                        break;
                    }
                }
            }

            if (toDeleteLine != null)
            {
                WpfUnityPacketHeader header = fillWallInfo(toDeleteLine, WallInfo.WallInfoAction.REMOVE);
                server.Send(header);
                mapCanvas.Children.Remove(toDeleteLine as UIElement);
            }

            
            //이미지지우기
            if (obj_image != null) //이미지 선택한 게 있다면
            {
                foreach (var el in mapCanvas.Children)
                {
                    if (el is Image l)
                    {
                        if (l.Name == toDeleteName)
                        {
                            toDeleteImage = l;

                            //여기서 지우면 자식에게 값을 줘서 해당 오브젝트네임을 가진 녀석을 지우게 해야함.

                            ObjConvertImageInfo oct = (ObjConvertImageInfo)toDeleteImage.Tag;
                            listWindow.DeleteList(oct);

                            mapCanvas.Children.Remove(toDeleteImage as UIElement);
                            


                            break;
                        }
                    }
                }
            }


        }



        public void treeviewTomainwindow_ReceiveDelimg(string _objName)
        {
            if (_objName != null)
            {

                Image delimg = null;
                foreach (var i in mapCanvas.Children)
                {
                    if (i is Image img)
                    {
                        ObjConvertImageInfo oci = (ObjConvertImageInfo)img.Tag;

                        //트리뷰에서 건내받은 _objName이 맵캔버스에 있는 이미지의 오브젝트 네임과 같다면 그걸 저장하고
                       

                        if (oci.ObjectName == _objName)
                        { 

                            delimg = img;
                            break;
                        }

                    }
                }

                mapCanvas.Children.Remove(delimg);  // 포이치문을 빠져나간 후 삭제한다.(포이치문 안에서는 삭제하면 오류남)


            }
        }


        //-----------------------------


        enum Direction
        {
            NW,
            N,
            NE,
            W,
            E,
            SW,
            S,
            SE,
            None
        }

     
        /*
            MouseButtonEventArgs e 마우스버튼다운
        MouseEventArgs e 마우스무브
            MouseButtonEventArgs e 마우스버튼업
             */


    

      
        // private void control_MouseDown(object sender, MouseButtonEventArgs e)
        // {
        //     if (e.LeftButton == Mouse.LeftButton)
        //     {
        //       //  this.Invalidate();  //unselect other control
        //         SelectedControl = (Control)sender;
        //         Control control = (Control)sender;
        //         mouseX = -e.GetPosition(null).X;
        //         mouseY = -e.GetPosition(null).Y;
        //    //     control.Invalidate();
        //     //   DrawControlBorder(sender);
        //         propertyGrid1.SelectedObject = SelectedControl;
        //     }
        // }
        // private void control_MouseMove(object sender, MouseEventArgs e)
        // {
        //     if (e.LeftButton == Mouse.LeftButton)
        //     {
        //        Control control = (System.Windows.Forms.Control)sender;
        //         Point nextPosition = new Point();
        //         nextPosition = this.TranslatePoint(nextPosition, this.mainWindow);
        //         //   nextPosition = this.PointToClient(MousePosition);
        //         nextPosition.Offset(mouseX, mouseY);

        //    //     control. = nextPosition;



        //    //       control.Location = nextPosition;
        //   //      Invalidate();
        //     }
        // }
        // private void control_MouseUp(object sender, MouseButtonEventArgs e)
        // {
        //     if (e.LeftButton == Mouse.LeftButton)
        //     {
        //         Control control = (Control)sender;
        ////         Cursor.Clip = System.Drawing.Rectangle.Empty;
        ////         control.Invalidate();
        //         //DrawControlBorder(control);
        //     }
        // }

        //public bool dragAction = false;

        //private void minuteHand_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    dragAction = true;
        //    minuteHand_MouseMove(this.minuteHand, e);
        //}

        //private void Window_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        this.DragMove();
        //    }
        //}

        //private void minuteHand_MouseLeftButtonUp(object sender, MouseEventArgs e)
        //{
        //    dragAction = false;
        //}


        //private void DrawControlBorder(object sender)
        //{
        //    Control control = (Control)sender;
        //    //define the border to be drawn, it will be offset by DRAG_HANDLE_SIZE / 2
        //    //around the control, so when the drag handles are drawn they will be seem
        //    //connected in the middle.
        //    Rectangle Border = new Rectangle(
        //        new Point(control.Location.X - DRAG_HANDLE_SIZE / 2,
        //            control.Location.Y - DRAG_HANDLE_SIZE / 2),
        //        new Size(control.Size.Width + DRAG_HANDLE_SIZE,
        //            control.Size.Height + DRAG_HANDLE_SIZE));
        //    //define the 8 drag handles, that has the size of DRAG_HANDLE_SIZE
        //    Rectangle NW = new Rectangle(
        //        new Point(control.Location.X - DRAG_HANDLE_SIZE,
        //            control.Location.Y - DRAG_HANDLE_SIZE),
        //        new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
        //    Rectangle N = new Rectangle(
        //        new Point(control.Location.X + control.Width / 2 - DRAG_HANDLE_SIZE / 2,
        //            control.Location.Y - DRAG_HANDLE_SIZE),
        //        new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
        //    Rectangle NE = new Rectangle(
        //        new Point(control.Location.X + control.Width,
        //            control.Location.Y - DRAG_HANDLE_SIZE),
        //        new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
        //    Rectangle W = new Rectangle(
        //        new Point(control.Location.X - DRAG_HANDLE_SIZE,
        //            control.Location.Y + control.Height / 2 - DRAG_HANDLE_SIZE / 2),
        //        new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
        //    Rectangle E = new Rectangle(
        //        new Point(control.Location.X + control.Width,
        //            control.Location.Y + control.Height / 2 - DRAG_HANDLE_SIZE / 2),
        //        new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
        //    Rectangle SW = new Rectangle(
        //        new Point(control.Location.X - DRAG_HANDLE_SIZE,
        //            control.Location.Y + control.Height),
        //        new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
        //    Rectangle S = new Rectangle(
        //        new Point(control.Location.X + control.Width / 2 - DRAG_HANDLE_SIZE / 2,
        //            control.Location.Y + control.Height),
        //        new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));
        //    Rectangle SE = new Rectangle(
        //        new Point(control.Location.X + control.Width,
        //            control.Location.Y + control.Height),
        //        new Size(DRAG_HANDLE_SIZE, DRAG_HANDLE_SIZE));

        //    //get the form graphic
        //    Graphics g = this.CreateGraphics();
        //    //draw the border and drag handles
        //    ControlPaint.DrawBorder(g, Border, Color.Gray, ButtonBorderStyle.Dotted);
        //    ControlPaint.DrawGrabHandle(g, NW, true, true);
        //    ControlPaint.DrawGrabHandle(g, N, true, true);
        //    ControlPaint.DrawGrabHandle(g, NE, true, true);
        //    ControlPaint.DrawGrabHandle(g, W, true, true);
        //    ControlPaint.DrawGrabHandle(g, E, true, true);
        //    ControlPaint.DrawGrabHandle(g, SW, true, true);
        //    ControlPaint.DrawGrabHandle(g, S, true, true);
        //    ControlPaint.DrawGrabHandle(g, SE, true, true);
        //    g.Dispose();
        //}



    }
}