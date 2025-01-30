using AutoMapper;
using MediatR;
using MedVisit.CatalogService.Entities;
using MedVisit.CatalogService.Models;

namespace MedVisit.CatalogService.Mediatr.MedicalWorker.Commands
{
    public record CreateMedicalWorkerCommand(MedicalWorkerDto dto) : IRequest<MedicalWorkerDto>;

    public class CreateMedicalWorkerCommandHandler : IRequestHandler<CreateMedicalWorkerCommand, MedicalWorkerDto>
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;

        public CreateMedicalWorkerCommandHandler(CatalogDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MedicalWorkerDto> Handle(CreateMedicalWorkerCommand command, CancellationToken ct)
        {
            var worker = _mapper.Map<MedicalWorkerDb>(command.dto);
            var result = await _context.MedicalWorkers.AddAsync(worker, ct);
            await _context.SaveChangesAsync(ct);

            return _mapper.Map<MedicalWorkerDto>(result.Entity);
        }
    }
}