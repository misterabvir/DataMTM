using MTM_RE_160.Devices;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MTM_RE_160.Elements
{
    /// <summary>
    /// Логика взаимодействия для StatusElement.xaml
    /// </summary>
    public partial class StatusElement : UserControl
    {

        /// <summary>
        /// Перечислитель возможных статусов загрузочного элемента
        /// </summary>
       public enum Stat
        {
            pause,
            wait,
            yes,
            no
        }

        MTMDevice mtmdevice;
        Stat status;
        bool? worked;

        /// <summary>
        /// Событие работа начата/завершена
        /// </summary>
        public Action<StatusElement, bool, string> WorkedChanged;

        /// <summary>
        /// Текущий статус элемента
        /// </summary>
        public Stat Status
        {
            get
            {
                return status;
            }

          private  set
            {
                status = value;
                ChangeIcon();
            }
        }
        /// <summary>
        /// МТМ РЭ-160 привязанный к этому элементу
        /// </summary>
        public MTMDevice Mtmdevice
        {
            get
            {
                return mtmdevice;
            }

            private set
            {
                mtmdevice = value;
            }
        }
        /// <summary>
        /// Работа
        /// true - идет,
        /// false - стоит,
        /// null - очередь,
        /// </summary>
        public bool? Worked
        {
            get
            {
                return worked;
            }

            set
            {
                worked = value;
                ChangeStatus();
                if (WorkedChanged != null) WorkedChanged(this, worked.GetValueOrDefault(), mtmdevice.PortName);
            }
        }

        /// <summary>
        /// Загрузочный элемент
        /// </summary>
        public StatusElement()
        {
            InitializeComponent();
            Status = Stat.pause;
        }


        /// <summary>
        /// Изменение статусной иконки и иконки повтора
        /// </summary>
        private void ChangeIcon()
        {
            switch (status)
            {
                case Stat.no:
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri("/MTM RE-160;component/Resources/status/no.png", UriKind.Relative);
                        bitmap.EndInit();
                        imageStatus.Source = bitmap;
                        bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri("/MTM RE-160;component/Resources/status/update.png", UriKind.Relative);
                        bitmap.EndInit();
                        imageAgain.Source = bitmap;
                        break;
                    }
                case Stat.yes:
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri("/MTM RE-160;component/Resources/status/yes.png", UriKind.Relative);
                        bitmap.EndInit();
                        imageStatus.Source = bitmap;
                        bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri("/MTM RE-160;component/Resources/status/update.png", UriKind.Relative);
                        bitmap.EndInit();
                        imageAgain.Source = bitmap;
                        break;
                    }
                case Stat.pause:
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri("/MTM RE-160;component/Resources/status/pause.png", UriKind.Relative);
                        bitmap.EndInit();
                        imageStatus.Source = bitmap;
                        bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri("/MTM RE-160;component/Resources/status/empty.png", UriKind.Relative);
                        bitmap.EndInit();
                        imageAgain.Source = bitmap;
                        textBlockProgr.Text = 0.ToString("P1");
                        textBlockStatus.Text = "В очереди...";
                        textBlockFile.Text = "";
                        progressBar.Value = 0;
                        break;
                    }
                case Stat.wait:
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri("/MTM RE-160;component/Resources/status/wait.png", UriKind.Relative);
                        bitmap.EndInit();
                        imageStatus.Source = bitmap;
                        bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri("/MTM RE-160;component/Resources/status/empty.png", UriKind.Relative);
                        bitmap.EndInit();
                        imageAgain.Source = bitmap;
                        break;
                    }


            }
        }

        /// <summary>
        /// Привязать МТМ РЭ-160
        /// </summary>
        /// <param name="mtmd"></param>
        public void AddMTMDevice(MTMDevice mtmd)
        {
            this.Mtmdevice = mtmd;
            mtmd.StatusComplete += StatuSChanged;
            mtmd.End += WorkEnded;
            textBlockAddress.Text = mtmd.Address.ToString();
            textBlockStatus.Text = "В очереди...";

        }
       
        /// <summary>
        /// Отменитьзагрузку
        /// </summary>
        public void Canseled()
        {
           
            mtmdevice.Canceled();
           
        }
        /// <summary>
        /// Начать загрузку
        /// </summary>
        public void Started()
        {
            mtmdevice.Start();
            textBlockFile.Cursor = Cursors.Arrow;
            Status = Stat.wait;
        }
        /// <summary>
        /// Работа завершена
        /// </summary>
        private void WorkEnded()
        {

            Action action = () =>
            {
                
                Worked = false;


            };
                if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
                {
                    Dispatcher.Invoke(action);

                }
                else action();
            }
        /// <summary>
        /// Изменить статус элемента
        /// </summary>
        private void ChangeStatus()
        {
            if (Worked.GetValueOrDefault())
            {
                Status = Stat.wait;
            }
            else if (Worked == null) Status = Stat.pause;
            else
            {
                if (File.Exists(Mtmdevice.path))
                {
                    Status = Stat.yes;
                    FileInfo fi = new FileInfo(mtmdevice.path);
                    textBlockFile.Cursor = Cursors.Hand;
                    textBlockFile.Text = fi.Name + "\r\n" + ((double)fi.Length / 1024).ToString("F3") + " кБ";
                }
                else
                {
                    Status = Stat.no;
                }
            }
        }
        /// <summary>
        /// Статус МТМ РЭ-160 изменился 
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="count">Ожидаемое полное число блоков данных</param>
        /// <param name="index">Текущее значение числа блока данных</param>
        private void StatuSChanged(string message, int count, int index)
        {
            Action action = () => {

                textBlockStatus.Text = message;
                progressBar.Maximum = count;
                progressBar.Value = index;
                textBlockProgr.Text = ((double)(index) / count).ToString("P1");
            };

            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(action);

            }
            else action();
        }

     /// <summary>
     /// Начать повтор
     /// </summary>
     /// <param name="sender"></param>
     /// <param name="e"></param>
        private void imageAgain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Worked == true) return;

            if (File.Exists(Mtmdevice.path))
            {
                File.Delete(Mtmdevice.path);
            }

                Worked = null;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double fontsize = Math.Min(this.ActualWidth, this.ActualHeight) / 15;
            this.FontSize = fontsize;
            textBlockAddress.FontSize = fontsize * 3;
        }

        private void textBlockFile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (File.Exists(Mtmdevice.path))
            {
                try
                {
                    Process.Start(Mtmdevice.path);
                }
                catch (Exception err) { MessageBox.Show(err.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }
    }
}
