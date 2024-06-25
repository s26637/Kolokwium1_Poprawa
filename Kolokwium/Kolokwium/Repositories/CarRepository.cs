using Kolokwium.Models;
using Microsoft.Data.SqlClient;

namespace Kolokwium.Repositories;


public class CarRepository : ICarRepository
{
    private readonly IConfiguration _configuration;
    public CarRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> DoesClientExist(int id)
    {
        var query = "SELECT 1 FROM Clients WHERE ID = @ID";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);
        
        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }
    public async Task<bool> DoesCarExist(int id)
    {
	    var query = "SELECT 1 FROM Cars WHERE ID = @ID";
        
	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand();

	    command.Connection = connection;
	    command.CommandText = query;
	    command.Parameters.AddWithValue("@ID", id);
        
	    await connection.OpenAsync();

	    var res = await command.ExecuteScalarAsync();

	    return res is not null;
    }
    
    
    public async Task<AutoDTO> GetCar(int id)
    {
        var query = @"SELECT 
							Clients.ID AS ClientId,
							Clients.FirstName AS FirstName,
							Clients.LastName as LastName,
							Clients.Address as Address,
							Car_rentals.DateFrom as DateFrom,
							Car_rentals.DateTo as DateTo,
                            Car_rentals.TotalPrice as TotalP,
                            Cars.Vin as Vin,
                            Colors.Name as ColorName,
                            Models.Name as ModelName
						FROM Clients
						JOIN [Car_rentals] ON [Car_rentals].ClientID = Clients.ID
						JOIN Cars ON Cars.ID = [Car_rentals].ClientID
						JOIN Models ON Models.ID = Cars.ID
						JOIN Colors ON Colors.ID = Cars.ID
		            	WHERE Clients.ID = @ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();

        var clientId = reader.GetOrdinal("ClientId");
        var firstName = reader.GetOrdinal("FirstName");
        var lastName = reader.GetOrdinal("LastName");
        var address = reader.GetOrdinal("Address");
        var dateFrom = reader.GetOrdinal("DateFrom");
        var dateTo = reader.GetOrdinal("DateTo");
        var totalPrice = reader.GetOrdinal("TotalP");
        var vin = reader.GetOrdinal("Vin");
        var color = reader.GetOrdinal("ColorName");
        var model = reader.GetOrdinal("ModelName");


        


        AutoDTO autoDto = null;
        
        while (await reader.ReadAsync())
        {
            if (autoDto is not null)
            {
                autoDto.Cars.Add(new CarDTO()
                {
                    Vin = reader.GetString(vin),
                    Color = reader.GetString(color),
                    Model = reader.GetString(model),
                    DateFrom = reader.GetDateTime(dateFrom),
                    DateTo = reader.GetDateTime(dateTo),
                    TotalPrice = reader.GetInt32(totalPrice)
                });
            
            }
            else
            {
                autoDto = new AutoDTO()
                {
                    Id = reader.GetInt32(clientId),
                    FirstName = reader.GetString(firstName),
                    LastName = reader.GetString(lastName),
                    Address = reader.GetString(address),
                    
                    Cars = new List<CarDTO>()
                    {
                    new CarDTO()
                    { 
                        
                        Vin = reader.GetString(vin), 
                        Color = reader.GetString(color),
                        Model = reader.GetString(model),
                        DateFrom = reader.GetDateTime(dateFrom),
                        DateTo = reader.GetDateTime(dateTo),
                        TotalPrice = reader.GetInt32(totalPrice)
                }
                }
                };
            }

		   
        }
        
        if (autoDto is null) throw new Exception();

        return autoDto;
    }
    
    
    public async Task<int> AddClient(NewClientAuto client)
    {
        var insert = @"INSERT INTO Clients VALUES(@FirstName, @LastName, @Address); SELECT @@IDENTITY AS ID";
		  
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
		  
        command.Connection = connection;
        command.CommandText = insert;
		  
        command.Parameters.AddWithValue("@FirstName", client.FirstName);
        command.Parameters.AddWithValue("@LastName", client.LastName);
        command.Parameters.AddWithValue("@Address", client.Address);
        
        await connection.OpenAsync();

        var id = await command.ExecuteScalarAsync();

        if (id is null) throw new Exception();

        int value = Convert.ToInt32(id);
        
        return value;
		  
    }

    public async Task AddCarsToClient(int clientId, CarToClient carToClient)
    {
	    var insert = $"INSERT INTO Car_rentals VALUES(@ClientID,@CarID, @DateFrom, @DateTo)";

	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand();

	    command.Connection = connection;
	    command.CommandText = insert;
		  
	    command.Parameters.AddWithValue("@ClientID", clientId);
	    command.Parameters.AddWithValue("@CarId", carToClient.Id);
	    command.Parameters.AddWithValue("@DateFrom", carToClient.DateFrom);
	    command.Parameters.AddWithValue("@DateTo", carToClient.DateTo);

	    await connection.OpenAsync();

	    await command.ExecuteNonQueryAsync();
	    
    }

    
}