using System;
using System.ComponentModel.DataAnnotations;

namespace FocusSalesModule.Models
{
    public class StockTransferItem
    {
        public int Id { get; set; }

        public int StockTransferId { get; set; }

        [Required]
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public string Unit { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        public string RmaNo { get; set; }
    }
}
