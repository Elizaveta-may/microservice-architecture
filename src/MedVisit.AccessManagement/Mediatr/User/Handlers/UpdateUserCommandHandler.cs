using AutoMapper;
using MediatR;
using MedVisit.AccessManagement.Models.User;
using MedVisit.Common.AuthDbContext;
using MedVisit.Common.AuthDbContext.Entities;
using Microsoft.EntityFrameworkCore;
namespace MedVisit.AccessManagement.Mediatr.User.Handlers;
public record UpdateUserCommand(int Id, UserDto User) : IRequest<UserDto>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly AuthDbContext _context;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(AuthDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(UpdateUserCommand command, CancellationToken ct)
    {
        var targetEntity = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.Id, ct)
            ?? throw new Exception(nameof(UserDb));

        _context.Update(targetEntity);
        _mapper.Map(command.User, targetEntity); 

        await _context.SaveChangesAsync(ct);
        return _mapper.Map<UserDto>(targetEntity);
    }
}