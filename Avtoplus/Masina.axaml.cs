using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;

namespace Avtoplus;

public partial class Masina : Window
{
    private Vehicles _selectedVehicles;
    private MySqlConnection _connection;
    private List<Vehicles> _vehicles;
    private string _connString = "server=localhost;database=PP;port=3306;User Id=sammy;password=Andr%123";
    public Masina()
    {
        InitializeComponent();
        
        ShowTable();
        _connection = new MySqlConnection(_connString);
        VehiclesGrid.SelectionChanged += VehiclesGrid_SelectionChanged;
        SearchTextBox.TextChanged += SearchTextBox_TextChanged;

        MakeComboBox.SelectionChanged += MakeComboBox_SelectionChanged;
        MakeComboBox.ItemsSource = GetMakes(); // Заполняем ComboBox с помощью метода GetMakes()
    }
    
    private void VehiclesGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (VehiclesGrid.SelectedItem is Vehicles selectedVehicles)
        {
            _selectedVehicles = selectedVehicles;
        }
    }

    public void ShowTable()
    {
        string sql = "SELECT Vehicles.VehicleID, Customers.Name, Vehicles.Make, Vehicles.Model, Vehicles.Year, Vehicles.VIN, Vehicles.LicensePlate " +
                     "FROM Vehicles " +
                     "JOIN Customers ON Vehicles.Customer_ID = Customers.customerID";

        _vehicles = new List<Vehicles>(); // Инициализируем коллекцию _vehicles здесь

        using (MySqlConnection connection = new MySqlConnection(_connString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var currentVehicle = new Vehicles
                    {
                        VehicleID = reader.GetInt32("VehicleID"),
                        Name = reader.GetString("Name"),
                        Make = reader.GetString("Make"),
                        Model = reader.GetString("Model"),
                        Year = reader.GetInt32("Year"),
                        VIN = reader.GetString("VIN"),
                        LicensePlate = reader.GetString("LicensePlate")
                    };

                    _vehicles.Add(currentVehicle); // Добавляем транспортное средство в коллекцию _vehicles
                }
            }
        }

        VehiclesGrid.ItemsSource = _vehicles; // Устанавливаем источник данных для DataGrid
    }

    
    
    
    private void OpenNextForm_Click(object? sender, RoutedEventArgs e)
    {
        var masina_DForm = new D_Masina(this);
        masina_DForm.Show();
    }

    private void DeleteVehiclesGrid_Click(object? sender, RoutedEventArgs e)
    {
        
        if (_selectedVehicles != null)
        {
            DeleteVehicles(_selectedVehicles.VehicleID);
        }
    }
    
    private void DeleteVehicles(int vehicleID)
    {
        using (_connection = new MySqlConnection(_connString))
        {
            _connection.Open();
            string queryString = $"DELETE FROM Vehicles WHERE VehicleID = {vehicleID}";
            MySqlCommand command = new MySqlCommand(queryString, _connection);
            command.ExecuteNonQuery();
        }

        ShowTable();
    }
    

    private void EditVehiclesGrid_Click(object? sender, RoutedEventArgs e)
    {
        if (_selectedVehicles != null)
        {
            // Открываем окно редактирования с выбранными данными
            var editForm = new E_Masina(this, _selectedVehicles);
            editForm.Show();
        }
        else
        {
            Console.WriteLine("Выберите продукт для редактирования");
        }
    }

    
    private List<string> GetMakes()
    {
        List<string> makes = new List<string>();

        using (MySqlConnection connection = new MySqlConnection(_connString))
        {
            connection.Open();

            string sql = "SELECT DISTINCT Make FROM Vehicles";
            MySqlCommand command = new MySqlCommand(sql, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                makes.Add(reader.GetString("Make"));
            }

            reader.Close();
        }

        return makes;
    }
    
    private void FilterByMake(string make)
    {
        string sql = "SELECT Vehicles.VehicleID, Customers.Name, Vehicles.Make, Vehicles.Model, Vehicles.Year, Vehicles.VIN, Vehicles.LicensePlate " +
                     "FROM Vehicles " +
                     "JOIN Customers ON Vehicles.Customer_ID = Customers.customerID " +
                     "WHERE Vehicles.Make = @Make";

        List<Vehicles> filteredVehicles = new List<Vehicles>();

        using (MySqlConnection connection = new MySqlConnection(_connString))
        {
            connection.Open();

            MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Make", make);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var currentVehicle = new Vehicles
                    {
                        VehicleID = reader.GetInt32("VehicleID"),
                        Name = reader.GetString("Name"), // Используем столбец Name из таблицы Customers
                        Make = reader.GetString("Make"),
                        Model = reader.GetString("Model"),
                        Year = reader.GetInt32("Year"),
                        VIN = reader.GetString("VIN"),
                        LicensePlate = reader.GetString("LicensePlate")
                    };

                    filteredVehicles.Add(currentVehicle);
                }
            }
        }

        VehiclesGrid.ItemsSource = filteredVehicles;
    }

    
    
    
    private void MakeComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (MakeComboBox.SelectedItem is string selectedMake)
        {
            FilterByMake(selectedMake);
        }
    }

    private void ResetFilterButton_Click(object? sender, RoutedEventArgs e)
    {
        // Сбросить фильтр и отобразить полную таблицу
        ShowTable();

        // Очистить выбор в ComboBox
        MakeComboBox.SelectedItem = null;
    }
    
    
    private void SearchTextBox_TextChanged(object sender, RoutedEventArgs e)
    {
        string searchQuery = SearchTextBox.Text.Trim();
        if (!string.IsNullOrEmpty(searchQuery))
        {
            // Выполните поиск по имени клиента и году выпуска и обновите данные в DataGrid
            SearchAndRefreshTable(searchQuery);
        }
        else
        {
            // Если поле поиска пусто, отобразите все данные
            ShowTable();
        }
    }

    private void SearchAndRefreshTable(string searchQuery)
    {
        if (_vehicles == null || _vehicles.Count == 0)
        {
            return;
        }

        searchQuery = searchQuery.ToLower();

        // Фильтруем коллекцию _vehicles по критериям поиска
        var filteredVehicles = _vehicles
            .Where(v =>
                v.Name.ToLower().Contains(searchQuery) ||
                v.Year.ToString().Contains(searchQuery))
            .ToList();

        // Обновляем отображаемые данные в DataGrid
        VehiclesGrid.ItemsSource = filteredVehicles;
    }


    private void SortByMakeAndModel_Click(object? sender, RoutedEventArgs e)
    {
        // Сортировка списка транспортных средств по общему количеству символов в полях Make и Model
        _vehicles.Sort((x, y) => (x.Make.Length + x.Model.Length).CompareTo(y.Make.Length + y.Model.Length));

        // Обновление отображаемых данных в DataGrid
        VehiclesGrid.ItemsSource = null;
        VehiclesGrid.ItemsSource = _vehicles;
    }
}