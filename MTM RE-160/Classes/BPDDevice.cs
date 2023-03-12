using MTM_RE_160.Devices;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace MTM_RE_160.Device
{
    class BPDDevice
    {
        public Action<MTMDevices> Founded;
        public Action NotFounded;
        List<SerialPort> ports;
        public BPDDevice()
        {
            ports = new List<SerialPort>();
            foreach (string name in SerialPort.GetPortNames())
            {
                SerialPort port = new SerialPort(name, 38400, Parity.None, 8, StopBits.One);
                ports.Add(port);
                port = new SerialPort(name, 9600, Parity.None, 8, StopBits.One);
                ports.Add(port);
            }
            ports = ports.OrderByDescending(x=>x.BaudRate).ToList();
        }
        public void Search()
        {
            Thread thread = new Thread(SearchBPD);
            thread.Start();

        }
        private void SearchBPD()
        {
            foreach (SerialPort port in ports)
            {
                try
                {
                   port.Open(); 
                }
                catch
                {
                    if (port.IsOpen) port.Close();
                    continue;
                }

                port.Write(new byte[1] { 0xFE }, 0, 1);
                Thread.Sleep(1000);

                if (port.BytesToRead == 0)
                {
                    port.Close();
                    continue;
                }

                port.Parity = Parity.None;
                byte[] bs = new byte[port.BytesToRead];
                port.Read(bs, 0, port.BytesToRead);
                port.Close();

                if (bs.Length < 25) break;
                AskParse(bs, port.PortName, port.BaudRate);
                return;
            }
            
            NotFounded?.Invoke();
        }

        private void AskParse(byte[] bs, string Name, int baudRate)
        {
           

            byte count = bs[24];

            MTMDevices mtms = new MTMDevices();

            for (int i = 0, j = 0; j < count; i += 2, j++)
            {
                MTMDevice mtm = new MTMDevice(bs[i], Name, baudRate, bs[i + 1], Settings.Path);
                mtms.Add(mtm);
            }
            Founded?.Invoke(mtms);
        }
    }
}
