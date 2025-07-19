using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp2.Models;
using WpfApp2.Services;

namespace WpfApp2.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort _serialPort;

        public MainWindow()
        {
            InitializeComponent();
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            // COMポートを安全に閉じる
            if (_serialPort != null)
            {
                try
                {
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.DiscardInBuffer();
                        _serialPort.DiscardOutBuffer();
                        _serialPort.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"COMポートのクローズに失敗しました: {ex.Message}");
                }
                finally
                {
                    _serialPort.Dispose();
                    _serialPort = null;
                }
            }

            base.OnClosing(e); // 必ず最後に呼ぶ
        }
    }
}
