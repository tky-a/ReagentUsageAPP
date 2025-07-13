using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfApp2.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfApp2.Services;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfApp2.Views;

namespace WpfApp2.ViewModels
{
    public partial class RegisterModeView2ViewModel : ObservableObject, INotifyPropertyChanged 
    {

        private readonly DatabaseManager _db;
        private readonly MainViewModel _parent;
        private InputSet _currentInputSet = new();

        private readonly RS232CToUsbConnectionService _connectionService;

        private int _panelNumber = 1;
        private int _reagentCount = 0;
        private string _inputText = string.Empty;
        private BitmapImage _imgPanel;
        private string _hintText;
        private string _helperText;
        private string _btnNextContent = "次へ";
        private Chemical _selectedChemical;
        private bool _isPoisonous;
        private bool _isDeleterious;
        private bool _isInUse;
        private UsageRecord _usageRecord = new();


        //あとで、こっちにする
        //[ObservableProperty]
        //private string inputText = string.Empty;
        //[ObservableProperty]
        //private Chemical _selectedChemical;


        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand NextCommand { get; }
        public ICommand ReturnCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand GoToMainPageCommand { get; } //表紙に戻る
              


        public RegisterModeView2ViewModel(
                ObservableCollection<InputSet> inputSets,
                DatabaseManager db,
                MainViewModel parent,          
                RS232CToUsbConnectionService connectionService)
        {
            _db = db;
            _parent = parent;
            _connectionService = connectionService;

            db.EnsureTablesCreated();

            // コマンドの初期化
            NextCommand = new RelayCommand(ExecuteNext, CanExecuteNext);
            ReturnCommand = new RelayCommand(ExecuteReturn, CanExecuteReturn);
            ConfirmCommand = new RelayCommand(ExecuteConfirm, CanExecuteConfirm);
            GoToMainPageCommand = new RelayCommand(() => _parent.NavigateToCover());

            // 初期設定
            HintText = "薬品IDをスキャン・入力";
            HelperText = "バーコードをスキャンするか入力します";
            ImgPanel = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/scan.png"));

        }

        #region Properties

        public int PanelNumber
        {
            get => _panelNumber;
            set
            {
                _panelNumber = value;
                OnPropertyChanged();
                UpdateHeaderVisuals();
            }
        }

        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged();
                ((RelayCommand)NextCommand).RaiseCanExecuteChanged();
            }
        }

        public BitmapImage ImgPanel
        {
            get => _imgPanel;
            set { _imgPanel = value; OnPropertyChanged(); }
        }

        public string HintText
        {
            get => _hintText;
            set { _hintText = value; OnPropertyChanged(); }
        }

        public string HelperText
        {
            get => _helperText;
            set { _helperText = value; OnPropertyChanged(); }
        }

        public int ReagentCount
        {
            get => _reagentCount;
            set
            {
                if(_reagentCount != value)
                {
                    _reagentCount = value;
                    OnPropertyChanged();
                    (ConfirmCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string BtnNextContent
        {
            get => _btnNextContent;
            set { _btnNextContent = value; OnPropertyChanged(); OnPropertyChanged(nameof(Chemical.Name));}
        }

        public string ChemicalName => SelectedChemical?.Name ?? string.Empty;

        public Chemical SelectedChemical
        {
            get => _selectedChemical;
            set
            {
                _selectedChemical = value;
                OnPropertyChanged();
                UpdateChemicalStatus();
            }
        }

        public bool IsPoisonous
        {
            get => _isPoisonous;
            set { _isPoisonous = value; OnPropertyChanged(); }
        }

        public bool IsDeleterious
        {
            get => _isDeleterious;
            set { _isDeleterious = value; OnPropertyChanged(); }
        }

        public bool IsInUse
        {
            get => _isInUse;
            set { _isInUse = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands

        private void ExecuteNext()
        {
            switch (PanelNumber)
            {
                case 1:
                    if (int.TryParse(InputText, out int chemicalId))
                    {
                        SelectedChemical = _db.GetChemicalById(chemicalId);

                        if (SelectedChemical != null)
                        {
                            _currentInputSet.InputReagentId = SelectedChemical.ChemicalId;
                            _currentInputSet.InputReagentName = SelectedChemical.Name;
                            AdvanceToWeighingPanel();                            
                        }
                    }
                    break;
                case 2:
                    if (decimal.TryParse(InputText, out decimal mass))
                    {
                        if(_selectedChemical.UseStatus == "貸出可能")
                        {
                            _currentInputSet.ActionType = "貸出";
                            _currentInputSet.MassBefore = mass;  
                        }
                        else
                        {
                            _currentInputSet.ActionType = "返却";
                            _currentInputSet.MassBefore = SelectedChemical.CurrentMass;
                            _currentInputSet.MassAfter = mass;
                        }
                        AdvanceToUserPanel();
                        
                    }
                    break;
                case 3:
                    if (int.TryParse(InputText, out int userId))
                    {
                        _currentInputSet.InputUserId = userId;

                        var user = _db.GetUserNameById(userId);
                        _currentInputSet.UserName = user?.UserName ?? userId.ToString();

                        _parent.AddInputSet(_currentInputSet);
                        _currentInputSet = new InputSet();
                        AdvanceToNextReagent();
                    }
                    break;
            }
            InputText = string.Empty;
        }

        private bool CanExecuteNext()
        {
            return !string.IsNullOrWhiteSpace(InputText);
        }

        private void ExecuteReturn()
        {
            switch (PanelNumber)
            {
                case 2:
                    ReturnToScanPanel();
                    InputText = _currentInputSet.InputReagentId.ToString();
                    break;
                case 3:
                    ReturnToWeighingPanel();
                    if(_currentInputSet.ActionType == "貸出")
                    {
                        InputText = _currentInputSet.MassBefore.ToString();
                    }
                    else
                    {
                        InputText = _currentInputSet.MassAfter.ToString();
                    }
                    break;
            }
        }

        private bool CanExecuteReturn()
        {
            return true;
        }

        private void ExecuteConfirm()
        {
            _parent.NavigateToConfim();

        }

        private bool CanExecuteConfirm()
        {
            return ReagentCount > 0;
        }

        [RelayCommand]
        public async Task LoadAndConnectAsync()
        {
            try
            {
                var config = await _connectionService.LoadSettingsAsync();
                _connectionService.Connect(config);
            }
            catch (Exception ex)
            {
                
            }
        }

        [RelayCommand]
        private async Task ShowChemicalDetail(Chemical selectedChemical)
        {
            if(selectedChemical != null)
            {
                //MessageBox.Show($"{SelectedChemical.Name}\n" +
                //                $"ID: {SelectedChemical.ChemicalId}\n" +
                //                $"質量: {SelectedChemical.CurrentMass} g\n" +
                //                $"クラス: {SelectedChemical.Class}\n" +
                //                $"使用状況: {SelectedChemical.UseStatus}\n" +
                //                $"保管場所: {SelectedChemical.LocationName}\n" +
                //                $"最終使用者: {SelectedChemical.LastUserName ?? "なし"}\n" +
                //                $"最終使用日: {SelectedChemical.LastUseDate?.ToShortDateString() ?? "なし"}\n",
                //                "薬品詳細");
                var dialogcontent = new ReagentDetail { DataContext = selectedChemical };
                await ShowTestDialog(dialogcontent);
            }
        }

        public async Task ShowTestDialog(UserControl dialogContent)
        {
            var result = await MaterialDesignThemes.Wpf.DialogHost.Show(dialogContent, "MainDialog");
        }

        #endregion



        #region Private Methods

        private async Task AdvanceToWeighingPanel()
        {
            ImgPanel = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/weighing2.png"));
            HintText = "質量を送信・入力";
            HelperText = "はかりにより送信ボタンを押す必要があります";
            PanelNumber ++;
            await LoadAndConnectAsync();
        }

        private void AdvanceToUserPanel()
        {
            ImgPanel = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/user_scan.png"));
            HintText = "ユーザーIDを入力";
            HelperText = "数値を入力";
            BtnNextContent = "一時保存";
            PanelNumber ++;
        }

        private void AdvanceToNextReagent()
        {
            ImgPanel = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/scan.png"));
            HintText = "続けて記録できます";
            HelperText = "バーコードをスキャンするか入力します";
            BtnNextContent = "次へ";
            PanelNumber = 1;
            ReagentCount++;
            SelectedChemical = null;
        }

        private void ReturnToScanPanel()
        {
            ImgPanel = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/scan.png"));            
            HintText = "薬品IDをスキャン・入力";
            HelperText = "バーコードをスキャンするか入力します";
            BtnNextContent = "次へ";
            PanelNumber--;
        }

        private void ReturnToWeighingPanel()
        {
            ImgPanel = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/weighing2.png"));
            HintText = "質量を送信・入力";
            HelperText = "はかりにより送信ボタンを押す必要があります";
            BtnNextContent = "次へ";
            PanelNumber--;
        }

        private void UpdateChemicalStatus()
        {
            if (SelectedChemical != null)
            {
                IsPoisonous = SelectedChemical.Class?.Contains("毒") ?? false;
                IsDeleterious = SelectedChemical.Class?.Contains("劇") ?? false;
                IsInUse = SelectedChemical.UseStatus == "貸出中";
            }
            else
            {
                IsPoisonous = false;
                IsDeleterious = false;
                IsInUse = false;
            }
        }

        private void UpdateHeaderVisuals()
        {
            // ヘッダーの視覚的状態更新は必要に応じてイベントを発生させる
            OnPropertyChanged(nameof(PanelNumber));
        }


        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    // RelayCommand実装
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object parameter)
        {
            _execute.Invoke();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}