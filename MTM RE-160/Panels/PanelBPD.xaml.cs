using MTM_RE_160.Device;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using MTM_RE_160.Devices;

namespace MTM_RE_160.Panels
{
    /// <summary>
    /// Логика взаимодействия для PanelBPD.xaml
    /// </summary>
    public partial class PanelBPD : UserControl
    {
        public bool Worked { get; private set; }


        public PanelBPD()
        {
            InitializeComponent();
            Settings.prephics = PrefhicsCurrent.BPD;
        }
        public void Start()
        {
            img.Opacity = 100;
            this.Cursor = Cursors.Wait;
            txtStatusSearch.Text = "ПОИСК...";
            txtStatusSearch.Cursor = this.Cursor;
            Worked = true;
            BPDDevice bpd = new BPDDevice();
            bpd.Founded += Found;
            bpd.NotFounded += NotFound;
            bpd.Search();
        }

        private void NotFound()
        {
            Action action = () =>
                       {
                           this.Cursor = Cursors.Arrow;
                           txtStatusSearch.Cursor = Cursors.Hand;
                           img.Opacity = 0;
                           txtStatusSearch.Text = "БПД не найден\r\nЩелкните по этой надписи для повторного поиска";
                           Worked = false;
                       };
                if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
                {
                    Dispatcher.Invoke(action);

                }
                else action();
            }

        private void Found(MTMDevices mtmds)
        {
            Action action = () =>
            {
                img.Opacity = 0;
                this.Cursor = Cursors.Arrow;
                txtStatusSearch.Cursor = Cursors.Hand;
                Settings.MTMsNeeded = mtmds;
                Settings.Panel = PanelEnum.load;
                Worked = false;
            };
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(action);

            }
            else action();
        }

        private void txtStatusSearch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Worked) return;
            Start();
        }
    }
}
