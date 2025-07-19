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
        private readonly RS232CToUsbConnectionService _scaleService;

        [ObservableProperty]
        private string buttonText = "登録モードへ";

        public CoverViewModel(MainViewModel parent)
        {
            _parent = parent;
            _scaleService = new RS232CToUsbConnectionService();
        }

        [RelayCommand]
        private async void GoToRegisterMode()
        {
            ButtonText = "読み込み中...";
            
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
