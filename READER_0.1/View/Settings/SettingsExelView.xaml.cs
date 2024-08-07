﻿using READER_0._1.Command.CommandExcel;
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

namespace READER_0._1.View.Settings
{
    /// <summary>
    /// Логика взаимодействия для SettingsExcelView.xaml
    /// </summary>
    public partial class SettingsExcelView : UserControl
    {   
        public SettingsExcelView()
        {
            InitializeComponent();
        }

        private void InputConfigurationsNameButton_Click(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(InputConfigurationsNameBox, 1);
            Panel.SetZIndex(ConfigurationsNameFragmentedList, 2);
        }       
    }
}
