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
using System.Windows.Shapes;

namespace Szakdolgozat
{
    
    public partial class Login : Window
    {
        MySqlConnection conn;
        int f_id;
        string f_nev;
        string f_jelszo;
        string f_aktivitas;
        string f_jogosultsag;
        bool felhasznaloZartaBe = true;
        

        public Login()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn = new MySqlConnection(File.ReadAllText("config.txt"));
                conn.Open();
            }
            catch (MySqlException exc)
            {
                MessageBox.Show("Nem sikerült a kapcsolódás: " + exc);
                return;
            }
            var command = conn.CreateCommand();
            command.CommandText = @"
                SELECT * FROM felhasznalok WHERE felhasznalonev = @nev
            ";
            command.Parameters.AddWithValue("@nev", LoginFelhasznaloTextBox.Text);
            
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        f_id = reader.GetInt32("id");
                        f_nev = reader.GetString("felhasznalonev");
                        f_jelszo = reader.GetString("jelszo");
                        f_aktivitas = reader.GetString("aktivitas");
                        f_jogosultsag = reader.GetString("jogosultsag");
                    }
                }
            
            

            if (f_nev == LoginFelhasznaloTextBox.Text && f_jelszo == CreateMD5(LoginJelszoPasswordBox.Password) && f_aktivitas == "aktiv")
            {
                MainWindow.main_id = f_id;
                MainWindow.main_nev = f_nev;
                MainWindow.main_jelszo = f_jelszo;
                MainWindow.main_aktivitas = f_aktivitas;
                MainWindow.main_jogosultsag = f_jogosultsag;
                felhasznaloZartaBe = false;
                this.Close();
                
            }
            else
            {
                MessageBox.Show("Hibás felhasználónév vagy jelszó.");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (felhasznaloZartaBe) CloseAllWindows();
        }

        private void CloseAllWindows()
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
                App.Current.Windows[intCounter].Close();
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
    }
}
