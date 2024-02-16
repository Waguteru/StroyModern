using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StroyModern
{
    public class CheckUser2
    {
        public string Login2 { get; set; }

        public bool IsAdmin2 { get; }

        public string Status => IsAdmin2 ? "Пользователь" : "Пользователь";
        public CheckUser2(string login2, bool isAdmin2)
        {
            Login2 = login2.Trim();
            IsAdmin2 = isAdmin2;
        }
    }
}
