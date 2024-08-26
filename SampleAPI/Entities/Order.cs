using System.ComponentModel.DataAnnotations;

namespace SampleAPI.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        [Required]
        public DateTime EntryDate { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "Description cannot exceed 100 characters.")]
        public string? Description { get; set; }
        public bool IsInvoiced { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}
