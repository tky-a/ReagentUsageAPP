using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Models
{
    public class ScaleSettingModel
    {
        public string PortName { get; set; } = "COM3";
        public int BaudRate { get; set; } = 9600;
        public int DataBits { get; set; } = 8;
        public Parity Parity { get; set; } = Parity.None;
        public StopBits StopBits { get; set; } = StopBits.One;
        public Handshake Handshake { get; set; } = Handshake.None;
    }
}
