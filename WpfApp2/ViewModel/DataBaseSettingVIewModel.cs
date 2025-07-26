using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp2.Models;
using WpfApp2.Views;
using WpfApp2.ViewModel;

namespace WpfApp2.ViewModel
{
    public partial class DataBaseSettingViewModel : ObservableObject
    {

        private readonly string[] headerList = { "薬品番号", "薬品名", "毒劇危", "現在質量", "保管場所", "登録日" };

        private readonly DatabaseManager databaseManager = new();

        public Action<string>? ShowSnackbarAction { get; set; }


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
                Task.Delay(100).Wait();
                Mouse.OverrideCursor = null;
            }
        }

        [RelayCommand]
        private async Task Edit(Chemical chemical)
        {
            try
            {
                var db = new DatabaseManager();

                string title = "編集画面（自動保存）";

                var vm = new ReagentDetailViewModel(chemical,title,isReadOnly:false);

                vm.StorageLocations = new ObservableCollection<StorageLocation>(
                    await db.GetAllStorageLocationsAsync());
                vm.Users = new ObservableCollection<User>(
                    await db.GetAllUsersAsync());

                vm.SelectedStorageLocation = vm.StorageLocations.FirstOrDefault(
                    loc => loc.LocationId == chemical.StorageLocationId);
                vm.SelectedUser = vm.Users.FirstOrDefault(
                    user => user.UserId == chemical.LastUserId);


                var dialogcontent = new ReagentDetail
                {
                    DataContext = vm
                };

                await ShowDialog(dialogcontent);

                ShowSnackbarAction?.Invoke("薬品情報を更新しました。");
                LoadChemicalsAsync(db);
            }
            catch (Exception ex)
            {
                // エラーハンドリング
                System.Diagnostics.Debug.WriteLine($"ダイアログ表示エラー: {ex.Message}");
            }


        }

        private async Task ShowDialog(UserControl dialogContent)
        {
            try
            {
                
                // DialogHostのIdentifierを指定してダイアログを表示
                var result = await DialogHost.Show(dialogContent, "MainDialog");
            }
            catch (Exception ex)
            {
                // DialogHost関連のエラーハンドリング
                System.Diagnostics.Debug.WriteLine($"DialogHost エラー: {ex.Message}");
            }
        }


        [RelayCommand]
        private void Export(DataGrid dataGrid)
        {
            if (dataGrid == null || dataGrid.ItemsSource == null) return;

            try
            {
                var sb = new StringBuilder();
                var headers = new List<string>();

                foreach (var column in dataGrid.Columns)
                {
                    var headerString = column.Header as string;
                    //List<string> headerList = new List<string> { "使用状況","編集","最終使用者","最終使用日","削除" };

                    if (!headerList.Contains(headerString))
                    {
                        if (column.Visibility == Visibility.Visible)
                        {

                            headers.Add(column.Header?.ToString() ?? "");
                        }                    
                    }
                    else
                    {               
                        continue;
                    }
                }

                sb.AppendLine(string.Join(",", headers));



                string filePath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "薬品テンプレート.csv"
                );

                File.WriteAllLines(
                filePath,
                new[] { string.Join(",", headers) },
                Encoding.UTF8);

                MessageBox.Show(
                    $"薬品テンプレートをデスクトップに保存しました", //\nファイル名: {filePath}",
                    "エクスポート完了",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch { }

        }


        [ObservableProperty]
        private ObservableCollection<string> importLogs = new();

        [RelayCommand]
        private async Task Import()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "CSVファイル (*.csv)|*.csv",
                Title = "CSVファイルを選択してください"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var manager = new DatabaseManager();
                    manager.ImportChemicalsFromCsv(openFileDialog.FileName);
                    LoadChemicalsAsync(manager);
                    ImportLogs.Add("インポートが" +
                        "完了しました。");
                }
                catch (Exception ex)
                {
                    ImportLogs.Add($"エラー: {ex.Message}");
                }
            }
        }




    }
}
