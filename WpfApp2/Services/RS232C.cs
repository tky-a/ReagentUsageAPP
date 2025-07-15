using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp2.ViewModels;

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
                // UIスレッドに戻してログ出力（エラー通知）
                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    Debug.WriteLine("受信エラー: " + ex.Message);
                //});
            }
        }

    }
}
