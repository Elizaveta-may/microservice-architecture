using AutoMapper;
using MediatR;
using MedVisit.CatalogService;
using MedVisit.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.CatalogService.Mediatr.MedicalWorker.Queries
{
    public record GetMedicalWorkerByIdQuery(int userId) : IRequest<MedicalWorkerDto>;
    public class GetMedicalWorkerByIdQueryHandler(CatalogDbContext context, IMapper mapper) : IRequestHandler<GetMedicalWorkerByIdQuery, MedicalWorkerDto>
    {
        public async Task<MedicalWorkerDto> Handle(GetMedicalWorkerByIdQuery query, CancellationToken ct)
        {
            var UserDb = await context.MedicalWorkers
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == query.userId, ct) ?? throw new Exception(nameof(MedicalWorkerDto));

            return mapper.Map<MedicalWorkerDto>(UserDb);
        }
    }
}
