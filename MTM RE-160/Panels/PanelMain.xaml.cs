using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MTM_RE_160.Panels
{
    /// <summary>
    /// Логика взаимодействия для PanelMain.xaml
    /// </summary>
    public partial class PanelMain : UserControl
    {
        public PanelMain()
        {
            InitializeComponent();
        }

        private void imageMTM_MouseEnter(object sender, MouseEventArgs e)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("/MTM RE-160;component/Resources/MTMselect.png", UriKind.Relative);
            bitmap.EndInit();
            imageMTM.Source = bitmap;
        }

        private void imageMTM_MouseLeave(object sender, MouseEventArgs e)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("/MTM RE-160;component/Resources/MTMunselect.png", UriKind.Relative);
            bitmap.EndInit();
            imageMTM.Source = bitmap;
        }

        private void imageBPD_MouseEnter(object sender, MouseEventArgs e)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("/MTM RE-160;component/Resources/BPDselect.png", UriKind.Relative);
            bitmap.EndInit();
            imageBPD.Source = bitmap;
        }

        private void imageBPD_MouseLeave(object sender, MouseEventArgs e)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("/MTM RE-160;component/Resources/BPDunselect.png", UriKind.Relative);
            bitmap.EndInit();
            imageBPD.Source = bitmap;
        }

        private void imageMTM_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Settings.Panel = PanelEnum.mtm;
        }

        private void imageBPD_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Settings.Panel = PanelEnum.bpd;
        }
    }
}
