using AutoMapper;
using MediatR;
using MedVisit.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.CatalogService.Mediatr.MedService.Queries
{
    public record GetMedicalWorkersForMedServiceQuery(int serviceId) : IRequest<IEnumerable<MedicalWorkerDto>>;
    public class GetMedicalWorkersForMedServiceQueryHandler(CatalogDbContext context, IMapper mapper) : IRequestHandler<GetMedicalWorkersForMedServiceQuery, IEnumerable<MedicalWorkerDto>>
    {
        public async Task<IEnumerable<MedicalWorkerDto>> Handle(GetMedicalWorkersForMedServiceQuery query, CancellationToken ct)
        {
            var workers = await context.MedServices
                .AsNoTracking().Include(ms => ms.MedicalWorkers)
                .Where(ms => ms.Id == query.serviceId)
                .SelectMany(ms => ms.MedicalWorkers) 
                .Select(worker => new MedicalWorkerDto
                {
                    Id = worker.Id,
                    FullName = worker.FullName,
                    Specialization = worker.Specialization,
                    OrganizationId = worker.OrganizationId
                })
                .ToListAsync(ct) ?? throw new Exception(nameof(MedicalWorkerDto));

            return mapper.Map<IEnumerable<MedicalWorkerDto>>(workers);
        }
    }
}