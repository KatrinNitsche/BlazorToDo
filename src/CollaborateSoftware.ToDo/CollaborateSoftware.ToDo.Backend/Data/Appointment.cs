using System;
using System.ComponentModel.DataAnnotations;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Data
{
    public class Appointment : BaseEntry
    {  
        [Required, MaxLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public TimeSpan Duration { get; set; }

        public bool EntireDay { get; set; }

        public Category Category { get; set; }

        public Priority Priority { get; set; }
    }
}
