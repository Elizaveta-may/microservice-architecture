using AutoMapper;
using MediatR;
using MedVisit.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.CatalogService.Mediatr.Organization.Queries
{
    public record GetOwnerOrganizationQuery(int ownerId) : IRequest<OrganizationDto>;
    public class GetOrganizationQueryHandler(CatalogDbContext context, IMapper mapper) : IRequestHandler<GetOwnerOrganizationQuery, OrganizationDto>
    {
        public async Task<OrganizationDto> Handle(GetOwnerOrganizationQuery query, CancellationToken ct)
        {
            var OrganizationDb = await context.Organizations
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.OwnerId == query.ownerId, ct) ?? throw new Exception(nameof(OrganizationDto));

            return mapper.Map<OrganizationDto>(OrganizationDb);
        }
    }

}
