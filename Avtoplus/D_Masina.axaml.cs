using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;

namespace Avtoplus;

public partial class D_Masina : Window
{
    private Masina _masinaForm;
    private string _connString = "server=localhost;database=PP;port=3306;User Id=sammy;password=Andr%123";
    
    public D_Masina(Masina masinaForm)
    {
        InitializeComponent();
        
        _masinaForm = masinaForm;

        // Заполните ComboBox данными
        CustomerComboBox.ItemsSource = GetCustomers();
        CustomerComboBox.SelectedIndex = 0;
    }

    private List<string> GetCustomers()
    {
        List<string> customers = new List<string>();

        using (MySqlConnection connection = new MySqlConnection(_connString))
        {
            connection.Open();

            string sql = "SELECT name FROM Customers";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    customers.Add(reader.GetString("name"));
                }
            }
        }

        return customers;
    }

    private void AddVehicle_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(MakeTextBox.Text) ||
                CustomerComboBox.SelectedItem == null)
            {
                Console.WriteLine("Please fill in all fields.");
                return;
            }

            int customerId = GetCustomerId(CustomerComboBox.SelectedItem?.ToString());

            if (customerId != -1)
            {
                InsertVehicle(MakeTextBox.Text, ModelTextBox.Text, int.Parse(YearTextBox.Text), VINTextBox.Text, LicensePlateTextBox.Text, customerId);
                _masinaForm.ShowTable();
                Close();
            }
            else
            {
                Console.WriteLine("Failed to get CustomerID");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding vehicle: {ex.Message}");
        }
    }

    private int GetCustomerId(string customerName)
    {
        using (MySqlConnection connection = new MySqlConnection(_connString))
        {
            connection.Open();

            string sql = "SELECT customerID FROM Customers WHERE name = @Name";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Name", customerName);

                object result = command.ExecuteScalar();

                return (result != null) ? Convert.ToInt32(result) : -1;
            }
        }
    }

    private void InsertVehicle(string make, string model, int year, string vin, string licensePlate, int customerId)
    {
        string sql = "INSERT INTO Vehicles (Make, Model, Year, VIN, LicensePlate, customer_ID) " +
                     "VALUES (@Make, @Model, @Year, @VIN, @LicensePlate, @customer_ID)";

        using (MySqlConnection connection = new MySqlConnection(_connString))
        using (MySqlCommand command = new MySqlCommand(sql, connection))
        {
            connection.Open();

            command.Parameters.AddWithValue("@Make", make);
            command.Parameters.AddWithValue("@Model", model);
            command.Parameters.AddWithValue("@Year", year);
            command.Parameters.AddWithValue("@VIN", vin);
            command.Parameters.AddWithValue("@LicensePlate", licensePlate);
            command.Parameters.AddWithValue("@customer_ID", customerId);

            command.ExecuteNonQuery();
        }
    }
}
