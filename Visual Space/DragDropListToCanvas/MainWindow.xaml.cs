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

namespace DragDropListToCanvas
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            createUserList();




        }

        public void createUserList()
        {

            List<User> items = new List<User>();

            for (int i = 0; i < 50; i++)
            {
                items.Add(new User()
                {
                    Name = "Name" + i,
                    Age = 0 + i,
                    Mail = i + "@"
                });
                DragList.ItemsSource = items;


            }

        }

        public class User
        {
            public String Name { get; set; }
            public int Age { get; set; }
            public string Mail { get; set; }
        }

        public class myitem
        {
            public string VisualName { get; set; }
            public string ProgramName { get; set; }
            public Image image { get; set; }

        }

        public void createMyItem()
        {


        }



        Point startPoint;
        int Selectedidx;

        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            
              
                startPoint = e.GetPosition(null);
           

        }

        private void List_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;



            /*   if (e.LeftButton == MouseButtonState.Pressed &&
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
             */
            if (e.LeftButton == MouseButtonState.Pressed && sender is ListView LView)
            {
                // Get the dragged ListViewItem
                LView = sender as ListView;

                ListViewItem listViewItem =
                    FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);




                // Find the data behind the ListViewItem
                User contact = (User)LView.ItemContainerGenerator.
                    ItemFromContainer(listViewItem);

                // Initialize the drag & drop operation
                DataObject dragData = new DataObject("myFormat", contact);
                DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
            }
        }

        private void DropList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("myFormat"))
            {
                User contact = e.Data.GetData("myFormat") as User;


                ListView listView = sender as ListView;
                listView.Items.Add(contact);
            }
        }




        private void imageList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Activate();

            // Store the mouse position
            if (imageListView.SelectedIndex == -1) //리스트뷰 아이템을 선택하지 않았다면 -1반환
            {
                Selectedidx = -1;
                return;
            }
            else //리스트뷰 아이템을 선택했다면
            {
                Selectedidx = imageListView.SelectedIndex; //제대로 리스트뷰 아이템 중에 하나를 선택했으면 그 인덱스 값을 저장한다.
                startPoint = e.GetPosition(null);
            }

        }

        private void imageList_MouseMove(object sender, MouseEventArgs e)
        {

            if (Selectedidx != -1) //아이템을 제대로 선택했다면
            {
                


                // Get the current mouse position
                Point mousePos = e.GetPosition(null);
                Vector diff = startPoint - mousePos;



                /*   if (e.LeftButton == MouseButtonState.Pressed &&
                    Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                 */
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    var LView = sender as ListView;
                    // Get the dragged ListViewItem
                    // LView = sender as ListView;


                    try
                    {
                        ListViewItem listViewItem =
                           FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);

                        // Find the data behind the ListViewItem
                        ListViewItem contact = (ListViewItem)LView.ItemContainerGenerator.
                            ItemFromContainer(listViewItem);


                        // Initialize the drag & drop operation
                        DataObject dragData = new DataObject("myFormat", contact);

                        DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
                    }
                    catch(Exception ex)
                    {

                    }

                    /*
                      // Find the data behind the ListViewItem
                User contact = (User)LView.ItemContainerGenerator.
                    ItemFromContainer(listViewItem);
                     */


                  
                }


            }
        }



        private void imageDropList_Drop(object sender, DragEventArgs e)
        {



            if (e.Data.GetDataPresent("myFormat"))
            {
                ListViewItem contact = e.Data.GetData("myFormat") as ListViewItem;
                var sp = contact.Content as StackPanel;


                // var 

                //string txt = null;
                //foreach (TextBlock tb in sp.Children.OfType<TextBlock>())
                //{

                //    if (tb.Name == "hiddenText")
                //    {
                //        txt = tb.Text;
                //        break;
                //    }
                //}

                string txt = null;
                foreach (TextBox tb in sp.Children.OfType<TextBox>())
                {

                    if (tb.Name == "_" + $"{Selectedidx}")
                    {
                        txt = tb.Text;
                        break;
                    }
                }



                Canvas listView = sender as Canvas;

                TextBlock newTb = new TextBlock() { Text = txt, FontSize = 20 };
                Image myImage3 = new Image();
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri($"../../images/{txt}.jpg", UriKind.Relative);
                bi3.EndInit();
                myImage3.Stretch = Stretch.Uniform;
                myImage3.Source = bi3;
                Canvas.SetLeft(myImage3, e.GetPosition(listView).X);
                Canvas.SetTop(myImage3, e.GetPosition(listView).Y);
                


                listView.Children.Add(myImage3);

            


                //  listView.Items.Add(contact);
            }
        }




        public static T FindAnchestor<T>(DependencyObject current)
    where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }









        private void DropList_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myFormat") ||
                sender == e.Source)   //같은 대상에 끌어놓는 경우 허용안함
            {
                e.Effects = DragDropEffects.None;
            }
        }

        //private void DropList_Drop(object sender, DragEventArgs e)
        //{
        //    if (e.Data.GetDataPresent("myFormat"))
        //    {
        //        User contact = e.Data.GetData("myFormat") as User;
        //        ListView listView = sender as ListView;
        //        listView.Items.Add(contact);
        //    }
        //}

       




        public class Contact
        {
            public Contact(string firstname, string lastname)
            {
                Firstname = firstname;
                Lastname = lastname;
            }

            public string Firstname { get; set; }
            public string Lastname { get; set; }

            public override string ToString()
            {
                return string.Format("Firstname: { 0}, Lastname: { 1}"
               , Firstname, Lastname);
            }
        }






    }
}
