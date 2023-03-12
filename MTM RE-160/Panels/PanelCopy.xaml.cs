using MTM_RE_160.Classes;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace MTM_RE_160.Panels
{
    /// <summary>
    /// Логика взаимодействия для PanelCopy.xaml
    /// </summary>
    public partial class PanelCopy : UserControl
    {
        public PanelCopy()
        {
            InitializeComponent();
           
        }

        public bool Worked { get; private set; }

        public void StartCopy()
        {
            img.Opacity = 100;
            this.Cursor = Cursors.Wait;
            txtStatusCopy.Text = "Копирование...";
            txtStatusCopy.Cursor = this.Cursor;
            Worked = true;
            CopyFiles copir = new CopyFiles(Settings.Path + "\\" + Settings.GetPrephiks, Settings.PathNet+"\\" + Settings.GetPrephiks, Settings.FolderToCopy);
            copir.EVENTSTRING += CopyMessage;
            copir.EVENTCOMPLETE += CopyComplete;
            copir.EVENTERROR += CopyError;

            copir.EVENTCOUNT += CopyCount;
            copir.Start();

        }

        private void CopyError()
        {
            Action action = () =>
            {
                img.Opacity = 0;
                this.Cursor = Cursors.Arrow;
                txtStatusCopy.Cursor = Cursors.Hand;
                Worked = false;
            };
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(action);

            }
            else action();
}

        private void CopyComplete()
        {
            Action action = () =>
            {
                Settings.Panel = PanelEnum.main;
            };
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(action);

            }
            else action();
        }

        private void CopyCount(int index, int count)
        {
            Action action = () =>
            {
                double proc = (double)index / count;
                textBlock.Text = string.Format("{0:P2} {1}/{2}", proc, index, count);
                progress.Maximum = count;
                progress.Value = index;
            };
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(action);

            }
            else action();            
        }

        private void CopyMessage(string obj)
        {
            Action action = () =>
            {
                txtStatusCopy.Text = obj;
            };
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(action);

            }
            else action();
        }

        private void txtStatusCopy_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Worked) return;
            StartCopy();
        }
    }
}
