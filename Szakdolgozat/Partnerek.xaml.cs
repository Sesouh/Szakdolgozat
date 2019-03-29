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
    /// Interaction logic for Partnerek.xaml
    /// </summary>
    public partial class Partnerek : UserControl
    {
        MySqlConnection conn;
        public Partnerek()
        {
            InitializeComponent();
            frissites();
        }

        private void PartnerFelvetelButton_Click(object sender, RoutedEventArgs e)
        {
            if (UjPartnerNevTextBox.Text != "" &&
                UjPartnerCimTextBox.Text != "" &&
                UjPartnerTelefonTextBox.Text != "" &&
                UjPartnerEmailTextBox.Text != "")
            {
                conn = new MySqlConnection(File.ReadAllText("config.txt"));
                conn.Open();

                string commands = @"CALL partner_insert('" + UjPartnerNevTextBox.Text +
                                                        "', '" + UjPartnerCimTextBox.Text +
                                                        "', '" + UjPartnerTelefonTextBox.Text +
                                                        "', '" + UjPartnerEmailTextBox.Text +
                                                        "', '" + MainWindow.main_nev + "')";

                var cmd_sql_commands = conn.CreateCommand();
                cmd_sql_commands.CommandText = commands;
                int result = cmd_sql_commands.ExecuteNonQuery();
                if (result > 0) MessageBox.Show("Sikeres felvétel!");
                else MessageBox.Show("Sikertelen felvétel!");
                conn.Close();
            }
            else
            {
                MessageBox.Show("A *-al jelzett mezőket ki kell tölteni!");
            }
            frissites();
        }

        private void PartnerTorlesButton_Click(object sender, RoutedEventArgs e)
        {
            if (PartnerekListView.SelectedIndex != -1)
            {
                if (PartnerekListView.SelectedIndex != -1)
                {
                    conn = new MySqlConnection(File.ReadAllText("config.txt"));
                    conn.Open();

                    var valami = PartnerekListView.SelectedItem as dynamic;

                    var cmd_sql_commands = conn.CreateCommand();
                    cmd_sql_commands.CommandText = @"SELECT COUNT(*)
                                                FROM tranzakcio
                                                JOIN partner ON partner.id=tranzakcio.partner_id
                                                WHERE partner.id = '" + valami.id + "';";

                    long db = (long)cmd_sql_commands.ExecuteScalar();
                    if (db > 0)
                    {
                        MessageBox.Show("Nem lehet törölni a partnert, mert van olyan tranzakció ami használja.");
                        return;
                    }
                    conn.Close();
                }

                if (MessageBox.Show("Biztosan törlöd?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    conn = new MySqlConnection(File.ReadAllText("config.txt"));
                    conn.Open();
                    Partner kijelolt_partner = PartnerekListView.SelectedValue as Partner;
                    var command = conn.CreateCommand();
                    command.CommandText = @"CALL partner_delete('" + kijelolt_partner.id + "','" + MainWindow.main_nev + "','" + kijelolt_partner.nev + "')";

                    int result = command.ExecuteNonQuery();
                    if (result > 0) MessageBox.Show("Sikeres törlés!");
                    else MessageBox.Show("Sikertelen törlés!");

                    conn.Close();
                    frissites();
                }

            }
            else
            {
                MessageBox.Show("Nincs kijelölve semmi!");
            }
        }

        private void frissites()
        {
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
        }
    }
}
