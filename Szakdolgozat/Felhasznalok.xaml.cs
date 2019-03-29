using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for Felhasznalok.xaml
    /// </summary>
    public partial class Felhasznalok : UserControl
    {
        MySqlConnection conn;
        public Felhasznalok()
        {
            InitializeComponent();
            
            comboboxokFeltolteseEsGombokZarolasa();
        }

        public void comboboxokFeltolteseEsGombokZarolasa()
        {
            JelszoModButton.IsEnabled = false;
            InactiveButton.IsEnabled = false;
            JogModositButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;

            if (MainWindow.main_jogosultsag == "admin")
            {
                CreateJogComboBox.Items.Add("admin");
                CreateJogComboBox.Items.Add("moderator");
                CreateJogComboBox.Items.Add("user");
                JogJogComboBox.Items.Add("admin");
                JogJogComboBox.Items.Add("moderator");
                JogJogComboBox.Items.Add("user");
            }
            else
            {
                CreateJogComboBox.Items.Add("moderator");
                CreateJogComboBox.Items.Add("user");
                JogJogComboBox.Items.Add("moderator");
                JogJogComboBox.Items.Add("user");
            }
            frissites();
        }

        private void CreatLetrehozButton_Click(object sender, RoutedEventArgs e)
        {
            if (CreateNevTextBox.Text != "" &&
                CreatePasswordBox.Password != "" &&
                CreateMegegyszerPasswordBox.Password == CreatePasswordBox.Password &&
                CreateJogComboBox.Text != "")
            {
                conn = new MySqlConnection(File.ReadAllText("config.txt"));
                conn.Open();

                string commands = @"INSERT INTO `felhasznalok` 
                            (`id`, `felhasznalonev`, `jelszo`, `aktivitas`, `jogosultsag`)
                            VALUES (NULL, '" + CreateNevTextBox.Text + "', '" + CreateMD5(CreatePasswordBox.Password) + "', 'aktiv', '"+ CreateJogComboBox.Text +"')";

                var cmd_sql_commands = conn.CreateCommand();
                cmd_sql_commands.CommandText = commands;
                int result = cmd_sql_commands.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Sikeres felvétel!");
                    string command_log = @"INSERT INTO `log` 
                            (`id`, `felhasznalo_nev`, `muvelet`, `idopont`, `reszletek`)
                            VALUES (NULL, '" + MainWindow.main_nev + "', 'Felhasználó felvétel', CURRENT_TIMESTAMP , '" + CreateNevTextBox.Text + " nevű felhasználó létrejött.')";

                    var cmd_sql_command_log = conn.CreateCommand();
                    cmd_sql_command_log.CommandText = command_log;
                    int result_log = cmd_sql_command_log.ExecuteNonQuery();
                    
                }
                else MessageBox.Show("Sikertelen felvétel!");
                conn.Close();
            }
            else
            {
                MessageBox.Show("A *-al jelzett mezőket ki kell tölteni illetve a jelszavaknak egyeznie kell!!");
            }
            frissites();
        }
        void frissites()
        {
            InactiveComboBox.Items.Clear();
            DeleteComboBox.Items.Clear();
            FelhasznalokListView.Items.Clear();
            JogNevComboBox.Items.Clear();
            JelszoNevComboBox.Items.Clear();

            conn = new MySqlConnection(File.ReadAllText("config.txt"));
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = @"
                SELECT felhasznalonev, jelszo, jogosultsag, aktivitas ,id
                FROM felhasznalok
                ";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string felhasznalonev = reader.GetString("felhasznalonev");
                    string jelszo = reader.GetString("jelszo");
                    string jogosultsag = reader.GetString("jogosultsag");
                    string aktivitas = reader.GetString("aktivitas");
                    int id = reader.GetInt32("id");

                    Felhasznalo aktualis_felhasznalo = new Felhasznalo(felhasznalonev, jelszo, jogosultsag, aktivitas, id);


                    if (aktualis_felhasznalo.jogosultsag != "admin")
                    {
                        DeleteComboBox.Items.Add(aktualis_felhasznalo.felhasznalonev);
                        InactiveComboBox.Items.Add(aktualis_felhasznalo.felhasznalonev);
                        JogNevComboBox.Items.Add(aktualis_felhasznalo.felhasznalonev);
                    }

                    JelszoNevComboBox.Items.Add(aktualis_felhasznalo.felhasznalonev);
                    FelhasznalokListView.Items.Add(new {aktualis_felhasznalo.id,
                                                        aktualis_felhasznalo.felhasznalonev,
                                                        aktualis_felhasznalo.jogosultsag,
                                                        aktualis_felhasznalo.aktivitas});
                }
            }
            conn.Close();
        }

        public string CreateMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteCheckBox.IsChecked == true && DeleteComboBox.Text != "")
            {
                if (DeleteComboBox.Text.Equals(MainWindow.main_nev))
                {
                    MessageBox.Show("Saját felhasználódat nem törölheted!");
                    return;
                }

                conn = new MySqlConnection(File.ReadAllText("config.txt"));
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = @"CALL felhasznalo_delete('"+ DeleteComboBox.Text +"','" + MainWindow.main_nev + "')";

                int result = command.ExecuteNonQuery();
                if (result > 0) MessageBox.Show("Sikeres törlés!");
                else MessageBox.Show("Sikertelen törlés!");
                conn.Close();
                
                frissites();
            }
        }

        private void InactiveButton_Click(object sender, RoutedEventArgs e)
        {
            if (InactiveCheckBox.IsChecked == true && InactiveComboBox.Text != "")
            {
                conn = new MySqlConnection(File.ReadAllText("config.txt"));
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = @"CALL felhasznalo_ujaktivitas('" + InactiveComboBox.Text + "','" + MainWindow.main_nev+"');";

                int result = command.ExecuteNonQuery();
                if (result > 0) MessageBox.Show("Sikeres módosítás!");
                else MessageBox.Show("Sikertelen módosítás!");

                conn.Close();
                frissites();
            }
        }

        private void JogModositButton_Click(object sender, RoutedEventArgs e)
        {
            if (JogModositCheckBox.IsChecked == true && JogJogComboBox.Text != "" && JogNevComboBox.Text != "")
            {
                conn = new MySqlConnection(File.ReadAllText("config.txt"));
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = @"CALL felhasznalo_ujjogosultsag(
                                        '" + JogNevComboBox.Text + "','" +
                                        JogJogComboBox.Text + "','" +
                                        MainWindow.main_nev + "');";

                int result = command.ExecuteNonQuery();
                if (result > 0) MessageBox.Show("Sikeres módosítás!");
                else MessageBox.Show("Sikertelen módosítás!");

                conn.Close();
                frissites();
            }
        }

        private void JelszoModButton_Click(object sender, RoutedEventArgs e)
        {
            if (UjJelszoCheckBox.IsChecked == true &&
                RegiJelszoModTextBox.Password != "" &&
                UjJelszoModTextBox.Password != "" && 
                UjJelszoModTextBox.Password == UjMegegyszerJelszoModTextBox.Password &&
                JelszoNevComboBox.Text != "")
            {
                conn = new MySqlConnection(File.ReadAllText("config.txt"));
                conn.Open();
                var command_ellen = conn.CreateCommand();
                command_ellen.CommandText = @"SELECT felhasznalonev , jelszo
                                            FROM felhasznalok
                                            WHERE felhasznalonev = '"+ JelszoNevComboBox.Text + "';";
                string jelszo = "";
                using (var reader = command_ellen.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        jelszo = reader.GetString("jelszo");
                    }
                }
                conn.Close();

                conn = new MySqlConnection(File.ReadAllText("config.txt"));
                conn.Open();
                if (jelszo == CreateMD5(RegiJelszoModTextBox.Password))
                {
                    var command = conn.CreateCommand();
                    command.CommandText = @"CALL felhasznalo_ujjelszo(
                                        '" + JelszoNevComboBox.Text + "','" +
                                            CreateMD5(UjJelszoModTextBox.Password) + "','" +
                                            MainWindow.main_nev + "');";
                    int result = command.ExecuteNonQuery();
                    if (result > 0) MessageBox.Show("Sikeres módosítás!");
                    else MessageBox.Show("Sikertelen módosítás!");
                }
                else
                {
                    MessageBox.Show("Hibás jelszót adtál meg!");
                }
                conn.Close();
                frissites();
            }
        }

        private void UjJelszoCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (UjJelszoCheckBox.IsChecked.Value) JelszoModButton.IsEnabled = true;
            else JelszoModButton.IsEnabled = false;
        }

        private void JogModositCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (JogModositCheckBox.IsChecked.Value) JogModositButton.IsEnabled = true;
            else JogModositButton.IsEnabled = false;
        }

        private void InactiveCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (InactiveCheckBox.IsChecked.Value) InactiveButton.IsEnabled = true;
            else InactiveButton.IsEnabled = false;
        }

        private void DeleteCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteCheckBox.IsChecked.Value) DeleteButton.IsEnabled = true;
            else DeleteButton.IsEnabled = false;
        }
    }
}
