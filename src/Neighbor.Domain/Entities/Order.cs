using Neighbor.Contract.Enumarations.PaymentMethod;
using Neighbor.Domain.Entities;

namespace Neighbor.Domain.Entities
{
    public class Order
    {
        public Order()
        { }
        public DateTime RentTime { get; private set; }
        public DateTime ReturnTime { get; private set; }
        public string DeliveryAddress { get; private set; }
        public double OrderValue { get; private set; }
        public OrderStatusType OrderStatus { get; private set; }
        public long? OrderId { get; set; }
        public virtual List<Feedback> Feedbacks { get; private set; }
        public PaymentMethodType PaymentMethodId { get; private set; }
        public virtual PaymentMethod PaymentMethod { get; private set; }
        public Guid? ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public Guid? AccountId { get; set; }
        public virtual Account? Account { get; set; }

    }
}
