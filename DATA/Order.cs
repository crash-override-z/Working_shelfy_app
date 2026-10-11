namespace Shelfy.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string TicketId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string PickupSlot { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public bool IsConfirmed { get; set; }

        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}