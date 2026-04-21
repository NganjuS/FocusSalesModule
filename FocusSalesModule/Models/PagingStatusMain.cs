using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Models
{
    public class PagingStatusMain
    {
        public int recordsTotal { get; set; }
        public int currentPage { get; set; }
        public int totalPages { get; set; }
        public bool hasMore { get; set; }
    }
}