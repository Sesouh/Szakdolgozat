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
    /// <summary>
    /// Interaction logic for Kimeno.xaml
    /// </summary>
    public partial class Bejovo : UserControl
    {
        MySqlConnection conn;
        public Bejovo()
        {
            InitializeComponent();
            frissit();
        }

        public void frissit()
        {
            ItemListView.Items.Clear();

            conn = new MySqlConnection(File.ReadAllText("config.txt"));
            conn.Open();
            
            var command_2 = conn.CreateCommand();
            command_2.CommandText = @"
                    SELECT item.nev, item.leiras, kategoria.nev AS 'kategoria' , item.id
                    FROM item
                    LEFT JOIN kategoria 
                    ON kategoria.id = item.kategoria_id";

            using (var reader = command_2.ExecuteReader())
            {
                while (reader.Read())
                {
                    string nev = reader.GetString("nev");
                    string leiras = reader.GetString("leiras");
                    string kategoria = reader.GetString("kategoria");
                    int id = reader.GetInt32("id");

                    ItemListView.Items.Add(new
                    {
                        nev,
                        kategoria,
                        leiras,
                        id
                    });
                }
            }
            conn.Close();

            PartnerekListView.Items.Clear();

            conn = new MySqlConnection(File.ReadAllText("config.txt"));
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = @"
                SELECT id, nev, cim, telefon, email
                FROM partner";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    string nev = reader.GetString("nev");
                    string cim = reader.GetString("cim");
                    string telefon = reader.GetString("telefon");
                    string email = reader.GetString("email");

                    Partner aktualis_partner = new Partner(id, nev, cim, telefon, email);

                    PartnerekListView.Items.Add(aktualis_partner);
                }
            }
            conn.Close();

            BeTranzListView.Items.Clear();

            conn = new MySqlConnection(File.ReadAllText("config.txt"));
            conn.Open();
            var command3 = conn.CreateCommand();
            command3.CommandText = @"
                SELECT id, darabszam, item_id, partner_id, idopont, ido_teljesitve
                FROM tranzakcio
                WHERE darabszam > 0";

            using (var reader = command3.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    int darabszam = reader.GetInt32("darabszam");
                    int item_id = reader.GetInt32("item_id");
                    int partner_id = reader.GetInt32("partner_id");
                    DateTime idopont = reader.GetDateTime("idopont");
                    string ido_teljesitve;
                    try
                    {
                        ido_teljesitve = reader.GetString("ido_teljesitve");
                    }
                    catch (System.Data.SqlTypes.SqlNullValueException)
                    {
                        ido_teljesitve = "Nem teljesített";
                    }

                    Tranzakcio aktualis_tranzakcio = new Tranzakcio(id, darabszam, item_id, idopont, ido_teljesitve, partner_id);
                    BeTranzListView.Items.Add(aktualis_tranzakcio);
                    /*
                    BeTranzListView.Items.Add(new
                    {
                        id,
                        darabszam,
                        item_id,
                        idopont,
                        ido_teljesitve,
                        partner_id
                    });
                    */

                }
            }
            conn.Close();
        }

        private void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemListView.SelectedIndex != -1)
            {
                var kijelolt_dolog = ItemListView.SelectedItem as dynamic;
                bejovoItemTextBox.Text = kijelolt_dolog.nev;
            }
        }

        private void PartnerekListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PartnerekListView.SelectedIndex != -1)
            {
                var kijelolt_dolog = PartnerekListView.SelectedItem as dynamic;
                bejovoPartnerTextBox.Text = kijelolt_dolog.nev;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            bejovoDarabLabel.Content = (int)DarabSlider.Value;
        }

        private void PlusszDarabButton_Click(object sender, RoutedEventArgs e)
        {
            int darab = Convert.ToInt32(bejovoDarabLabel.Content);
            if (darab != 100)
            {
                darab++;
                bejovoDarabLabel.Content = darab;
                DarabSlider.Value = darab;
            }
        }

        private void MinuszDarabButton_Click(object sender, RoutedEventArgs e)
        {
            int darab = Convert.ToInt32(bejovoDarabLabel.Content);
            if (darab != 0)
            {
                darab--;
                bejovoDarabLabel.Content = darab;
                DarabSlider.Value = darab;
            }
        }

        private void BejovoFelvetel_Click(object sender, RoutedEventArgs e)
        {
            if (bejovoItemTextBox.Text != "" &&
                bejovoPartnerTextBox.Text != "" &&
                Convert.ToInt32(bejovoDarabLabel.Content) != 0)
            {
                string s = "";
                s += "Bejövő tárgy: " + bejovoItemTextBox.Text + "\n";
                s += "Darabszám: " + bejovoDarabLabel.Content + "\n";
                s += "Partner: " + bejovoPartnerTextBox.Text + "\n";
                int teljesitve = 0;
                if (TeljesitveCheckBox.IsChecked == true)
                {
                    s += "Teljesítve: Igen\n\n";
                    teljesitve = 1;
                }
                else s += "Teljesítve: Nem\n\n";
                s += "Biztosan felveszed a tranzakciót ?";

                if (MessageBox.Show(s, "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    conn = new MySqlConnection(File.ReadAllText("config.txt"));
                    conn.Open();
                    var kijelolt_item = ItemListView.SelectedItem as dynamic;
                    var kijelolt_partner = PartnerekListView.SelectedItem as dynamic;
                    
                    string commands = @"CALL bejovo_insert('" + kijelolt_item.id +
                                                        "', '" + Convert.ToInt32(bejovoDarabLabel.Content) +
                                                        "', '" + kijelolt_partner.id +
                                                        "', '" + MainWindow.main_nev + 
                                                        "', '" + teljesitve + "')";

                    var cmd_sql_commands = conn.CreateCommand();
                    cmd_sql_commands.CommandText = commands;
                    int result = cmd_sql_commands.ExecuteNonQuery();
                    if (result > 0) MessageBox.Show("Sikeres felvétel!");
                    else MessageBox.Show("Sikertelen felvétel!");
                    conn.Close();

                    PartnerekListView.SelectedIndex = -1;
                    ItemListView.SelectedIndex = -1;
                    bejovoItemTextBox.Text = "";
                    bejovoPartnerTextBox.Text = "";
                    DarabSlider.Value = 0;
                    bejovoDarabLabel.Content = 0;

                    frissit();
                }
            }
            else
            {
                MessageBox.Show("A *-al jelzett mezőket ki kell tölteni!");
            }
            
        }
    }
}
