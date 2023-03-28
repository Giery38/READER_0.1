using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace READER_0._1.View
{
    public partial class MainWindow : Window
    {       
        public MainWindow()
        {
            InitializeComponent();            
        }
        private void ShowMainMenu_Button_Click(object sender, RoutedEventArgs e)
        {
            ShowMainMenu();
            GridMainMenu.IsHitTestVisible = true;
        }
        private void CloseMainMenu_Button_Click(object sender, RoutedEventArgs e)
        {
             CloseMainMenu();
             GridMainMenu.IsHitTestVisible = false;
        }
        private void ShowMainMenu_Completed(object sender, EventArgs e)
        { 
            if (GridMainMenu.IsMouseOver == false)
            {
                CloseMainMenu();
                GridMainMenu.IsHitTestVisible = false;
            }            
        }
        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {           
            CollapseTreeNodes(TreeViewMainMenu, false);
            double left = Canvas.GetLeft(MainMenuSwap);
            if (left == 0)
            {
                CloseMainMenu();
            }
            GridMainMenu.IsHitTestVisible = false;
        }
        private void CollapseTreeNodes(TreeView treeView, bool isExpanded)
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
        private void ShowMainMenu()
        {
            Storyboard storyboardShow = (Storyboard)MainGrid.Resources["ShowMainMenu"];
            storyboardShow.Begin();
        }
        private void CloseMainMenu()
        {
            Storyboard storyboard = (Storyboard)MainGrid.Resources["HideMainMenu"];
            storyboard.Begin();
        }
    }
}
