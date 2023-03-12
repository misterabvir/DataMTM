using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MTM_RE_160.Elements
{
    /// <summary>
    /// Логика взаимодействия для ButtonSelect.xaml
    /// </summary>
    public partial class ButtonSelect : UserControl
    {
        bool isChecked;//7F1263F0
        public Action<ButtonSelect, bool> CheckedChanged;
        LinearGradientBrush brushN = new LinearGradientBrush(Color.FromArgb(50, 0x33, 0x33, 0x33), Color.FromArgb(50, 0x66, 0x66, 0x66), 90);
        LinearGradientBrush brushY = new LinearGradientBrush(Color.FromArgb(0xff, 0x33, 0xcc, 0x33), Color.FromArgb(0xff, 0x33, 0xff, 0x33), 90);



        public ButtonSelect()
        {
            InitializeComponent();
            this.SizeChanged += ButtonSelect_SizeChanged;
            IsChecked = false;
            this.Background = IsChecked ? brushY : brushN;
        }

        private void ButtonSelect_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Height = this.ActualWidth/5;
            this.FontSize = this.Height / 4;
        }

        public bool IsChecked
        {
            get
            {
                return isChecked;
            }

            set
            {
                isChecked = value;
                if (CheckedChanged != null)
                {
                    CheckedChanged(this, isChecked);
                    this.Background = IsChecked ? brushY : brushN;
                }
            }
        }

        public string Text
        {
            get
            {
                return textBlock.Text;
            }

            set
            {
                textBlock.Text = value;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            IsChecked = !IsChecked;

        }

        
    }
}
