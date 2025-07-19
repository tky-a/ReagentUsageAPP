using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using WpfApp2.Models;
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
                _parent.Database.SaveUsageHistory(usage);
                _parent.Database.UpdateChemicalAfterUsage(usage);
            }

            this.PendingUsages.Clear();
            _parent.InputSets.Clear();
            _parent.CurrentViewModel = new CoverViewModel(_parent);
        }
    }
}
