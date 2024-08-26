using System.ComponentModel.DataAnnotations;

namespace SampleAPI.Requests
{
    public class CreateOrderResponse
    {
        public Guid Id { get; set; }
        public DateTime EntryDate { get; set; }
        public string Name { get; set; }

        public string? Description { get; set; }
        public bool IsInvoiced { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}
