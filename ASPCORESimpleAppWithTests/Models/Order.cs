using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPCORESimpleAppWithTests.Models
{
    public class Order : IEquatable<Order>
    {
        public int Id;
        public string ClientName { get; set; }
        public DateTime OrderTime { get; set; }
        public int Amount { get; set; }
        public string DeliveryAddress { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Order);
        }

        public bool Equals(Order other)
        {
            return other != null &&
                   Id == other.Id &&
                   ClientName == other.ClientName &&
                   OrderTime == other.OrderTime &&
                   Amount == other.Amount &&
                   DeliveryAddress == other.DeliveryAddress &&
                   ProductId == other.ProductId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, ClientName, OrderTime, Amount, DeliveryAddress, ProductId);
        }

        public override string ToString()
        {
            return ClientName + " " + OrderTime + " "  + Amount + " " + DeliveryAddress;
        }
    }
}
