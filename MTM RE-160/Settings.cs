using Microsoft.Win32;
using MTM_RE_160.Devices;
using MTM_RE_160.MTMConfig;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MTM_RE_160
{
    static public class Settings
    {
        static private PanelEnum panel = PanelEnum.main;
        static public PanelEnum Panel {
            get { return panel; }
            set
            {
                panel = value;
                if (PanelChanged != null) PanelChanged(Panel);
            }
        }

        public static int CountDay
        {
            get
            {
                return countDay;
            }

            set
            {
                int val = value;
                switch (val)
                {
                    case 0: countDay = 1; break;
                    case 1: countDay = 3; break;
                    case 2: countDay = 7; break;
                    default: countDay = int.MaxValue/500; break;
                }
                 
            }
        }

        public static string Path
        {
            get
            {
                return path;
            }

            set
            {
                path = value;
            }
        }

        public static string PathNet
        {
            get
            {
                return pathNet;
            }

            set
            {
                pathNet = value;
            }
        }

        static public PrefhicsCurrent prephics = PrefhicsCurrent.MTM;
        public static string GetPrephiks
        {
            get
            {
                if(prephics == PrefhicsCurrent.MTM)
                    return PrephiksMTM;
                else
                    return PrephiksBPD;
            }

            private set
            {
                PrephiksMTM = value;
            }
        }

        public static string PrephiksMTM
        {
            get
            {
                return prephiksMTM;
            }

            set
            {
                prephiksMTM = value;
            }
        }

        public static string PrephiksBPD
        {
            get
            {
                return prephiksBPD;
            }

            set
            {
                prephiksBPD = value;
            }
        }

        public static int DelayBetweenRequests
        {
            get
            {
                return delayBetweenRequests;
            }

            set
            {
                delayBetweenRequests = value;
            }
        }

        public static int ResponseTime
        {
            get
            {
                return responseTime;
            }

            set
            {
                responseTime = value;
            }
        }

        public static int ResponseCount
        {
            get
            {
                return responseCount;
            }

            set
            {
                responseCount = value;
            }
        }

        public static bool Display
        {
            get
            {
                return display;
            }

            set
            {
                display = value;
                if (DisplayChanged != null) DisplayChanged(display);
            }
        }

        public static int Hour
        {
            get
            {
                return hour;
            }

            set
            {
                hour = value;
            }
        }

        public static string FolderToCopy = "";

        static public Action<PanelEnum> PanelChanged;
        static public Action<bool> DisplayChanged;

        static public MTMs mtms = new MTMs();

        static public void SaveSettingsFile()
        {
            FileStream fs = new FileStream(
             "default.mtm",
             FileMode.Create,
             FileAccess.Write);
            IFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, Settings.mtms);
            fs.Close();
        }

        static public void LoadSettingsFile()
        {
            try
            {
                FileStream fs = new FileStream(
              "default.mtm",
              FileMode.Open,
              FileAccess.Read);
                IFormatter bf = new BinaryFormatter();
                Settings.mtms = (MTMs)bf.Deserialize(fs);
                fs.Close();
            }
            catch { Settings.mtms = new MTMs(); }
        }

        static private int countDay = 1;

        static public MTMDevices MTMsNeeded = new MTMDevices();




        private static string prephiksMTM = "МТМ";
        private static string prephiksBPD = "БПД";
        private static string path = "E:\\";
        private static string pathNet = "E:\\1";
        private static int delayBetweenRequests = 10;
        private static int responseTime = 500;
        private static int responseCount = 5;
        private static bool display = false;
        private static int hour = 7;

        static private void LoadDefaultSettings()
        {
            prephiksMTM = "МТМ";
            prephiksBPD = "БПД";
            path =  Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Abvirsoft\\MTMRE160\\Downloads";
            pathNet = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Abvirsoft\\MTMRE160\\Up_data";
            delayBetweenRequests = 10;
            responseTime = 500;
            responseCount = 5;
            Display = false;
            Hour = 7;
    }
        static public void LoadSettings()
        {
            try
            {
                RegistryKey loadKey = Registry.CurrentUser.OpenSubKey("software\\AbvirSoft\\MTMRE160");
                prephiksMTM = (string)loadKey.GetValue("Папка МТМ");
                prephiksBPD = (string)loadKey.GetValue("Папка БПД");
                path = (string)loadKey.GetValue("Директория для сохранения");
                pathNet = (string)loadKey.GetValue("Директория для копий");
                Hour = (int)loadKey.GetValue("Время переключения");
                Display = (string)loadKey.GetValue("На весь экран") == "True";
                DelayBetweenRequests = (int)loadKey.GetValue("Задержка между запросами");
                ResponseTime = (int)loadKey.GetValue("Задержка перед чтением данных");
                ResponseCount = (int)loadKey.GetValue("Количество повторных запросов");
                loadKey.Close();
            }
            catch { LoadDefaultSettings(); }

        }
        static public void SaveSettings()
        {
            RegistryKey saveKey = Registry.CurrentUser.CreateSubKey("software\\AbvirSoft\\MTMRE160");
            saveKey.SetValue("Папка МТМ", prephiksMTM);
            saveKey.SetValue("Папка БПД", prephiksBPD);

            saveKey.SetValue("Время переключения", Hour);
            saveKey.SetValue("Директория для копий", pathNet);
            saveKey.SetValue("Директория для сохранения", path);
            saveKey.SetValue("На весь экран", Display);
            saveKey.SetValue("Задержка между запросами", DelayBetweenRequests);
            saveKey.SetValue("Задержка перед чтением данных", ResponseTime);
            saveKey.SetValue("Количество повторных запросов", ResponseCount);
            saveKey.Close();
        }



    }
    public enum PrefhicsCurrent { MTM, BPD }



    


}
