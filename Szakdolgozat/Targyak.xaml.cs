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
    /// Interaction logic for Targyak.xaml
    /// </summary>
    public partial class Targyak : UserControl
    {
        MySqlConnection conn;
        public Targyak()
        {
            InitializeComponent();
            frissites();
        }

        void frissites()
        {
            KategoriaComboBox.Items.Clear();
            KategoriaListBox.Items.Clear();
            ItemListView.Items.Clear();

            conn = new MySqlConnection(File.ReadAllText("config.txt"));
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = @"
                SELECT nev, id
                FROM kategoria";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string nev = reader.GetString("nev");
                    int id = reader.GetInt32("id");

                    Kategoria kategoria = new Kategoria(nev,id);
                    
                    KategoriaComboBox.Items.Add(kategoria);
                    KategoriaListBox.Items.Add(kategoria.nev);
                }
            }
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
        }

        private void ItemFelvetelButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemNevTextBox.Text != "" &&
                KategoriaComboBox.Text != "" &&
                ItemLeirasTextBox.Text != "")
            {
                conn = new MySqlConnection(File.ReadAllText("config.txt"));
                conn.Open();

                string commands = @"CALL item_insert('" + ItemNevTextBox.Text +
                                                "', '" + ItemLeirasTextBox.Text +
                                                "','" + KategoriaComboBox.SelectedValue + 
                                                "','" + MainWindow.main_nev +"')";
                
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

        private void KategoriaFelvetelButton_Click(object sender, RoutedEventArgs e)
        {
            if (KategoriaNevTextBox.Text != "")
            {
                conn = new MySqlConnection(File.ReadAllText("config.txt"));
                conn.Open();

                string commands = @"CALL kategoria_insert('"+ KategoriaNevTextBox.Text + "','"+MainWindow.main_nev+"')";

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

        private void DeleteKategoriaButton_Click(object sender, RoutedEventArgs e)
        {

            if (KategoriaListBox.SelectedIndex != -1)
            {
                conn = new MySqlConnection(File.ReadAllText("config.txt"));
                conn.Open();
                var cmd_sql_commands = conn.CreateCommand();
                cmd_sql_commands.CommandText = @"SELECT COUNT(*)
                                                FROM item
                                                JOIN kategoria ON kategoria.id=item.kategoria_id
                                                WHERE kategoria.nev = '" + KategoriaListBox.SelectedValue + "';";

                long db = (long)cmd_sql_commands.ExecuteScalar();
                if (db > 0)
                {
                    MessageBox.Show("Nem lehet törölni a kategóriát, mert van olyan tárgy ami használja.");
                    return;
                }
                conn.Close();

                if (MessageBox.Show("Biztosan törlöd?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    conn = new MySqlConnection(File.ReadAllText("config.txt"));
                    conn.Open();
                    var command = conn.CreateCommand();
                    command.CommandText = @"CALL kategoria_delete('"+ KategoriaListBox.SelectedValue + "','"+MainWindow.main_nev+"')";

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

        private void DeleteItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemListView.SelectedIndex != -1)
            {
                conn = new MySqlConnection(File.ReadAllText("config.txt"));
                conn.Open();

                var valami = ItemListView.SelectedItem as dynamic;

                var cmd_sql_commands = conn.CreateCommand();
                cmd_sql_commands.CommandText = @"SELECT COUNT(*)
                                                FROM tranzakcio
                                                JOIN item ON item.id=tranzakcio.item_id
                                                WHERE item.id = '" + valami.id + "';";

                long db = (long)cmd_sql_commands.ExecuteScalar();
                if (db > 0)
                {
                    MessageBox.Show("Nem lehet törölni a kategóriát, mert van olyan tranzakció ami használja.");
                    return;
                }
                conn.Close();

                if (MessageBox.Show("Biztosan törlöd?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    conn = new MySqlConnection(File.ReadAllText("config.txt"));
                    conn.Open();
                    var kijelolt_targy = ItemListView.SelectedItem as dynamic;
                    var command = conn.CreateCommand();
                    command.CommandText = @"CALL item_delete('" + kijelolt_targy.id +
                                                            "','" + kijelolt_targy.nev +
                                                            "','" + MainWindow.main_nev + "')";

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
            frissites();
            
        }
    }
}
