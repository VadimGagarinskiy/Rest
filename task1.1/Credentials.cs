using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task1._1
{
    public class Credentials
    {
        private string username { get; set; }
        private string password { get; set; }

        public Credentials(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
