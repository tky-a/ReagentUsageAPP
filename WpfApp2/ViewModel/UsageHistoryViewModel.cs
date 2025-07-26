using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.Models;

namespace WpfApp2.ViewModel
{
    public partial class UsageHistoryViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<UsageHistory> usageHistories;

        public UsageHistoryViewModel(DatabaseManager databaseManager)
        {
            //var histories = databaseManager.GetAllUsageHistory();
            //MessageBox.Show("" + histories.Count + "件の使用履歴が取得されました。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
            try
            {
                UsageHistories = databaseManager.GetAllUsageHistory();
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        [RelayCommand]
        private void Export()
        {
            if(UsageHistories == null || UsageHistories.Count==0)
            {
                MessageBox.Show("出力するデータがありません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "CSVファイル(*.csv)|*.csv",
                FileName = $"使用履歴_{DateTime.Now:yyyyMMdd}.csv"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    using var writer = new StreamWriter(dialog.FileName, false, Encoding.UTF8);

                    // ヘッダー行
                    writer.WriteLine("手続き日付,手続き,手続き者名,薬品番号,薬品名,貸出質量,返却質量,使用量");

                    foreach (var history in UsageHistories)
                    {
                        var line = string.Join(",",
                            history.ActionDate.ToString("yyyy/MM/dd"),
                            EscapeCsv(history.ActionType),
                            EscapeCsv(history.UserName),
                            history.ChemicalId,
                            history.ChemicalName,
                            history.MassBefore,
                            history.MassAfter,
                            history.MassChange
                        );
                        writer.WriteLine(line);
                    }

                    MessageBox.Show("CSVファイルを出力しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("CSVの出力に失敗しました。\n" + ex.Message, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Contains(',') || value.Contains('"') || value.Contains('\n')
                ? $"\"{value.Replace("\"", "\"\"")}\""
                : value;
        }
    }
}
