using System;
using System.ComponentModel.DataAnnotations;

namespace FocusSalesModule.Models
{
    public class Outlet
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        public string Location { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
