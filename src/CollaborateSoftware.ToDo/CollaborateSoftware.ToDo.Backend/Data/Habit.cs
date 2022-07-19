using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Data
{
    public class Habit : BaseEntry
    {   
        [Required]
        public string Title { get; set; }

        public Category Category { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public List<HabitDone> Done { get; set; }        
        
        public bool DoneToday()
        {
            if (Done.Count == 0) return false;
            if (Done.Any(d => d.Date.Date == DateTime.Now.Date)) return true;

            return false;
        }

        public bool DoneOnDay(DateTime date)
        {
            if (Done.Count == 0) return false;
            if (Done.Any(d => d.Date.Date == date.Date)) return true;

            return false;
        }
    }
}
