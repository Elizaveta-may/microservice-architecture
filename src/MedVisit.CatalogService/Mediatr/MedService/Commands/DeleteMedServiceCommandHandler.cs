using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.CatalogService.Mediatr.MedService.Commands
{

    public record DeleteMedServiceCommand(int Id) : IRequest<Unit>;

    public class DeleteMedServiceCommandHandler : IRequestHandler<DeleteMedServiceCommand, Unit>
    {
        private readonly CatalogDbContext _context;

        public DeleteMedServiceCommandHandler(CatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteMedServiceCommand command, CancellationToken ct)
        {
            var targetEntity = await _context.MedServices.FirstOrDefaultAsync(x => x.Id == command.Id, ct)
                               ?? throw new Exception(nameof(MedService));

            _context.Remove(targetEntity);
            await _context.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
