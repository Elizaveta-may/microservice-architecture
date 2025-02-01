using MediatR;
using MedVisit.PaymentService.Mediatr.Account.Commands;
using MedVisit.PaymentService.Mediatr.Account.Queries;
using MedVisit.PaymentService.Models;
using MedVisit.PaymentService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MedVisit.PaymentService.Controllers
{
    public class AccountRequest
    {
        public int UserId { get; set; }
    }
     
    [ApiController]
    [Route("payment")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPaymentService _paymentService;
        public AccountController(IMediator mediator, IPaymentService paymentService)
        {
            _mediator = mediator;
            _paymentService = paymentService;
        }

        [HttpPost("account")]
        public async Task<IActionResult> CreateAccount([FromBody] AccountRequest request)
        {
            var existingAccount = await _mediator.Send(new GetAccountByUserIdQuery(request.UserId));
            if (existingAccount != null)
            {
                return Conflict(new { message = "Учетная запись уже существует для данного пользователя." });
            }

            var createdAccountId = await _mediator.Send(new CreateAccountCommand(request.UserId));
            return CreatedAtAction(nameof(CreateAccount), new { id = createdAccountId }, createdAccountId);
        }

        [Authorize]
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst("user_id").Value;

            var result = await _paymentService.WithdrawAsync(int.Parse(userId), request.Amount);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new
            {
                message = result.Message,
                balance = result.Balance
            });
        }

        [Authorize]
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst("user_id").Value;

            var result = await _paymentService.DepositAsync(int.Parse(userId), request.Amount);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new
            {
                message = result.Message,
                balance = result.Balance
            });
        }

        [Authorize]
        [HttpGet("account")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetAccount()
        {

            var userId = User.FindFirst("user_id").Value;

            var profile = int.Parse(userId);
            return profile == null ? NotFound("Аккаунт не найден") : Ok(await _mediator.Send(new GetAccountByUserIdQuery(profile)));
        }
    }
}
