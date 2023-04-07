using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Practice14_Bank
{
    /// <summary>
    /// Обычный клиент
    /// </summary>
    public class Client : IEquatable<Client>
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public ObservableCollection<BankAccount> BankAccounts { get; set; }
        public double Cash { get; set; } // наличные персонажа
        public double Debt { get; set; } // долг персонажа перед банком
        public bool GoodCreditHistory { get; set; } = true;
        public int Id { get; set; }
        private static int lastId = -1;
        public string ClassName { get; }
        [JsonIgnore]public BitmapImage Image { get; } = new BitmapImage();
        private string imName;
        public string ImageName { get { return imName; }
            set {
                if (value != null)
                {
                    imName = value;
                    LoadImage(value);
                }
            }
        }
        public bool Edited { get; set; } = false;
        public static event Action<string> ClientCreated;

        public Client() : this("Фамилия", "Имя", "Отчество") { }
 
        public Client(string LastName, string FirstName, string MiddleName) : this(lastId + 1, LastName, FirstName, MiddleName) { }

        public Client(int Id, string LastName, string FirstName, string MiddleName)
        {
            this.Id = Id;
            this.FirstName = FirstName;
            this.MiddleName = MiddleName;
            this.LastName = LastName;
            BankAccounts = new ObservableCollection<BankAccount>();
            ClassName = GetType().Name;
            if (Id <= lastId)
                ClientCreated?.Invoke($"Клиент {Id} {ToString()} отредактирован");
            else
                ClientCreated?.Invoke($"Клиент {Id} {ToString()} создан");
            if (lastId < Id) { lastId = Id; }
        }

        public Client(int Id, string LastName, string FirstName, string MiddleName, string imageName) : this(Id, LastName, FirstName, MiddleName)
        {
            LoadImage(imageName);
        }

        /// <summary>
        /// Загрузка изображения для "аватара" клиента
        /// </summary>
        /// <param name="imageName">Путь к картинке</param>
        private void LoadImage(string imageName)
        {
            if (imageName != string.Empty && imageName != null)
            {
                imName = imageName;
                Image.BeginInit();
                Image.UriSource = new Uri(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, imageName));
                Image.EndInit();
            }
        }

        public override string ToString()
        {
            return LastName + " " + FirstName + " " + MiddleName;
        }

        public bool Equals (Client other)
        {
            return Id == other.Id && FirstName == other.FirstName && MiddleName == other.MiddleName && LastName == other.LastName && ClassName == other.ClassName;
        }
    }

    /// <summary>
    /// Клиент юридическое лицо
    /// </summary>
    public class LegalClient : Client
    {
        public double TaxRate { get; set; } = 0.2;
        public LegalClient() : base() { }
        public LegalClient(string LastName, string FirstName, string MiddleName) : base(LastName, FirstName, MiddleName) { }
        public LegalClient(int Id, string LastName, string FirstName, string MiddleName) : base(Id, LastName, FirstName, MiddleName) { }
        public LegalClient(int Id, string LastName, string FirstName, string MiddleName, double taxRate) : base(Id, LastName, FirstName, MiddleName) { TaxRate = taxRate; }
        public LegalClient(int Id, string LastName, string FirstName, string MiddleName, double taxRate, string imageName) : base(Id, LastName, FirstName, MiddleName, imageName) { TaxRate = taxRate; }
}

    /// <summary>
    /// Клиент VIP
    /// </summary>
    public class VIPClient : LegalClient
    {
        public double VipRating { get; set; } = 1;
        public VIPClient() : base() { }
        public VIPClient(string LastName, string FirstName, string MiddleName) : base(LastName, FirstName, MiddleName) { }
        public VIPClient(int Id, string LastName, string FirstName, string MiddleName) : base(Id, LastName, FirstName, MiddleName) { }
        public VIPClient(int Id, string LastName, string FirstName, string MiddleName, double taxRate) : base(Id, LastName, FirstName, MiddleName) { TaxRate = taxRate; }
        public VIPClient(int Id, string LastName, string FirstName, string MiddleName, double taxRate, double vipRating) : this(Id, LastName, FirstName, MiddleName, taxRate) { VipRating = vipRating; }
        public VIPClient(int Id, string LastName, string FirstName, string MiddleName, double taxRate, double vipRating, string imageName) : base(Id, LastName, FirstName, MiddleName, taxRate, imageName) { VipRating = vipRating; }
    }
}
