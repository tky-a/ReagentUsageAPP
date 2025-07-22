//using System;
//using System.Configuration;
//using System.IO;
//using System.IO.Ports;
//using System.Management;
//using System.Runtime.InteropServices;
//using System.Runtime.Serialization;
//using System.Text.Json;
//using System.Text.Json.Serialization;
//using System.Threading.Tasks;
//using System.Threading;
//using WpfApp2.Models;


//namespace WpfApp2.Services
//{
//    public enum ConnectionType
//    {
//        RS232C,
//        USB,
//        Unknown
//    }

//    public class ScaleDataReceivedEventArgs : EventArgs
//    {
//        public string RawData { get; set; }
//        public decimal? Weight { get; set; }
//        public string Unit { get; set; }
//        public DateTime ReceivedTime { get; set; }

//        public ScaleDataReceivedEventArgs(string rawData)
//        {
//            RawData = rawData;
//            ReceivedTime = DateTime.Now;
//            ParseData(rawData);
//        }

//        private void ParseData(string data)
//        {
//            try
//            {
//                var cleanData = data.Trim();
//                if (decimal.TryParse(cleanData.Replace("g", "").Replace("kg", "").Trim(), out decimal weight))
//                {
//                    Weight = weight;
//                    Unit = cleanData.Contains("kg") ? "kg" : "g";
//                }
//            }
//            catch
//            {

//            }
//        }
//    }

//    public class RS232CToUsbConnectionService
//    {
//        private SerialPort? _serialPort;
//        private Timer? _reconnectTimer;
//        private readonly string _settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScaleSettings.json");
//        private ScaleSettingModel? _currentSettings;
//        private ConnectionType _detectedConnectionType = ConnectionType.Unknown;
//        private string? _detectedPortName;

//        // キーボード入力の有効/無効を制御
//        private bool _keyboardInputEnabled = false;

//        public bool IsConnected => _serialPort != null && _serialPort.IsOpen;
//        public ConnectionType DetectedConnectionType => _detectedConnectionType;
//        public string? DetectedPortName => _detectedPortName;
//        public bool KeyboardInputEnabled
//        {
//            get => _keyboardInputEnabled;
//            set => _keyboardInputEnabled = value;
//        }

//        public event Action<bool>? ConnectionStatusChanged;
//        public event EventHandler<ScaleDataReceivedEventArgs>? DataReceived;
//        public event EventHandler<string>? ErrorOccurred;

//        // Windows API for keyboard input simulation
//        [DllImport("user32.dll")]
//        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

//        [DllImport("user32.dll")]
//        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

//        [DllImport("user32.dll")]
//        private static extern short VkKeyScan(char ch);

//        private const int KEYEVENTF_KEYUP = 0x0002;
//        private const int KEYEVENTF_UNICODE = 0x0004;

//        /// <summary>
//        /// アプリ起動時の接続判別（エラーは出さない）
//        /// </summary>
//        public async Task<ConnectionType> DetectConnectionTypeAsync()
//        {
//            try
//            {
//                _detectedConnectionType = ConnectionType.Unknown;
//                _detectedPortName = null;
//                // USB接続のスケールデバイスを検索
//                if (await DetectUsbScaleAsync())
//                {
//                    _detectedConnectionType = ConnectionType.USB;
//                    return ConnectionType.USB;
//                }

//                // シリアル接続のスケールデバイスを検索
//                var serialPort = await DetectSerialScaleAsync();
//                if (!string.IsNullOrEmpty(serialPort))
//                {
//                    _detectedConnectionType = ConnectionType.RS232C;
//                    _detectedPortName = serialPort;
//                    return ConnectionType.RS232C;
//                }

//                return ConnectionType.Unknown;
//            }
//            catch
//            {
//                // アプリ起動時はエラーを出さない
//                return ConnectionType.Unknown;
//            }
//        }

//        /// <summary>
//        /// USB接続のスケールデバイスを検出
//        /// </summary>
//        private async Task<bool> DetectUsbScaleAsync()
//        {
//            try
//            {
//                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name IS NOT NULL");

//                foreach (var obj in searcher.Get())
//                {
//                    var name = obj["Name"]?.ToString();
//                    if (name != null && IsScaleDevice(name))
//                    {
//                        return true;
//                    }
//                }
//                return false;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        /// <summary>
//        /// シリアル接続のスケールデバイスを検出
//        /// </summary>
//        private async Task<string?> DetectSerialScaleAsync()
//        {
//            try
//            {
//                var availablePorts = SerialPort.GetPortNames();
//                return availablePorts.Length > 0 ? availablePorts[0] : null;
//            }
//            catch
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// 測定画面読み込み時の接続処理
//        /// </summary>
//        public async Task<bool> ConnectForMeasurementAsync()
//        {
//            try
//            {
//                // 再度接続判別
//                var connectionType = await DetectConnectionTypeAsync();

//                if (connectionType == ConnectionType.Unknown)
//                {
//                    ErrorOccurred?.Invoke(this, "スケールデバイスが検出されませんでした。");
//                    return false;
//                }

//                if (connectionType == ConnectionType.USB)
//                {
//                    // USB接続の場合は直接入力モードに移行
//                    ConnectionStatusChanged?.Invoke(true);
//                    return true;
//                }

//                if (connectionType == ConnectionType.RS232C && !string.IsNullOrEmpty(_detectedPortName))
//                {
//                    // シリアル接続の場合は接続を開始
//                    var settings = await LoadSettingsAsync();
//                    settings.PortName = _detectedPortName;

//                    return Connect(settings);
//                }

//                return false;
//            }
//            catch (Exception ex)
//            {
//                ErrorOccurred?.Invoke(this, $"測定接続エラー: {ex.Message}");
//                return false;
//            }
//        }

//        /// <summary>
//        /// デバイス名からスケールデバイスかどうかを判定
//        /// </summary>
//        private bool IsScaleDevice(string deviceName)
//        {
//            var scaleKeywords = new[] { "METTLER", "A&D", "SHIMADZU", "Balance", "Scale", "Weight" };
//            return scaleKeywords.Any(keyword => deviceName.Contains(keyword, StringComparison.OrdinalIgnoreCase));
//        }

//        /// <summary>
//        /// 設定をJSONファイルから読み込みます。ファイルがない場合はデフォルト値で作成します。
//        /// </summary>
//        public async Task<ScaleSettingModel> LoadSettingsAsync()
//        {
//            if (!File.Exists(_settingsFilePath))
//            {
//                var defaultSettings = new ScaleSettingModel();
//                await SaveSettingsAsync(defaultSettings);
//                return defaultSettings;
//            }

//            try
//            {
//                var json = await File.ReadAllTextAsync(_settingsFilePath);
//                var options = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } };
//                return JsonSerializer.Deserialize<ScaleSettingModel>(json, options) ?? new ScaleSettingModel();
//            }
//            catch (Exception) // 例外処理（ファイルの破損など）
//            {
//                return new ScaleSettingModel(); // デフォルト値を返す
//            }
//        }

//        /// <summary>
//        /// 設定をJSONファイルに保存します。
//        /// </summary>
//        public async Task SaveSettingsAsync(ScaleSettingModel settings)
//        {
//            var options = new JsonSerializerOptions
//            {
//                WriteIndented = true,
//                Converters = { new JsonStringEnumConverter() }
//            };
//            var json = JsonSerializer.Serialize(settings, options);
//            await File.WriteAllTextAsync(_settingsFilePath, json);
//        }

//        /// <summary>
//        /// 指定された設定でシリアルポートに接続します。
//        /// </summary>
//        public bool Connect(ScaleSettingModel settings)
//        {
//            if (settings == null) throw new ArgumentNullException(nameof(settings));

//            if (IsConnected) Disconnect();

//            try
//            {
//                GetDeviceDescription(settings.PortName);
//                if (settings.PortName == "USB") return false;

//                _currentSettings = settings;
//                _serialPort = new SerialPort(settings.PortName,
//                    settings.BaudRate, settings.Parity,
//                    settings.DataBits, settings.StopBits)
//                {
//                    Handshake = settings.Handshake,
//                    ReadTimeout = 5000,
//                };

//                _serialPort.DataReceived += OnSerialDataReceived;
//                _serialPort.ErrorReceived += OnSerialErrorReceived;

//                _serialPort.Open();

//                // 長時間接続時のエラー対策として、一定間隔で接続を試みるタイマーを開始
//                StartReconnectTimer();

//                ConnectionStatusChanged?.Invoke(true);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                _serialPort?.Dispose();
//                _serialPort = null;
//                ConnectionStatusChanged?.Invoke(false);
//                ErrorOccurred?.Invoke(this, $"接続エラー:{ex.Message}");
//                return false;
//            }
//        }

//        /// <summary>
//        /// シリアルポートからデータを受信した際のイベントハンドラ
//        /// </summary>
//        private void OnSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
//        {
//            try
//            {
//                if (_serialPort == null || !_serialPort.IsOpen) return;

//                var data = _serialPort.ReadExisting();
//                if (!string.IsNullOrEmpty(data))
//                {
//                    var eventArgs = new ScaleDataReceivedEventArgs(data);

//                    // データ受信イベントを発火
//                    DataReceived?.Invoke(this, eventArgs);

//                    // キーボード入力が有効な場合、データを入力として送信
//                    if (_keyboardInputEnabled && eventArgs.Weight.HasValue)
//                    {
//                        var inputText = $"{eventArgs.Weight.Value}{eventArgs.Unit}";
//                        SendKeyboardInput(inputText);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                ErrorOccurred?.Invoke(this, $"データ受信エラー: {ex.Message}");
//            }
//        }

//        /// <summary>
//        /// テキストをキーボード入力として送信
//        /// </summary>
//        private void SendKeyboardInput(string text)
//        {
//            try
//            {
//                // 各文字を順番に送信
//                foreach (char c in text)
//                {
//                    SendChar(c);
//                    Thread.Sleep(10); // 文字間の遅延
//                }

//                // Enterキーを送信（必要に応じて）
//                SendEnterKey();
//            }
//            catch (Exception ex)
//            {
//                ErrorOccurred?.Invoke(this, $"キーボード入力エラー: {ex.Message}");
//            }
//        }

//        /// <summary>
//        /// 1文字をキーボード入力として送信
//        /// </summary>
//        private void SendChar(char c)
//        {
//            short vkCode = VkKeyScan(c);
//            byte virtualKey = (byte)(vkCode & 0xFF);
//            byte scanCode = (byte)MapVirtualKey(virtualKey, 0);

//            // キーダウン
//            keybd_event(virtualKey, scanCode, 0, UIntPtr.Zero);
//            // キーアップ
//            keybd_event(virtualKey, scanCode, KEYEVENTF_KEYUP, UIntPtr.Zero);
//        }

//        /// <summary>
//        /// Enterキーを送信
//        /// </summary>
//        private void SendEnterKey()
//        {
//            const byte VK_RETURN = 0x0D;
//            byte scanCode = (byte)MapVirtualKey(VK_RETURN, 0);

//            // キーダウン
//            keybd_event(VK_RETURN, scanCode, 0, UIntPtr.Zero);
//            // キーアップ
//            keybd_event(VK_RETURN, scanCode, KEYEVENTF_KEYUP, UIntPtr.Zero);
//        }

//        /// <summary>
//        /// 手動でキーボード入力を送信（テスト用）
//        /// </summary>
//        public void SendManualKeyboardInput(string text)
//        {
//            if (_keyboardInputEnabled)
//            {
//                SendKeyboardInput(text);
//            }
//        }

//        /// <summary>
//        /// シリアルポートでエラーが発生した際のイベントハンドラ
//        /// </summary>
//        private void OnSerialErrorReceived(object sender, SerialErrorReceivedEventArgs e)
//        {
//            ErrorOccurred?.Invoke(this, $"シリアルエラー: {e.EventType}");
//        }

//        /// <summary>
//        /// 特定のコマンドをスケールに送信します
//        /// </summary>
//        public bool SendCommand(string command)
//        {
//            try
//            {
//                if (_serialPort == null || !_serialPort.IsOpen) return false;

//                _serialPort.WriteLine(command);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                ErrorOccurred?.Invoke(this, $"コマンド送信エラー: {ex.Message}");
//                return false;
//            }
//        }

//        private string GetDeviceDescription(string portName)
//        {
//            try
//            {
//                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'");

//                foreach (var obj in searcher.Get())
//                {
//                    var name = obj["Name"]?.ToString();

//                    if (name == null) continue;

//                    if (name.Contains("Prolific"))
//                    {
//                        return "profolific";
//                    }
//                    else if (name.Contains("FDTI"))
//                    {
//                        return "fdti";
//                    }
//                    else if (name.Contains("CH340"))
//                    {
//                        return "ch340";
//                    }
//                }
//                var usbSearchar = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name IS NOT NULL");
//                foreach (var obj in usbSearchar.Get())
//                {
//                    var name = obj["Name"]?.ToString();
//                    var deviceId = obj["DeviceID"]?.ToString();

//                    if (name != null &&
//                        (
//                        name.Contains("METTLER") ||
//                        name.Contains("A&D") ||
//                        name.Contains("SHIMADZU") ||
//                        name.Contains("Balance") ||
//                        name.Contains("Scale")
//                        ))
//                    {
//                        return "USB";
//                    }
//                    else
//                    {
//                        return "unknown"; // 既知のデバイスではない場合
//                    }
//                }
//            }
//            catch { }

//            return null;
//        }

//        /// <summary>
//        /// シリアルポートから切断します。
//        /// </summary>
//        public void Disconnect()
//        {
//            _reconnectTimer?.Dispose();
//            _reconnectTimer = null;

//            if (_serialPort != null)
//            {
//                _serialPort.DataReceived -= OnSerialDataReceived;
//                _serialPort.ErrorReceived -= OnSerialErrorReceived;

//                if (_serialPort.IsOpen)
//                {
//                    _serialPort.Close();
//                }
//                _serialPort.Dispose();
//            }
//            _serialPort = null;
//            ConnectionStatusChanged?.Invoke(false);
//        }

//        /// <summary>
//        /// 接続が意図せず切れた場合に再接続を試みるタイマー
//        /// </summary>
//        private void StartReconnectTimer()
//        {
//            _reconnectTimer?.Dispose();
//            _reconnectTimer = new Timer(CheckConnection, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
//        }

//        private void CheckConnection(object? state)
//        {
//            if (_currentSettings == null) return;

//            // 接続が切れていたら再接続を試みる
//            if (_serialPort == null || !_serialPort.IsOpen)
//            {
//                try
//                {
//                    ConnectionStatusChanged?.Invoke(false);
//                    Connect(_currentSettings);
//                }
//                catch
//                {
//                    // 再接続失敗時は何もしない（次のタイマーで再試行）
//                }
//            }
//        }

//        /// <summary>
//        /// リソースの解放
//        /// </summary>
//        public void Dispose()
//        {
//            try
//            {
//                Disconnect();
//            }
//            catch { }
//        }
//    }
//}