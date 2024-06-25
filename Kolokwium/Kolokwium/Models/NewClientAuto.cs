using System.ComponentModel.DataAnnotations;

namespace Kolokwium.Models;

public class NewClientAuto
{

    [MaxLength(50)]  
    public string FirstName { get; set; } = string.Empty;
    [MaxLength(100)]  

    public string LastName { get; set; } = string.Empty;
    [MaxLength(100)]
    public string Address { get; set; } = string.Empty;

    public List<CarToClient> CarToClients { get; set; } = new List<CarToClient>();
}

public class CarToClient
{
    public int Id { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    
    
    
}