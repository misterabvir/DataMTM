using System;
using System.Windows;
using System.Windows.Input;
using MTM_RE_160.Panels;

namespace MTM_RE_160
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PanelMain panelMain;
        PanelMTM panelMTM;
        PanelBPD panelBPD;
        PanelLoad panelLoad;
        PanelMTMconfig panelMTMconfig;
        PanelCopy panelCopy;
        PanelSettings panelSettings;
        public MainWindow()
        {
            InitializeComponent();
            Settings.PanelChanged += PanelWasChanged;
            Settings.DisplayChanged += DisplayWasChanged;
            LoadPanel(Settings.Panel);
        }

        private void DisplayWasChanged(bool obj)
        {
            this.WindowState = obj ? WindowState.Maximized : WindowState.Normal;
        }

        private void PanelWasChanged(PanelEnum panel)
        {
            LoadPanel(panel);
        }

        private void LoadPanel(PanelEnum panel)
        {
            switch (panel)
            {
                case PanelEnum.main:
                    {
                        if (panelMain == null)
                        {
                            panelMain = new PanelMain();
                            this.Content = panelMain;
                        }
                        else
                            this.Content = panelMain;
                        break;
                    }
                case PanelEnum.mtm:
                    {
                        if (panelMTM == null)
                        {
                            panelMTM = new PanelMTM();
                            this.Content = panelMTM;
                        }
                        else
                            this.Content = panelMTM;
                        break;
                    }
                case PanelEnum.bpd:
                    {
                        if (panelBPD == null)
                        {
                            panelBPD = new PanelBPD();
                            this.Content = panelBPD;
                        }
                        else
                            this.Content = panelBPD;
                        panelBPD.Start();
                        break;
                    }
                case PanelEnum.config:
                    {
                        if (panelMTMconfig == null)
                        {
                            panelMTMconfig = new PanelMTMconfig();
                            this.Content = panelMTMconfig;
                        }
                        else
                            this.Content = panelMTMconfig;
                        break;
                    }
                case PanelEnum.load:
                    {
                       
                            panelLoad = new PanelLoad();
                            this.Content = panelLoad;
                       
                        panelLoad.StartLoad();
                        break;
                    }
                case PanelEnum.copy:
                    {

                        panelCopy = new PanelCopy();
                        this.Content = panelCopy;
                        panelCopy.StartCopy();
                        break;
                    }
                case PanelEnum.settings:
                    {

                        if (panelSettings == null)
                        {
                            panelSettings = new PanelSettings();
                            this.Content = panelSettings;
                        }
                        else
                            this.Content = panelSettings;
                        panelSettings.LoadSet();
                        break;
                    }


            }



        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3 && Settings.Panel == PanelEnum.main)
                Settings.Panel = PanelEnum.settings;
            if (e.Key == Key.F2 && Settings.Panel == PanelEnum.main)
                Settings.Panel = PanelEnum.config;
            if (e.Key == Key.Escape && Settings.Panel != PanelEnum.main)
                Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ClosedApplication(e);
        }

        private  void ClosedApplication(System.ComponentModel.CancelEventArgs e)
        {
            if (Settings.Panel == PanelEnum.copy && panelCopy.Worked)
            {
                e.Cancel = true;
                return;
            }

            if (!(Settings.Panel == PanelEnum.main))
            {
                if (Settings.Panel == PanelEnum.load && panelLoad.IsWorked())
                {
                    panelLoad.Cancel();
                    e.Cancel = true;
                    return;
                }
                else if (Settings.Panel == PanelEnum.bpd && panelBPD.Worked)
                {
                    e.Cancel = true;
                    return;
                }
               else if (Settings.Panel == PanelEnum.load && !panelLoad.IsWorked())
                {
                    if (MessageBox.Show("Скопировать данные в " + Settings.PathNet, Title, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        Settings.Panel = PanelEnum.copy;
                        e.Cancel = true;
                        return;
                    }
                    Settings.Panel = PanelEnum.main;

                    e.Cancel = true;
                    return;
                }

                Settings.Panel = PanelEnum.main;                
                    e.Cancel = true;
            }



            Settings.SaveSettingsFile();
            Settings.SaveSettings();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Settings.LoadSettings();
            Settings.LoadSettingsFile();
        }
    }
}
