using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
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
using System.Reflection;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;

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
            this.Frame = frame;
            Loaded += PageChangeAdmins_Loaded;

            // Инициализируем коллекцию администраторов
            Admins = new ObservableCollection<Admin>();

            // Привязываем коллекцию к контексту данных
            DataContext = this;

            dataGridAdmins.CellEditEnding += DataGridAdmins_CellEditEnding;
        }

        private void DataGridAdmins_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // получаем новое значение ячейки
            var textBox = e.EditingElement as TextBox;
            var newValue = textBox.Text;

            // получаем объект Admin из выбранной строки
            var admin = e.Row.Item as Admin;

            // обновляем свойство объекта Admin с учетом нового значения ячейки
            if (e.Column.Header.ToString() == "ФИО")
            {
                admin.FIO = newValue;
            }
            else if (e.Column.Header.ToString() == "Логин")
            {
                admin.Login = newValue;
            }
            else if (e.Column.Header.ToString() == "Пароль")
            {
                admin.Password = newValue;
            }

            // обновляем запись в базе данных
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


                            // Создаем нового администратора
                            Admin newAdmin = new Admin()
                            {
                                ID = id,
                                FIO = fio,
                                Login = login,
                                Password = password
                            };

                            // Добавляем его в коллекцию администраторов
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

                // Обновление представления DataGrid
                dataGridAdmins.ItemsSource = null;
                dataGridAdmins.ItemsSource = Admins;
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
                string insertQuery = "INSERT INTO admins (FIO, login, password) VALUES (@fio, @login, @password)";

                // 3. Выполняем SQL-запрос и получаем ID последней вставленной записи
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
                        MessageBox.Show("Такая строка уже существует. Пожалуйста, убедитесь в уникальности новой строки и повторите попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                long lastInsertRowId = connection.LastInsertRowId;

                // 4. Создаем новый объект Admin и заполняем его свойства значениями из текстовых полей
                Admin newAdmin = new Admin
                {
                    ID = lastInsertRowId.ToString(),
                    FIO = textBoxFIO.Text,
                    Login = textBoxLogin.Text,
                    Password = textBoxPassword.Text
                };

                // 5. Добавляем новый объект Admin в ObservableCollection
                Admins.Add(newAdmin);

                // 6. Очищаем текстовые поля
                textBoxFIO.Text = "";
                textBoxLogin.Text = "";
                textBoxPassword.Text = "";

                // 7. Обновляем DataGrid
                dataGridAdmins.Items.Refresh();
            }
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
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
