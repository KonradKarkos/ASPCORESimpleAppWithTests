using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPCORESimpleAppWithTests.Models
{
    public class Product : IEquatable<Product>
    {
        public int Id;
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int AmountInStock { get; set; }
        public double? Price { get; set; }

        public List<Order> Orders { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Product);
        }

        public bool Equals(Product other)
        {
            return other != null &&
                   Id == other.Id &&
                   ProductName.Equals(other.ProductName) &&
                   Category.Equals(other.Category) &&
                   AmountInStock == other.AmountInStock &&
                   Price == other.Price;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, ProductName, Category, AmountInStock, Price);
        }

        public override string ToString()
        {
            return ProductName + " " + Category + " " + AmountInStock + " " + Price;
        }
    }
}
