using System;
using System.ComponentModel.DataAnnotations;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Data
{
    public class ToDoListEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        public bool Done { get; set; }

        public Priority Priority { get; set; }

        public RepetitionType RepetitionType { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }
    }

    public enum Priority { Low, Middle, High }
    public enum RepetitionType { None, Daily, Weekly, TwoWeekly, Monthly, Quarterly, Halfyearly, Yearly }
}
