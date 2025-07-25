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
using WpfApp2.Models;
using WpfApp2.ViewModel;

namespace WpfApp2.Views
{
    /// <summary>
    /// SettingMenuView.xaml の相互作用ロジック
    /// </summary>
    public partial class UserSettingView : UserControl
    {
        public UserSettingView(DatabaseManager db)
        {
            InitializeComponent();
            DataContext = new UserSettingViewModel(db);
        }

        private void UserControlUnloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserSettingViewModel vm)
            {
                vm.SaveAllUsersToDatabase();
            }
        }
    }
}
