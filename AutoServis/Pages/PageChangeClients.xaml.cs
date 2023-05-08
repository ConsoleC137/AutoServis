using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace AutoServis.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageChangeClients.xaml
    /// </summary>
    public partial class PageChangeClients : Page
    {
        private Frame Frame;
        public ObservableCollection<Client> Clients { get; set; }
        public PageChangeClients(Frame frame)
        {
            InitializeComponent();
            Frame = frame;
            Loaded += PageChangeClients_Loaded;

            Clients = new ObservableCollection<Client>();
            DataContext = this;

            dataGridClients.CellEditEnding += DataGridClients_CellEditEnding;
        }

        private void DataGridClients_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var textBox = e.EditingElement as TextBox;
            var newValue = textBox.Text;

            var client = e.Row.Item as Client;

            if (e.Column.Header.ToString() == "ФИО")
                client.FIO = newValue;
            else if (e.Column.Header.ToString() == "ID автомобиля")
                client.Auto = newValue;
            else if (e.Column.Header.ToString() == "Номер телефона")
                client.Phone = newValue;

            string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE clients SET FIO=@FIO, car_id=@car_id, phone=@phone WHERE id=@id";
                    command.Parameters.AddWithValue("@FIO", client.FIO);
                    command.Parameters.AddWithValue("@car_id", client.Auto);
                    command.Parameters.AddWithValue("@phone", client.Phone);
                    command.Parameters.AddWithValue("@id", client.ID);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void PageChangeClients_Loaded(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT id, FIO, car_id, phone FROM clients";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string id = reader.GetInt16(0).ToString();
                            string fio = reader.GetString(1);
                            string auto = reader.GetString(2);
                            string phone = reader.GetString(3);

                            Client newClient = new Client()
                            {
                                ID = id,
                                FIO = fio,
                                Auto = auto,
                                Phone = phone
                            };

                            Clients.Add(newClient);
                        }
                    }
                }

                using (var command = new SQLiteCommand("SELECT id FROM cars", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        comboBoxAuto.Items.Add(reader["id"]);
                    }
                }
            }
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridClients.SelectedItem != null)
            {
                Client selectedClient = (Client)dataGridClients.SelectedItem;
                Clients.Remove(selectedClient);

                string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "DELETE FROM clients WHERE car_id=@car_id AND phone=@phone";
                        command.Parameters.AddWithValue("@car_id", selectedClient.Auto);
                        command.Parameters.AddWithValue("@phone", selectedClient.Phone);
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
                }

                dataGridClients.Items.Refresh();
            }
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string insertQuery = "INSERT INTO clients (FIO, car_id, phone) VALUES (@fio, @car_id, @phone)";

                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@fio", textBoxFIO.Text);
                    command.Parameters.AddWithValue("@car_id", comboBoxAuto.Text);
                    command.Parameters.AddWithValue("@phone", textBoxPhone.Text);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Такая строка уже существует. Пожалуйста, убедитесь в уникальности новой строки и повторите попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                long lastInsertRowId = connection.LastInsertRowId;

                Client newClient = new Client
                {
                    ID = lastInsertRowId.ToString(),
                    FIO = textBoxFIO.Text,
                    Auto = comboBoxAuto.Text,
                    Phone = textBoxPhone.Text
                };

                Clients.Add(newClient);

                textBoxFIO.Text = "";
                textBoxPhone.Text = "";

                dataGridClients.Items.Refresh();
            }
        }
    }

    public class Client
    {
        public string ID { get; set; }
        public string FIO { get; set; }
        public string Auto { get; set; }
        public string Phone { get; set; }
    }
}
