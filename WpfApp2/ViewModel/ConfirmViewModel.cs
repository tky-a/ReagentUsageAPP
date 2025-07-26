using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using WpfApp2.Models;
using WpfApp2.ViewModel;
using WpfApp2.Views;

namespace WpfApp2.ViewModels
{
    public partial class ConfirmViewModel : ObservableObject, INotifyPropertyChanged
    {
        private readonly MainViewModel _parent;

        [ObservableProperty]
        private InputSet selectedUsage;

        public ObservableCollection<InputSet> PendingUsages { get; } = new();
        
        public ConfirmViewModel(ObservableCollection<InputSet> inputSets,
               MainViewModel parent)
        {
            _parent = parent;
            foreach (var inputSet in inputSets)
            {
                if (inputSet != null && !PendingUsages.Contains(inputSet))
                {
                    PendingUsages.Add(inputSet);
                }
            }
        }

        [RelayCommand]
        private void GoToMainPage()
        {
            _parent.NavigateToCover();
        }

        [RelayCommand()]//CanExecute = nameof(CanDelete))]
        private void DeleteChecked()
        {
            var toDelete = PendingUsages.Where(x => x.IsChecked).ToList();
            if (toDelete.Count == 0)
            {
                MessageBox.Show("チェックを入れたデータが削除されます。", "削除エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            foreach (var item in toDelete)
            {
                PendingUsages.Remove(item);
            }

            DeleteCheckedCommand.NotifyCanExecuteChanged();
            MessageBox.Show("選択されたデータを削除しました。", "削除完了", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private bool CanDelete() => true;

        [RelayCommand]
        private void EditSelected(Object? parameter)
        {
            if (parameter is InputSet input)
            {
                // 編集用ウィンドウを表示  
                var window = new EditInputSetWindow(input);
                window.Owner = Application.Current.MainWindow;

                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                window.ShowDialog();

            }
            else
            {
                MessageBox.Show("データが取得できませんでした。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ConfirmAll()
        {
            // データベースに書き込み
            foreach (var usage in PendingUsages)
            {
                _parent.Database.
                _parent.Database.SaveUsageHistory(usage);
                _parent.Database.UpdateChemicalAfterUsage(usage);
            }

            this.PendingUsages.Clear();
            _parent.InputSets.Clear();
            _parent.NavigateToCover();
        }

        //[RelayCommand]
        //private async Task EditSelected(InputSet inputSet)
        //{
        //    try
        //    {
        //        var db = new DatabaseManager();

        //        string title = "編集画面（自動保存）";

        //        var vm = new ReagentDetailViewModel(inputset.chemical, title, isReadOnly: false);

        //        vm.StorageLocations = new ObservableCollection<StorageLocation>(
        //            await db.GetAllStorageLocationsAsync());
        //        vm.Users = new ObservableCollection<User>(
        //            await db.GetAllUsersAsync());

        //        vm.SelectedStorageLocation = vm.StorageLocations.FirstOrDefault(
        //            loc => loc.LocationId == chemical.StorageLocationId);
        //        vm.SelectedUser = vm.Users.FirstOrDefault(
        //            user => user.UserId == chemical.LastUserId);


        //        var dialogcontent = new ReagentDetail
        //        {
        //            DataContext = vm
        //        };

        //        await ShowDialog(dialogcontent);

        //        //ShowSnackbarAction?.Invoke("薬品情報を更新しました。");
        //    }
        //    catch (Exception ex)
        //    {
        //        // エラーハンドリング
        //        System.Diagnostics.Debug.WriteLine($"ダイアログ表示エラー: {ex.Message}");
        //    }


        //}

        //private async Task ShowDialog(UserControl dialogContent)
        //{
        //    try
        //    {

        //        // DialogHostのIdentifierを指定してダイアログを表示
        //        var result = await DialogHost.Show(dialogContent, "MainDialog");
        //    }
        //    catch (Exception ex)
        //    {
        //        // DialogHost関連のエラーハンドリング
        //        System.Diagnostics.Debug.WriteLine($"DialogHost エラー: {ex.Message}");
        //    }
        //}
    }
}
