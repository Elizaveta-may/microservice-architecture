using AutoMapper;
using MediatR;
using MedVisit.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.CatalogService.Mediatr.MedicalWorker.Queries
{
    public record GetMedicalWorkersInOrganizationQuery(int organizationId) : IRequest<IEnumerable<MedicalWorkerDto>>;

    public class GetMedicalWorkersInOrganizationQueryHandler(CatalogDbContext context, IMapper mapper)
        : IRequestHandler<GetMedicalWorkersInOrganizationQuery, IEnumerable<MedicalWorkerDto>>
    {
        public async Task<IEnumerable<MedicalWorkerDto>> Handle(GetMedicalWorkersInOrganizationQuery query, CancellationToken ct)
        {

            var workers = await context.MedicalWorkers
                .Where(w=> w.OrganizationId == query.organizationId)
                .ToListAsync(ct);
            return mapper.Map<IEnumerable<MedicalWorkerDto>>(workers);

        }
    }
}