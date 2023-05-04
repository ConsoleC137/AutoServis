using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;

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
            Frame = frame;
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
                        Frame.Navigate(new Pages.PageAdmin(Frame));
                    else
                    {
                        command.CommandText = "SELECT id FROM masters WHERE login = @login AND password = @password";
                        command.Parameters.AddWithValue("@login", textBoxLogin.Text);
                        command.Parameters.AddWithValue("@password", passwordBox.Password);

                        result = command.ExecuteScalar();

                        if (result != null)
                            Frame.Navigate(new Pages.PageMaster(Frame));
                        else
                            LabelAuth.Content = "Неверный логин/пароль";
                    }
                }
            }
        }
    }
}
