using AutoMapper;
using MediatR;
using MedVisit.CatalogService.Entities;
using MedVisit.CatalogService.Models;
using MedVisit.Core.Service;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.CatalogService.Mediatr.Organization.Commands
{

    public record CreateOrganizationCommand(int ownerId, OrganizationDto Organization) : IRequest<OrganizationDto>;

    public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, OrganizationDto>
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;

        public CreateOrganizationCommandHandler(CatalogDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrganizationDto> Handle(CreateOrganizationCommand command, CancellationToken ct)
        {
            var orgExist = await _context.Organizations.AnyAsync(x => x.OwnerId == command.ownerId);
            if (orgExist)
            {
                throw new InvalidOperationException("Organization with the same OwnerId already exists.");
            }
            var organizationDb = _mapper.Map<OrganizationDb>(command.Organization);
            organizationDb.OwnerId = command.ownerId;
            var result = await _context.Organizations.AddAsync(organizationDb, ct);
            await _context.SaveChangesAsync(ct);

            return _mapper.Map<OrganizationDto>(result.Entity);
        }
    }

}
