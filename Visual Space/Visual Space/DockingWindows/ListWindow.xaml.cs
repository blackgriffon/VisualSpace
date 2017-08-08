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

namespace Nollan.Visual_Space.DockingWindows
{
    /// <summary>
    /// ListWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ListWindow : DockingLibrary.DockableContent
    {
        public ListWindow()
        {
            InitializeComponent();
            //    this.MouseWheel += new MouseWheelEventHandler(MainWindow_MouseWheel);
         //   this.AddHandler(UIElement.MouseWheelEvent, new MouseWheelEventHandler(OnMouseWheel), true);
            

        }

        public MainWindow call_mainwindow;
  
              

        public enum CollectionTable
        {
             chairs,
             beds,
             desk
           

             
        };

        //public string ReturnKorea(string english)
        //{


        //    return;
        //}

        
        public void ReceiveAppendedList(ObjConvertImageInfo ObjConvertimgInfo) //메인창에서 이 함수를 호출하게 되면 리스트 윈도우에 있는 트리뷰에 해당 내용을 추가한다.
        {

            string VisualName = ObjConvertimgInfo.VisualName;
            string ObjType = ObjConvertimgInfo.ObjectType;
            string ObjName = ObjConvertimgInfo.ObjectName;
            

            //오브젝트타입으로 트리뷰아이템을 추가하는데 해당트리뷰아이템 이름이 없다면 최상위에 트리뷰아이템(대분류)를 추가한 후 하위분류를 추가, 해당트리뷰가 있다면 이미 있는 대분류에 하위분류 추가

          
                TreeViewItem tvItem = new TreeViewItem();              


                tvItem.Header = VisualName;
                tvItem.Tag = ObjConvertimgInfo;
                tvItem.Selected += TvItem_Selected;
                tvItem.KeyDown += TvItem_KeyDown;

                MyTreeView.Items.Add(tvItem);

                tvItem = null;
             

        }



        //메인윈도우에서 이미지 델리트키로 지우면 여기 리스트윈도우창의 트리뷰 안의 내용도 지워짐.
        public void DeleteList(ObjConvertImageInfo ObjConvertimgInfo) //메인창에서 이 함수를 호출하게 되면 
        {
            string mainObjimg = ObjConvertimgInfo.ObjectName;
            

            for (int i = 0; i < MyTreeView.Items.Count; i++)
            {
                TreeViewItem tvi  = (TreeViewItem)MyTreeView.Items[i];
                ObjConvertImageInfo oci = (ObjConvertImageInfo)tvi.Tag;
                

                if (mainObjimg == oci.ObjectName)
                {

                    MyTreeView.Items.Remove(tvi);

                }
            }                            
        }






        //메인윈도우로 문자열값 전달하여 메인윈도우에서 전달받은 해당 문자열과 매칭되는 이미지를 찾아서 지움.
        public void TvItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (Selected_ObjItemName != null) //선택한 게 있을 때
            {
                if (e.Key == Key.Delete) //델리트키 눌렀을 시
                {

                   TreeViewItem tvi = sender as TreeViewItem;


                    ObjConvertImageInfo oci = (ObjConvertImageInfo)tvi.Tag;                                        
                    call_mainwindow.treeviewTomainwindow_ReceiveDelimg(oci.ObjectName); //메인윈도우로 문자열값 전달하여 메인윈도우에서 전달받은 해당 문자열과 매칭되는 이미지를 찾아서 지움.



                    MyTreeView.Items.Remove(sender);


                   // Selected_ObjItemName = null;
                   // Selected_ObjType = null;





                }
            }
        }



        string Selected_ObjItemName; //선택된 오브젝트의 내부이름
        string Selected_ObjType; //선택된 오브젝트의 타입
        public void TvItem_Selected(object sender, RoutedEventArgs e)
        {

            TreeViewItem tvi = e.Source as TreeViewItem;

            ObjConvertImageInfo objconvertimginfo;
            objconvertimginfo = (ObjConvertImageInfo)tvi.Tag;
           

            Selected_ObjItemName = objconvertimginfo.ObjectName;
            Selected_ObjType = objconvertimginfo.ObjectType;

            //if (tvi != null)
            //{
            //    MessageBox.Show( string.Format("{0} : tvi헤더", tvi.Header.ToString())   ); //비주얼네임
            //    MessageBox.Show( string.Format("{0} : 선택된 오브젝트네임", Selected_ObjItemName)    ); //내부 이름.
            //}        

        }

      





        /* 구버전

             public void ReceiveAppendedList(ObjConvertImageInfo ObjConvertimgInfo) //메인창에서 이 함수를 호출하게 되면 리스트 윈도우에 있는 트리뷰에 해당 내용을 추가한다.
       {

           string VisualName = ObjConvertimgInfo.VisualName;
           string ObjType = ObjConvertimgInfo.ObjectType;
           string ObjName = ObjConvertimgInfo.ObjectName;





           //오브젝트타입으로 트리뷰아이템을 추가하는데 해당트리뷰아이템 이름이 없다면 최상위에 트리뷰아이템(대분류)를 추가한 후 하위분류를 추가, 해당트리뷰가 있다면 이미 있는 대분류에 하위분류 추가

           //  < TreeViewItem Name = "desk" Header = "책상상" >

           TreeViewItem tvItem = new TreeViewItem();
           if (MyTreeView.Items.Count == 0) //내 트리뷰에 아이템이 하나도 없으면
           {

               //대분류 생성 후 대분류 및에 소분류 넣음
               TreeViewItem AppendUpperCategori = new TreeViewItem();
               AppendUpperCategori.Name = ObjType;
               //ObjType을 주고 한글로 반환
               AppendUpperCategori.Header = ObjType;
               MyTreeView.Items.Add(AppendUpperCategori); //상위 카테고리에는 이름과 헤더에 모두 오브젝트타입이 들어감.



               tvItem.Header = VisualName;
               tvItem.Tag = ObjConvertimgInfo;
               tvItem.Selected += TvItem_Selected;
               tvItem.KeyDown += TvItem_KeyDown;

               AppendUpperCategori.Items.Add(tvItem);

               tvItem = null;
               return;
           }


           if(MyTreeView.Items.Count > 0) //이미 MyTreeView에 TreeviewItem이 하나라도 있을 경우(카운트가 1이상일때)
           {
               foreach (var mtvi in MyTreeView.Items)
               {
                   if (mtvi is TreeViewItem AppendUpperCate)
                   {
                       if (AppendUpperCate.Name == ObjType) //같은 문자열이 상위 카테고리에 있으면, 상위카테고리를 생성하지 않고 해당 하테고리에 넣는다.
                       {

                           tvItem.Header = VisualName;
                           tvItem.Tag = ObjConvertimgInfo;
                           tvItem.Selected += TvItem_Selected;
                           tvItem.KeyDown += TvItem_KeyDown;

                           AppendUpperCate.Items.Add(tvItem);
                           tvItem = null;

                           return;

                       }                       
                   }                     
               }


               //foreach문에서 이곳으로 내려왔단 소리는 같은 문자열이 없다는 뜻. 그러므로 상위 카테고리 생성하고 그 하위에 tvitem을 넣는다.
               //대분류 생성 후 대분류 및에 소분류 넣음
               TreeViewItem CreateUpperCate = new TreeViewItem();
               CreateUpperCate.Name = ObjType;
               //ObjType을 매개변수로 주고 한글로 반환을 추가하면 어떨까?
               CreateUpperCate.Header = ObjType;
               MyTreeView.Items.Add(CreateUpperCate); //상위 카테고리에는 이름과 헤더에 모두 오브젝트타입이 들어감.


               tvItem.Header = VisualName;
               tvItem.Tag = ObjConvertimgInfo;
               tvItem.Selected += TvItem_Selected;
               tvItem.KeyDown += TvItem_KeyDown;

               CreateUpperCate.Items.Add(tvItem);

               tvItem = null;

           }


            */




    }
}
