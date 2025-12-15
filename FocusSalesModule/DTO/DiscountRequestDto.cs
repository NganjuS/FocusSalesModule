using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.DTO
{
    public class DiscountRequestDto
    {
        public int TxnDate { get; set; }
        public List<int> Item {  get; set; }
    }
}