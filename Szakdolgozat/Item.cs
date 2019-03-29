using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szakdolgozat
{
    class Item
    {
        public string nev { get; set; }
        public string leiras { get; set; }
        public int id { get; set; }
        public int kategoria_id { get; set; }

        public Item(string nev, string leiras, int id, int kategoria_id)
        {
            this.nev = nev;
            this.leiras = leiras;
            this.id = id;
            this.kategoria_id = kategoria_id;
        }


    }
}
