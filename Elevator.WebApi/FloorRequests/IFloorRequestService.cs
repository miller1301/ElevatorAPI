namespace Elevator.WebApi.Controllers;

/// <summary>
/// Interfaz IFloorRequestService para gestionar solicitudes de pisos de ascensores.
/// </summary>
public interface IFloorRequestService
{
    /// <summary>
    /// Agrega una solicitud de uso de la palabra a la lista de solicitudes pendientes.
    /// </summary>
    Task AddFloorRequestAsync(int floorNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene la lista de solicitudes de piso pendientes.
    /// </summary>
    Task<List<int>> GetOutstandingRequestsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el siguiente piso en el que detenerse, según el piso actual y la dirección de viaje.
    /// </summary>
    Task<int?> GetNextFloorToStopAsync( int currentFloor, ElevatorTravelDirection currentElevatorTravelDirection, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una solicitud de piso de la lista de solicitudes pendientes.
    /// </summary>
    Task<bool> RemoveFloorRequestAsync(int floorNumber, CancellationToken cancellationToken = default);
}