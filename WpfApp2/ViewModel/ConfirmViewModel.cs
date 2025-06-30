using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfApp2.Models;

namespace WpfApp2.ViewModels
{
    public partial class ConfirmViewModel : ObservableObject
    {
        //public ObservableCollection<InputSet> PendingUsages { get; } = new ObservableCollection<InputSet>();

        [ObservableProperty]
        private InputSet selectedUsage;
        private ObservableCollection<InputSet> PendingUsages = new();

        public ConfirmViewModel(ObservableCollection<InputSet> inputSets, MainViewModel parent)
        {
            foreach(var inputSet in inputSets)
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
            // DatabaseHelper.SaveUsageHistory(PendingUsages);

            PendingUsages.Clear();
        }

        private void ShowDwtails(InputSet inputSet)
        {
            MessageBox.Show("TEST");
        }
    }
}
