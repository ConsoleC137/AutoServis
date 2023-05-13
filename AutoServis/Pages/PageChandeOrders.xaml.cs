using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace AutoServis.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageChandeOrders.xaml
    /// </summary>
    public partial class PageChandeOrders : Page
    {
        private Frame Frame;
        public ObservableCollection<Order> Orders { get; set; }
        public PageChandeOrders(Frame frame)
        {
            InitializeComponent();
            Frame = frame;
            Loaded += PageChandeOrders_Loaded;
            Orders = new ObservableCollection<Order>();
            DataContext = this;
            dataGridOrders.CellEditEnding += DataGridOrders_CellEditEnding;
        }

        private void DataGridOrders_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var textBox = e.EditingElement as TextBox;
            var newValue = textBox.Text;

            var order = e.Row.Item as Order;

            if (e.Column.Header.ToString() == "ID клиента")
                order.IDClient = newValue;
            else if (e.Column.Header.ToString() == "ID мастера")
                order.IDMaster = newValue;
            else if (e.Column.Header.ToString() == "Описание заказа")
                order.Description = newValue;
            else if (e.Column.Header.ToString() == "Стоимость")
                order.Price = newValue;
            else if (e.Column.Header.ToString() == "Дата начала заказа")
                order.DateStart = newValue;
            else if (e.Column.Header.ToString() == "Дата окончания заказа")
                order.DateEnd = newValue;
            else if (e.Column.Header.ToString() == "Статус")
                order.Status = newValue;

            string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE orders SET client_id=@client_id, master_id=@master_id, start_date=@start_date, end_time=@end_time, price=@price, description=@description, status=@status WHERE id=@id";
                    command.Parameters.AddWithValue("@client_id", order.IDClient);
                    command.Parameters.AddWithValue("@master_id", order.IDMaster);
                    command.Parameters.AddWithValue("@start_date", order.DateStart);
                    command.Parameters.AddWithValue("@end_time", order.DateEnd);
                    command.Parameters.AddWithValue("@price", order.Price);
                    command.Parameters.AddWithValue("@description", order.Description);
                    command.Parameters.AddWithValue("@status", order.Status);
                    command.Parameters.AddWithValue("@id", order.ID);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void PageChandeOrders_Loaded(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT id, client_id, master_id, start_date, end_time, price, description, status FROM orders";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        Orders.Clear();
                        while (reader.Read())
                        {
                            string id = reader.GetInt16(0).ToString();
                            string client = reader.GetInt16(1).ToString();
                            string master = reader.GetInt16(2).ToString();
                            string dateStart = reader.GetString(3);
                            string dateEnd = reader.GetString(4);
                            string price = reader.GetDouble(5).ToString();
                            string decription = reader.GetString(6);
                            string status = reader.GetString(7);

                            Order newOrder = new Order()
                            {
                                ID = id,
                                IDClient = client,
                                IDMaster = master,
                                Description = decription,
                                Price = price,
                                DateStart = dateStart,
                                DateEnd = dateEnd,
                                Status = status,
                            };

                            Orders.Add(newOrder);
                        }
                    }
                }
                using (var command = new SQLiteCommand("SELECT id FROM clients", connection))
                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                        if (!comboBoxClient.Items.Contains(reader["id"]))
                            comboBoxClient.Items.Add(reader["id"]);

                using (var command = new SQLiteCommand("SELECT id FROM masters", connection))
                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                        if (!comboBoxMaster.Items.Contains(reader["id"]))
                            comboBoxMaster.Items.Add(reader["id"]);
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

                string insertQuery = "INSERT INTO orders (client_id, master_id, start_date, end_time, price, description, status) VALUES (@client_id, @master_id, @start_date, @end_time, @price, @description, @status)";
                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@client_id", comboBoxClient.Text); 
                    command.Parameters.AddWithValue("@master_id", comboBoxMaster.Text);
                    command.Parameters.AddWithValue("@start_date", DatePickerStart.Text);
                    command.Parameters.AddWithValue("@end_time", DatePickerEnd.Text);
                    command.Parameters.AddWithValue("@price", textBoxPrice.Text);
                    command.Parameters.AddWithValue("@description", textBoxDescription.Text);
                    command.Parameters.AddWithValue("@status", comboBoxStatus.Text);
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

                Order newOrder = new Order
                {
                    ID = lastInsertRowId.ToString(),
                    IDClient = comboBoxClient.Text,
                    IDMaster = comboBoxMaster.Text,
                    Description = textBoxDescription.Text,
                    Price = textBoxPrice.Text,
                    DateStart = DatePickerStart.Text,
                    DateEnd = DatePickerEnd.Text,
                    Status = comboBoxStatus.Text,
                };

                Orders.Add(newOrder);

                textBoxDescription.Text = "";
                textBoxPrice.Text = "";
                DatePickerStart.Text = "";
                DatePickerEnd.Text = "";

                dataGridOrders.Items.Refresh();
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridOrders.SelectedItem != null)
            {
                Order selectedOrder = (Order)dataGridOrders.SelectedItem;
                Orders.Remove(selectedOrder);

                string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "DELETE FROM orders WHERE id=@id";
                        command.Parameters.AddWithValue("@id", selectedOrder.ID);
                        command.ExecuteNonQuery();
                    }
                }

                dataGridOrders.Items.Refresh();
            }
        }

        private void buttonChageAdmins_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new Pages.PageChangeAdmins(Frame));
        }

        private void buttonChageMasters_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new Pages.PageChangeMasters(Frame));
        }

        private void buttonChageClients_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new Pages.PageChangeClients(Frame));
        }

        private void buttonChageCars_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new Pages.PageChangeCars(Frame));
        }
    }

    public class Order
    {
        public string ID { get; set; }
        public string IDClient { get; set; }
        public string IDMaster { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public string Status { get; set; }
    }
}
