using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szakdolgozat
{
    class Tranzakcio
    {
        public int id { get; set; }
        public int darabszam { get; set; }
        public int item_id { get; set; }
        public DateTime idopont { get; set; }
        public string ido_teljesitve { get; set; }
        public int partner_id { get; set; }

        public Tranzakcio(int id, int darabszam, int item_id, DateTime idopont, string ido_teljesitve, int partner_id)
        {
            this.id = id;
            this.darabszam = darabszam;
            this.item_id = item_id;
            this.idopont = idopont;
            this.ido_teljesitve = ido_teljesitve;
            this.partner_id = partner_id;
        }
    }
}
