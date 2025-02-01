using AutoMapper;
using MediatR;
using MedVisit.CatalogService.Entities;
using MedVisit.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.CatalogService.Mediatr.Organization.Commands
{
    public record UpdateOrganizationCommand(int ownerId, OrganizationDto Organization) : IRequest<OrganizationDto>;

    public class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand, OrganizationDto>
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;

        public UpdateOrganizationCommandHandler(CatalogDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrganizationDto> Handle(UpdateOrganizationCommand command, CancellationToken ct)
        {
            var targetEntity = await _context.Organizations.FirstOrDefaultAsync(x => x.OwnerId == command.ownerId, ct)
                               ?? throw new Exception(nameof(OrganizationDb));

            _context.Update(targetEntity);
            _mapper.Map(command.Organization, targetEntity);
            targetEntity.OwnerId = command.ownerId;

            await _context.SaveChangesAsync(ct);
            return _mapper.Map<OrganizationDto>(targetEntity);
        }
    }
}
