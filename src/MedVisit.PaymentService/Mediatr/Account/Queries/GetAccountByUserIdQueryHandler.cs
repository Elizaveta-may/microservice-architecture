using MediatR;
using MedVisit.PaymentService.Models;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.PaymentService.Mediatr.Account.Queries
{
    public record GetAccountByUserIdQuery(int id) : IRequest<AccountDto>;
    public class GetAccountByUserIdQueryHandler(PaymentDbContext context) : IRequestHandler<GetAccountByUserIdQuery, AccountDto>
    {
        public async Task<AccountDto> Handle(GetAccountByUserIdQuery query, CancellationToken ct)
        {
            var accountDb = await context.Accounts
                .AsNoTracking()
                .Where(v => v.UserId == query.id)
                .Select(v => new AccountDto
                {
                    Id = v.Id,
                    UserId = v.UserId,
                    Balance = v.Balance,
                    IsActive = v.IsActive
                })
                .FirstOrDefaultAsync(ct);

            return accountDb ?? null;
        }
    }
}