using System;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace AutoServis.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageChangeAdmins.xaml
    /// </summary>
    public partial class PageChangeAdmins : Page
    {
        private Frame Frame;
        public ObservableCollection<Admin> Admins { get; set; }
        public PageChangeAdmins(Frame frame)
        {
            InitializeComponent();
            Frame = frame;
            Loaded += PageChangeAdmins_Loaded;
            Admins = new ObservableCollection<Admin>();
            DataContext = this;
            dataGridAdmins.CellEditEnding += DataGridAdmins_CellEditEnding;
        }

        private void DataGridAdmins_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var textBox = e.EditingElement as TextBox;
            var newValue = textBox.Text;

            var admin = e.Row.Item as Admin;

            if (e.Column.Header.ToString() == "ФИО")
                admin.FIO = newValue;
            else if (e.Column.Header.ToString() == "Логин")
                admin.Login = newValue;
            else if (e.Column.Header.ToString() == "Пароль")
                admin.Password = newValue;

            string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE admins SET FIO=@FIO, login=@login, password=@password WHERE id=@id";
                    command.Parameters.AddWithValue("@FIO", admin.FIO);
                    command.Parameters.AddWithValue("@login", admin.Login);
                    command.Parameters.AddWithValue("@password", admin.Password);
                    command.Parameters.AddWithValue("@id", admin.ID);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void PageChangeAdmins_Loaded(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT id, FIO, login, password FROM admins";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string id = reader.GetInt16(0).ToString();
                            string fio = reader.GetString(1);
                            string login = reader.GetString(2);
                            string password = reader.GetString(3);

                            Admin newAdmin = new Admin()
                            {
                                ID = id,
                                FIO = fio,
                                Login = login,
                                Password = password
                            };

                            Admins.Add(newAdmin);
                        }
                    }
                }
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridAdmins.SelectedItem != null && Admins.Count > 1)
            {
                Admin selectedAdmin = (Admin)dataGridAdmins.SelectedItem;
                Admins.Remove(selectedAdmin);

                string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "DELETE FROM admins WHERE login=@login AND password=@password";
                        command.Parameters.AddWithValue("@login", selectedAdmin.Login);
                        command.Parameters.AddWithValue("@password", selectedAdmin.Password);
                        command.ExecuteNonQuery();
                    }
                }

                dataGridAdmins.Items.Refresh();
            }
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string insertQuery = "INSERT INTO admins (FIO, login, password) VALUES (@fio, @login, @password)";

                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@fio", textBoxFIO.Text);
                    command.Parameters.AddWithValue("@login", textBoxLogin.Text);
                    command.Parameters.AddWithValue("@password", textBoxPassword.Text);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Проверьте уникальность введённых данных. Убедитесь, что вы заполнили все поля и повторите попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                long lastInsertRowId = connection.LastInsertRowId;

                Admin newAdmin = new Admin
                {
                    ID = lastInsertRowId.ToString(),
                    FIO = textBoxFIO.Text,
                    Login = textBoxLogin.Text,
                    Password = textBoxPassword.Text
                };

                Admins.Add(newAdmin);

                textBoxFIO.Text = "";
                textBoxLogin.Text = "";
                textBoxPassword.Text = "";

                dataGridAdmins.Items.Refresh();
            }
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
        
    }

    public class Admin
    {
        public string ID { get; set; }
        public string FIO { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
