using ASPCORESimpleAppWithTests;
using ASPCORESimpleAppWithTests.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ASPCORESimpleTest
{
    public class OrderControllerTests
    {
        public OrderControllerTests()
        {
            ContextOptions = new DbContextOptionsBuilder<OrderContext>().UseSqlite("Filename=TestOrders.db").Options;
            FillDB();
        }
        protected DbContextOptions<OrderContext> ContextOptions { get; }
        private void FillDB()
        {
            Random rnd = new Random();
            using(var context = new OrderContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                Product addedProduct = context.Add(new Product()
                {
                    ProductName = "Chocolate",
                    Category = "Sweets",
                    AmountInStock = 50,
                    Price = 21.99,
                }).Entity;
                context.SaveChanges();
                context.Add(new Order()
                {
                    ClientName = "client1",
                    OrderTime = DateTime.Now,
                    Amount = rnd.Next(2, 10),
                    DeliveryAddress = "Wiœniowa",
                    ProductId = addedProduct.Id
                });
                context.Add(new Order()
                {
                    ClientName = "client2",
                    OrderTime = DateTime.Now,
                    Amount = rnd.Next(2, 10),
                    DeliveryAddress = "Brzoskwiniowa",
                    ProductId = addedProduct.Id
                });
                context.Add(new Order()
                {
                    ClientName = "client3",
                    OrderTime = DateTime.Now,
                    Amount = rnd.Next(2, 10),
                    DeliveryAddress = "Czekoladowa",
                    ProductId = addedProduct.Id
                });
                context.SaveChanges();
            }
        }
        [Fact]
        public void CanGetAllOrders()
        {
            using (var context = new OrderContext(ContextOptions))
            {
                var controller = new OrdersController(context);

                List<Order> orders = ((DbSet<Order>)controller.Get()).ToList();

                Assert.Equal(3, orders.Count);
                Assert.Equal("client1", orders[0].ClientName);
                Assert.Equal("client2", orders[1].ClientName);
                Assert.Equal("client3", orders[2].ClientName);
            }
        }
        [Fact]
        public void CanAddOrder()
        {
            Product addedProduct;
            using (var context = new OrderContext(ContextOptions))
            {
                addedProduct = context.Add(new Product()
                {
                    ProductName = "Lolipop",
                    Category = "Sweets",
                    AmountInStock = 27,
                    Price = 1.29,
                }).Entity;
                context.SaveChanges();
            }
            Order newOrder = new Order()
            {
                ClientName = "client4",
                OrderTime = DateTime.Now,
                Amount = 25,
                DeliveryAddress = "Morelowa",
                ProductId = addedProduct.Id
            };
            Order gottenOrder;
            OkObjectResult postResult;

            using (var context = new OrderContext(ContextOptions))
            {
                var controller = new OrdersController(context);
                postResult = controller.Post(newOrder) as OkObjectResult;

            }
            using (var context = new OrderContext(ContextOptions))
            {
                gottenOrder = context.Set<Order>().Single(e => e.ClientName.Equals("client4"));
            }

            Assert.NotNull(postResult);
            Assert.Equal(newOrder, postResult.Value);
            Assert.Equal(newOrder, gottenOrder);
        }
        [Theory]
        [InlineData(4, 8, 4)]
        [InlineData(12, 20, 8)]
        [InlineData(20, 30, 10)]
        [InlineData(-10, 25, 35)]
        public void CanDeleteOrderAndReturnProducts(int betweenValue, int amountInStock, int orderedAmount)
        {
            Product firstStateProduct = new Product()
            {
                ProductName = "Carrot",
                Category = "Vegetables",
                AmountInStock = amountInStock,
                Price = 1.99,
            };
            Order newOrder = new Order()
            {
                ClientName = "client4",
                OrderTime = DateTime.Now,
                Amount = orderedAmount,
                DeliveryAddress = "Morelowa"
            };
            Product betweenStateProduct = null;
            Product secondStateProduct = null;
            ActionResult orderPostResult = null;

            using (var context = new OrderContext(ContextOptions))
            {
                OrdersController ordersController = new OrdersController(context);
                ProductsController productsController = new ProductsController(context);
                newOrder.ProductId = ((Product)((OkObjectResult)productsController.Post(firstStateProduct)).Value).Id;
                orderPostResult = ordersController.Post(newOrder);
            }
            if (orderPostResult is OkObjectResult)
            {
                newOrder = ((OkObjectResult)orderPostResult).Value as Order;
                using (var context = new OrderContext(ContextOptions))
                {
                    ProductsController productsController = new ProductsController(context);
                    betweenStateProduct = productsController.Get(newOrder.ProductId);
                }
                using (var context = new OrderContext(ContextOptions))
                {
                    OrdersController ordersController = new OrdersController(context);
                    ProductsController productsController = new ProductsController(context);
                    ordersController.Delete(newOrder.Id);
                    secondStateProduct = productsController.Get(newOrder.ProductId);
                }
            }

            if (orderedAmount > amountInStock)
            {
                Assert.IsType<BadRequestResult>(orderPostResult);
            }
            else
            {
                Assert.NotNull(betweenStateProduct);
                Assert.NotNull(secondStateProduct);
                Assert.NotNull(orderPostResult);
                Assert.Equal(orderedAmount, newOrder.Amount);
                Assert.Equal(betweenValue, betweenStateProduct.AmountInStock);
                Assert.Equal(amountInStock, secondStateProduct.AmountInStock);
            }
        }
    }
}
