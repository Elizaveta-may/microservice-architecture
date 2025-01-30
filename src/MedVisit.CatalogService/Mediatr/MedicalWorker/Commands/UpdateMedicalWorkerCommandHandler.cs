using AutoMapper;
using MediatR;
using MedVisit.CatalogService.Entities;
using MedVisit.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.CatalogService.Mediatr.MedicalWorker.Commands
{
    public record UpdateMedicalWorkerCommand(int Id, MedicalWorkerDto MedicalWorker) : IRequest<MedicalWorkerDto>;
    public class UpdateMedicalWorkerCommandHandler : IRequestHandler<UpdateMedicalWorkerCommand, MedicalWorkerDto>
    {
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;

        public UpdateMedicalWorkerCommandHandler(CatalogDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MedicalWorkerDto> Handle(UpdateMedicalWorkerCommand command, CancellationToken ct)
        {
            var targetEntity = await _context.MedicalWorkers.FirstOrDefaultAsync(x => x.Id == command.Id, ct)
                               ?? throw new Exception(nameof(MedicalWorkerDb));

            _context.Update(targetEntity);
            _mapper.Map(command.MedicalWorker, targetEntity);

            await _context.SaveChangesAsync(ct);
            return _mapper.Map<MedicalWorkerDto>(targetEntity);
        }
    }
}