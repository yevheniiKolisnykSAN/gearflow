using GearFlow.Domain.Interfaces;
using MediatR;

namespace GearFlow.Application.Commands.Location;

public class ArchiveLocationCommandHandler : IRequestHandler<ArchiveLocationCommand, bool>
{
    private readonly IRepository<Domain.Entities.Location> _repository;

    public ArchiveLocationCommandHandler(IRepository<Domain.Entities.Location> repository)
    {
        _repository = repository;
    }


    public async Task<bool> Handle(ArchiveLocationCommand request, CancellationToken token)
    {
        var location = await _repository.GetByIdAsync(request.Id);
        if (location == null)
        {
            throw new KeyNotFoundException("Location not found");
        }

        if (location.Archived)
        {
            throw new InvalidOperationException("Location already deleted");
        }

        location.Archived = true;

        await _repository.UpdateAsync(location);
        return true;
    }
}