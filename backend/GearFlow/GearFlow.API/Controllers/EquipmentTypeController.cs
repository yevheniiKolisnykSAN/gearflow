using GearFlow.Application.Commands.EquipmentType;
using GearFlow.Application.Queries.EquipmentType;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GearFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentTypeController : ControllerBase
{
    private readonly IMediator _mediator;

    public EquipmentTypeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllEquipmentTypesQuery());
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] CreateEquipmentTypeCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpPut]
    [Route("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEquipmentTypeCommand command)
    {
        var updatedCommand = command with { Id = id };
        var result = await _mediator.Send(updatedCommand);
        return Ok(result);
    }

    [HttpDelete]
    [Route("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new ArchiveEquipmentTypeCommand(id));
        if (!result) return NotFound(result);
        return Ok(result);
    }
}