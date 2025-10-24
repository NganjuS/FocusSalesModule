using System;
using System.ComponentModel.DataAnnotations;

namespace FocusSalesModule.Models
{
    public class DiscountVoucher
    {
        public int Id { get; set; }

        [Required]
        public string ItemCode { get; set; }

        [Required]
        public string DiscountCode { get; set; }

        [Required]
        public int MaxUsageCount { get; set; }

        public int CurrentUsageCount { get; set; } = 0;

        [Required]
        public decimal VoucherValue { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Validation helpers
        public bool IsValid()
        {
            var now = DateTime.Now;
            return IsActive
                && now >= StartDate
                && now <= ExpiryDate
                && CurrentUsageCount < MaxUsageCount;
        }

        public bool CanBeUsed()
        {
            return IsValid();
        }

        public string GetStatusMessage()
        {
            if (!IsActive)
                return "Voucher is inactive";

            var now = DateTime.Now;
            if (now < StartDate)
                return "Voucher is not yet active";

            if (now > ExpiryDate)
                return "Voucher has expired";

            if (CurrentUsageCount >= MaxUsageCount)
                return "Voucher usage limit reached";

            return "Valid";
        }
    }
}
