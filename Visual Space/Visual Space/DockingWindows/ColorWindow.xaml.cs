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
using DockingLibrary;
using MahApps.Metro;
using System.Reflection;

using Nollan.Visual_Space.classes;
using System.Windows.Forms;
using System.IO;
using Xceed.Wpf.Toolkit;

namespace Nollan.Visual_Space.DockingWindows
{
    /// <summary>
    /// ColorWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ColorWindow : DockingLibrary.DockableContent
    {
        public ColorWindow()
        {
            InitializeComponent();

            bgChange();
           
            //  ColorReflection();



        }

     

        //902
        string bg_RootFilePath = @"\Resources\bg3.png";
        string bg_FloorFilePath = @"\Resources\bg2.png";       
        private void bgChange()
        {

            grid_root.Background = GetBrushFromPath(bg_RootFilePath);
            grid_Floor.Background = GetBrushFromPath(bg_FloorFilePath);            

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



        public MainWindow Call_MainWindow;
        public int SelectedViewIdx;



        //  public void ColorReflection()
        //  {

        //      IEnumerable<PropertyInfo> properties = typeof(Colors).GetTypeInfo().DeclaredProperties; //리플렉션

        //      foreach (PropertyInfo property in properties)
        //      {
        //          Color clr = (Color)property.GetValue(null);
        //          ColorReflectionwithWPF.ColorItem clrItem = new ColorReflectionwithWPF.ColorItem(property.Name, clr);

        //          classes.colorTag ct = new classes.colorTag();
        //          ct.color = clr; //컬러 정보를 컬러태그 클래스 안에 삽입
        //          ct.colorName = property.Name;

        //          clrItem.Tag = ct;


        //     //     _colorListView.Items.Add(clrItem);
        //      }

        //  }        
        //  public void ColorFinder(object sender, MouseButtonEventArgs e)
        //  {

        //      System.Windows.Controls.ListView lv = sender as System.Windows.Controls.ListView;
        //      // lv.Items.Count;

        //      int colorCount = 0;
        //      ColorReflectionwithWPF.ColorItem SelectedColor = null;
        //      string LineName = null;
        //      foreach (ColorReflectionwithWPF.ColorItem item in lv.Items)
        //      {

        //          if (SelectedViewIdx == colorCount)
        //          {
        //              SelectedColor = item;
        //              LineName = "#" + (SelectedColor.Name).Substring(1); // _ 붙은 거 서브스트링으로 지워줌.


        //              // MessageBox.Show(LineName);
        //              break;
        //          }
        //          colorCount++;
        //      }
        //  //    tbx_ColorName.Text = LineName;
        //      colorTag ct = SelectedColor.Tag as colorTag;
        //      SendColor = ct.color;


        //      Call_MainWindow.ChangeLineColor(SendColor);

        //  }
        //  public Color SendColor;
        //  public void btn_Apply_Click(object sender, RoutedEventArgs e)
        //  {
        //      Call_MainWindow.ChangeLineColor(SendColor);

        //  }
        //  private void _colorListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //  {

        ////      SelectedViewIdx = _colorListView.SelectedIndex; //리스트뷰 안에서 자체적으로 선택된 인덱스 값을 가지고 있음.
        //                                                      // MessageBox.Show((SelectedViewIdx).ToString());
        //      ColorFinder(sender, e);
        //  }


        public void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            Call_MainWindow.ResetBrush();
            ColorSelectTextClear();


        }
        public void ColorSelectTextClear()
        {
            //  tbx_ColorName.Text = null;

        }




        string tail = " 을 선택 중 입니다.";
        //817
        private void btn_FileDialog_Click(object sender, RoutedEventArgs e) //일반벽지 선택
        {


            OpenFileDialog ofd = new OpenFileDialog(); //파일 열기 생성
            ofd.Multiselect = false; //중복 선택 금지
            ofd.Filter = "PNG(*.PNG)|*.PNG"; //파일 유형 선택 정의. PNG로 일단 고정
                                             // "엑셀 파일 (*.xls)|*.xls|엑셀 파일 (*.xlsx)|*.xlsx";
                                             //   ofd.RestoreDirectory = false;
            ofd.AddExtension = true;            
            ofd.Title = "벽지 선택";






#if DEBUG

            string dir = Directory.GetCurrentDirectory();
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir + @"\pictures\wallpapers";
            ofd.InitialDirectory = dir;

#else
            string dir = Directory.GetCurrentDirectory();
            dir = dir + @"\pictures\wallpapers";
            ofd.InitialDirectory = dir;
#endif

                   


            //경로, 

            DialogResult result = ofd.ShowDialog();
            if (result.ToString() == "OK") //파일 열기 실행
            {
                string Wallpaper_imgFilePath = ofd.FileName; //파일 경로
                int index = Wallpaper_imgFilePath.LastIndexOf("\\");
                string imgFileName = Wallpaper_imgFilePath.Substring(index + 1);
                tbk_FilePath.Text = imgFileName + tail; // " 을 선택 중 입니다.";



                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(Wallpaper_imgFilePath);
                bitmap.EndInit();



                BitmapInfo bmi = new BitmapInfo();
                bmi.SaveBitmap = bitmap;
                bmi.BitmapName = imgFileName; //파일명(.png까지)
                bmi.BitmapFilePath = Wallpaper_imgFilePath; //이미지 파일 경로
                histroyClickSendParam = Wallpaper_imgFilePath; // 히스토리 버튼 눌렀을 때 텍스트창에 띄워주고, 메인창에 값보내기 위한 변수 
                historyClickSendName = imgFileName; // 히스토리 버튼 눌렀을 때 텍스트창에 띄워주고, 메인창에 값보내기 위한 변수


                isCustomWall = false;
                SendImgBrush(bitmap, Wallpaper_imgFilePath, imgFileName, isCustomWall); //메인윈도우의 이미지브러시 변경


                img_UserSelectedFile.Source = bitmap; //벽지 선택 왼쪽의 네모 하나 이미지 변경.


                //818 랙탱글 보더 색 바꾸기
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = bitmap;
                rc_border.BorderBrush = imgBrush;


                stackimgInput(bmi);


                //823
                CustomThumnailClear(); //커스텀 벽지 섬네일쪽 선택되어 있으면 섬네일 해제.

            }
        }



        string histroyClickSendParam; //선택한 파일 경로 기억
        string historyClickSendName; //선택한 파일 이름 기억

        public class BitmapInfo
        {
            public BitmapImage SaveBitmap;
            public string BitmapName;
            public string BitmapFilePath;
        }
        public List<BitmapInfo> BitmapList = new List<BitmapInfo>(); //선택된 비트인포들을 저장할 리스트



        public List<BitmapInfo> Floor_BitmapList = new List<BitmapInfo>(); //선택된 바닥 비트맵인포를 저장할 리스트

        //  public List<BitmapImage> BitmapList = new List<BitmapImage>(); 

        public int historyCnt = 0;
        //벽지 선택 완료했을 시 히스토리에 하나씩 등록하게 됨. 3개가 넘어가면 맨 처음 거 하나 지우고 두번째 거를 3번째로 옮기고 세번
        public void stackimgInput(BitmapInfo bmi)
        {

            BitmapList.Add(bmi);

            switch (BitmapList.Count)
            {
                case 1:
                    img_ViewBox1.Source = BitmapList[0].SaveBitmap;
                    break;

                case 2:
                    img_ViewBox1.Source = BitmapList[1].SaveBitmap;
                    img_ViewBox2.Source = BitmapList[0].SaveBitmap;
                    break;

                case 3:
                    img_ViewBox1.Source = BitmapList[2].SaveBitmap;
                    img_ViewBox2.Source = BitmapList[1].SaveBitmap;
                    img_ViewBox3.Source = BitmapList[0].SaveBitmap;
                    break;

                case 4:
                    BitmapList.Remove(BitmapList[0]);
                    img_ViewBox1.Source = BitmapList[2].SaveBitmap;
                    img_ViewBox2.Source = BitmapList[1].SaveBitmap;
                    img_ViewBox3.Source = BitmapList[0].SaveBitmap;
                    break;

            }


        }


        public void SendImgBrush(BitmapImage bitmap, string Wallpaper_imgFilePath, string Wallpaper_imgFileName, bool _isCustomWall)
        {
            Call_MainWindow.ChangeImgBrush(bitmap, Wallpaper_imgFilePath, Wallpaper_imgFileName, _isCustomWall);


            // new ImageBrush( new BitmapImage(new Uri("D:/winshare/mainproject/VisualSpace/Visual Space/Visual Space/icons/tiles/linetile1.png", UriKind.Absolute)  ));
        }
        public void SendDelImgBrush()
        {

            Call_MainWindow.DeleteImgBrush();
        }


        //메인 윈도우쪽으로 벽지 삭제 버튼이 눌렸다고 알려줌.
        public void btn_FileDialogClear_Click(object sender, RoutedEventArgs e)
        {
            SendDelImgBrush();
            WallpaperThumnailClear();


        }
       
        //벽지 섬네일 해제
        public void WallpaperThumnailClear()
        {

            img_UserSelectedFile.Source = null;
            tbk_FilePath.Text = null; //파일 경로 띄우는 텍스트 박스 삭제
            rc_border.BorderBrush = Brushes.Black;

        }

        //새 파일 누를 때 호출됨.
        public void HistroyWallpaperThumClear()
        {
            if (BitmapList.Count != 0)
            {
                img_ViewBox1.Source = null;
                img_ViewBox2.Source = null;
                img_ViewBox3.Source = null;
                BitmapList.Clear();

            }
        }



        private void btn_ViewBox1_Click(object sender, RoutedEventArgs e)
        {
            if (BitmapList.Count == 1)
            {
                SendImgBrush(BitmapList[0].SaveBitmap, histroyClickSendParam, historyClickSendName, isCustomWall);
                img_UserSelectedFile.Source = BitmapList[0].SaveBitmap;

                tbk_FilePath.Text = BitmapList[0].BitmapName + tail;


                //818 랙탱글 보더 색 바꾸기
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = BitmapList[0].SaveBitmap;
                rc_border.BorderBrush = imgBrush;

                
                CustomThumnailClear(); //일반 벽지를 선택했으면 커스텀 벽지 섬네일은 해제
            }

            else if (BitmapList.Count == 2)
            {
                SendImgBrush(BitmapList[1].SaveBitmap, histroyClickSendParam, historyClickSendName, isCustomWall);
                img_UserSelectedFile.Source = BitmapList[1].SaveBitmap;
                tbk_FilePath.Text = BitmapList[1].BitmapName + tail;

                //818 랙탱글 보더 색 바꾸기
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = BitmapList[1].SaveBitmap;
                rc_border.BorderBrush = imgBrush;

               
                CustomThumnailClear(); //일반 벽지를 선택했으면 커스텀 벽지 섬네일은 해제
            }

            else if (BitmapList.Count == 3)
            {
                SendImgBrush(BitmapList[2].SaveBitmap, histroyClickSendParam, historyClickSendName, isCustomWall);
                img_UserSelectedFile.Source = BitmapList[2].SaveBitmap;
                tbk_FilePath.Text = BitmapList[2].BitmapName + tail;

                //818 랙탱글 보더 색 바꾸기
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = BitmapList[2].SaveBitmap;
                rc_border.BorderBrush = imgBrush;

                
                CustomThumnailClear(); //일반 벽지를 선택했으면 커스텀 벽지 섬네일은 해제
            }           

        }

        private void btn_ViewBox2_Click(object sender, RoutedEventArgs e)
        {

            if (BitmapList.Count == 2)
            {
                SendImgBrush(BitmapList[0].SaveBitmap, histroyClickSendParam, historyClickSendName, isCustomWall);
                img_UserSelectedFile.Source = BitmapList[0].SaveBitmap;
                tbk_FilePath.Text = BitmapList[0].BitmapName + tail;

                //818 랙탱글 보더 색 바꾸기
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = BitmapList[0].SaveBitmap;
                rc_border.BorderBrush = imgBrush;

                
                CustomThumnailClear(); //일반 벽지를 선택했으면 커스텀 벽지 섬네일은 해제

            }
            else if (BitmapList.Count == 3)
            {
                SendImgBrush(BitmapList[1].SaveBitmap, histroyClickSendParam, historyClickSendName, isCustomWall);
                img_UserSelectedFile.Source = BitmapList[1].SaveBitmap;
                tbk_FilePath.Text = BitmapList[1].BitmapName + tail;

                //818 랙탱글 보더 색 바꾸기
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = BitmapList[1].SaveBitmap;
                rc_border.BorderBrush = imgBrush;

                
                CustomThumnailClear(); //일반 벽지를 선택했으면 커스텀 벽지 섬네일은 해제

            }


       

        }

        private void btn_ViewBox3_Click(object sender, RoutedEventArgs e)
        {
            if (BitmapList.Count == 3)
            {
                SendImgBrush(BitmapList[0].SaveBitmap, histroyClickSendParam, historyClickSendName, isCustomWall);
                img_UserSelectedFile.Source = BitmapList[0].SaveBitmap;
                tbk_FilePath.Text = BitmapList[0].BitmapName + tail;


                //818 랙탱글 보더 색 바꾸기
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = BitmapList[0].SaveBitmap;
                rc_border.BorderBrush = imgBrush;

                //823
                CustomThumnailClear(); //일반 벽지를 선택했으면 커스텀 벽지 섬네일은 해제
            }

           

        }





        //바닥쪽

        private void btn_FloorViewBox1_Click(object sender, RoutedEventArgs e)
        {

            if (Floor_BitmapList.Count == 1)
            {
                SendFloorImgBrush(Floor_BitmapList[0].SaveBitmap, FloorClickHistoryFilePath, FloorClickHistoryFileName, isCustomFloor);
                img_UserSelectedFloorFile.Source = Floor_BitmapList[0].SaveBitmap;

                tbk_FloorFilePath.Text = Floor_BitmapList[0].BitmapName + tail;

                //823
                CustomFloorThumnailClear(); //커스텀 바닥 섬네일 해제

                // 랙탱글 보더 색 바꾸기
                //  ImageBrush imgBrush = new ImageBrush();
                //  imgBrush.ImageSource = Floor_BitmapList[0].SaveBitmap;
                //  Floor_border.Stroke = imgBrush;
                
               
            }

            else if (Floor_BitmapList.Count == 2)
            {
                SendFloorImgBrush(Floor_BitmapList[1].SaveBitmap, FloorClickHistoryFilePath, FloorClickHistoryFileName, isCustomFloor);
                img_UserSelectedFloorFile.Source = Floor_BitmapList[1].SaveBitmap;
                tbk_FloorFilePath.Text = Floor_BitmapList[1].BitmapName + tail;

             
                CustomFloorThumnailClear(); //커스텀 바닥 섬네일 해제

                // 랙탱글 보더 색 바꾸기
                // ImageBrush imgBrush = new ImageBrush();
                // imgBrush.ImageSource = Floor_BitmapList[1].SaveBitmap;
                // Floor_border.Stroke = imgBrush;
            }

            else if (Floor_BitmapList.Count == 3)
            {
                SendFloorImgBrush(Floor_BitmapList[2].SaveBitmap, FloorClickHistoryFilePath, FloorClickHistoryFileName, isCustomFloor);
                img_UserSelectedFloorFile.Source = Floor_BitmapList[2].SaveBitmap;
                tbk_FloorFilePath.Text = Floor_BitmapList[2].BitmapName + tail;
                
               
                CustomFloorThumnailClear(); //커스텀 바닥 섬네일 해제

                // 랙탱글 보더 색 바꾸기
                //   ImageBrush imgBrush = new ImageBrush();
                //  imgBrush.ImageSource = Floor_BitmapList[2].SaveBitmap;
                //   Floor_border.Stroke = imgBrush;
            }


            

        }

        private void btn_FloorViewBox2_Click(object sender, RoutedEventArgs e)
        {

            if (Floor_BitmapList.Count == 2)
            {
                SendFloorImgBrush(Floor_BitmapList[0].SaveBitmap, FloorClickHistoryFilePath, FloorClickHistoryFileName, isCustomFloor);
                img_UserSelectedFloorFile.Source = Floor_BitmapList[0].SaveBitmap;
                tbk_FloorFilePath.Text = Floor_BitmapList[0].BitmapName + tail;

                
                CustomFloorThumnailClear(); //커스텀 바닥 섬네일 해제

                // 랙탱글 보더 색 바꾸기
                //ImageBrush imgBrush = new ImageBrush();
                //imgBrush.ImageSource = Floor_BitmapList[0].SaveBitmap;
                // Floor_border.Stroke = imgBrush;

            }
            else if (Floor_BitmapList.Count == 3)
            {
                SendFloorImgBrush(Floor_BitmapList[1].SaveBitmap, FloorClickHistoryFilePath, FloorClickHistoryFileName, isCustomFloor);
                img_UserSelectedFloorFile.Source = Floor_BitmapList[1].SaveBitmap;
                tbk_FloorFilePath.Text = Floor_BitmapList[1].BitmapName + tail;

                CustomFloorThumnailClear(); //커스텀 바닥 섬네일 해제

                // 랙탱글 보더 색 바꾸기
                // ImageBrush imgBrush = new ImageBrush();
                //  imgBrush.ImageSource = BitmapList[1].SaveBitmap;
                //  Floor_border.Stroke = imgBrush;

            }

           
        }

        private void btn_FloorViewBox3_Click(object sender, RoutedEventArgs e)
        {
            if (Floor_BitmapList.Count == 3)
            {
                SendFloorImgBrush(Floor_BitmapList[0].SaveBitmap, FloorClickHistoryFilePath, FloorClickHistoryFileName, isCustomFloor);
                img_UserSelectedFloorFile.Source = Floor_BitmapList[0].SaveBitmap;
                tbk_FloorFilePath.Text = Floor_BitmapList[0].BitmapName + tail;
               
                CustomFloorThumnailClear(); //커스텀 바닥 섬네일 해제

                // 랙탱글 보더 색 바꾸기
                // ImageBrush imgBrush = new ImageBrush();
                // imgBrush.ImageSource = Floor_BitmapList[0].SaveBitmap;
                // Floor_border.Stroke = imgBrush;
            }

        }

        public string FloorClickHistoryFilePath;
        public string FloorClickHistoryFileName;
        private void btn_FloorFileDialog_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog(); //파일 열기 생성
            ofd.Multiselect = false; //중복 선택 금지
            ofd.Filter = "PNG(*.PNG)|*.PNG"; //파일 유형 선택 정의. PNG로 일단 고정
                                             // "엑셀 파일 (*.xls)|*.xls|엑셀 파일 (*.xlsx)|*.xlsx";
            ofd.Title = "바닥 선택";
            // AppDomain.CurrentDomain.BaseDirectory + $"../../../pictures/{_ObjectType}/convert/{_ObjectType}.png";


            //   ofd.RestoreDirectory = false;
            ofd.AddExtension = true;


#if DEBUG

            string dir = Directory.GetCurrentDirectory();
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir + @"\pictures\floors";
            ofd.InitialDirectory = dir;

#else
            string dir = Directory.GetCurrentDirectory();
            dir = dir + @"\pictures\floors";
            ofd.InitialDirectory = dir;
#endif
                
            

            DialogResult result = ofd.ShowDialog();
            if (result.ToString() == "OK") //파일 열기 실행
            {
                string Floor_imgFilePath = ofd.FileName; //파일 경로
                int index = Floor_imgFilePath.LastIndexOf("\\");
                string FloorFileName = Floor_imgFilePath.Substring(index + 1);
                tbk_FloorFilePath.Text = FloorFileName + tail; // " 을 선택 중 입니다.";



                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(Floor_imgFilePath);
                bitmap.EndInit();



                BitmapInfo bmi = new BitmapInfo();
                bmi.SaveBitmap = bitmap;
                bmi.BitmapName = FloorFileName; //파일명(.png까지)
                bmi.BitmapFilePath = Floor_imgFilePath; //이미지 파일 경로
                FloorClickHistoryFilePath = Floor_imgFilePath;
                FloorClickHistoryFileName = FloorFileName;

                isCustomFloor = false;
                SendFloorImgBrush(bitmap, Floor_imgFilePath, FloorClickHistoryFileName, isCustomFloor); //메인윈도우의 이미지브러시 변경


                img_UserSelectedFloorFile.Source = bitmap;


        


                Floor_stackimgInput(bmi);


                //823
                CustomFloorThumnailClear(); //벽지 선택했을 땐 커스텀 벽지 선택한 거 섬네일 해제해주기.   


            }
        }

        public void Floor_stackimgInput(BitmapInfo bmi)
        {

            Floor_BitmapList.Add(bmi);

            switch (Floor_BitmapList.Count)
            {
                case 1:
                    img_FloorViewBox1.Source = Floor_BitmapList[0].SaveBitmap;
                    break;

                case 2:
                    img_FloorViewBox1.Source = Floor_BitmapList[1].SaveBitmap;
                    img_FloorViewBox2.Source = Floor_BitmapList[0].SaveBitmap;
                    break;

                case 3:
                    img_FloorViewBox1.Source = Floor_BitmapList[2].SaveBitmap;
                    img_FloorViewBox2.Source = Floor_BitmapList[1].SaveBitmap;
                    img_FloorViewBox3.Source = Floor_BitmapList[0].SaveBitmap;
                    break;

                case 4:
                    Floor_BitmapList.Remove(Floor_BitmapList[0]);
                    img_FloorViewBox1.Source = Floor_BitmapList[2].SaveBitmap;
                    img_FloorViewBox2.Source = Floor_BitmapList[1].SaveBitmap;
                    img_FloorViewBox3.Source = Floor_BitmapList[0].SaveBitmap;
                    break;

            }


        }

        private void btn_FloorFileDialogClear_Click(object sender, RoutedEventArgs e)
        {

            SendDelFloorImgBrush();
            FloorThumnailClear();

        }




        public void SendFloorImgBrush(BitmapImage bitmap, string Floor_imgFilePath, string FloorClickHistoryFileName, bool _isCustomFloor)
        {
            Call_MainWindow.ChangeFloorImgBrush(bitmap, Floor_imgFilePath, FloorClickHistoryFileName, _isCustomFloor);

            
            // new ImageBrush( new BitmapImage(new Uri("D:/winshare/mainproject/VisualSpace/Visual Space/Visual Space/icons/tiles/linetile1.png", UriKind.Absolute)  ));
        }
        public void SendDelFloorImgBrush()
        {

            Call_MainWindow.DeleteFloorImgBrush();
        }


        //벽지 섬네일 해제
        public void FloorThumnailClear()
        {

            img_UserSelectedFloorFile.Source = null;
            tbk_FloorFilePath.Text = null; //파일 경로 띄우는 텍스트 박스 삭제
                                           //  Floor_border.Stroke = Brushes.Black;

        }

        //새 파일 누를 때 호출됨.
        public void HistroyFloorThumClear()
        {
            if (Floor_BitmapList.Count != 0)
            {
                img_FloorViewBox1.Source = null;
                img_FloorViewBox2.Source = null;
                img_FloorViewBox3.Source = null;
                Floor_BitmapList.Clear();

            }

        }


        //821
        public CustomWallpaper _customWallpaper; //벽지쪽 부르기 위한 변수 선언
        private void btn_CustomWallpaper_Click(object sender, RoutedEventArgs e)
        {
            if (_customWallpaper != null) //이미 벽지 나 바닥 커스텀창이 있는데 새로 눌렀으면 그거 꺼주고 새로운 창 띄워준다.
            {
                _customWallpaper = null;
            }
               // this._customWallpaper.Activate();

            System.Windows.Controls.Button _sender = sender as System.Windows.Controls.Button;
            if (_sender.Name == "btn_MakeCustomFloor")
            {
                //System.Windows.MessageBox.Show("들어옴");

                _customWallpaper = new CustomWallpaper();
                _customWallpaper.Closed += _customWallpaper_Closed;
                _customWallpaper.Show();

                _customWallpaper.isCustomFloor(); //커스텀 바닥 버튼이 눌렸을 땐 이쪽 함수를 호출하여 callCustomFloor 플래그를 true로 해준다. 그래야 이미지 저장 및 불러오기 시 초기 경로 바뀌게 할 수 있음.

            }
            else //if (_sender.Name == "btn_MakeCustomWallpaper")
            {

                if (_customWallpaper == null)
                {
                    _customWallpaper = new CustomWallpaper();
                    _customWallpaper.Closed += _customWallpaper_Closed;
                    _customWallpaper.Show();


                }
            }

         

        }

        private void _customWallpaper_Closed(object sender, EventArgs e)
        {
            _customWallpaper = null;
        }


        //824
        bool isCustomWall = false;
        bool isCustomFloor = false;


        //823 커스텀벽지 버튼 클릭 다이얼로그창 띄우기
        private void btn_CustomViewBox1_Click(object sender, RoutedEventArgs e)
        {
            
            //823

            OpenFileDialog ofd = new OpenFileDialog(); //파일 열기 생성
            ofd.Multiselect = false; //중복 선택 금지
            ofd.Filter = "PNG(*.PNG)|*.PNG"; //파일 유형 선택 정의. PNG로 일단 고정
                                             // "엑셀 파일 (*.xls)|*.xls|엑셀 파일 (*.xlsx)|*.xlsx";
            ofd.Title = "커스텀 벽지 선택";
            // AppDomain.CurrentDomain.BaseDirectory + $"../../../pictures/{_ObjectType}/convert/{_ObjectType}.png";
            
            //   ofd.RestoreDirectory = false;
            ofd.AddExtension = true;


#if DEBUG

            string dir = Directory.GetCurrentDirectory();
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir + @"\customs\customwall";
            ofd.InitialDirectory = dir;




#else
            string dir = Directory.GetCurrentDirectory();
            dir = dir +@"\customs\customwall";
            ofd.InitialDirectory = dir;
#endif




            //경로, 

            DialogResult result = ofd.ShowDialog();
            if (result.ToString() == "OK") //파일 열기 실행
            {
                string custom_imgFilePath = ofd.FileName; //파일 경로
                int index = custom_imgFilePath.LastIndexOf("\\");
                string imgFileName = custom_imgFilePath.Substring(index + 1); //파일 이름
            //  tbk_CustomFilePath.Text = imgFileName + tail; // " 을 선택 중 입니다.";

                //선택해제 필요

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(custom_imgFilePath);
                bitmap.EndInit();



                BitmapInfo bmi = new BitmapInfo();
                bmi.SaveBitmap = bitmap;
                bmi.BitmapName = imgFileName; //파일명(.png까지)
                bmi.BitmapFilePath = custom_imgFilePath; //이미지 파일 경로
                                                         //   histroyClickSendParam = custom_imgFilePath; // 히스토리 버튼 눌렀을 때 텍스트창에 띄워주고, 메인창에 값보내기 위한 변수 
                                                         //  historyClickSendName = imgFileName; // 히스토리 버튼 눌렀을 때 텍스트창에 띄워주고, 메인창에 값보내기 위한 변수


                isCustomWall = true;//824
                SendImgBrush(bitmap, custom_imgFilePath, imgFileName, isCustomWall); //메인윈도우의 이미지브러시 변경


                img_SelectedCustom.Source = bitmap; //버튼 이미지 변경.

                               
                WallpaperThumnailClear();

                ////818 랙탱글 보더 색 바꾸기
                //ImageBrush imgBrush = new ImageBrush();
                //imgBrush.ImageSource = bitmap;
                //rc_border.Stroke = imgBrush;


                // stackimgInput(bmi);



            }
        }

        private void btn_ClearCustom_Click(object sender, RoutedEventArgs e)
        {

            //커스텀 벽지 섬네일 삭제
            SendDelImgBrush();           
            CustomThumnailClear();

        }

        //커스텀 벽지 섬네일 해제
        public void CustomThumnailClear()
        {
            img_SelectedCustom.Source = null;            
          

        }
            

        //커스텀바닥 선택 다이얼로그
        private void btn_CustomFloorViewBox1_Click(object sender, RoutedEventArgs e)
        {
            //823

            OpenFileDialog ofd = new OpenFileDialog(); //파일 열기 생성
            ofd.Multiselect = false; //중복 선택 금지
            ofd.Filter = "PNG(*.PNG)|*.PNG"; //파일 유형 선택 정의. PNG로 일단 고정
                                             // "엑셀 파일 (*.xls)|*.xls|엑셀 파일 (*.xlsx)|*.xlsx";
            ofd.Title = "커스텀 바닥 선택";
            // AppDomain.CurrentDomain.BaseDirectory + $"../../../pictures/{_ObjectType}/convert/{_ObjectType}.png";

            //   ofd.RestoreDirectory = false;
            ofd.AddExtension = true;


#if DEBUG
            string dir = Directory.GetCurrentDirectory();
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir + @"\customs\customfloor";
            ofd.InitialDirectory = dir;

#else
            string dir = Directory.GetCurrentDirectory();
            dir = dir +@"\customs\customfloor";
            ofd.InitialDirectory = dir;
#endif
            

            //경로, 

            DialogResult result = ofd.ShowDialog();
            if (result.ToString() == "OK") //파일 열기 실행
            {
                string custom_imgFilePath = ofd.FileName; //파일 경로
                int index = custom_imgFilePath.LastIndexOf("\\");
                string imgFileName = custom_imgFilePath.Substring(index + 1); //파일 이름
                                                                              //  tbk_CustomFilePath.Text = imgFileName + tail; // " 을 선택 중 입니다.";

                //선택해제 필요

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(custom_imgFilePath);
                bitmap.EndInit();



                BitmapInfo bmi = new BitmapInfo();
                bmi.SaveBitmap = bitmap;
                bmi.BitmapName = imgFileName; //파일명(.png까지)
                bmi.BitmapFilePath = custom_imgFilePath; //이미지 파일 경로
                                                         //   histroyClickSendParam = custom_imgFilePath; // 히스토리 버튼 눌렀을 때 텍스트창에 띄워주고, 메인창에 값보내기 위한 변수 
                                                         //  historyClickSendName = imgFileName; // 히스토리 버튼 눌렀을 때 텍스트창에 띄워주고, 메인창에 값보내기 위한 변수

                isCustomFloor = true; //824
                SendFloorImgBrush(bitmap, custom_imgFilePath, imgFileName, isCustomFloor); //메인윈도우의 바닥 이미지브러시 변경
                


                img_SelectedCustomFloor.Source = bitmap; //버튼 이미지 변경.


                FloorThumnailClear(); //커스텀 바닥을 선택하면 일반 벽지 선택하는 거 선택 해제
                

                ////818 랙탱글 보더 색 바꾸기
                //ImageBrush imgBrush = new ImageBrush();
                //imgBrush.ImageSource = bitmap;
                //rc_border.Stroke = imgBrush;


                // stackimgInput(bmi);



            }

        }


        private void btn_ClearCustomFloor_Click(object sender, RoutedEventArgs e)
        {
            SendDelFloorImgBrush();     //바닥 이미지 브러시 메인창에서 해제하도록 함 
            CustomFloorThumnailClear(); //

        }


        //커스텀 벽지 섬네일 해제
        public void CustomFloorThumnailClear()
        {
            img_SelectedCustomFloor.Source = null;            
        }
    }
}
