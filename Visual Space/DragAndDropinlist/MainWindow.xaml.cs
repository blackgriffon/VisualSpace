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
using System.Diagnostics.Contracts;

namespace DragAndDropinlist
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            List<User> items = new List<User>();

            for (int i = 0; i < 50; i++)
            {
                items.Add ( new User()
                {
                    Name = "Name" + i,
                    Age = 0 + i,
                    Mail = i + "@"
                } );
                DragList.ItemsSource = items;


            }
        }


        public class User
        {
            public String Name { get; set; }
            public int Age { get; set; }
            public string Mail { get; set; }
        }



        Point startPoint;

        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
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
            if (e.LeftButton == MouseButtonState.Pressed                )
            {
                // Get the dragged ListViewItem
                ListView listView = sender as ListView;
                ListViewItem listViewItem =
                    FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);




                // Find the data behind the ListViewItem
                User contact = (User)listView.ItemContainerGenerator.
                    ItemFromContainer(listViewItem);

                // Initialize the drag & drop operation
                DataObject dragData = new DataObject("myFormat", contact);
                DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
            }
        }










        private static T FindAnchestor<T>(DependencyObject current)
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

        private void DropList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("myFormat"))
            {
                User contact = e.Data.GetData("myFormat") as User;
                ListView listView = sender as ListView;
                listView.Items.Add(contact);
            }
        }




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




        /*
        private void List_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                // Get the dragged ListViewItem
                ListView listView = sender as ListView;
                ListViewItem listViewItem =
                    FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);

                // Find the data behind the ListViewItem
                Contact contact = (Contact)listView.ItemContainerGenerator.
                    ItemFromContainer(listViewItem);

                // Initialize the drag & drop operation
                DataObject dragData = new DataObject("myFormat", contact);
                DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
            }
        }

        private void DropList_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myFormat") ||
                sender == e.Source)   //같은 대상에 끌어놓는 경우 허용안함
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void DropList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("myFormat"))
            {
                Contact contact = e.Data.GetData("myFormat") as Contact;
                ListView listView = sender as ListView;
                listView.Items.Add(contact);
            }
        }




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

    */


    }
}
