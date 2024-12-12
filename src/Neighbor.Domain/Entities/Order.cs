using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Enumarations.PaymentMethod;
using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities
{
    public class Order : DomainEntity<Guid>
    {
        public Order()
        { }
        public Order(Guid accountId, Guid productId, Guid lessorId, DateTime rentTime, DateTime returnTime, string deliveryAddress, double orderValue, OrderStatusType orderStatus, long orderId,  PaymentMethodType paymentMethodId)
        {
            Id = Guid.NewGuid();
            AccountId = accountId;
            ProductId = productId;
            LessorId = lessorId;
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
        public OrderReportStatusType OrderReportStatus { get; private set; } = OrderReportStatusType.NotConflict;
        public string? UserReasonReject {  get; private set; }
        public string? LessorReasonReject { get; private set; }
        public string? UserReport { get; private set; }
        public string? AdminReasonReject { get; private set; }
        public long? OrderId { get; private set; }
        public Guid LessorId { get; private set; }
        public virtual List<Feedback> Feedbacks { get; private set; }
        public PaymentMethodType PaymentMethodId { get; private set; }
        public virtual PaymentMethod PaymentMethod { get; private set; }
        public Guid? ProductId { get; private set; }
        public virtual Product? Product { get; private set; }
        public Guid? AccountId { get; private set; }
        public virtual Account? Account { get; private set; }

        public static Order CreateOrder(Guid accountId, Guid productId, Guid lessorId, DateTime rentTime, DateTime returnTime, string deliveryAddress, double productPrice, long orderId)
        {
            double orderValue = Math.Ceiling((returnTime - rentTime).TotalDays) * productPrice;
            OrderStatusType orderStatus = OrderStatusType.Pending;
            PaymentMethodType paymentMethod = PaymentMethodType.Banking;
            return new Order(accountId, productId, lessorId, rentTime, returnTime, deliveryAddress, orderValue, orderStatus, orderId, paymentMethod);
        }

        public void UpdateConfirmOrderByUser(OrderStatusType orderStatus, string? rejectReason)
        {
            OrderStatus = orderStatus;
            UserReasonReject = rejectReason;
        }

        public void UpdateConfirmOrderByLessor(OrderStatusType orderStatus, string? rejectReason)
        {
            OrderStatus = orderStatus;
            LessorReasonReject = rejectReason;
        }

        public void UpdateConfirmOrderByAdmin(OrderReportStatusType orderReportStatus, string? rejectReason)
        {
            OrderReportStatus = orderReportStatus;
            AdminReasonReject = rejectReason;
        }

        public void UpdateReportOrder(string userReport)
        {
            OrderReportStatus = OrderReportStatusType.PendingConflict;
            UserReport = userReport;
        }

        public void UpdateProductOrder(Product product)
        {
            Product = product;
        }

        public void UpdateUserOrder(Account account)
        {
            Account = account;
        }

        public static Order CreateOrderWithIdAndAccountAndOrderStatus(Guid orderId, Account account, OrderStatusType orderStatus)
        {
            return new Order()
            {
                Id = orderId,
                Account = account,
                OrderStatus = orderStatus
            };
        }

        public static Order CreateOrderWithIdAndAccountAndOrderStatusAndProduct(Guid orderId, Account account, OrderStatusType orderStatus, Product product)
        {
            return new Order()
            {
                Id = orderId,
                Account = account,
                OrderStatus = orderStatus,
                Product = product
            };
        }

        public static Order CreateOrderWithIdAndProductAndOrderStatus(Guid orderId, Product product, OrderStatusType orderStatus)
        {
            return new Order()
            {
                Id = orderId,
                Product = product,
                OrderStatus = orderStatus,
            };
        }

        public static Order CreateOrderWithOrderReportStatusType(OrderReportStatusType orderReportStatus)
        {
            return new Order()
            {
                OrderReportStatus = orderReportStatus
            };
        }

        public static Order CreateOrderWithIdAndAccountAndOrderReportStatusAndProduct(Guid orderId, Account account, OrderReportStatusType orderReportStatus, Product product)
        {
            return new Order()
            {
                Id = orderId,
                Account = account,
                OrderReportStatus = orderReportStatus,
                Product = product
            };
        }
    }


}
