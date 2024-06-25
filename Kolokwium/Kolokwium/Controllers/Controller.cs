using System.Transactions;
using Kolokwium.Models;
using Kolokwium.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium.Controllers;

[Route("api/clients")]
[ApiController]
public class Controller : ControllerBase
{
    private readonly ICarRepository _carRepository;
    
    public Controller(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }
    
    [HttpGet("{clientId}")]
    public async Task<IActionResult> GetCars(int clientId)
    {
        if (!await _carRepository.DoesClientExist(clientId))
        {
            return NotFound($"Client not found");
        }

        var book = await _carRepository.GetCar(clientId);

        return Ok(book);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddClient(NewClientAuto newClient)
    {
     
        foreach (var car in newClient.CarToClients)
        {
            if (!await _carRepository.DoesCarExist(car.Id))
                return NotFound($"Car with given ID - {car.Id} doesn't exist");
        }
        var id = 0;
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            id = await _carRepository.AddClient(new NewClientAuto()
            {
                FirstName = newClient.FirstName,
                LastName = newClient.LastName,
                Address = newClient.Address,
                
            });

          

            scope.Complete();
        }

        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
             foreach (var car in newClient.CarToClients)
             {
                 await _carRepository.AddCarsToClient(id, car);
             } 
             scope.Complete();

        }
        return Created("/api/clients", new
        {
            FirstName = newClient.FirstName,
            LastName = newClient.LastName,
            Address = newClient.Address,
            Cars = newClient.CarToClients

        });

    }
   
    
    

    
    
}