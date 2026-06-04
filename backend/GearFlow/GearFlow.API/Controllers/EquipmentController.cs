using GearFlow.Application.Commands.Equipment;
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
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllEquipmentQuery());
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
    public async Task<IActionResult> Create([FromBody] CreateEquipmentCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEquipmentCommand command)
    {
        var updatedCommand = command with { Id = id };
        var result = await _mediator.Send(updatedCommand);
        return Ok(result);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteEquipmentCommand(id));
        if (!result) return NotFound(result);
        return Ok(result);
    }
}