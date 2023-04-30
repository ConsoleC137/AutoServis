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
using System.Data.SQLite;
using System.Data.SqlClient;

namespace AutoServis.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageAuth.xaml
    /// </summary>
    public partial class PageAuth : Page
    {
        private Frame Frame;
        public PageAuth(Frame frame)
        {
            InitializeComponent();
            this.Frame = frame;
        }

        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "SELECT id FROM admins WHERE login = @login AND password = @password";
                    command.Parameters.AddWithValue("@login", textBoxLogin.Text);
                    command.Parameters.AddWithValue("@password", passwordBox.Password);

                    var result = command.ExecuteScalar();

                    if (result != null)
                    {
                        MessageBox.Show("Вы админ");
                    }
                    else
                    {
                        command.CommandText = "SELECT id FROM masters WHERE login = @login AND password = @password";
                        command.Parameters.AddWithValue("@login", textBoxLogin.Text);
                        command.Parameters.AddWithValue("@password", passwordBox.Password);

                        result = command.ExecuteScalar();

                        if (result != null)
                        {
                            MessageBox.Show("Вы мастер");
                        }
                        else
                        {
                            LabelAuth.Content = "Неверный логин/пароль";
                        }
                    }
                }
            }
        }
    }
}
