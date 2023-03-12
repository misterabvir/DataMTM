using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace MTM_RE_160.Devices
{
    public class MTMDevice
    {
        public Action<string, int, int> StatusComplete;
        public Action End;

        /// <summary>
        /// Адресс
        /// </summary>
        public byte Address { get; set; }
        /// <summary>
        /// Имя порта (COM1, COM2...)
        /// </summary>
        public string PortName { get; set; }
        /// <summary>
        /// Количество блоков
        /// </summary>
        public byte Blocks { get; set; }

        public string path { get; }

        public int SpeedPort { get; private set; }

        public MTMDevice(byte address, string portName, int speed, byte blocks, string path)
        {
            this.Address = address;
            this.PortName = portName;
            this.Blocks =  blocks;
            this.SpeedPort = speed;

            DateTime dt = DateTime.Now;
            if (dt.Hour >= Settings.Hour) dt = dt.AddDays(1);
            Settings.FolderToCopy = Settings.FolderToCopy == null || Settings.FolderToCopy == "" ? FolderName(dt) : Settings.FolderToCopy;
            this.path = path + "\\" + Settings.GetPrephiks + "\\"  + Settings.FolderToCopy;


            if (!Directory.Exists(this.path))
                Directory.CreateDirectory(this.path);

            this.path = this.path + "\\" + address + "_" + Settings.FolderToCopy + ".160";

            for (int i = 1; File.Exists(this.path); i++)
            {
                Regex reg = new Regex("\\([0-9]+\\)");
                if (!reg.IsMatch(this.path))
                {
                    this.path = this.path.Substring(0, this.path.Length - 4);
                    this.path += "(" + i + ")";
                    this.path += ".160";
                }
                else
                {

                    this.path=this.path.Replace(reg.Match(this.path).Value, "(" + i + ")");
                }

            }
            
        }

        private string FolderName(DateTime dt)
        {
            int day = dt.Day;
            int month = dt.Month;
            int year = dt.Year;

            string sday = day < 10 ? ("0" + day.ToString()) : day.ToString();
            string smonth = month < 10 ? ("0" + month.ToString()) : month.ToString();
            string syear = year.ToString();

            string output = string.Format("{0}_{1}_{2}", sday, smonth, syear);
            return output; 
        }
        DownLoadsMTMData load;
        public void Start()
        {
            load = new DownLoadsMTMData(this);
            load.BlockLoad += BLOCKLOAD;
            load.Message += MESSAGE;
            load.StatusAllCount += StatusCount;
            load.Started += StartedLoad;
            load.Ended += EndLoad;
            load.Start();
        }

        public void Canceled()
        {
            if (load != null)
                load.Canseled();
        }




        private void EndLoad(string obj, List<byte[]> bytesreaded)
        {

            if (StatusComplete != null)
                StatusComplete(obj, count, count);
            count = 1;
            index = 0;

            SavedBytes(bytesreaded);
            if (End != null) End();
        }

        private void SavedBytes(List<byte[]> bytesreaded)
        {
            if (bytesreaded==null||bytesreaded.Count == 0)
            {
                return;
            }

                try
            {
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                fs.Write(new byte[8] { 0x4d, 0x54, 0x4d, 0x31, 0x36, 0x30, 0x20, 0x33 }, 0, 8);
                fs.Write(new byte[1] { Address }, 0, 1);
                for (int i = 0; i < bytesreaded.Count; i++)
                {
                    fs.Write(bytesreaded[i], 0, bytesreaded[i].Length);
                    fs.Write(new byte[2] { 0x00, 0x17 }, 0, 2);
                    if (StatusComplete != null)
                        StatusComplete("Сохранение...", bytesreaded.Count, i + 1);
                }


                fs.Close();
                if (bytesreaded.Count != 0)
                {
                    if (StatusComplete != null)
                        StatusComplete("Сохранено", bytesreaded.Count, bytesreaded.Count);
                }
             
            }
            catch(Exception err) {
                if (StatusComplete != null)
                    StatusComplete(err.Message, 1, 0);
            }
        }

        private void StartedLoad(string obj)
        {
            if (StatusComplete != null)
                StatusComplete("", count, 0);
        }

        int count = 1;
        int index = 0;
        private void StatusCount(int obj)
        {
            count = obj;
        }

        private void MESSAGE(MTMDevice mtm, string message)
        {
            string str = string.Format("{0}", message);

            if (StatusComplete != null)
                StatusComplete(str, count, index);





        }

        private void BLOCKLOAD(MTMDevice mtm, SerialPort port, byte address, byte channel, byte block, string message, bool getted)
        {
            string str = string.Format("Канал {0} ({1} блок) ...{2}",
                channel, block, message);
            if (getted) index++;
            if (StatusComplete != null)
                StatusComplete(str, count, index);


            
        }
    }
}
