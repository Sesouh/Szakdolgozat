using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Szakdolgozat
{
    
    public partial class Elozmenyek : UserControl
    {
        
        public Elozmenyek()
        {
            InitializeComponent();
            frissit();
        }

        public void frissit()
        {
            ElozmenyekListView.Items.Clear();
            MySqlConnection conn = new MySqlConnection(File.ReadAllText("config.txt"));
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = @"SELECT * FROM log
                                    GROUP BY id DESC";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    string felhasznalonev = reader.GetString("felhasznalo_nev");
                    string muvelet = reader.GetString("muvelet");
                    DateTime idopont = reader.GetDateTime("idopont");
                    string reszletek = reader.GetString("reszletek");
                    

                    Elozmeny aktualis_elozmeny = new Elozmeny(id, felhasznalonev, muvelet, idopont, reszletek);
                    
                    ElozmenyekListView.Items.Add(new { id,felhasznalonev,muvelet,idopont,reszletek});
                }
            }
            conn.Close();
        }
    }
}
