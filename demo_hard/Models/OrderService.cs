namespace demo_hard.Models;

public class OrderService
{
    public int OrderId { get; set; }
    public int ServiceId { get; set; }
    public int RentTime { get; set; }
    
    public virtual Order Order { get; set; }
    
    public virtual Service Service { get; set; }
}