
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using WpfApp2.Views;
using WpfApp2.Models;
using CommunityToolkit.Mvvm.Input;

namespace WpfApp2.ViewModels
{
    public partial class SettingViewModel : ObservableObject, INotifyPropertyChanged
    {
        private readonly DatabaseManager _dataBaseManager;
        private readonly MainViewModel _parent;

        private string _homeReturnName = "ホームに戻る";
        private string _generalSettingName = "一般設定";
        private string _userSettingName = "使用者設定";
        private string _locationSettingName = "場所設定";
        private string _reagentSettingName = "薬品一覧編集";
        private string _scaleSettingName = "はかり設定";

        public ObservableCollection<SettingItem> Settings { get; }

        [ObservableProperty] private string name;
        [ObservableProperty] private string icon;
        [ObservableProperty] private SettingItem currentSetting;
        [ObservableProperty] private object currentSettingView;

        [ObservableProperty] private bool _isMenuExpanded = false;

        public ICommand GoBackCommand { get; }
        public ICommand ToggleMenuExpandedCommand { get; }


        public SettingViewModel(MainViewModel parent, DatabaseManager databaseManager)
        {
            _dataBaseManager = databaseManager;
            
            _dataBaseManager.EnsureTablesCreated();

            _parent = parent;
            Settings = new ObservableCollection<SettingItem>
            {
                new SettingItem { Name = " " },//, Icon = "\uE700" },
                new SettingItem { Name = _homeReturnName, Icon = "\uE80F" },
                //new SettingItem { Name = _generalSettingName, Icon = "\uF78C" },//一般設定
                new SettingItem { Name = _userSettingName, Icon = "\uE716" },//使用者設定
                //new SettingItem { Name = _locationSettingName, Icon = "\uE716" },//場所設定
                new SettingItem { Name = _reagentSettingName, Icon = "\uE71D" },//薬品一覧
                new SettingItem { Name = _scaleSettingName, Icon = "\uEE6F" }//RS232C設定
            };
            GoBackCommand = new RelayCommand(() => parent.NavigateToCover());
            CurrentSetting = Settings[0];
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
            else if(CurrentSetting?.Name == _homeReturnName)
            {
                _parent.NavigateToCover();
                CurrentSettingView = null;
            }
            else
            {
                CurrentSettingView = null;
            }
        }
    }
}
