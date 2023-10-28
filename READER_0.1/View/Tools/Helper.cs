using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace READER_0._1.View.Tools
{
    public class Helper
    {
        public static void OpacityChange(FrameworkElement frameworkElement, double start, double end, double step)
        {
            if (end > start)
            {
                for (double i = start; i < end; i += step)
                {
                    frameworkElement.Opacity = i;
                }
            }    
            else
            {
                for (double i = start; i >= end; i -= step)
                {
                    frameworkElement.Opacity = i;
                }
            }
        }
        public static T FindChild<T>(DependencyObject parent, string childName) where T : FrameworkElement
        {
            if (parent == null) return null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild && typedChild.Name == childName)
                {
                    return typedChild;
                }

                T foundChild = FindChild<T>(child, childName);
                if (foundChild != null)
                    return foundChild;
            }

            return null;
        }
        public static List<T> FindChild<T>(DependencyObject parent) where T : FrameworkElement
        {
            List<T> result = new List<T>();
            if (parent == null)
            {
                return result;
            }                          
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                {
                    result.Add(child as T);
                }
                result.AddRange(FindChild<T>(child));
            }

            return result;
        }
        public static void CollapseTreeNodes(TreeView treeView, bool isExpanded)
        {
            foreach (object node in treeView.Items)
            {
                TreeViewItem item = node as TreeViewItem;
                if (item != null)
                {
                    item.IsExpanded = isExpanded;
                }
            }
        }
    }
}
