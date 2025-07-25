using System;
using System.Collections.Generic;
using System.ComponentModel;
using WpfApp2.Models;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.IO;

namespace WpfApp2.ViewModels
{
    public class ChemicalViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseManager _db = new();

        private string _inputId;
        public string InpuId
        {
            get => _inputId;
            set
            {
                _inputId = value;
                OnPropertyChanged();
            }
        }

        private Chemical _selectedChemical = new();
        public Chemical SelectedChemical
        {
            get => _selectedChemical;
            set
            {
                _selectedChemical = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchCommand { get; }

        public ChemicalViewModel()
        {
            SearchCommand = new 
                RelayCommand(() => Search());
        }

        private void Search()
        {
            if(int.TryParse(_inputId, out int id))
            {
                var result = _db.GetChemicalById(id);
                SelectedChemical = result ?? new Chemical
                {
                    Name = "見つかりません",
                    Class = "",
                    CurrentMass = 0,
                    UseStatus = "",
                    FirstDate = DateTime.MinValue
                };
            }
            else
            {
                SelectedChemical = new Chemical
                {
                    Name = "無効なID",
                    Class = "",
                    CurrentMass = 0,
                    UseStatus = "",
                    FirstDate = DateTime.MinValue
                };
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
