using GearFlow.Application.Commands.Equipment;
using GearFlow.Application.DTOs;
using GearFlow.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GearFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquipmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public EquipmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? name,
        [FromQuery] string? serialNumber,
        [FromQuery] string? specification,
        [FromQuery] int[]? maxLoanDays,
        [FromQuery] int[]? locationId,
        [FromQuery] int[]? statusId,
        [FromQuery] int[]? typeId)
    {
        var query = new GetAllEquipmentQuery(search, name, serialNumber, specification, maxLoanDays, locationId,
            statusId, typeId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var equipment = await _mediator.Send(new GetEquipmentByIdQuery(id));
        if (equipment == null) return NotFound();
        return Ok(equipment);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] CreateEquipmentRequestDto request)
    {
        var command = new CreateEquipmentCommand(
            $"{request.LocationId:D3}-{request.TypeId:D3}-{Random.Shared.Next(1000, 9999)}",
            request.Name,
            request.Specification,
            request.MaxLoanDays,
            request.StatusId,
            request.TypeId,
            request.LocationId
        );


        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpPut]
    [Route("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEquipmentRequestDto request)
    {
        var command = new UpdateEquipmentCommand(
            id,
            request.Name,
            request.LocationId,
            request.Specification,
            request.MaxLoanDays,
            request.StatusId,
            request.TypeId
        );

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete]
    [Route("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new ArchiveEquipmentCommand(id));
        if (!result) return NotFound(result);
        return Ok(result);
    }
}