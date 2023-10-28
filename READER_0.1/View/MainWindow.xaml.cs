using READER_0._1.Model.Settings.Word;
using READER_0._1.Model;
using READER_0._1.Tools;
using READER_0._1.View.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
using System.Data;
using System.Threading.Tasks;
using System.DirectoryServices.ActiveDirectory;
using READER_0._1.Command.CommandExcel;
using READER_0._1.Command.CommandMain;
using READER_0._1.ViewModel;
using static READER_0._1.View.Elements.SwapWindow;
using READER_0._1.View.Elements;

namespace READER_0._1.View
{
    public partial class MainWindow : Window
    {       
        public MainWindow()
        {
            InitializeComponent();            
            NotificationManager.ShowNotificationEvent += CreateNotification;            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel)
            {
                MainViewModel mainViewModel = DataContext as MainViewModel;
                AddNotificationCommand = mainViewModel.AddNotificationCommand;
            }
            functions = Helper.FindChild<TreeView>(this)?.Find(item => item.Tag.Equals("FunctionsMainMenu") == true);
            notificationsListSwap = Helper.FindChild<SwapWindow>(this)?.Find(item => item.Tag?.Equals("NotificationsListSwap") == true);                   
        }
        public static readonly DependencyProperty AddNotificationCommandProperty =
          DependencyProperty.RegisterAttached(nameof(AddNotificationCommand), typeof(ICommand), typeof(MainWindow), new PropertyMetadata(null));
        public ICommand AddNotificationCommand
        {
            get
            {
                return (ICommand)GetValue(AddNotificationCommandProperty);
            }
            set
            {
                SetValue(AddNotificationCommandProperty, value);
            }
        }
        #region MainMenu      
        private TreeView functions;
        private void ShowMainMenu_Button_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Show();
            ShowMainMenu_Button.IsHitTestVisible = false;
        }        
        private void CloseMainMenu_Button_Click(object sender, RoutedEventArgs e)
        {
             MainMenu.Hide();
             ShowMainMenu_Button.IsHitTestVisible = true;                 
        }
        private void MainMenu_ShowStateChange(object sender, ShowState e)
        {
            if (e == ShowState.Opening || e == ShowState.Open)
            {
                Helper.OpacityChange(BlackoutGrid, 0, 0.5, 0.00001);
                ShowMainMenu_Button.IsHitTestVisible = false;
            }
            else
            {
                Helper.OpacityChange(BlackoutGrid, BlackoutGrid.Opacity, 0, 0.00001);
                ShowMainMenu_Button.IsHitTestVisible = true;
                Helper.CollapseTreeNodes(functions, false);
            }
        }                 
        #endregion
        #region Notification
        private (Notification value, bool sent) currentNotification;
        private Task notificationTask = null;        
        private CancellationTokenSource notificationCancelToken;
        private void CreateNotification(object sender, EventArgs e)
        {            
            if (notificationTask?.Status == TaskStatus.WaitingForActivation)
            {
                notificationCancelToken.Cancel();
                AddNotificationExecute(true);
            }
            NotificationSwap.Show();
            currentNotification.value = sender as Notification;
            currentNotification.sent = false;
            notificationCancelToken = new CancellationTokenSource();
            notificationTask = Task.Run(async delegate
            {
                await Task.Delay(TimeSpan.FromSeconds(currentNotification.value.TimeExistence), notificationCancelToken.Token);
                if (NotificationSwap.IsMouseOver == false)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        NotificationSwap.Hide();
                        AddNotificationCommand.Execute(true);
                    });
                }              
            });                                  
                     
        }              
        private void NotificationSwap_MouseLeave(object sender, MouseEventArgs e)
        {
            if (NotificationSwap.State == ShowState.Open && notificationTask?.Status == TaskStatus.RanToCompletion)
            {
                NotificationSwap.Hide();
            }
            AddNotificationExecute(false);           
        }
        private void CloseNotification_Button_Click(object sender, RoutedEventArgs e)
        {
            if (notificationTask?.Status == TaskStatus.WaitingForActivation)
            {
                notificationCancelToken.Cancel();
            }
            NotificationSwap.Hide();
            AddNotificationExecute(false);
        }
        private void AddNotificationExecute(bool param)
        {
            if (currentNotification.sent == false)
            {
                currentNotification.sent = true;
                AddNotificationCommand.Execute(param);
            }       
        }      

        #endregion
        #region  NotificationList 
        private SwapWindow notificationsListSwap;
        private void NotificationsList_Button_Click(object sender, RoutedEventArgs e)
        {
            if (notificationsListSwap.State == ShowState.Opening || notificationsListSwap.State == ShowState.Open)
            {
                notificationsListSwap.Hide();
            }
            else
            {
                notificationsListSwap.Show();                
            }           
        }       
        #endregion
        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;                  
            Process.Start("explorer.exe", path);            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {               
            Random rnd = new Random(); 
            Notification n = new Notification("tat ", rnd.Next(90000000,900000000).ToString(), NotificationManager.ErrorColor, 3);
            n.Show();
        }      
    }
}
