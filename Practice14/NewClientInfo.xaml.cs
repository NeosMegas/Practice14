using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Practice14_Bank;

namespace Practice14
{
    /// <summary>
    /// Логика взаимодействия для NewClientInfo.xaml
    /// </summary>
    public partial class NewClientInfo : Window
    {
        public string ImagePath { get; set; }

        public NewClientInfo()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            AcceptClientInfo();
        }

        private void btncancel_Click(object sender, RoutedEventArgs e)
        {
            CancelClientInfo();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    CancelClientInfo();
                    break;
                case Key.Enter:
                    AcceptClientInfo();
                    break;
            }
        }

        private void CancelClientInfo()
        {
            DialogResult = false;
            Close();
        }

        private void AcceptClientInfo()
        {
            DialogResult = true;
            Close();
        }

        private void btnImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog() { Filter = "PNG files|*.png|JPG files|*.jpg|All Files|*.*" };
            if ((bool)fd.ShowDialog())
            {
                if (System.IO.File.Exists(fd.FileName))
                    ImagePath = fd.FileName;
            }
        }

        /// <summary>
        /// Подготовка формы к редактированию клиента
        /// </summary>
        /// <param name="client">Клиент, информация о котором заносится в форму</param>
        public void PrepareClientEdit(Client client)
        {
            tbFirstName.Text = client.FirstName;
            tbMiddleName.Text = client.MiddleName;
            tbLastName.Text = client.LastName;
            rbStandard.IsChecked = client.GetType() == typeof(Client);
            rbLegal.IsChecked = client.GetType() == typeof(LegalClient);
            rbVIP.IsChecked = client.GetType() == typeof(VIPClient);
            ImagePath = @"" + client.ImageName;
        }

    }
}
