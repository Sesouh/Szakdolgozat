using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szakdolgozat
{
    class Felhasznalo
    {
        public string felhasznalonev { get; set; }
        public string jelszo { get; set; }
        public string jogosultsag { get; set; }
        public string aktivitas { get; set; }
        public int id { get; set; }

        public Felhasznalo(string felhasznalonev, string jelszo, string jogosultsag, string aktivitas, int id)
        {
            this.felhasznalonev = felhasznalonev;
            this.jelszo = jelszo;
            this.jogosultsag = jogosultsag;
            this.aktivitas = aktivitas;
            this.id = id;
        }
        
    }
}
