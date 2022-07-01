using System.ComponentModel.DataAnnotations;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Data
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public string Color { get; set; }

        public string Icon { get; set; }
    }
}
