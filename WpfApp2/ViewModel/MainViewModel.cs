﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Models;
using WpfApp2.Services;

namespace WpfApp2.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object currentViewModel;
        public List<User> Users { get; set; }

        public object CurrentViewModel
        {
            get => currentViewModel;
            set { currentViewModel = value; OnPropertyChanged(); }
        }

        public ObservableCollection<InputSet> InputSets { get; } = new();
        public DatabaseManager Database = new();

        

        public MainViewModel()
        {
            NavigateToCover();
        }

        public void NavigateToRegisterMode()
        {
            
            CurrentViewModel = new RegisterModeView2ViewModel(InputSets, Database, this);
        }

        public void NavigateToCover()
        {
            CurrentViewModel = new CoverViewModel(this, Database);
        }

        public void NavigateToConfim()
        {
            CurrentViewModel = new ConfirmViewModel(InputSets, this);
        }

        public void NavigateToSettingMode()
        {
            CurrentViewModel = new SettingViewModel(this, Database);
        }

        public void AddInputSet(InputSet inputSet)
        {
            if (inputSet != null && !InputSets.Contains(inputSet))
            {
                InputSets.Add(inputSet);
                OnPropertyChanged(nameof(InputSets));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
