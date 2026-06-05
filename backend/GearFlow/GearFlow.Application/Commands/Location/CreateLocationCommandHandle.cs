using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Commands.Location;

public class CreateLocationCommandHandle : IRequestHandler<CreateLocationCommand, int>
{
    private readonly IRepository<Domain.Entities.Location> _repository;

    public CreateLocationCommandHandle(IRepository<Domain.Entities.Location> repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = new Domain.Entities.Location
        {
            Name = request.Name
        };

        await _repository.AddAsync(location);
        return location.Id;
    }
}