using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nollan.Visual_Space
{
    public class MyTreeNode
    {
        
            public TreeViewItem _item = null;

            public TreeViewItem Item
            {
                get { return _item; }
                set { _item = value; }
            }
            public List<MyTreeNode> _children = new List<MyTreeNode>();

            public List<MyTreeNode> Children
            {
                get { return _children; }
                set { _children = value; }
            }

            private int _level = 0;

            public int Level
            {
                get { return _level; }
                set { _level = value; }
            }





        //This will return a custom tree, each item has a TreeViewItem and a children collection.
        public MyTreeNode GetMyTree(TreeView treeview)
        {
            MyTreeNode tree = new MyTreeNode();
            FillChild(treeview, tree);

            return tree;
        }

        public void FillChild(Visual current, MyTreeNode parent)
        {
            if (current == null) return;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(current); i++)
            {
                var child = VisualTreeHelper.GetChild(current, i) as Visual;
                if (child != null)
                {
                    if (child is TreeViewItem)
                    {
                        var childTreeViewItem = child as TreeViewItem;
                        MyTreeNode myChild = new MyTreeNode();
                        myChild.Level = parent.Level + 1;
                        myChild.Item = childTreeViewItem;
                        parent.Children.Add(myChild);

                        //Just for debug
                        Console.WriteLine(myChild.Level.ToString() + ":" + childTreeViewItem.Header.ToString());

                        FillChild(child as Visual, myChild);
                    }
                    else
                    {
                        FillChild(child as Visual, parent);
                    }
                }
            }
        }


    }



    



}
