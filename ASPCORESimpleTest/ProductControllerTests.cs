using ASPCORESimpleAppWithTests;
using ASPCORESimpleAppWithTests.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ASPCORESimpleTest
{
    public class ProductControllerTests
    {
        public ProductControllerTests()
        {
            ContextOptions = new DbContextOptionsBuilder<OrderContext>().UseSqlite("Filename=TestProducts.db").Options;
            FillDB();
        }
        protected DbContextOptions<OrderContext> ContextOptions { get; }
        private void FillDB()
        {
            Random rnd = new Random();
            using (var context = new OrderContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                context.Add(new Product()
                {
                    ProductName = "Apple",
                    Category = "Fruits",
                    AmountInStock = rnd.Next(2, 10),
                    Price = 5.25,
                });
                context.Add(new Product()
                {
                    ProductName = "Orange",
                    Category = "Fruits",
                    AmountInStock = rnd.Next(2, 20),
                    Price = 3.45,
                });
                context.Add(new Product()
                {
                    ProductName = "Doll",
                    Category = "Toys",
                    AmountInStock = rnd.Next(1, 7),
                    Price = 11.99,
                });
                context.SaveChanges();
            }
        }
        [Fact]
        public void CanGetAllProducts()
        {
            using (var context = new OrderContext(ContextOptions))
            {
                var controller = new ProductsController(context);

                List<Product> products = ((DbSet<Product>)controller.Get()).ToList();

                Assert.Equal(3, products.Count);
                Assert.True(products.FirstOrDefault(p => p.ProductName.Equals("Apple")) != null);
                Assert.True(products.FirstOrDefault(p => p.ProductName.Equals("Orange")) != null);
                Assert.True(products.FirstOrDefault(p => p.ProductName.Equals("Doll")) != null);
            }
        }
        [Fact]
        public void CanAddProduct()
        {
            Product newProduct = new Product()
            {
                ProductName = "Carrot",
                Category = "Vegetables",
                AmountInStock = 8,
                Price = 1.99,
            };
            OkObjectResult postedProductResult;
            Product gottenProduct;


            using (var context = new OrderContext(ContextOptions))
            {
                var controller = new ProductsController(context);
                postedProductResult = controller.Post(newProduct) as OkObjectResult;

            }
            using (var context = new OrderContext(ContextOptions))
            {
                gottenProduct = context.Set<Product>().Single(e => e.ProductName.Equals("Carrot"));
            }

            Assert.NotNull(postedProductResult);
            Assert.Equal(newProduct, postedProductResult.Value);
            Assert.Equal(newProduct, gottenProduct);
        }
        [Fact]
        public void CanDeleteProduct()
        {
            List<Product> allProducts;
            Product toDelete;
            OkObjectResult deleteResult;


            using (var context = new OrderContext(ContextOptions))
            {
                var controller = new ProductsController(context);
                allProducts = controller.Get().ToList();
                toDelete = controller.Get(allProducts[0].Id);
                deleteResult = controller.Delete(toDelete.Id) as OkObjectResult;
                allProducts = controller.Get().ToList();
            }


            Assert.NotNull(deleteResult);
            Assert.Equal(deleteResult.Value, toDelete);
            Assert.DoesNotContain(toDelete, allProducts);
        }
    }
}
