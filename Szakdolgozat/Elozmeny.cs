using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szakdolgozat
{
    class Elozmeny
    {
        private int id { get; }
        private string felhasznalonev { get; }
        private string muvelet { get; }
        private DateTime idopont { get; }
        private string reszletek { get; }

        public Elozmeny(int id, string felhasznalonev, string muvelet, DateTime idopont, string reszletek)
        {
            this.id = id;
            this.felhasznalonev = felhasznalonev;
            this.muvelet = muvelet;
            this.idopont = idopont;
            this.reszletek = reszletek;
        }
        
    }
}
