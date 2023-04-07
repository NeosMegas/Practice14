using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Practice14_Bank
{

    public abstract class BankAccount : IEquatable<BankAccount>, INotifyPropertyChanged
    {
        private int id;
        public int Id { get => id;
            set {id = value;
                if (lastId < value) lastId = value;}}
        public string Name { get; set; }
        public bool Opened { get; set; } = false;
        public int ClientId { get; set; }
        private double balance;
        public double Balance{
            get => balance;
            set{
                balance = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Balance)));
            }
        }
        private protected static int lastId = -1;
        public string ClassName { get; }
        public static event Action<string> BankAccountStateChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public BankAccount(int id, string name)
        {
            Name = name;
            Id = id;
            Opened = false;
            ClassName = GetType().Name;
            BankAccountStateChanged?.Invoke(ToString() + $" создан.");
            if (lastId < Id) { lastId = Id; }
        }

        public BankAccount(string name) : this(lastId, name) { }
        public BankAccount() : this(lastId+1, "Account " + (lastId+1)) { }

        /// <summary>
        /// Открываем счёт для транзакций
        /// </summary>
        /// <param name="bank">Банк (открыть счёт можно только в банке)</param>
        /// <returns>true, если закрытый счёт открыт, false если счёт уже открыт или банк равен null</returns>
        public bool Open(Bank bank, Client client)
        {
            if (bank == null)
            {
                BankAccountStateChanged?.Invoke("Ошибка открытия счёта: банк не задан.");
                return false;
            }
            if (Opened)
            {
                BankAccountStateChanged?.Invoke("Ошибка открытия счёта: счёт уже открыт.");
                return false;
            }
            else
            {
                ClientId = client.Id;
                Opened = true;
                BankAccountStateChanged?.Invoke(ToString() + $" открыт успешно.");
                return true;
            }
        }

        /// <summary>
        /// Закрываем счёт для транзакций
        /// </summary>
        /// <param name="bank">Банк (закрыть счёт можно только в банке)</param>
        /// <returns>true, если отрытый счёт закрыт, false если счёт уже закрыт или банк равен null</returns>
        public bool Close(Bank bank)
        {
            if (bank == null)
            {
                BankAccountStateChanged?.Invoke("Ошибка закрытия счёта: банк не задан.");
                return false;
            }
            if (!Opened)
            {
                BankAccountStateChanged?.Invoke("Ошибка закрытия счёта: счёт уже закрыт.");
                return false;
            }
            else
            {
                Opened = false;
                BankAccountStateChanged?.Invoke(ToString() + $" закрыт успешно.");
                return true;
            }
        }

        /// <summary>
        /// Положить деньги на счёт
        /// </summary>
        /// <param name="bank">Банк (положить деньги на счёт можно только в банке)</param>
        /// <param name="amount">Количество денег</param>
        /// <returns>Сумма пополнения в случае успеха, -1 в случае неудачи</returns>
        public virtual double AddMoney(Bank bank, double amount)
        {
            double amt = ChangeAmount(bank, 1, amount);
            BankAccountStateChanged?.Invoke(ToString() + $": пополнение в размере {amount} {(amt > -1 ? "" : "не")}успешно.");
            return amt;
        }

        /// <summary>
        /// Снять деньги со счёта
        /// </summary>
        /// <param name="bank">Банк (снять деньги со счёта можно только в банке)</param>
        /// <param name="amount">Количество денег</param>
        /// <returns>Сумма снятия в случае успеха, -1 в случае неудачи</returns>
        public virtual double Withdraw(Bank bank, double amount)
        {
            double amt = ChangeAmount(bank, -1, amount);
            BankAccountStateChanged?.Invoke(ToString() + $": снятие средств в размере {amount} {(amt > -1 ? "":"не")}успешно.");
            return amt;
        }

        /// <summary>
        /// Изменение состояния счёта
        /// </summary>
        /// <param name="bank">Банк</param>
        /// <param name="c">c=1 - пополнить, c=-1 - снять</param>
        /// <param name="amount">количество денег</param>
        /// <returns>Положительная сумма изменения в случае успеха, -1 в случае неудачи</returns>
        private double ChangeAmount(Bank bank, sbyte c, double amount)
        {
            if (c != 1 && c != -1 || amount <= 0) throw new Exception("Недопустимый множитель!");
            if (bank != null)
            {
                if (Opened)
                {
                    Balance += c * amount;
                    return amount;
                }
                else
                    throw new Exception("Счёт не открыт!");
            }
            else throw new Exception("Банк не задан!");
        }

        public override string ToString()
        {
            return $"Счёт № {Id}";
        }

        public bool Equals(BankAccount other)
        {
            return Id == other.Id;
        }
    }

    /// <summary>
    /// Депозитный счёт. Баланс может быть только неотрицательным
    /// </summary>
    public class DepositAccount : BankAccount
    {
        public DepositAccount() : base() { Name = $"Депозитный счёт № {Id}"; }
        public DepositAccount(int id, string name) : base(id, name) { }
        public DepositAccount(string name) : base(name) { }

        public override double Withdraw(Bank bank, double amount)
        {
            if (Balance > 0 && amount <= Balance) return base.Withdraw(bank, amount);
            else throw new BankAccountException("Недостаточно средств.");
        }

        public override string ToString()
        {
            return $"Депозитный счёт № {Id}";
        }
    }

    /// <summary>
    /// Недепозитный счёт, баланс может быть любым. В случае отрицательного баланса у клиента становится "плохая" кредитная история.
    /// </summary>
    public class NonDepositAccount : BankAccount
    {
        public NonDepositAccount() : base() { Name = $"Недепозитный счёт № {Id}"; }
        public NonDepositAccount(int id, string name) : base(id, name) { }
        public NonDepositAccount(string name) : base(name) { }

        public override double AddMoney(Bank bank, double amount)
        {
            double addedMoney = base.AddMoney(bank, amount);
            if (Balance > 0) bank.GetClientByAccount<NonDepositAccount>(this).GoodCreditHistory = true;
            return addedMoney;
        }

        public override double Withdraw(Bank bank, double amount)
        {
            double withdrawnMoney = base.Withdraw(bank, amount);
            if (Balance < 0) bank.GetClientByAccount<NonDepositAccount>(this).GoodCreditHistory = false;
            return withdrawnMoney;
        }

        public override string ToString()
        {
            return $"Недепозитный счёт № {Id}";
        }
    }

    public class BankAccountException: Exception
    {
        public BankAccountException(string message) : base(message) { }
    }

}
