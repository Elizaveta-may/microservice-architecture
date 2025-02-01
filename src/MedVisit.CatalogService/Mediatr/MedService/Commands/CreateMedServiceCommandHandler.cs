using AutoMapper;
using MediatR;
using MedVisit.CatalogService.Entities;
using MedVisit.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.CatalogService.Mediatr.MedService.Commands
{
    public record CreateMedServiceCommand(MedServiceDto dto) : IRequest<MedServiceDto>;

    public class CreateMedServiceCommandHandler : IRequestHandler<CreateMedServiceCommand, MedServiceDto>
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;

        public CreateMedServiceCommandHandler(CatalogDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MedServiceDto> Handle(CreateMedServiceCommand command, CancellationToken ct)
        {
           var medicalWorkers = await _context.MedicalWorkers
                .Where(worker => command.dto.MedicalWorkerIds.Contains(worker.Id))
                .ToListAsync(ct);
            var service = _mapper.Map<MedServiceDb>(command.dto);
            service.MedicalWorkers = medicalWorkers;
            var result = await _context.MedServices.AddAsync(service, ct);
            await _context.SaveChangesAsync(ct);

            return _mapper.Map<MedServiceDto>(result.Entity);
        }
    }
}
