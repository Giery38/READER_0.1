using Microsoft.WindowsAPICodePack.Dialogs;
using READER_0._1.Command;
using READER_0._1.Command.CommandMain;
using READER_0._1.Model;
using READER_0._1.Tools;
using READER_0._1.ViewModel.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using static READER_0._1.View.Elements.SwapWindow;

namespace READER_0._1.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly Navigation.Navigation navigation;
        private readonly WindowFileBase windowFileBase;
        public ViewModelBase CurrentViewModel => navigation.CurrentViewModel;
        public ICommand ShiftingInExcelViewModelCommand { get; }
        public ICommand ShiftingInWordViewModelCommand { get; }
        public ICommand ShiftingInSettingsViewModelCommand { get; }
        public ICommand AddNotificationCommand { get; }
        public ICommand ClearNotificationCountCommand { get; }
        public MainViewModel(WindowFileBase windowFileBase)
        {
            //ExcelViewModel excelViewModel = new ExcelViewModel(windowFileBase);
            WordViewModel wordViewModel = new WordViewModel(windowFileBase);
            navigation = new Navigation.Navigation();           
            this.windowFileBase = windowFileBase;          
            if (navigation.CurrentViewModel == null)
            {
                navigation.CurrentViewModel = wordViewModel;
                //создать homePage
            }
            ShiftingInExcelViewModelCommand = new NavigateCommand(new ExcelViewModel(windowFileBase), navigation);
            //ShiftingInExcelViewModelCommand = new NavigateCommand(excelViewModel, navigation);
            ShiftingInWordViewModelCommand = new NavigateCommand(wordViewModel, navigation); 
            //ShiftingInWordViewModelCommand = new NavigateCommand(new WordViewModel(windowFileBase), navigation);         
            ShiftingInSettingsViewModelCommand = new NavigateCommand(new SettingsViewModel(windowFileBase.settings), navigation);
            //
            navigation.CurrentViewModelChanged += Navigation_CurrentViewModelChanged;

            ClearNotificationCountCommand = new ClearNotificationCountCommand(this);
            NotificationManager.ShowNotificationEvent += CreateNotification;
            AddNotificationCommand = new AddNotificationCommand(this);            
            Notifications = new ObservableCollection<Notification>();
        }
        private int maxNotificationsCount = 10;
        public ObservableCollection<Notification> Notifications { get; private set; }      
        private int _notificationCount;
        private int notificationCount 
        {
            get
            {
                return _notificationCount;
            }
            set
            {               
                _notificationCount = value;
                if (_notificationCount > 10)
                {
                    NotificationCountValue = "10+";
                }
                else
                {
                    NotificationCountValue = value.ToString();
                }
                if (_notificationCount == 0)
                {
                    VisionNotificationCounter = 0;
                }
                else
                {
                    VisionNotificationCounter = 1;
                }
            }
        }
        private int visionNotificationCounter = 0;
        public int VisionNotificationCounter
        {
            get
            {
                return visionNotificationCounter;
            }
            private set
            {
                visionNotificationCounter = value;
                OnPropertyChanged(nameof(VisionNotificationCounter));
            }
        }
        private string notificationCountValue;
        public string NotificationCountValue
        {
            get
            {
                return notificationCountValue;
            }
            private set
            {
                notificationCountValue = value;
                OnPropertyChanged(nameof(NotificationCountValue));
            }
        }
        private Notification currentNotification;
        public Notification CurrentNotification
        {
            get
            {
                return currentNotification;
            }
            set
            {
                currentNotification = value;
                OnPropertyChanged(nameof(CurrentNotification));
            }
        }
        private ShowState notificationListState;
        public ShowState NotificationListState
        {
            get
            {
                return notificationListState;
            }
            set
            {
                notificationListState = value;
                OnPropertyChanged(nameof(NotificationListState));
            }
        }
        public void AddNotification(bool increasingNotificationCount)
        {            
            if (increasingNotificationCount == true && (NotificationListState == ShowState.Hide || NotificationListState == ShowState.Hiding))
            {
                notificationCount++;
            }
            if (Notifications.Count >= maxNotificationsCount)
            {
                Notifications.RemoveAt(0);
            }
            Notifications.Add(CurrentNotification);
        }
        public void ClearNotificationCount()
        {
            notificationCount = 0;
        }
        private double windowHeight;
        public double WindowHeight
        {
            get
            {
                return windowHeight;
            }
            set
            {
                windowHeight = value;
                OnPropertyChanged(nameof(WindowHeight));
            }
        }
        private void CreateNotification(object sender, EventArgs e)
        {
            CurrentNotification = sender as Notification;
        }
        private void Navigation_CurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }              
    }
}
