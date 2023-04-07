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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.IO;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using System.CodeDom;
using System.ComponentModel;
using System.Windows.Markup.Localizer;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using Practice14_Bank;

namespace Practice14
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Bank bank = new Bank("Банк А");

        public MainWindow()
        {
            InitializeComponent();
            lvClients1.ItemsSource = bank.Clients;
            lvClients2.ItemsSource = bank.Clients;
            bank.Clients.CollectionChanged += Clients_CollectionChanged;
            lvHistory.ItemsSource = bank.Events;
        }

        private void Client_ClientCreated(string Msg)
        {
            bank.Events.Add(new BankEventInfo(Msg));
        }

        /// <summary>
        /// Загрузка базы данных клиентов и счетов
        /// </summary>
        private void LoadData()
        {
            if (!File.Exists(@"db.json")) return;
            string json = File.ReadAllText(@"db.json");
            var clients = JArray.Parse(json);
            Client client = null;
            foreach (var c in clients)
            {
                if (c["ClassName"].ToString() == "Client" )
                    client = new Client(int.Parse(c["Id"].ToString()),
                                        c["LastName"].ToString(),
                                        c["FirstName"].ToString(),
                                        c["MiddleName"].ToString(),
                                        c["ImageName"].ToString());
                else if (c["ClassName"].ToString() == "LegalClient")
                    client = new LegalClient(int.Parse(c["Id"].ToString()),
                                             c["LastName"].ToString(),
                                             c["FirstName"].ToString(),
                                             c["MiddleName"].ToString(),
                                             double.Parse(c["TaxRate"].ToString()),
                                             c["ImageName"].ToString());
                else if (c["ClassName"].ToString() == "VIPClient")
                    client = new VIPClient(int.Parse(c["Id"].ToString()),
                                           c["LastName"].ToString(),
                                           c["FirstName"].ToString(),
                                           c["MiddleName"].ToString(),
                                           double.Parse(c["TaxRate"].ToString()),
                                           double.Parse(c["VipRating"].ToString()),
                                           c["ImageName"].ToString());
                client.Cash = double.Parse(c["Cash"].ToString());
                client.Debt = double.Parse(c["Debt"].ToString());
                client.GoodCreditHistory = bool.Parse(c["GoodCreditHistory"].ToString());
                bank.Clients.Add(client);
                var accs = c["BankAccounts"].ToArray();
                BankAccount ac = null;
                foreach (var a in accs)
                {
                    if (a["ClassName"].ToString() == "DepositAccount")
                    {
                        ac = JsonConvert.DeserializeObject<DepositAccount>(a.ToString());
                        bank.Accounts.Add(ac);
                        client.BankAccounts.Add(ac);
                    }
                    else if (a["ClassName"].ToString() == "NonDepositAccount")
                    {
                        ac = JsonConvert.DeserializeObject<NonDepositAccount>(a.ToString());
                        bank.Accounts.Add(ac);
                        client.BankAccounts.Add(ac);
                    }
                }
            }
        }

        /// <summary>
        /// Сохранение базы данных клиентов и счетов
        /// </summary>
        private void SaveData()
        {
            string json;
            json = JsonConvert.SerializeObject(bank.Clients, Formatting.Indented);
            File.WriteAllText(@"db.json", json);
            json = JsonConvert.SerializeObject(bank.Events, Formatting.Indented);
            File.AppendAllText(@"log.json", json);
        }

        /// <summary>
        /// Для обновления состояния кнопок при добавлении/удалении клиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clients_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    if ((e.OldItems[0] as Client).Edited) (e.OldItems[0] as Client).Edited = false;
                    else bank.Events.Add(new BankEventInfo($"Клиент {(e.OldItems[0] as Client).Id}: {e.OldItems[0] as Client} удалён."));
                    //lvClients1.SelectedIndex = lvClients1.SelectedIndex - 1;
                    break;
            }
            ToggleClientButtons();
        }

        /// <summary>
        /// Открытие окна ввода информации о клиенте. В случае, если введено хотябы что-то из фамилии, имени или отчества, новый клиент добавляетя в банк, иначе - отмена.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewClient_Click(object sender, RoutedEventArgs e)
        {
            NewClientInfo nci = new NewClientInfo();
            if (nci.ShowDialog().Value)
            {
                if (nci.tbLastName.Text != string.Empty || nci.tbFirstName.Text != string.Empty || nci.tbMiddleName.Text != string.Empty)
                {
                    Client c = null;
                    if (nci.rbStandard.IsChecked.Value)
                        c=new Client(nci.tbLastName.Text, nci.tbFirstName.Text, nci.tbMiddleName.Text);
                    else if (nci.rbLegal.IsChecked.Value)
                        c=new LegalClient(nci.tbLastName.Text, nci.tbFirstName.Text, nci.tbMiddleName.Text);
                    else if (nci.rbVIP.IsChecked.Value)
                        c=new VIPClient(nci.tbLastName.Text, nci.tbFirstName.Text, nci.tbMiddleName.Text);
                    if (nci.ImagePath != string.Empty)
                        if (File.Exists(nci.ImagePath))
                        {
                            File.Copy(nci.ImagePath, @"" + c.Id.ToString()+System.IO.Path.GetExtension(nci.ImagePath), true);
                            c.ImageName = c.Id.ToString() + System.IO.Path.GetExtension(nci.ImagePath);
                        }
                    bank.Clients.Add(c);
                }
            }
        }

        private void lvClients1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvClients1.SelectedItem != null)
            {
                lvAccounts1.ItemsSource = (lvClients1.SelectedItem as Client).BankAccounts;
                if ((lvClients1.SelectedItem as Client).BankAccounts.Count > 0)
                    lvAccounts1.SelectedItem = (lvClients1.SelectedItem as Client).BankAccounts[0];
            }
            ToggleClientButtons();
        }

        private void lvClients2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvClients2.SelectedItem != null)
            {
                lvAccounts2.ItemsSource = (lvClients2.SelectedItem as Client).BankAccounts;
                if ((lvClients2.SelectedItem as Client).BankAccounts.Count > 0)
                    lvAccounts2.SelectedItem = (lvClients2.SelectedItem as Client).BankAccounts[0];
            }
        }

        private void btnRemoveClient_Click(object sender, RoutedEventArgs e)
        {
            if (lvClients1.SelectedItem != null)
                if (MessageBox.Show($"Вы действительно хотите удалить клиента {(lvClients1.SelectedItem as Client)}?", "Удаление клиента", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    (lvClients1.SelectedItem as Client).BankAccounts.Clear();
                    bank.Clients.Remove(lvClients1.SelectedItem as Client);
                }
        }

        /// <summary>
        /// Переключение видимости кнопок в зависимости от текущего выбора клиентов и счетов
        /// </summary>
        private void ToggleClientButtons()
        {
            btnRemoveClient.IsEnabled = false;
            btnOpenAccount.IsEnabled = false;
            btnCloseAccount.IsEnabled = false;
            btnTransfer.IsEnabled = false;
            btnAddMoney.IsEnabled = false;
            btnWithdraw.IsEnabled = false;
            if (lvClients1.SelectedItem != null)
            {
                tbClientInfo.Text = $"Наличные: {(lvClients1.SelectedItem as Client).Cash}\n" +
                                    $"Долг: {(lvClients1.SelectedItem as Client).Debt}\n" +
                                    $"Хорошая кредитная история: {((lvClients1.SelectedItem as Client).GoodCreditHistory ? "да" : "нет")}";
                if ((lvClients1.SelectedItem as Client) is LegalClient)
                {
                    tbClientInfo.Text += $"\nСтавка налога: {(lvClients1.SelectedItem as LegalClient).TaxRate : #.##%}";
                }
                if ((lvClients1.SelectedItem as Client) is VIPClient)
                {
                    tbClientInfo.Text += $"\nУровень VIP: {(lvClients1.SelectedItem as VIPClient).VipRating: #.##%}";
                }
                Image1.Source = (lvClients1.SelectedItem as Client).Image;
                btnRemoveClient.IsEnabled = true;
                if ((lvClients1.SelectedItem as Client).BankAccounts.Count < 2)
                    btnOpenAccount.IsEnabled = true;
                if ((lvClients1.SelectedItem as Client).BankAccounts.Count > 0)
                {
                    if (lvAccounts1.SelectedItem != null)
                    {
                        btnCloseAccount.IsEnabled = true;
                        btnAddMoney.IsEnabled = true;
                        if ((lvAccounts1.SelectedItem as BankAccount) is DepositAccount)
                        {
                            if ((lvAccounts1.SelectedItem as BankAccount).Balance > 0)
                                btnWithdraw.IsEnabled = true;
                        }
                        else if ((lvAccounts1.SelectedItem as BankAccount) is NonDepositAccount)
                            btnWithdraw.IsEnabled = true;
                        if (lvAccounts2.SelectedItem != null)
                            if ((lvClients1.SelectedItem as Client) != (lvClients2.SelectedItem as Client))
                            {
                                if (((lvAccounts1.SelectedItem as BankAccount) is DepositAccount && (lvAccounts2.SelectedItem as BankAccount) is DepositAccount) ||
                                    ((lvAccounts1.SelectedItem as BankAccount) is NonDepositAccount && (lvAccounts2.SelectedItem as BankAccount) is NonDepositAccount))
                                    btnTransfer.IsEnabled = true;
                            }
                            else if ((lvAccounts1.SelectedItem as BankAccount) != (lvAccounts2.SelectedItem as BankAccount)) 
                                btnTransfer.IsEnabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Открытие нового счёта для выбранного в верхнем ListView клиента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenAccount_Click(object sender, RoutedEventArgs e)
        {
            NewClientInfo nci = new NewClientInfo();
            DepositOperations<Client> depositOps = new DepositOperations<Client>(bank);
            NonDepositOperations<Client> nonDepositOps = new NonDepositOperations<Client>(bank);
            #region UpdateDialog
            (VisualTreeHelper.GetParent(nci.tbFirstName) as StackPanel).Children.Clear();
            (VisualTreeHelper.GetParent(nci.tbClientData) as StackPanel).Children.Remove(nci.tbClientData);
            (VisualTreeHelper.GetParent(nci.rbVIP) as StackPanel).Children.Remove(nci.rbVIP);
            (VisualTreeHelper.GetParent(nci.btnImage) as StackPanel).Children.Remove(nci.btnImage);
            nci.Title = "Открытие счёта";
            nci.SizeToContent = SizeToContent.Height;
            nci.tbClientType.Text = "Тип счёта";
            nci.rbStandard.Content = "Депозитный";
            nci.rbLegal.Content = "Недепозитный";
            if (depositOps.HasAccount((lvClients1.SelectedItem as Client)))
            {
                nci.rbStandard.IsEnabled = false;
                nci.rbLegal.IsChecked = true;
            }
            if (nonDepositOps.HasAccount((lvClients1.SelectedItem as Client)))
            {
                nci.rbStandard.IsChecked = true;
                nci.rbLegal.IsEnabled = false;
            }
            #endregion
            if (nci.ShowDialog().Value)
            {
                if (nci.rbStandard.IsChecked.Value)
                {
                    if ((lvClients1.SelectedItem as Client) is Client)
                        bank.OpenNewAccount((lvClients1.SelectedItem as Client), BankAccountType.Deposit);
                    else if ((lvClients1.SelectedItem as Client) is LegalClient)
                        bank.OpenNewAccount((lvClients1.SelectedItem as LegalClient), BankAccountType.Deposit);
                    else if ((lvClients1.SelectedItem as Client) is VIPClient)
                        bank.OpenNewAccount((lvClients1.SelectedItem as VIPClient), BankAccountType.Deposit);
                }
                else if (nci.rbLegal.IsChecked.Value)
                    if ((lvClients1.SelectedItem as Client) is Client)
                        bank.OpenNewAccount((lvClients1.SelectedItem as Client), BankAccountType.NonDeposit);
                    else if ((lvClients1.SelectedItem as Client) is LegalClient)
                        bank.OpenNewAccount((lvClients1.SelectedItem as LegalClient), BankAccountType.NonDeposit);
                    else if ((lvClients1.SelectedItem as Client) is VIPClient)
                        bank.OpenNewAccount((lvClients1.SelectedItem as VIPClient), BankAccountType.NonDeposit);
                ToggleClientButtons();
            }
            if ((lvClients1.SelectedItem as Client).BankAccounts.Count >= 2) btnOpenAccount.IsEnabled = false;
        }

        /// <summary>
        /// Закрытие счёта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCloseAccount_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Вы действительно хотите закрыть счёт {lvAccounts1.SelectedItem as BankAccount}?", "Закрытие счёта", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                bool success = bank.CloseAccount(lvAccounts1.SelectedItem as BankAccount);
                if (success) (lvClients1.SelectedItem as Client).BankAccounts.Remove(lvAccounts1.SelectedItem as BankAccount);
            }
        }

        private void lvAccounts1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ToggleClientButtons();
        }

        private void lvAccounts2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ToggleClientButtons();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveData();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            if (bank.Clients.Count > 0)
            {
                lvClients1.SelectedItem = bank.Clients[0];
                lvClients2.SelectedItem = bank.Clients[0];
                if (bank.Clients[0].BankAccounts.Count > 0)
                {
                    lvAccounts1.SelectedItem = bank.Clients[0].BankAccounts[0];
                    lvAccounts2.SelectedItem = bank.Clients[0].BankAccounts[0];
                }
            }
            Client.ClientCreated += Client_ClientCreated;
            BankAccount.BankAccountStateChanged += BankAccount_BankAccountStateChanged;
        }

        private void BankAccount_BankAccountStateChanged(string Msg)
        {
            bank.Events.Add(new BankEventInfo(Msg));
        }

        /// <summary>
        /// Перевод средств между любыми счетами одного клиента или однотипными счетами разных клиентов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTransfer_Click(object sender, RoutedEventArgs e)
        {
            if (lvAccounts1.SelectedItem != null)
            {
                WindowTransfer wt = new WindowTransfer();
                if ((lvClients1.SelectedItem as Client).BankAccounts.Contains(lvAccounts2.SelectedItem as BankAccount) &&
                    (lvAccounts2.SelectedItem as BankAccount) != (lvAccounts1.SelectedItem as BankAccount))
                {
                    // Перевод между счетами одного клиента
                    wt.tbInfo.Text = $"Перевод средств между счетами клиента {lvClients1.SelectedItem as Client} со счёта " +
                                        $"{lvAccounts1.SelectedItem as BankAccount} " +
                                        $"на счёт {lvAccounts2.SelectedItem as BankAccount}.";
                    if (wt.ShowDialog().Value)
                    {
                        bool success = bank.Transfer(lvAccounts1.SelectedItem as BankAccount, lvAccounts2.SelectedItem as BankAccount, wt.Amount);
                        bank.Events.Add(new BankEventInfo($"Перевод средств клиента {lvClients1.SelectedItem as Client} со счёта " +
                                        $"{lvAccounts1.SelectedItem as BankAccount} " +
                                        $"на счёт {lvAccounts2.SelectedItem as BankAccount} в размере {wt.Amount}: {(success ? "" : "не")}успешно."));
                    }
                }
                else if((lvClients1.SelectedItem as Client) != (lvClients2.SelectedItem as Client))
                {
                    // Перевод между счетами разных клиентов
                    wt.tbInfo.Text = $"Перевод средств клиента {lvClients1.SelectedItem as Client} со счёта " +
                                        $"{lvAccounts1.SelectedItem as BankAccount} " +
                                        $"клиенту {lvClients2.SelectedItem as Client} на счёт {lvAccounts2.SelectedItem as BankAccount}.";
                    IBankAccountOperations<BankAccount, Client> ops = null;
                    if ((lvAccounts1.SelectedItem as BankAccount) is DepositAccount && (lvAccounts2.SelectedItem as BankAccount) is DepositAccount && (lvAccounts1.SelectedItem as BankAccount).Balance > 0)
                        ops = new DepositOperations<Client>(bank);
                    else if ((lvAccounts1.SelectedItem as BankAccount) is NonDepositAccount && (lvAccounts2.SelectedItem as BankAccount) is NonDepositAccount)
                        ops = new NonDepositOperations<Client>(bank);
                    if (ops != null)
                    {
                        if (wt.ShowDialog().Value)
                        {
                            double amount = ops.TransferBetweenClients(lvClients1.SelectedItem as Client, lvClients2.SelectedItem as Client, wt.Amount);
                            bank.Events.Add(new BankEventInfo($"Перевод средств клиента {lvClients1.SelectedItem as Client} со счёта " +
                                            $"{lvAccounts1.SelectedItem as BankAccount} " +
                                            $"клиенту {lvClients2.SelectedItem as Client} на счёт {lvAccounts2.SelectedItem as BankAccount} в размере {wt.Amount}: {(amount > 0 ? "" : "не")}успешно."));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Редактирование информации о клиенте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvClients1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvClients1.SelectedItem != null)
            {
                NewClientInfo nci = new NewClientInfo();
                nci.PrepareClientEdit(lvClients1.SelectedItem as Client);
                if (nci.ShowDialog().Value)
                    if (nci.tbLastName.Text != string.Empty || nci.tbFirstName.Text != string.Empty || nci.tbMiddleName.Text != string.Empty)
                        EditClient(nci);
            }
        }

        /// <summary>
        /// Редактирование клиента
        /// </summary>
        /// <param name="nci">Окно NewClientInfo с данными редакритуемого клиента</param>
        private void EditClient(NewClientInfo nci)
        {
            Client c = null;
            if (nci.rbStandard.IsChecked.Value)
                c = new Client((lvClients1.SelectedItem as Client).Id, nci.tbLastName.Text, nci.tbFirstName.Text, nci.tbMiddleName.Text);
            else if (nci.rbLegal.IsChecked.Value)
            {
                c = new LegalClient((lvClients1.SelectedItem as Client).Id, nci.tbLastName.Text, nci.tbFirstName.Text, nci.tbMiddleName.Text);
                if (lvClients1.SelectedItem as Client is LegalClient || lvClients1.SelectedItem as Client is VIPClient)
                {
                    (c as LegalClient).TaxRate = (lvClients1.SelectedItem as LegalClient).TaxRate;
                }
            }
            else if (nci.rbVIP.IsChecked.Value)
            {
                c = new VIPClient((lvClients1.SelectedItem as Client).Id, nci.tbLastName.Text, nci.tbFirstName.Text, nci.tbMiddleName.Text);
                if (lvClients1.SelectedItem as Client is VIPClient)
                {
                    (c as VIPClient).TaxRate = (lvClients1.SelectedItem as VIPClient).TaxRate;
                    (c as VIPClient).VipRating = (lvClients1.SelectedItem as VIPClient).VipRating;
                }
                else if (lvClients1.SelectedItem as Client is LegalClient)
                {
                    (c as VIPClient).TaxRate = (lvClients1.SelectedItem as LegalClient).TaxRate;
                }

            }
            c.Cash = (lvClients1.SelectedItem as Client).Cash;
            c.Debt = (lvClients1.SelectedItem as Client).Debt;
            c.GoodCreditHistory = (lvClients1.SelectedItem as Client).GoodCreditHistory;
            if (nci.ImagePath != string.Empty)
                if (File.Exists(nci.ImagePath))
                {
                    if (nci.ImagePath != (lvClients1.SelectedItem as Client).ImageName)
                    {
                        File.Copy(nci.ImagePath, @"" + c.Id.ToString() + System.IO.Path.GetExtension(nci.ImagePath), true);
                        c.ImageName = c.Id.ToString() + System.IO.Path.GetExtension(nci.ImagePath);
                    }
                    else c.ImageName = nci.ImagePath;
                }

            if ((lvClients1.SelectedItem as Client).GetType() != c.GetType())
            {
                bank.Clients.Insert(bank.Clients.IndexOf(lvClients1.SelectedItem as Client), c);
                (lvClients1.SelectedItem as Client).Edited = true;
                bank.Clients.Remove((lvClients1.SelectedItem as Client));
            }
            else bank.Clients[bank.Clients.IndexOf(lvClients1.SelectedItem as Client)] = c;
        }

        /// <summary>
        /// Пополнение счёта.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddMoney_Click(object sender, RoutedEventArgs e)
        {
            if (lvClients1.SelectedItem != null && lvAccounts1.SelectedItem != null)
            {
                WindowTransfer wt = new WindowTransfer();
                wt.tbInfo.Text = "Пополнение счёта";
                if (wt.ShowDialog().Value)
                {
                    AddMoney(wt);
                }
            }
        }

        /// <summary>
        /// Пополнение счёта
        /// </summary>
        /// <param name="wt">Окно с информацией по переводу</param>
        private void AddMoney(WindowTransfer wt)
        {
            double amount = (lvAccounts1.SelectedItem as BankAccount).AddMoney(bank, wt.Amount);
            if ((amount > -1))
            {
                (lvClients1.SelectedItem as Client).Cash -= amount;
                if ((lvClients1.SelectedItem as Client).Cash < 0)
                {
                    (lvClients1.SelectedItem as Client).Debt += -(lvClients1.SelectedItem as Client).Cash;
                    (lvClients1.SelectedItem as Client).Cash = 0;
                }
                ToggleClientButtons();
            }
        }

        /// <summary>
        /// Снятие средств со счёта.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWithdraw_Click(object sender, RoutedEventArgs e)
        {
            if (lvClients1.SelectedItem != null)
            {
                if (lvAccounts1.SelectedItem != null)
                {
                    WindowTransfer wt = new WindowTransfer();
                    wt.tbInfo.Text = "Снятие средств";
                    if (wt.ShowDialog().Value)
                    {
                        try
                        {
                            double amount = (lvAccounts1.SelectedItem as BankAccount).Withdraw(bank, wt.Amount);
                            if ((amount > -1))
                            {
                                (lvClients1.SelectedItem as Client).Cash += amount;
                                ToggleClientButtons();
                            }
                        }
                        catch (BankAccountException ex) { MessageBox.Show(ex.Message, "Ошибка банковского счёта", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                        catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    }
                }
            }
        }

    }
}
