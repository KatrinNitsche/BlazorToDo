using System;
using System.ComponentModel.DataAnnotations;

namespace CollaborateSoftware.ToDo.Backend.Data
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
    }

    public enum Priority { Low, Middle, High }
}
