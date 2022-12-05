using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomIdentityApp
{
    public class Mailer
    {
        public string login;
        public string password;
        public string server;
        public int port;
        public bool ssl;

        public Mailer()
        { }

        public Mailer(string login, string password, string server, int port, bool ssl)
        {
            this.login = login;
            this.password = password;
            this.server = server;
            this.port = port;
            this.ssl = ssl;
        }

        public Mailer Parse(string str)
        {
            string[] mas = new string[5];
            mas = str.Split(" ");
            
            var m = new Mailer();
            m.login = mas[0];
            m.password = mas[1];
            m.server = mas[2];
            m.port = Convert.ToInt32(mas[3]);
            m.ssl = Convert.ToBoolean(mas[4]);

            return m;
        }
    }
}
