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
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;

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
                    List<string> headerList = new List<string> { "使用状況","編集","最終使用者","最終使用日","削除" };

                    if (headerList.Contains(headerString))
                    {
                        continue;
                    }
                    if (column.Visibility == Visibility.Visible)
                    {
                        headers.Add(column.Header?.ToString() ?? "");
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
    }
}
