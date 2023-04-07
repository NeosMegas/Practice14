using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Practice14_Bank
{
    public enum BankAccountType
    {
        Deposit = 0,
        NonDeposit = 1
    }

    public class Bank
    {
        public string Name { get; set; }
        public ObservableCollection<Client> Clients { get; set; }
        public ObservableCollection<BankAccount> Accounts { get; }
        public ObservableCollection<BankEventInfo> Events { get; set; }

        public Bank(): this("NoName Банк") { }
        
        public Bank(string name)
        {
            Name = name;
            Clients = new ObservableCollection<Client>();
            Accounts = new ObservableCollection<BankAccount>();
            Events = new ObservableCollection<BankEventInfo>();
        }

        /// <summary>
        /// Открытие нового счёта
        /// </summary>
        /// <typeparam name="T">Тип клиента</typeparam>
        /// <param name="client">Клиент, для которого открывается счёт</param>
        /// <returns>true, если счёт успешно открыт</returns>
        public bool OpenNewAccount<T>(T client, BankAccountType accountType)
            where T : Client
        {
            if (!Clients.Contains(client)) { return false; }
            BankAccount bankAccount = null;
            switch (accountType)
            {
                case BankAccountType.Deposit:
                    bankAccount = new DepositAccount();
                    break;
                case BankAccountType.NonDeposit:
                    bankAccount = new NonDepositAccount();
                    break;
            }
            Accounts.Add(bankAccount);
            client.BankAccounts.Add(bankAccount);
            return bankAccount.Open(this, client);
        }

        /// <summary>
        /// Закрытие счёта
        /// </summary>
        /// <typeparam name="T">Тип счёта</typeparam>
        /// <param name="account">Счёт</param>
        /// <returns>true, если счёт успешно закрыт</returns>
        public bool CloseAccount<T>(T account)
            where T : BankAccount
        {
            if (!Accounts.Contains(account)) { return false; }
            if (account.Balance > 0)
            {
                GetClientByAccount(account).Cash += account.Balance;
                account.Withdraw(this, account.Balance);
            }
            else if (account.Balance < 0)
            {
                GetClientByAccount(account).Debt += -account.Balance;
                account.AddMoney(this, -account.Balance);
            }
            return account.Close(this);
        }

        /// <summary>
        /// Перевод денег с одного счёта на другой
        /// </summary>
        /// <typeparam name="T">Тип банковского счёта</typeparam>
        /// <param name="fromAccount">Счёт, с которого переводят</param>
        /// <param name="toAccount">Счёт, на который переводят</param>
        /// <param name="amount">Переводимая сумма</param>
        /// <returns></returns>
        public bool Transfer<T>(T fromAccount, T toAccount, double amount)
            where T : BankAccount
        {
            if (amount <= 0) { return false; }
            if (fromAccount.Withdraw(this, amount) > 0)
            {
                toAccount.AddMoney(this, amount);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Получает клиента по счёту
        /// </summary>
        /// <typeparam name="T">Тип счёта</typeparam>
        /// <param name="account">Счёт</param>
        /// <returns>Найденный клиент или null</returns>
        public Client GetClientByAccount<T>(T account)
            where T : BankAccount
        {
            foreach (Client client in Clients)
            {
                if (client.Id == account.ClientId) return client;
            }
            return null;
        }
    }

    /// <summary>
    /// Ковариантно-контрвариантный интерфейс для банковских операций.
    /// </summary>
    /// <typeparam name="T1">Обобщённый банковский счёт (ковариантность)</typeparam>
    /// <typeparam name="T2">Обобщённый клиент (контрвариантность)</typeparam>
    public interface IBankAccountOperations<out T1, in T2>
        where T1 : BankAccount
        where T2 : Client
    {
        T1 GetClientAccount(T2 client);
        bool HasAccount(T2 client);
        double TransferBetweenClients(T2 fromClient, T2 toClient, double amount);
    }

    /// <summary>
    /// Операции с депозитными счетами
    /// </summary>
    /// <typeparam name="T">Обобщённый клиент</typeparam>
    public class DepositOperations<T> : IBankAccountOperations<DepositAccount, T>
        where T : Client
    {
        readonly Bank bank;
        public DepositOperations(in Bank b) { bank = b; }
        public DepositAccount GetClientAccount(T client)
        {
            foreach (BankAccount a in client.BankAccounts)
                if (a.GetType() == typeof(DepositAccount))
                    return a as DepositAccount;
            return null;

        }

        /// <summary>
        /// true если клиент имеет депозитный счёт
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool HasAccount(T client)
        {
            if (GetClientAccount(client) != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Перевод средств между депозитнымы счетами двух клиентов
        /// </summary>
        /// <param name="fromClient">Кто переводит</param>
        /// <param name="toClient">Кому переводит</param>
        /// <param name="amount">Сумма перевода</param>
        /// <returns>Если удачно, возвращает сумму перевода, если седств недостаточно возвращает 0, если хотя бы оди из счетов не существует возвращает -1.</returns>
        public double TransferBetweenClients(T fromClient, T toClient, double amount)
        {
            DepositAccount fromAccount = GetClientAccount(fromClient), toAccount = GetClientAccount(toClient);
            double amt;

            if (fromAccount != null && toAccount != null)
            {
                amt = fromAccount.Withdraw(bank, amount);
                if (amt > 0)
                {
                    return toAccount.AddMoney(bank, amt);
                }
                else return 0;
            }
            else return -1;
        }

    }

    /// <summary>
    /// Операции с недепозитными счетами
    /// </summary>
    /// <typeparam name="T">Обобщённый клиент</typeparam>
    public class NonDepositOperations<T> : IBankAccountOperations<NonDepositAccount, T>
        where T : Client
    {
        readonly Bank bank;
        public NonDepositOperations(in Bank b) { bank = b; }

        public NonDepositAccount GetClientAccount(T client)
        {
            foreach (BankAccount a in client.BankAccounts)
                if (a.GetType() == typeof(NonDepositAccount))
                    return a as NonDepositAccount;
            return null;
        }

        /// <summary>
        /// true если клиент имеет недепозитный счёт
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool HasAccount(T client)
        {
            if (GetClientAccount(client) != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Перевод средств между депозитнымы счетами двух клиентов
        /// </summary>
        /// <param name="fromClient">Кто переводит</param>
        /// <param name="toClient">Кому переводит</param>
        /// <param name="amount">Сумма перевода</param>
        /// <returns>Если удачно, возвращает сумму перевода, если седств недостаточно возвращает 0, если хотя бы оди из счетов не существует возвращает -1.</returns>
        public double TransferBetweenClients(T fromClient, T toClient, double amount)
        {
            NonDepositAccount fromAccount = GetClientAccount(fromClient), toAccount = GetClientAccount(toClient);
            double amt;

            if (fromAccount != null && toAccount != null)
            {
                amt = fromAccount.Withdraw(bank, amount);
                if (amt > 0)
                {
                    return toAccount.AddMoney(bank, amt);
                }
                else return 0;
            }
            else return -1;
        }

    }
}
