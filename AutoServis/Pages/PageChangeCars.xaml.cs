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
    /// Логика взаимодействия для PageChangeCars.xaml
    /// </summary>
    public partial class PageChangeCars : Page
    {
        private Frame Frame;
        public ObservableCollection<Car> Cars { get; set; }
        public PageChangeCars(Frame frame)
        {
            InitializeComponent();
            Frame = frame;
            Loaded += PageChangeCars_Loaded;
            DataContext = this;
            Cars = new ObservableCollection<Car>();
            dataGridCars.CellEditEnding += DataGridCars_CellEditEnding;
        }

        private void DataGridCars_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var textBox = e.EditingElement as TextBox;
            var newValue = textBox.Text;

            var car = e.Row.Item as Car;

            if (e.Column.Header.ToString() == "VIN номер")
                car.VIN = newValue;
            else if (e.Column.Header.ToString() == "Пробег")
                car.Mileage = newValue;
            else if (e.Column.Header.ToString() == "Марка")
                car.Mark = newValue;
            else if (e.Column.Header.ToString() == "Модель")
                car.Model = newValue;
            else if (e.Column.Header.ToString() == "Год выпуска")
                car.Year = newValue;

            string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE cars SET VIN=@VIN, mileage=@mileage, mark=@mark, model=@model, year=@year WHERE id=@id";
                    command.Parameters.AddWithValue("@VIN", car.VIN);
                    command.Parameters.AddWithValue("@mileage", car.Mileage);
                    command.Parameters.AddWithValue("@mark", car.Mark);
                    command.Parameters.AddWithValue("@model", car.Model);
                    command.Parameters.AddWithValue("@year", car.Year);
                    command.Parameters.AddWithValue("@id", car.ID);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void PageChangeCars_Loaded(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT id, VIN, mileage, brand, model, year FROM cars";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string id = reader.GetInt16(0).ToString();
                            string vin = reader.GetString(1);
                            string mileage = reader.GetInt16(2).ToString();
                            string mark = reader.GetString(3);
                            string model = reader.GetString(4);
                            string year = reader.GetInt16(5).ToString();

                            Car newCar = new Car()
                            {
                                ID = id,
                                VIN = vin,
                                Mileage = mileage,
                                Mark = mark,
                                Model = model,
                                Year = year
                            };

                            Cars.Add(newCar);
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

                string insertQuery = "INSERT INTO cars (VIN, mileage, brand, model, year) VALUES (@VIN, @mileage, @brand, @model, @year)";
                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@VIN", textBoxVIN.Text);
                    command.Parameters.AddWithValue("@mileage", textBoxMileage.Text);
                    command.Parameters.AddWithValue("@brand", textBoxMark.Text);
                    command.Parameters.AddWithValue("@model", textBoxModel.Text);
                    command.Parameters.AddWithValue("@year", textBoxYear.Text);
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

                Car newCar = new Car()
                {
                    ID = lastInsertRowId.ToString(),
                    VIN = textBoxVIN.Text,
                    Mileage = textBoxMileage.Text,
                    Mark = textBoxMark.Text,
                    Model = textBoxModel.Text,
                    Year = textBoxYear.Text,
                };

                Cars.Add(newCar);

                textBoxVIN.Text = "";
                textBoxMileage.Text = "";
                textBoxMark.Text = "";
                textBoxModel.Text = "";
                textBoxYear.Text = "";

                dataGridCars.Items.Refresh();
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridCars.SelectedItem != null)
            {
                Car selectedCar = (Car)dataGridCars.SelectedItem;
                Cars.Remove(selectedCar);

                string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "DELETE FROM cars WHERE id=@id";
                        command.Parameters.AddWithValue("@id", selectedCar.ID);
                        command.ExecuteNonQuery();
                    }
                }

                dataGridCars.Items.Refresh();
            }
        }
    }

    public class Car
    {
        public string ID { get; set; }
        public string VIN { get; set; }
        public string Mileage { get; set; }
        public string Mark { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
    }
}
