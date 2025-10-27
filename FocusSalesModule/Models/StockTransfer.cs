using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FocusSalesModule.Models
{
    public class StockTransfer
    {
        public int Id { get; set; }

        public string StockIssueNo { get; set; }

        [Required]
        public int FromOutletId { get; set; }

        public string FromOutletName { get; set; }

        [Required]
        public int ToOutletId { get; set; }

        public string ToOutletName { get; set; }

        public string Narration { get; set; }

        public DateTime TransferDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Issued"; // Issued, Received

        public bool IsReceived { get; set; } = false;

        public DateTime? ReceivedDate { get; set; }

        public List<StockTransferItem> Items { get; set; } = new List<StockTransferItem>();
    }
}
