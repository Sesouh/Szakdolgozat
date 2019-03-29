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
    /// Interaction logic for Beallitasok.xaml
    /// </summary>
    public partial class Beallitasok : UserControl
    {
        MySqlConnection conn;
        public Beallitasok()
        {
            InitializeComponent();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".sql";
                dlg.Filter = "SQL Files (*.sql)|*.sql";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    // Open document 
                    string file = dlg.FileName;
                    using (conn = new MySqlConnection(File.ReadAllText("config.txt")))
                    {
                        conn.Open();
                        var sc = new MySqlScript(conn, File.ReadAllText(file));
                        sc.Execute();
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sikertelen importálás: " + ex.ToString());
            }
            MessageBox.Show("Sikeres importálás!");
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".sql";
                dlg.Filter = "SQL Files (*.sql)|*.sql";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    // Open document 
                    string file = dlg.FileName;

                    using (conn = new MySqlConnection(File.ReadAllText("config.txt")))
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            using (MySqlBackup mb = new MySqlBackup(cmd))
                            {
                                cmd.Connection = conn;
                                conn.Open();
                                mb.ExportToFile(file);
                                conn.Close();
                            }
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sikertelen exportálás: " + ex.ToString());
            }
            MessageBox.Show("Sikeres exportálás!");
        }
    }
}
