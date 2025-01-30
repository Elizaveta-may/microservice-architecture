using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.CatalogService.Mediatr.MedicalWorker.Commands
{
    public record DeleteMedicalWorkerCommand(int Id) : IRequest<Unit>;
    public class DeleteMedicalWorkerCommandHandler : IRequestHandler<DeleteMedicalWorkerCommand, Unit>
    {
        private readonly CatalogDbContext _context;
            
        public DeleteMedicalWorkerCommandHandler(CatalogDbContext context)
        {
            _context = context;
        }
        public async Task<Unit> Handle(DeleteMedicalWorkerCommand command, CancellationToken ct)
        {
            var targetEntity = await _context.MedicalWorkers.FirstOrDefaultAsync(x => x.Id == command.Id, ct)
                               ?? throw new Exception(nameof(MedicalWorker));

            _context.Remove(targetEntity);
            await _context.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
