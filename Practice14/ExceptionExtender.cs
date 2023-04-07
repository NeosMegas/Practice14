using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Practice14
{
    public static class ExceptionExtender
    {
        public static void ShowMessage(this Exception ex, string title = "Ошибка")
        {
            MessageBox.Show(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}
