using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfApp2.Models;

namespace WpfApp2.ViewModels
{
    public class RegisterModeView2ViewModel : INotifyPropertyChanged
    {
        //private readonly DatabaseManager _databaseManager = new();
        private readonly MainViewModel _parent;
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

        //public ObservableCollection<InputSet> inputSets { get; } = new();
        //private InputSet _currentSet = new();
        public string CurrentInput { get; set; }
        


        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand NextCommand { get; }
        public ICommand ReturnCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand StartRecordingCommand { get; } // 記録開始コマンドを追加
        public ICommand SettingsCommand { get; } // 設定コマンドを追加
        public ICommand GoToMainPageCommand { get; } //表紙に戻る
              


        public RegisterModeView2ViewModel(ObservableCollection<InputSet> inputSets,DatabaseManager db, MainViewModel parent)
        {
            //_databaseManager.EnsureTablesCreated();

            InputSets = inputSets

            _parent = parent;

            // 初期設定
            HintText = "薬品IDをスキャン・入力";
            HelperText = "バーコードをスキャンするか入力します";
            ImgPanel = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Slide1.jpg"));

            // コマンドの初期化
            NextCommand = new RelayCommand(ExecuteNext, CanExecuteNext);
            ReturnCommand = new RelayCommand(ExecuteReturn, CanExecuteReturn);
            ConfirmCommand = new RelayCommand(ExecuteConfirm, CanExecuteConfirm);
            StartRecordingCommand = new RelayCommand(ExecuteStartRecording); // 記録開始コマンド
            SettingsCommand = new RelayCommand(ExecuteSettings); // 設定コマンド

            GoToMainPageCommand = new RelayCommand(() => _parent.NavigateToCover());
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

        /// <summary>
        /// 記録開始コマンドの実行
        /// </summary>
        private void ExecuteStartRecording()
        {
            // 記録画面の初期状態に設定
            PanelNumber = 1;
            InputText = string.Empty;
            ReagentCount = 0;
            SelectedChemical = null;

            // 初期画像とテキストを設定
            ImgPanel = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Slide1.jpg"));
            HintText = "薬品IDをスキャン・入力";
            HelperText = "バーコードをスキャンするか入力します";
            BtnNextContent = "次へ";
        }

        /// <summary>
        /// 設定コマンドの実行
        /// </summary>
        private void ExecuteSettings()
        {
            // 設定画面を開く処理（必要に応じて実装）
            // 例: 新しいウィンドウを開く、設定ダイアログを表示など
            System.Windows.MessageBox.Show("設定画面を開きます", "設定",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }


        private void ExecuteNext()
        {
            switch (PanelNumber)
            {
                case 1:
                    if (int.TryParse(InputText, out int chemicalId))
                    {
                        SelectedChemical = _databaseManager.GetChemicalById(chemicalId);

                        if (SelectedChemical != null)
                        {
                            _currentSet.InputReagentId = CurrentInput;
                            AdvanceToWeighingPanel();
                        }
                    }
                    break;
                case 2:
                    if (decimal.TryParse(InputText, out decimal mass))
                    {
                        SaveTemporaryRecord(mass, 0);

                        AdvanceToUserPanel();
                    }
                    break;
                case 3:
                    if (int.TryParse(InputText, out int userId))
                    {
                        // 一時保存処理
                        SaveTemporaryRecord(0,userId);

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
            // 最初のステップで戻るボタンが押された場合は表紙に戻る
            if (PanelNumber == 1)
            {
                return;
            }

            switch (PanelNumber)
            {
                case 2:
                    ReturnToScanPanel();
                    break;
                case 3:
                    ReturnToWeighingPanel();
                    break;
            }
            InputText = string.Empty;
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

        #endregion

        #region Private Methods

        private void AdvanceToWeighingPanel()
        {
            ImgPanel = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/weighing2.png"));
            HintText = "質量を送信・入力";
            HelperText = "はかりにより送信ボタンを押す必要があります";
            PanelNumber = 2;
        }

        private void AdvanceToUserPanel()
        {
            ImgPanel = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/user_scan.png"));
            HintText = "ユーザーIDを入力";
            HelperText = "数値を入力";
            BtnNextContent = "一時保存";
            PanelNumber = 3;
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
            ImgPanel = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/start.png"));
            HintText = "薬品IDをスキャン・入力";
            HelperText = "バーコードをスキャンするか入力します";
            BtnNextContent = "次へ";
            PanelNumber = 1;
        }

        private void ReturnToWeighingPanel()
        {
            ImgPanel = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/weighing2.png"));
            HintText = "質量を送信・入力";
            HelperText = "はかりにより送信ボタンを押す必要があります";
            BtnNextContent = "次へ";
            PanelNumber = 2;
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

        private void SaveTemporaryRecord(decimal mass, int userId)
        {
            if (userId == 0)
            {
                _currentSet.MassBefore = mass;
                
            }
            else
            {
                _currentSet.InputUserId = _databaseManager.GetUserNameById(userId).ToString();
            }
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