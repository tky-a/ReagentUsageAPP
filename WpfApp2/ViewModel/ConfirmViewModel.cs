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
    public partial class ConfirmViewModel : ObservableObject
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
        [RelayCommand(CanExecute = nameof(CanDelete))]
        private void DeleteSelected()
        {
            if (SelectedUsage != null)
                PendingUsages.Remove(SelectedUsage);
        }


        private bool CanDelete() => SelectedUsage != null;

        [RelayCommand]
        private void ConfirmAll()
        {
            // TODO: UsageHistory に書き込む処理をここに追加
            this.PendingUsages.Clear();
            _parent.InputSets.Clear();
            _parent.CurrentViewModel = new CoverViewModel(_parent);
        }


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
    }
}
