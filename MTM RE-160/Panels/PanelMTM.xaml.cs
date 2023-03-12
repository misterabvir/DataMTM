using MTM_RE_160.MTMConfig;
using MTM_RE_160.Elements;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;


namespace MTM_RE_160.Panels
{
    /// <summary>
    /// Логика взаимодействия для PanelMTM.xaml
    /// </summary>
    public partial class PanelMTM : UserControl
    {
        public PanelMTM()
        {
            InitializeComponent();
            Settings.prephics = PrefhicsCurrent.MTM;
            comboBox.Items.Add("За одни сутки");
            comboBox.Items.Add("За трое суток");
            comboBox.Items.Add("За неделю");
            comboBox.Items.Add("Максимум");
            comboBox.SelectedIndex = 0;

            LoadObject();
        }

        private void LoadObject()
        {

            List<string> portnames = new List<string>();
            int line = 1;
            portnames.AddRange(SerialPort.GetPortNames());
            foreach (string portname in portnames)
            {
             
                MTMs mtmscom = new MTMs();
                foreach (MTMClass mtm in Settings.mtms)
                {
                    if(portname == mtm.ComPortName)
                        mtmscom.Add(mtm);
                }

                if (mtmscom.Count == 0) continue;
                ButtonSelectContainer btnCont = new ButtonSelectContainer();
                btnCont.LoadButtons(mtmscom);
                grid.Children.Add(btnCont);
                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;
                grid.RowDefinitions.Add(row);
                Grid.SetRow(btnCont, line);
                line++;          
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox.SelectedIndex != -1) 
                Settings.CountDay = comboBox.SelectedIndex;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Settings.MTMsNeeded.Clear();
            foreach (UIElement elem in grid.Children)
            {
                if (!(elem is ButtonSelectContainer)) continue;
                ButtonSelectContainer btncon = elem as ButtonSelectContainer;
                Settings.MTMsNeeded.AddRangeClasses(btncon.MtmsNEED);
            }

            if (Settings.MTMsNeeded.Count == 0)
            {
                MessageBox.Show("Не выбрано ни одного элемента", "Будьте внимательней!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Settings.Panel = PanelEnum.load;

        }
    }
}
