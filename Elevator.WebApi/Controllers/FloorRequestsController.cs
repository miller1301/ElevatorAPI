using Microsoft.AspNetCore.Mvc;

namespace Elevator.WebApi.Controllers;

/// <summary>
/// Controlador para gestionar las solicitudes de piso del ascensor.
/// </summary>
[Route("elevator-request")]
[ApiController]
public class FloorRequestsController : ControllerBase
{
    private readonly IFloorRequestService _floorRequestService;

    /// <summary>
    /// Constructor para el controlador
    /// </summary>
    public FloorRequestsController(IFloorRequestService floorRequestService)
    {
        _floorRequestService = floorRequestService;
    }

    /// <summary>
    /// Solicitar un ascensor a un piso espec�fico.
    /// </summary>
    /// <param name="floorNumber">El n�mero de piso donde se solicita el ascensor.</param>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /floor-requests
    ///     Request Body: 5
    /// 
    /// </remarks>
    /// <response code="200">La solicitud fue exitosa.</response>
    /// <response code="400">Si el n�mero de piso no es un n�mero entero positivo.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestElevator([FromBody] int floorNumber)
    {
        if (floorNumber <= 0)
        {
            return BadRequest("N�mero de piso no v�lido. El n�mero de piso debe ser un n�mero entero positivo.");
        }

        await _floorRequestService.AddFloorRequestAsync(floorNumber);

        return Ok();
    }

    /// <summary>
    /// Obtenga la lista de solicitudes de pisos pendientes para el ascensor.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /floor-requests
    /// 
    /// </remarks>
    /// <response code="200">La lista de solicitudes de piso pendientes.</response>
    /// <response code="204">No se encontraron solicitudes pendientes.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetOutstandingRequests()
    {
        // Devolver la lista de solicitudes pendientes.
        return Ok(await _floorRequestService.GetOutstandingRequestsAsync());
    }

    /// <summary>
    /// Ir al siguiente piso donde el ascensor debe detenerse seg�n su posici�n actual y direcci�n de viaje.
    /// </summary>
    /// <param name="currentFloor">El piso actual del ascensor.</param>
    /// <param name="elevatorTravelDirection">La direcci�n de viaje actual del ascensor (opcional, el valor predeterminado es Estacionado).</param>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /floor-requests/next?currentFloor=10&amp;elevatorTravelDirection=Up
    /// 
    /// </remarks>
    /// <response code="200">El siguiente n�mero del piso donde el ascensor debe detenerse.</response>
    /// <response code="204">No se encontraron solicitudes coincidentes para la direcci�n indicada.</response>
    [HttpGet("next")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetNextFloorToStopAsync(
        int currentFloor, 
        ElevatorTravelDirection elevatorTravelDirection = ElevatorTravelDirection.Stationary)
    {
        if (currentFloor <= 0)
        {
            return BadRequest("N�mero de piso no v�lido. El n�mero de piso debe ser un n�mero entero positivo.");
        }

        int? nextFloor = await _floorRequestService.GetNextFloorToStopAsync(currentFloor, elevatorTravelDirection);

        if (nextFloor == null)
        {
            // No hay m�s solicitudes
            return NoContent();
        }

        return Ok(nextFloor);
    }


    /// <summary>
    /// Elimine una solicitud de piso cumplida de la lista de solicitudes pendientes.
    /// </summary>
    /// <param name="floorNumber">El ID de la solicitud de piso cumplida para eliminar.</param>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE /floor-requests/1
    /// 
    /// </remarks>
    /// <response code="200">La solicitud de uso de la palabra se elimin� con �xito.</response>
    /// <response code="404">Si no se encontr� la solicitud de piso con el ID especificado.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{floorNumber}")]
    public async Task<IActionResult> DeleteFulfilledRequest(int floorNumber)
    {
        bool isDeleted = await _floorRequestService.RemoveFloorRequestAsync(floorNumber);

        if (isDeleted)
        {
            return Ok();
        }

        return NotFound();
    }
}