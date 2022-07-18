using System;
using System.ComponentModel.DataAnnotations;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Data
{
    public class BudgetEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public DateTime BudgetDate { get; set; }

        public Category Category { get; set; }
    }
}
