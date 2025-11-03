using System;
using System.ComponentModel.DataAnnotations;

namespace FocusSalesModule.Models
{
    public class Outlet
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
