using System;
using System.ComponentModel.DataAnnotations;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Data
{
    public class BaseEntry
    {
        [Key]
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid UserId { get; set; }
    }
}
