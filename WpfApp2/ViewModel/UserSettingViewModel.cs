using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Models;

namespace WpfApp2.ViewModel
{
    public partial class UserSettingViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<User> allUsers;

        public UserSettingViewModel(DatabaseManager dataBaseManager)
        {
            AllUsers = dataBaseManager.GetAllUsers();
        }
    }
}
