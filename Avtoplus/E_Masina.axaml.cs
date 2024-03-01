using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;

namespace Avtoplus;

public partial class E_Masina : Window
{
    private Masina _masinaForm;
    private Vehicles _selectedVehicle;
    private string _connString = "server=localhost;database=PP;port=3306;User Id=sammy;password=Andr%123";

    public E_Masina(Masina masinaForm, Vehicles selectedVehicle)
    {
        _masinaForm = masinaForm;
        _selectedVehicle = selectedVehicle;
        InitializeComponent();

        // Заполняем поля формы данными выбранного транспортного средства
        MakeTextBox.Text = _selectedVehicle.Make;
        ModelTextBox.Text = _selectedVehicle.Model;
        YearTextBox.Text = _selectedVehicle.Year.ToString();
        VINTextBox.Text = _selectedVehicle.VIN;
        LicensePlateTextBox.Text = _selectedVehicle.LicensePlate;

        // Заполняем ComboBox данными из таблицы Customers
        CustomerComboBox.ItemsSource = GetCustomers();
        CustomerComboBox.SelectedItem = _selectedVehicle.Name;
    }

    private List<string> GetCustomers()
    {
        List<string> customers = new List<string>();

        using (MySqlConnection connection = new MySqlConnection(_connString))
        {
            connection.Open();

            string sql = "SELECT Name FROM Customers";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    customers.Add(reader.GetString("Name"));
                }
            }
        }

        return customers;
    }

    private void UpdateVehicle(int vehicleId, string make, string model, int year, string vin, string licensePlate, string customerName)
    {
        int customerId = GetCustomerId(customerName);

        if (customerId != -1)
        {
            using (MySqlConnection connection = new MySqlConnection(_connString))
            using (MySqlCommand command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText = "UPDATE Vehicles " +
                                      "SET Make = @Make, " +
                                      "Model = @Model, " +
                                      "Year = @Year, " +
                                      "VIN = @VIN, " +
                                      "LicensePlate = @LicensePlate, " +
                                      "Customer_ID = @CustomerID " +
                                      "WHERE VehicleID = @VehicleID";

                command.Parameters.AddWithValue("@VehicleID", vehicleId);
                command.Parameters.AddWithValue("@Make", make);
                command.Parameters.AddWithValue("@Model", model);
                command.Parameters.AddWithValue("@Year", year);
                command.Parameters.AddWithValue("@VIN", vin);
                command.Parameters.AddWithValue("@LicensePlate", licensePlate);
                command.Parameters.AddWithValue("@CustomerID", customerId);

                command.ExecuteNonQuery();
            }

            _masinaForm.ShowTable();
            Close();
        }
        else
        {
            Console.WriteLine("Не удалось получить CustomerID");
        }
    }

    private int GetCustomerId(string customerName)
    {
        using (MySqlConnection connection = new MySqlConnection(_connString))
        {
            connection.Open();

            string sqlSelect = "SELECT CustomerID FROM Customers WHERE Name = @CustomerName";

            using (MySqlCommand command = new MySqlCommand(sqlSelect, connection))
            {
                command.Parameters.AddWithValue("@CustomerName", customerName);

                object result = command.ExecuteScalar();

                return (result != null) ? Convert.ToInt32(result) : -1;
            }
        }
    }

    private void SaveChanges_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            int vehicleId = _selectedVehicle.VehicleID;
            string make = MakeTextBox.Text;
            string model = ModelTextBox.Text;
            int year = int.Parse(YearTextBox.Text);
            string vin = VINTextBox.Text;
            string licensePlate = LicensePlateTextBox.Text;
            string customerName = CustomerComboBox.SelectedItem?.ToString();

            UpdateVehicle(vehicleId, make, model, year, vin, licensePlate, customerName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении изменений: {ex.Message}");
        }
    }
}

