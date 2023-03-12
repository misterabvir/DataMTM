using MTM_RE_160.Devices;
using MTM_RE_160.Elements;
using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;

namespace MTM_RE_160.Panels
{
    /// <summary>
    /// Панель загрузчик
    /// </summary>
    public partial class PanelLoad : UserControl
    {
       

        /// <summary>
        /// Панель загрузчик
        /// </summary>
        public PanelLoad()
        {
            InitializeComponent();
            unigrid.Children.Clear();
            unigrid.Columns = Convert.ToInt32(Math.Sqrt(Settings.MTMsNeeded.Count));

            

            foreach (MTMDevice mtmd in Settings.MTMsNeeded)
            {
                StatusElement elem = new StatusElement();
                elem.WorkedChanged += NextStart;
                elem.AddMTMDevice(mtmd);
                unigrid.Children.Add(elem);
            }

        }
        /// <summary>
        /// Работает ли загрузка в настоящее время
        /// </summary>
        /// <returns>Да/Нет</returns>
        public bool IsWorked()
        {
            foreach (UIElement elem in unigrid.Children)
            {
                if (elem is StatusElement)
                {
                    StatusElement el = elem as StatusElement;
                    if (el.Status == StatusElement.Stat.wait)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Занят ли CОМ-порт
        /// </summary>
        /// <param name="portname"></param>
        /// <returns></returns>
        private bool IsWorked(string portname)
        {
            foreach (UIElement elem in unigrid.Children)
            {
                if (elem is StatusElement)
                {
                    StatusElement el = elem as StatusElement;
                    if (el.Status == StatusElement.Stat.wait&&el.Mtmdevice.PortName == portname)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Запускаем следующий на съем, если он есть в очереди 
        /// </summary>
        /// <param name="element">Элемент чей статус поменялся</param>
        /// <param name="work">Работает ли он или остановился</param>
        /// <param name="portname">СОМ-порт</param>
        private void NextStart(StatusElement element, bool work, string portname)
        {

            if (IsWorked(portname))return;

            if (!work)
            {
                LoadStartOne(portname);
            }

        }

        /// <summary>
        /// Начальный старт
        /// </summary>
        internal void StartLoad()
        {
            foreach (string portname in SerialPort.GetPortNames())
            {
                LoadStartOne(portname);
            }
        }

        /// <summary>
        /// Начать загрузку на свободном СОМ-порте
        /// </summary>
        /// <param name="portname">Освободившийся СОМ-порт</param>
        private void LoadStartOne(string portname)
        {
            foreach (UIElement elem in unigrid.Children)
            {
                if (elem is StatusElement)
                {
                    StatusElement el = elem as StatusElement;
                    if (el.Status == StatusElement.Stat.pause && portname == el.Mtmdevice.PortName)
                    {
                        el.Started();
                        return; 
                    }
                }
            }


        }


        /// <summary>
        /// Отменить все загрузки
        /// </summary>
        internal void Cancel()
        {
            foreach (UIElement elem in unigrid.Children)
            {
                if (elem is StatusElement)
                {
                    StatusElement el = elem as StatusElement;
                    if (el.Status == StatusElement.Stat.wait )
                    {
                        el.Canseled();
                    }
                }
            }
        }
    }
}
