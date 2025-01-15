using System.Transactions;
using MediatR;
using MedVisit.PaymentService;
using MedVisit.PaymentService.Entities;
using MedVisit.PaymentService.Mediatr.Account.Queries;
using MedVisit.PaymentService.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MedVisit.PaymentService.Mediatr.Transaction.Queries
{
    public record GetTransactionHistoryQuery(int accoountId) : IRequest<IEnumerable<TransactionDto>>;
    public class GetTransactionHistoryQueryHandler : IRequestHandler<GetTransactionHistoryQuery, IEnumerable<TransactionDto>>
    {
        private readonly PaymentDbContext _context;
        public GetTransactionHistoryQueryHandler(PaymentDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<TransactionDto>> Handle(GetTransactionHistoryQuery query, CancellationToken ct)
        {
            var transactionHistory = await _context.Transactions
                .Where(t => t.AccountId == query.accoountId
                            && t.Status == (int)Enums.TransactionStatus.Completed)
                .Select(v => new TransactionDto
                {
                    Id = v.Id,
                    AccountId = v.AccountId,
                    Amount = v.Amount,
                    TransactionType = (Enums.TransactionType)v.TransactionType,
                    Status = (Enums.TransactionStatus)v.Status
                })
                .ToListAsync(ct);

            return transactionHistory ?? throw new KeyNotFoundException($"Account with ID {query.accoountId} not found.");
        }
    }
}
