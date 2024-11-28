using AutoMapper;
using MediatR;
using MedVisit.AccessManagement.Models.User;
using MedVisit.Common.AuthDbContext;
using Microsoft.EntityFrameworkCore;
namespace MedVisit.AccessManagement.Mediatr.User.Queries;

public record GetUsersQuery : IRequest<IEnumerable<UserDto>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
{
    private readonly AuthDbContext _context;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(AuthDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetUsersQuery query, CancellationToken ct)
    {
        var users = await _context.Users.ToListAsync(ct);
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }
}