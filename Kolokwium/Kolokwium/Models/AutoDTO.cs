namespace Kolokwium.Models;

public class AutoDTO
{
    
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    
    public List<CarDTO> Cars { get; set; } = null!;

    
}

public class CarDTO
{
    public string Vin { get; set; } = string.Empty;
    
    public string Color { get; set; } = string.Empty;
    
    public string Model { get; set; } = string.Empty;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int TotalPrice { get; set; }
    
    

}