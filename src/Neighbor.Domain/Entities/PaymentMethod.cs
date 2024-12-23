﻿using Neighbor.Contract.Enumarations.PaymentMethod;

namespace Neighbor.Domain.Entities
{
    public class PaymentMethod
    {
        public PaymentMethod()
        { }
        public PaymentMethodType Id { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public virtual ICollection<Order> Orders { get; set; }
    }
}
