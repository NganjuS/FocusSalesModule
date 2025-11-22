using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace FocusSalesModule.Models
{
    public class Outlet
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = true;
        public int DefaultCostCenter { get; set; } = 0;
        public int DefaultCustomer { get; set; } = 0;
        public int DefaultSalesAccount { get; set; } = 0;
        public int DefaultCashAccount { get; set; } = 0;
        public int DefaultBankAccount { get; set; } = 0;
        public int DefaultOnlineAccount { get; set; } = 0;
        public int DefaultCreditNoteAccount { get; set; } = 0;
        public int DefaultDiscountAccount { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
