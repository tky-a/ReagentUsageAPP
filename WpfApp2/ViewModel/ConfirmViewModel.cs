using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfApp2.Models;

namespace WpfApp2.ViewModels
{
    public partial class ConfirmViewModel : ObservableObject
    {
        public ObservableCollection<InputSet> PendingUsages { get; } = new ObservableCollection<InputSet>();

        [ObservableProperty]
        private InputSet selectedUsage;

        public ConfirmViewModel(MainViewModel parent)
        {

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
            // DatabaseHelper.SaveUsageHistory(PendingUsages);

            PendingUsages.Clear();
        }
    }
}
