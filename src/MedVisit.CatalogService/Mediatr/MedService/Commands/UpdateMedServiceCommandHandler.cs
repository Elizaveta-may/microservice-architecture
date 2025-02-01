using AutoMapper;
using MediatR;
using MedVisit.CatalogService.Entities;
using MedVisit.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.CatalogService.Mediatr.MedService.Commands
{

    public record UpdateMedServiceCommand(int Id, MedServiceDto MedService) : IRequest<MedServiceDto>;
    public class UpdateMedServiceCommandHandler : IRequestHandler<UpdateMedServiceCommand, MedServiceDto>
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;

        public UpdateMedServiceCommandHandler(CatalogDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MedServiceDto> Handle(UpdateMedServiceCommand command, CancellationToken ct)
        {
            var targetEntity = await _context.MedServices.FirstOrDefaultAsync(x => x.Id == command.Id, ct)
                               ?? throw new Exception(nameof(MedServiceDb));
            var medWorkers = await _context.MedicalWorkers
                .Where(worker => command.MedService.MedicalWorkerIds.Contains(worker.Id))
                .ToListAsync(ct);
            targetEntity.MedicalWorkers = medWorkers;

            _context.Update(targetEntity);
            _mapper.Map(command.MedService, targetEntity);

            await _context.SaveChangesAsync(ct);
            return _mapper.Map<MedServiceDto>(targetEntity);
        }
    }
}
