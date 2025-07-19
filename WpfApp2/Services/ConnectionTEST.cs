//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.IO.Ports;
//using System.Linq;
//using System.Reflection.Metadata;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;

//namespace WpfApp2.Services
//{
//    class ConnectionTEST
//    {
//        public void TEST()
//        {
//            string[] ports = SerialPort.GetPortNames();
//            foreach (string port in ports)
//            {
//                try
//                {
//                    using (SerialPort serialPort = new SerialPort(port))
//                    {
//                        serialPort.BaudRate = 9600;
//                        serialPort.DataBits = 8;
//                        serialPort.Parity = Parity.None;
//                        serialPort.StopBits = StopBits.One;
//                        serialPort.Handshake = Handshake.None;


                        
//                        if(IsScaleDevice(port))
//                        {
//                            serialPort.Open();
//                            serialPort.Close();
//                            MessageBox.Show($"Connected to {port}");
//                            break;
//                        }
//                        else
//                        {
//                            continue;
//                        }
//                    }
//                }
//                catch (UnauthorizedAccessException)
//                {
//                    MessageBox.Show($"Access denied to {port}. 別なアプリで使用されています。");
//                }
//                catch (IOException ex)
//                {
//                    MessageBox.Show($"I/O error on {port}. ドライバはインストールされていますか？");
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Error connecting to {port}: {ex.Message}");
//                }
//                finally
//                {
//                    MessageBox.Show($"接続できるCOMポートはありません。");
//                }
//            }
//        }
//        private bool IsScaleDevice(string deviceName)
//        {
//            var scaleKeywords = new[] { "METTLER", "A&D", "SHIMADZU", "Balance", "Scale", "Weight" };
//            return scaleKeywords.Any(keyword => deviceName.Contains(keyword, StringComparison.OrdinalIgnoreCase));
//        }
//    }
//}
