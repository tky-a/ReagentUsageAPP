using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp2.Models;
using WpfApp2.ViewModels;
using System.IO;
using WpfApp2.Services;

namespace WpfApp2.Services
{
    public class RS232C
    {
        private RegisterModeView2ViewModel _viewModel;


        public byte[] readBuffer;
        public const int rxBufSizeMax = 1000;

        public string[] portName;
        public BaudRateItem[] baudRateItems;

        public struct BaudRateItem
        {
            public string rateName;
            public int rateValue;
            public BaudRateItem(string _baudRateName, int _baudRateValue)
            {
                rateName = _baudRateName;
                rateValue = _baudRateValue;
            }
        }

        public RS232C()

        {

            readBuffer = new byte[rxBufSizeMax];
            portName = SerialPort.GetPortNames();
            string[] baudRateNames = { "9600bps", "4800bps", "19200bps", "38400bps", "57600bps", "115200bps" };
            int[] baudRateValues = { 9600, 4800, 19200, 38400, 57600, 115200 };
            baudRateItems = new BaudRateItem[baudRateNames.Length];
            for (int i = 0; i < baudRateNames.Length; i++)
            {
                baudRateItems[i] = new BaudRateItem(baudRateNames[i], baudRateValues[i]);
            }

        }
    }

    public class SerialPortManager
    {
        private static SerialPortManager _instance;
        private SerialPort _serialPort;
        private RS232C _mySerialCOM;

        private readonly string _settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScaleSettings.json");


    public static SerialPortManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SerialPortManager();
                }
                return _instance;
            }
        }

        private SerialPortManager()
        {
            _serialPort = new SerialPort();
            _mySerialCOM = new RS232C();
        }

        public SerialPort SerialPort => _serialPort;
        public RS232C MySerialCOM => _mySerialCOM;

        // イベントで受信通知を外部に伝える
        public event EventHandler<string> DataReceived;

        public void Initialize(string portName, int baudRate)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
            _serialPort.PortName = portName;
            _serialPort.BaudRate = baudRate;
            _serialPort.DataBits = 8;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;
            _serialPort.Encoding = Encoding.ASCII;
            _serialPort.ReceivedBytesThreshold = 1;
        }

        public async Task Open2()
        {    
            try
            {
                var settings = await LoadSettingsAsync();

                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }

                _serialPort.PortName = ComPortScan();
                _serialPort.BaudRate = settings.BaudRate;
                _serialPort.DataBits = settings.DataBits;
                _serialPort.Parity = settings.Parity;
                _serialPort.StopBits = settings.StopBits;
                _serialPort.Handshake = settings.Handshake;
                _serialPort.Encoding = Encoding.ASCII;
                _serialPort.ReceivedBytesThreshold = 1;

                _serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"シリアルポートのオープンに失敗しました。\n{ex.Message}", "接続エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void Open()
        {
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }
        }

        public void Close()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        public void Dispose()
        {
            _serialPort.Dispose();
        }

        public void WriteData(string data)
        {
            if (_serialPort.IsOpen)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(data);
                _serialPort.Write(bytes, 0, bytes.Length);
            }
        }

        public void StartListening()
        {
            _serialPort.DataReceived += SerialPort_DataReceived;
        }

        public void StopListening()
        {
            _serialPort.DataReceived -= SerialPort_DataReceived;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = _serialPort.ReadExisting();

                // 数字とドットのみ抽出（例：1.23kg → 1.23）
                string formattedData = new string(data.Where(c => char.IsDigit(c) || c == '.').ToArray());

                if (!string.IsNullOrEmpty(formattedData))
                {
                    // UIスレッドで実行
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var focusedElement = Keyboard.FocusedElement;

                        if (focusedElement is TextBox textBox)
                        {
                            int caret = textBox.CaretIndex;
                            textBox.Text = textBox.Text.Insert(caret, formattedData);
                            textBox.CaretIndex = caret + formattedData.Length;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
            }
        }

        public string ComPortScan()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%)'");

                foreach (var obj in searcher.Get())
                {
                    var manufacturer = obj["Manufacturer"]?.ToString();

                    if (manufacturer == null) continue;

                    string portType = null;

                    if (manufacturer.Contains("Prolific"))
                    {
                        portType = "prolific";
                    }
                    else if (manufacturer.Contains("FTDI", StringComparison.OrdinalIgnoreCase))
                    {
                        portType = "ftdi";
                    }
                    else if (manufacturer.Contains("CH340", StringComparison.OrdinalIgnoreCase))
                    {
                        portType = "ch340";
                    }

                    if (portType != null)
                    {
                        // COMポート名を抽出（例: "USB-SERIAL CH340 (COM3)" → "COM3"）
                        string name = obj["Name"].ToString();
                        string portName = null;
                        var match = Regex.Match(name ?? "", @"\((COM\d+)\)");
                        if (match.Success)
                        {
                            portName = match.Groups[1].Value;
                        }
                        // 接続テスト（受信のみ）
                                                
                        using var serialPort = new SerialPort(portName)
                        {
                            BaudRate = 9600, // デバイス仕様に合わせて設定
                            Parity = Parity.None,
                            DataBits = 8,
                            StopBits = StopBits.One,
                        };
                        try
                        {
                            serialPort.Open();
                            serialPort.Close();
                            return portName;
                        }
                        catch
                        {
                        // 開けなかった or タイムアウト
                        }
                    }

                }

                //USB接続のときはココ

            }
            catch { }

            
            return null;
        }

        /// <summary>
        /// 設定をJSONファイルから読み込みます。ファイルがない場合はデフォルト値で作成します。
        /// </summary>
        public async Task<ScaleSettingModel> LoadSettingsAsync()
        {
            if (!File.Exists(_settingsFilePath))
            {
                var defaultSettings = new ScaleSettingModel();
                await SaveSettingsAsync(defaultSettings);
                return defaultSettings;
            }

            try
            {
                var json = await File.ReadAllTextAsync(_settingsFilePath);
                var options = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } };
                return JsonSerializer.Deserialize<ScaleSettingModel>(json, options) ?? new ScaleSettingModel();
            }
            catch (Exception) // 例外処理（ファイルの破損など）
            {
                return new ScaleSettingModel(); // デフォルト値を返す
            }
        }
        /// <summary>
        /// 設定をJSONファイルに保存します。
        /// </summary>
        public async Task SaveSettingsAsync(ScaleSettingModel settings)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };
            var json = JsonSerializer.Serialize(settings, options);
            await File.WriteAllTextAsync(_settingsFilePath, json);
        }

    }

}