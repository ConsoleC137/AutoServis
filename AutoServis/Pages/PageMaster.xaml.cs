using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace AutoServis.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageMaster.xaml
    /// </summary>
    public partial class PageMaster : Page
    {
        private Frame Frame;
        public ObservableCollection<OrderForMaster> Orders { get; set; }

        public PageMaster(Frame frame)
        {
            InitializeComponent();
            Frame = frame;
            Loaded += PageMaster_Loaded;
            Orders = new ObservableCollection<OrderForMaster>();
            DataContext = this;
        }

        private void PageMaster_Loaded(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT orders.id, clients.FIO, clients.numberOfCar, clients.phone, orders.description, orders.price, orders.start_date, orders.end_time, orders.status FROM orders INNER JOIN clients ON orders.client_id = clients.id";
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
                            string decription = reader.GetString(4);
                            string price = reader.GetDouble(5).ToString();
                            string dateStart = reader.GetString(6);
                            string dateEnd = reader.GetString(7);
                            string status = reader.GetString(8);

                            OrderForMaster newOrder = new OrderForMaster()
                            {
                                ID = id,
                                FIO = fio,
                                Auto = auto,
                                Phone = phone,
                                Desription = decription,
                                Price = price,
                                DateStart = dateStart,
                                DateEnd = dateEnd,
                                Status = status
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

        private void buttonChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridOrders.SelectedItem != null)
            {
                OrderForMaster selectedOrder = (OrderForMaster)dataGridOrders.SelectedItem;

                if (selectedOrder.Status == "В работе")
                {
                    selectedOrder.Status = "Выполнено";
                    selectedOrder.DateEnd = DateTime.Now.ToString("dd.MM.yyyy");
                    string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        using (SQLiteCommand command = new SQLiteCommand(connection))
                        {
                            command.CommandText = "UPDATE orders SET status=@status, end_time=@end_time WHERE id=@id";
                            command.Parameters.AddWithValue("@status", "Выполнен");
                            command.Parameters.AddWithValue("@end_time", selectedOrder.DateEnd);
                            command.Parameters.AddWithValue("@id", selectedOrder.ID);
                            command.ExecuteNonQuery();
                        }
                    }
                }

                dataGridOrders.Items.Refresh();
            }
        }
    }

    public class OrderForMaster
    {
        public string ID { get; set; }
        public string FIO { get; set; }
        public string Auto { get; set; }
        public string Phone { get; set; }
        public string Desription { get; set; }
        public string Price { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public string Status { get; set; }
    }
}
