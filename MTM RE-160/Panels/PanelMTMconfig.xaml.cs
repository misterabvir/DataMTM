using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MTM_RE_160.MTMConfig;
using System.IO.Ports;

namespace MTM_RE_160.Panels
{
    /// <summary>
    /// Панель конфигурации приборов в сети
    /// </summary>
    public partial class PanelMTMconfig : UserControl
    {
        public PanelMTMconfig()
        {
            InitializeComponent();
            ShowList();
        }
        /// <summary>
        /// Дефолтное состояние
        /// </summary>
        private void ShowList()
        {
            string[] portNames = SerialPort.GetPortNames();
            foreach (string portname in portNames)
                comboBoxCOM.Items.Add(portname);
            comboBoxCOM.SelectedIndex = -1;
            comboBoxSpeed.Items.Add("38400");
            comboBoxSpeed.Items.Add("9600");
            comboBoxSpeed.SelectedIndex = -1;
            LoadInList();
        }
        /// <summary>
        /// Загрузить список
        /// </summary>
        private void LoadInList()
        {
            var sort = from mtm in Settings.mtms
                       orderby mtm.Address
                       select mtm;
            listBox.Items.Clear();
            foreach (MTMClass mtmc in sort)
            {
                listBox.Items.Add(mtmc);
            }
                  
        }
        /// <summary>
        /// Сохранить изменения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedIndex != -1)
            {
                MTMClass mtm = listBox.SelectedItem as MTMClass;

                mtm.Address = numAddr.Value.ToString();
                mtm.NameOfObject = textBox.Text;
                mtm.ComPortName = comboBoxCOM.SelectedItem as string;
                mtm.SpeedPort = comboBoxSpeed.SelectedItem as string;
                mtm.SpeedRegistration = numSpeed.Value.ToString();
            }


            Settings.SaveSettingsFile();
        }
        /// <summary>
        /// Добавить запись
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (textBox.Text == "" || comboBoxSpeed.SelectedIndex == -1 || comboBoxCOM.SelectedIndex == -1)
                return;
            MTMClass mtm = new MTMClass();
            mtm.Address = numAddr.Value.ToString();
            mtm.NameOfObject = textBox.Text;
            mtm.ComPortName = comboBoxCOM.SelectedItem as string;
            mtm.SpeedPort = comboBoxSpeed.SelectedItem as string;
            mtm.SpeedRegistration = numSpeed.Value.ToString();

            Settings.mtms.Add(mtm);
            LoadInList();
        }
        /// <summary>
        /// Удалить запись
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDel_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedIndex != -1)
            {
                MTMClass mtm = listBox.SelectedItem as MTMClass;
                Settings.mtms.Remove(mtm);
                LoadInList();
            }
            
        }
        /// <summary>
        /// Выбор записи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.SelectedIndex != -1)
            {
                MTMClass mtm = listBox.SelectedItem as MTMClass;

                textBox.Text = mtm.NameOfObject;
                numAddr.Value = Convert.ToInt32(mtm.Address);
                comboBoxCOM.SelectedItem = mtm.ComPortName;
                comboBoxSpeed.SelectedItem = mtm.SpeedPort;
                numSpeed.Value = Convert.ToInt32(mtm.SpeedRegistration);
            }

        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double fontsize = ((this.ActualHeight + this.ActualWidth) / 2) / 30;
            if (!double.IsNaN(fontsize))
            {
                this.FontSize = fontsize;
            } 
        }
    }
}
