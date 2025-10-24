using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusSalesModule.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int BaseUnitId { get; set; }
        public string BaseUnit { get; set; }
        public string RmaNo { get; set; }
        public decimal Price { get; set; }

        public decimal Stock { get; set; }
    }
}
