using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Data
{
    public class NotesEntry : BaseEntry
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string Text { get; set; }

        public Category Category { get; set; }

        public int? ParentNoteId { get; set; }
    }
}
