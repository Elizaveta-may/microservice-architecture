using AutoMapper;
using MediatR;
using MedVisit.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.CatalogService.Mediatr.MedService.Queries
{
    public record GetMedServicesInOrganizationQuery(int organizationId) : IRequest<IEnumerable<MedServiceDetailsDto>>;

    public class GetMedServicesInOrganizationQueryHandler(CatalogDbContext context, IMapper mapper)
        : IRequestHandler<GetMedServicesInOrganizationQuery, IEnumerable<MedServiceDetailsDto>>
    {
        public async Task<IEnumerable<MedServiceDetailsDto>> Handle(GetMedServicesInOrganizationQuery query, CancellationToken ct)
        {
            var services = await context.MedServices
                .Include(ms => ms.MedicalWorkers)
                .Where(w => w.OrganizationId == query.organizationId)
                .ToListAsync(ct);
            return mapper.Map<IEnumerable<MedServiceDetailsDto>>(services);

        }
    }
}
