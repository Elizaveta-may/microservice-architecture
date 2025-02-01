using AutoMapper;
using MediatR;
using MedVisit.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.CatalogService.Mediatr.MedService.Queries
{
    public record GetMedServiceByIdQuery(int serviceId) : IRequest<MedServiceDetailsDto>;
    public class GetMedServiceByIdQueryHandler(CatalogDbContext context, IMapper mapper) : IRequestHandler<GetMedServiceByIdQuery, MedServiceDetailsDto>
    {
        public async Task<MedServiceDetailsDto> Handle(GetMedServiceByIdQuery query, CancellationToken ct)
        {
            var MedService = await context.MedServices
                .Include(ms => ms.MedicalWorkers) 
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == query.serviceId, ct) ?? throw new Exception(nameof(MedServiceDetailsDto));


            return mapper.Map<MedServiceDetailsDto>(MedService);
        }
    }
}
