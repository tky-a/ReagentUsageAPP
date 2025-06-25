using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object currentViewModel;
        public object CurrentViewModel
        {
            get => currentViewModel;
            set { currentViewModel = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            var coverVM = new CoverViewModel(this);
            CurrentViewModel = coverVM;
        }
        public void NavigateToInput()
        {
            CurrentViewModel = new SettingViewModel(this);
        }

        public void NavigateToRegisterMode()
        {
            CurrentViewModel = new RegisterModeView2ViewModel(this);
        }

        public void NavigateToCover()
        {
            CurrentViewModel = new CoverViewModel(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
