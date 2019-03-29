using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szakdolgozat
{
    class Partner
    {
        public int id { get; set; }
        public string nev { get; set; }
        public string cim { get; set; }
        public string telefon { get; set; }
        public string email { get; set; }

        public Partner(int id, string nev, string cim, string telefon, string email)
        {
            this.id = id;
            this.nev = nev;
            this.cim = cim;
            this.telefon = telefon;
            this.email = email;
        }
    }
}
