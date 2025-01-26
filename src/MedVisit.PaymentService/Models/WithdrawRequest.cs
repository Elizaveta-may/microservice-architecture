using System.ComponentModel.DataAnnotations;

namespace MedVisit.PaymentService.Models
{
    public class WithdrawRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "The amount must be greater than 0.")]
        public decimal Amount { get; set; }
    }

}
