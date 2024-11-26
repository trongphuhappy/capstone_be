using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Enumarations.PaymentMethod;
using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities
{
    public class Order : DomainEntity<Guid>
    {
        public Order()
        { }

        public Order(Guid accountId, Guid productId, DateTime rentTime, DateTime returnTime, string deliveryAddress, double orderValue, OrderStatusType orderStatus, long orderId, bool IsConflict,  PaymentMethodType paymentMethodId)
        {
            AccountId = accountId;
            ProductId = productId;
            RentTime = rentTime;
            ReturnTime = returnTime;
            DeliveryAddress = deliveryAddress;
            OrderValue = orderValue;
            OrderStatus = orderStatus;
            OrderId = orderId;
            PaymentMethodId = paymentMethodId;
        }
        public DateTime RentTime { get; private set; }
        public DateTime ReturnTime { get; private set; }
        public string DeliveryAddress { get; private set; }
        public double OrderValue { get; private set; }
        public OrderStatusType OrderStatus { get; private set; }
        public string? UserReasonReject {  get; private set; }
        public string? LessorReaonReject { get; private set; }
        public long? OrderId { get; private set; }
        public bool IsConflict { get; private set; }
        public virtual List<Feedback> Feedbacks { get; private set; }
        public PaymentMethodType PaymentMethodId { get; private set; }
        public virtual PaymentMethod PaymentMethod { get; private set; }
        public Guid? ProductId { get; private set; }
        public virtual Product? Product { get; private set; }
        public Guid? AccountId { get; private set; }
        public virtual Account? Account { get; private set; }

        public static Order CreateOrder(Guid accountId, Guid productId, DateTime rentTime, DateTime returnTime, string deliveryAddress, double productPrice, long orderId)
        {
            double orderValue = Math.Ceiling((returnTime - rentTime).TotalDays) * productPrice;
            OrderStatusType orderStatus = OrderStatusType.Pending;
            PaymentMethodType paymentMethod = PaymentMethodType.Banking;
            bool isConflict = false;
            return new Order(accountId, productId, rentTime, returnTime, deliveryAddress, orderValue, orderStatus, orderId, isConflict, paymentMethod);
        }
    }


}
