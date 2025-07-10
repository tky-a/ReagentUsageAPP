using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Models;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace WpfApp2.ViewModel
{
    public partial class DataBaseSettingViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Chemical> chemicals;

        public DataBaseSettingViewModel(DatabaseManager dataBaseManager)
        {
            LoadChemicalsAsync(dataBaseManager);
        }

        private async void LoadChemicalsAsync(DatabaseManager dataBaseManager)
        {
            try
            {
                Chemicals = await Task.Run(() => 
                dataBaseManager.GetAllChemicals());
                Mouse.OverrideCursor = Cursors.Wait;
            }
            finally
            {
                Task.Delay(500).Wait();
                Mouse.OverrideCursor = null;
            }
        }
    }
}
