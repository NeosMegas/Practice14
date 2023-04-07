using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice14_Bank
{
    public class BankEventInfo
    {
        public DateTime DateTime { get; set; }
        public string Info { get; set; }
        public BankEventInfo(string info)
        {
            Info = info;
            DateTime = DateTime.Now;
        }

        public override string ToString()
        {
            return DateTime.ToString("yyyy.MM.dd hh:mm:ss") + Info;
        }
    }
}
