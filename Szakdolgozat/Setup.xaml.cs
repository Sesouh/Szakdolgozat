using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
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
using System.Windows.Shapes;
using System.Threading;

namespace Szakdolgozat
{
    
    public partial class Setup : Window
    {
        bool felhasznaloZartaBe = true;
        public Setup()
        {
            InitializeComponent();
            
        }

        private void SetupKapcsolodasButton_Click(object sender, RoutedEventArgs e)
        {
            if (SetupServerCimTextBox.Text != "" &&
                SetupAdatbazisNevTextBox.Text != "" &&
                SetupAdatbazisFelhasznaloTextBox.Text != "")
            {
                string connserver = "Server=" + SetupServerCimTextBox.Text;
                string connport = ";Port=" + SetupServerPortTextBox.Text;
                string conndatabase = ";Database=" + SetupAdatbazisNevTextBox.Text;
                string connuid = ";Uid=" + SetupAdatbazisFelhasznaloTextBox.Text;
                string connpw = ";Pwd=" + SetupAdatbazisPasswordBox.Password;
                string connutf = ";Character Set = utf8";
                string commands = "CREATE DATABASE IF NOT EXISTS " + SetupAdatbazisNevTextBox.Text + " CHARACTER SET utf8 COLLATE utf8_hungarian_ci;";
                commands += "USE " + SetupAdatbazisNevTextBox.Text + ";";

                try
                {
                    commands += File.ReadAllText(@"database_source.sql");
                }
                catch (Exception exc)
                {
                    MessageBox.Show("" + exc.Message);
                    return;
                }

                MySqlConnection conn = new MySqlConnection(connserver + connport + connuid + connpw + connutf);
                conn.Open();
                try
                {
                    var cmd_sql_commands = conn.CreateCommand();
                    cmd_sql_commands.CommandText = commands;
                    cmd_sql_commands.ExecuteNonQuery();
                }
                catch (Exception exc)
                {
                    MessageBox.Show("" + exc.Message);
                    return;
                }
                conn.Close();

                try
                {
                    File.WriteAllText("config.txt", connserver + connport + conndatabase + connuid + connpw + connutf);
                }
                catch (Exception exc)
                {
                    MessageBox.Show("" + exc.Message);
                    return;
                }

                MessageBox.Show("Sikeres kapcsolódás");
                felhasznaloZartaBe = false;
                
                this.Close();
            }
            else
            {
                MessageBox.Show("A *-al jelzett mezőket ki kell tölteni!");
            }
        }

        private void SetupTesztButton_Click(object sender, RoutedEventArgs e)
        {

            

            if (SetupServerCimTextBox.Text != "" &&
                SetupAdatbazisNevTextBox.Text != "" &&
                SetupAdatbazisFelhasznaloTextBox.Text != "")
            {
                string connserver = "Server=" + SetupServerCimTextBox.Text;
                string connport = ";Port=" + SetupServerPortTextBox.Text;
                string conndatabase = ";Database=" + SetupAdatbazisNevTextBox.Text;
                string connuid = ";Uid=" + SetupAdatbazisFelhasznaloTextBox.Text;
                string connpw = ";Pwd=" + SetupAdatbazisPasswordBox.Password;
                
                

                try
                {
                    MySqlConnection conn = new MySqlConnection(connserver + connport + connuid + connpw);
                    conn.Open();

                    var cmd_sql_commands = conn.CreateCommand();
                    cmd_sql_commands.CommandText = "SHOW DATABASES";
                    var reader = cmd_sql_commands.ExecuteReader();

                    string elen = "";

                    while (reader.Read())
                    {
                        elen += reader[0] + "\n";
                    }
                    
                    conn.Close();

                    SetupTesztLabel.Foreground = new SolidColorBrush(Colors.Green);
                    SetupTesztLabel.Content = "Sikeres kapcsolódás!";
                }
                catch (Exception)
                {
                    SetupTesztLabel.Foreground = new SolidColorBrush(Colors.Red);
                    SetupTesztLabel.Content = "Sikertelen kapcsolódás";
                }

                SetupProgressBar.Value = 0;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (felhasznaloZartaBe) Environment.Exit(0);
        }
        
    }
}
