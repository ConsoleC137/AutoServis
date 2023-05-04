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
                                Desription = decription,
                                Price = price,
                                DateStart = dateStart,
                                DateEnd = dateEnd,
                                Status = status,
                            };

                            Orders.Add(newOrder);
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

                string insertQuery = "INSERT INTO orders (client_id, master_id, start_date, end_time, price, description, status) VALUES (@client_id, @master_id, @start_date, @end_time, @price, @description, @status)";
                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@client_id", textBoxClient.Text); 
                    command.Parameters.AddWithValue("@master_id", textBoxMaster.Text);
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
                    IDClient = textBoxClient.Text,
                    IDMaster = textBoxMaster.Text,
                    Desription = textBoxDescription.Text,
                    Price = textBoxPrice.Text,
                    DateStart = DatePickerStart.Text,
                    DateEnd = DatePickerEnd.Text,
                    Status = comboBoxStatus.Text,
                };

                Orders.Add(newOrder);

                textBoxClient.Text = "";
                textBoxClient.Text = "";
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
    }

    public class Order
    {
        public string ID { get; set; }
        public string IDClient { get; set; }
        public string IDMaster { get; set; }
        public string Desription { get; set; }
        public string Price { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public string Status { get; set; }
    }
}
