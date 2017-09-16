#define RUN_3DVIWER_IN_UNITY_EDITER
#define RUN_3DVIWER //하단부에 유니티뷰어 띄우고 싶으면 이쪽은 주석해제하면 됨.


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
using MahApps.Metro.Controls;
using System.Windows.Interop;
using DockingLibrary;
using Nollan.Visual_Space.classes;
using System.Windows.Media.Media3D;
using Nollan.Visual_Space.Data;
using Nollan.Visual_Space.Util;
using static Nollan.Visual_Space.classes.ObjSizeInfo;

namespace Nollan.Visual_Space
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class MainWindow : MetroWindow, ICloneable
    {

        IWpfUnityTCPServer server = null;
        int zeroPos = 400;


        public MainWindow()
        {
            InitializeComponent();

#if RUN_3DVIWER
            server = new WpfUnityTCPServer("127.0.0.1", 9010);
            server.Connect();
            server.OnReceviedCompleted += OnReceviedCompleted;

#else
            server = new NullWpfUnityTCPServer();
#endif
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;

            // Window1_Loaded(); 맥시마이즈시 리사이징


        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
#if !RUN_3DVIWER_IN_UNITY_EDITER && RUN_3DVIWER
            ((UnityExe)host.Child).CloseExe();
#endif
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

#if DEBUG
            string dir = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\exe\3D Viewer.exe";
#else
        string dir = AppDomain.CurrentDomain.BaseDirectory + @"exe\3D Viewer.exe";
#endif

#if !RUN_3DVIWER_IN_UNITY_EDITER && RUN_3DVIWER
            host.Child = new UnityExe(dir);
#endif

            //-----
            //   Window1_Loaded(); //리사이징 코드
            //----//

            //818
            initActualWidth = mapCanvas.ActualWidth; // 맥시마이즈 시작시 맵캔버스의 렌더링 사이즈 초기값 저장
            initActualHeight = mapCanvas.ActualHeight;
            //    drawCircle(); //유니티 뷰어 중심점 표시


            checkCanvasWidthCount();
            showSubPanel(); //왼쪽 오른쪽 UI 띄우기



        }


        DockingWindows.ListWindow listWindow;
        DockingWindows.ObjectWindow objWindowDocking;
        DockingWindows.ColorWindow colorWindow;
        public void showSubPanel()
        {
            //0731추가-----
            //set window parent for dragging operations
            //  ListDockManager.ParentWindow = this;
            ExpenderDockManager.ParentWindow = this;

            listWindow = new DockingWindows.ListWindow();
            listWindow.call_mainwindow = this; // 자식폼에서 부모폼을 등록??? 자식에서 부모에게 갑 전달 실험용.
            listWindow.DockManager = ExpenderDockManager;// ListDockManager;            
                                                         // listWindow.Show(Dock.Top);


            objWindowDocking = new DockingWindows.ObjectWindow();
            objWindowDocking.DockManager = ExpenderDockManager;

            colorWindow = new DockingWindows.ColorWindow();
            colorWindow.Call_MainWindow = this;
            colorWindow.DockManager = ExpenderDockManager;



            listWindow.ShowAsDocument();
            objWindowDocking.ShowAsDocument();
            colorWindow.ShowAsDocument();


            //  Show(Dock.Top);
            //-------//
            ;            /*
             
             //set window parent for dragging operations
        dockManager.ParentWindow = this;

        //Show PropertyWindow docked to the top border
        propertyWindow.DockManager = dockManager;
        propertyWindow.Show(Dock.Top);

        //Show ExplorerWindow docked to the right border as default
        explorerWindow.DockManager = dockManager;
        explorerWindow.Show();

        //Show ListWindow in documents pane
        listWindow.DockManager = dockManager;
        listWindow.ShowAsDocument();             
             */

        }



        public static List<Point> ListRenderPoint = new List<Point>();//각 꼭지점 좌표를 기억하는 리스트

        Line selectedLine; //선택된 선과 선택 안된 선의 컬러를 구분해주기 위해 이전에 선택된 선을 기억하고 있는 변수.
        bool? Flag_HorizontalLinePlusMinus = null;
        bool? Flag_VerticalLinePlusMinus = null;
        bool? Result_separateLine = null;
        int i = 0;
        Image selectedImage;
        bool imageClick = false;

        //선 증가 감소 플래그
        public bool linePlusMinus_Check;

        double initActualWidth; //처음 시작시 사용자의 맥시마이즈 캔버스 초기값
        double initActualHeight; //처음 시작시 사용자의 맥시마이즈 캔버스 초기값

        bool bMouseDown = false;
        bool bLine_check = false; //선 그리기 플래그
        Point stPoint;
        Point curPoint;
        Line line;
        int _gridSize = 20; //캔버스 격자 간격 변수. 이걸 바꾸면 MapCanvas의 거도 바꿔줘야함.
        bool Flag_ImageOneClick = false;







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
#if DEBUG
            return;
            StreamWriter writer = new StreamWriter(

            File.Open("../../Log.txt", FileMode.Append));

            TextWriterTraceListener listener = new TextWriterTraceListener(writer);

            Debug.Listeners.Add(listener);

            Debug.WriteLine(string.Format("x1y1({0}, {1}), x2y2({2}, {3}) - {4} : {5}.", linex1, liney1, linex2, liney2, DateTime.Now, str));
            //Debug.WriteLine(string.Format("{0} : Test Log2.", DateTime.Now));
            Debug.Flush();

            writer.Close();
#endif
        }

        public Brush lineBrush;
        public ImageBrush ib;
        public string Wallpaper_imgFilePath; //컬러창에서 받은 이미지 경로. 이걸로 유니티에서 월페이어 정보를 얻을 수 있다.
        public string Wallpaer_imgFileName; //선택된 파일이름 저장 변수
        //824
        public bool Flag_isCustomWall; //커스텀 벽지 쪽을 눌러서 호출됬는가의 여부

        //817
        public void ChangeImgBrush(BitmapImage bitmap, string Receive_WallpaperPath, string Receive_WallpaperName, bool Receive_FlagisCustomWall)
        {
            ib = new ImageBrush(bitmap);
            Wallpaper_imgFilePath = Receive_WallpaperPath;
            Wallpaer_imgFileName = Receive_WallpaperName;

            Flag_isCustomWall = Receive_FlagisCustomWall;
            //824  벽지 에셋번들 네임 기억해야됨 

        }
        public void DeleteImgBrush()
        {
            if (ib != null)
            {
                ib = null;
                Wallpaper_imgFilePath = null;
            }

        }


        //820
        public string Floor_imgFilePath; //유니티로 보낼 바닥 파일 경로
        public string Floor_imgFileName; //선택된 파일이름 저장 변수
        public ImageBrush Floorib;        // 이미지 브러시를 통해 바닥의 이미지를 바꾼다.
        //824
        public bool Flag_isCustomFloor; //커스텀 바닥 쪽을 눌러서 호출됬는가 아닌가의 여부

        public void ChangeFloorImgBrush(BitmapImage bitmap, string Receive_FloorFilePath, string Receive_FloorFileName, bool Receive_FlagisCustomFloor)
        {

            Floorib = new ImageBrush(bitmap);
            Floorib.TileMode = TileMode.Tile; //821 타일로 반복하게
            Floor_imgFilePath = Receive_FloorFilePath;

            Floor_imgFileName = Receive_FloorFileName; //824 일반바닥이나 커스텀이나 구분하지 않고 전달되는 파일이름을 저장. 

            Flag_isCustomFloor = Receive_FlagisCustomFloor;


        }
        public void DeleteFloorImgBrush()
        {
            if (Floorib != null)
            {
                Floorib = null;
                Floor_imgFilePath = null;


            }

        }

        //마우스다운시
        private void MapCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {


            //816
            MakeFloor_ClickDOWN(sender, e); //바닥 생성



            // 선을 그리는 경우 마우스다운시
            if (!bMouseDown && bLine_check && !linePlusMinus_Check && selectedLine == null && selectedImage == null)
            {

                this.mapCanvas.CaptureMouse(); //831

                bMouseDown = true;
                stPoint = TranslatePointToCanvas(e, mapCanvas);
                remainderToFindVertex(stPoint.X, stPoint.Y); //가장 가까운 꼭지점을 찾는다. 그 값은 튜플형식으로 Result_StartPoint에 담기게됨.
                line = new Line();

                //812
                Canvas.SetZIndex(line, 1);

                line.MouseEnter += control_MouseEnter;
                line.MouseLeave += control_MouseLeave;

                line.X1 = Result_StartPoint.Item1;
                line.X2 = Result_StartPoint.Item1;
                line.Y1 = Result_StartPoint.Item2;
                line.Y2 = Result_StartPoint.Item2;




                //815 리니어그라디언트브러시
                //LinearGradientBrush lgb = new LinearGradientBrush();
                //lgb.StartPoint = new Point(0, 0.5);
                //lgb.EndPoint = new Point(1, 0.5);
                //lgb.GradientStops.Add(new GradientStop(Colors.Yellow, 0.0));
                //lgb.GradientStops.Add(new GradientStop(Colors.Red, 0.25));
                //lgb.GradientStops.Add(new GradientStop(Colors.Blue, 0.75));
                //lgb.GradientStops.Add(new GradientStop(Colors.Yellow, 1.0));
                // line.Stroke = lgb;




                // System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap("..\\..\\icons\tiles\\linetile1.png");

                //  ImageBrush ib = new ImageBrush( new BitmapImage(new Uri("D:/winshare/mainproject/VisualSpace/Visual Space/Visual Space/icons/tiles/linetile1.png", UriKind.Absolute)  ));

                //pack://application:,,,/Resources/MyImage.png
                //AppDomain.CurrentDomain.BaseDirectory+$"../../../pictures/{_ObjectType}/convert/{_ObjectType}.png";

                /*
                      myImage3 = new Image();
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri($"../../pictures/{txt}.PNG", UriKind.Relative);
            bi3.EndInit(); 
                 */

                //817
                if (ib != null)
                {
                    ib.AlignmentX = AlignmentX.Center;
                    ib.AlignmentY = AlignmentY.Center;
                    ib.Stretch = Stretch.UniformToFill;
                    line.Stroke = ib;

                    //823
                    WallConvertInfo wci = new WallConvertInfo();
                    wci.WallLineimgBrush = ib;
                    //824
                    wci.AssetBundleName = Wallpaer_imgFileName; //컬러창에서 선택할 시 다이얼로그에서 선택된 이름 wallpaper1.png 을 기억

                    line.Tag = wci; //선이 생성된 라인의 경우 태그에 자신에게 그려진 이미지 브러시를 가지고 있게 된다.



                }
                else //ib, 즉 이미지 브러시가 널 일 때, 선택된 벽지가 없을 때 이쪽으로 들어옴.
                {


                    //824 디폴트일 경우 태그에 디폴트라고 넣어줌
                    WallConvertInfo wci = new WallConvertInfo();
                    wci.AssetBundleName = "default";
                    line.Tag = wci;


                    line.Stroke = Brushes.Black;
                    //line.StrokeDashArray = DoubleCollection.Parse("1, 0.1");
                }


                //line.Stroke = Brushes.Black;

                line.StrokeThickness = 6;
                line.Name = $"Line_{i++}";
                mapCanvas.Children.Add(line);

            }

            // 선 그리기가 아닐때는 어떠한 오브젝트가 선택되었는지 확인한다.
            // 우선 Line 선택되었는지 확인한다.
            else
            {

                //0731추가------
                //선이 그려져 있고, 선택된 선이 있는데 그냥 허공(캔버스)를 선택하는 경우                
                if (selectedLine != null)
                {
                    if (e.OriginalSource != line) //오리지널소스가 선이 아닐 때.(맵캔버스 or 이미지일 경우가 된다.)
                    {
                        // 선택해재 정보 전송
                        server.Send(fillWallInfo(selectedLine, WpfUnityCommand.DESELECT));

                        ////815
                        //if (lineBrush != null) //라인브러시 설정했으면 설정한 색대로
                        //{
                        //    selectedLine.Stroke = lineBrush;
                        //}
                        //else
                        //{
                        //    selectedLine.Stroke = Brushes.Black;
                        //}


                        //817

                        //823

                        WallConvertInfo wci = selectedLine.Tag as WallConvertInfo;

                        if (wci.WallLineimgBrush != null)
                        {
                            selectedLine.Stroke = wci.WallLineimgBrush; // 선
                                                                        // selectedLine.Stroke = ib;
                        }
                        else
                        {
                            selectedLine.Stroke = Brushes.Black;
                            selectedLine.StrokeDashArray = DoubleCollection.Parse("1, 0.1");
                        }






                        //  selectedLine.Stroke = Brushes.Black;


                        selectedLine = null; //선택된 선 해제.

                        /*
                    1.선택된 선이 있을 때 선 그릴 경우 일단 캔버스를 클릭하게 되므로 선택해제가 됨. 
                    2.캔버스를 누르면 선택이 취소됨
                    3.선증가감소 체크 버튼이 켜져있을 시 선을 누르면 선택한 것임.
                    4.선이동시(선그리기아니고, 선증감도 아닐 때) 내려놓으면 붉은색으로 남아있음.    

                     */

                    }
                }

                // 전에 선택된것이 오브젝트일때
                else if (selectedImage != null)
                {
                    server.Send(fillObjectInfo(selectedImage, WpfUnityCommand.DESELECT));

                    //811
                    DeleteControlBorder(selectedImage); //보더 지운다


                    selectedImage = null;
                }

                // 선 클릭                                         //817 바닥 생성이 Off일 때 조건 추가
                if (e.OriginalSource.GetType() == typeof(Line) && !Flag_isCheckedFloor) //클릭했을 시 오리지널소스가 Line일 경우
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

                            // writeLog(line.X1, line.Y1, line.X2, line.Y2, "마우스다운-세퍼레이트라인처리 전");
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
                //  Object(Image)가 선택되었는지 확인한다. //817 바닥생성 on일 경우 이미지 움직이지 않음.
                else if (e.OriginalSource.GetType() == typeof(Image) && !Flag_isCheckedFloor) // 클릭했을 때 original source가 image인 경우
                {
                    selectedImage = e.OriginalSource as Image; //현재 클릭된 이미지를 기억?
                                                           // MessageBox.Show("이미지");
                                                           //  imageStPoint = TranslatePointToCanvas(e, mapCanvas); //클릭한 좌표를 맵캔버스 기준으로 변환해서 반환값을 가져온다.


                    //811
                    if (selectedImage != null)
                    {
                        imageClick = true;

                        //818
                        Flag_ImageOneClick = true;

                        txtBox.Text = selectedImage.Name;

                        server.Send(fillObjectInfo(selectedImage, WpfUnityCommand.SELECT));



                        // e

                        DrawControlBorder(selectedImage); //이미지 클릭하면 보더생김.

                    }



                    //  MessageBox.Show((obj_image.Name).ToString());

                }

            }

        }

        //마우스무브시
        private void MapCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            //812
            if (Flag_isCheckedFloor) //바닥 생성
            {

                //현재 이동한 마우스의 좌표를 얻어온다
                Point currnetPosition = e.GetPosition(mapCanvas);
                //좌표를 표시한다.
                // this.tbPosition.Text = string.Format("마우스 좌표 : [{0},{1}]", currnetPosition.X, currnetPosition.Y);
                //마우스 왼쪽 버튼이 눌려있으면
                if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                {

                    if (currentRect != null)
                    {
                        //사각형이 나타날 기준점을 설정한다. 처음 마우스오른쪽 이벤트 발생지점.
                        double left = prePosition.X;
                        double top = prePosition.Y;


                        //마우스의 위치에 따라 적절히 기준점을 변경한다.
                        //if (prePosition.X > currnetPosition.X && prePosition.Y > currnetPosition.Y)
                        //{

                        //    /*  프리 > 현재점 */

                        //    if (prePosition.X > currnetPosition.X)
                        //    {

                        //        double distanceX = Math.Abs(left - currnetPosition.X);
                        //        currentRect.Width = currnetPosition.X + distanceX;

                        //      //  prePosition.X = currnetPosition.X;


                        //    }
                        //    if (prePosition.Y > currnetPosition.Y)
                        //    {

                        //        double distanceX = Math.Abs(left - currnetPosition.Y);
                        //        currentRect.Height = currnetPosition.Y + distanceX;

                        //      //  prePosition.Y = currnetPosition.Y;


                        //    }

                        //    Canvas.SetLeft(currentRect, currnetPosition.X);
                        //    Canvas.SetTop(currentRect, currnetPosition.Y);

                        //    return;
                        //}





                        //사각형의 크기를 설정한다. 음수가 나올 수 없으므로 절대값을 취해준다.
                        //   currentRect.Width = Math.Abs(prePosition.X - currnetPosition.X);
                        //   currentRect.Height = Math.Abs(prePosition.Y - currnetPosition.Y);

                        //816 바닥 최소 범위(_gridSize 이하로 내려가지 않도록)
                        double AbsX = Math.Abs(prePosition.X - currnetPosition.X);
                        double AbsY = Math.Abs(prePosition.Y - currnetPosition.Y);

                        if (AbsX < _gridSize)
                        {
                            currentRect.Width = _gridSize;
                        }
                        else
                        {
                            currentRect.Width = AbsX;
                        }

                        if (AbsY < _gridSize)
                        {
                            currentRect.Height = _gridSize;
                        }
                        else
                        {
                            currentRect.Height = AbsY;
                        }


                        Canvas.SetLeft(currentRect, prePosition.X);
                        Canvas.SetTop(currentRect, prePosition.Y);

                    }
                }

            }
            //812
            else //( !Flag_isCheckedFloor ) //바닥 생성 버튼이 꺼져있다면? 이동을 의미
            {
                if (e.MouseDevice.LeftButton == MouseButtonState.Pressed
                    && selectedRectangle != null && !Flag_isCheckedFloor && Flag_ImageOneClick) //왼쪽 눌렸을 때 , 818 플래그 추가
                {
                    Point Rc_CurPoint = e.GetPosition(mapCanvas);

                    double Rc_distanceX = Rc_CurPoint.X - prePosition.X;
                    double Rc_distanceY = Rc_CurPoint.Y - prePosition.Y;

                    Canvas.SetLeft(selectedRectangle, prePosition.X + Rc_distanceX - (selectedRectangle.ActualWidth / 2));
                    Canvas.SetTop(selectedRectangle, prePosition.Y + Rc_distanceY - (selectedRectangle.ActualHeight / 2));

                    Flag_imgMove = true; //818 여기서 트루해줘야 바닥 움직일 수 있음.
                }
            }






            if (imageClick == false)
            {

                //선 그리기
                if (bMouseDown && bLine_check)
                {
                    //선 그릴 때 늘어났다 줄어났다를 마우스 다운하는 지점에 맞춰야하니까.
                    curPoint = TranslatePointToCanvas(e, mapCanvas);
                    line.X2 = curPoint.X;
                    line.Y2 = curPoint.Y;


                    //815
                    //if (Math.Abs(line.X1 - line.X2) > Math.Abs(line.Y1 - line.Y2)) //x의 차이가 더 크면 수평
                    //{
                    //    ImageBrush ib = new ImageBrush(new BitmapImage(
                    //        new Uri("D:/winshare/mainproject/VisualSpace/Visual Space/Visual Space/icons/tiles/linetile1_h.png", UriKind.Absolute)));
                    //  //  ib.TileMode = TileMode.Tile;
                    //    ib.Stretch = Stretch.None;
                    //    line.Stroke = ib;

                    //}
                    //if (Math.Abs(line.X1 - line.X2) < Math.Abs(line.Y1 - line.Y2)) //x의 차이가 더 크면 수평
                    //{
                    //    ImageBrush ib = new ImageBrush(new BitmapImage(
                    //        new Uri("D:/winshare/mainproject/VisualSpace/Visual Space/Visual Space/icons/tiles/linetile1.png", UriKind.Absolute)));
                    //    //ib.TileMode = TileMode.Tile;
                    //    ib.Stretch = Stretch.None;
                    //    line.Stroke = ib;

                    //}



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
                    if (line != null && !linePlusMinus_Check && !Flag_isCheckedFloor) //816 바닥 생성 플래그가 false일 때. 
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
                //817 바닥생성 플래그가 off 상태여야 이미지 움직일 수 있음
                if (selectedImage != null && !Flag_isCheckedFloor && Flag_ImageOneClick == true) //816 이거 안하면 이미지 클릭 삭제 하다보면 오류 뜸
                {
                    ObjConvertImageInfo oci = (ObjConvertImageInfo)selectedImage.Tag;

                    //818
                    Flag_imgMove = true;

                    Canvas.SetLeft(selectedImage, e.GetPosition(mapCanvas).X - selectedImage.ActualWidth / 2);
                    Canvas.SetTop(selectedImage, e.GetPosition(mapCanvas).Y - selectedImage.ActualHeight / 2);


                    //811
                    if (oci.ObjectType == "doors" || oci.ObjectType == "windows")
                    {
                        Point windoorClickPoint = new Point(e.GetPosition(mapCanvas).X, e.GetPosition(mapCanvas).Y); //마우스현재좌표 얻어오기
                        Point bitSize = new Point(selectedImage.ActualWidth, selectedImage.ActualHeight);
                        WindowDoorPointSearching(selectedImage, bitSize, windoorClickPoint);
                    }

                }

            }





        }

        bool Flag_imgMove = false;

        private void MapCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            //816
            MakeFloor_ClickUP(sender, e); //바닥 생성 or 이동 쪽 로직


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


                    WpfUnityPacketHeader header = fillWallInfo(line, WpfUnityCommand.CREATE);
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

                        WpfUnityPacketHeader header = fillWallInfo(line, WpfUnityCommand.MOVE);
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

                        WpfUnityPacketHeader header = fillWallInfo(line, WpfUnityCommand.MOVE);
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


                if (selectedImage != null && Flag_imgMove) //816 오류방지, 818 플래그 추가하여 이미지 클릭시 움직이지 않게.(무브해야 플래그 트루됨)
                {
                    Canvas.SetLeft(selectedImage, e.GetPosition(mapCanvas).X - selectedImage.ActualWidth / 2);
                    Canvas.SetTop(selectedImage, e.GetPosition(mapCanvas).Y - selectedImage.ActualHeight / 2);



                    //811
                    ObjConvertImageInfo oci = (ObjConvertImageInfo)selectedImage.Tag;

                    if (oci.ObjectType == "doors" || oci.ObjectType == "windows")
                    {
                        Point windoorClickPoint = new Point(e.GetPosition(mapCanvas).X, e.GetPosition(mapCanvas).Y); //마우스현재좌표 얻어오기
                        Point bitSize = new Point(selectedImage.ActualWidth, selectedImage.ActualHeight);
                        WindowDoorPointSearching(selectedImage, bitSize, windoorClickPoint);
                    }


                    server.Send(fillObjectInfo(selectedImage, WpfUnityCommand.MOVE));


                    Flag_ImageOneClick = false; //818
                    Flag_imgMove = false; //818
                    //obj_image = null;
                }
            }

            this.mapCanvas.ReleaseMouseCapture();

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
            catch (Exception)
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
                catch (Exception)
                {

                }
            }

            ms.Close();


        }
        #endregion



        private WpfUnityPacketHeader fillObjectInfo(Image img, WpfUnityCommand action)
        {
            // 공통적인 작업
            ObjectInfoPacket objectInfo = new ObjectInfoPacket();
            ObjConvertImageInfo ObjInfo = (ObjConvertImageInfo)img.Tag;
            objectInfo.Name = ObjInfo.ObjectName;
            objectInfo.ObjectName = ObjInfo.AssetBundleName;
            objectInfo.Command = action;

            // del 일때는 굳이 다른 정보가 필요없다.
            switch (action)
            {

                case WpfUnityCommand.CREATE:
                case WpfUnityCommand.MOVE:


                    // 계산
                    // 2d 좌표 기준으로 200 / 200Canvas.GetTop(img) 센터로 지정한다.

                    float xc = (float)Canvas.GetLeft(img) - zeroPos + (float)img.Source.Width / 2;
                    float yc = ((float)Canvas.GetTop(img) - zeroPos + (float)img.Source.Height / 2) * -1;


                    objectInfo.PosX = xc / 40;
                    objectInfo.PosY = 0f;
                    objectInfo.PosZ = yc / 40;

                    objectInfo.Rotation = (float)ObjInfo.rotationAngle;
                    break;

                case WpfUnityCommand.ROTATION:
                    objectInfo.Rotation = (float)ObjInfo.rotationAngle;
                    break;

            }


            return objectInfo;
        }


        private WpfUnityPacketHeader fillWallInfo(Line line, WpfUnityCommand action)
        {

            // 공통적인 작업
            WallInfoPacket wallInfo = new WallInfoPacket();
            wallInfo.Name = line.Name;
            wallInfo.Command = action;
            WallConvertInfo wcinfo = (WallConvertInfo)line.Tag;



            // del 일때는 굳이 좌표정보가 필요없다.
            switch (action)
            {


                case WpfUnityCommand.CREATE:
                case WpfUnityCommand.MOVE:


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

                    wallInfo.PosX = xc / 40;
                    wallInfo.PosY = 1.5f;

                    wallInfo.PosZ = yc / 40;

                    wallInfo.ScaleX = w / 40;
                    wallInfo.ScaleY = 3f;
                    wallInfo.ScaleZ = h / 40;

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

            if (action == WpfUnityCommand.CREATE)
            {

                ImageBrush ib = line.Stroke as ImageBrush;
                wallInfo.ImageData = ImageConvertor.ConvertImageBrushToJpegByteArray(ib);
            }


            return wallInfo;


        }


        private WpfUnityPacketHeader fillFloorInfo(Rectangle rect, WpfUnityCommand command)
        {

            // 공통적인 작업
            FloorInfoPacket floorInfo = new FloorInfoPacket();
            floorInfo.Name = rect.Name;
            floorInfo.Command = command;

            FloorConvertInfo fcinfo = (FloorConvertInfo)rect.Tag;



            // del 일때는 굳이 좌표정보가 필요없다.
            switch (command)
            {
                case WpfUnityCommand.CREATE:
                case WpfUnityCommand.MOVE:


                    // 계산
                    // 2d 좌표 기준으로 200 / 200 센터로 지정한다.
                    float xc = (float)Canvas.GetLeft(rect) - zeroPos + (float)rect.Width / 2;
                    float yc = ((float)Canvas.GetTop(rect) - zeroPos + (float)rect.Height / 2) * -1;


                    floorInfo.PosX = xc / 40;
                    floorInfo.PosY = 0;
                    floorInfo.PosZ = yc / 40;

                    floorInfo.ScaleX = (float)rect.Width / 40;
                    floorInfo.ScaleY = 0.01f;
                    floorInfo.ScaleZ = (float)rect.Height / 40;


                    break;

            }
            if (command == WpfUnityCommand.CREATE)
            {

                ImageBrush ib = rect.Fill as ImageBrush;
                floorInfo.ImageData = ImageConvertor.ConvertImageBrushToJpegByteArray(ib);
            }


            return floorInfo;


        }


        private WpfUnityPacketHeader fillCommandInfo(WpfUnityCommand command)
        {
            // 공통적인 작업
            CommandPacket cmd = new CommandPacket();
            cmd.Command = command;
            return cmd;


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
                    //selectedLine.Stroke = Brushes.Black;


                    ////815
                    //if (lineBrush != null) //라인브러시 설정했으면 설정한 색대로
                    //{
                    //    selectedLine.Stroke = lineBrush;
                    //}
                    //else
                    //{
                    //    selectedLine.Stroke = Brushes.Black;
                    //}

                    //817
                    if (ib != null)
                    {
                        selectedLine.Stroke = ib;
                    }
                    else if (ib != null && selectedLine.Tag != null) //라인브러시 설정했으면 설정한 색대로
                    {
                        WallConvertInfo wci = selectedLine.Tag as WallConvertInfo;
                        selectedLine.Stroke = wci.WallLineimgBrush;
                    }
                    else
                    {
                        selectedLine.Stroke = Brushes.Black;
                        selectedLine.StrokeDashArray = DoubleCollection.Parse("1, 0.1");
                    }


                    server.Send(fillWallInfo(selectedLine, WpfUnityCommand.DESELECT));

                    line.Stroke = Brushes.Red;
                    server.Send(fillWallInfo(line, WpfUnityCommand.SELECT));

                }
                else //else1. 선을 한 번도 선택을 하지 않았으면 이쪽으로.
                {
                    line.Stroke = Brushes.Red;
                    server.Send(fillWallInfo(line, WpfUnityCommand.SELECT));
                }

            }

        }



        public bool Flag_isCheckedFloor = false;
        private void Toggle_Floor_Checked(object sender, RoutedEventArgs e)
        {
            // Flag_isCheckedFloor = true;


            if (Flag_isCheckedFloor == false)
            {
                Flag_isCheckedFloor = true;


                //816
                if (bLine_check) // 바닥 생성이 켜져있는데 선그리기도 켜져있으면 바닥 생성 해제
                {
                    Ch_line.IsChecked = false;
                    bLine_check = false;

                }
                else if (linePlusMinus_Check)
                {

                    Ch_linePlusMinus.IsChecked = false;
                    linePlusMinus_Check = false;

                }


            }
            else //Flag_isCheckedFloor == true
            {
                Flag_isCheckedFloor = false;
            }


        }

        private void Toggle_Floor_Unchecked(object sender, RoutedEventArgs e)
        {

            Flag_isCheckedFloor = false;

        }


        private void Ch_line_Checked(object sender, RoutedEventArgs e)
        {
            if (bLine_check == false)
            {
                bLine_check = true;


                //816
                if (Flag_isCheckedFloor) // 바닥 생성이 켜져있는데 선그리기도 켜져있으면 바닥 생성 해제
                {
                    Toggle_Floor.IsChecked = false;
                    Flag_isCheckedFloor = false;

                }


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

                //816
                Toggle_Floor.IsChecked = false;
            }

            linePlusMinus_Check = true;
            Ch_line.IsEnabled = false;


        }



        public void Ch_linePlusMinus_Unchecked(object sender, RoutedEventArgs e)
        {
            linePlusMinus_Check = false;
            Ch_line.IsEnabled = true;
        }




        //817 자믈에서 호출 지움. 
        public void mapCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            //처음 실행시
            double canvasWidth = e.NewSize.Width;
            double canvasHeight = e.NewSize.Height;

            //     mapCanvas.Width = canvas_FirstWidth;
            //    mapCanvas.Height = canvas_FirstHeight;

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
            //if (e.NewSize.Width <= canvas_FirstWidth)
            // { 

            // mapCanvas.Width = canvas_FirstWidth;
            // mapCanvas.Height = canvas_FirstHeight;




            //     MessageBox.Show(string.Format("width {0}, height {1}", canvas_FirstWidth, canvas_FirstHeight));
            //     return;
            // }

            // double canvasWidth = e.NewSize.Width; //현재 스크롤뷰어의 크기만큼 대입됨.
            // double canvasHeight = e.NewSize.Height;

            // canvas_FirstWidth = canvasWidth;
            // canvas_FirstHeight = canvasHeight;


            // mapCanvas.Width = canvasWidth;
            // mapCanvas.Height = canvasHeight;


            // //리사이즈시 버튼 카운트를 초기화
            // btn_clickWidthCount = 0;
            // btn_clickHeightCount = 0;


        }


        double scViewer_Width; //스크롤뷰어 위스
        double scViewer_Height;
        //  double canvas_FirstWidth = 1400; //실행시 캔버스가 처음 가지는 넓이로 초기화
        //  double canvas_FirstHeight = 800; //실행시 캔버스가 처음 가지는 높이로 초기화
        int btn_clickWidthCount = 0;
        int btn_clickHeightCount = 0;
        int plusCellDistance = 80;

        private void btn_PlusWidthHeight_Click(object sender, RoutedEventArgs e)
        {



            ++btn_clickWidthCount;
            ++btn_clickHeightCount;

            //817 맵캔버스 2배씩 증가되는 거 수정
            mapCanvas.Width = initActualWidth + (plusCellDistance * btn_clickWidthCount);
            mapCanvas.Height = initActualHeight + (plusCellDistance * btn_clickHeightCount);


            //   mapCanvas.Width = canvas_FirstWidth;
            //  mapCanvas.Height = canvas_FirstHeight;

            checkCanvasWidthCount();

            btn_PlusWidthHeight.IsChecked = false;
        }



        public void checkCanvasWidthCount()
        {
            if (btn_clickWidthCount == 0)
            {
                btn_MinusWidhtHeight.IsEnabled = false;

            }
            else if (btn_clickWidthCount > 0) //0보다 클 경우
            {
                btn_MinusWidhtHeight.IsEnabled = true;
            }

        }


        private void btn_MinusWidhtHeight_Click(object sender, RoutedEventArgs e)
        {

            mapCanvas.Width = mapCanvas.Width - (plusCellDistance * 1);
            mapCanvas.Height = mapCanvas.Height - (plusCellDistance * 1);

            --btn_clickWidthCount;
            --btn_clickHeightCount;



            // mapCanvas.Width = canvas_FirstWidth;
            // mapCanvas.Height = canvas_FirstHeight;

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
            if (sender is Line)
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







                            Image img = _element.Content as Image;
                            ObjectInfo newInfo = (ObjectInfo)img.Tag;


                            //  newInfo = (ObjectInfo)img.Tag;
                            string _ObjectType = newInfo.ObjectType;
                            string _FilePath = newInfo.FilePath; //원본 이미지 경로(오른쪽 UI에 있는 이미지 경로)
                            string _VisualName = newInfo.VisualName;
                            string _assetBundleName = newInfo.AssetBundleName;

                            //816
                            int _price = newInfo.price;
                            string _brand = newInfo.brand;
                            string _explain = newInfo.explain;


                            //824                            
                            string _objConvertSize = newInfo.obj_ConvertSize.ToString();


                            //#if DEBUG
                            //                            string convertImgPath = AppDomain.CurrentDomain.BaseDirectory + $"../../../pictures/{_ObjectType}/convert/{_ObjectType}.png";

                            //#else
                            //                            string convertImgPath = AppDomain.CurrentDomain.BaseDirectory + $"pictures/{_ObjectType}/convert/{_ObjectType}.png";
                            //#endif

#if DEBUG
                            string convertImgPath = AppDomain.CurrentDomain.BaseDirectory + $"../../../pictures/{_ObjectType}/convert/{_objConvertSize}.png";

#else
                                                        string convertImgPath = AppDomain.CurrentDomain.BaseDirectory + $"pictures/{_ObjectType}/convert/{_objConvertSize}.png";
#endif


                            Image convertimg = new Image();
                            BitmapImage convertbitmap = new BitmapImage();
                            convertbitmap.BeginInit();
                            convertbitmap.UriSource = new Uri(convertImgPath, UriKind.RelativeOrAbsolute);
                            convertbitmap.EndInit();

                            //  int bitpixwidth = convertbitmap.PixelWidth;
                            //  int bitpixheight = convertbitmap.PixelHeight;
                            double bitWidth = convertbitmap.Width;   // 드롭할 때도 가운데에 이미지가 나오게 하기 위해 비트맵변환한 크기 알아오기
                            double bitHeight = convertbitmap.Height; //


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
                            ObjConvertimgInfo.AssetBundleName = _assetBundleName; // 에셋번들이름

                            //816
                            ObjConvertimgInfo.price = _price;
                            ObjConvertimgInfo.brand = _brand;
                            ObjConvertimgInfo.explain = _explain;



                            ObjConvertimgInfo.convertFilePath = convertImgPath; //변환 이미지 경로. 혹시 필요할까봐...
                            ObjConvertimgInfo.ObjectName = $"Object{innerObjName}"; //이 이름을 가지고 유니티와 연동해서 양쪽으로 지우고 움직이고 함.
                                                                                    //  ObjConvertimgInfo.rotationPoint = null;//회전값을 위한 2차원의 x,y point좌표.    

                            ObjConvertimgInfo.obj_ConvertSize = newInfo.obj_ConvertSize;

                            //---회전 811                      
                            rotateTransform = new RotateTransform();
                            convertimg.RenderTransform = rotateTransform;
                            convertimg.RenderTransformOrigin = new Point(0.5, 0.5); //이미지 가운데 지점?

                            //812
                            Canvas.SetZIndex(convertimg, 3);

                            //회전백터 포지션
                            position = VisualTreeHelper.GetOffset(convertimg);
                            position.X += convertimg.ActualWidth / 2;
                            position.Y += convertimg.ActualHeight / 2;
                            //---


                            convertimg.Name = ObjConvertimgInfo.ObjectName;
                            convertimg.Tag = ObjConvertimgInfo; //이미지의 tag에 정보를 기억.






                            Canvas.SetLeft(convertimg, e.GetPosition(mapCanvas).X - bitWidth / 2);
                            Canvas.SetTop(convertimg, e.GetPosition(mapCanvas).Y - bitHeight / 2);


                            //오브젝트 타입이 문이나 창문일 경우, 클릭한 지점에서 가장 가까운 모서리점을 찾아서 그곳에 중심이 위치해야 한다.
                            if (_ObjectType == "doors" || _ObjectType == "windows")
                            {
                                Point windoorClickPoint = new Point(e.GetPosition(mapCanvas).X, e.GetPosition(mapCanvas).Y); //마우스현재좌표 얻어오기
                                Point bitSize = new Point(bitWidth, bitHeight);
                                WindowDoorPointSearching(convertimg, bitSize, windoorClickPoint);

                                // return;
                            }



                            AppendToChildList(ObjConvertimgInfo); //리스트쪽으로 컨버트된 이미지 태그에 등록한 클래스 정보들을 자식의 함수를 통해 넘겨줌.



                            ObjConvertimgList.Add(ObjConvertimgInfo); //왼쪽 UI에서 지울 수 있도록 이미지의 tag에 넣었던 정보를 리스트 안에도 저장. 
                            _panel.Children.Add(convertimg); //캔버스에 이미지 출력하기 위한 자식으로 추가.



                            server.Send(fillObjectInfo(convertimg, WpfUnityCommand.CREATE));



                            innerObjName++;


                            //811
                            if (selectedImage != null) //선택된 이미지가 있다면 맵캔버스에 새로 추가되니 선택된 이미지 취소하자.
                            {

                                DeleteControlBorder(selectedImage);
                                selectedImage = null;

                            }

                        }
                    }
                }
            }
        }


        //창문
        public void WindowDoorPointSearching(Image _convertimg, Point _bitSize, Point _windoorClickPoint)
        {

            remainderToFindVertex(_windoorClickPoint.X, _windoorClickPoint.Y); //가장 가까운 꼭지점 튜플로 저장

            //Result_StartPoint //튜플

            Canvas.SetLeft(_convertimg, Result_StartPoint.Item1 - _bitSize.X / 2);
            Canvas.SetTop(_convertimg, Result_StartPoint.Item2 - _bitSize.Y / 2);


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
        public void addTreeViewNodes(string _ObjectType, string _VisualName)
        {


        }


        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }



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




        // 170808 수정 : 버튼 클릭과 delete 키 눌러서 작동하는 로직이 같으므로  DeleteObjectOrLine()라는 메서드로
        // 분리하며 호출하게 함.

        private void ScViewer_canvas_KeyDown(object sender, KeyEventArgs e)
        {
            //  MessageBox.Show("들어옴");

            if (e.Key == Key.Delete)
            {
                if (selectedImage != null)
                {
                    WpfUnityPacketHeader header = fillObjectInfo(selectedImage, WpfUnityCommand.REMOVE);
                    server.Send(header);
                    DeleteObject();



                }
                else if (selectedLine != null)
                {

                    WpfUnityPacketHeader header = fillWallInfo(selectedLine, WpfUnityCommand.REMOVE);
                    server.Send(header);
                    DeleteLine();
                }

                //812
                else if (selectedRectangle != null)

                {

                    WpfUnityPacketHeader header = fillFloorInfo(selectedRectangle, WpfUnityCommand.REMOVE);
                    server.Send(header);
                    DeleteRectangle();


                }


            }
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

            if (selectedImage != null)
            {

                WpfUnityPacketHeader header = fillObjectInfo(selectedImage, WpfUnityCommand.REMOVE);
                server.Send(header);

                DeleteObject();
            }
            else if (selectedLine != null)
            {

                WpfUnityPacketHeader header = fillWallInfo(selectedLine, WpfUnityCommand.REMOVE);
                server.Send(header);

                DeleteLine();
            }
            else if (selectedRectangle != null)
            {
                WpfUnityPacketHeader header = fillFloorInfo(selectedRectangle, WpfUnityCommand.REMOVE);
                server.Send(header);
                DeleteRectangle();
            }
        }


        public void DeleteObject()
        {

            ObjConvertImageInfo oct = (ObjConvertImageInfo)selectedImage.Tag;
            listWindow.DeleteList(oct);

            //811
            //  aLayer.Remove(aLayer.GetAdorners(obj_image)[0]);


            mapCanvas.Children.Remove(selectedImage as UIElement);
            selectedImage = null;

        }

        public void DeleteLine()
        {
            mapCanvas.Children.Remove(selectedLine as UIElement);
            selectedLine = null;
        }

        public void DeleteRectangle()
        {

            mapCanvas.Children.Remove(selectedRectangle as UIElement);
            selectedRectangle = null;

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
                            //   DeleteControlBorder(img); 
                            DeleteControlBorder();//이미지에 생성된 보더 지우기. 리스트쪽에서 지울 땐 맵캔버스에 있는 보더 무조건 없애버린다. 831
                            delimg = img;
                            break;
                        }

                    }
                }

                mapCanvas.Children.Remove(delimg);  // 포이치문을 빠져나간 후 삭제한다.(포이치문 안에서는 삭제하면 오류남)
                selectedImage = null; //831

            }
        }

        //리스트창 에서 이미지 지우려 하면 보더 있을 경우 보더 지우고 화면에 보더 없애줌.
        public void DeleteControlBorder()
        {
            if (selectedImage != null)
            {
                aLayer.Remove(aLayer.GetAdorners(selectedImage)[0]);
            }
        }



        //811
        public Vector position;
        public RotateTransform rotateTransform;
        public int rotateAngle = 45; //회전각도 상수
        private void RightAngleTransform_Click(object sender, RoutedEventArgs e) //오른쪽 회전. 45도씩.
        {
            if (selectedImage != null)
            {



                if (((RotateTransform)selectedImage.RenderTransform).Angle == 360)
                {
                    ((RotateTransform)selectedImage.RenderTransform).Angle = 0;

                }

                 ((RotateTransform)selectedImage.RenderTransform).Angle += rotateAngle;


                ObjConvertImageInfo oci = (ObjConvertImageInfo)selectedImage.Tag;
                oci.rotationAngle = ((RotateTransform)selectedImage.RenderTransform).Angle;
                selectedImage.Tag = oci; //변한 회전값 입력해줌.        

                // MessageBox.Show(rotateTransform.Angle.ToString());
                //  MessageBox.Show(((RotateTransform)obj_image.RenderTransform).Angle.ToString());

                server.Send(fillObjectInfo(selectedImage, WpfUnityCommand.ROTATION));

            }
        }

        //811
        private void LeftAngleTransform_Click(object sender, RoutedEventArgs e) //왼쪽 회전버튼 -45도씩.
        {
            if (selectedImage != null)
            {

                if (((RotateTransform)selectedImage.RenderTransform).Angle == 0)
                {
                    ((RotateTransform)selectedImage.RenderTransform).Angle = 360;
                    //MessageBox.Show("-0임");
                }

                ((RotateTransform)selectedImage.RenderTransform).Angle -= rotateAngle;



                ObjConvertImageInfo oci = (ObjConvertImageInfo)selectedImage.Tag;
                oci.rotationAngle = ((RotateTransform)selectedImage.RenderTransform).Angle;
                selectedImage.Tag = oci; //변한 회전값 입력해줌.
                server.Send(fillObjectInfo(selectedImage, WpfUnityCommand.ROTATION));



                //MessageBox.Show(rotateTransform.Angle.ToString());
                //MessageBox.Show(((RotateTransform)obj_image.RenderTransform).Angle.ToString());

            }
        }


        AdornerLayer aLayer; //보더레이어
        //보더 그리기
        public void DrawControlBorder(Image border)
        {

            Image img = border as Image;
            aLayer = AdornerLayer.GetAdornerLayer(img);
            aLayer.Add(new ResizingAdorner(img));

            // img.

            // new Point(img.)
        }


        public void DeleteControlBorder(Image border)
        {
            if (border != null)
            {
                //Image img = border as Image;
                // Remove the adorner from the selected element
                if (border is Image img)
                    aLayer.Remove(aLayer.GetAdorners(img)[0]);


            }
        }




        //사각형 보더 그리기
        public void DrawControlRectangleBorder(Rectangle border)
        {

            Rectangle rc = border as Rectangle;
            aLayer = AdornerLayer.GetAdornerLayer(rc);
            aLayer.Add(new ResizingAdorner(rc));

            // img.

            // new Point(img.)
        }
        //사각형 보더 지우기
        public void DeleteControlRectangleBorder(Rectangle border)
        {
            if (border != null)
            {
                //Image img = border as Image;
                // Remove the adorner from the selected element
                if (border is Rectangle rc)
                    aLayer.Remove(aLayer.GetAdorners(rc)[0]);


            }
        }


        /// <summary>
        /// 드레그를 시작한 마우스 좌표;
        /// </summary>
        Point prePosition;
        /// <summary>
        /// 현재 그려지는 사각형
        /// </summary>
        Rectangle currentRect;
        Rectangle selectedRectangle;
        //812
        private void MakeFloor_ClickDOWN(object sender, MouseButtonEventArgs e)
        {

            if (e.OriginalSource.GetType() == typeof(MapCanvas) || e.OriginalSource.GetType() == typeof(Image) || e.OriginalSource.GetType() == typeof(Line))
            {
                if (selectedRectangle != null) //맵캔버스, 이미지, 라인 등을 오른쪽 클릭했을 시
                {


                    //814
                    DeleteControlRectangleBorder(selectedRectangle);
                    server.Send(fillFloorInfo(selectedRectangle, WpfUnityCommand.DESELECT));
                    selectedRectangle = null;




                    //814 바닥 선택했을 때 선이랑 이미지 선택 해제
                    if (selectedImage != null)
                    {
                        DeleteControlBorder(selectedImage);
                        selectedImage = null;
                    }
                    else if (selectedLine != null)
                    {

                        ////815
                        //if (lineBrush != null) //라인브러시 설정했으면 설정한 색대로
                        //{
                        //    selectedLine.Stroke = lineBrush;
                        //}
                        //else
                        //{
                        //    selectedLine.Stroke = Brushes.Black;
                        //}


                        //817
                        if (ib != null)
                        {
                            //823 선은 자기가 가지고 있는 브러시를 기억하고 그 색으로 다시 돌아옴.
                            WallConvertInfo wci = selectedLine.Tag as WallConvertInfo;
                            selectedLine.Stroke = wci.WallLineimgBrush;
                            // selectedLine.Stroke = ib;
                        }
                        else
                        {
                            selectedLine.Stroke = Brushes.Black;
                            selectedLine.StrokeDashArray = DoubleCollection.Parse("1, 0.1");
                        }

                        // selectedLine.Stroke = Brushes.Black;
                        selectedLine = null;
                    }


                }


            }


            if (Flag_isCheckedFloor && !bLine_check) //선그리기 해제 && 바닥생성 토글 버튼이 체크되어 있을 때만 오른쪽 클릭으로 발생.
            {


                //마우스의 좌표를 저장한다.
                prePosition = e.GetPosition(mapCanvas);

                remainderToFindVertex(prePosition.X, prePosition.Y); //마우스 오른쪽 클릭에서 가장 가까운 꼭지점을 찾고 튜플로 저장한다.
                prePosition.X = Result_StartPoint.Item1; //찾아온 가까운 지점을 클릭 시작점으로 삼는다.
                prePosition.Y = Result_StartPoint.Item2;

                //마우스가 Grid밖으로 나가도 위치를 알 수 있도록 마우스 이벤트를 캡처한다.
                this.mapCanvas.CaptureMouse();
                if (currentRect == null)
                {
                    isFloorClicked = true;
                    //사각형을 생성한다.
                    CreteRectangle();

                }
            }

            if (Flag_isCheckedFloor == false) //이동전에 마우스클릭했을 때?
            {
                if (e.OriginalSource.GetType() == typeof(Rectangle) && !bLine_check) //817 선그리기 체그해제일 때
                {
                    //814
                    if (selectedRectangle != null)
                    {
                        DeleteControlRectangleBorder(selectedRectangle);
                        server.Send(fillFloorInfo(selectedRectangle, WpfUnityCommand.DESELECT));
                        selectedRectangle = null;
                    }

                    selectedRectangle = e.OriginalSource as Rectangle;
                    Point prePosition = e.GetPosition(mapCanvas);
                    server.Send(fillFloorInfo(selectedRectangle, WpfUnityCommand.SELECT));


                    //814 바닥 보더 생성
                    DrawControlRectangleBorder(selectedRectangle);

                    Flag_ImageOneClick = true; //818 여기서 true해줘야 무브에서 움직일 때 통과함




                    //814 바닥 선택했을 때 선이랑 이미지 선택 해제
                    if (selectedImage != null)
                    {
                        DeleteControlBorder(selectedImage);
                        selectedImage = null;
                    }
                    else if (selectedLine != null)
                    {

                        ////815
                        //if (lineBrush != null) //라인브러시 설정했으면 설정한 색대로
                        //{
                        //    selectedLine.Stroke = lineBrush;
                        //}
                        //else
                        //{
                        //    selectedLine.Stroke = Brushes.Black;
                        //}

                        //817
                        if (ib != null)
                        {
                            selectedLine.Stroke = ib;
                        }
                        else if (lineBrush != null) //라인브러시 설정했으면 설정한 색대로
                        {
                            selectedLine.Stroke = lineBrush;
                        }
                        else
                        {
                            selectedLine.Stroke = Brushes.Black;
                            selectedLine.StrokeDashArray = DoubleCollection.Parse("1, 0.1");
                        }

                        // selectedLine.Stroke = Brushes.Black;
                    }


                }
            }
        }

        bool isFloorClicked; //바닥을 생성하기 위해서 클릭을 했냐?

        private void MakeFloor_ClickUP(object sender, MouseButtonEventArgs e)
        {



            if (Flag_isCheckedFloor && isFloorClicked)
            {
                //마우스 캡춰를 제거한다.
                mapCanvas.ReleaseMouseCapture();
                Point upPoint = e.GetPosition(mapCanvas);
                remainderToFindVertex(upPoint.X, upPoint.Y);
                // Canvas.SetLeft(currentRect, Result_StartPoint.Item1);
                // Canvas.SetTop(currentRect, Result_StartPoint.Item2);

                currentRect.Width = Math.Abs(prePosition.X - Result_StartPoint.Item1);
                currentRect.Height = Math.Abs(prePosition.Y - Result_StartPoint.Item2);
                currentRect.Name = $"Object{innerObjName++}";

                if (currentRect.Width < 10 && currentRect.Height < 10)
                {
                    mapCanvas.Children.Remove(currentRect);
                }
                else
                {

                    SetRectangleProperty();
                    server.Send(fillFloorInfo(currentRect, WpfUnityCommand.CREATE));
                }

                currentRect = null;
                isFloorClicked = false;


                ////사각형의 크기를 설정한다. 음수가 나올 수 없으므로 절대값을 취해준다.
                //currentRect.Width = Math.Abs(prePosition.X - currnetPosition.X);
                //currentRect.Height = Math.Abs(prePosition.Y - currnetPosition.Y);

                //Canvas.SetLeft(currentRect, prePosition.X);
                //Canvas.SetTop(currentRect, prePosition.Y);


            }
            // 이동
            else //바닥 이동 끝 이벤트
            {

                if (e.OriginalSource.GetType() == typeof(Rectangle) && Flag_imgMove) //호출한 e가 바닥(랙탱글)일 때
                {
                    //812
                    if (selectedRectangle != null)
                    {
                        Point upPoint = e.GetPosition(mapCanvas);
                        double Rc_LeftPoint = upPoint.X - (selectedRectangle.ActualWidth / 2); //선택된 사각형의 왼쪽 모서리 점
                        double Rc_TopPoint = upPoint.Y - (selectedRectangle.ActualHeight / 2);

                        remainderToFindVertex(Rc_LeftPoint, Rc_TopPoint);
                        Canvas.SetLeft(selectedRectangle, Result_StartPoint.Item1);
                        Canvas.SetTop(selectedRectangle, Result_StartPoint.Item2);

                        //  SelectedRectangle = null;           


                        server.Send(fillFloorInfo(selectedRectangle, WpfUnityCommand.MOVE));

                        Flag_ImageOneClick = false; //818
                        Flag_imgMove = false; //818    

                    }
                }
            }

        }




        private void SetRectangleProperty()
        {
            //사각형의 투명도를 100% 로 설정
            currentRect.Opacity = 0.6;
            //사각형의 색상을 지정

            //820
            if (Floorib != null)
            {

                currentRect.Fill = Floorib; //사용자가 선택한 바닥 이미지브러시 적용   

                //823 바닥이 생성될 때 마다 자신이 가진 이미지 브러시를 기억하고 있음.
                FloorConvertInfo fci = new FloorConvertInfo();
                fci.RectimgBrush = Floorib;

                //824               
                fci.AssetBundleName = Floor_imgFileName;


                currentRect.Tag = fci;

            }
            else
            {
                FloorConvertInfo fci = new FloorConvertInfo();
                fci.AssetBundleName = "default";
                currentRect.Tag = fci;
                currentRect.Fill = new SolidColorBrush(Colors.LightYellow); //사용자가 바닥을 따로 선택하지 않았을 땐 기본 바닥 적용.

            }
            //사각형의 테두리를 선으로 지정
            currentRect.StrokeDashArray = new DoubleCollection();
        }

        private void CreteRectangle()
        {

            currentRect = new Rectangle();
            currentRect.Stroke = new SolidColorBrush(Colors.DarkGreen);
            currentRect.StrokeThickness = 2;
            currentRect.Opacity = 0.7;
            Canvas.SetZIndex(currentRect, 0);

            //사각형을 그리는 동안은 테두리를 Dash 스타일로 설정한다.
            DoubleCollection dashSize = new DoubleCollection();
            dashSize.Add(1);
            dashSize.Add(1);
            currentRect.StrokeDashArray = dashSize;
            //사각형의 정렬 기준을 설정한다.
            currentRect.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            currentRect.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            //그리드에 추가한다.
            mapCanvas.Children.Add(currentRect);
        }



        private void btn_CreateListWindow_Click(object sender, RoutedEventArgs e)
        {



            if (listWindow == null)
            {


                listWindow = new DockingWindows.ListWindow();
                listWindow.call_mainwindow = this; // 자식폼에서 부모폼을 등록??? 자식에서 부모에게 갑 전달 실험용.
                listWindow.DockManager = ExpenderDockManager;// ListDockManager;            
                                                             // listWindow.Show(Dock.Top);
                listWindow.ShowAsDocument();
            }

            listWindow.ShowAsDocument();



        }

        private void btn_CreateObjectWindow_Click(object sender, RoutedEventArgs e)
        {



            if (listWindow == null)
            {

                objWindowDocking = new DockingWindows.ObjectWindow();
                objWindowDocking.DockManager = ExpenderDockManager;
                objWindowDocking.ShowAsDocument();
            }

            objWindowDocking.ShowAsDocument();



        }


        public void ChangeLineColor(Color SendColor)
        {

            //815

            //string a = "FF";
            //string r = "00";

            //Color cr = new Color();
            //cr.A = Convert.ToByte(a);
            //cr.R = Convert.ToByte(r); 
            //cr.G = Convert.ToByte(a); 
            //cr.B = Convert.ToByte(a);

            lineBrush = new SolidColorBrush(SendColor);




        }

        public void ResetBrush()
        {
            if (lineBrush != null)
            {

                lineBrush = null;

            }



        }


        //818
        private void btn_New_Click(object sender, RoutedEventArgs e)
        {
            //831            

            if (MessageBox.Show("정말 초기화하시겠습니까?", "알림", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                AllClear();
                server.Send(fillCommandInfo(WpfUnityCommand.ALLCLEAR));

            }
            else
            {

            }

        }

        private void AllClear()
        {
            mapCanvas.Children.Clear();




            bLine_check = false;
            linePlusMinus_Check = false;
            Flag_isCheckedFloor = false;
            Ch_line.IsChecked = false;
            Ch_linePlusMinus.IsChecked = false;
            Toggle_Floor.IsChecked = false;

            selectedImage = null;
            line = null;
            selectedLine = null;
            ib = null;
            lineBrush = null;
            selectedRectangle = null;

            imageClick = false;
            bMouseDown = false;

            stPoint = new Point(0, 0);
            curPoint = new Point(0, 0);
            Result_StartPoint = null;
            txtBox.Text = null;

            prePosition = new Point(0, 0);
            currentRect = null;


            btn_clickWidthCount = 0;
            btn_clickHeightCount = 0;
            checkCanvasWidthCount();
            btn_PlusWidthHeight.IsChecked = false;

            mapCanvas.Width = initActualWidth; //맥시마이즈 시작 초기값을 넣어준다.
            mapCanvas.Height = initActualHeight;



            listWindow.DeleteAllClear(); //트리뷰의 모든 내용을 삭제 및 초기화

            colorWindow.WallpaperThumnailClear();
            // colorWindow.ColorSelectTextClear();
            colorWindow.HistroyWallpaperThumClear();
            Wallpaper_imgFilePath = null;

            colorWindow.FloorThumnailClear();
            colorWindow.HistroyFloorThumClear();
            Floor_imgFilePath = null;
            //innerObjName 
        }


        private void btn_Help_Click(object sender, RoutedEventArgs e)
        {

            //MetroWindow helpwindow = new MetroWindow();
            //helpwindow.Show();

            HelpWindow helpWindow = new HelpWindow();
            //  mainWindow.AddChild(helpWindow);
            helpWindow.PopUp();




        }

        private void btnCreateColorWindow_Click(object sender, RoutedEventArgs e)
        {

            if (colorWindow == null)
            {
                colorWindow = new DockingWindows.ColorWindow();
                colorWindow.Call_MainWindow = this; // 자식폼에서 부모폼을 등록??? 자식에서 부모에게 갑 전달 실험용.
                colorWindow.DockManager = ExpenderDockManager;// ListDockManager;            
                                                              // listWindow.Show(Dock.Top);
                colorWindow.ShowAsDocument();
            }

            colorWindow.ShowAsDocument();

            //else
            //{
            //    if (colorWindow.Visibility == Visibility.Collapsed)
            //    {
            //        // MessageBox.Show("접힘");
            //        colorWindow = null;
            //        colorWindow = new DockingWindows.ColorWindow();
            //        colorWindow.Call_MainWindow = this;
            //        colorWindow.DockManager = ExpenderDockManager;
            //        colorWindow.ShowAsDocument();
            //        //   colorWindow.ChangeDock(Dock.Top);
            //        // colorWindow.Visibility = Visibility.Visible;
            //    }
            //}


        }



        //818
        public void drawCircle()
        {

            Ellipse circle = new Ellipse() { Width = 8, Height = 8, Fill = Brushes.Blue };

            mapCanvas.Children.Add(circle);
            circle.ToolTip = "3D뷰어 중심점";

            Canvas.SetLeft(circle, 396);
            Canvas.SetTop(circle, 396);
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








    


       //====================================== 김태우 작성 ==========================================//
       // 데이터 받는 부분
       // 저장 및 불러오기
       // 불러온데이터 셋팅

        // 유니티에서 데이터가 오면 받아서 처리하는 부분
        private void OnReceviedCompleted(WpfUnityPacketHeader header)
        {
            Dispatcher.Invoke(() => processReceivedPacket(header));
        }

        private void processReceivedPacket(WpfUnityPacketHeader header)
        {
            switch (header.PacketType)
            {
                case WpfUnityPacketType.WallInfoPacket:
                    processReceivedWallInfo(header as WallInfoPacket);
                    break;

                case WpfUnityPacketType.ObjectInfoPacket:
                    processReceivedObjectInfo(header as ObjectInfoPacket);
                    break;

                case WpfUnityPacketType.FloorInfoPacket:
                    processReceivedFloorInfo(header as FloorInfoPacket);
                    break;
            }
        }

        private void processReceivedFloorInfo(FloorInfoPacket floorInfoPk)
        {

            switch (floorInfoPk.Command)
            {
                case WpfUnityCommand.MOVE3D:

                    var pos = convert3DFloorPosTo2DRect(floorInfoPk);
                    Canvas.SetLeft(selectedRectangle, pos[0]);
                    Canvas.SetTop(selectedRectangle, pos[1]);
                    break;

                case WpfUnityCommand.REMOVE3D:
                    DeleteRectangle();
                    break;

                case WpfUnityCommand.SELECT3D:
                    Rectangle toSelect = findUIElementByName(floorInfoPk.Name) as Rectangle;

                    if (toSelect != null)
                    {
                        selectedRectangle = toSelect;
                        DrawControlRectangleBorder(selectedRectangle);
                    }
                    break;

                case WpfUnityCommand.DESELECT3D:
                    DeleteControlRectangleBorder(selectedRectangle);
                    selectedRectangle = null;
                    break;


            }
        }

        private void processReceivedObjectInfo(ObjectInfoPacket objInfoPk)
        {
            switch (objInfoPk.Command)
            {
                case WpfUnityCommand.REMOVE3D:
                    DeleteObject();
                    selectedImage = null;
                    break;


                case WpfUnityCommand.MOVE3D:
                    List<float> pos = convert3DObjectPosTo2DImage(objInfoPk);
                    Canvas.SetLeft(selectedImage, pos[0] - selectedImage.ActualWidth / 2);
                    Canvas.SetTop(selectedImage, pos[1] - selectedImage.ActualHeight / 2);
                    break;


                case WpfUnityCommand.SELECT3D:
                    Image toSelect = findUIElementByName(objInfoPk.Name) as Image;

                    if (toSelect != null)
                    {
                        selectedImage = toSelect;
                        DrawControlBorder(selectedImage);
                    }
                    break;



                case WpfUnityCommand.DESELECT3D:
                    if (selectedImage != null)
                    {
                        DeleteControlBorder(selectedImage);
                        selectedImage = null;
                    }
                    break;
            }
        }

        private void processReceivedWallInfo(WallInfoPacket wallInfo)
        {
            switch (wallInfo.Command)
            {
                case WpfUnityCommand.MOVE3D:
                    List<int> pos = convert3DWallPosTo2DLine(wallInfo);
                    if (selectedLine != null)
                    {
                        selectedLine.X1 = pos[2];
                        selectedLine.Y1 = pos[3];
                        selectedLine.X2 = pos[0];
                        selectedLine.Y2 = pos[1];
                    }
                    break;

                case WpfUnityCommand.REMOVE3D:

                    if (selectedLine != null)
                    {
                        mapCanvas.Children.Remove(selectedLine);
                    }

                    break;


                case WpfUnityCommand.SELECT3D:

                    UIElement toSelect = findUIElementByName(wallInfo.Name);
                    if (toSelect != null)
                    {
                        selectedLine = toSelect as Line;
                        selectedLine.Stroke = Brushes.Red;
                    }
                    break;

                case WpfUnityCommand.DESELECT3D:

                    if (selectedLine != null)
                    {

                        WallConvertInfo wci = selectedLine.Tag as WallConvertInfo;

                        if (wci.WallLineimgBrush != null)
                        {
                            selectedLine.Stroke = wci.WallLineimgBrush; // 선
                                                                        // selectedLine.Stroke = ib;
                        }
                        else
                        {
                            selectedLine.Stroke = Brushes.Black;
                            selectedLine.StrokeDashArray = DoubleCollection.Parse("1, 0.1");
                            selectedLine = null;
                        }
                    }

                    selectedLine = null;
                    break;
            }
        }


        UIElement findUIElementByName(string name)
        {
            UIElement uie = null;
            try
            {
                uie = mapCanvas.Children.OfType<UIElement>().First<UIElement>(e => e.GetValue(FrameworkElement.NameProperty).ToString() == name);
            }
            catch (InvalidOperationException)
            { }

            return uie;
        }






        private List<int> convert3DFloorPosTo2DRect(FloorInfoPacket floorInfo)
        {

            int w = (int)(floorInfo.ScaleX * 40);
            int h = (int)(floorInfo.ScaleZ * 40);
            int left = (int)((floorInfo.PosX * 40 + zeroPos) - w / 2f);
            int top = (int)((floorInfo.PosZ * 40 * -1 + zeroPos) - h / 2f);
            return new List<int> { left, top, w, h };

        }

        private List<int> convert3DWallPosTo2DLine(WallInfoPacket wallInfo)
        {
            // 2D 상의 중심점을 구한다.
            int xc = (int)(wallInfo.PosX * 40 + zeroPos);
            int yc = (int)(wallInfo.PosZ * 40 * -1 + zeroPos);
            int w, h;

            // 2D 상의 길이를 구한다.
            // float이기 때문에 유니티상에서 정확하게 소수점 1자리로 안떨어지고 2.6이면 2.5xxxxx 로 나온다.
            // 따라서 반올림을 해줘서 값을 보정한다.
            if (wallInfo.ScaleX > wallInfo.ScaleZ)
            {
                w = (int)((wallInfo.ScaleX - 0.2f) * 40);
                h = 0;
            }
            else
            {
                w = 0;
                h = (int)((wallInfo.ScaleZ - 0.2f) * 40);
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


        private List<float> convert3DObjectPosTo2DImage(ObjectInfoPacket objInofPk)
        {


            float xc = objInofPk.PosX * 40 + zeroPos;
            float yc = objInofPk.PosZ * 40 * -1 + zeroPos;
            List<float> pos = new List<float>();
            pos.Add(xc);
            pos.Add(yc);

            return pos;
        }




        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {


            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "NCVSF(*.NCVSF)|*.NCVSF";


            sfd.AddExtension = true;

            string dir = Directory.GetCurrentDirectory();

            sfd.InitialDirectory = dir; //대화상자 시작시 초기 디렉토리 지정
            sfd.Title = "저장 하기";


            bool? result = sfd.ShowDialog();
            if (result == true) //파일 저장 실해
            {
                JsonTypeDesignDataControler controler = new JsonTypeDesignDataControler();
                controler.GetCanvasData(mapCanvas);
                controler.Save(sfd.FileName);
                MessageBox.Show("성공적으로 저장하였습니다.");
            }


        }

        private void btn_Load_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "NCVSF(*.NCVSF)|*.NCVSF";
            ofd.Multiselect = false;

            ofd.AddExtension = true;

            string dir = Directory.GetCurrentDirectory();

            ofd.InitialDirectory = dir; //대화상자 시작시 초기 디렉토리 지정
            ofd.Title = "불러 오기";


            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                AllClear();
                server.Send(fillCommandInfo(WpfUnityCommand.ALLCLEAR));
                JsonTypeDesignDataControler controler = new JsonTypeDesignDataControler();
                controler.Load(ofd.FileName);
                SetLoadedDataIn2DAnd3D(mapCanvas, controler.data);
                MessageBox.Show("성공적으로 불러왔습니다.");
            }
        }



        public void SetLoadedDataIn2DAnd3D(Canvas canvaes, JsonTypeDesignData data)
        {

            List<ImageBrush> imageBrushes = new List<ImageBrush>();
            foreach (var id in data.imageData)
            {

                imageBrushes.Add(GetBitmapBrushByBinary(id.ImageBinary.ToArray()));
            }

            foreach (var ld in data.lineData)
            {


                Line line = new Line()
                {
                    X1 = ld.x1,
                    Y1 = ld.y1,
                    X2 = ld.x2,
                    Y2 = ld.y2,
                    Name = $"Object_{i++}",
                    StrokeThickness = 6,
                    Stroke = System.Windows.Media.Brushes.Black
                };


                WallConvertInfo wallConvertInfo = new WallConvertInfo();
                wallConvertInfo.AssetBundleName = ld.WallImageName;

                Canvas.SetZIndex(line, 1);


                if (ld.ImageIndex >= 0)
                {
                    wallConvertInfo.WallLineimgBrush = imageBrushes[ld.ImageIndex];
                    line.Stroke = imageBrushes[ld.ImageIndex];

                }

                line.Tag = wallConvertInfo;
                canvaes.Children.Add(line);
            }

            foreach (var fd in data.FloorData)
            {
                Rectangle rect = new Rectangle()
                {
                    Width = fd.Width,
                    Height = fd.Height,
                    Stroke = System.Windows.Media.Brushes.Green,
                    Name = $"Object_{i++}",
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightYellow), //사용자가 바닥을 따로 선택하지 않았을 땐 기본 바닥 적용.
                    Opacity = 0.6,
                };

                FloorConvertInfo floorConvertInfo = new FloorConvertInfo();
                floorConvertInfo.AssetBundleName = fd.FloorImageName;
                rect.Stroke = new SolidColorBrush(Colors.DarkGreen);
                Canvas.SetZIndex(rect, 0);

                // fd.ImageIndex가 0이상이면 커스텀 바닥을 적용한다.
                if (fd.ImageIndex >= 0)
                {
                    rect.Fill = imageBrushes[fd.ImageIndex];
                    floorConvertInfo.RectimgBrush = imageBrushes[fd.ImageIndex];
                }

                rect.Tag = floorConvertInfo;
                Canvas.SetTop(rect, fd.Top);
                Canvas.SetLeft(rect, fd.Left);
                canvaes.Children.Add(rect);
            }

            foreach (var imgData in data.objectData)
            {
                Image img = new Image()
                {
                    Name = $"Object_{i++}",

                };

                ObjConvertImageInfo objconverInfo = new ObjConvertImageInfo();
                objconverInfo.AssetBundleName = imgData.ObjectName;
                objconverInfo.brand = imgData.brand;
                objconverInfo.explain = imgData.explain;
                objconverInfo.ObjectName = img.Name;
                objconverInfo.ObjectType = imgData.ObjectType;
                objconverInfo.price = imgData.price;
                objconverInfo.rotationAngle = imgData.rotationAngle;
                objconverInfo.VisualName = imgData.VisualName;
                objconverInfo.obj_ConvertSize = (ObjSize)imgData.obj_ConvertSize;
                img.Tag = objconverInfo;
                Canvas.SetZIndex(img, 3);

#if DEBUG
                string convertImgPath = AppDomain.CurrentDomain.BaseDirectory
                    + $"../../../pictures/{objconverInfo.ObjectType}/convert/{objconverInfo.obj_ConvertSize.ToString()}.png";

                string imageFilePath = AppDomain.CurrentDomain.BaseDirectory
                    + $"../../../pictures/{objconverInfo.ObjectType}/{objconverInfo.AssetBundleName}.png";

#else
                            string convertImgPath = AppDomain.CurrentDomain.BaseDirectory + 
                $"pictures/{objconverInfo.ObjectType}/convert/{objconverInfo.obj_ConvertSize.ToString()}.png";

                
                string imageFilePath = AppDomain.CurrentDomain.BaseDirectory
                    + $"pictures/{objconverInfo.ObjectType}/{objconverInfo.AssetBundleName}.png";
#endif

                objconverInfo.ImgFilePath = imageFilePath;



                BitmapImage convertbitmap = new BitmapImage();
                convertbitmap.BeginInit();
                convertbitmap.UriSource = new Uri(convertImgPath, UriKind.RelativeOrAbsolute);
                convertbitmap.EndInit();


                img.Stretch = Stretch.Uniform;
                img.Source = convertbitmap;
                img.MouseEnter += Convertimg_MouseEnter;
                img.MouseLeave += Convertimg_MouseLeave;


                //---회전 811                      
                rotateTransform = new RotateTransform();
                img.RenderTransform = rotateTransform;
                img.RenderTransformOrigin = new Point(0.5, 0.5); //이미지 가운데 지점?

                //812
                Canvas.SetZIndex(img, 3);
                Canvas.SetTop(img, imgData.Top);
                Canvas.SetLeft(img, imgData.Left);

                ((RotateTransform)img.RenderTransform).Angle = objconverInfo.rotationAngle;
                AppendToChildList(objconverInfo); //리스트쪽으로 컨버트된 이미지 태그에 등록한 클래스 정보들을 자식의 함수를 통해 넘겨줌.
                ObjConvertimgList.Add(objconverInfo); //왼쪽 UI에서 지울 수 있도록 이미지의 tag에 넣었던 정보를 리스트 안에도 저장. 

                canvaes.Children.Add(img);
            }

            // 로드한 데이터를 3D View에 나타내기 위해 데이터를 전송한다.
            foreach (var item in canvaes.Children)
            {
                switch (item)
                {
                    case Line line:
                        server.Send(fillWallInfo(line, WpfUnityCommand.CREATE));
                        break;

                    case Image image:
                        server.Send(fillObjectInfo(image, WpfUnityCommand.CREATE));
                        break;

                    case Rectangle rect:
                        server.Send(fillFloorInfo(rect, WpfUnityCommand.CREATE));
                        break;
                }
            }
        }




        //경로 넣어서 리턴값이 이미지브러시인 함수
        public ImageBrush GetBitmapBrushByPath(string Param_imgFilePath)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(Param_imgFilePath); //파일 경로
            bitmap.EndInit();

            ImageBrush imgBrush = new ImageBrush();
            imgBrush.ImageSource = bitmap; //이미지소스에 비트맵 넣으면 됨.
            return imgBrush;
        }


        // 바이너리 집어넣어서 리턴값이 이미지 브러시인 함수
        public ImageBrush GetBitmapBrushByBinary(byte[] binary)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(binary);
            bitmap.EndInit();

            ImageBrush imgBrush = new ImageBrush();
            imgBrush.ImageSource = bitmap; //이미지소스에 비트맵 넣으면 됨.
            return imgBrush;
        }

    }
}