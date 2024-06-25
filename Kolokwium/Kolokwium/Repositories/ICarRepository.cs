using Kolokwium.Models;

namespace Kolokwium.Repositories;

public interface ICarRepository
{
    public Task<bool> DoesClientExist(int id);
    
    public Task<bool> DoesCarExist(int id);

    public Task<AutoDTO> GetCar(int id);

    public Task<int> AddClient(NewClientAuto client);
    public Task AddCarsToClient(int clientId, CarToClient carToClient);

    
    
}