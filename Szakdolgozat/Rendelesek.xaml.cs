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
    /// Interaction logic for Rendelesek.xaml
    /// </summary>
    public partial class Rendelesek : UserControl
    {
        MySqlConnection conn;

        public Rendelesek()
        {
            InitializeComponent();
            frissit();
        }

        public void frissit()
        {
            RendelesekListView.Items.Clear();
            conn = new MySqlConnection(File.ReadAllText("config.txt"));
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = @"SELECT tranzakcio.id,item.nev as 'itemnev',darabszam,partner.nev as 'partnernev',idopont,ido_teljesitve 
                                    FROM tranzakcio
                                    LEFT JOIN item
                                    ON tranzakcio.item_id = item.id
                                    LEFT JOIN partner
                                    ON tranzakcio.partner_id = partner.id
                                    GROUP BY id DESC";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    string itemnev = reader.GetString("itemnev");
                    int darabszam = reader.GetInt32("darabszam");
                    string partnernev = reader.GetString("partnernev");
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
                    string tipus;
                    if (darabszam > 0) tipus = "Bejövő tranzakció";
                    else tipus = "Kimenő tranzakció";

                    RendelesekListView.Items.Add(new
                    {
                        id,
                        itemnev,
                        darabszam,
                        partnernev,
                        idopont,
                        ido_teljesitve,
                        tipus
                    });
                }
            }

            conn.Close();
        }

        private void TeljesitButton_Click(object sender, RoutedEventArgs e)
        {
            if (RendelesekListView.SelectedIndex != -1)
            {
                var kijelolt_rendeles = RendelesekListView.SelectedValue as dynamic;
                
                if (kijelolt_rendeles.ido_teljesitve == "Nem teljesített")
                {

                    if (MessageBox.Show("Biztosan módosítod?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        conn = new MySqlConnection(File.ReadAllText("config.txt"));
                        conn.Open();

                        var command = conn.CreateCommand();
                        if (kijelolt_rendeles.darabszam > 0)
                        {
                            command.CommandText = @"CALL tranzakcio_update('" + kijelolt_rendeles.id + "','" + MainWindow.main_nev + "','Bejövő tranzakció módosítás')";
                        }
                        else
                        {
                            command.CommandText = @"CALL tranzakcio_update('" + kijelolt_rendeles.id + "','" + MainWindow.main_nev + "','Kimenő tranzakció módosítás')";
                        }
                        int result = command.ExecuteNonQuery();
                        if (result > 0) MessageBox.Show("Sikeres módosítás!");
                        else MessageBox.Show("Sikertelen módosítás!");

                        conn.Close();
                        frissit();
                    }

                }
                else
                {
                    MessageBox.Show("Már teljesítve van a rendelés!");
                }
            }
            else
            {
                MessageBox.Show("Nincs kijelölve semmi!");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (RendelesekListView.SelectedIndex != -1)
            {
                if (MessageBox.Show("Biztosan törlöd?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    conn = new MySqlConnection(File.ReadAllText("config.txt"));
                    conn.Open();
                    var kijelolt_rendeles = RendelesekListView.SelectedValue as dynamic;
                    var command = conn.CreateCommand();
                    if (kijelolt_rendeles.darabszam > 0)
                    {
                        command.CommandText = @"CALL tranzakcio_delete('" + kijelolt_rendeles.id + "','" + MainWindow.main_nev + "','Bejövő tranzakció törlés')";
                    }
                    else
                    {
                        command.CommandText = @"CALL tranzakcio_delete('" + kijelolt_rendeles.id + "','" + MainWindow.main_nev + "','Kimenő tranzakció törlés')";
                    }
                    int result = command.ExecuteNonQuery();
                    if (result > 0) MessageBox.Show("Sikeres törlés!");
                    else MessageBox.Show("Sikertelen törlés!");

                    conn.Close();
                    frissit();
                }
            }
            else
            {
                MessageBox.Show("Nincs kijelölve semmi!");
            }
        }
    }
}
