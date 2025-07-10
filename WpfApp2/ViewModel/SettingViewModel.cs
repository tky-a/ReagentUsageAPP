using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using WpfApp2.Views;
using WpfApp2.Models;

namespace WpfApp2.ViewModels
{
    public partial class SettingViewModel : ObservableObject, INotifyPropertyChanged
    {
        private readonly DatabaseManager _dataBaseManager;

        private string _generalSettingName = "一般設定";
        private string _userSettingName = "使用者設定";
        private string _reagentSettingName = "薬品一覧編集";
        private string _scaleSettingName = "はかり設定";

        public ObservableCollection<SettingItem> Settings { get; }

        [ObservableProperty] private string name;
        [ObservableProperty] private string icon;
        [ObservableProperty] private SettingItem currentSetting;
        [ObservableProperty] private object currentSettingView;

        public ICommand GoBackCommand { get; }

        public SettingViewModel(MainViewModel parent, DatabaseManager databaseManager)
        {
            _dataBaseManager = databaseManager;
            Settings = new ObservableCollection<SettingItem>
            {
                new SettingItem { Name = _generalSettingName, Icon = "\uF78C" },
                new SettingItem { Name = _userSettingName, Icon = "\uE716" },
                new SettingItem { Name = _reagentSettingName, Icon = "\uE71D" },
                new SettingItem { Name = _scaleSettingName, Icon = "\uEE6F" }
            };
            GoBackCommand = new RelayCommand(() => parent.NavigateToCover());
            currentSetting = Settings[0];
            UpdateCurrentSettingView();

        }

        partial void OnCurrentSettingChanged(SettingItem value)
        {
            UpdateCurrentSettingView();
        }


        private void UpdateCurrentSettingView()
        {
            if(CurrentSetting?.Name == _generalSettingName)
            {
                CurrentSettingView = new GeneralSettingView();

            }
            else if(CurrentSetting?.Name == _userSettingName)
            {
                CurrentSettingView = new UserSettingView(_dataBaseManager);

            }
            else if(CurrentSetting?.Name == _reagentSettingName)
            {
                CurrentSettingView = new DatabaseSettingView(_dataBaseManager);

            }
            else if(CurrentSetting?.Name == _scaleSettingName)
            {
                CurrentSettingView = new ScaleSettingView();

            }
            else
            {
                CurrentSettingView = null;
            }
        }
    }
}
