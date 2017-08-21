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
using ColorReflectionwithWPF;
using Nollan.Visual_Space.classes;
using System.Windows.Forms;
using System.IO;

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


            //  ColorReflection();

           

        }


        public MainWindow Call_MainWindow;
        public int SelectedViewIdx;



        public void ColorReflection()
        {

            IEnumerable<PropertyInfo> properties = typeof(Colors).GetTypeInfo().DeclaredProperties; //리플렉션

            foreach (PropertyInfo property in properties)
            {
                Color clr = (Color)property.GetValue(null);
                ColorItem clrItem = new ColorItem(property.Name, clr);

                classes.colorTag ct = new classes.colorTag();
                ct.color = clr; //컬러 정보를 컬러태그 클래스 안에 삽입
                ct.colorName = property.Name;

                clrItem.Tag = ct;
          

           //     _colorListView.Items.Add(clrItem);
            }

        }        
        public void ColorFinder(object sender, MouseButtonEventArgs e)
        {

            System.Windows.Controls.ListView lv = sender as System.Windows.Controls.ListView;
            // lv.Items.Count;

            int colorCount = 0;
            ColorItem SelectedColor = null;
            string LineName = null;
            foreach (ColorItem item in lv.Items)
            {

                if (SelectedViewIdx == colorCount)
                {
                    SelectedColor = item;
                    LineName = "#" + (SelectedColor.Name).Substring(1); // _ 붙은 거 서브스트링으로 지워줌.


                    // MessageBox.Show(LineName);
                    break;
                }

                colorCount++;


            }


        //    tbx_ColorName.Text = LineName;
            colorTag ct = SelectedColor.Tag as colorTag;
            SendColor = ct.color;


            Call_MainWindow.ChangeLineColor(SendColor);

        }
        public Color SendColor;
        public void btn_Apply_Click(object sender, RoutedEventArgs e)
        {
            Call_MainWindow.ChangeLineColor(SendColor);
            
        }
        private void _colorListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

      //      SelectedViewIdx = _colorListView.SelectedIndex; //리스트뷰 안에서 자체적으로 선택된 인덱스 값을 가지고 있음.
                                                            // MessageBox.Show((SelectedViewIdx).ToString());


            ColorFinder(sender, e);


        }
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
        private void btn_FileDialog_Click(object sender, RoutedEventArgs e)
        {


            OpenFileDialog ofd = new OpenFileDialog(); //파일 열기 생성
            ofd.Multiselect = false; //중복 선택 금지
            ofd.Filter = "PNG(*.PNG)|*.PNG"; //파일 유형 선택 정의. PNG로 일단 고정
                                             // "엑셀 파일 (*.xls)|*.xls|엑셀 파일 (*.xlsx)|*.xlsx";

            // AppDomain.CurrentDomain.BaseDirectory + $"../../../pictures/{_ObjectType}/convert/{_ObjectType}.png";




            //   ofd.RestoreDirectory = false;
            ofd.AddExtension = true;

            string dir = Directory.GetCurrentDirectory();
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir + @"\pictures\wallpapers";

            //   ofd.InitialDirectory = "D:\\winshare\\mainproject\\VisualSpace\\Visual Space\\Visual Space\\pictures\\wallpapers"; //절대경로로 실험
            ofd.InitialDirectory = dir; //대화상자 시작시 초기 디렉토리 지정
            //  ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + $"pictures/wallpapers";
            ofd.Title = "벽지 선택";




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
                histroyClickSendParam = Wallpaper_imgFilePath;



                SendImgBrush(bitmap, Wallpaper_imgFilePath); //메인윈도우의 이미지브러시 변경


                img_UserSelectedFile.Source = bitmap;


                //818 랙탱글 보더 색 바꾸기
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = bitmap;
                rc_border.Stroke = imgBrush;


                stackimgInput(bmi);

          

                // string str2 = Directory.GetCurrentDirectory(); //현재 디렉토리 가져오기
                // System.Windows.MessageBox.Show(str2);

                //  string[] str = Directory.GetFiles(imgFilePath);
                // System.Windows.MessageBox.Show((str).ToString());



            }
        }

      

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


        public void SendImgBrush(BitmapImage bitmap, string Wallpaper_imgFilePath)
        {
            Call_MainWindow.ChangeImgBrush(bitmap, Wallpaper_imgFilePath);


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
            rc_border.Stroke = Brushes.Black;

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




        string histroyClickSendParam;
        private void btn_ViewBox1_Click(object sender, RoutedEventArgs e)
        {
            if (BitmapList.Count == 1)
            {
                SendImgBrush(BitmapList[0].SaveBitmap, histroyClickSendParam);
                img_UserSelectedFile.Source = BitmapList[0].SaveBitmap;
                
                tbk_FilePath.Text = BitmapList[0].BitmapName + tail;


                //818 랙탱글 보더 색 바꾸기
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = BitmapList[0].SaveBitmap;
                rc_border.Stroke = imgBrush;



            }

            else if (BitmapList.Count == 2)
            {
                SendImgBrush(BitmapList[1].SaveBitmap, histroyClickSendParam);
                img_UserSelectedFile.Source = BitmapList[1].SaveBitmap;
                tbk_FilePath.Text = BitmapList[1].BitmapName + tail;

                //818 랙탱글 보더 색 바꾸기
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = BitmapList[1].SaveBitmap;
                rc_border.Stroke = imgBrush;
            }

            else if (BitmapList.Count == 3)
            {
                SendImgBrush(BitmapList[2].SaveBitmap, histroyClickSendParam);
                img_UserSelectedFile.Source = BitmapList[2].SaveBitmap;
                tbk_FilePath.Text = BitmapList[2].BitmapName + tail;

                //818 랙탱글 보더 색 바꾸기
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = BitmapList[2].SaveBitmap;
                rc_border.Stroke = imgBrush;
            }

        }

        private void btn_ViewBox2_Click(object sender, RoutedEventArgs e)
        {

            if (BitmapList.Count == 2)
            {
                SendImgBrush(BitmapList[0].SaveBitmap, histroyClickSendParam);
                img_UserSelectedFile.Source = BitmapList[0].SaveBitmap;
                tbk_FilePath.Text = BitmapList[0].BitmapName + tail;

                //818 랙탱글 보더 색 바꾸기
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = BitmapList[0].SaveBitmap;
                rc_border.Stroke = imgBrush;

            }
            else if (BitmapList.Count == 3)
            {
                SendImgBrush(BitmapList[1].SaveBitmap, histroyClickSendParam);
                img_UserSelectedFile.Source = BitmapList[1].SaveBitmap;
                tbk_FilePath.Text = BitmapList[1].BitmapName + tail;

                //818 랙탱글 보더 색 바꾸기
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = BitmapList[1].SaveBitmap;
                rc_border.Stroke = imgBrush;

            }

        }

        private void btn_ViewBox3_Click(object sender, RoutedEventArgs e)
        {
            if (BitmapList.Count == 3)
            {
                SendImgBrush(BitmapList[0].SaveBitmap, histroyClickSendParam);
                img_UserSelectedFile.Source = BitmapList[0].SaveBitmap;
                tbk_FilePath.Text = BitmapList[0].BitmapName + tail;


                //818 랙탱글 보더 색 바꾸기
                ImageBrush imgBrush = new ImageBrush();
                imgBrush.ImageSource = BitmapList[0].SaveBitmap;
                rc_border.Stroke = imgBrush;
            }

        }





        //바닥쪽

        private void btn_FloorViewBox1_Click(object sender, RoutedEventArgs e)
        {

            if (Floor_BitmapList.Count == 1)
            {
                SendFloorImgBrush(Floor_BitmapList[0].SaveBitmap, FloorClickHistoryFilePath);
                img_UserSelectedFloorFile.Source = Floor_BitmapList[0].SaveBitmap;

                tbk_FloorFilePath.Text = Floor_BitmapList[0].BitmapName + tail;


                // 랙탱글 보더 색 바꾸기
              //  ImageBrush imgBrush = new ImageBrush();
              //  imgBrush.ImageSource = Floor_BitmapList[0].SaveBitmap;
              //  Floor_border.Stroke = imgBrush;



            }

            else if (Floor_BitmapList.Count == 2)
            {
                SendFloorImgBrush(Floor_BitmapList[1].SaveBitmap, FloorClickHistoryFilePath);
                img_UserSelectedFloorFile.Source = Floor_BitmapList[1].SaveBitmap;
                tbk_FloorFilePath.Text = Floor_BitmapList[1].BitmapName + tail;

                // 랙탱글 보더 색 바꾸기
               // ImageBrush imgBrush = new ImageBrush();
               // imgBrush.ImageSource = Floor_BitmapList[1].SaveBitmap;
               // Floor_border.Stroke = imgBrush;
            }

            else if (Floor_BitmapList.Count == 3)
            {
                SendFloorImgBrush(Floor_BitmapList[2].SaveBitmap, FloorClickHistoryFilePath);
                img_UserSelectedFloorFile.Source = Floor_BitmapList[2].SaveBitmap;
                tbk_FloorFilePath.Text = Floor_BitmapList[2].BitmapName + tail;

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
                SendFloorImgBrush(Floor_BitmapList[0].SaveBitmap, FloorClickHistoryFilePath);
                img_UserSelectedFloorFile.Source = Floor_BitmapList[0].SaveBitmap;
                tbk_FloorFilePath.Text = Floor_BitmapList[0].BitmapName + tail;

                // 랙탱글 보더 색 바꾸기
                //ImageBrush imgBrush = new ImageBrush();
                //imgBrush.ImageSource = Floor_BitmapList[0].SaveBitmap;
               // Floor_border.Stroke = imgBrush;

            }
            else if (Floor_BitmapList.Count == 3)
            {
                SendFloorImgBrush(Floor_BitmapList[1].SaveBitmap, FloorClickHistoryFilePath);
                img_UserSelectedFloorFile.Source = Floor_BitmapList[1].SaveBitmap;
                tbk_FloorFilePath.Text = Floor_BitmapList[1].BitmapName + tail;

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
                SendFloorImgBrush(Floor_BitmapList[0].SaveBitmap, FloorClickHistoryFilePath);
                img_UserSelectedFloorFile.Source = Floor_BitmapList[0].SaveBitmap;
                tbk_FloorFilePath.Text = Floor_BitmapList[0].BitmapName + tail;


                // 랙탱글 보더 색 바꾸기
               // ImageBrush imgBrush = new ImageBrush();
               // imgBrush.ImageSource = Floor_BitmapList[0].SaveBitmap;
               // Floor_border.Stroke = imgBrush;
            }

        }

        public string FloorClickHistoryFilePath;
        private void btn_FloorFileDialog_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog(); //파일 열기 생성
            ofd.Multiselect = false; //중복 선택 금지
            ofd.Filter = "PNG(*.PNG)|*.PNG"; //파일 유형 선택 정의. PNG로 일단 고정
                                             // "엑셀 파일 (*.xls)|*.xls|엑셀 파일 (*.xlsx)|*.xlsx";

            // AppDomain.CurrentDomain.BaseDirectory + $"../../../pictures/{_ObjectType}/convert/{_ObjectType}.png";




            //   ofd.RestoreDirectory = false;
            ofd.AddExtension = true;

            string dir = Directory.GetCurrentDirectory();
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir + @"\pictures\floors";

            //   ofd.InitialDirectory = "D:\\winshare\\mainproject\\VisualSpace\\Visual Space\\Visual Space\\pictures\\wallpapers"; //절대경로로 실험
            ofd.InitialDirectory = dir; //대화상자 시작시 초기 디렉토리 지정
            //  ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + $"pictures/wallpapers";
            ofd.Title = "바닥 선택";


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
                


                SendFloorImgBrush(bitmap, Floor_imgFilePath); //메인윈도우의 이미지브러시 변경


                img_UserSelectedFloorFile.Source = bitmap;


                //818 랙탱글 보더 색 바꾸기
             //   ImageBrush imgBrush = new ImageBrush();
             //   imgBrush.ImageSource = bitmap;             //   
                // Floor_border.Stroke = imgBrush;  //바닥은 랙탱글 보더 안바꿈


                Floor_stackimgInput(bmi);

                
                // string str2 = Directory.GetCurrentDirectory(); //현재 디렉토리 가져오기
                // System.Windows.MessageBox.Show(str2);

                //  string[] str = Directory.GetFiles(imgFilePath);
                // System.Windows.MessageBox.Show((str).ToString());

            
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




        public void SendFloorImgBrush(BitmapImage bitmap, string Floor_imgFilePath)
        {
            Call_MainWindow.ChangeFloorImgBrush(bitmap, Floor_imgFilePath);


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




    }
}
