using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
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
            this.Frame = frame;
            Loaded += PageChangeClients_Loaded;

            Clients = new ObservableCollection<Client>();
            DataContext = this;

            dataGridClients.CellEditEnding += DataGridClients_CellEditEnding;
        }

        private void DataGridClients_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // получаем новое значение ячейки
            var textBox = e.EditingElement as TextBox;
            var newValue = textBox.Text;

            // получаем объект Admin из выбранной строки
            var admin = e.Row.Item as Client;

            // обновляем свойство объекта Admin с учетом нового значения ячейки
            if (e.Column.Header.ToString() == "ФИО")
            {
                admin.FIO = newValue;
            }
            else if (e.Column.Header.ToString() == "Номер автомобиля")
            {
                admin.Auto = newValue;
            }
            else if (e.Column.Header.ToString() == "Номер телефона")
            {
                admin.Phone = newValue;
            }

            // обновляем запись в базе данных
            string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE clients SET FIO=@FIO, numberOfCar=@numberOfCar, phone=@phone WHERE id=@id";
                    command.Parameters.AddWithValue("@FIO", admin.FIO);
                    command.Parameters.AddWithValue("@numberOfCar", admin.Auto);
                    command.Parameters.AddWithValue("@phone", admin.Phone);
                    command.Parameters.AddWithValue("@id", admin.ID);
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
                string query = "SELECT id, FIO, numberOfCar, phone FROM clients";
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


                            // Создаем нового клиента
                            Client newClient = new Client()
                            {
                                ID = id,
                                FIO = fio,
                                Auto = auto,
                                Phone = phone
                            };

                            // Добавляем его в коллекцию клиента
                            Clients.Add(newClient);
                        }
                    }
                }
            }
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridClients.SelectedItem != null && Clients.Count > 1)
            {
                Client selectedClient = (Client)dataGridClients.SelectedItem;
                Clients.Remove(selectedClient);

                string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "DELETE FROM clients WHERE numberOfCar=@numberOfCar AND phone=@phone";
                        command.Parameters.AddWithValue("@numberOfCar", selectedClient.Auto);
                        command.Parameters.AddWithValue("@phone", selectedClient.Phone);
                        command.ExecuteNonQuery();
                    }
                }

                // Обновление представления DataGrid
                dataGridClients.ItemsSource = null;
                dataGridClients.ItemsSource = Clients;
            }
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            // 1. Создаем подключение к базе данных SQLite
            string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // 2. Формируем SQL-запрос для вставки новых данных в таблицу
                string insertQuery = "INSERT INTO clients (FIO, numberOfCar, phone) VALUES (@fio, @numberOfCar, @phone)";

                // 3. Выполняем SQL-запрос и получаем ID последней вставленной записи
                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@fio", textBoxFIO.Text);
                    command.Parameters.AddWithValue("@numberOfCar", textBoxAuto.Text);
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

                // 4. Создаем новый объект Client и заполняем его свойства значениями из текстовых полей
                Client newClient = new Client
                {
                    ID = lastInsertRowId.ToString(),
                    FIO = textBoxFIO.Text,
                    Auto = textBoxAuto.Text,
                    Phone = textBoxPhone.Text
                };

                // 5. Добавляем новый объект Client в ObservableCollection
                Clients.Add(newClient);

                // 6. Очищаем текстовые поля
                textBoxFIO.Text = "";
                textBoxAuto.Text = "";
                textBoxPhone.Text = "";

                // 7. Обновляем DataGrid
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
