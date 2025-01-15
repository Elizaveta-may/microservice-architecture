using MedVisit.PaymentService.Entities;
using MedVisit.PaymentService.Enums;
using MedVisit.PaymentService.Models;
using Microsoft.EntityFrameworkCore;

namespace MedVisit.PaymentService.Services
{
    public interface IPaymentService
    {
        Task<DepositResult> DepositAsync(int userId, decimal amount);
        Task<WithdrawResult> WithdrawAsync(int userId, decimal amount);
    }

    public class PaymentService : IPaymentService
    {
        private PaymentDbContext _dbContext;
        public PaymentService(PaymentDbContext context)
        {
            _dbContext = context;
        }

        public async Task<DepositResult> DepositAsync(int userId, decimal amount)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
            {
                return new DepositResult { Success = false, Message = "Пополнение счета прошло успешно." };
            }

            account.Balance += amount;

            var transaction = new TransactionDb
            {
                AccountId = account.Id,
                Amount = amount,
                TransactionType = (int)TransactionType.Deposit,
                Status = (int)TransactionStatus.Completed
            };

            _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();

            return new DepositResult { Success = true, Balance = account.Balance, Message = "Deposit successful." };
        }

        public async Task<WithdrawResult> WithdrawAsync(int userId, decimal amount)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null || !account.IsActive)
            {
                return new WithdrawResult { Success = false, Message = "Аккаунт не найден или неактивен." };
            }

            if (account.Balance < amount)
            {
                return new WithdrawResult { Success = false, Message = "Недостаточно средств." };
            }

            account.Balance -= amount;

            var transaction = new TransactionDb
            {
                AccountId = account.Id,
                Amount = amount,
                TransactionType = (int)TransactionType.Withdraw,
                Status = (int)TransactionStatus.Completed,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Transactions.Add(transaction);

            await _dbContext.SaveChangesAsync();

            return new WithdrawResult
            {
                Success = true,
                Balance = account.Balance,
                Message = "Оплата прошла."
            };
        }

    }
}
