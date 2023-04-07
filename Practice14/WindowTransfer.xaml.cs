using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Practice14_Bank;

namespace Practice14
{
    /// <summary>
    /// Логика взаимодействия для WindowTransfer.xaml
    /// </summary>
    public partial class WindowTransfer : Window
    {
        private static readonly Regex regex = new Regex("[^0-9.]+");
        public double Amount { get; set; }
        public WindowTransfer()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (double.Parse(tbAmount.Text) > 0)
            {
                DialogResult = true;
                Amount = double.Parse(tbAmount.Text);
            }
            else DialogResult = false;
            Close();
        }

        private void tbAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (regex.IsMatch(e.Text)) e.Handled = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    btnCancel.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;
                case Key.Enter:
                    btnOK.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;
            }
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)) tbAmount.Focus();
        }
    }
}
