using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using Xceed.Wpf.Toolkit;
using Nollan.Visual_Space.classes;



namespace Nollan.Visual_Space.DockingWindows
{
    /// <summary>
    /// CustomWallpaper.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CustomWallpaper : Window
    {
        public CustomWallpaper()
        {
            InitializeComponent();

            this.Loaded += CustomWallpaper_Loaded;


        }

        public static bool callCustomFloor;
        public void isCustomFloor()
        {
            //823
            callCustomFloor = true; //컬러창에서 커스텀 바닥을 누르면 이 플래그가 켜지고 경로 등이 커스텀 바닥쪽으로 바뀐다.

        }



        // Add an InkCanvas to the window, and allow the user to 
        // switch between using a green pen and a purple highlighter 
        // on the InkCanvas.
        private void CustomWallpaper_Loaded(object sender, RoutedEventArgs e)
        {



            // inkCanvas.Background = Brushes.DarkSlateBlue;
            inkCanvas.DefaultDrawingAttributes.Color = Colors.Transparent;

            //  root.Children.Add(inkCanvas);

            // Set up the DrawingAttributes for the pen.
            inkDA = new DrawingAttributes();
            inkDA.Color = Colors.SpringGreen;
            inkDA.Height = 5;
            inkDA.Width = 5;
            inkDA.FitToCurve = false;

            // Set up the DrawingAttributes for the highlighter.
            highlighterDA = new DrawingAttributes();
            highlighterDA.Color = Colors.Orchid;
            highlighterDA.IsHighlighter = true;
            highlighterDA.IgnorePressure = true;
            highlighterDA.StylusTip = StylusTip.Rectangle;
            highlighterDA.Height = 20;
            highlighterDA.Width = 10;

            inkCanvas.DefaultDrawingAttributes = inkDA;



            btn_Preview1.ToolTipOpening += Btn_Preview_ToolTipOpening;
            btn_Preview2.ToolTipOpening += Btn_Preview_ToolTipOpening;
            btn_Preview3.ToolTipOpening += Btn_Preview_ToolTipOpening;
            btn_Preview4.ToolTipOpening += Btn_Preview_ToolTipOpening;
            btn_Preview5.ToolTipOpening += Btn_Preview_ToolTipOpening;




            textBox1.Focus();
            
        }

        private void Btn_Preview_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
           Button btn = sender as Button;
          

           // int childCnt = VisualTreeHelper.GetChildrenCount(btn);
           //object child = VisualTreeHelper.GetChild(btn, childCnt-1);
           Image img = btn.Content as Image;
            UIElement copyimg = DeepCopy(img);
            btn.ToolTip = copyimg as Image;



        }

        public UIElement DeepCopy(UIElement element)
        {

          string shapestring = XamlWriter.Save(element);
          StringReader stringReader = new StringReader(shapestring);

           XmlTextReader xmlTextReader = new XmlTextReader(stringReader);

          UIElement DeepCopyobject = (UIElement)XamlReader.Load(xmlTextReader);

           return DeepCopyobject;

       }

    private byte[] Pixels = new byte[4];
        public BitmapImage bitmapImage;
        public static bool SaveCheck;
        public Func makeBtn;
        public delegate void Func();

        // WallPaper wallpaper = new WallPaper();


        //  You can use InkCanvas.Stokes property to get the existing strokes. And use Stroke.DrawingAttributes to change the color.
        //  Here is a simple example.

        public void ChangeColor(InkCanvas inkCanvas, Color color)
        {
            foreach (var stroke in inkCanvas.Strokes)
            {
                stroke.DrawingAttributes.Color = color;
            }
        }

        // 모든 잉크 지우기
        private void btn_AllErase(object sender, RoutedEventArgs e)
        {
            inkCanvas.Strokes.Clear();
        }

        // 지우기 모드
        private void btn_Erase(object sender, RoutedEventArgs e)
        {
            inkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }

        // 잉크 모드 821
        private void btn_Pen(object sender, RoutedEventArgs e)
        {

            if (highlighterDA != null)
            {
                highlighterDA = null;
                inkCanvas.EditingMode = InkCanvasEditingMode.None;
            }

            inkCanvas.EditingMode = InkCanvasEditingMode.Ink;

            // Set up the DrawingAttributes for the pen.
            inkDA = new DrawingAttributes();
            //   inkDA.Color = Colors.SpringGreen;
            inkDA.Height = 3;
            inkDA.Width = 3;
            inkDA.StylusTip = StylusTip.Ellipse;

            if (PickedColor != null)
            {
                inkDA.Color = PickedColor; //색. 사용자가 선택한 색으로 된다.
            }
            else
            {
                inkDA.Color = Colors.Transparent;
            }

            inkDA.FitToCurve = true;


            inkCanvas.DefaultDrawingAttributes = inkDA;
            //    

        }

        //형광팬 821
        public DrawingAttributes highlighterDA; //형광팬
        public DrawingAttributes inkDA; //일반 팬
        private void btn_Hightlighter(object sender, RoutedEventArgs e)
        {


            if (inkDA != null)
            {
                inkDA = null;
                inkCanvas.EditingMode = InkCanvasEditingMode.None;
            }

            inkCanvas.EditingMode = InkCanvasEditingMode.Ink;

            // Set up the DrawingAttributes for the highlighter.
            highlighterDA = new DrawingAttributes();

            if (PickedColor != null)
            {
                highlighterDA.Color = PickedColor; //Colors.Orchid; //색. 사용자가 선택한 색으로 된다.
            }
            else
            {
                highlighterDA.Color = Colors.Transparent;
            }

            highlighterDA.IsHighlighter = true;
            highlighterDA.IgnorePressure = true;
            highlighterDA.StylusTip = StylusTip.Rectangle;
            highlighterDA.Height = 10;
            highlighterDA.Width = 10;


            inkCanvas.DefaultDrawingAttributes = highlighterDA;

        }

        // 잉크 색상 변경
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point point = e.GetPosition(sender as Image);

            //821
            inkCanvas.DefaultDrawingAttributes.Color = PickedColor; // GetPixelColor(point);
        }


        // 픽셀 값 얻어 오기
        /*자믈쪽
          <Image Source="D:\winshare\mainproject\VisualSpace\Visual Space\Visual Space\icons\Toolbar\file-upload.png" 
                x:Name="colorsImage" Height="28" Width="48.333"   />
             */
        //private Color GetPixelColor(Point CurrentPoint)
        //{
        //    BitmapSource CurrentSource = colorsImage.Source as BitmapSource;

        //    // 비트맵 내의 좌표 값 계산
        //    CurrentPoint.X *= CurrentSource.PixelWidth / colorsImage.ActualWidth;
        //    CurrentPoint.Y *= CurrentSource.PixelHeight / colorsImage.ActualHeight;

        //    if (CurrentSource.Format == PixelFormats.Bgra32 || CurrentSource.Format == PixelFormats.Bgr32)
        //    {
        //        // 32bit stride = (width *bpp + 7) / 8
        //        int Stride = (CurrentSource.PixelWidth * CurrentSource.Format.BitsPerPixel + 7) / 8;

        //        // 한 픽셀 복사
        //        CurrentSource.CopyPixels(new Int32Rect((int)CurrentPoint.X, (int)CurrentPoint.Y, 1, 1), Pixels, Stride, 0);

        //        // 컬러로 변환 후 리턴
        //        return Color.FromArgb(Pixels[3], Pixels[2], Pixels[1], Pixels[0]);
        //    }

        //    else
        //    {
        //        System.Windows.MessageBox.Show("지원되지 않는 포맷형식");
        //    }

        //    return Color.FromArgb(Pixels[3], Pixels[2], Pixels[1], Pixels[0]);
        //}




        // 이미지 불러오기
        public void btn_Open(object sender, RoutedEventArgs e)
        {


            OpenFileDialog openDialog = new OpenFileDialog();
            
            //822
#if DEBUG

            string dir = Directory.GetCurrentDirectory();
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir.Substring(0, dir.LastIndexOf('\\'));

            if (callCustomFloor) //바닥 커스텀 버튼에서 호출됬으면 이쪽
            {
                dir = dir + @"\customs\customfloor";
            }
            else //벽지 커스텀 버튼에서 호출됬으면 이쪽
            {
                dir = dir + @"\customs\customwall";
            }
            openDialog.InitialDirectory = dir;

#else
            string dir = Directory.GetCurrentDirectory();

            if (callCustomFloor) //바닥 커스텀 버튼에서 호출됬으면 이쪽
            {
               dir = dir + @"\customs\customfloor";
            }
            else
            {
               dir = dir + @"\customs\customwall";
            }


            openDialog.InitialDirectory = dir;
#endif



            if (openDialog.ShowDialog() == true)
            {
                if (File.Exists(openDialog.FileName))
                {
                    bitmapImage = new BitmapImage(new Uri(openDialog.FileName,
                        UriKind.RelativeOrAbsolute));


                    // InkCanvas의 배경으로 지정
                    inkCanvas.Background = new ImageBrush(bitmapImage);


                    PrevBtn.IsEnabled = true; 
                    NextBtn.IsEnabled = true;
                }
            }
        }


        // 해당 이미지 저장
        public static void ImageSave(BitmapSource source)
        {


            // 이미지 포멧 들
            saveDialog.Filter = "PNG|*.png"; //|JPG|*.jpg|GIF|*.gif|BMP|*.bmp
            saveDialog.AddExtension = true;
            //saveDialog.CheckPathExists = true;


#if DEBUG

            string dir = Directory.GetCurrentDirectory();
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir.Substring(0, dir.LastIndexOf('\\'));
            dir = dir.Substring(0, dir.LastIndexOf('\\'));

            if (callCustomFloor) //바닥 커스텀 버튼에서 호출됬으면 이쪽
            {
                dir = dir + @"\customs\customfloor";
            }
            else //벽지 커스텀 버튼에서 호출됬으면 이쪽
            {
                dir = dir + @"\customs\customwall";
            }
            saveDialog.InitialDirectory = dir;

#else
            string dir = Directory.GetCurrentDirectory();

            if (callCustomFloor) //바닥 커스텀 버튼에서 호출됬으면 이쪽
            {
               dir = dir + @"\customs\customfloor";
            }
            else
            {
               dir = dir + @"\customs\customwall";
            }
            saveDialog.InitialDirectory = dir;
#endif


            // 전체 경로와 파일명과 확장자
            if (saveDialog.ShowDialog() == true)
            {
                BitmapEncoder encoder = null;

                // 파일 경로만 얻기
                string pathName = System.IO.Path.GetDirectoryName(saveDialog.FileName);
                string FileName = saveDialog.SafeFileName;

                // 파일 중복 확인후 전체 경로와 파일명 saveDialog.FileName에 넣어주기.
                saveDialog.FileName = GetAvailablePathname(pathName, FileName);


                // 파일 생성!
                FileStream stream = new FileStream(saveDialog.FileName, FileMode.Create, FileAccess.Write);

                // 파일  포멧
                string upper = saveDialog.SafeFileName.ToUpper();
                char[] format = upper.ToCharArray(saveDialog.SafeFileName.Length - 3, 3);
                upper = new string(format);


                // 해당 포멧에 맞게 인코더
                switch (upper.ToString())
                {
                    case "PNG":
                        encoder = new PngBitmapEncoder();
                        break;

                    case "JPG":
                        encoder = new JpegBitmapEncoder();
                        break;

                    case "GIF":
                        encoder = new GifBitmapEncoder();
                        break;

                    case "BMP":
                        encoder = new BmpBitmapEncoder();
                        break;
                }


                // 인코더 프레임에 이미지 추가
                encoder.Frames.Add(BitmapFrame.Create(source));

                // 파일에 저장            
                encoder.Save(stream);
                stream.Close();

                SaveCheck = true;
                //---------------------------------------------------------
                string[] splite = @pathName.Split('\\');
                string path = splite[0];
                for (int i = 1; i < splite.Length; i++)
                {
                    path += "\\\\" + splite[i].TrimEnd();
                }
                path += "\\\\" + FileName;
                //MessageBox.Show(path);
                //  string sqlStr = "ADDWALLPAPER/" + MainWindow.dbData.user_id + "/" + path;
                //   MainWindow.SendData(sqlStr);

                //MessageBox.Show(saveDialog.FileName);
            }
        }


  
        public void BtnHide()
        {
            textBox1.Text = null;
            // < 버튼 비활성화
            if (DaumPageNum == 1)
            {
                PrevBtn.IsEnabled = false;
                NextBtn.IsEnabled = true;
            }
            else if (DaumPageNum < 1)
            {
                DaumPageNum = 1;
                return;
            }
            else
            {
                PrevBtn.IsEnabled = true;
                NextBtn.IsEnabled = true;
            }

            // > 버튼 비활성화
            if (DaumPageNum == 50 )
            {
                NextBtn.IsEnabled = false;
                DaumPageNum = 50;
               
            }
           
        }

        //검색버튼 눌렀을 때
        public void btn_Search(object sender, RoutedEventArgs e)
        {
            DaumPageNum = 1;
            ImageNumber = 1;
            EnterParse = textBox1.Text;
            ClickCnt = 1;

            Flag_SearchEnter = true;
            tbk_Page.Text = DaumPageNum.ToString();

            BtnHide();

            SearchDaum(EnterParse);
            
        }

        //823
        bool Flag_SearchEnter; //엔터 누르기 전까진 페이지 버튼 안먹기
        // 검색창에서 검색 키워드를 입력하고 Enter를 쳤을때 이벤트 처리
        private void OnKeyDownHandler(object sender, KeyEventArgs e) //텍스트박스에 엔터쳤을 때
        {
            if (textBox1.Text != "")
            {
                if (e.Key == Key.Return)
                {
                    DaumPageNum = 1; //페이지 넘 0하면 오류나서 튕김
                    ImageNumber = 1;
                    EnterParse = textBox1.Text;
                    tbk_Page.Text = DaumPageNum.ToString();
                    ClickCnt = 1;

                    BtnHide();

                    SearchDaum(EnterParse);
                   


                    Flag_SearchEnter = true;

                }
            }
            else if (e.Key == Key.Return)
            {
                System.Windows.MessageBox.Show("검색 키워드를 입력해 주세요");
                Flag_SearchEnter = false;
            }
        }

        // 파일 경로와 파일 이름
        public string fullPathName = saveDialog.FileName;
        // 파일 이름
        public string fileName = saveDialog.SafeFileName;

        public int ImageNumber = 1;
        public int DaumPageNum = 1; //디폴트1
        public string EnterParse;
        public string testStr;

        public int testNum = 0;
        public int ClickCnt = 1; //처음 검색할 때 5개 나오니까. 0으로 안하고 1로 해줌.

        public int apiResult = 50;

        // daum 사이트에서 이미지 uri 받아오기
        public string SearchDaum(string keyword, int page)
        {

            System.Windows.MessageBox.Show("SearchDaum 2");

            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.Load("http://apis.daum.net/search/image?q=" + keyword +
                "&apikey=9e43e56e97e37bdbf8c72561251f35c4&result=20&pageno=" + page.ToString());
            testStr = string.Empty;

            //테스트용인가봄
            //BitmapImage BI;
            //BI = LoadImage(testStr);
            //


            for (int i = 0; i < 5; i++)
            {
                XmlNodeList mainNode = XmlDoc.GetElementsByTagName("image");
                testStr = mainNode[testNum++].InnerText; //ImageNumber


                Image SearchImage = new Image();
                BitmapImage BI2 = new BitmapImage(new Uri(testStr, UriKind.RelativeOrAbsolute));
                // inkCanvas.Background = new ImageBrush(BI2);

                switch (i)
                {
                    case 0:
                        img_Preview1.Source = BI2;
                        break;

                    case 1:
                        img_Preview2.Source = BI2;
                        break;

                    case 2:
                        img_Preview3.Source = BI2;
                        break;

                    case 3:
                        img_Preview4.Source = BI2;
                        break;
                    case 4:
                        img_Preview5.Source = BI2;
                        break;
                }


            }


            return testStr;
        } //전 기수 원본

        public string SearchDaum(string keyword) //json으로 받아온 거 읽기
        {    
            try
            {                
                string url = "https://dapi.kakao.com/v2/search/image?query=" + keyword + "&page=" + DaumPageNum.ToString() + "&size=5";
                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
                string credentials = "0059414e9a3248f85f3a6aec9def4146";
                CredentialCache mycache = new CredentialCache();
                myReq.Headers.Add("Authorization", "KakaoAK " + credentials);
                myReq.Method = "GET";

                HttpWebResponse wr = (HttpWebResponse)myReq.GetResponse();
                Stream receiveStream = wr.GetResponseStream();
                StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
                string content = reader.ReadToEnd();
                Console.WriteLine(content);

                var json = "[" + content + "]"; // change this to array
                var objects = JArray.Parse(json); // parse as array  
                foreach (JObject o in objects.Children<JObject>())
                {

                    for (int i = 0; i < 5; i++)
                    {
                        testStr = o.SelectToken($"documents[{i}].image_url").ToString();

                        BitmapImage BI2 = new BitmapImage(new Uri(testStr, UriKind.RelativeOrAbsolute));

                        // img_Preview[i].Source = BI2;

                        switch (i)
                        {
                            case 0:
                                img_Preview1.Source = BI2;
                                btn_Preview1.ToolTip = BI2;
                                break;

                            case 1:
                                img_Preview2.Source = BI2;
                                btn_Preview2.ToolTip = BI2;
                                break;

                            case 2:
                                img_Preview3.Source = BI2;
                                btn_Preview3.ToolTip = BI2;
                                break;

                            case 3:
                                img_Preview4.Source = BI2;
                                btn_Preview4.ToolTip = BI2;
                                break;
                            case 4:
                                img_Preview5.Source = BI2;
                                btn_Preview5.ToolTip = BI2;
                                break;
                        }

                    }

                    // inkCanvas.Background = new ImageBrush(BI2);
                }


            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);

            }

            return testStr;
        }




        public string XmlSearchDaum(string keyword)
        {

            try
            {

                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.Load("http://apis.daum.net/search/image?q=" + keyword +
                    "&apikey=9e43e56e97e37bdbf8c72561251f35c4&result=20&pageno=" + DaumPageNum.ToString() + "&output=xml");
                testStr = string.Empty;

                //테스트용인가봄
                //BitmapImage BI;
                //BI = LoadImage(testStr);
                //


                for (int i = 0; i < 5; i++)
                {
                    XmlNodeList mainNode = XmlDoc.GetElementsByTagName("image");
                    testStr = mainNode[testNum++].InnerText; //ImageNumber


                    Image SearchImage = new Image();
                    BitmapImage BI2 = new BitmapImage(new Uri(testStr, UriKind.RelativeOrAbsolute));
                    // inkCanvas.Background = new ImageBrush(BI2);

                    switch (i)
                    {
                        case 0:
                            img_Preview1.Source = BI2;
                            break;

                        case 1:
                            img_Preview2.Source = BI2;
                            break;

                        case 2:
                            img_Preview3.Source = BI2;
                            break;

                        case 3:
                            img_Preview4.Source = BI2;
                            break;
                        case 4:
                            img_Preview5.Source = BI2;
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);

            }

            return testStr;
        }


        //Image URL -> Bitmap으로..
        public BitmapImage LoadImage(string url)
        {

            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    return null;
                }
                WebClient wc = new WebClient();
                Byte[] MyData = wc.DownloadData(url);
                wc.Dispose();
                BitmapImage bimgTemp = new BitmapImage();
                bimgTemp.BeginInit();
                bimgTemp.StreamSource = new MemoryStream(MyData);
                bimgTemp.EndInit();
                return bimgTemp;
            }

            catch
            {
                return null;
            }
        }

        // > 버튼 기능
        private void MyNextImage(object sender, RoutedEventArgs e)
        {
            if (Flag_SearchEnter)
            {
                if ((ClickCnt * 5) % 5 == 0) //20의 배수일 때마다 Daumpage 넘버를 증가시킨다.
                {
                    DaumPageNum++;
                    testNum = 0;


                }



                SearchDaum(EnterParse);
                
                ClickCnt++;
                tbk_Page.Text = ClickCnt.ToString();


                BtnHide();

                //첫번째로 클릭시 2, 그 다음은 3 ...   


            }
        }


        // < 버튼 기능
        private void MyPrevImage(object sender, RoutedEventArgs e)
        {
            if (Flag_SearchEnter)
            {
                if ((ClickCnt * 5) % 5 == 0) //20의 배수일 때마다 Daumpage 넘버를 증가시킨다.
                {
                    DaumPageNum--;
                    testNum = 0;


                }



                SearchDaum(EnterParse);

                ClickCnt--;
                tbk_Page.Text = ClickCnt.ToString();
                //첫번째로 클릭시 2, 그 다음은 3 ...   
                BtnHide();
            }

        }



  
        private void NextImage(object sender, RoutedEventArgs e)
        {
            if (Flag_SearchEnter)
            {


                if (ImageNumber == 9)
                {
                    DaumPageNum++;
                    ImageNumber = 1;
                }

                else
                {
                    ImageNumber++;
                }

                SearchDaum(EnterParse, DaumPageNum);
                //    BtnHide();
            }
        }  
        private void PrevImage(object sender, RoutedEventArgs e)
        {
            if (Flag_SearchEnter)
            {
                if (ImageNumber < 2)
                {
                    DaumPageNum--;
                    ImageNumber = 9;
                }

                else
                {
                    ImageNumber--;
                }
                SearchDaum(EnterParse, DaumPageNum);
                //  BtnHide();
            }

        }

       

        // 이미지 캡쳐
        private void btn_Capture(object sender, RoutedEventArgs e)
        {

            RenderTargetBitmap bitmap = ConverterBitmapImage(inkCanvas);
            ImageSave(bitmap); // 822 잉크캔버스에 있는 이미지를 비트맵으로 바꿔서 이미지를 저장한다.

            if (SaveCheck)
            {
                //  makeBtn(); //버튼 만들기.
                SaveCheck = false;
            }
        }

        // 해당 객체를 이미지로 변환
        private static RenderTargetBitmap ConverterBitmapImage(FrameworkElement element)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            // 해당객체의 그래픽 요쇼로 사각형의 그림을 그립니다.
            drawingContext.DrawRectangle(new VisualBrush(element), null,
                new Rect(new Point(0, 0), new Point(element.ActualWidth, element.ActualHeight)));

            drawingContext.Close();

            // 비트 맵으로 변환합니다.
            RenderTargetBitmap target = new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight,
                96, 96, System.Windows.Media.PixelFormats.Pbgra32);

            target.Render(drawingVisual);
            return target;
        }




        // 파일 중복 확인후 중복일 경우 파일 이름뒤에 (숫자) 넣기.
        static string GetAvailablePathname(string folderPath, string filename)
        {
            int invalidChar = 0;
            do
            {
                invalidChar = filename.IndexOfAny(System.IO.Path.GetInvalidFileNameChars());
                if (invalidChar != -1)
                {
                    filename = filename.Remove(invalidChar, 1);
                }
            }
            while (invalidChar != -1);

            string fullPath = System.IO.Path.Combine(folderPath, filename);
            string filenameWithoutExtention = System.IO.Path.GetFileNameWithoutExtension(filename);
            string extension = System.IO.Path.GetExtension(filename);

            while (File.Exists(fullPath))
            {
                Regex rg = new Regex(@".*\((?<Num>\d*)\)");
                Match mt = rg.Match(fullPath);

                if (mt.Success)
                {
                    string numberOfCopy = mt.Groups["Num"].Value;
                    int nextNumberOfCopy = int.Parse(numberOfCopy) + 1;
                    int posStart = fullPath.LastIndexOf("(" + numberOfCopy + ")");
                    fullPath = string.Format("{0}({1}){2}", fullPath.Substring(0, posStart), nextNumberOfCopy, extension);
                }
                else
                {
                    fullPath = folderPath + "\\" + filenameWithoutExtention + "(2)" + extension;
                }
            }
            return fullPath;

        }


        static SaveFileDialog saveDialog = new SaveFileDialog(); //win32로?      


        //821
        public Color PickedColor; //선택한 컬러
        private void _colorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            PickedColor = (Color)_colorPicker.SelectedColor; //Color? 형식이라 명시적으로 형변환해줘야 함.


            //821
            inkCanvas.DefaultDrawingAttributes.Color = PickedColor; // GetPixelColor(point);

        }



        private void isClick_Click(object sender, RoutedEventArgs e)
        {
            PickedColor = (Color)_colorPicker.SelectedColor; //Color? 형식이라 명시적으로 형변환해줘야 함.

            string str = _colorPicker.SelectedColorText;

            System.Windows.MessageBox.Show(str);
        }

 
        //901 jpeg말고 다른 거 되면 오류?
        private void InputButtonToinkCanvas(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;             
            UIElement copyimg = DeepCopy(btn.Content as Image);
            Image img = copyimg as Image;


            /*
             // 파일  포멧
                string upper = saveDialog.SafeFileName.ToUpper();
                char[] format = upper.ToCharArray(saveDialog.SafeFileName.Length - 3, 3);
                upper = new string(format);


                // 해당 포멧에 맞게 인코더
                switch (upper.ToString())
                {
                    case "PNG":
                        encoder = new PngBitmapEncoder();
                        break;

                    case "JPG":
                        encoder = new JpegBitmapEncoder();
                        break;

                    case "GIF":
                        encoder = new GifBitmapEncoder();
                        break;

                    case "BMP":
                        encoder = new BmpBitmapEncoder();
                        break;
                }

                // 인코더 프레임에 이미지 추가
                encoder.Frames.Add(BitmapFrame.Create(source));

                // 파일에 저장            
                encoder.Save(stream);
                stream.Close();
             
             */



            byte[] _tempbyte = null;

            try
            {
                byte[] bytesource = Utils.ConvertBitmapSourceToByteArray(img.Source);
                _tempbyte = bytesource;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("마우스 우클릭을 허용하지 않는 블로그 사진입니다. 저작권을 보호를 위해 양해해주세요.");
            }

            if (_tempbyte != null) //바이트배열이 존재할 경우에만 진입
            {
                MemoryStream memorystream = new MemoryStream(_tempbyte);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = memorystream;
                bitmap.EndInit();


                // BitmapImage bmp = img.Source as BitmapImage;
                ImageBrush imgBrush = new ImageBrush(bitmap);
                inkCanvas.Background = imgBrush;


            }
           //  Utils.BufferFromImage(bmp); //비트맵이미지에서 바이트배열로 변환

            //System.Drawing.Bitmap newBMP = new System.Drawing.Bitmap(originalBMP, newWidth, newHeight);
            //System.IO.MemoryStream stream = new System.IO.MemoryStream();
            //newBMP.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);


            //using (MemoryStream ms = new MemoryStream(imageData))
            //{
            //    var decoder = BitmapDecoder.Create(ms,
            //        BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            //    return decoder.Frames[0];
            //}

            //Bitmap img = (Bitmap)Image.FromStream(memStream);
            //BitmapImage bmImg = new BitmapImage();

            //using (MemoryStream memStream2 = new MemoryStream())
            //{
            //    img.Save(memStream2, System.Drawing.Imaging.ImageFormat.Png);
            //    ms.Position = 0;

            //    bmImg.BeginInit();
            //    bmImg.CacheOption = BitmapCacheOption.OnLoad;
            //    bmImg.UriSource = null;
            //    bmImg.StreamSource = memStream2;
            //    bmImg.EndInit();
            //}




        }
    }
}
