using MediatR;
using MedVisit.PaymentService.Entities;

namespace MedVisit.PaymentService.Mediatr.Account.Commands
{
    public record CreateAccountCommand(int userId) : IRequest<int>;
    public class CreateAccountCommandHandler: IRequestHandler<CreateAccountCommand, int>
    {
        private readonly PaymentDbContext _context;

        public CreateAccountCommandHandler(PaymentDbContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(CreateAccountCommand command, CancellationToken ct)
        {
            var accountDb = new AccountDb
            {
                UserId = command.userId,
                Balance = 0
            };

            await _context.Accounts.AddAsync(accountDb, ct);
            await _context.SaveChangesAsync(ct);

            return accountDb.Id;
        }
    }
}
