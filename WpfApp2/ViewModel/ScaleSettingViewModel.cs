using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.Models;
using WpfApp2.Services;

namespace WpfApp2.ViewModel
{
    public partial class ScaleSettingViewModel : ObservableObject
    {
        private readonly RS232CToUsbConnectionService _connectionService;

        public ScaleSettingViewModel()
        {
            _connectionService = new RS232CToUsbConnectionService();
            _connectionService.ConnectionStatusChanged += isConnected =>
            {
                IsConnected = isConnected;
                ConnectCommand.NotifyCanExecuteChanged();
                DisconnectCommand.NotifyCanExecuteChanged();
                ConnectionStatus = IsConnected ? $"接続中: {SelectedComPort}" : "切断";
            };

            // 設定の選択肢を初期化
            ComPorts = new ObservableCollection<string>(SerialPort.GetPortNames());
            BaudRates = new ObservableCollection<int> { 9600, 19200, 38400, 57600, 115200 };
            DataBitsOptions = new ObservableCollection<int> { 7, 8 };
            ParityOptions = new ObservableCollection<Parity>((Parity[])Enum.GetValues(typeof(Parity)));
            StopBitsOptions = new ObservableCollection<StopBits>((StopBits[])Enum.GetValues(typeof(StopBits)));
            HandshakeOptions = new ObservableCollection<Handshake>((Handshake[])Enum.GetValues(typeof(Handshake)));

            // 非同期で設定を読み込む
            LoadSettingsAsync();
        }

        // --- Observable Properties for UI Binding ---
        [ObservableProperty]
        private bool _isConnected;

        [ObservableProperty]
        private string _connectionStatus = "切断";

        [ObservableProperty]
        private string? _selectedComPort;

        [ObservableProperty]
        private int _selectedBaudRate;

        [ObservableProperty]
        private int _selectedDataBits;

        [ObservableProperty]
        private Parity _selectedParity;

        [ObservableProperty]
        private StopBits _selectedStopBits;

        [ObservableProperty]
        private Handshake _selectedHandshake;

        // --- Collections for ComboBox ItemsSource ---
        public ObservableCollection<string> ComPorts { get; }
        public ObservableCollection<int> BaudRates { get; }
        public ObservableCollection<int> DataBitsOptions { get; }
        public ObservableCollection<Parity> ParityOptions { get; }
        public ObservableCollection<StopBits> StopBitsOptions { get; }
        public ObservableCollection<Handshake> HandshakeOptions { get; }

        /// <summary>
        /// 設定を非同期で読み込み、UIに反映します。
        /// </summary>
        private async Task LoadSettingsAsync()
        {
            var settings = await _connectionService.LoadSettingsAsync();

            // 利用可能なCOMポートがない場合、設定ファイルを更新
            if (!ComPorts.Any() && settings.PortName != null)
            {
                settings.PortName = ComPorts.FirstOrDefault() ?? string.Empty;
            }
            else if (!ComPorts.Contains(settings.PortName) && ComPorts.Any())
            {
                settings.PortName = ComPorts.First();
            }

            SelectedComPort = settings.PortName;
            SelectedBaudRate = settings.BaudRate;
            SelectedDataBits = settings.DataBits;
            SelectedParity = settings.Parity;
            SelectedStopBits = settings.StopBits;
            SelectedHandshake = settings.Handshake;
        }

        private ScaleSettingModel CreateModelFromProperties()
        {
            return new ScaleSettingModel
            {
                PortName = SelectedComPort ?? string.Empty,
                BaudRate = SelectedBaudRate,
                DataBits = SelectedDataBits,
                Parity = SelectedParity,
                StopBits = SelectedStopBits,
                Handshake = SelectedHandshake
            };
        }

        [RelayCommand]
        private async Task SaveSettingsAsync()
        {
            var settings = CreateModelFromProperties();
            await _connectionService.SaveSettingsAsync(settings);
            MessageBox.Show("設定を保存しました。", "保存完了", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand(CanExecute = nameof(CanConnect))]
        private void Connect()
        {
            if (string.IsNullOrEmpty(SelectedComPort))
            {
                MessageBox.Show("COMポートが選択されていません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var settings = CreateModelFromProperties();
            if (!_connectionService.Connect(settings))
            {
                MessageBox.Show($"ポート {settings.PortName} への接続に失敗しました。", "接続エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CanConnect() => !IsConnected;

        [RelayCommand(CanExecute = nameof(CanDisconnect))]
        private void Disconnect()
        {
            _connectionService.Disconnect();
        }
        private bool CanDisconnect() => IsConnected;
    }
}