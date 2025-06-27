using System;
using System.Collections.Generic;
using System.ComponentModel;
using WpfApp2.Models;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.IO;

namespace WpfApp2.ViewModels
{
    public class CoverViewModel
    {
        public RelayCommand GoToRegisterModeCommand { get; }

        public CoverViewModel(MainViewModel parent)
        {
            GoToRegisterModeCommand = new RelayCommand(() => parent.NavigateToRegisterMode());
        }
    }
}
