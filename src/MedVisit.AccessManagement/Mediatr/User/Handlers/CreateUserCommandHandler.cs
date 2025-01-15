using AutoMapper;
using MediatR;
using MedVisit.AccessManagement.Models.User;
using MedVisit.Common.AuthDbContext.Entities;
using MedVisit.Common.AuthDbContext;
using MedVisit.Core.Service;

namespace MedVisit.AccessManagement.Mediatr.User.Handlers;

public record CreateUserCommand(UserDto User) : IRequest<UserDto>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly AuthDbContext _context;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(AuthDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(CreateUserCommand command, CancellationToken ct)
    {
        var userDb = _mapper.Map<UserDb>(command.User);
        var generatedPassword = PasswordService.GeneratePassword(12);
        var (hash, salt) = PasswordService.HashPassword(generatedPassword);
        userDb.PasswordHash = hash;
        userDb.PasswordSalt = salt;
        var result = await _context.Users.AddAsync(userDb, ct);
        await _context.SaveChangesAsync(ct);

        //TODO Send invitation email with first password
        return _mapper.Map<UserDto>(result.Entity);
    }
}