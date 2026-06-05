using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Commands.Location;

public class UpdateLocationCommandHandle : IRequestHandler<UpdateLocationCommand, int>
{
    private readonly IRepository<Domain.Entities.Location> _repository;

    public UpdateLocationCommandHandle(IRepository<Domain.Entities.Location> repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _repository.GetByIdAsync(request.Id);
        if (location == null) return 0;

        location.Name = request.Name;
        await _repository.UpdateAsync(location);
        return location.Id;
    }
}