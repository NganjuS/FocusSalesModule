using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusSalesModule.Models
{
    public class PosLine
    {
        public int ItemId { get; set; }
        public string Code { get; set; }
        public string Item { get; set; }
        public string Unit { get; set; }
        public decimal AvailableStock { get; set; }
        public decimal SellingRate { get; set; }
        public decimal Gross
        {
            get
            {
                return AvailableStock * SellingRate;
            }
        }

    }
}
