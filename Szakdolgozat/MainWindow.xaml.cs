using System;
using System.Collections.Generic;
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
using System.IO;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;

namespace Szakdolgozat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int main_id;
        public static string main_nev;
        public static string main_jelszo;
        public static string main_aktivitas;
        public static string main_jogosultsag;
        public static bool bezarul = false;

        public MainWindow()
        {
            
            config_betoltes();
            admin_check();
            InitializeComponent();
            bejelentkezes();
            funkciokZarolasa();

        }

        private void funkciokZarolasa()
        {
            beallitasokTab.Content = null;
            itemsTab.Content = null;
            felhasznalokTab.Content = null;
            elozmenyekTab.Content = null;
            partnersTab.Content = null;
            bejovoTab.Content = new Bejovo();
            kimenoTab.Content = null;
            rendelesekTab.Content = null;

            FunkciokTabControl.SelectedIndex = 0;
            felhasznalokTab.IsEnabled = true;
            beallitasokTab.IsEnabled = true;
            switch (main_jogosultsag)
            {
                case "admin":
                    FelhasznaloNevLabel.Foreground = Brushes.Orange;
                    FelhasznaloJogosultsagLabel.Foreground = Brushes.Orange;
                    break;
                case "moderator":
                    FelhasznaloNevLabel.Foreground = Brushes.Green;
                    FelhasznaloJogosultsagLabel.Foreground = Brushes.Green;
                    beallitasokTab.IsEnabled = false;
                    break;
                case "user":
                    FelhasznaloNevLabel.Foreground = Brushes.Blue;
                    FelhasznaloJogosultsagLabel.Foreground = Brushes.Blue;
                    felhasznalokTab.IsEnabled = false;
                    beallitasokTab.IsEnabled = false;
                    break;
            }
            FelhasznaloNevLabel.Content = main_nev;
            FelhasznaloJogosultsagLabel.Content = main_jogosultsag;
            if (!bezarul)  this.Visibility = Visibility.Visible;
        }

        void admin_check()
        {
            MySqlConnection conn = new MySqlConnection();

            try
            {
                conn = new MySqlConnection(File.ReadAllText("config.txt"));
                conn.Open();
                var cmd_sql_commands = conn.CreateCommand();
                cmd_sql_commands.CommandText = "SELECT COUNT(*) FROM felhasznalok";

                long db = (long)cmd_sql_commands.ExecuteScalar();
                if (db < 1)
                {
                    AdminCreat admin_creat = new AdminCreat();
                    admin_creat.ShowDialog();
                }
            }
            catch (MySqlException exc)
            {
                MessageBox.Show("Nem sikerült kapcsolódni az adatbázishoz: " + exc.Message);
                
                conn.Close();
                Setup setup_window = new Setup();
                setup_window.ShowDialog();
                admin_check();
            }
            
        }

        void config_betoltes()
        {
            if (!File.Exists("config.txt"))
            {
                Setup setup_window = new Setup();
                setup_window.ShowDialog();
            }

            if (!File.Exists("config.txt")) this.Close();

        }

        public void bejelentkezes()
        {
            Login login_window = new Login();
            login_window.ShowDialog();
            if (!bezarul)
            {
                funkciokZarolasa();
            }
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

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            
            this.Visibility = Visibility.Collapsed;
            main_id = 0;
            main_nev = null;
            main_jelszo = null;
            main_aktivitas = null;
            main_jogosultsag = null;
            bejelentkezes();
        }
        
        

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bezarul = true;
        }

        private void FunkciokTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (itemsTab.IsSelected && itemsTab.Content == null)
            {
                itemsTab.Content = new Targyak();
                elozmenyekTab.Content = null;
                felhasznalokTab.Content = null;
                partnersTab.Content = null;
                bejovoTab.Content = null;
                kimenoTab.Content = null;
                rendelesekTab.Content = null;
                beallitasokTab.Content = null;
            }

            if (elozmenyekTab.IsSelected && elozmenyekTab.Content == null)
            {
                elozmenyekTab.Content = new Elozmenyek();
                itemsTab.Content = null;
                felhasznalokTab.Content = null;
                partnersTab.Content = null;
                bejovoTab.Content = null;
                kimenoTab.Content = null;
                rendelesekTab.Content = null;
                beallitasokTab.Content = null;
            }

            if (felhasznalokTab.IsSelected && felhasznalokTab.Content == null)
            {
                felhasznalokTab.Content = new Felhasznalok();
                itemsTab.Content = null;
                elozmenyekTab.Content = null;
                partnersTab.Content = null;
                bejovoTab.Content = null;
                kimenoTab.Content = null;
                rendelesekTab.Content = null;
                beallitasokTab.Content = null;
            }

            if (partnersTab.IsSelected && partnersTab.Content == null)
            {
                partnersTab.Content = new Partnerek();
                itemsTab.Content = null;
                elozmenyekTab.Content = null;
                felhasznalokTab.Content = null;
                bejovoTab.Content = null;
                kimenoTab.Content = null;
                rendelesekTab.Content = null;
                beallitasokTab.Content = null;
            }
            if (bejovoTab.IsSelected && bejovoTab.Content == null)
            {
                bejovoTab.Content = new  Bejovo();
                itemsTab.Content = null;
                elozmenyekTab.Content = null;
                felhasznalokTab.Content = null;
                partnersTab.Content = null;
                kimenoTab.Content = null;
                rendelesekTab.Content = null;
                beallitasokTab.Content = null;
            }
            if (kimenoTab.IsSelected && kimenoTab.Content == null)
            {
                kimenoTab.Content = new Kimeno();
                bejovoTab.Content = null;
                itemsTab.Content = null;
                elozmenyekTab.Content = null;
                felhasznalokTab.Content = null;
                partnersTab.Content = null;
                rendelesekTab.Content = null;
                beallitasokTab.Content = null;
            }
            if (rendelesekTab.IsSelected && rendelesekTab.Content == null)
            {
                rendelesekTab.Content = new Rendelesek();
                bejovoTab.Content = null;
                itemsTab.Content = null;
                elozmenyekTab.Content = null;
                felhasznalokTab.Content = null;
                partnersTab.Content = null;
                kimenoTab.Content = null;
                beallitasokTab.Content = null;
            }
            if (beallitasokTab.IsSelected && beallitasokTab.Content == null)
            {
                beallitasokTab.Content = new Beallitasok();
                bejovoTab.Content = null;
                itemsTab.Content = null;
                elozmenyekTab.Content = null;
                felhasznalokTab.Content = null;
                partnersTab.Content = null;
                kimenoTab.Content = null;
                rendelesekTab.Content = null;
            }
        }
    }
}
