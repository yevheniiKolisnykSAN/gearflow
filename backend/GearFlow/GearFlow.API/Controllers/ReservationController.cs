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

    [HttpGet("history")]
    public async Task<IActionResult> GetHistoryByUserId()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _mediator.Send(new GetHistoryByUserIdQuery(userId));
        return Ok(result);
    }

    [HttpGet("history/equipment/{id}")]
    public async Task<IActionResult> GetHistoryByEquipmentId(int id)
    {
        var result = await _mediator.Send(new GetReservationHistoryByEquipmentIdQuery(id));
        return Ok(result);
    }

    [HttpPut("complete/{id}")]
    public async Task<IActionResult> CompleteReservation(int id, [FromBody] CompleteReservationRequestDto request)
    {
        var roleId = int.Parse(User.FindFirst(ClaimTypes.Role)!.Value);
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _mediator.Send(new CompleteReservationCommand(id, userId, roleId, request.DefectComment));
        return Ok(result);
    }

    [HttpPut("cancel/{id}")]
    public async Task<IActionResult> CancelReservation(int id)
    {
        var roleId = int.Parse(User.FindFirst(ClaimTypes.Role)!.Value);
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _mediator.Send(new CancelReservationCommand(id, userId, roleId));
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

    [HttpGet("dates/{id}")]
    public async Task<IActionResult> GetReservedDates(int id)
    {
        var result = await _mediator.Send(new GetReservedDatesQuery(id));
        return Ok(result);
    }

    [HttpGet("active-by-equipment/{id}")]
    public async Task<IActionResult> GetActiveReservationsListByEquipmentId(int id)
    {
        var result = await _mediator.Send(new GetActiveReservationsListByEquipmentIdQuery(id));
        return Ok(result);
    }
    
    [HttpGet("pending")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAllPendingReservation()
    {
        var result = await _mediator.Send(new GetAllPendingReservationsQuery());
        return Ok(result);
    }
    
    [HttpGet("pending/{id}")]
    public async Task<IActionResult> GetAllPendingReservationByUserId(int id)
    {
        var result = await _mediator.Send(new GetPendingReservationsByUserIdQuery(id));
        return Ok(result);
    }
    
    [HttpPut("confirm/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ConfirmReturn(int id, [FromBody] ConfirmReturnRequestDto request)
    {
        var result = await _mediator.Send(new ConfirmReturnCommand(id, request.DefectComment));
        return Ok(result);
    }
    
    [HttpGet("admin")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAdminReservations()
    {
        var result = await _mediator.Send(new GetAdminReservationsQuery());
        return Ok(result);
    }
}