using AutoMapper;
using MediatR;
using MedVisit.Common.AuthDbContext;
using Microsoft.EntityFrameworkCore;
namespace MedVisit.AccessManagement.Mediatr.User.Handlers;
public record DeleteUserCommand(int Id) : IRequest<Unit>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly AuthDbContext _context;

    public DeleteUserCommandHandler(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteUserCommand command, CancellationToken ct)
    {
        var targetEntity = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.Id, ct)
            ?? throw new Exception(nameof(User));

        _context.Remove(targetEntity);
        await _context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}