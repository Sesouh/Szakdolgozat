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
    
    public partial class AdminCreat : Window
    {
        bool felhasznaloZartaBe = true;
        public AdminCreat()
        {
            InitializeComponent();
        }
        

        private void AdminCreatLetrehozButton_Click(object sender, RoutedEventArgs e)
        {
            if (AdminCreatAdminNevTextBox.Text != "" &&
                AdminCreatAdminPasswordBox.Password != "" &&
                AdminCreatAdminMegegyszerPasswordBox.Password == AdminCreatAdminPasswordBox.Password)
            {
                MySqlConnection conn = new MySqlConnection(File.ReadAllText("config.txt"));
                conn.Open();

                string commands = @"INSERT INTO `felhasznalok` 
                            (`id`, `felhasznalonev`, `jelszo`, `aktivitas`, `jogosultsag`)
                            VALUES (NULL, '" + AdminCreatAdminNevTextBox.Text + "', '" + CreateMD5(AdminCreatAdminPasswordBox.Password) + "', 'aktiv', 'admin')";

                var cmd_sql_commands = conn.CreateCommand();
                cmd_sql_commands.CommandText = commands;
                int result = cmd_sql_commands.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Sikeres felvétel!");
                    string command_log = @"INSERT INTO `log` 
                            (`id`, `felhasznalo_nev`, `muvelet`, `idopont`, `reszletek`)
                            VALUES (NULL, 'SYSTEM', 'Felhasználó felvétel', CURRENT_TIMESTAMP , '" + AdminCreatAdminNevTextBox.Text + " nevű felhasználó létrejött.')";

                    var cmd_sql_command_log = conn.CreateCommand();
                    cmd_sql_command_log.CommandText = command_log;
                    int result_log = cmd_sql_command_log.ExecuteNonQuery();
                }
                else MessageBox.Show("Sikertelen felvétel!");
                conn.Close();
                felhasznaloZartaBe = false;
                this.Close();
                
            }
            else
            {
                MessageBox.Show("A *-al jelzett mezőket ki kell tölteni illetve a jelszavaknak egyeznie kell!");
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
