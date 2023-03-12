using MTM_RE_160.MTMConfig;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MTM_RE_160.Elements
{
    /// <summary>
    /// Логика взаимодействия для ButtonSelectContainer.xaml
    /// </summary>
    public partial class ButtonSelectContainer : UserControl
    {
        public MTMs MtmsALL { get; private set; }
        public MTMs MtmsNEED { get; private set; }
       
        public ButtonSelectContainer()
        {
            InitializeComponent();
        }

        public void LoadButtons(MTMs mtms)
        {
            if (mtms==null||mtms.Count == 0) return;

            textBlock.Text = mtms[0].ComPortName;
            MtmsNEED = new MTMs();
            MtmsALL = mtms;

            List<string> nameOfObjects = new List<string>();
            foreach (MTMClass mtmt in MtmsALL)
            {
                if (nameOfObjects.IndexOf(mtmt.NameOfObject) == -1)
                    nameOfObjects.Add(mtmt.NameOfObject);
            }

            grid.Columns = Convert.ToInt32(Math.Sqrt(nameOfObjects.Count));
            



            foreach (string name in nameOfObjects)
            {
                ButtonSelect btn = new ButtonSelect();
                btn.Text = name;
                btn.CheckedChanged += CheckedBTN;
                grid.Children.Add(btn);
                
               
            }

        }

        private void CheckedBTN(ButtonSelect btn, bool IsChecked)
        {
            string str = btn.Text;
            foreach (MTMClass mtmt in MtmsALL)
                if (mtmt.NameOfObject == str)
                    if (IsChecked) MtmsNEED.Add(mtmt);
                    else MtmsNEED.Remove(mtmt);
        }
    }
}
