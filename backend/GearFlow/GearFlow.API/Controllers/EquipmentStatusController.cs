using GearFlow.Application.Queries.EquipmentStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GearFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquipmentStatusController : ControllerBase
{
    private readonly IMediator _mediator;

    public EquipmentStatusController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllEquipmentStatusesQuery());
        return Ok(result);
    }
}