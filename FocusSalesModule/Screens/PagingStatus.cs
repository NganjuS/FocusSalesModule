using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Screens
{
    public class PagingStatus<T>
    {
        public int recordsTotal { get; set; } = 0;
        public int currentPage { get; set; } = 1;
        public int totalPages { get; set; } = 1;
        public List<T> data { get; set; }

    }
}