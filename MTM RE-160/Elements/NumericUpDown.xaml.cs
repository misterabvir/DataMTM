using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MTM_RE_160.Elements
{
    /// <summary>
    /// Счетчик целых чисел
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        private int minimum = 0;
        private int maximum=100;
        private int value;
        private int step=1;

        /// <summary>
        /// Минимум
        /// </summary>
        public int Minimum
        {
            get
            {
                return minimum;
            }

            set
            {
                

                int m = value; ;
                if (m >= Maximum)
                    minimum = Maximum - 1;
                else minimum = m;
                if (m > Value)
                    this.Value = m;
              
            }
        }
        /// <summary>
        /// Максимум
        /// </summary>
        public int Maximum
        {
            get
            {
                return maximum;
            }

            set
            {
                int m = value;;
                if (m <= Minimum)
                {
                    maximum = Minimum + 1;
                    
                }else maximum = m;
                if (m < Value)
                    this.Value = m;
              
            }
        }
        /// <summary>
        /// Текущее значение
        /// </summary>
        public int Value
        {
            get
            {
                return value;
            }

            set
            {
                int val = value;

                if (val > Maximum)
                    this.value = Maximum;
                else if (val < Minimum)
                    this.value = Minimum;
                else this.value = val;
                textBox.Text = this.value.ToString();

            }
        }
        /// <summary>
        /// Шаг
        /// </summary>
        public int Step
        {
            get
            {
                return step;
            }

            set
            {
                int val = value;
                if (val > (maximum - minimum))
                    step = (maximum - minimum);
                else
                    step = value;
            }
        }

        public NumericUpDown()
        {
            InitializeComponent();
           
          
            textBox.Text = Value.ToString();
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            int val;
            if (int.TryParse(textBox.Text, out val))
            {
                Value = val;

            }
            else e.Handled = true;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int val;
            if (int.TryParse(textBox.Text, out val))
            {
                Value = val;

            }
            else {
                Value = 0;
                e.Handled = true; }
        }

        private void buttonMin_Click(object sender, RoutedEventArgs e)
        {
            Value = Minimum;
        }

        private void buttonLess_Click(object sender, RoutedEventArgs e)
        {
            Value -= Step;
        }

        private void buttonMore_Click(object sender, RoutedEventArgs e)
        {
            Value += Step;
        }

        private void buttonMax_Click(object sender, RoutedEventArgs e)
        {
            Value = Maximum;
        }
    }
}
