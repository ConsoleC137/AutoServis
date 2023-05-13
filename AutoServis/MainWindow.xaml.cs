using System.Data.SQLite;
using System.Windows;


namespace AutoServis
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Pages.PageAuth pageAuth = new Pages.PageAuth(mainFrame)
            {
                Width = Width,
                Height = Height,
                Title = "Авторизация"
            };
            mainFrame.Navigate(pageAuth);

            //string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";

            //using (var connection = new SQLiteConnection(connectionString))
            //{
            //    connection.Open();

            //    using (var command = new SQLiteCommand("CREATE TABLE admins (id INTEGER PRIMARY KEY AUTOINCREMENT, FIO TEXT NOT NULL, login TEXT UNIQUE NOT NULL, password TEXT UNIQUE NOT NULL)", connection))
            //        command.ExecuteNonQuery();

            //    string query = "INSERT INTO admins (FIO, login, password) VALUES ('Администратор по умолчанию', 'admin', 'admin')";
            //    SQLiteCommand c = new SQLiteCommand(query, connection);
            //    c.ExecuteNonQuery();

            //    using (var command = new SQLiteCommand("CREATE TABLE masters (id INTEGER PRIMARY KEY AUTOINCREMENT, FIO TEXT NOT NULL, login TEXT UNIQUE NOT NULL, password TEXT UNIQUE NOT NULL)", connection))
            //        command.ExecuteNonQuery();

            //    using (var command = new SQLiteCommand("CREATE TABLE clients (id INTEGER PRIMARY KEY AUTOINCREMENT, FIO TEXT NOT NULL, car_id TEXT UNIQUE NOT NULL, phone TEXT UNIQUE NOT NULL)", connection))
            //        command.ExecuteNonQuery();

            //    using (var command = new SQLiteCommand("CREATE TABLE orders (id INTEGER PRIMARY KEY AUTOINCREMENT, client_id INTEGER NOT NULL, master_id INTEGER NOT NULL, start_date DATE NOT NULL, end_time DATE, price REAL NOT NULL, description TEXT NOT NULL, status TEXT NOT NULL)", connection))
            //        command.ExecuteNonQuery();

            //    using (var command = new SQLiteCommand("CREATE TABLE cars (id INTEGER PRIMARY KEY AUTOINCREMENT, VIN TEXT NOT NULL UNIQUE , mileage INTEGER NOT NULL, brand TEXT NOT NULL, model TEXT NOT NULL, year INTEGER NOT NULL)", connection))
            //        command.ExecuteNonQuery();
            //}
        }
    }
}
