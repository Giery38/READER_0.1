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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button ShowMainMenuButton = this.FindName("ShowMainMenu_button") as Button;
            ShowMainMenuButton.IsHitTestVisible = false;
            Storyboard storyboardShow = (Storyboard)MainGrid.Resources["ShowMainMenu"];
            storyboardShow.Begin();
            GridMainMenu.IsHitTestVisible = true;
        }
        private void ShowMainMenu_Completed(object sender, EventArgs e)
        {
            Button ShowMainMenuButton = this.FindName("ShowMainMenu_button") as Button;
            ShowMainMenuButton.IsHitTestVisible = false;
            Grid GridMainMenu = this.FindName("GridMainMenu") as Grid;
            if (GridMainMenu.IsMouseOver == false)
            {
                Storyboard storyboard = (Storyboard)MainGrid.Resources["HideMainMenu"];
                storyboard.Begin();
                ShowMainMenuButton.IsHitTestVisible = true;
                GridMainMenu.IsHitTestVisible = false;
            }            
        }
        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            TreeView TreeViewMainMenu = this.FindName("TreeViewMainMenu") as TreeView;
            CollapseTreeNodes(TreeViewMainMenu, false);
            Grid MainMenuSwap = this.FindName("MainMenuSwap") as Grid;
            double left = Canvas.GetLeft(MainMenuSwap);
            if (left == 0)
            {
                Button ShowMainMenuButton = this.FindName("ShowMainMenu_button") as Button;
                ShowMainMenuButton.IsHitTestVisible = true;
                Storyboard storyboard = (Storyboard)MainGrid.Resources["HideMainMenu"];
                storyboard.Begin();
                GridMainMenu.IsHitTestVisible = false;
            }
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
    }
}
