namespace Avtoplus;

public class Vehicles
{
    // Свойство для хранения уникального идентификатора транспортного средства
    public int VehicleID { get; set; }
    
    // Свойство для хранения марки транспортного средства
    public string Make { get; set; }

    // Свойство для хранения модели транспортного средства
    public string Model { get; set; }

    // Свойство для хранения года выпуска транспортного средства
    public int Year { get; set; }

    // Свойство для хранения идентификационного номера транспортного средства
    public string VIN { get; set; }

    // Свойство для хранения государственного регистрационного знака транспортного средства
    public string LicensePlate { get; set; }

    public string Name { get; set; }
}