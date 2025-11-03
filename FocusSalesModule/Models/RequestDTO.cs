using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Models
{
    public class RequestDTO
    {
        public string DocNo { get; set; }
        public List<Outlet> Outlets { get; set; }
        public List<MasterDTO> CostCenters { get; set; }
    }
}