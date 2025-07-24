using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using WpfApp2.Models;
using WpfApp2.ViewModels;

namespace WpfApp2.ViewModel
{
    public partial class ReagentDetailViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<StorageLocation> storageLocations = new();

        [ObservableProperty]
        private ObservableCollection<User> users = new();


        [ObservableProperty] private string title;
        [ObservableProperty] private Chemical chemical;
        [ObservableProperty] private bool isReadOnly;
        [ObservableProperty] private StorageLocation? selectedStorageLocation;
        [ObservableProperty] private User? selectedUser;


        public Action? CloseDialogAction { get; set; }

        public DatabaseManager databaseManager;

        public ReagentDetailViewModel(Chemical chemical, string title, bool isReadOnly = false)
        {
            Title = title;
            IsReadOnly = isReadOnly;
            Chemical = chemical;
            databaseManager = new DatabaseManager();
        }

        [RelayCommand]
        private async Task Close()
        {
            if (!IsReadOnly)
            {
                Chemical.StorageLocationId = SelectedStorageLocation?.LocationId ?? 0;
                Chemical.LastUserId = SelectedUser?.UserId ?? null;           
            
                await UpdateChemicalAsync();
            }

            DialogHost.Close("MainDialog");
        }

        public async Task UpdateChemicalAsync()
        {
            if (Chemical == null) return;
            try
            {
                await databaseManager.UpdateChemicalDataBase(Chemical);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新中にエラーが発生しました: {ex.Message}");
            }
        }

    }
}
