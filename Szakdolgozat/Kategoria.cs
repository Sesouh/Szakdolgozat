using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szakdolgozat
{
    class Kategoria
    {
        public string nev { get; set; }
        public int id { get; set; }

        public Kategoria()
        {
        }
        public Kategoria(string nev, int id)
        {
            this.nev = nev;
            this.id = id;
        }
    }
}
