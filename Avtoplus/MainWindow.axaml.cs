using Avalonia.Controls;
using Avalonia.Interactivity;
using MySql.Data.MySqlClient;

namespace Avtoplus;

public partial class MainWindow : Window
{
    private MySqlConnection _sqlConnection;
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        string connectionString = "server=localhost;database=PP;port=3306;User Id=sammy;password=Andr%123";

        using (_sqlConnection = new MySqlConnection(connectionString))
        {
            _sqlConnection.Open();

            string selectQuery = "SELECT Login FROM Users WHERE Login = @Login AND Password = @Password";
            using (MySqlCommand sqlCommand = new MySqlCommand(selectQuery, _sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@Login", Login.Text);
                sqlCommand.Parameters.AddWithValue("@Password", Password.Text);

                if (sqlCommand.ExecuteScalar() != null)
                {
                    Main window = new Main();
                    Hide();
                    window.Show();
                }
            }
        }
    }
}