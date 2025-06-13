using System;
using System.ComponentModel;

namespace WpfApp2.Models
{
    public class Chemical
    {
        public int ChemicalId { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public decimal CurrentMass { get; set; }
        public string UseStatus { get; set; }
        public int StorageLocationId { get; set; }
        public string LocationName { get; set; }
        public int? LastUserId { get; set; }
        public string LastUserName { get; set; }
        public DateTime? LastUseDate { get; set; }
        public DateTime FirstDate { get; set; }
    }

    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

    public class StorageLocation
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }
    }
    public class UsageRecord
    {
        public DateTime ActionDate { get; set; }
        public string ActionType { get; set; }
        public decimal MassBefore { get; set; }
        public decimal MassAfter { get; set; }
        public decimal UsageAmount { get; set; }
        public string Notes { get; set; }
        public string UserName { get; set; }
        public string ChemicalName { get; set; }

    }

    //古いほう。いずれは削除する。
    //public class DrugItem : INotifyPropertyChanged
    //{
    //    private string _itemId;
    //    private string _drugName;
    //    private double _totalCapacity;
    //    private double _currentWeight;
    //    private string _unit;
    //    private DateTime _registrationDate;
    //    private DateTime? _lendingDate;
    //    private DateTime? _returnDate;
    //    private string _userId;
    //    private bool _isLent;
    //    private string _notes;
    //    private string _location;
    //    private string _category;

    //    public event PropertyChangedEventHandler PropertyChanged;

    //    // 管理番号
    //    public string 管理番号
    //    {
    //        get => _itemId;
    //        set
    //        {
    //            if (_itemId != value)
    //            {
    //                _itemId = value;
    //                OnPropertyChanged(nameof(管理番号));
    //            }
    //        }
    //    }

    //    // 薬品名
    //    public string 薬品名
    //    {
    //        get => _drugName;
    //        set
    //        {
    //            if (_drugName != value)
    //            {
    //                _drugName = value;
    //                OnPropertyChanged(nameof(薬品名));
    //            }
    //        }
    //    }

    //    // 総容量
    //    public double TotalCapacity
    //    {
    //        get => _totalCapacity;
    //        set
    //        {
    //            if (_totalCapacity != value)
    //            {
    //                _totalCapacity = value;
    //                OnPropertyChanged(nameof(TotalCapacity));
    //                OnPropertyChanged(nameof(現在量));
    //            }
    //        }
    //    }

    //    // 現在の重量
    //    public double CurrentWeight
    //    {
    //        get => _currentWeight;
    //        set
    //        {
    //            if (_currentWeight != value)
    //            {
    //                _currentWeight = value;
    //                OnPropertyChanged(nameof(CurrentWeight));
    //                OnPropertyChanged(nameof(現在量));
    //            }
    //        }
    //    }

    //    // 現在量（表示用）
    //    public string 現在量
    //    {
    //        get => $"{_currentWeight:F1} {_unit}";
    //    }

    //    // 容量（表示用）
    //    public string 容量
    //    {
    //        get => $"{_totalCapacity:F1} {_unit}";
    //    }

    //    // 単位
    //    public string Unit
    //    {
    //        get => _unit;
    //        set
    //        {
    //            if (_unit != value)
    //            {
    //                _unit = value;
    //                OnPropertyChanged(nameof(Unit));
    //                OnPropertyChanged(nameof(現在量));
    //                OnPropertyChanged(nameof(容量));
    //            }
    //        }
    //    }

    //    // 登録日
    //    public DateTime RegistrationDate
    //    {
    //        get => _registrationDate;
    //        set
    //        {
    //            if (_registrationDate != value)
    //            {
    //                _registrationDate = value;
    //                OnPropertyChanged(nameof(RegistrationDate));
    //                OnPropertyChanged(nameof(登録日));
    //            }
    //        }
    //    }

    //    // 登録日（表示用）
    //    public string 登録日
    //    {
    //        get => _registrationDate.ToString("yyyy/MM/dd");
    //    }

    //    // 貸出日
    //    public DateTime? LendingDate
    //    {
    //        get => _lendingDate;
    //        set
    //        {
    //            if (_lendingDate != value)
    //            {
    //                _lendingDate = value;
    //                OnPropertyChanged(nameof(LendingDate));
    //                OnPropertyChanged(nameof(貸出日));
    //            }
    //        }
    //    }

    //    // 貸出日（表示用）
    //    public string 貸出日
    //    {
    //        get => _lendingDate?.ToString("yyyy/MM/dd HH:mm") ?? "";
    //    }

    //    // 返却日
    //    public DateTime? ReturnDate
    //    {
    //        get => _returnDate;
    //        set
    //        {
    //            if (_returnDate != value)
    //            {
    //                _returnDate = value;
    //                OnPropertyChanged(nameof(ReturnDate));
    //                OnPropertyChanged(nameof(返却日));
    //            }
    //        }
    //    }

    //    // 返却日（表示用）
    //    public string 返却日
    //    {
    //        get => _returnDate?.ToString("yyyy/MM/dd HH:mm") ?? "";
    //    }

    //    // ユーザーID
    //    public string UserId
    //    {
    //        get => _userId;
    //        set
    //        {
    //            if (_userId != value)
    //            {
    //                _userId = value;
    //                OnPropertyChanged(nameof(UserId));
    //            }
    //        }
    //    }

    //    // 貸出中フラグ
    //    public bool IsLent
    //    {
    //        get => _isLent;
    //        set
    //        {
    //            if (_isLent != value)
    //            {
    //                _isLent = value;
    //                OnPropertyChanged(nameof(IsLent));
    //                OnPropertyChanged(nameof(状態));
    //            }
    //        }
    //    }

    //    // 状態（表示用）
    //    public string 状態
    //    {
    //        get => _isLent ? "貸出中" : "在庫";
    //    }

    //    // 備考
    //    public string Notes
    //    {
    //        get => _notes;
    //        set
    //        {
    //            if (_notes != value)
    //            {
    //                _notes = value;
    //                OnPropertyChanged(nameof(Notes));
    //            }
    //        }
    //    }

    //    // 保管場所
    //    public string Location
    //    {
    //        get => _location;
    //        set
    //        {
    //            if (_location != value)
    //            {
    //                _location = value;
    //                OnPropertyChanged(nameof(Location));
    //            }
    //        }
    //    }

    //    // カテゴリ
    //    public string Category
    //    {
    //        get => _category;
    //        set
    //        {
    //            if (_category != value)
    //            {
    //                _category = value;
    //                OnPropertyChanged(nameof(Category));
    //            }
    //        }
    //    }

    //    public DrugItem()
    //    {
    //        _itemId = "";
    //        _drugName = "";
    //        _totalCapacity = 0;
    //        _currentWeight = 0;
    //        _unit = "g";
    //        _registrationDate = DateTime.Now;
    //        _lendingDate = null;
    //        _returnDate = null;
    //        _userId = "";
    //        _isLent = false;
    //        _notes = "";
    //        _location = "";
    //        _category = "";
    //    }

    //    protected virtual void OnPropertyChanged(string propertyName)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }

    //    // 残量の割合を計算
    //    public double GetRemainingPercentage()
    //    {
    //        if (_totalCapacity <= 0) return 0;
    //        return (_currentWeight / _totalCapacity) * 100;
    //    }

    //    // 残量が少ないかどうか判定
    //    public bool IsLowStock(double threshold = 20.0)
    //    {
    //        return GetRemainingPercentage() < threshold;
    //    }

    //    // 貸出処理
    //    public void LendTo(string userId)
    //    {
    //        UserId = userId;
    //        LendingDate = DateTime.Now;
    //        IsLent = true;
    //        ReturnDate = null;
    //    }

    //    // 返却処理
    //    public void Return(double returnWeight)
    //    {
    //        CurrentWeight = returnWeight;
    //        ReturnDate = DateTime.Now;
    //        IsLent = false;
    //        UserId = "";
    //    }
    //}
}