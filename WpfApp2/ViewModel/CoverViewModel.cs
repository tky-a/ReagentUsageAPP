using System;
using System.Collections.Generic;
using System.ComponentModel;
using WpfApp2.Models;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.IO;
using System.Xml.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfApp2.Services;
using System.Windows;



namespace WpfApp2.ViewModels
{
    public partial class CoverViewModel : ObservableObject
    {
        private readonly MainViewModel _parent;
        private readonly ScaleSettingModel _scaleSetting;
        private readonly DatabaseManager _databaseManager;

        [ObservableProperty]
        private string buttonText = "登録モードへ";

        public CoverViewModel(MainViewModel parent, DatabaseManager db)
        {
            _parent = parent;
            _databaseManager = db;
        }

        [RelayCommand]
        private async void GoToRegisterMode()
        {
            ButtonText = "読み込み中...";

            _databaseManager.EnsureTablesCreated();

            var chemicals = _databaseManager.GetAllChemicals();
            var users = _databaseManager.GetAllUsers();

            if(chemicals == null || chemicals.Count == 0) 
            {
                MessageBox.Show("初期設定をお願いします。薬品一覧からCSVの出力、入力によりデータベースを作成してください。");
                _parent.NavigateToSettingMode();
                return;
            }
            //else if(users == null || users.Count == 0)
            //{
            //    MessageBox.Show("使用者を1人以上入力お願いします。");
            //    _parent.NavigateToSettingMode();
            //    return;
            //}

                await Task.Delay(500);

            _parent.NavigateToRegisterMode();
        }
        [RelayCommand]
        private void GoToSettingMode()
        {
            _parent.NavigateToSettingMode();
        }
    }
}
