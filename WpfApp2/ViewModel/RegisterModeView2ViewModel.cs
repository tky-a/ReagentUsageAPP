using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfApp2.Models;
using WpfApp2.Services;
using WpfApp2.ViewModel;
using WpfApp2.Views;



namespace WpfApp2.ViewModels
{
    public partial class RegisterModeView2ViewModel : ObservableObject, INotifyPropertyChanged 
    {

        private readonly DatabaseManager _db;
        private readonly MainViewModel _parent;
        private InputSet _currentInputSet = new();

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
        //private UsageRecord _usageRecord = new();

        private readonly SerialPortManager _spManager;
        [ObservableProperty]
        private string receivedData;
        [ObservableProperty]
        private string selectedPortName;
        [ObservableProperty]
        private int selectedBaudRateIndex;



        public RS232C MySerialCOM => _spManager.MySerialCOM;
        private void OnDataReceived(object sender, string data)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                ReceivedData += data;
            });
        }
        [RelayCommand]
        private void Connect()
        {
            try
            {

                selectedPortName = _spManager.ComPortScan();
                if (selectedPortName != "USBorNOT")
                {
                    try
                    {
                        if (string.IsNullOrEmpty(SelectedPortName))
                        {
                            return;
                        }

                        int baudRate = MySerialCOM.baudRateItems[0].rateValue;

                        _spManager.Initialize(SelectedPortName, baudRate);
                        _spManager.Open();
                        _spManager.StartListening();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"接続失敗: {ex.Message}");
                    }
                }
                else
                {  }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"接続でエラーが発生しました。{ex}");
            }
        }

        [RelayCommand]
        private void Disconnect()
        {
            try
            {
                _spManager.StopListening();
                _spManager.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"切断失敗: {ex.Message}");
            }
            finally
            {
                _spManager.Dispose();
                selectedPortName = null;
            }
        }


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
                MainViewModel parent          
                )
        {
            _db = db;
            _parent = parent;

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

            _spManager = SerialPortManager.Instance;
            _spManager.DataReceived += OnDataReceived;

            Connect();
            
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
            return PanelNumber >= 2;
        }

        private void ExecuteConfirm()
        {
            Disconnect();
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

            }
            catch (Exception ex)
            {
                
            }
        }


        [RelayCommand]
        private async Task ShowChemicalDetail()
        {

            if (SelectedChemical != null)
            {
                try
                {
                    var db = new DatabaseManager();

                    string title = "薬品詳細";

                    var vm = new ReagentDetailViewModel(SelectedChemical, title, isReadOnly: true);

                    vm.StorageLocations = new ObservableCollection<StorageLocation>(
                        await db.GetAllStorageLocationsAsync());
                    vm.Users = new ObservableCollection<User>(
                        await db.GetAllUsersAsync());

                    vm.SelectedStorageLocation = vm.StorageLocations.FirstOrDefault(
                        loc => loc.LocationId == SelectedChemical.StorageLocationId);
                    vm.SelectedUser = vm.Users.FirstOrDefault(
                        user => user.UserId == SelectedChemical.LastUserId);

                    var dialogcontent = new ReagentDetail
                    {
                        DataContext = vm
                    };

                    await ShowDialog(dialogcontent);
                }
                catch (Exception ex)
                {
                    // エラーハンドリング
                    System.Diagnostics.Debug.WriteLine($"ダイアログ表示エラー: {ex.Message}");
                }
            }
        }

        private async Task ShowDialog(UserControl dialogContent)
        {
            try
            {
                // DialogHostのIdentifierを指定してダイアログを表示
                var result = await DialogHost.Show(dialogContent, "MainDialog");          
            }
            catch (Exception ex)
            {
                // DialogHost関連のエラーハンドリング
                System.Diagnostics.Debug.WriteLine($"DialogHost エラー: {ex.Message}");
            }
        }

        #endregion




        #region Private Methods

        private async Task AdvanceToWeighingPanel()
        {
            ImgPanel = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/weighing2.png"));
            HintText = "質量を送信・入力";
            HelperText = "はかりにより送信ボタンを押す必要があります";
            PanelNumber++;
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

    }
        #endregion

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