using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using WpfApp2.Models;

namespace WpfApp2.ViewModel
{
    public partial class UserSettingViewModel : ObservableObject
    {
        private readonly DatabaseManager _dataBaseManager;
        [ObservableProperty] private ObservableCollection<User> allUsers;
        [ObservableProperty] private int number;
        [ObservableProperty] private int userId;
        [ObservableProperty] private string userName;

        public UserSettingViewModel(DatabaseManager dataBaseManager)
        {
            _dataBaseManager = dataBaseManager;

            AllUsers = dataBaseManager.GetAllUsers();

            AllUsers.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                int maxId = AllUsers.Any() ? AllUsers.Max(u => u.UserId) : 0;
                foreach (User user in e.NewItems)
                {
                    user.UserId = maxId + 1;
                    maxId++;

                    //_dataBaseManager.AddUser(user);
                }
            }
        }

        public void SaveAllUsersToDatabase()
        {
            foreach (var user in AllUsers)
            {
                if (!string.IsNullOrWhiteSpace(user.UserName))
                {
                    _dataBaseManager.AddUser(user);
                }
            }
        }
    }
}
