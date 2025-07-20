using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WpfApp2.ViewModel;



namespace WpfApp2.Models
{
    public class Chemical : INotifyPropertyChanged
    {
        private int _chemicalId;
        private string _name;
        private string _class;
        private decimal _currentMass;
        private string _useStatus;
        private int _storageLocationId;
        private string _locationName;
        private int? _lastUserId;
        private string _lastUserName;
        private DateTime? _lastUseDate;
        private DateTime _firstDate;
        private string _note;


        public int ChemicalId
        {
            get => _chemicalId;
            set { _chemicalId = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Class
        {
            get => _class;
            set { _class = value; OnPropertyChanged(); }
        }

        public decimal CurrentMass
        {
            get => _currentMass;
            set { _currentMass = value; OnPropertyChanged(); }
        }

        public string UseStatus
        {
            get => _useStatus;
            set { _useStatus = value; OnPropertyChanged(); }
        }

        public int StorageLocationId
        {
            get => _storageLocationId;
            set { _storageLocationId = value; OnPropertyChanged(); }
        }

        public string LocationName
        {
            get => _locationName;
            set { _locationName = value; OnPropertyChanged(); }
        }

        public int? LastUserId
        {
            get => _lastUserId;
            set { _lastUserId = value; OnPropertyChanged(); }
        }

        public string LastUserName
        {
            get => _lastUserName;
            set { _lastUserName = value; OnPropertyChanged(); }
        }

        public DateTime? LastUseDate
        {
            get => _lastUseDate;
            set { _lastUseDate = value; OnPropertyChanged(); }
        }

        public DateTime FirstDate
        {
            get => _firstDate;
            set { _firstDate = value; OnPropertyChanged(); }
        }
        public string Note
        {
            get => _note;
            set { _note = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class User : INotifyPropertyChanged
    {
        private int _userId;
        private string _userName;

        public int UserId
        {
            get => _userId;
            set { _userId = value; OnPropertyChanged(); }
        }

        public string UserName
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class StorageLocation : INotifyPropertyChanged
    {
        private int _locationId;
        private string _locationName;

        public int LocationId
        {
            get => _locationId;
            set { _locationId = value; OnPropertyChanged(); }
        }

        public string LocationName
        {
            get => _locationName;
            set { _locationName = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class UsageRecord : INotifyPropertyChanged
    {
        private DateTime _actionDate;
        private string _actionType;
        private decimal _massBefore;
        private decimal _massAfter;
        private decimal _usageAmount;
        private string _notes;
        private string _userName;
        private string _chemicalName;

        public DateTime ActionDate
        {
            get => _actionDate;
            set { _actionDate = value; OnPropertyChanged(); }
        }

        public string ActionType
        {
            get => _actionType;
            set { _actionType = value; OnPropertyChanged(); }
        }

        public decimal MassBefore
        {
            get => _massBefore;
            set { _massBefore = value; OnPropertyChanged(); }
        }

        public decimal MassAfter
        {
            get => _massAfter;
            set { _massAfter = value; OnPropertyChanged(); }
        }

        public decimal UsageAmount
        {
            get => _usageAmount;
            set { _usageAmount = value; OnPropertyChanged(); }
        }

        public string Notes
        {
            get => _notes;
            set { _notes = value; OnPropertyChanged(); }
        }

        public string UserName
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(); }
        }

        public string ChemicalName
        {
            get => _chemicalName;
            set { _chemicalName = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class InputSet : ObservableObject
    {
        public int InputReagentId { get; set; }
        public string InputReagentName { get; set; }
        public int InputUserId { get; set; }
        public string ActionType { get; set; }
        public decimal? MassBefore { get; set; }
        public decimal? MassAfter { get; set; }
        public decimal? MassChange => (MassBefore.HasValue && MassAfter.HasValue)
            ? MassBefore - MassAfter : null;
        public string Notes { get; set; }
        public string UserName { get; set; }

        [ObservableProperty]
        private bool isChecked;
    }

    public class SettingItem
    {
        public string Name { get; set; }
        public string Icon { get; set; }
    }

    public enum ThemeMode
    {
        Light,
        Dark,
        System
    }
    public class  ThemeOption
    {
        public ThemeMode Mode { get; set; }
        public string DisplayName { get; set; }
    }

}