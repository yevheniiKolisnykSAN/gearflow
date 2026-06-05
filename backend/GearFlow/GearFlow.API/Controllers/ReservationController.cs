using System.Security.Claims;
using GearFlow.Application.Commands.Reservation;
using GearFlow.Application.DTOs;
using GearFlow.Application.Queries.Reservations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GearFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReservationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _mediator.Send(new GetMyActiveReservationsQuery(userId));
        return Ok(result);
    }
    
    [HttpGet("active/{id}")]
    public async Task<IActionResult> GetActiveByEquipmentId(int id)
    {
        var result = await _mediator.Send(new GetActiveReservationByEquipmentIdQuery(id));
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByAll(int id)
    {
        var result = await _mediator.Send(new GetReservationByIdQuery(id));
        return Ok(result);
    }

    [HttpGet("history/{id}")]
    public async Task<IActionResult> GetHistoryByEquipmentId(int id)
    {
        var result = await _mediator.Send(new GetReservationHistoryByEquipmentIdQuery(id));
        return Ok(result);
    }

    [HttpPut("complete/{id}")]
    public async Task<IActionResult> CompleteReservation(int id)
    {
        var roleId = int.Parse(User.FindFirst(ClaimTypes.Role)!.Value);
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _mediator.Send(new CompleteReservationCommand(id, userId, roleId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationRequestDto request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _mediator.Send(new CreateReservationCommand(
            request.EquipmentId,
            request.StartDate,
            request.EndDate,
            userId
        ));
        return Ok(result);
    }
    
}