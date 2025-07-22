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
        private readonly SerialPortManager _serialManager = SerialPortManager.Instance;

        [ObservableProperty] private ObservableCollection<string> availablePorts;
        [ObservableProperty] private ObservableCollection<int> baudRates = new() { 9600, 4800, 19200, 38400, 57600, 115200 };
        [ObservableProperty] private ObservableCollection<int> dataBitsList = new() { 8, 7, 6, 5 };
        [ObservableProperty] private ObservableCollection<Parity> parityOptions = new() { Parity.None, Parity.Mark, Parity.Space};
        [ObservableProperty] private ObservableCollection<StopBits> stopBitsOptions = new() { StopBits.One, StopBits.Two, StopBits.None, StopBits.OnePointFive};
        [ObservableProperty] private ObservableCollection<Handshake> handshakeOptions = new() { Handshake.None, Handshake.XOnXOff, Handshake.RequestToSendXOnXOff};


        [ObservableProperty] private string selectedPort;
        [ObservableProperty] private int selectedBaudRate;
        [ObservableProperty] private int selectedDataBits;
        [ObservableProperty] private Parity selectedParity;
        [ObservableProperty] private StopBits selectedStopBits;
        [ObservableProperty] private Handshake selectedHandshake;

        public ScaleSettingViewModel()
        {
            LoadAsync();
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            var setting = await _serialManager.LoadSettingsAsync();
            SelectedPort = setting.PortName;
            SelectedBaudRate = setting.BaudRate;
            SelectedDataBits = setting.DataBits;
            SelectedParity = setting.Parity;
            SelectedStopBits = setting.StopBits;
            SelectedHandshake = setting.Handshake;


            // COMポートの自動検出結果をリストに追加
            var scanned = _serialManager.ComPortScan();
            AvailablePorts = new ObservableCollection<string>(_serialManager.MySerialCOM.portName);
            if (!AvailablePorts.Contains(scanned))
                AvailablePorts.Insert(0, scanned);
        }

        [RelayCommand]
        private async Task Save()
        {
            var setting = new ScaleSettingModel
            {
                PortName = SelectedPort,
                BaudRate = SelectedBaudRate,
                Parity = SelectedParity,
                StopBits = SelectedStopBits,
                Handshake = SelectedHandshake
            };
            await _serialManager.SaveSettingsAsync(setting);
        }


        [RelayCommand]
        private void Disconnect()
        {
            _serialManager.Close();
            _serialManager.Dispose();
        }
        [RelayCommand]
        private void Connect()
        {

            _serialManager.Open();
        }
    }
}