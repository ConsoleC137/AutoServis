using System.Data.SQLite;
namespace TestAutoServis
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CheckSelectForAdmins()
        {
            var connectionString = "Data Source=MyDatabase.sqlite;Version=3;";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "SELECT id FROM admins WHERE login = @login AND password = @password";
                    command.Parameters.AddWithValue("@login", "admin");
                    command.Parameters.AddWithValue("@password", "admin");

                    var actualAdminId = command.ExecuteScalar();

                    Assert.AreEqual(true, actualAdminId != null);
                }
            }
        }



        [TestMethod]
        public void UpdateOrderTest()
        {
            // Arrange
            var connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
            var orderIdToUpdate = 2;
            var newClientId = 2;
            var newMasterId = 1;
            var newStartDate = DateTime.Now;
            var newEndDate = DateTime.Now.AddDays(1);
            var newPrice = 100.0;
            var newDescription = "Updated order";
            var newStatus = "completed";

            // Act
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE Orders SET client_id = @client_id, master_id = @master_id, start_date = @start_date, end_time = @end_date, price = @price, description = @description, status = @status WHERE id = @id";
                    command.Parameters.AddWithValue("@client_id", newClientId);
                    command.Parameters.AddWithValue("@master_id", newMasterId);
                    command.Parameters.AddWithValue("@start_date", newStartDate);
                    command.Parameters.AddWithValue("@end_date", newEndDate);
                    command.Parameters.AddWithValue("@price", newPrice);
                    command.Parameters.AddWithValue("@description", newDescription);
                    command.Parameters.AddWithValue("@status", newStatus);
                    command.Parameters.AddWithValue("@id", orderIdToUpdate);

                    var rowsAffected = command.ExecuteNonQuery();

                    // Assert
                    Assert.AreEqual(1, rowsAffected);
                }
            }
        }

    }
}