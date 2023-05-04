using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace AutoServis.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageChangeMasters.xaml
    /// </summary>
    public partial class PageChangeMasters : Page
    {
        private Frame Frame;
        public ObservableCollection<Master> Masters { get; set; }
        public PageChangeMasters(Frame frame)
        {
            InitializeComponent();
            Frame = frame;
            Loaded += PageChangeMasters_Loaded;

            Masters = new ObservableCollection<Master>();

            DataContext = this;

            dataGridMasters.CellEditEnding += DataGridMasters_CellEditEnding;
        }

        private void DataGridMasters_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var textBox = e.EditingElement as TextBox;
            var newValue = textBox.Text;

            var master = e.Row.Item as Master;

            if (e.Column.Header.ToString() == "ФИО")
                master.FIO = newValue;
            else if (e.Column.Header.ToString() == "Логин")
                master.Login = newValue;
            else if (e.Column.Header.ToString() == "Пароль")
                master.Password = newValue;

            string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE masters SET FIO=@FIO, login=@login, password=@password WHERE id=@id";
                    command.Parameters.AddWithValue("@FIO", master.FIO);
                    command.Parameters.AddWithValue("@login", master.Login);
                    command.Parameters.AddWithValue("@password", master.Password);
                    command.Parameters.AddWithValue("@id", master.ID);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void PageChangeMasters_Loaded(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT id, FIO, login, password FROM masters";
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

                            Master newMaster = new Master()
                            {
                                ID = id,
                                FIO = fio,
                                Login = login,
                                Password = password
                            };

                            Masters.Add(newMaster);
                        }
                    }
                }
            }
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string insertQuery = "INSERT INTO masters (FIO, login, password) VALUES (@fio, @login, @password)";

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

                Master newMaster = new Master
                {
                    ID = lastInsertRowId.ToString(),
                    FIO = textBoxFIO.Text,
                    Login = textBoxLogin.Text,
                    Password = textBoxPassword.Text
                };

                Masters.Add(newMaster);

                textBoxFIO.Text = "";
                textBoxLogin.Text = "";
                textBoxPassword.Text = "";

                dataGridMasters.Items.Refresh();
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridMasters.SelectedItem != null && Masters.Count > 1)
            {
                Master selectedAdmin = (Master)dataGridMasters.SelectedItem;
                Masters.Remove(selectedAdmin);

                string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "DELETE FROM masters WHERE login=@login AND password=@password";
                        command.Parameters.AddWithValue("@login", selectedAdmin.Login);
                        command.Parameters.AddWithValue("@password", selectedAdmin.Password);
                        command.ExecuteNonQuery();
                    }
                }

                dataGridMasters.Items.Refresh();
            }
        }
    }

    public class Master
    {
        public string ID { get; set; }
        public string FIO { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
