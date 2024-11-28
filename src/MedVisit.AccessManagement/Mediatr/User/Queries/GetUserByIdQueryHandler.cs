using AutoMapper;
using MediatR;
using MedVisit.AccessManagement.Models.User;
using MedVisit.Common.AuthDbContext;
using MedVisit.Common.AuthDbContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.AccessManagement.Mediatr.User.Queries;

public record GetUserByIdQuery(int userId) : IRequest<UserDto>;
public class GetUserByIdQueryHandler(AuthDbContext context, IMapper mapper) : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery query, CancellationToken ct)
    {
        var UserDb = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == query.userId, ct) ?? throw new Exception(nameof(UserDto));

        return mapper.Map<UserDto>(UserDb);
    }
}
