using System;
using System.Windows.Input;
using System.Windows.Forms;
using System.IO;
namespace MTM_RE_160.Panels
{
    /// <summary>
    /// Логика взаимодействия для PanelSettings.xaml
    /// </summary>
    public partial class PanelSettings : System.Windows.Controls.UserControl
    {
        public PanelSettings()
        {
            InitializeComponent();

        }

        #region КНОПОЧКИ
        private void imageOK_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            imageOK.Opacity = 1;
        }

        private void imageOK_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            imageOK.Opacity = 0.2;
        }

        private void imageCancel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            imageCancel.Opacity = 1;
        }

        private void imageCancel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            imageCancel.Opacity = 0.2;
        }

        private void imageViewPath_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            imageViewPath.Opacity = 1;
        }

        private void imageViewPath_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            imageViewPath.Opacity = 0.2;
        }

        private void imageViewPathNet_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            imageViewPathNet.Opacity = 1;
        }

        private void imageViewPathNet_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            imageViewPathNet.Opacity = 0.2;
        }
        #endregion

        public void LoadSet()
        {
            textBoxPath.Text = Settings.Path;
            textBoxPathNet.Text = Settings.PathNet;
            textBoxBPD.Text = Settings.PrephiksBPD;
            textBoxMTM.Text = Settings.PrephiksMTM;
            numdel.Value = Settings.DelayBetweenRequests;
            numReq.Value = Settings.ResponseTime;
            numCount.Value = Settings.ResponseCount;
            checkBoxDisplay.IsChecked = Settings.Display;
            numDate.Value = Settings.Hour;
        }

        private void imageViewPath_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.RootFolder = Environment.SpecialFolder.MyComputer;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxPath.Text = dlg.SelectedPath;
            }
        }

        private void imageViewPathNet_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.RootFolder = Environment.SpecialFolder.MyComputer;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxPathNet.Text = dlg.SelectedPath;
            }
        }

        private void imageOK_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Settings.Path = Directory.Exists(textBoxPath.Text) ? textBoxPath.Text : Settings.Path;
            Settings.PathNet = Directory.Exists(textBoxPathNet.Text) ? textBoxPathNet.Text : Settings.PathNet;
            Settings.PrephiksBPD = textBoxBPD.Text == "" ? Settings.PrephiksBPD : textBoxBPD.Text;
            Settings.PrephiksMTM = textBoxMTM.Text == "" ? Settings.PrephiksMTM : textBoxMTM.Text;
            Settings.DelayBetweenRequests = numdel.Value;
            Settings.ResponseTime = numReq.Value;
            Settings.ResponseCount = numCount.Value;
            Settings.Display = checkBoxDisplay.IsChecked.GetValueOrDefault();
            Settings.Hour = numDate.Value;

        }

        private void imageCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Settings.Panel = PanelEnum.main;
        }

       
    }
}
